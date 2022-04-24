using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projet_v2
{
    
    partial class Form_Strat : Form
    {
        public Form_Strat()
        {   
            //---------------------------------------
            // Méthode d'initialisation du formulaire
            // --------------------------------------
            InitializeComponent();
            FillCombos();
        }

        private void FillCombos()
        {
            //----------------------------------------------------------------------------------
            // Méthode qui permet de remplir les listes lors de l'initialisation du formulaire
            // ---------------------------------------------------------------------------------

            this.Combo_StratType.Items.Add("Moyenne mobile");
            this.Combo_StratType.Items.Add("Bandes de Bollinger");
            this.Combo_StratType.Items.Add("RSI");
            this.Combo_StratType.SelectedItem = "Moyenne mobile";

            this.Combo_Mom.Items.Add("Momentum");
            this.Combo_Mom.Items.Add("Contrariante");
            this.Combo_Mom.SelectedItem = "Momentum";

        }

        private void Combo_StratType_SelectedIndexChanged(object sender, EventArgs e)
        {

            //-------------------------------------------------------------------------------------------------------
            // Méthode qui permet de masquer ou d'afficher chertains objets du formulaire en fonction de la séléction
            // de l'utilisateur car chaque stratégie requiert des paramètres qui peuvent être communs à toutes 
            // et d'autres plus spécifiques.
            // ------------------------------------------------------------------------------------------------------

            if ( Combo_StratType.SelectedItem.ToString() =="Moyenne mobile" || Combo_StratType.SelectedItem.ToString() == "Bandes de Bollinger")
            {

                if (Combo_StratType.SelectedItem.ToString() == "Bandes de Bollinger")
                {
                    this.Label_nbStd.Visible = true;
                    this.Txt_nbStd.Visible = true;
                }
                else
                {
                    this.Label_nbStd.Visible = false;
                    this.Txt_nbStd.Visible = false;
                }
                this.Combo_Mom.Visible = true;
                this.Txt_UpperRSI.Visible = false;
                this.Txt_LowerRSI.Visible = false;
                this.Label_UpperRSI.Visible = false;
                this.Label_LowerRSI.Visible = false;
                

            }
            else
            {
                this.Combo_Mom.Visible = false;
                this.Txt_UpperRSI.Visible = true;
                this.Txt_LowerRSI.Visible = true;
                this.Label_UpperRSI.Visible = true;
                this.Label_LowerRSI.Visible = true;
                this.Label_nbStd.Visible = false;
                this.Txt_nbStd.Visible = false;

            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            //---------------------------------------------------------------------------------------------
            // Méthode qui lance le calcul du modèle (récupération des données, calcul de la stratégie...)
            // --------------------------------------------------------------------------------------------

            RunModel();            
        }

        private MatriceFinanciere GetData()
        {
            //-----------------------------------------------------------------------------------------------------------------------
            // Fonction qui permet de récupérer les données depuis un fichier csv. Elle retourne un objet de type MatriceFinancière
            // ----------------------------------------------------------------------------------------------------------------------

            // Récupération des données
            string path = "C:/Users/Sara/OneDrive - Université Paris-Dauphine/Scolarité/Cours/Cours M2/C#/Projet Fin Semestre/Projet_v2/Projet_v2/Data STOXX.csv";
            string[] lines = File.ReadAllLines(path);

            // Création d'une matrice avec les données brutes
            int iNbCol = lines[1].Split(';').Length;
            int iNbRow = lines.Count();
            string[,] Data = new string[iNbRow, iNbCol];

            Matrice.DataBruteToMatrice(lines, Data);

            // Création d'un objet MatriceFinanciere qui permet de séparer les données en différents tableaux (en-têtes, dates, prix)
            // et d'effectuers des calculs financiers
            MatriceFinanciere mSTOXX = new MatriceFinanciere(Data);

            // Calcul des rendements
            mSTOXX.ComputeReturns();

            return mSTOXX;

        }

        private void btn_Optimize_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------------------------------------------------------------------
            // Méthode qui permet de calculer le n optimal pours les différentes stratégies. L'utilisateur choisit une stratégie
            // dans le formulaire et lance l'optimisation. La méthode remplit la zone de texte avec le paramètre n avec le n optimal
            // et recalcule le modèle.
            // -----------------------------------------------------------------------------------------------------------------------

            btn_Optimize.Text = "Optimizing...";

            // Récupération des paramètres de la stratégie 
            string sStratType = this.Combo_StratType.Text;
            bool bMomentum = (this.Combo_Mom.Text == "Momentum") ? true : false;
            int iUpperRSI = Int32.Parse(this.Txt_UpperRSI.Text);
            int iLowerRSI = Int32.Parse(this.Txt_LowerRSI.Text);
            int nb_Std = Int32.Parse(this.Txt_nbStd.Text);
            double RatioMax = 0;
            double RatioRtnVol = 0;
            int n_opt = 0;

            // Récupération des données financières 
            MatriceFinanciere mSTOXX = GetData();

            if (sStratType == "Moyenne mobile")
            {
                for (int i = 10; i <= 300; i+=10)
                {
                    // Instanciation d'un objet stratégie en fonction de la stratégie séléctionnées par l'utilisateur
                    MovingAvg Strat = new MovingAvg(mSTOXX, i, bMomentum);

                    // Calcul du ratio rendement/volatilité
                    RatioRtnVol = Strat.Return / Strat.Vol;

                    //Récupération du ratio maximum et du n optimal
                    if (RatioRtnVol > RatioMax)
                    {
                        RatioMax = RatioRtnVol;
                        n_opt = i;
                    }
                }

            }
            else if (sStratType == "Bandes de Bollinger")
            {            
                for (int i = 10; i <= 300; i+=10)
                {
                    // Instanciation d'un objet stratégie en fonction de la stratégie séléctionnées par l'utilisateur
                    Bollinger Strat = new Bollinger(mSTOXX, i, nb_Std, bMomentum);

                    // Calcul du ratio rendement/volatilité
                    RatioRtnVol = Strat.Return / Strat.Vol;

                    //Récupération du ratio maximum et du n optimal
                    if (RatioRtnVol > RatioMax)
                    {
                        RatioMax = RatioRtnVol;
                        n_opt = i;
                    }
                }
            }
            else
            {
                
                for (int i = 10; i <= 300; i+=10)
                {
                    // Instanciation d'un objet stratégie en fonction de la stratégie séléctionnées par l'utilisateur
                    RSI Strat = new RSI(mSTOXX, i, iUpperRSI, iLowerRSI);

                    // Calcul du ratio rendement/volatilité
                    RatioRtnVol = Strat.Return / Strat.Vol;

                    //Récupération du ratio maximum et du n optimal
                    if (RatioRtnVol > RatioMax)
                    {
                        RatioMax = RatioRtnVol;
                        n_opt = i;
                    }
                }
            }

            //Report du n optimal et lancement du calcul du modèle avec ce nouveau n
            this.Txt_n.Text = n_opt.ToString();
            RunModel();
        }

        private void RunModel()
        {
            //-----------------------------------------------------------------------------------------------------------------------
            // Méthode qui lance le calcul du modèle : 
            //      Récupération des données depuis le fichier csv
            //      Instanciation de l'objet stratégie
            //      Report des statistiques (perf/vol) dans le formulaire
            //      Report de la performance cumulée dans le graphique du formulaire
            // ----------------------------------------------------------------------------------------------------------------------

            // Récupération des paramètres de la stratégie 
            string sStratType = this.Combo_StratType.Text;
            bool bMomentum = (this.Combo_Mom.Text == "Momentum") ? true : false;
            int n = Int32.Parse(this.Txt_n.Text);
            int iUpperRSI = Int32.Parse(this.Txt_UpperRSI.Text);
            int iLowerRSI = Int32.Parse(this.Txt_LowerRSI.Text);
            int nb_Std = Int32.Parse(this.Txt_nbStd.Text);


            // Récupération des données financières 
            MatriceFinanciere mSTOXX = GetData();

            // Instanciation d'un objet stratégie en fonction de la stratégie séléctionnées par l'utilisateur
            if (sStratType == "Moyenne mobile")
            {
                MovingAvg Strat = new MovingAvg(mSTOXX, n, bMomentum);
                txt_RdtIndice.Text = mSTOXX.Return.ToString("P");
                Txt_Rdt.Text = Strat.Return.ToString("P");
                Txt_Vol.Text = Strat.Vol.ToString("P");
                Txt_RdtVol.Text = (Strat.Return / Strat.Vol).ToString("0.00");
                Chart_Return.Series[0].Points.DataBindXY(Strat.DateRange, Strat.CumReturn);
                Chart_Return.Series[1].Points.DataBindY(mSTOXX.TotalCumReturn);
            }
            else if (sStratType == "Bandes de Bollinger")
            {
                Bollinger Strat = new Bollinger(mSTOXX, n, nb_Std, bMomentum);
                txt_RdtIndice.Text = mSTOXX.Return.ToString("P");
                Txt_Rdt.Text = Strat.Return.ToString("P");
                Txt_Vol.Text = Strat.Vol.ToString("P");
                Txt_RdtVol.Text = (Strat.Return / Strat.Vol).ToString("0.00");
                Chart_Return.Series[0].Points.DataBindXY(Strat.DateRange, Strat.CumReturn);
                Chart_Return.Series[1].Points.DataBindY(mSTOXX.TotalCumReturn);
            }
            else
            {
                RSI Strat = new RSI(mSTOXX, n, iUpperRSI, iLowerRSI);
                txt_RdtIndice.Text = mSTOXX.Return.ToString("P");
                Txt_Rdt.Text = Strat.Return.ToString("P");
                Txt_Vol.Text = Strat.Vol.ToString("P");
                Txt_RdtVol.Text = (Strat.Return / Strat.Vol).ToString("0.00");
                Chart_Return.Series[0].Points.DataBindXY(Strat.DateRange, Strat.CumReturn);
                Chart_Return.Series[1].Points.DataBindXY(Strat.DateRange, mSTOXX.TotalCumReturn);
            }
        }
    }
}
