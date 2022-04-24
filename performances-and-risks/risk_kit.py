import pandas as pd
import numpy as np
import math
from datetime import datetime

def retrieve_data(df_data, Isin : list = ["US4642875078", "US4642875318", "US4642875235","US4642874402", "US4642871929", "US4642872000", "US4642864007"]):
    """
    Description :
        Récupère dans un dico de pd.DataFrame les datas des Isins qui nous intéressent
    Input :
        le dataset du fichier Prices.txt
        liste d'Isin : par défaut, la liste est celle du bench
    Output :
        un dico de dictionnaire des datas triés dans l'ordre de dates
    """
    
    dict_isin={}
    for isin in Isin: 
        tab = df_data[df_data["ISIN"] == isin]
        dict_isin[isin] = tab.sort_values(by="Date").set_index("Date")
    
    return dict_isin

def retrieve_Nav(dict_isin, drop_na : bool = True):
    """
    Description :
        Ajoute des lignes de dates n'étant pas présent dans dict_isin
        Chaque nouvelle ligne de date correspond à la data de la date précédente
    Input :
    
    Output:
    
    """
    nav = pd.DataFrame()
    for isin in dict_isin:
        nav = pd.concat([nav,dict_isin[isin]["NAV"] + dict_isin[isin]["Ex-Dividends"]],axis = 1,sort = False)
    
    nav.columns = dict_isin.keys()
    
    if drop_na :
        return nav.dropna().sort_index()
    else :
        return nav.sort_index()

    
def fill_Nav(nav, period : str = "BM"):
    """
    function permet de remplir les datas ne possédant pas de données à des dates précises par les données de la veille
    """
    dates = pd.date_range(nav.index[0],nav.index[-1],freq=period)
    list_index = list(nav.index)
    for e in dates:
        if (e.date().strftime("%Y-%m-%d") not in list_index) or len(nav.loc[e.date().strftime("%Y-%m-%d")].dropna())<15:
            #print(e)
            tmp = [t>=e.date().strftime("%Y-%m-%d") for t in list_index]
            idx = tmp.index(True)
            nav.loc[e.date().strftime("%Y-%m-%d")] = nav.iloc[idx-1]
    
    return nav.sort_index()

def fill_Nav_bench(nav, period : str = "BM"):
    """
    function permet de remplir les datas ne possédant pas de données à des dates précises par les données de la veille
    """
    dates = pd.date_range(nav.index[0],nav.index[-1],freq=period)
    list_index = list(nav.index)
    for e in dates:
        if (e.date().strftime("%Y-%m-%d") not in list_index):
            tmp = [t>=e.date().strftime("%Y-%m-%d") for t in list_index]
            idx = tmp.index(True)
            nav.loc[e.date().strftime("%Y-%m-%d")] = nav.iloc[idx-1]
    
    return nav.sort_index()
    
def ret(prices):
    l_ret=np.log(prices/prices.shift(1))
    l_ret.dropna(inplace=True)
    return l_ret

def benchmark_return(prices, weights):
    """
    retreive a pd.Series representing the return of the bench 
    """
    dates = pd.date_range(prices.index[0],prices.index[-1],freq="BM")
    #31/10/2003, 30/11/2003etc
    
    bench_ret = pd.Series()
    
    #pour les dates sans data, on prend la date précédente
    prices = fill_Nav_bench(prices)
    
    #initialisation
    #bench_ret[prices.index[0]] = 0;
    
    #e_ex = prices.index[0]
    e_ex = dates[0].date().strftime("%Y-%m-%d")
    for e in dates:
        ret = prices.loc[e.date().strftime("%Y-%m-%d")]*0.999/(prices.loc[e_ex]*1.001) - 1
        
        #produit matricielle pour trouver le return du portefeuille
        bench_ret[e] = weights.T@ret
        
        e_ex = e.date().strftime("%Y-%m-%d")
    
    #pour le 19/02/2021
    ret = prices.iloc[-1]/prices.loc[e_ex] - 1
    bench_ret[e] = weights.T@ret
    
    return bench_ret
                     
def skewness(r):
    """
    Description : 
        Alternative to scipy.stats.skew()
        Computes the skewness of the supplied Series or DataFrame
        Returns a float or a Series
    Input :
        pd.Series or pd.DataFrame of the returns
    Output :
        skewness of r
    """
    demeaned_r = r - r.mean()
    # use the population standard deviation, so set dof=0
    sigma_r = r.std(ddof=0)
    exp = (demeaned_r**3).mean()
    return exp/sigma_r**3


def kurtosis(r):
    """
    Description :
        Alternative to scipy.stats.kurtosis()
        Computes the kurtosis of the supplied Series or DataFrame
        Returns a float or a Series
    
    Input :
        pd.Series or pd.DataFrame of the returns
    Output :
        skewness of r
        
    """
    demeaned_r = r - r.mean()
    # use the population standard deviation, so set dof=0
    sigma_r = r.std(ddof=0)
    exp = (demeaned_r**4).mean()
    return exp/sigma_r**4


def compound(r):
    """
    returns the result of compounding the set of returns in r
    """
    return np.expm1(np.log1p(r).sum())

                         
def annualize_rets(r, periods_per_year):
    """
    Decription :
        Annualizes a set of returns
        We should infer the periods per year
    input : return r and periods_per_year (ex : 12 for montly returns, 252 for daily returns)
    
    """
    #begin_date = datetime.strptime(r.index[0], "%Y-%m-%d")
    #final_date = datetime.strptime(r.index[-1], "%Y-%m-%d")
    #n_periods = (final_date-begin_date).days
    compounded_growth = (1+r).prod()
    n_periods = r.shape[0]
    return compounded_growth**(periods_per_year/n_periods)-1

def annualize_vol(r, periods_per_year):
    """
    Description :
        Annualizes the vol of a set of returns
        We should infer the periods per year
    input : return r and periods_per_year (ex : 12 for montly returns, 252 for daily returns)
    
    """
    return r.std()*(periods_per_year**0.5)


def sharpe_ratio(r, riskfree_rate, periods_per_year):
    """
    Description :
        Computes the annualized sharpe ratio of a set of returns
    Input :
        return r and periods_per_year (ex : 12 for montly returns, 252 for daily returns)
    """
    # convert the annual riskfree rate to per period
    rf_per_period = (1+riskfree_rate)**(1/periods_per_year)-1
    excess_ret = r - rf_per_period
    ann_ex_ret = annualize_rets(excess_ret, periods_per_year)
    ann_vol = annualize_vol(r, periods_per_year)
    return ann_ex_ret/ann_vol


import scipy.stats
def drawdown(return_series: pd.Series, mise_depart : float = 1000):
    """
    Input :
        Takes a time series of asset returns
    
    Output :
        returns a DataFrame with columns for
            the wealth index, 
            the previous peaks, and 
            the percentage drawdown
    """
    wealth_index = mise_depart*(1+return_series).cumprod()
    previous_peaks = wealth_index.cummax()
    drawdowns = (wealth_index - previous_peaks)/previous_peaks
    return pd.DataFrame({"Wealth": wealth_index, 
                         "Previous Peak": previous_peaks, 
                         "Drawdown": drawdowns})

def max_drawdown(return_series: pd.Series, mise_depart : float = 1000):
    """
    input : Takes a time series of asset returns and the upfront investment
    output : max draxdown and the date of the max_drawdown
    """
    wealth_index = mise_depart*(1+return_series).cumprod()
    previous_peaks = wealth_index.cummax()
    drawdowns = (wealth_index - previous_peaks)/previous_peaks
    index = (drawdowns.idxmin()).date()
    max_dd = drawdowns.min()
    return [max_dd,index]

def semideviation(r):
    """
    Returns the semideviation aka negative semideviation of r
    r must be a Series or a DataFrame, else raises a TypeError
    """
    if isinstance(r, pd.Series):
        is_negative = r < 0
        return r[is_negative].std(ddof=0)
    elif isinstance(r, pd.DataFrame):
        return r.aggregate(semideviation)
    else:
        raise TypeError("Expected r to be a Series or DataFrame")


def var_historic(r, level=5):
    """
    Returns the historic Value at Risk at a specified level
    i.e. returns the number such that "level" percent of the returns
    fall below that number, and the (100-level) percent are above
    """
    if isinstance(r, pd.DataFrame):
        return r.aggregate(var_historic, level=level)
    elif isinstance(r, pd.Series):
        return -np.percentile(r, level)
    else:
        raise TypeError("Expected r to be a Series or DataFrame")


def cvar_historic(r, level=5):
    """
    Computes the Conditional VaR of Series or DataFrame
    """
    if isinstance(r, pd.Series):
        is_beyond = r <= -var_historic(r, level=level)
        return -r[is_beyond].mean()
    elif isinstance(r, pd.DataFrame):
        return r.aggregate(cvar_historic, level=level)
    else:
        raise TypeError("Expected r to be a Series or DataFrame")


from scipy.stats import norm
def var_gaussian(r, level=5, modified=False):
    """
    Returns the Parametric Gauusian VaR of a Series or DataFrame
    If "modified" is True, then the modified VaR is returned,
    using the Cornish-Fisher modification
    """
    # compute the Z score assuming it was Gaussian
    z = norm.ppf(level/100)
    if modified:
        # modify the Z score based on observed skewness and kurtosis
        s = skewness(r)
        k = kurtosis(r)
        z = (z +
                (z**2 - 1)*s/6 +
                (z**3 -3*z)*(k-3)/24 -
                (2*z**3 - 5*z)*(s**2)/36
            )
    return -(r.mean() + z*r.std(ddof=0))

def portfolio_vol(weights, covmat):
    """
    Computes the vol of a portfolio from a covariance matrix and constituent weights
    weights are a numpy array or N x 1 maxtrix and covmat is an N x N matrix
    """
    return (weights.T @ covmat @ weights)**0.5

def portfolio_return(weights, returns):
    #multiplication de matrice
    return weights.T @ returns


from scipy.optimize import minimize
def minimize_vol(target_return, er, cov):
    """
    Returns the optimal weights that achieve the target return
    given a set of expected returns and a covariance matrix
    """
    n = er.shape[0]
    init_guess = np.repeat(1/n, n)
    bounds = ((0.0, 1.0),) * n # an N-tuple of 2-tuples!
    # construct the constraints
    weights_sum_to_1 = {'type': 'eq',
                        'fun': lambda weights: np.sum(weights) - 1
    }
    return_is_target = {'type': 'eq',
                        'args': (er,),
                        'fun': lambda weights, er: target_return - portfolio_return(weights,er)
    }
    weights = minimize(portfolio_vol, init_guess,
                       args=(cov,), method='SLSQP',
                       options={'disp': False},
                       constraints=(weights_sum_to_1,return_is_target),
                       bounds=bounds)
    return weights.x



def msr(riskfree_rate, er, cov):
    """
    Returns the weights of the portfolio that gives you the maximum sharpe ratio
    given the riskfree rate and expected returns and a covariance matrix
    """
    n = er.shape[0]
    init_guess = np.repeat(1/n, n)
    bounds = ((0.0, 1.0),) * n # an N-tuple of 2-tuples!
    # construct the constraints
    weights_sum_to_1 = {'type': 'eq',
                        'fun': lambda weights: np.sum(weights) - 1
    }
    #constraint weight < 50%
    
    def neg_sharpe(weights, riskfree_rate, er, cov):
        """
        Returns the negative of the sharpe ratio
        of the given portfolio
        """
        r = portfolio_return(weights, er)
        vol = portfolio_vol(weights, cov)
        return -(r - riskfree_rate)/vol
    
    weights = minimize(neg_sharpe, init_guess,
                       args=(riskfree_rate, er, cov), method='SLSQP',
                       options={'disp': False},
                       constraints=(weights_sum_to_1,),
                       bounds=bounds)
    return weights.x


def gmv(cov):
    """
    Returns the weights of the Global Minimum Volatility portfolio
    given a covariance matrix
    """
    n = cov.shape[0]
    return msr(0, np.repeat(1, n), cov)

def overall_performance(r):
    """
    Return the performance from the beginning to the end of the Series of return r 
    
    Input :
        pd.Series of return r
        
    Output :
        return a float of the overall performance
    """
    wealth_index = 100*(1+r).cumprod()
    perf = wealth_index[-1]/wealth_index[0] - 1
    return perf

def traking_error(portfolio_return, benchmark_return, period_in_one_year):
    """
    this function return the tracking error
    input:
        portfolio return, benchmark return and periods per year
    """
    TE = (portfolio_return-benchmark_return).std()*period_in_one_year**0.5
    return TE

def traking_error_series(portfolio_return, benchmark_return, period_in_one_year):
    """
    this function return the tracking error
    input:
        portfolio return, benchmark return and periods per year
    """
    TE = pd.Series()
    
    dates = benchmark_return[12:].index
    
    for e in dates:
        TE[e] = traking_error(portfolio_return[:e],benchmark_return[:e],period_in_one_year)
    
    return TE

def vol_ex_ante(weights, cov, period_in_one_year = 252):
    """
    Input :
        weights
        cov
        
    Description :
    return the vol ex_ante
    """
    vol = (weights.transpose().dot(cov).dot(weights)*period_in_one_year)**0.5
    return vol

def summary_stats(r, riskfree_rate=0.03, periods : int = 12):
    """
    Return a DataFrame that contains aggregated summary stats for the returns in the columns of r
    """
    overall_perf = r.aggregate(overall_performance)
    ann_r = r.aggregate(annualize_rets, periods_per_year=periods)
    ann_vol = r.aggregate(annualize_vol, periods_per_year=periods)
    ann_sr = r.aggregate(sharpe_ratio, riskfree_rate=riskfree_rate, periods_per_year=periods)
    dd = r.aggregate(lambda r: drawdown(r).Drawdown.min())
    skew = r.aggregate(skewness)
    kurt = r.aggregate(kurtosis)
    historic_var5 = r.aggregate(var_historic)
    cf_var5 = r.aggregate(var_gaussian, modified=True)
    hist_cvar5 = r.aggregate(cvar_historic)
    return pd.DataFrame({
        "Overall Performance": overall_perf,
        "Annualized Return": ann_r,
        "Annualized Vol": ann_vol,
        "Skewness": skew,
        "Kurtosis": kurt,
        "Cornish-Fisher VaR (5%)": cf_var5,
        "Historic VaR (5%)": historic_var5,
        "Historic CVaR (5%)": hist_cvar5,
        "Sharpe Ratio": ann_sr,
        "Max Drawdown": dd
    })

def summary_stats_serie(r, periods : int = 4):
    """
    Return a DataFrame that contains summary stats for the Series returns r
    """
    overall_perf = overall_performance(r)
    ann_r = annualize_rets(r,periods_per_year=periods)
    ann_vol = annualize_vol(r, periods_per_year=periods)
    ann_sr = sharpe_ratio(r, riskfree_rate=0, periods_per_year=periods) #on utilise la vol annualisé comme dans le cours à la place du risk_free_rate
    s_r = overall_perf/ann_vol
    dd = drawdown(r).Drawdown.min()
    skew = skewness(r)
    kurt = kurtosis(r)
    historic_var5 = var_historic(r)
    cf_var5 = var_gaussian(r, modified=True)
    hist_cvar5 = cvar_historic(r)
    return pd.Series({
        "Overall Performance": overall_perf,
        "Annualized Return": ann_r,
        "Annualized Vol": ann_vol,
        "Skewness": skew,
        "Kurtosis": kurt,
        "Cornish-Fisher VaR (5%)": cf_var5,
        "Historic VaR (5%)": historic_var5,
        "Historic CVaR (5%)": hist_cvar5,
        "Sharpe Ratio Annualisé": ann_sr,
        "Sharpe_ratio": s_r,
        "Max Drawdown": dd
    })
