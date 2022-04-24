# -*- coding: utf-8 -*-
"""
Created on Tue Nov 10 00:54:03 2020

@author: saraboutigny
"""


#Importations
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import math
#from pypfopt.efficient_frontier import EfficientFrontier
#from pypfopt import risk_models
#from pypfopt import expected_returns



#-----------------------------------------------------------------------------
# Récupération des données
#-----------------------------------------------------------------------------

# PRIX ======================================================================
# Récupération des prix depuis la feuille Excel
prices = pd.read_excel('data.xlsx',index_col=0,header=3, sheet_name='Feuil2')

# A supprimer plus tard : récupération seulement de la colonne 
prices=prices.iloc[3:,] #30 pour sanofi

# Récupération des dates en indices pour les garder en mémoire
dates=prices.index.values

# suppression des dates en indices dans le dataframe + mise en base 1
prices.reset_index(drop=True,inplace=True)
prices.index+=1

# Expression des prix en base 100
prices_rebased = prices/prices.iloc[0,:]*100
prices_rebased.index+=1

# Définition de la plage d'indices générique (qui servira d'abscisse aux graphs)
# Voir si nécessaire
idx = pd.DataFrame(np.linspace(1,prices.size,prices.size))

# RENDEMENTS ================================================================

# Calcul des rendements, mise à zéro de la première ligne
rtn=prices.pct_change()
rtn.loc[1,]=0

# Suppression des dates en indices
rtn.reset_index(drop=True,inplace=True)
rtn.index+=1

# Calcul des rendements cumulés et remplacement de la première ligne par 1
rtncum=(1 + rtn).cumprod()
rtncum.loc[1,] = 1


#--------------------------------------------------------------------------
# 1) Algorithmes de trading systématique
#--------------------------------------------------------------------------

def strategy_signal(df_prices,n,strat_type='mom',short=False,bollinger=False,nb_std=0):
    
    prices=df_prices
    movavg=prices.rolling(n).mean()
    
    if bollinger == False:
        #Stratégies : (1 et 0 pour LONG ONLY, 1 et -1 pour LONG SHORT)
        if strat_type == 'mom':
            if short == False:
                #Momentum LONG ONLY
                df=(prices.shift(1)>=movavg.shift(1))*1
                df.index-=1
            else:
                #Momentum LONG SHORT
                df=(prices.shift(1)>=movavg.shift(1))*2-1
                df.index-=1
        
        else:
            if short == False:
                #Contrariante LONG ONLY
                df = (prices.shift(1)<=movavg.shift(1))*1
                df.index-=1
            else:
                #Contrariante LONG SHORT
                df = (prices.shift(1)<=movavg.shift(1))*2-1
                df.index-=1
    
    else:
        #Bollinger
        ecartyp=prices.rolling(2).std()
        ecartyp.fillna(0)
        ubound = movavg+nb_std*ecartyp
        lbound = movavg-nb_std*ecartyp
        
        df = pd.DataFrame(np.full((prices.shape[0],prices.shape[1]),np.nan))
        df.index+=1
        df.iloc[0,:]=0
        df.columns = prices.columns
        
       
        if strat_type == 'mom':
            if short==False:
                for i in range(prices.index.min()-1,prices.index.max()-1):
                    for j in range(0,len(prices.columns)):
                        
                        if (prices.iloc[i-1,j] >= ubound.iloc[i-1,j]) and (prices.iloc[i-2,j] <= ubound.iloc[i-2,j]):
                            df.iloc[i,j] = 1
                    
                        elif (prices.iloc[i-1,j] <= lbound.iloc[i-1,j]) and (prices.iloc[i-2,j] >= lbound.iloc[i-2,j]):
                            df.iloc[i,j] = 0
                            
                        else:
                            
                            df.iloc[i,j]=df.iloc[i-1,j]
            else:
                
                for i in range(prices.index.min()-1,prices.index.max()-1):
                    for j in range(0,len(prices.columns)):
                        
                        if (prices.iloc[i-1,j] >= ubound.iloc[i-1,j]) and (prices.iloc[i-2,j] <= ubound.iloc[i-2,j]):
                            df.iloc[i,j] = 1
                    
                        elif (prices.iloc[i-1,j] <= lbound.iloc[i-1,j]) and (prices.iloc[i-2,j] >= lbound.iloc[i-2,j]):
                            df.iloc[i,j] = -1
                            
                        else:
                            
                            df.iloc[i,j]=df.iloc[i-1,j]
        
        else:
            
            if short==False:
                #Contrariante LONG ONLY
                for i in range(prices.index.min()-1,prices.index.max()-1):
                    for j in range(0,len(prices.columns)):
                        
                        if (prices.iloc[i-1,j] >= ubound.iloc[i-1,j]) and (prices.iloc[i-2,j] <= ubound.iloc[i-2,j]):
                            df.iloc[i,j] = 0
                    
                        elif (prices.iloc[i-1,j] <= lbound.iloc[i-1,j]) and (prices.iloc[i-2,j] >= lbound.iloc[i-2,j]):
                            df.iloc[i,j] = 1
                            
                        else:
                            
                            df.iloc[i,j]=df.iloc[i-1,j]

            else:
               #Contrariante LONG SHORT
               for i in range(prices.index.min()-1,prices.index.max()-1):
                   for j in range(0,len(prices.columns)):
                       
                       if (prices.iloc[i-1,j] >= ubound.iloc[i-1,j]) and (prices.iloc[i-2,j] <= ubound.iloc[i-2,j]):
                           df.iloc[i,j] = -1
                   
                       elif (prices.iloc[i-1,j] <= lbound.iloc[i-1,j]) and (prices.iloc[i-2,j] >= lbound.iloc[i-2,j]):
                           df.iloc[i,j] = 1
                           
                       else:
                           
                           df.iloc[i,j]=df.iloc[i-1,j]
                         
    return df

# RECHERCHE DU N OPTIMAL ==================================================
#"""
strat_rtnvol = {}
loop=0

for n in range(prices.index.min(),251):
    loop+=1
    if n%50==0:
        print(n)
    strat_collection = {}
    
    
    strat_collection['mom_LO']=strategy_signal(prices_rebased,n,'mom',short=False)
    strat_collection['cont_LO']=strategy_signal(prices_rebased,n,'cont',short=False)
    strat_collection['mom_LS']=strategy_signal(prices_rebased,n,'mom',short=True)
    strat_collection['cont_LS']=strategy_signal(prices_rebased,n,'cont',short=True)
    strat_collection['boll_mom_LO']=strategy_signal(prices_rebased,n,'mom',short=False,bollinger=True,nb_std=2)
    strat_collection['boll_cont_LO']=strategy_signal(prices_rebased,n,'cont',short=False,bollinger=True,nb_std=2)
    strat_collection['boll_mom_LS']=strategy_signal(prices_rebased,n,'mom',short=True,bollinger=True,nb_std=2)
    strat_collection['boll_cont_LS']=strategy_signal(prices_rebased,n,'cont',short=True,bollinger=True,nb_std=2)                                     
    
   
    #Calcul des performances des 4 stratégies 
    strat_rtn={}
    strat_rtncum={}
    strat_rtncum_rebased={}
    
    #initialisation du dictionnaire dans lequel on récupère les tableaux de 
    #perf/vol des stratégies
    if loop==1:
        for name, df in strat_collection.items():
            strat_rtnvol[name] = pd.DataFrame(np.full((prices.shape[0],prices.shape[1]),np.nan))
            strat_rtnvol[name].index+=1
            strat_rtnvol[name].columns = prices.columns
    
    #Calcul, pour chaque stratégie
    for name, df in strat_collection.items():
        #perf
        strat_rtn[name]=df*rtn

        #perf cumulée
        strat_rtncum[name]=(1 + strat_rtn[name]).cumprod()
        strat_rtncum[name].loc[1, ] = 1
        
        #perf rebasée au prix de l'actif à la date n
        strat_rtncum_rebased[name] = strat_rtncum[name]/strat_rtncum[name].iloc[0,:] \
                                    * prices_rebased.iloc[n-1,:]
                                    
        #on fait démarrer la stratégie en n en affectant nan aux valeurs précédentes
        strat_rtncum_rebased[name].iloc[:n,:]=np.nan

        #Récupération des perf/vol : 4 tableaux de [nbPeriodes * nbActifs], 
        #1 tableau par stratégie
        #Calcul de la vol annualisée
        vol=strat_rtn[name].std(axis=0)*math.sqrt(252)
        strat_rtnvol[name].loc[n] = (strat_rtncum[name].iloc[-1]-1)/vol
    
    n_opt ={}
    for name,df in strat_rtnvol.items():
        n_opt[name]=df.idxmax(axis=0)


#Affichage des graphiques pour le dernier n de la boucle, pour chaque titre
"""
titres = prices.columns

for titre in titres:
    plt.figure()
    plt.title('Stratégies ' + titre) 
    
    plt.plot(strat_rtncum_rebased['mom_LO'][titre])
    plt.plot(strat_rtncum_rebased['cont_LO'][titre])   
    plt.plot(strat_rtncum_rebased['mom_LS'][titre])
    plt.plot(strat_rtncum_rebased['cont_LS'][titre])
    plt.plot(strat_rtncum_rebased['boll_mom_LO'][titre])
    plt.plot(strat_rtncum_rebased['boll_cont_LO'][titre])   
    plt.plot(strat_rtncum_rebased['boll_mom_LS'][titre])
    plt.plot(strat_rtncum_rebased['boll_cont_LS'][titre])
    
    plt.plot(prices_rebased[titre],'b')
    
    plt.legend(('Mom LO', 'Cont LO', 'Mom LS', 'Cont LS','boll_mom_LO','boll_cont_LO','boll_mom_LS','boll_cont_LS','Price'))
"""
#"""      
#--------------------------------------------------------------------------
# 2) Optimisation de portefeuille
#--------------------------------------------------------------------------

# BACKTESTING =============================================================
#"""



def GenerateWeights(df_prices, obj='min_vol',equi=False):
    
    #Calcul du portefeuille efficient à investir
    avg_returns = expected_returns.mean_historical_return(df_prices)
    cov_mat = risk_models.sample_cov(df_prices)
    frontier = EfficientFrontier(avg_returns, cov_mat, weight_bounds=(0.01,0.10))
    
    #Poids en fonction de l'objectif
    if obj == 'min_vol':
        weights = pd.DataFrame(frontier.min_volatility(),index=[0])
    else:
        weights = pd.DataFrame(frontier.max_sharpe(),index=[0])
    
    if equi==True:
        weights[:] = 1/weights.shape[1]
        
    return weights

def AddAlloc(df_alloc, df_weights,EDate):
    df=df_alloc.copy()
    #report de l'allocation pour le mois
        #définition de la fenêtre du mois
    Date_MonthStart = EDate + pd.tseries.offsets.Day(1)
    Date_NextM = EDate + pd.tseries.offsets.MonthEnd(M)
        #extension de la ligne au nombre de lignes du mois pour pouvoir l'ajouter dans alloc_minvol
    df_weights_ext = pd.concat([df_weights]*df[Date_MonthStart:Date_NextM].shape[0])
    df_weights_ext.index=df[Date_MonthStart:Date_NextM].index
        #ajout
    df[Date_MonthStart:Date_NextM]=df_weights_ext
    
    return df


#On remet en indice les dates
prices.index=dates

#définition de la date de départ
M = 1
Y = 12 * M

#Réindexation du dataframe de prix pour inclure toutes les dates existantes 
#(si P_t n'exitait pas, on prend P_t-1)
prices=prices.reindex(pd.date_range(prices.index.min(), prices.index.max()),method='pad')

#Définition des bornes initiales de la fenêtre roulante
SDate= prices.index[0]
EDate = prices.index[0] + pd.tseries.offsets.MonthEnd(5*Y)

#Initialisation des dataframes qui regroupent l'allocation sur toute la période
alloc_minvol = pd.DataFrame(np.full((prices.shape[0],prices.shape[1]),np.nan))
alloc_minvol.index=prices.index
alloc_minvol.columns = prices.columns
alloc_maxsharpe=alloc_minvol
alloc_equi=alloc_minvol

DateTest=SDate

#Calcul de l'allocation stratégique
while DateTest in prices.index:
    
    #définition de la fenêtre roulante
    prices_5y=prices[SDate:EDate]
    
    #Portefeuille min_vol
        #Génération des poids optimaux
    weights=GenerateWeights(prices_5y,'min_vol')
    
        #report de l'allocation pour le mois
    alloc_minvol=AddAlloc(alloc_minvol,weights,EDate)
    
    #Portefeuille max sharpe
        #Génération des poids optimaux
    weights=GenerateWeights(prices_5y,'max_sharpe')
    
        #report de l'allocation pour le mois
    alloc_maxsharpe=AddAlloc(alloc_maxsharpe,weights,EDate)
    
    #Portefeuille équipondéré pour comparaison
    weights = GenerateWeights(prices_5y,'max_sharpe',equi=True)
    alloc_equi=AddAlloc(alloc_equi,weights,EDate)

    #Décalage de la fenêtre au mois suivant
    SDate+=pd.tseries.offsets.MonthEnd(M)
    EDate+=pd.tseries.offsets.MonthEnd(M)
    DateTest = EDate + pd.tseries.offsets.Day(1)

#Calcul de la performance des portefeuilles minvol et maxsharpe
rtn = prices/prices.shift(1)-1

    #Portefeuille équipondéré
port_equi_rtn = pd.DataFrame(np.sum(rtn*alloc_equi.shift(1),axis=1))
port_equi_rtncum = (1 + port_equi_rtn).cumprod()
port_equi_rtncum.iloc[0,] = 1
port_equi_rtncum_rebased = port_equi_rtncum/port_equi_rtncum.iloc[0,:]*100
vol_equi = port_equi_rtn.std()  * math.sqrt(252)

    #Portefeuille min vol
port_minvol_rtn = pd.DataFrame(np.sum(rtn*alloc_minvol.shift(1),axis=1))
port_minvol_rtncum = (1 + port_minvol_rtn).cumprod()
port_minvol_rtncum.iloc[0,] = 1
port_minvol_rtncum_rebased = port_minvol_rtncum/port_minvol_rtncum.iloc[0,:]*100
vol_minvol = port_minvol_rtn.std() * math.sqrt(252)

    #Portefeuille max Sharpe
port_maxsharpe_rtn = pd.DataFrame(np.sum(rtn*alloc_maxsharpe.shift(1),axis=1))
port_maxsharpe_rtncum = (1 + port_maxsharpe_rtn).cumprod()
port_maxsharpe_rtncum.iloc[0,] = 1
port_maxsharpe_rtncum_rebased = port_maxsharpe_rtncum/port_maxsharpe_rtncum.iloc[0,:]*100
vol_maxsharpe = port_maxsharpe_rtn.std() * math.sqrt(252)

#Affichage des performances
plt.title('Performances')
plt.plot(port_equi_rtncum_rebased)
plt.plot(port_minvol_rtncum_rebased)
plt.plot(port_maxsharpe_rtncum_rebased)

plt.figure()

plt.title('Performances rapportées à la volatilité')
plt.plot(port_equi_rtncum_rebased/vol_equi)
plt.plot(port_minvol_rtncum_rebased/vol_minvol)
plt.plot(port_maxsharpe_rtncum_rebased/vol_maxsharpe)

plt.legend(('equi', 'min vol', 'max sharpe'))

