function [ Spot_Trajectories ] = Generate_Trajectory_BS( Spot_Init,Dividend_Yield,Repo,Volatility,Rate,Maturities,Nb_Trajectories,Normal_Maxtrix)
%Spot init = prix à la date t-1 du sous jacent 

%Maturities should contain the value 0 corresponding to the initial date

Nb_Dates=length(Maturities);
if (nargin < 8)
    Normal_Maxtrix=Generate_Normal_Matrix(Nb_Dates-1,Nb_Trajectories);
end

Spot_Trajectories=zeros(Nb_Dates,Nb_Trajectories);
Spot_Trajectories(1,:)=Spot_Init*ones(1,Nb_Trajectories);
Delta_t=Maturities(2:Nb_Dates)-Maturities(1:Nb_Dates-1);

for t=1:Nb_Dates-1
    Spot_Trajectories(t+1,:)=Spot_Trajectories(t,:).*exp((Rate-Repo-Dividend_Yield-0.5*Volatility*Volatility)*Delta_t(t)+Volatility*sqrt(Delta_t(t))*Normal_Maxtrix(t,:));
end

Spot_Trajectories=Spot_Trajectories';
end