﻿
namespace TomMotos.view
{
    partial class FmrListProdutos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FmrListProdutos));
            this.dgListProdutos = new System.Windows.Forms.DataGridView();
            this.cbxBuscar = new System.Windows.Forms.ComboBox();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnMostrarTudo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgListProdutos)).BeginInit();
            this.SuspendLayout();
            // 
            // dgListProdutos
            // 
            this.dgListProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgListProdutos.Location = new System.Drawing.Point(1, 63);
            this.dgListProdutos.MultiSelect = false;
            this.dgListProdutos.Name = "dgListProdutos";
            this.dgListProdutos.ReadOnly = true;
            this.dgListProdutos.Size = new System.Drawing.Size(800, 384);
            this.dgListProdutos.TabIndex = 0;
            this.dgListProdutos.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgListProdutos_CellDoubleClick);
            // 
            // cbxBuscar
            // 
            this.cbxBuscar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBuscar.FormattingEnabled = true;
            this.cbxBuscar.Items.AddRange(new object[] {
            "ID",
            "DESCRICAO",
            "VALOR",
            "QUANTIDADE",
            "MARCA"});
            this.cbxBuscar.Location = new System.Drawing.Point(12, 12);
            this.cbxBuscar.Name = "cbxBuscar";
            this.cbxBuscar.Size = new System.Drawing.Size(105, 21);
            this.cbxBuscar.TabIndex = 26;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(123, 13);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(380, 20);
            this.txtBuscar.TabIndex = 27;
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Location = new System.Drawing.Point(509, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(37, 23);
            this.button2.TabIndex = 28;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnMostrarTudo
            // 
            this.btnMostrarTudo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMostrarTudo.Location = new System.Drawing.Point(552, 16);
            this.btnMostrarTudo.Name = "btnMostrarTudo";
            this.btnMostrarTudo.Size = new System.Drawing.Size(84, 20);
            this.btnMostrarTudo.TabIndex = 47;
            this.btnMostrarTudo.Text = "Mostrar tudo";
            this.btnMostrarTudo.UseVisualStyleBackColor = true;
            this.btnMostrarTudo.Click += new System.EventHandler(this.btnMostrarTudo_Click);
            // 
            // FmrListProdutos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnMostrarTudo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.cbxBuscar);
            this.Controls.Add(this.dgListProdutos);
            this.Name = "FmrListProdutos";
            this.Text = "FmrListProdutos";
            this.Load += new System.EventHandler(this.FmrListProdutos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgListProdutos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgListProdutos;
        private System.Windows.Forms.ComboBox cbxBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnMostrarTudo;
    }
}