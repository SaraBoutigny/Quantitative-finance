function [ B ] = ZC( Rate, Maturity)
    B=exp(-Rate*Maturity);
end
