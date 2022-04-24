import pandas as pd
import numpy as np
import math
import matplotlib.pyplot as plt
import risk_kit as rk
import datetime as dt

def signal_momentum(df_px_last : pd.DataFrame, nbre : int = 250, half : bool = False) -> pd.DataFrame:
    """
    input : px_last of the assets
    output : 
        if half :
            1Y + 6months momentum signal 
        else:
        the  1Y momentum factor signal => P(t-20)/P(t-250) - 1
    """
    if half :
        ret_momentum = (df_px_last.shift(20)/df_px_last.shift(nbre)-1)*0.75 + (df_px_last.shift(20)/df_px_last.shift(nbre/2)-1)*0.25
    else:
        ret_momentum = df_px_last.shift(20)/df_px_last.shift(nbre)-1
    
    return ret_momentum[250:]


def plot_portfolio(portfolio_return : pd.DataFrame, mise_depart : float = 100):
    """
    plot l'évolution du portefeuille avec une mise de départ initiale (100euros par défaut)
    """
    portfolio_value = mise_depart*np.cumprod(1+portfolio_return)
    plt.plot(portfolio_value)


def strategie(df_px_last : pd.DataFrame, signal : pd.DataFrame, weights_in_bench, bench_isin : list,
                       bench_weights : float = 0.8, number_assets_portfolio = 2, 
                       rebalancing_frequency : str = "BM"):
    """
    une stratégie qui construit un portefeuille comme suit :
        - alloue bench_weights au bench (dans les mêmes proportions que le bench)
        - alloue le reste (ie) [1 - bench_weights] à des actifs choisi selon le signal momentum
        
    cette fonction calcule également la vol ex_ante sur une période de 1 an
    
    input : 
            les px_last des assets : df_px_last 
            le signal pour la sélection des assets à allouer : signal
            la composition du poids du bench : weights_in_bench
            la composition du bench : bench_isin
            le pourcentage alloué au bench : bench_weights
            le nombre d'actif alloué en plus dans le portefeuille : number_assets_portfolio
            la période de rebalancement : rebalancing_frequency
            
    output : 
            un dictionnaire de pd.DataFrame : 
                le rendement du portefeuille
                les rendements des actifs composant le portefeuille à chaque date
                les actifs allouée à chaque date
                la vol_ex_ante après un an de data
            
    """
    #Series Initialisation for stocking return
    portfolio_return = pd.Series()
    ret_tot = pd.DataFrame()
    assets = pd.DataFrame()
    vol_ex_ante_series = pd.Series()
    
    portfolio_weights = np.concatenate([weights_in_bench * bench_weights, np.repeat((1.0-bench_weights)/number_assets_portfolio,number_assets_portfolio)])
    print(sum(portfolio_weights))
    
    #fill px_last
    df_px_last = rk.fill_Nav(df_px_last,rebalancing_frequency)
    signal = rk.fill_Nav(signal,rebalancing_frequency)
    
    dates_rebalancing = pd.date_range(df_px_last.index[0],df_px_last.index[-1],freq=rebalancing_frequency)
    ret = (df_px_last/df_px_last.shift()-1)[1:]
    
    e_ex = dates_rebalancing[0]
    e_1y_before = dates_rebalancing[0]
    for e in dates_rebalancing :
        
        index = list(df_px_last.index).index(e.date().strftime("%Y-%m-%d"))
        ex_index = list(df_px_last.index).index(e_ex.date().strftime("%Y-%m-%d"))
        
        #facteur permettant de choisir les assets dans le portefeuille
        sorted_signal = signal.iloc[ex_index].sort_values().dropna().index
        
        #actif à allouer dans le portefeuille
        assets_alloc = bench_isin + list(sorted_signal[-number_assets_portfolio:])
        #assets_alloc = bench_isin + list(sorted_signal[0:number_assets_portfolio])
        
        #return of assets
        price_long = df_px_last.iloc[index][assets_alloc]*0.9995 #spread bid/ask de 0.1%
        price_short = df_px_last.iloc[ex_index][assets_alloc]*1.0005 #spread bid/ask de 0.1%
        ret_assets = price_long / price_short - 1
        
        #saving data
            #saving the weights, allocation and portfolio assets from t = dates[ex_index] to t = dates[index]
        assets[e_ex] = assets_alloc
            #saving the return of the assets 
        ret_tot[e_ex] = ret_assets.values
        
        #portfolio return
        portfolio_return[e] = portfolio_weights@ret_assets
        
        #tracking error et vol_ex_ante
        if e >= dates_rebalancing[12]:
            #vol ex ante
            index_ret = list(ret.index).index(e.date().strftime("%Y-%m-%d"))
            cov = ret[index_ret-252:index_ret][assets_alloc].cov()
            #cov = ret[index_ret-126:index_ret][assets_alloc].cov()
            vol_ex_ante_series[e] = rk.vol_ex_ante(portfolio_weights,cov)
            
        #reinitialisation
        e_ex = e
        
    #faire pour la dernière date (ie ex_index) jusqu'à le dernier jour du dataset ?
    

    return {
        "Portfolio Return": portfolio_return,
        "return" : ret_tot,
        "Assets in Portfolio" : assets,
        "Vol Ex_Ante" : vol_ex_ante_series
    }