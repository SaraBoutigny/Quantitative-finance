clear;
clc;

%% Loading Data
load data_set ;

%% Definition of Xt
Xt_ASE = log(ASE_data);
Xt_NKY = log(NKY_data);
Xt_SENSEX = log(SENSEX_data);
Xt_SPX = log(SPX_data);
Xt_CAC = log(CAC_data);

%% Parameters -------------------------------------------------------------
Xt = Xt_CAC ;
Xt_dates = CAC_dates;
Pt = SPX_data;

%GHE 
q_min = 1;
q_max = 5;
q_step = 1;
tau = 9; % for plotting

% Weighted GHEs
q_wgted = 0.1;
theta = 250;
delta_t = 250;

%Standardized GHE
q1 = 0.1;
q2 = 4;
q1_prime = 0.1;
q2_prime = 1;
q_step_prime = 0.04;

%% Initialisation ---------------------------------------------------------
w0 = (1-exp(-1/theta))/(1-exp(-delta_t/theta));
ws=w0*exp((-(0:delta_t-1))/theta);

%% GHE --------------------------------------------------------------------
H = GHE(Xt,q_min,q_max,q_step);

% Plotting Hq according to q, for a given tau

tau=2;
B = (H(1,tau)-H(2,tau))/(1-2);
A = H(1,tau)-B*1;

figure;
plot (H(:,tau));
title(['GHE for tau = ', num2str(tau)] ) ;
xlabel('q');
ylabel('Hurst Exponent');

% Plotting Hq according to time
figure;
plot (datetime(Xt_dates(3:end)),H');
title(['GHE according to time '] ) ;
xlabel('Time');
ylabel('Hurst Exponent');

%% WEIGHTED GHEs ----------------------------------------------------------
H_wgted= wGHE(Xt, q_wgted, delta_t,theta);

% Plotting
figure;
plot (datetime(Xt_dates(delta_t:end-delta_t)),H_wgted);
title(['Weighted GHE for q = ', num2str(q_wgted), ', delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');
%line([0 length(Xt)],[1 1],'Color', 'black','LineStyle','--','LineWidth',0.1);
line(datetime(Xt_dates(delta_t:end-delta_t)),ones(size(H_wgted,1),1)*0.5,'Color', 'black','LineStyle','--','LineWidth',0.1);


%% WEIGHTED GHEs - VOLATILITY CALCULATION
V=zeros(length(Xt)-delta_t+1,1);

for t = delta_t:length(Xt)
    X_tau_plus_1 =  Xt(t-delta_t+2:t);
    X_tau = Xt(t-delta_t+1:t-1);
    V(t-delta_t+1,1)=std(log(X_tau_plus_1./X_tau).*transpose(ws(2:end)));
    
end


%% SURROGATE SERIE --------------------------------------------------------
[Pt_brownian]=gbm2(0, V, 12, Pt(delta_t), true);
plot(Pt_brownian);
Xt_brownian = log(Pt_brownian);

%Plotting the brownian motion
figure;
plot(Xt_brownian);

% Calculating the metrics for the surrogate serie
H_wgted_surr = wGHE(Xt_brownian, q_wgted, delta_t,theta);
std_surr = std(H_wgted_surr);


%% STANDARDIZATION --------------------------------------------------------

% Weighted GHEs standardization
H_prime = (H_wgted - 0.5)./std_surr;
plot(H_prime)

% Width of multiscaling (W) standardization
H_q1 = wGHE(Xt, q1, delta_t,theta);
H_q2 = wGHE(Xt, q2, delta_t,theta);
H_q1_surr = wGHE(Xt_brownian, q1, delta_t,theta);
H_q2_surr = wGHE(Xt_brownian, q2, delta_t,theta);

W = H_q1 - H_q2 ;
std_W_surr = sqrt(std(H_q1_surr).^2+std(H_q2_surr).^2);

W_prime = W./std_W_surr;

% Curvature of multiscaling (B) standardization
Ht = GHE(Xt,q1_prime,q2_prime,q_step_prime);
Bt = zeros(size(Ht,2),1);

    % Loop on tau to get Bt for each tau with Least Squares Linear Fit
for t=1:size(Ht,2)
    P = polyfit(q1_prime:q_step_prime:q2_prime,Ht(:,t)',1);
    Bt(t) = P(1);

end

h_surr_test = GHE(Xt_brownian,q1_prime,q2_prime,q_step_prime);
Bt_surr = zeros(size(h_surr_test,2),1);

    % Loop on tau to get Bt for each tau with Least Squares Linear Fit
for t=1:size(h_surr_test,2)
    P = polyfit(q1_prime:q_step_prime:q2_prime,h_surr_test(:,t)',1);
    Bt_surr(t) = P(1);
    
end

B_prime = Bt ./ std(Bt_surr(2:end));

%% Tableau de Statistiques ------------------------------------------------
% mean values of H1, H_0.1, H_4, E[|W|] et B
theta = 750;
H01_w = wGHE(Xt, 0.1, delta_t,theta);
H1_w = wGHE(Xt, 1, delta_t,theta);
H2_w = wGHE(Xt, 2, delta_t,theta);
H3_w = wGHE(Xt, 3, delta_t,theta);
H4_w = wGHE(Xt, 4, delta_t,theta);

H01_w_surr = wGHE(Xt_brownian, 0.1, delta_t,theta);
H1_w_surr = wGHE(Xt_brownian, 1, delta_t,theta);
H2_w_surr = wGHE(Xt_brownian, 2, delta_t,theta);
H3_w_surr = wGHE(Xt_brownian, 3, delta_t,theta);
H4_w_surr = wGHE(Xt_brownian, 4, delta_t,theta);

std01_surr = std(H01_w_surr);
std1_surr = std(H1_w_surr);
std2_surr = std(H2_w_surr);
std3_surr = std(H3_w_surr);
std4_surr = std(H4_w_surr);

H01_prime = (H01_w - 0.5)./std01_surr;
H1_prime = (H1_w - 0.5)./std1_surr;
H2_prime = (H1_w - 0.5)./std1_surr;
H3_prime = (H1_w - 0.5)./std1_surr;
H4_prime = (H4_w - 0.5)./std4_surr;


%% PLOTS ------------------------------------------------------------------

% Weighted GHEs on surrogate serie
figure;
plot (datetime(Xt_dates(2*delta_t-1:end-delta_t)),H_wgted_surr);
title(['Weighted GHE of Surrogate series for q = ', num2str(q_wgted), ', delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');
%line(datetime(Xt_dates(2*delta_t-1:end-delta_t)),ones(size(H_wgted_surr,1),1),'Color', 'black','LineStyle','--','LineWidth',0.1);
line(datetime(Xt_dates(2*delta_t-1:end-delta_t)),ones(size(H_wgted_surr,1),1)*0.5,'Color', 'black','LineStyle','--','LineWidth',0.1);

% the wGHE of the randomly generated surrogates are evenly distributed
% around 0.5
% il y a des points où H_4 décroit soudainement par rapport à H_0.1
%=> correspond aux évènements extrèmes de queues épaisses 

% Standardized Weighted GHEs for q=0.1;1;2;4 
figure;

plot (H01_prime,'color','red');
hold all;
plot (H4_prime,'color','blue');
hold all;
plot (H1_prime,'color','black');
hold all;
plot (H2_prime,'color','magenta');
hold all;
plot (H3_prime,'color','cyan');

title(['Weighted GHE for q = ', num2str(q_wgted), ', delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');
%ylim([0 1])
line([0 length(Xt)],[1 1],'Color', 'black','LineStyle','--','LineWidth',0.1);
line([0 length(Xt)],[0.5 0.5],'Color', 'black','LineStyle','--','LineWidth',0.1);

    %Les dates pour lesquelles les valeurs sont quasi-identiques => uniscalaire 
    %et les dates où les valeurs divergents => multiscalaire

% Print W_prime 
figure;

plot (datetime(Xt_dates(delta_t:end-delta_t)),W_prime,'color','red');

title(['W_p_r_i_m_e  , delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');

line(datetime(Xt_dates(delta_t:end-delta_t)),ones(size(W_prime,1),1)*1.64,'Color', 'black','LineStyle','--','LineWidth',0.1);
line(datetime(Xt_dates(delta_t:end-delta_t)),ones(size(W_prime,1),1)*0.32,'Color', 'black','LineStyle','--','LineWidth',0.1);

% Print B_prime 
figure;

plot (B_prime,'color','red');

title(['B_p_r_i_m_e  , delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');

line([0 length(Xt)],[1.64 1.64],'Color', 'black','LineStyle','--','LineWidth',0.1);
line([0 length(Xt)],[0.32 0.32],'Color', 'black','LineStyle','--','LineWidth',0.1);

%%

figure;
plot (H01_w,'color','black');
hold all;
plot (H4_w,'color','red');

title(['Weighted GHE for q = ', num2str(q_wgted), ', delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');
ylim([0 1])
line([0 length(Xt)],[1 1],'Color', 'black','LineStyle','--','LineWidth',0.1);
line([0 length(Xt)],[0.5 0.5],'Color', 'black','LineStyle','--','LineWidth',0.1);

%On remarque qu’à certaines dates, les wGHEs évoluent différemment, voire 
%avec des trends opposés => cela correspond à des évènements de queues 
%épaisses (large baisse / augmentation)

%%
figure;
plot (H_prime,'color','black');
hold all;

title(['Weighted GHE for q = ', num2str(q_wgted), ', delta_t = ', num2str(delta_t), ', theta = ', num2str(theta)] ) ;
xlabel('Days');
ylabel('Hurst Exponent');
%ylim([0 1])
line([0 length(Xt)],[1 1],'Color', 'black','LineStyle','--','LineWidth',0.1);
line([0 length(Xt)],[0.5 0.5],'Color', 'black','LineStyle','--','LineWidth',0.1);


%% TP identification ------------------------------------------------------

gamma_prime = W_prime;
phi_l = 0.32;
phi_h = 1.64;

% Fit H_prime avec un polynome du second degré
H_prime_fit = zeros(size(H_prime,1),1);

y_mean = mean(H_prime,2);

plot(H_prime,'color','black')
hold on

P = polyfit(1:size(H_prime,1),y_mean',70);
    
ywant = (1:0.01:size(H_prime));
xwant = polyval(P,ywant);
plot(ywant,xwant,'color','white')




