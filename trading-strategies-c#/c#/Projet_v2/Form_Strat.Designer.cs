namespace Projet_v2
{
    partial class Form_Strat
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.Combo_StratType = new System.Windows.Forms.ComboBox();
            this.Txt_n = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Txt_UpperRSI = new System.Windows.Forms.TextBox();
            this.Txt_LowerRSI = new System.Windows.Forms.TextBox();
            this.Label_UpperRSI = new System.Windows.Forms.Label();
            this.Label_LowerRSI = new System.Windows.Forms.Label();
            this.Chart_Return = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Label_nbStd = new System.Windows.Forms.Label();
            this.Txt_nbStd = new System.Windows.Forms.TextBox();
            this.Combo_Mom = new System.Windows.Forms.ComboBox();
            this.btn_Run = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Txt_Rdt = new System.Windows.Forms.TextBox();
            this.Txt_RdtVol = new System.Windows.Forms.TextBox();
            this.Txt_Vol = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_RdtIndice = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_Optimize = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Return)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Combo_StratType
            // 
            this.Combo_StratType.FormattingEnabled = true;
            this.Combo_StratType.Location = new System.Drawing.Point(265, 20);
            this.Combo_StratType.Name = "Combo_StratType";
            this.Combo_StratType.Size = new System.Drawing.Size(180, 24);
            this.Combo_StratType.TabIndex = 0;
            this.Combo_StratType.SelectedIndexChanged += new System.EventHandler(this.Combo_StratType_SelectedIndexChanged);
            // 
            // Txt_n
            // 
            this.Txt_n.Location = new System.Drawing.Point(265, 54);
            this.Txt_n.Name = "Txt_n";
            this.Txt_n.Size = new System.Drawing.Size(75, 22);
            this.Txt_n.TabIndex = 3;
            this.Txt_n.Text = "20";
            this.Txt_n.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Type de stratégie";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "n";
            // 
            // Txt_UpperRSI
            // 
            this.Txt_UpperRSI.Location = new System.Drawing.Point(265, 83);
            this.Txt_UpperRSI.Name = "Txt_UpperRSI";
            this.Txt_UpperRSI.Size = new System.Drawing.Size(75, 22);
            this.Txt_UpperRSI.TabIndex = 7;
            this.Txt_UpperRSI.Text = "70";
            this.Txt_UpperRSI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Txt_UpperRSI.Visible = false;
            // 
            // Txt_LowerRSI
            // 
            this.Txt_LowerRSI.Location = new System.Drawing.Point(265, 112);
            this.Txt_LowerRSI.Name = "Txt_LowerRSI";
            this.Txt_LowerRSI.Size = new System.Drawing.Size(75, 22);
            this.Txt_LowerRSI.TabIndex = 8;
            this.Txt_LowerRSI.Text = "30";
            this.Txt_LowerRSI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Txt_LowerRSI.Visible = false;
            // 
            // Label_UpperRSI
            // 
            this.Label_UpperRSI.AutoSize = true;
            this.Label_UpperRSI.Location = new System.Drawing.Point(17, 83);
            this.Label_UpperRSI.Name = "Label_UpperRSI";
            this.Label_UpperRSI.Size = new System.Drawing.Size(223, 17);
            this.Label_UpperRSI.TabIndex = 9;
            this.Label_UpperRSI.Text = "Borne haute (calcul du signal RSI)";
            this.Label_UpperRSI.Visible = false;
            // 
            // Label_LowerRSI
            // 
            this.Label_LowerRSI.AutoSize = true;
            this.Label_LowerRSI.Location = new System.Drawing.Point(17, 115);
            this.Label_LowerRSI.Name = "Label_LowerRSI";
            this.Label_LowerRSI.Size = new System.Drawing.Size(225, 17);
            this.Label_LowerRSI.TabIndex = 10;
            this.Label_LowerRSI.Text = "Borne basse (calcul du signal RSI)";
            this.Label_LowerRSI.Visible = false;
            // 
            // Chart_Return
            // 
            chartArea2.Name = "ChartArea1";
            this.Chart_Return.ChartAreas.Add(chartArea2);
            legend2.Alignment = System.Drawing.StringAlignment.Center;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend2.Name = "Legend1";
            this.Chart_Return.Legends.Add(legend2);
            this.Chart_Return.Location = new System.Drawing.Point(251, 10);
            this.Chart_Return.Name = "Chart_Return";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.LegendText = "Strategy Return";
            series3.Name = "Series1";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.LegendText = "Index Return";
            series4.Name = "Series2";
            this.Chart_Return.Series.Add(series3);
            this.Chart_Return.Series.Add(series4);
            this.Chart_Return.Size = new System.Drawing.Size(615, 419);
            this.Chart_Return.TabIndex = 11;
            this.Chart_Return.Text = "chart1";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.btn_Optimize);
            this.groupBox1.Controls.Add(this.Label_nbStd);
            this.groupBox1.Controls.Add(this.Txt_nbStd);
            this.groupBox1.Controls.Add(this.Combo_Mom);
            this.groupBox1.Controls.Add(this.Label_UpperRSI);
            this.groupBox1.Controls.Add(this.Combo_StratType);
            this.groupBox1.Controls.Add(this.Label_LowerRSI);
            this.groupBox1.Controls.Add(this.Txt_n);
            this.groupBox1.Controls.Add(this.Txt_LowerRSI);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Txt_UpperRSI);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(21, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(671, 146);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Paramètres";
            // 
            // Label_nbStd
            // 
            this.Label_nbStd.AutoSize = true;
            this.Label_nbStd.Location = new System.Drawing.Point(464, 54);
            this.Label_nbStd.Name = "Label_nbStd";
            this.Label_nbStd.Size = new System.Drawing.Size(101, 17);
            this.Label_nbStd.TabIndex = 13;
            this.Label_nbStd.Text = "Nb écart-types";
            // 
            // Txt_nbStd
            // 
            this.Txt_nbStd.Location = new System.Drawing.Point(571, 54);
            this.Txt_nbStd.Name = "Txt_nbStd";
            this.Txt_nbStd.Size = new System.Drawing.Size(50, 22);
            this.Txt_nbStd.TabIndex = 12;
            this.Txt_nbStd.Text = "1";
            this.Txt_nbStd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Combo_Mom
            // 
            this.Combo_Mom.FormattingEnabled = true;
            this.Combo_Mom.Location = new System.Drawing.Point(462, 21);
            this.Combo_Mom.Name = "Combo_Mom";
            this.Combo_Mom.Size = new System.Drawing.Size(159, 24);
            this.Combo_Mom.TabIndex = 11;
            // 
            // btn_Run
            // 
            this.btn_Run.Location = new System.Drawing.Point(705, 25);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(185, 146);
            this.btn_Run.TabIndex = 13;
            this.btn_Run.Text = "Run";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.groupBox2.Controls.Add(this.txt_RdtIndice);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.Txt_Rdt);
            this.groupBox2.Controls.Add(this.Txt_RdtVol);
            this.groupBox2.Controls.Add(this.Txt_Vol);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.Chart_Return);
            this.groupBox2.Location = new System.Drawing.Point(18, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(872, 435);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stratégie";
            // 
            // Txt_Rdt
            // 
            this.Txt_Rdt.Enabled = false;
            this.Txt_Rdt.Location = new System.Drawing.Point(160, 56);
            this.Txt_Rdt.Name = "Txt_Rdt";
            this.Txt_Rdt.Size = new System.Drawing.Size(75, 22);
            this.Txt_Rdt.TabIndex = 15;
            this.Txt_Rdt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Txt_RdtVol
            // 
            this.Txt_RdtVol.Enabled = false;
            this.Txt_RdtVol.Location = new System.Drawing.Point(160, 114);
            this.Txt_RdtVol.Name = "Txt_RdtVol";
            this.Txt_RdtVol.Size = new System.Drawing.Size(75, 22);
            this.Txt_RdtVol.TabIndex = 17;
            this.Txt_RdtVol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Txt_Vol
            // 
            this.Txt_Vol.Enabled = false;
            this.Txt_Vol.Location = new System.Drawing.Point(160, 85);
            this.Txt_Vol.Name = "Txt_Vol";
            this.Txt_Vol.Size = new System.Drawing.Size(75, 22);
            this.Txt_Vol.TabIndex = 16;
            this.Txt_Vol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "Rendement/Volatilité";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Volatilité";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Rendement stratégie";
            // 
            // txt_RdtIndice
            // 
            this.txt_RdtIndice.Enabled = false;
            this.txt_RdtIndice.Location = new System.Drawing.Point(160, 28);
            this.txt_RdtIndice.Name = "txt_RdtIndice";
            this.txt_RdtIndice.Size = new System.Drawing.Size(75, 22);
            this.txt_RdtIndice.TabIndex = 19;
            this.txt_RdtIndice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 17);
            this.label6.TabIndex = 18;
            this.label6.Text = "Rendement indice";
            // 
            // btn_Optimize
            // 
            this.btn_Optimize.Location = new System.Drawing.Point(344, 50);
            this.btn_Optimize.Name = "btn_Optimize";
            this.btn_Optimize.Size = new System.Drawing.Size(101, 34);
            this.btn_Optimize.TabIndex = 14;
            this.btn_Optimize.Text = "Optimize";
            this.btn_Optimize.UseVisualStyleBackColor = true;
            this.btn_Optimize.Click += new System.EventHandler(this.btn_Optimize_Click);
            // 
            // Form_Strat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 624);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_Run);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form_Strat";
            this.Text = "Stratégie de trading";
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Return)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox Combo_StratType;
        private System.Windows.Forms.TextBox Txt_n;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Txt_UpperRSI;
        private System.Windows.Forms.TextBox Txt_LowerRSI;
        private System.Windows.Forms.Label Label_UpperRSI;
        private System.Windows.Forms.Label Label_LowerRSI;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_Return;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox Txt_Rdt;
        private System.Windows.Forms.TextBox Txt_RdtVol;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Combo_Mom;
        private System.Windows.Forms.TextBox Txt_Vol;
        private System.Windows.Forms.Label Label_nbStd;
        private System.Windows.Forms.TextBox Txt_nbStd;
        private System.Windows.Forms.TextBox txt_RdtIndice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_Optimize;
    }
}

