clear;
clc;

%% RECUPERATION DES DONNEES DE L'EURO STOXX 50
[UPrice,dates]=xlsread('STOXX50E.xlsx','STOXX50E');

%% DEFINITION DES PARAMETRES DU MODELE
Strike = mean([3414.85 3438.78 3419.71 3443.97 3448.00]); % prix au 9/04/2018

Spot_Init=3414.85;
Dividend_Yield=2.19/100; % Dividend Yield calculé par Reuters
Repo=0; % Taux repo de la BCE
Rate=-0.48/100; % EONIA
Maturity=10;
Nb_Trajectories=10000;

%yeti coupon = 3%
%yeti barrier = 85%
%Barrière de bonus
YetiCoupon = 3/100;
YetiBarrier = 0.85;

%Barrière de rappel
AutoCall = 1;

%Barrière de capital
CapitalBarrier=0.6;

%Calcul de la volatilité sur l'année passée
Return = diff(log(UPrice));
Volatility = std(Return(length(Return)-252:end))*sqrt(252);

Freq=1;

Dates = 0:Freq:Maturity;


Price_BS = Price_AutoCall('BS',YetiCoupon,Strike,YetiBarrier,AutoCall,Freq,Spot_Init,Dividend_Yield,Repo,Volatility,Rate,Maturity,Nb_Trajectories,CapitalBarrier);
Price_Heston = Price_AutoCall('Heston',YetiCoupon,Strike,YetiBarrier,AutoCall,Freq,Spot_Init,Dividend_Yield,Repo,Volatility,Rate,Maturity,Nb_Trajectories,CapitalBarrier);

%% GREEKS -----------------------------------------------------------------
%% Delta

Bump=5;
Part_Spot = 20:1:200;
Price_Delta = zeros(length(Part_Spot),1);

for i =1:1:length(Part_Spot)
    Price_Delta(i) = Price_AutoCall('BS',YetiCoupon,Strike,YetiBarrier,AutoCall,Freq,Spot_Init * Part_Spot(i)/100,Dividend_Yield,Repo,Volatility,Rate,Maturity,Nb_Trajectories,CapitalBarrier);
end

Delta = ((Price_Delta(1+Bump:end)-Price_Delta(1:end-Bump))/Bump);


plot((Spot_Init * (Part_Spot(1:end-Bump)/100))',Delta*100);
title('Delta');
xlabel('Initial Spot (€)');
ylabel('Price variation (%)');
ytickformat('%.1f');
hold on
plot(Spot_Init,Delta((size(Delta,1))/2-Bump-1)*100,'r.','MarkerSize',12);
%% Gamma

Gamma = (Price_Delta(1+2*Bump:end)+Price_Delta(1:end-2*Bump)-2*Price_Delta(1+Bump:end-Bump))/Bump^2;
figure;
plot((Spot_Init * (Part_Spot(1:end-2*Bump)/100))',Gamma*100)
title('Gamma');
xlabel('Initial Spot (€)');
ylabel('Gamma (%)');
ytickformat('%.2f');
hold on
plot(Spot_Init,Gamma(round(size(Gamma,1)/2-Bump-1))*100,'r.','MarkerSize',12);


%% Vega

Bump=0.1/100;

Price_Vega_Up = zeros(length(Part_Spot),1);
Price_Vega_Down = zeros(length(Part_Spot),1);

for i =1:1:length(Part_Spot)
    Price_Vega_Up(i) = Price_AutoCall('BS',YetiCoupon,Strike,YetiBarrier,AutoCall,Freq,Spot_Init * Part_Spot(i)/100,Dividend_Yield,Repo,Volatility+Bump,Rate,Maturity,Nb_Trajectories,CapitalBarrier);
    Price_Vega_Down(i) = Price_AutoCall('BS',YetiCoupon,Strike,YetiBarrier,AutoCall,Freq,Spot_Init * Part_Spot(i)/100,Dividend_Yield,Repo,Volatility-Bump,Rate,Maturity,Nb_Trajectories,CapitalBarrier);
end

Vega = (Price_Vega_Up-Price_Vega_Down)/2*Bump;
figure;
plot((Volatility * (Part_Spot/100))*100',Vega*100)
title('Vega');
xlabel('Volatility (%)');
ylabel('Vega (%)');
ytickformat('%.2f');
hold on
plot(Volatility*100,Vega(round(size(Vega,1)/2-1))*100,'r.','MarkerSize',12);

%% MODELE HESTON ----------------------------------------------------------
%%
Maturities = Dates;
V0 = Volatility*Volatility;
Eta = 0; %vol de vol (ie) variance
Theta = Volatility*Volatility; %Dans le modèle, on simule la variance => Theta = variance à l'infini, variance long terme
Kappa = 0; % force de retour à la moyenne
Rho = 0; %corrélation entre les 2 lois normales (ie) le spot et la vol
Spot_Trajectories_Heston = Generate_Trajectory_Heston(Spot_Init,Dividend_Yield,Repo,V0,Eta,Theta,Kappa,Rho,Rate,Maturities,Nb_Trajectories);
Forward_Heston = mean(Spot_Trajectories_Heston); 


