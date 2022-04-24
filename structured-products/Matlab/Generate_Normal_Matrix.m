function [ W ] = Generate_Normal_Matrix( Nb_Dates,Nb_Simulations )

rng(90)

W = randn(Nb_Dates,Nb_Simulations);

end

