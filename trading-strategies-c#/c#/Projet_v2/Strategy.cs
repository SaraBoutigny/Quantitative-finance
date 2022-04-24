using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_v2
{
    // *************************************************************************************************************************************************************
    // CLASSE MERE : STRATEGY 
    // Cette classe contient des méthodes et attibuts communs aux différents types de stratégies. Il s'agit notamment des calculs de performances, de performances
    // cumulées
    // *************************************************************************************************************************************************************

    class Strategy
    {
        // Attributs ==============================================================================================================================================
        protected double[,] _tbl_rtn;
        protected double[,] _cum_tbl_rtn;
        protected double[,] _tbl_total_rtn;
        protected double[,] _tbl_total_cum_rtn;
        protected double _vol;
        protected double _rtn;
        protected double _rtn_vol_ratio;
        protected MatriceFinanciere _matfin;
        protected int[,] _signal;
        protected double[,] _movavg;
        protected int _n;

        // Attributs publics ====================================================================================================================================
        public double Return
        {
            get { return _rtn; }
            set { _rtn = value; }
        }
        public double Vol
        {
            get { return _vol; }
            set { _vol = value; }
        }

        public double[,] CumReturn
        {
            get { return _tbl_total_cum_rtn; }
            set { _tbl_total_cum_rtn = value; }
        }
        public object[,] DateRange
        {
            get { return this._matfin.Dates; }
            set { this._matfin.Dates = value; }
        }

        // Constructeurs ==========================================================================================================================================
        public Strategy(MatriceFinanciere Data)
        {
            this._matfin = Data;

        }

        // Méthodes ================================================================================================================================================

        protected void ComputeReturn()
        {
            //-------------------------------------------------------------------------------------------------------------------------------------------------------
            //Méthode qui permet de calculer les performances d'une stratégie de trading, à partir des performances des actifs et du signal qui indique à quelle date
            //acheter ou vendre.
            //-------------------------------------------------------------------------------------------------------------------------------------------------------

            this._tbl_rtn = new double[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                for (int i = 0; i < this._matfin.NbLignes; i++)
                {

                    this._tbl_rtn[i, j] = ((Double.NaN.CompareTo(this._matfin.TblReturn[i, j]) == 0) ? 0 : this._matfin.TblReturn[i, j]) * this._signal[i, j];
                    Double.NaN.CompareTo(this._matfin.TblReturn[i, j]);
                }
            }
        }

        protected void ComputeCumulativeReturn()
        {
            //---------------------------------------------------------------------------------------------------------------------------
            //Méthode qui permet de calculer les performances cumulées d'une stratégie de trading, à partir des performances de celle-ci.
            //---------------------------------------------------------------------------------------------------------------------------

            this._cum_tbl_rtn = new double[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                for (int i = 1; i < this._matfin.NbLignes; i++)
                {

                    this._cum_tbl_rtn[i, j] = (1 + this._cum_tbl_rtn[i - 1, j]) * (1 + this._tbl_rtn[i, j]) - 1;

                }


            }
        }
        protected void ComputeTotalReturn()
        {
            //-------------------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui, à partir de la performance d'une stratégie, permet de calculer le return total en considérant la matrice de données financières 
            // comme étant un portefeuille équipondéré
            //-------------------------------------------------------------------------------------------------------------------------------------------------------

            this._tbl_total_rtn = new double[this._matfin.NbLignes,1];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                for (int i = 0; i < this._matfin.NbLignes; i++)
                {

                    this._tbl_total_rtn[i,0] = this._matfin.Avg(i, i, 0, this._matfin.NbColonnes-1, this._tbl_rtn);
             
                }
            }
        }

        protected void ComputeTotalCumulativeReturn()
        {
            //-------------------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui, à partir de la performance totale d'une stratégie, permet de calculer le return total cumulé en considérant la matrice de données financières 
            // comme étant un portefeuille équipondéré
            //-------------------------------------------------------------------------------------------------------------------------------------------------------

            this._tbl_total_cum_rtn = new double[this._matfin.NbLignes,1];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                for (int i = 1; i < this._matfin.NbLignes; i++)
                {

                    this._tbl_total_cum_rtn[i,0] = (1 + this._tbl_total_cum_rtn[i - 1,0]) * (1 + this._tbl_total_rtn[i,0]) - 1;

                }
            }
            // Récupération du rendement total de la stratégie
            this._rtn = this._tbl_total_cum_rtn[this._matfin.NbLignes - 1, 0];
        }

        protected void ComputeTotalVol()
        {
            //-------------------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui, à partir de la performance d'une stratégie, permet de calculer le return total en considérant la matrice de données financières 
            // comme étant un portefeuille équipondéré
            //-------------------------------------------------------------------------------------------------------------------------------------------------------

            this._vol = this._matfin.Volatility(0, this._matfin.NbLignes - 1, 0, 0, this._tbl_total_rtn) * Math.Sqrt(252);
            this._rtn_vol_ratio = this._tbl_total_cum_rtn[this._matfin.NbLignes - 1, 0] / this._vol;


        }

    }

    // *******************************************************************************************************************************************************************
    // CLASSE MOVINGAVG HERITEE DE STRATEGY
    // Cette classe correspond à une stratégie de trading appelée moyenne mobile. Elle contient des éléments spécifiques à cette stratégies. Il s'agit notamment du calcul
    // d'une moyenne mobile glissante pour chaque actif, et du calcul du signal d'achat. 
    // *******************************************************************************************************************************************************************
    class MovingAvg : Strategy
    {

        private bool _momentum;

        // Constructeurs ========================================================================================================================================
        public MovingAvg(MatriceFinanciere Data, int n, bool bMomentum = true) : base(Data)
        {

            this._n = n;
            this._momentum = bMomentum;
            ComputeMovAvg();
            ComputeSignal();
            ComputeReturn();
            ComputeCumulativeReturn();
            ComputeTotalReturn();
            ComputeTotalCumulativeReturn();
            ComputeTotalVol();
        }

        // Méthodes ==============================================================================================================================================
        private void ComputeMovAvg()
        {
            //--------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer la moyenne mobile à n jours d'une série financière, et ce, sur plusieurs dates de sorte à obtenir une série. 
            //--------------------------------------------------------------------------------------------------------------------------------------------

            int n = this._n;
            int k = 0;
            this._movavg = new double[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                k = 0;
                while (this._matfin.Price[k, j] == 0)
                {
                    k++;
                }

                for (int i = k + n - 1; i < this._matfin.NbLignes; i++)
                {
                    if (i < n) { this._movavg[i, j] = Double.NaN; }

                    this._movavg[i, j] = this._matfin.Avg(i - (n - 1), i, j, j,this._matfin.Price);
                }
            }
        }

        private void ComputeSignal()
        {
            //------------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer le signal d'achat dans le cadre d'une stratégie moyenne mobile à n jours. Cette méthode tient également compte
            // du Paramètre Momentum renseigné lors de l'instanciation de l'objet MovingAvg. 
            // En effet, dans le cadre d'une stratégie moyenne mobile momentum, le signal d'achat intervient dès lors que le cours est au dessus de sa moyenne
            // mobile à n jours. Dans le cas d'une stratégie momentum contrariante, l'inverse se produit : on achète lorsque le cours est en-dessous de sa 
            // moyenne mobile à n jours.
            // NB : il faut intégrer dans le calcul du signal un temps de latence équivalent à une période (ici 1 jour) étant donné que l'achat/vente se produit
            // le lendemain du constat que le cours est au-dessus/en-dessous. 
            //------------------------------------------------------------------------------------------------------------------------------------------------

            this._signal = new int[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {
                for (int i = 0; i < this._matfin.NbLignes - 1; i++)
                {
                    if (this._movavg[i, j] != 0)
                    {
                        this._signal[i + 1, j] = (this._momentum) ? Convert.ToInt32(this._matfin.Price[i, j] > this._movavg[i, j]) :
                                                                    Convert.ToInt32(this._matfin.Price[i, j] < this._movavg[i, j]);
                    }

                }
            }
        }

    }

    // *******************************************************************************************************************************************************************
    // CLASSE BOLLINGER HERITEE DE STRATEGY
    // Cette classe correspond à une stratégie de trading appelée Bollinger. Elle contient des éléments spécifiques à cette stratégie. Il s'agit notamment du calcul
    // d'une moyenne mobile glissante pour chaque actif ainsi que des bandes de bollinger, et du calcul du signal d'achat. 
    // *******************************************************************************************************************************************************************
    class Bollinger : Strategy
    {
        private int _n;
        private bool _momentum;
        private double[,] _upper_band;
        private double[,] _lower_band;
        private int _nb_ecartyp;

        // Constructeurs ========================================================================================================================================
        public Bollinger(MatriceFinanciere Data, int n, int nb_ecartyp = 1, bool bMomentum = true) : base(Data)
        {

            this._n = n;
            this._momentum = bMomentum;
            this._nb_ecartyp = nb_ecartyp;
            ComputeMovAvg();
            ComputeBands();
            ComputeSignal();
            ComputeReturn();
            ComputeCumulativeReturn();
            ComputeTotalReturn();
            ComputeTotalCumulativeReturn();
            ComputeTotalVol();
        }

        // Méthodes ==============================================================================================================================================
        private void ComputeMovAvg()
        {
            //--------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer la moyenne mobile à n jours d'une série financière, et ce, sur plusieurs dates de sorte à obtenir une série. 
            //--------------------------------------------------------------------------------------------------------------------------------------------

            int n = this._n;
            int k = 0;
            this._movavg = new double[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                k = 0;
                while (this._matfin.Price[k, j] == 0)
                {
                    k++;
                }

                for (int i = k + n - 1; i < this._matfin.NbLignes; i++)
                {
                    if (i < n) { this._movavg[i, j] = Double.NaN; }

                    this._movavg[i, j] = this._matfin.Avg(i - (n - 1), i, j, j, this._matfin.Price);
                }
            }
        }

        private void ComputeBands()
        {
            //--------------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer les bandes de Bollinger à n jours d'une série financière, et ce, sur plusieurs dates de sorte à obtenir une série.
            // Les bandes de Bollinger se calculent en ajoutant à la moyenne mobile 1 ou plusieurs écarts-types, eux aussi calculés de façon glissante. 
            // Cette stratégie permet ainsi de tenir compte également de la volatilité de l'actif.
            //--------------------------------------------------------------------------------------------------------------------------------------------------

            int n = this._n;
            int k = 0;
            double ecartyp = 0;

            this._upper_band = new double[this._matfin.NbLignes, this._matfin.NbColonnes];
            this._lower_band = new double[this._matfin.NbLignes, this._matfin.NbColonnes];


            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {

                k = 0;
                while (this._matfin.Price[k, j] == 0)
                {
                    k++;
                }

                for (int i = k + n - 1; i < this._matfin.NbLignes; i++)
                {
                    if (i < n)
                    {
                        this._upper_band[i, j] = Double.NaN;
                        this._lower_band[i, j] = Double.NaN;
                    }
                    if (j == 3)
                    {
                        int x = 0;
                    }
                    ecartyp = this._matfin.Volatility(i - (n - 1), i, j, j,this._matfin.Price);
                    this._upper_band[i, j] = this._movavg[i, j] + (double)this._nb_ecartyp * ecartyp;
                    this._lower_band[i, j] = this._movavg[i, j] - (double)this._nb_ecartyp * ecartyp;
                }
            }
        }


        private void ComputeSignal()
        {
            //------------------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer le signal d'achat dans le cadre d'une stratégie moyenne mobile à n jours. Cette méthode tient également compte
            // du Paramètre Momentum et nb_ecartyp, renseignés lors de l'instanciation de l'objet Bollinger.
            // En effet, dans le cadre d'une stratégie Bollinger momentum, le signal d'achat intervient dès lors que le cours est au dessus de la bande haute.
            //Dans le cas d'une stratégie momentum contrariante, l'inverse se produit : on achète lorsque le cours est en-dessous de la bande basse.
            // NB : il faut intégrer dans le calcul du signal un temps de latence équivalent à une période (ici 1 jour) étant donné que l'achat/vente se produit
            // le lendemain du constat que le cours est au-dessus/en-dessous. 
            //------------------------------------------------------------------------------------------------------------------------------------------------

            this._signal = new int[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {
                for (int i = 0; i < this._matfin.NbLignes - 1; i++)
                {
                    if (this._movavg[i, j] != 0)
                    {
                        if (this._matfin.Price[i, j] > this._upper_band[i, j])
                        {
                            this._signal[i + 1, j] = (this._momentum) ? 1 : 0;
                        }
                        else if (this._matfin.Price[i, j] < this._lower_band[i, j])
                        {
                            this._signal[i + 1, j] = (this._momentum) ? 0 : 1;
                        }
                        else
                        {
                            this._signal[i + 1, j] = this._signal[i, j];
                        }

                    }

                }
            }
        }
    }

    // *******************************************************************************************************************************************************************
    // CLASSE BOLLINGER HERITEE DE STRATEGY
    // Cette classe correspond à une stratégie de trading appelée Bollinger. Elle contient des éléments spécifiques à cette stratégie. Il s'agit notamment du calcul
    // d'une moyenne mobile glissante pour chaque actif ainsi que des bandes de bollinger, et du calcul du signal d'achat. 
    // *******************************************************************************************************************************************************************
    class RSI : Strategy
    {
        
        private int _upper_band;
        private int _lower_band;
        private double[,] _change;
        private double[,] _gain;
        private double[,] _loss;
        private double[,] _avg_gain;
        private double[,] _avg_loss;
        private double[,] _RS;
        private double[,] _RSI;

        // Constructeurs ========================================================================================================================================
        public RSI(MatriceFinanciere Data, int n, int upper_bound, int lower_bound) : base(Data)
        {

            this._n = n;
            this._upper_band = upper_bound;
            this._lower_band = lower_bound;

            ComputePnL();
            ComputeRSI();
            ComputeSignal();
            ComputeReturn();
            ComputeCumulativeReturn();
            ComputeTotalReturn();
            ComputeTotalCumulativeReturn();
            ComputeTotalVol();
        }

        // Méthodes =======================================================================================================================================
        private void ComputePnL()
        {
            //------------------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer les matrices de gain et de perte. Elle consiste à calculer la variation arithmétique du prix et à 
            // regrouper dans une matrice de gains les variations positives et dans une matrice de pertes les variations négatives
            //------------------------------------------------------------------------------------------------------------------------------------

            this._change = new double[this._matfin.NbLignes, this._matfin.NbColonnes];
            this._gain = new double[this._matfin.NbLignes, this._matfin.NbColonnes];
            this._loss = new double[this._matfin.NbLignes, this._matfin.NbColonnes];

            // Calcul de la variation arithmétique des prix (change = p(i)-p(i-1))


            for (int i = 1; i < this._matfin.NbLignes; i++)
            {
                for (int j = 0; j < this._matfin.NbColonnes; j++)
                {


                    if (i == 0 || (Double.NaN.CompareTo(this._matfin.Price[i, j]) == 0 || Double.NaN.CompareTo(this._matfin.Price[i - 1, j]) == 0)
                        || this._matfin.Price[i, j] == 0 || this._matfin.Price[i - 1, j] == 0)
                    {
                        this._change[i, j] = 0;
                    }
                    else
                    {
                        this._change[i, j] = this._matfin.Price[i, j] - this._matfin.Price[i - 1, j];
                        if (this._change[i, j] > 0)
                        {
                            this._gain[i, j] = this._change[i, j];

                        }
                        else
                        {
                            this._loss[i, j] = this._change[i, j];
                        }
                    }


                }
            }

        }

        private void ComputeRSI()
        {
            //-------------------------------------------------------------------------------------------------------------------------------------
            // Cette méthode permet de calculer les matrices de gains moyens et de pertes moyennes glissantes sur n jours,
            // nécessaires pour le calcul de l'indicateur RSI ensuite. 
            //-------------------------------------------------------------------------------------------------------------------------------------

            // initialisation
            double avg_loss = 0;
            double avg_gain = 0;
            double RS = 0;

            this._RSI = new double[this._matfin.NbLignes, this._matfin.NbColonnes];

            int n = this._n;
            int k = 0;
            
            // boucle sur les colonnes (=sur les différents titres)
            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {
                // recherche de la cellule à partir de laquelle on calcule la moyenne mobile des gains et pertes
                // étant donné que les valeurs n'ont pas toutes une cotation dès le départ
                k = 0;
                while (this._matfin.Price[k, j] == 0)
                {
                    k++;
                }

                if (j == 0)
                {
                    int a = 2;
                }
                // calcul des deux matrices de moyennes mobiles : la matrice des gains moyens et des pertes moyennes
                // boucle sur les lignes (=sur les dates)
                for (int i = k + n; i < this._matfin.NbLignes; i++)
                {

                    avg_loss = Math.Abs(this._matfin.Avg(i - n + 1, i, j, j, this._loss));
                    avg_gain = this._matfin.Avg(i - n + 1 , i, j, j, this._gain);

                    RS = avg_gain/avg_loss;
                    this._RSI[i, j] = 100 - 100 / (1 + RS);

                }
                avg_loss = 0;
                avg_gain = 0;
                RS = 0;
            }
            
        }


        public void ComputeSignal()
        {
            this._signal = new int[this._matfin.NbLignes, this._matfin.NbColonnes];

            for (int j = 0; j < this._matfin.NbColonnes; j++)
            {
                for (int i = 0; i < this._matfin.NbLignes - 1; i++)
                {
                    if (this._RSI[i, j] != 0)
                    {
                        if (this._RSI[i+1, j] > this._lower_band && this._RSI[i, j] < this._lower_band)
                        {
                            this._signal[i + 1, j] = 1;
                        }
                        else if (this._RSI[i+1, j] < this._upper_band && this._RSI[i, j] > this._upper_band)
                        {
                            this._signal[i + 1, j] = 0;
                        }
                        else
                        {
                            this._signal[i + 1, j] = this._signal[i, j];
                        }

                    }

                }
            }
        }

    }
}
