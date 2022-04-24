function H_wgted = wGHE(Xt,q,delta_t,theta)
    % w0 (1 x 1)
    % Yt (t x 1)
    % wgted_moments (t x tau)
    % ws (delta_t x 1)
    % H_wgted ((t-delta_t) x tau)
    % taus (delta_t-1 x 1)

% Initialisation
w0 = (1-exp(-1/theta))/(1-exp(-delta_t/theta));
ws=w0*exp((-(0:delta_t-1))/theta);
wgted_moments = zeros(length(Xt)-2*delta_t+1,19);
H_wgted = zeros(length(Xt)-2*delta_t+1,18);
taus = transpose(1:19);

for t = delta_t:length(Xt)-delta_t
    
    for tau=1:19
        
        %initialisation
        Yt = abs(Xt(t-(0:delta_t-1)+tau) - Xt(t-(0:delta_t-1)));
        wgted_moment = ws * Yt.^q;
        
        wgted_moments(t-delta_t + 1,tau)=wgted_moment;
    end
   
end

%calcul de Kq
tmp = log(wgted_moments(:,:)./wgted_moments(:,1))... 
                             ./(q*log(taus)');
H_wgted(:,:) = tmp(:,2:end);

end