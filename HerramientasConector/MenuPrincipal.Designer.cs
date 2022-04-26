namespace HerramientasConector
{
    partial class MenuPrincipal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuPrincipal));
            this.label1 = new System.Windows.Forms.Label();
            this.BuscarFacturasButton = new System.Windows.Forms.Button();
            this.tb_NumeroReferencia = new System.Windows.Forms.TextBox();
            this.LV_Facturas = new System.Windows.Forms.ListView();
            this.id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NumeroConsecutivo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NumeroReferencia = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Fecha = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReversarButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.UbicacionTB = new System.Windows.Forms.TextBox();
            this.LoadButton = new System.Windows.Forms.Button();
            this.ArchivoTxtOFD = new System.Windows.Forms.OpenFileDialog();
            this.ReversoLoteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(644, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reverso Automatico , Buscar Facturas por Numero de Referencia Externa :";
            // 
            // BuscarFacturasButton
            // 
            this.BuscarFacturasButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.BuscarFacturasButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BuscarFacturasButton.ForeColor = System.Drawing.Color.White;
            this.BuscarFacturasButton.Image = global::HerramientasConector.Properties.Resources.iniciar_24x24_ticopay_02;
            this.BuscarFacturasButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BuscarFacturasButton.Location = new System.Drawing.Point(63, 158);
            this.BuscarFacturasButton.Margin = new System.Windows.Forms.Padding(4);
            this.BuscarFacturasButton.Name = "BuscarFacturasButton";
            this.BuscarFacturasButton.Size = new System.Drawing.Size(105, 44);
            this.BuscarFacturasButton.TabIndex = 1;
            this.BuscarFacturasButton.Text = "Buscar";
            this.BuscarFacturasButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BuscarFacturasButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BuscarFacturasButton.UseVisualStyleBackColor = false;
            this.BuscarFacturasButton.Click += new System.EventHandler(this.BuscarFacturasButton_Click);
            // 
            // tb_NumeroReferencia
            // 
            this.tb_NumeroReferencia.Location = new System.Drawing.Point(16, 81);
            this.tb_NumeroReferencia.Margin = new System.Windows.Forms.Padding(4);
            this.tb_NumeroReferencia.Name = "tb_NumeroReferencia";
            this.tb_NumeroReferencia.Size = new System.Drawing.Size(189, 22);
            this.tb_NumeroReferencia.TabIndex = 2;
            // 
            // LV_Facturas
            // 
            this.LV_Facturas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.id,
            this.NumeroConsecutivo,
            this.NumeroReferencia,
            this.Fecha});
            this.LV_Facturas.Location = new System.Drawing.Point(215, 42);
            this.LV_Facturas.Margin = new System.Windows.Forms.Padding(4);
            this.LV_Facturas.MultiSelect = false;
            this.LV_Facturas.Name = "LV_Facturas";
            this.LV_Facturas.Size = new System.Drawing.Size(708, 159);
            this.LV_Facturas.TabIndex = 3;
            this.LV_Facturas.UseCompatibleStateImageBehavior = false;
            this.LV_Facturas.View = System.Windows.Forms.View.Details;
            // 
            // id
            // 
            this.id.Text = "Id Factura";
            this.id.Width = 150;
            // 
            // NumeroConsecutivo
            // 
            this.NumeroConsecutivo.Text = "Numero Consecutivo Ticopay";
            this.NumeroConsecutivo.Width = 202;
            // 
            // NumeroReferencia
            // 
            this.NumeroReferencia.Text = "Numero de Referencia Externa";
            this.NumeroReferencia.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NumeroReferencia.Width = 207;
            // 
            // Fecha
            // 
            this.Fecha.Text = "Fecha";
            this.Fecha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Fecha.Width = 128;
            // 
            // ReversarButton
            // 
            this.ReversarButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.ReversarButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ReversarButton.ForeColor = System.Drawing.Color.White;
            this.ReversarButton.Image = global::HerramientasConector.Properties.Resources.eliminar_24x24_ticopay_02;
            this.ReversarButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ReversarButton.Location = new System.Drawing.Point(960, 70);
            this.ReversarButton.Margin = new System.Windows.Forms.Padding(4);
            this.ReversarButton.Name = "ReversarButton";
            this.ReversarButton.Size = new System.Drawing.Size(123, 44);
            this.ReversarButton.TabIndex = 4;
            this.ReversarButton.Text = "Reversar";
            this.ReversarButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ReversarButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ReversarButton.UseVisualStyleBackColor = false;
            this.ReversarButton.Click += new System.EventHandler(this.ReversarButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Numero de Referencia";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(931, 42);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Acciones a Ejecutar :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(219, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Procesar un archivo de Reversos";
            // 
            // UbicacionTB
            // 
            this.UbicacionTB.Location = new System.Drawing.Point(249, 217);
            this.UbicacionTB.Name = "UbicacionTB";
            this.UbicacionTB.ReadOnly = true;
            this.UbicacionTB.Size = new System.Drawing.Size(579, 22);
            this.UbicacionTB.TabIndex = 8;
            // 
            // LoadButton
            // 
            this.LoadButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.LoadButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LoadButton.ForeColor = System.Drawing.Color.White;
            this.LoadButton.Image = global::HerramientasConector.Properties.Resources.abrir_24x24_ticopay_02;
            this.LoadButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LoadButton.Location = new System.Drawing.Point(852, 206);
            this.LoadButton.Margin = new System.Windows.Forms.Padding(4);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(71, 44);
            this.LoadButton.TabIndex = 9;
            this.LoadButton.Text = "Abrir";
            this.LoadButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LoadButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.LoadButton.UseVisualStyleBackColor = false;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // ArchivoTxtOFD
            // 
            this.ArchivoTxtOFD.Filter = "Text files (*.txt) | *.txt";
            this.ArchivoTxtOFD.Title = "Abrir archivo de Documentos a Reversar";
            // 
            // ReversoLoteButton
            // 
            this.ReversoLoteButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.ReversoLoteButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ReversoLoteButton.ForeColor = System.Drawing.Color.White;
            this.ReversoLoteButton.Image = global::HerramientasConector.Properties.Resources.eliminar_24x24_ticopay_02;
            this.ReversoLoteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ReversoLoteButton.Location = new System.Drawing.Point(934, 206);
            this.ReversoLoteButton.Margin = new System.Windows.Forms.Padding(4);
            this.ReversoLoteButton.Name = "ReversoLoteButton";
            this.ReversoLoteButton.Size = new System.Drawing.Size(183, 44);
            this.ReversoLoteButton.TabIndex = 10;
            this.ReversoLoteButton.Text = "Reversar Lote desde Archivo";
            this.ReversoLoteButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ReversoLoteButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ReversoLoteButton.UseVisualStyleBackColor = false;
            this.ReversoLoteButton.Click += new System.EventHandler(this.ReversoLoteButton_Click);
            // 
            // MenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 317);
            this.Controls.Add(this.ReversoLoteButton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.UbicacionTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ReversarButton);
            this.Controls.Add(this.LV_Facturas);
            this.Controls.Add(this.tb_NumeroReferencia);
            this.Controls.Add(this.BuscarFacturasButton);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MenuPrincipal";
            this.Text = "Herramientas para el Conector de Ticopay";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BuscarFacturasButton;
        private System.Windows.Forms.TextBox tb_NumeroReferencia;
        private System.Windows.Forms.ListView LV_Facturas;
        private System.Windows.Forms.ColumnHeader NumeroConsecutivo;
        private System.Windows.Forms.ColumnHeader NumeroReferencia;
        private System.Windows.Forms.ColumnHeader Fecha;
        private System.Windows.Forms.ColumnHeader id;
        private System.Windows.Forms.Button ReversarButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox UbicacionTB;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.OpenFileDialog ArchivoTxtOFD;
        private System.Windows.Forms.Button ReversoLoteButton;
    }
}