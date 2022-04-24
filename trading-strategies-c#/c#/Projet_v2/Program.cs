using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Projet_v2
{
    
    public static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // -------------------------------------------------------------------------------------------
            // Programme principal
            // -------------------------------------------------------------------------------------------

            //TestsUnitaires();

            // Lancement du formulaire
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_Strat());

        }


        public static bool TestsUnitaires()
        {
            //------------------------------------------------------------------------------------------------------------------------------------
            // Cette fonction consiste a créer plusieurs objets stratégie pour en étudier les propriétés (notamment rendement et volatilité)
            // afin de savoir si le modèle est bien calibré. Nous avons effectué les mêmes calculs sur Excel afin de comparer avec les 
            // résultats de notre modèle. La fonction renvoie un booléen qui vaut true si le modèle est bien calibré et false dans le cas contraire
            //------------------------------------------------------------------------------------------------------------------------------------
            
            bool bResult = true;

            string path = "C:/Users/sarab/OneDrive - Université Paris-Dauphine/Scolarité/Cours/Cours M2/C#/Projet Fin Semestre/Projet_v1/Projet_v1/Data STOXX.csv";
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

            MovingAvg Strat1 = new MovingAvg(mSTOXX, 20, true);
            MovingAvg Strat2 = new MovingAvg(mSTOXX, 100, false);
            Bollinger Strat3 = new Bollinger(mSTOXX, 50, 1, true);
            Bollinger Strat4 = new Bollinger(mSTOXX, 20, 2, false);
            RSI Strat5 = new RSI(mSTOXX, 14, 70, 30);
            RSI Strat6 = new RSI(mSTOXX, 30, 70, 30);

            // Vérification des rendements obtenus pour l'indice STOXX 50
            bResult = bResult && Math.Round(mSTOXX.Return,5) == 1.36644;

            // Vérification des rendements et volatilités obtenus pour les différentes stratégies testées
            bResult = bResult && Math.Round(Strat1.Return, 5) == 0.21185;
            bResult = bResult && Math.Round(Strat1.Vol, 5) == 0.09227;

            bResult = bResult && Math.Round(Strat2.Return, 5) == 0.84462;
            bResult = bResult && Math.Round(Strat2.Vol, 5) == 0.13074;

            bResult = bResult && Math.Round(Strat3.Return, 5) == 0.39345;
            bResult = bResult && Math.Round(Strat3.Vol, 5) == 0.09283;

            bResult = bResult && Math.Round(Strat4.Return, 5) == 1.04331;
            bResult = bResult && Math.Round(Strat4.Vol, 5) == 0.13283;

            bResult = bResult && Math.Round(Strat5.Return, 5) == 3.65769;
            bResult = bResult && Math.Round(Strat5.Vol, 5) == 0.11101;

            bResult = bResult && Math.Round(Strat6.Return, 5) == 1.17424;
            bResult = bResult && Math.Round(Strat6.Vol, 5) == 0.09803;

            return bResult;
        }

    }
}
