using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_v2
{
    class MatriceFinanciere : Matrice
    {
        // Attributs ==============================================================================================================================================
        private double[,] _price;
        private double[,] _tbl_rtn;
        private object[,] _dates;
        private object[,] _head;
        private double[,] _tbl_tot_return;
        private double[,] _tbl_tot_cum_return;
        private double _return;

        // Attributs publics ====================================================================================================================================
        public double[,] Price
        {
            get { return _price; }
            set { _price = value; }
        }
        public object[,] Dates
        {
            get { return _dates; }
            set { _dates = value; }
        }
        public object[,] Head
        {
            get { return _head; }
            set { _head = value; }
        }
        public double[,] TotalCumReturn
        {
            get { return _tbl_tot_cum_return; }
            set { _tbl_tot_cum_return = value; }
        }
        public double Return
        {
            get { return _return; }
            set { _return = value; }
        }

        public double[,] TblReturn
        {
            get { return _tbl_rtn; }
            set { _tbl_rtn = value; }
        }

        // Constructeurs ==========================================================================================================================================
        public MatriceFinanciere(int n, int p) : base(n, p)
        {

        }

        public MatriceFinanciere(object[,] tableau) : base(tableau)
        {
            //---------------------------------------------------------------------------------------------
            // Constructeur à partir des données brutes : on sépare les données : en-têtes, dates, et cours
            //Séparation des dates, en-têtes et prix
            //---------------------------------------------------------------------------------------------

            this.SeparateData();
        }

        // Méthodes ==========================================================================================================================================

        public override Matrice SubMatrix(int n1, int n2, int p1, int p2)
        {
            //-----------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet d'extraire un sous-matrice financière en spécifiant les indices de début et de fin en ligne et en colonne
            //-----------------------------------------------------------------------------------------------------------------------------

            MatriceFinanciere mat_temp = new MatriceFinanciere(n2 - n1 + 1, p2 - p1 + 1);

            int k = 0;
            int l = 0;

            for (int i = n1; i <= n2; i++)
            {
                for (int j = p1; j <= p2; j++)
                {
                    mat_temp._data[k, l] = this._data[i, j];
                    l++;
                }
                k++;
                l = 0;
            }

            return mat_temp;

        }

        public void ComputeReturns()
        {
            //--------------------------------------------------------------------------------------------------------------
            //Méthode qui permet de calculer les performances et performances cumulées de la matrice de données financières
            //La méthode remplit l'attribut Return préexistant à partir du tableau Price
            //--------------------------------------------------------------------------------------------------------------

            this._tbl_rtn = new double[this.NbLignes, this.NbColonnes];
            this._tbl_tot_return = new double[this.NbLignes, 1];
            this._tbl_tot_cum_return = new double[this.NbLignes,1];

            //Calcul des rendements de chaque actif
            for (int i = 1; i < this.NbLignes; i++)
            {
                for (int j = 0; j < this.NbColonnes; j++)
                {

                    // Si le prix est manquant, on report 0 comme return
                    if (i == 0 || (Double.NaN.CompareTo(this._price[i, j]) == 0 || Double.NaN.CompareTo(this._price[i - 1, j]) == 0)
                        || this._price[i, j] == 0 || this._price[i - 1, j] == 0)
                    {
                        this._tbl_rtn[i, j] = 0;
                    }
                    else
                    {
                        this._tbl_rtn[i, j] = this._price[i, j] / this._price[i - 1, j] - 1;
                    }


                }
            }
            
            // Calcul des rendements du portefeuilles dans sa globalité
            for (int j = 0; j < this.NbColonnes; j++)
            {

                for (int i = 0; i < this.NbLignes; i++)
                {
                    this._tbl_tot_return[i, 0] = this.Avg(i, i, 0, this.NbColonnes - 1, this._tbl_rtn);
                }
            }

            //Calcul de la performance cumulée du portefeuille
            for (int j = 0; j < this.NbColonnes; j++)
            {
                for (int i = 1; i < this.NbLignes; i++)
                {
                    this._tbl_tot_cum_return[i, 0] = (1 + this._tbl_tot_cum_return[i - 1, 0]) * (1 + this._tbl_tot_return[i, 0]) - 1;
                }
            }

            // Récupération du dernier point de performance cumulée, équivalent à la performance de l'indice sur toute la période
            this._return = this._tbl_tot_cum_return[this.NbLignes - 1, 0];
        }


        private void SeparateData()
        {
            //---------------------------------------------------------------------------------------------------------------------
            //Méthode utilisée dans le constructeur, qui permet lors de la création d'une matrice financière à partir d'un tableau,
            //de séparer les données entre en-têtes, tableau de prix, et dates
            //---------------------------------------------------------------------------------------------------------------------

            // Séparation des données en extrayant des sous-matrices
            MatriceFinanciere mEnTetes = (MatriceFinanciere)this.SubMatrix(0, 0, 1, this.NbColonnes - 1);
            MatriceFinanciere mDates = (MatriceFinanciere)this.SubMatrix(1, this.NbLignes - 1, 0, 0);
            MatriceFinanciere mData = (MatriceFinanciere)this.SubMatrix(1, this.NbLignes - 1, 1, this.NbColonnes - 1);

            this.Head = mEnTetes._data;
            this.Dates = mDates._data;
            this.Price = mData.ConvertDataToDouble();

            this.NbLignes = this.NbLignes - 1;
            this.NbColonnes = this.NbColonnes - 1;

        }

        // Fonctions ==================================================================================================================================
        private double[,] ConvertDataToDouble()
        {
            //------------------------------------------------------------------
            // Fonction qui permet de convertir un tableau de string en double.
            //------------------------------------------------------------------

            double[,] tab_temp = new double[this.NbLignes, this.NbColonnes];

            for (int i = 0; i < this.NbLignes; i++)
            {
                for (int j = 0; j < this.NbColonnes; j++)
                {
                    if (Convert.ToString(this._data[i, j]) == "")
                    {
                        tab_temp[i, j] = 0;
                    }
                    else
                    {
                        tab_temp[i, j] = Convert.ToDouble(this._data[i, j]);
                    }


                }


            }

            return tab_temp;

        }

        public double Avg(int n1, int n2, int p1, int p2, double [,] data)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------------------
            //Fonction qui, à partir d'un tableau de données, calcule la moyenne d'une partie de ce tableau.
            //La partie du tableau à utiliser pour le calcul est spécifiée par l'utilisateur en renseignant en ligne et en colonne les indices de début et de fin
            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            double somme = 0;
            int nb = 0;

            for (int i = n1; i <= n2; i++)
            {
                for (int j = p1; j <= p2; j++)
                {
                    somme += data[i, j];
                    nb++;
                }

            }

            return somme / nb;

        }

        public double Volatility(int n1, int n2, int p1, int p2, double[,] data)
        {

            //-----------------------------------------------------------------------------------------------------------------------------------------------------
            //Fonction qui, à partir d'un tableau de données, calcule la volatilité d'une partie de ce tableau.
            //La partie du tableau à utiliser pour le calcul est spécifiée par l'utilisateur en renseignant en ligne et en colonne les indices de début et de fin
            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            double somme;
            int nb = 0;
            double avg;

            avg = this.Avg(n1, n2, p1, p2, data);
            somme = 0;

            for (int i = n1; i <= n2; i++)
            {
                for (int j = p1; j <= p2; j++)
                {
                    somme += Math.Pow(data[i, j] - avg, 2);
                    nb++;
                }

            }

            return Math.Sqrt(somme / (nb));

        }
    }

}

