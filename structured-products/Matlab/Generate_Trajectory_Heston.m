function [ Spot_Trajectories ] = Generate_Trajectory_Heston( Spot_Init,Dividend_Yield,Repo,V0,Eta,Theta,Kappa,Rho,Rate,Maturities,Nb_Trajectories,Normal_Maxtrix_Spot,Normal_Maxtrix_Temp)



%Maturities should contain the value 0 corresponding to the initial date
dT=0.01;
Dates=0:dT:Maturities(end);
Dates=union(Dates,Maturities);
Nb_Dates=length(Dates);

if (nargin < 13)  
    Normal_Maxtrix_Spot=Generate_Normal_Matrix(Nb_Dates-1,Nb_Trajectories);
    Normal_Maxtrix_Temp=Generate_Normal_Matrix(Nb_Dates-1,Nb_Trajectories);
end

Normal_Maxtrix_Vol=Rho*Normal_Maxtrix_Spot+sqrt(1-Rho^2)*Normal_Maxtrix_Temp;

Spot_Trajectories=zeros(Nb_Dates+1,Nb_Trajectories);
Spot_Trajectories(1,:)=Spot_Init*ones(1,Nb_Trajectories);

Vol_Trajectories=zeros(Nb_Dates+1,Nb_Trajectories);
Vol_Trajectories(1,:)=V0*ones(1,Nb_Trajectories);

Delta_t=Dates(2:end)-Dates(1:end-1);

Spot_To_Return = zeros(length(Maturities),Nb_Trajectories);
Spot_To_Return(1,:)= Spot_Trajectories(1,:);
i=2;
for t=1:Nb_Dates-1
   
    Spot_Trajectories(t+1,:)=Spot_Trajectories(t,:).*exp((Rate-Repo-Dividend_Yield-0.5*Vol_Trajectories(t,:))*Delta_t(t) + sqrt(Delta_t(t) * Vol_Trajectories(t,:)).*Normal_Maxtrix_Spot(t,:));
    Vol_Trajectories(t+1,:)= Vol_Trajectories(t,:) + Kappa*(Theta-Vol_Trajectories(t,:))*Delta_t(t)+Eta*sqrt(Vol_Trajectories(t,:)).*Normal_Maxtrix_Vol(t,:)*sqrt(Delta_t(t));
    Vol_Trajectories(t+1,:)= Vol_Trajectories(t+1,:).*(Vol_Trajectories(t+1,:)>0)+(1-(Vol_Trajectories(t+1,:)>0)).*(Vol_Trajectories(t,:) + Kappa*(Theta-Vol_Trajectories(t,:))*Delta_t(t)+Eta*sqrt(Vol_Trajectories(t,:)).*-Normal_Maxtrix_Vol(t,:)*sqrt(Delta_t(t)));
    if(Dates(t+1)==Maturities(i))
        Spot_To_Return(i,:)= Spot_Trajectories(t+1,:);
        i=i+1;
    end
end

Spot_Trajectories=Spot_To_Return';
end