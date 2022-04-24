clc
clear

%% IMPORT DONNEES 
data=xlsread("SP_Apple.xlsx","apple_sp");
y=data(:,3)';
m_date=x2mdate(data(1:end,1));
%% DEFINITION PARAMETRES POUR APPLICATION 1
T=length(y);
Q=ones(1,1,T);
R=ones(1,1,T);
QQ=ones(1,T);
RR=ones(1,T);

x0=0.6;
P0=1;

k=1;
beta=1;
alpha=0.5;

lambda_f=1;
c_f=0.11;
sigma_f=0.11;

lambda_h=1;
c_h=0.11;
sigma_h=0.11;

%% LANCEMENT FILTRES UKF ET SPPF
[prev_x_UKF] = UKF(@f,@h,Q,R,y,x0,P0,alpha,beta,k,lambda_f,c_f,sigma_f,lambda_h,c_h,sigma_h);
[prev_x_SPPF] = SPPF2(@f,@h,QQ,RR,y,x0,P0,alpha,beta,k,lambda_f,c_f,sigma_f,lambda_h,c_h,sigma_h);

%% GRAPHIQUES APPLICATION 1

prev_y_UKF=h(sqrt(prev_x_UKF),lambda_h,c_h,sigma_h)/10;
prev_y_SPPF=h(sqrt(prev_x_SPPF),lambda_h,c_h,sigma_h)/10;

figure(1)
hold on
plot(m_date,prev_y_UKF,'b')
plot(m_date,prev_y_SPPF)
plot(m_date,y)
datetick('x','dd/mm/yy');
legend('UKF','SPPF','APPLE');
hold off

%% DEFINITION DONNEES APPLICATION 2
K=1;
theta=0.04;
eta=0.8;
rho=0.4;
mu=0.6;
q=1;
r=1;

k=0.9;
beta=1;
alpha=0.01;


yy=log(y);

x0=0.5;
P0=0.11;

parametres=[K theta eta rho mu];

%% LANCEMENT FILTRES EKF ET UKF
[prev_x_UKF_vol] = UKF_forVOL2(@f_vol,@h_s,x0,P0,yy,parametres,alpha,k,beta);
[prev_x_EKF] = EKF_forVOL1(@f_vol,@h_s,x0,P0,R,Q,y,parametres);

%% GRAPHIQUES APPLICATION 2
figure(2)
plot(m_date(4:end),prev_x_UKF_vol(2:end),'b')
datetick('x','dd/mm/yy');

figure(3)
%EKF SINDUJA
plot(m_date(2:end-1),prev_x_EKF(2:end-1))
datetick('x','dd/mm/yy');

%% FONCTIONS

% FONCTION f
function [y] = f(x,lambda_f,c_f,sigma_f)
y=lambda_f.*(x-c_f).^2./sigma_f;
end

% FONCTION h
function [y] = h(x,lambda_h,c_h,sigma_h)
y=lambda_h.*(x-c_h).^2./sigma_h;
end

function [y] = f_vol(x,dlnSt,Bt,k,theta,eta,rho,mu)
y=x+k*(theta-x)+eta*rho*(dlnSt-(mu-0.5*x))+eta*sqrt(x*(1-rho^2))*Bt;
end

function [y] = h_s(S,x,Wt,mu)
y=S+(mu-0.5*x)+sqrt(x)*Wt;
end

% FONCTION SIGMA-POINTS
function [sigma_points] = SP(x_a,gamma,P)
% FONCTION GENERATRICE DE SIGMA-POINTS

% OUTPUT:
% sigma_points: matrice des sigma_points

% INPUT:
%x_a: vecteur d'état+bruits
%gamma: paramètre d'échelle
%P: matrice de variance-covariance d'état+bruits


M=gamma.*chol(P)'; % Calcul de la matrice racine carré
Y=repmat(x_a,1,length(x_a));

sigma_points=[x_a Y+M Y-M];
end

% FONCTION UKF
function [prev_x,prev_Px] = UKF(f,h,Q,R,y,x0,P0,alpha,beta,k,lambda_f,c_f,sigma_f,lambda_h,c_h,sigma_h)
% FONCTION UNSCENTED KALMAN FILTER 

% OUTPUT:
% prev_x: vecteur de prevision de la volatilité
% prev_Px: matrice de variance-covariance de prevision

% INPUT:
% f: fonction d'état
% h: fonction de mesure
% Q: matrice de variance-covariance des bruits de l'équation d'état
% R: matrice de variance-covariance des bruits de l'équation de mesure
% y: vecteur des observations
% x0: croyance initiale de l'état
% P0: croyance initiale de la matrice de variance-covariance de l'état
% alpha,beta,k: paramètre pour le filtre
% lambda_f,c_f,sigma_f: paramètre de la fonction d'état
% lambda_h,c_h,sigma_h: paramètre de la fonction de mesure

% ON CONSIDERE LE SSM SUIVANT :
% EQUATION D'ETAT : x(k+1)=f(x(k))+v(k)
% EQUATION D'OBSERVATION : y(k)=h(x(k))+n(k)

n=size(Q,1); % TAILLE VECTEUR D'ETAT
T=size(y,2); % NOMBRE D'OBSERVATIONS

% RECUPERATION DES DIMENSIONS
Lx=size(x0,1);
Lv=size(Q,2);
Ln=size(R,2);
L=Lx+Lv+Ln;

prev_x=zeros(n,T); % INITIALISATION PREVISION VECTEUR D'ETAT
prev_Px=zeros(Lx,Lx,T); % INITIALISATION PREVISION VARIANCE D'ETAT

lambda=alpha^2*(L+k)-L;
Wm=[lambda/(L+lambda) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS MOYENNE
Wc=[lambda/(L+lambda) + (1-alpha^2+beta) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS COVARIANCE

% BOUCLE SUR LE TEMPS
for t=1:T
    
    if t==1
        P=P0;
        x=x0;
    else
        P=prev_Px(:,:,t-1);
        x=prev_x(:,t-1);
    end
    
    % CONSTRUCTION DU VECTEUR Xa
    x_a=[x;zeros(Lv,1);zeros(Ln,1)];
    
    % CONSTRUCTION DE LA MATRICE Pa
    P_a=zeros(L,L);
    P_a(1:Lx,1:Lx)=P;
    P_a(1+Lx:Lx+Lv,1+Lx:Lx+Lv)=Q(:,:,t);
    P_a(1+Lx+Lv:Lx+Lv+Ln,1+Lx+Lv:Lx+Lv+Ln)=R(:,:,t);
    
    % CALCULE DES SIGMA-POINTS
    gamma=sqrt(L+lambda);
    sigma_points=SP(x_a,gamma,P_a);
   
    % PREDICTION STEP
    sigma_points_X_pred=zeros(Lv,2*L+1);
    for i=1:size(Q,2)
        for j=1:(2*L+1)
            sigma_points_X_pred(i,j)=f(sigma_points(i,j),lambda_f,c_f,sigma_f)+sigma_points(i+Lv,j);
        end
    end
    
    prev_x(:,t)=sigma_points_X_pred*Wm;
    prev_Px(:,:,t)=zeros(Lx,Lx);
    for i=0:2*L
        prev_Px(:,:,t)=prev_Px(:,:,t)+Wc(i+1)*(sigma_points_X_pred(:,i+1)-prev_x(:,t))*(sigma_points_X_pred(:,i+1)-prev_x(:,t))';
    end
    
    
    % INNOVATION
    sigma_points_Y_pred=zeros(Ln,2*L+1);
    for i=1:size(R,2)
        for j=1:(2*L+1)
            sigma_points_Y_pred(i,j)=h(sigma_points(i,j),lambda_h,c_h,sigma_h)+sigma_points(i+Lv+Ln,j);
        end
    end
    prev_y=sigma_points_Y_pred*Wm;
    
    error=y(1,t)-prev_y;
    
    % MEASUREMENT UPDATE STEP
    prev_Py=zeros(Ln,Ln);
    for i=0:2*L
        prev_Py=prev_Py+Wc(i+1)*(sigma_points_Y_pred(:,i+1)-prev_y)*(sigma_points_Y_pred(:,i+1)-prev_y)';
    end
    prev_PxPy=zeros(n,Ln);
    for i=0:2*L
        prev_PxPy=prev_PxPy+Wc(i+1)*(sigma_points_X_pred(:,i+1)-prev_x(:,t))*(sigma_points_Y_pred(:,i+1)-prev_y)';
    end
    
    % CALCUL DE LA MATRUCE DE GAIN DE KALMAN
    K=prev_PxPy*inv(prev_Py);
    
    % REPORT DES RESULTATS
    prev_x(:,t)=prev_x(:,t)+K*error;
    prev_Px(:,:,t)=prev_Px(:,:,t)-K*prev_Py*K';
    
end

end

% FONCTION UKF FOR SPPF
function [prev_x,prev_Px,vk,ek] = UKF_forSPPF(f,h,Q,R,y,x0,P0,alpha,beta,k,t)

% UNSCENTED KALMAN FILTER ADAPTE AU FILTRE SPPF

% ON CONSIDERE LE SSM SUIVANT :
% EQUATION D'ETAT : x(k+1)=f(x(k))+v(k)
% EQUATION D'OBSERVATION : y(k)=h(x(k))+n(k)

n=size(Q,1); % TAILLE VECTEUR D'ETAT
Lx=size(x0,1);
Lv=size(Q,1);
Ln=size(R,1);
L=Lx+Lv+Ln;

prev_x=zeros(n,1); % INITIALISATION PREVISION VECTEUR D'ETAT
prev_Px=zeros(Lx,Lx); % INITIALISATION PREVISION VARIANCE D'ETAT

lambda=alpha^2*(L+k)-L;
Wm=[lambda/(L+lambda) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS MOYENNE
Wc=[lambda/(L+lambda) + (1-alpha^2+beta) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS COVARIANCE

    
    x_a=[x0;zeros(Lv,1);zeros(Ln,1)];
    P_a=zeros(L,L);
    P_a(1:Lx,1:Lx)=P0;
    P_a(1+Lx:Lx+Lv,1+Lx:Lx+Lv)=Q;
    P_a(1+Lx+Lv:Lx+Lv+Ln,1+Lx+Lv:Lx+Lv+Ln)=R;
    
    % CALCULATE SIGMA-POINTS
    gamma=sqrt(L+lambda);
    sigma_points=SP(x_a,gamma,P_a);
   
    % PREDICTION STEP
    sigma_points_X_pred=zeros(Lv,2*L+1);
    for i=1:size(Q,2)
        for j=1:(2*L+1)
            sigma_points_X_pred(i,j)=f(sigma_points(i,j))+sigma_points(i+Lv,j);
        end
    end
    
    prev_x(:)=sigma_points_X_pred*Wm;
    prev_Px(:,:)=zeros(Lx,Lx);
    for i=0:2*L
        prev_Px(:,:)=prev_Px(:,:)+Wc(i+1)*(sigma_points_X_pred(:,i+1)-prev_x(:))*(sigma_points_X_pred(:,i+1)-prev_x(:))';
    end
    
    
    % INNOVATION
    sigma_points_Y_pred=zeros(Ln,2*L+1);
    for i=1:size(R,2)
        for j=1:(2*L+1)
            sigma_points_Y_pred(i,j)=h(sigma_points(i,j))+sigma_points(i+Lv+Ln,j);
        end
    end
    prev_y=sigma_points_Y_pred*Wm;
    
    error=y(1,t)-prev_y;
    
    % MEASUREMENT UPDATE STEP
    prev_Py=zeros(Ln,Ln);
    for i=0:2*L
        prev_Py=prev_Py+Wc(i+1)*(sigma_points_Y_pred(:,i+1)-prev_y)*(sigma_points_Y_pred(:,i+1)-prev_y)';
    end
    prev_PxPy=zeros(n,Ln);
    for i=0:2*L
        prev_PxPy=prev_PxPy+Wc(i+1)*(sigma_points_X_pred(:,i+1)-prev_x(:))*(sigma_points_Y_pred(:,i+1)-prev_y)';
    end
    
    K=prev_PxPy*inv(prev_Py);
    
    prev_x(:)=prev_x(:)+K*error;
    prev_Px(:,:)=prev_Px(:,:)-K*prev_Py*K';
    vk=prev_x(:)-f(x0);
    ek=y(1,t)-h(prev_x(:));

end

% FONCTION SPPF
function [prev_x_SPPF] = SPPF2(f,h,QQ,RR,y,x0,P0,alpha,beta,k,lambda_f,c_f,sigma_f,lambda_h,c_h,sigma_h)
% FONCTION SIGMA POINTS PARTICLE FILTER

% OUTPUT:
% prev_x: vecteur de prevision de la volatilité

% INPUT:
% f: fonction d'état
% h: fonction de mesure
% QQ: matrice de variance-covariance des bruits de l'équation d'état
% RR: matrice de variance-covariance des bruits de l'équation de mesure
% y: vecteur des observations
% x0: croyance initiale de l'état
% P0: croyance initiale de la matrice de variance-covariance de l'état
% alpha,beta,k: paramètre pour le filtre
% lambda_f,c_f,sigma_f: paramètre de la fonction d'état
% lambda_h,c_h,sigma_h: paramètre de la fonction de mesure

% ON CONSIDERE LE SSM SUIVANT :
% EQUATION D'ETAT : x(k+1)=f(x(k))+v(k)
% EQUATION D'OBSERVATION : y(k)=h(x(k))+n(k)

N=1000; % NOMBRE DE SIMULATIONS
T=size(y,2); % NOMBRE D'OBSERVATIONS

% RECUPERATION DES DIMENSIONS
Lx=size(x0,1);
Lv=size(QQ,1);
Ln=size(RR,1);
L=Lx+Lv+Ln;

x=zeros(N,1); % VECTEUR TIRAGE EN t=0
x_new=zeros(N,T); % VECTEUR TIRAGE EN SUITE A L'UKF

tpr=zeros(N,1); % TRANSITION PRIOR DISTRIBUTION
llh=zeros(N,1); % VRAISEMBLANCE
ppd=zeros(N,1); % PROPOSAL DISTRIBUTION

K=zeros(N,1); %VECTEUR DES COEFFICIENTS DE KALMAN

lambda=alpha^2*(L+k)-L;
Wm=[lambda/(L+lambda) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS MOYENNE
Wc=[lambda/(L+lambda) + (1-alpha^2+beta) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS COVARIANCE

w=zeros(N,T); % VECTEUR DES POIDS
prev_x_SPPF=zeros(T,1);
vk=zeros(N,1);

% DRAW N PARTICLES FILTERS x(i) ~ N(x0,P0)
for i=1:N
    a=randn;
    while a<0
        a=randn;
    end
    x(i)=x0+P0*randn;
end

prev_Px=zeros(T,N);
prev_Py=zeros(T,N);
prev_PxPy=zeros(T,N);
prev_x=zeros(T,1);


for k=1:T
% IMPORTANCE SAMPLING STEP
    for i=1:N
        
        if k==1
            PP=P0;
            xx=x(i);
        else
            PP=prev_Px(k-1,i);
            xx=prev_x(k-1);
        end
            
        
        % CALCULATE SIGMA-POINTS FOR PARTICLE
        x_a=[xx;0; 0];
        P_a=[PP 0 0; 0 QQ(1,k) 0; 0 0 RR(1,k)];
        
        gamma=sqrt(L+lambda);
        sigma_points=SP(x_a,gamma,P_a);
        
        % PREDICTION STEP
        sigma_points_X_pred=zeros(N,2*L+1);
        x_bar=zeros(N,1);
        
        for j=1:(2*L+1)
            sigma_points_X_pred(i,j)=f(sigma_points(1,j),lambda_h,c_h,sigma_h)+sigma_points(2,j);
        end
        
        x_bar(i)=sigma_points_X_pred(i,:)*Wm;
        for m=0:2*L
             prev_Px(k,i)=prev_Px(k,i)+Wc(m+1)*(sigma_points_X_pred(i,m+1)-x_bar(i))*(sigma_points_X_pred(i,m+1)-x_bar(i))';
        end
        
        % INNOVATION
        sigma_points_Y_pred=zeros(N,2*L+1);
        y_bar=zeros(N,1);
        error=zeros(N,1);
        
        for j=1:(2*L+1)
            sigma_points_Y_pred(i,j)=h(sigma_points(1,j),lambda_h,c_h,sigma_h)+sigma_points(3,j);
        end
        
        y_bar(i)=sigma_points_Y_pred(i,:)*Wm;
        error(i)=y(1,k)-y_bar(i);
        
        % MEASUREMENT UPDATE STEP
        for m=0:2*L
             prev_Py(k,i)=prev_Py(k,i)+Wc(m+1)*(sigma_points_Y_pred(i,m+1)-y_bar(i))*(sigma_points_Y_pred(i,m+1)-y_bar(i))';
        end
        
        for m=0:2*L
             prev_PxPy(k,i)=prev_PxPy(k,i)+Wc(m+1)*(sigma_points_X_pred(i,m+1)-x_bar(i))*(sigma_points_Y_pred(i,m+1)-y_bar(i))';
        end
        
        K(i)=prev_PxPy(k,i)*inv(prev_Py(k,i));
        
        x_bar(i)=x_bar(i)+K(i)*error(i);
        prev_Px(k,i)=prev_Px(k,i)-K(i)*prev_Py(k,i)*K(i)';
        
        % SAMPLE
        x_new(i,k)=x_bar(i)+prev_Px(k,i)*randn;
        if k==1
            vk(i)=x_new(i,k)-f(xx,lambda_f,c_f,sigma_f);
        else
            vk(i)=x_new(i,k)-f(x_new(i,k-1),lambda_f,c_f,sigma_f);
        end
        % TRANSITION PRIOR DISTRIBUTION
        
        tpr(i)=(2*pi)^(-0.5)*(QQ(1,k)^(-0.5))*exp(-0.5*vk(i)'*inv(QQ(:,k))*vk(i));
        
        % LIKELIHOOD DISTRIBUTION
        
        llh(i)=(2*pi)^(-0.5)*(det(RR(:,k))^(-0.5))*exp(-0.5*error(i)'*inv(RR(:,k))*error(i));
        
        % PROPOSAL DISTRIBUTION
        
        ppd(i)=(2*pi)^(-0.5)*(det(prev_Px(k,i))^(-0.5))*exp(-0.5*(x_new(i,k)-x_bar(i))'*inv(prev_Px(k,i))*(x_new(i,k)-x_bar(i)));
        
        if k==1
            w(i,k)=llh(i)*tpr(i)/ppd(i);
        else
            w(i,k)=w(i,k-1)*llh(i)*tpr(i)/ppd(i);
        end
    end
    
    somme=sum(w(:,k));
    w(:,k)=w(:,k)./somme;
    
    % RESAMPLING
    
    resampling=[w(:,k) x_new(:,k)];
    resampling=sort(resampling);
    resampling(1:1+0.2*N,:)=resampling(0.8*N:N,:);
    
    prev_x_SPPF(k)=sum(resampling(:,2))/N;
end

end

% FONCTION UKF POUR VOL
function [prev_x] = UKF_forVOL2(f_vol,h_s,x0,P0,y,parameters,alpha,k,beta)
% UNSCENTED KALMAN FILTER APPLIQUE A L'APPLICATION SUR LA VOLATILITE
% D'HESTON

% RECUPERATION DES PARAMETRES
kappa=parameters(1);
theta=parameters(2);
eta=parameters(3);
rho=parameters(4);
mu=parameters(5);


T=length(y);
L=3;
lambda=alpha^2*(L+k)-L;
prev_X_a=zeros(2*L+1,1);

x=x0;
x_a=[x;zeros(2,1)];
P_a=diag(ones(1,3));
P_a(1,1)=P0;

Wm=[lambda/(L+lambda) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS MOYENNE
Wc=[lambda/(L+lambda) + (1-alpha^2+beta) ; 0.5/(L+lambda)*ones(2*L,1)]; %VECTEUR DES POIDS COVAR

for t=2:T-1
    prev_x(t-1)=x;
    
    gamma=sqrt(L+lambda);
    X_a=SP(x_a,gamma,P_a);
    
    for l=1:2*L+1
        prev_X_a(l)=f_vol(X_a(1,l),y(1,t)-y(1,t-1),X_a(2,l),kappa,eta,theta,rho,mu);
    end
    
    x1=0;
    for l=1:2*L+1
        x1=x1+Wm(l)*prev_X_a(l);
    end
    
    P1=0;
    for l=1:2*L+1
        P1=P1+Wc(l)*(prev_X_a(l)-x1)^2;
    end
    
    yhat=0;
    
    for l=1:2*L+1

        prev_Y(l)=h_s(y(1,t),prev_X_a(l),X_a(3,l),mu);
        
        yhat=yhat+Wm(l)*prev_Y(l);
    end
    
    Pyy=0;
    for l = 1:2*L+1
        Pyy = Pyy + Wc(l)*((prev_Y(l)-yhat)^2);
    end
    
    Pxy = 0;
    for l = 1:2*L+1
        Pxy = Pxy + Wc(l)*(prev_X_a(l)-x1)*(prev_Y(l)-yhat);
    end
    
    Kalman=Pxy/Pyy;
    
    x = x1 + Kalman*(y(1,t+1)-yhat);    %State update
    P = P1 - ((Kalman^2)*Pyy);           %Covariance update

    x_a(1) = x;
    P_a(1,1) = P;
    
    
end
end

% FONCTION EKF POUR VOL
function [prev_x,prev_P] = EKF_forVOL1(f_vol,h_s,x0,P0,R,Q,y,parameters)

%f : fonction f(x) %%%%%%% f_vol
%h: fonction h(x)  %%%%%%% h_s
%vk,nk => bruits (process and measurement noise sequences)
%Q: matrice de variance covariance de v (process noise covariance)
%R: matrice de variance covariance de n (measurement noise covariance)

K1 = parameters(1,1);
theta1 = parameters(1,3);
eta1 = parameters(1,2);
rho1 = parameters(1,4);
mu1=parameters(1,5);

N=size(Q,1); % TAILLE VECTEUR D'ETAT
T=size(y,2); % NOMBRE D'OBSERVATIONS

prev_x=zeros(N,T); % INITIALISATION PREVISION VECTEUR D'ETAT 
prev_P=zeros(N,N,T); % INITIALISATION PREVISION VARIANCE D'ETAT

F=zeros(N,N,T);
G=zeros(N,N,T);
H=zeros(N,N,T);
D=zeros(N,N,T);

syms x Bt Wt dlnSt Bt k theta eta rho mu S;

prev_x(:,1)=x0;
prev_P(:,:,1)=P0;

for t=2:T-1
    
    % PREDICITION STEP
    %Calcul du Jacobien
    [y1] = f_vol(x,dlnSt,Bt,k,theta,eta,rho,mu);
    [y2] = h_s(S,x,Wt,mu);
    
    %Calculer la dérivée par rapport à x et v=Bt
    F_t=jacobian(y1,x);
    G_t=jacobian(y1,Bt);
    
    x_hat=prev_x(:,t-1);  %x^k-1
    P_hat=prev_P(:,:,t-1); %Pk-1|k-1
    
        
    %Calculer F(x_hat) et G(x_hat) 
   
    if isreal(F_t)==1
    F(:,:,t)=F_t;
    else
        F(:,:,t)=subs(F_t, [theta dlnSt eta rho k Bt x mu], [theta1 y(1,t)-y(1,t-1) eta1 rho1 K1 0 x_hat mu1]) ;
    end
    
    if isreal(G_t)==1
    G(:,:,t)=G_t;
    else
         G(:,:,t)=subs(G_t, [theta dlnSt eta rho k Bt x mu], [theta1 y(1,t)-y(1,t-1) eta1 rho1 K1 0 x_hat mu1]);
    end
    
    prev_x(:,t)=f_vol(prev_x(:,t),y(1,t)-y(1,t-1),0,K1,theta1,eta1,rho1,mu1);
    prev_P(:,:,t)=F(:,:,t)*P_hat*F(:,:,t)'+G(:,:,t)*R(:,:,t)*G(:,:,t)';
    
    
    % CORRECTION STEP
    
    %Calcul du Jacobien
    H_t=jacobian(y2,x);
    D_t=jacobian(y2,Wt);
    
    %%Calculer la dérivée par rapport à x et n=Wt
    if isreal(H_t)==1
    H(:,:,t)= H_t;
    else
         H(:,:,t)=subs(H_t,[S x Wt mu],[y(1,t) x_hat 0 mu1]);
    end
    
    if isreal(D_t)==1
    D(:,:,t)=D_t;
    else  
        D(:,:,t)=subs(D_t, [S x Wt mu], [y(1,t) x_hat 0 mu1]);
    end
        
    var_y_hat=H(:,:,t)*prev_P(:,:,t)*H(:,:,t)'+D(:,:,t)*R(:,:,t)*D(:,:,t)'; % VARIANCE DE LA PREVISION DE y
    Kalman=prev_P(:,:,t)*H(:,:,t)'*inv(var_y_hat); % MATRICE DE GAIN DE KALMAN
    error=y(:,t)-h_s(y(1,t),prev_x(:,t),0,mu1);

    prev_x(:,t)=prev_x(:,t)+Kalman*error;
    prev_P(:,:,t)=prev_P(:,:,t)-Kalman*H(:,:,t)*prev_P(:,:,t);
    
end

end