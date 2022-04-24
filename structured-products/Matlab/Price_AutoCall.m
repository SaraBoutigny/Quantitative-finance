function [ Prix, Error ] = Price_AutoCall(Model,Coupon,Strike,Barrier,AutoCall,Freq,Spot_Init,Dividend_Yield,Repo,Volatility,Rate,Maturity,Nb_Trajectories,CapitalBarrier)

Dates=0:Freq:Maturity;

if Model == "BS"
    Spot_Trajectories = Generate_Trajectory_BS( Spot_Init,Dividend_Yield,Repo,Volatility,Rate,Dates,Nb_Trajectories);
elseif Model == "Heston"
    Maturities = Dates;
    V0 = Volatility*Volatility;
    Eta = 0;
    Theta = Volatility*Volatility;
    Kappa = 0;
    Rho = 0; 
    Spot_Trajectories = Generate_Trajectory_Heston(Spot_Init,Dividend_Yield,Repo,V0,Eta,Theta,Kappa,Rho,Rate,Maturities,Nb_Trajectories);

end 



Price=zeros(Nb_Trajectories,1);
ACoupon = zeros(Nb_Trajectories,1);
YCoupon = zeros(Nb_Trajectories,1);
ZCt=zeros(Nb_Trajectories,1);
IsAutocalled=zeros(Nb_Trajectories,length(Dates));


for traj=1:Nb_Trajectories
    for t=2:length(Dates)
            BT=ZC(Rate,Dates(t));
            
            %BARRIERE DE RAPPEL -------------------------------------------
            if(Spot_Trajectories(traj,t)>AutoCall*Strike)
                %Si le spot est au dessus de la barrière de rappel, on
                %rappelle le produit
                IsAutocalled(traj,t)=1; 
            else
                % Si en dessous de la barrière de rappel, report de la
                % situation précédente
                IsAutocalled(traj,t)=IsAutocalled(traj,t-1); 
            end
            
            %Si le produit vient d'être autocalled, alors on applique
            % les 100%
            if( IsAutocalled(traj,t) && (IsAutocalled(traj,t-1)==false))
                ZCt(traj)=BT;
                ACoupon(traj) = AutoCall * BT;
            end
            
            %BARRIERE DE BONUS -------------------------------------------
            % Si le produit n'est pas autocalled en t-1
            if(IsAutocalled(traj,t-1)==false)
                if(Spot_Trajectories(traj,t)> Barrier*Strike ) 
                    YCoupon(traj)=YCoupon(traj)+ BT * Coupon;
                end
            end 
            
            %BARRIERE DE CAPITAL ------------------------------------------
            if (t==Maturity+1)
                if(IsAutocalled(traj,t-1)==false)
                    if (Spot_Trajectories(traj,t)> CapitalBarrier*Strike ) 
                        ACoupon(traj) = AutoCall * BT;
                    else
                        ACoupon(traj) = Spot_Trajectories(traj,t)/Strike * BT;
                    end
                    
                end
            end    
            
            
        Price(traj)=YCoupon(traj) + ACoupon(traj);
    end
end

Prix=mean(Price);
Error=std(Price);


end

