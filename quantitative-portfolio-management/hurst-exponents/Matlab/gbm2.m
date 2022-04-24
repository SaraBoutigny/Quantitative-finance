function result =gbm2(mu, sigma, steps_per_year, s_0, prices)

    dt = 1/steps_per_year;
    rets_plus_1 = normrnd((1+mu)^dt,sigma.*sqrt(dt));
    %rets_plus_1 = normrnd((1+mu)^dt,sigma.*sqrt(dt),n_steps,n_scenarios);
    
    rets_plus_1(1,:) = 1;
    if prices
        result = s_0*cumprod(rets_plus_1);
    else
        result = rets_plus_1-1;
    end
   
end

    