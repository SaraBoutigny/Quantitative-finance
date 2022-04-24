function H = GHE(Xt,q_min,q_max,q_step)
    % H (q_max x tau)
    % taus (t x 1)
    % moment_Xt (tau x 1)
    % Yt_temp (tau x 1)

% Initialisation
tmp = zeros(floor((q_max-q_min)/q_step)+1,length(Xt)-1);
taus = transpose(1:length(Xt)-1);
i=1;

for q=q_min:q_step:q_max
    
    %vecteur des moments
    moment_Xt = zeros(length(Xt)-1,1);
    
    for tau = 1:length(Xt)-1
        
        Yt_temp = Xt-lagmatrix(Xt,tau);
        
        %pour ne pas prendre les NaN
        Yt = abs(Yt_temp(tau+1:end));
        moment_Xt(tau) = moment_non_centre(Yt,q);
        
    end
    
    tmp(i,:) = log(moment_Xt/moment_Xt(1))./(q*log(taus));
    i=i+1;
end

H = tmp(:,2:end);
end
