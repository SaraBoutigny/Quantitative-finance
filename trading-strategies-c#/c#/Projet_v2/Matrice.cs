using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_v2
{
    class Matrice
    {
        // Attributs ==============================================================================================================================================
        protected object[,] _data;
        private int _nbLignes;
        private int _nbColonnes;

        // Attributs publics ====================================================================================================================================
        public int NbLignes
        {
            get { return _nbLignes; }
            set { _nbLignes = value; }
        }
        public int NbColonnes
        {
            get { return _nbColonnes; }
            set { _nbColonnes = value; }
        }

        // Constructeurs ==========================================================================================================================================
        public Matrice() { }

        public Matrice(int n, int p)
        {
            this._data = new object[n, p];
            this._nbLignes = n;
            this._nbColonnes = p;
        }

        public Matrice(object[,] tableau)
        {
            int n = tableau.GetLength(0);
            int p = tableau.GetLength(1);
            this._data = new object[n, p];
            this._nbLignes = n;
            this._nbColonnes = p;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    this._data[i, j] = tableau[i, j];
                }
            }
        }


        // Méthodes =========================================================================================================================================
        public static void DataBruteToMatrice(string[] lines, string[,] Data)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------------------
            // Lors de l'extraction depuis le fichier csv, les données sont récupérées sous un format qui n'est pas manipulable tel quel.
            // Cette méthode permet de transformer en objet Matrice les données. Elle range l'ensemble des données dans un tableau
            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            int i = 0;
            int j = 0;

            foreach (string line in lines)
            {

                string[] columns = line.Split(';');
                foreach (string column in columns)
                {
                    Data[i, j] = column;
                    j++;
                }

                i++;
                j = 0;
            }


        }

        // Fonctions ==========================================================================================================================================
        public virtual Matrice SubMatrix(int n1, int n2, int p1, int p2)
        {
            //-----------------------------------------------------------------------------------------------------------------------------------------------------
            // Cette méthode permet d'extraire une matrice d'une autre matrice en spécifiant des indices de début et de fin en 
            // ligne et en colonne. Elle est de type vitrual car est overridée dans la classe MatriceFinanciere, héritée de la classe matrice.
            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            Matrice mat_temp = new Matrice(n2 - n1 + 1, p2 - p1 + 1);
            if (n1 == n2) { n2++; }
            if (p1 == p2) { p2++; }

            int k = 0;
            int l = 0;

            for (int i = n1; i < n2; i++)
            {
                for (int j = p1; j < p2; j++)
                {
                    mat_temp._data[k, l] = this._data[i, j];
                    l++;
                }
                k++;
                l = 0;
            }

            return mat_temp;

        }


    }
}









