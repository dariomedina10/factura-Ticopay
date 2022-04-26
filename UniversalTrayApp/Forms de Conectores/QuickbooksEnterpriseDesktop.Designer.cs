namespace UniversalTrayApp.Forms_de_Conectores
{
    partial class QuickbooksEnterpriseDesktop
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuickbooksEnterpriseDesktop));
            this.TicopayDataGB = new System.Windows.Forms.GroupBox();
            this.ProbarTicopayBtn = new System.Windows.Forms.Button();
            this.ClaveTxb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.UsuarioTbx = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SubDominioTxb = new System.Windows.Forms.TextBox();
            this.DatosQuickbooksGB = new System.Windows.Forms.GroupBox();
            this.ProbarConexionContaPymeBtn = new System.Windows.Forms.Button();
            this.IdEmpresaQuickbooksTxb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.BuscarBDBtn = new System.Windows.Forms.Button();
            this.DireccionArchivoCompaniaTxb = new System.Windows.Forms.TextBox();
            this.AgregarBtn = new System.Windows.Forms.Button();
            this.CancelarBtn = new System.Windows.Forms.Button();
            this.ArchivoCompaniaOFD = new System.Windows.Forms.OpenFileDialog();
            this.SalesReceiptCheckBox = new System.Windows.Forms.CheckBox();
            this.TicopayDataGB.SuspendLayout();
            this.DatosQuickbooksGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // TicopayDataGB
            // 
            this.TicopayDataGB.Controls.Add(this.ProbarTicopayBtn);
            this.TicopayDataGB.Controls.Add(this.ClaveTxb);
            this.TicopayDataGB.Controls.Add(this.label1);
            this.TicopayDataGB.Controls.Add(this.label2);
            this.TicopayDataGB.Controls.Add(this.UsuarioTbx);
            this.TicopayDataGB.Controls.Add(this.label3);
            this.TicopayDataGB.Controls.Add(this.SubDominioTxb);
            this.TicopayDataGB.Location = new System.Drawing.Point(12, 12);
            this.TicopayDataGB.Name = "TicopayDataGB";
            this.TicopayDataGB.Size = new System.Drawing.Size(329, 208);
            this.TicopayDataGB.TabIndex = 0;
            this.TicopayDataGB.TabStop = false;
            this.TicopayDataGB.Text = "Datos de configuración de Ticopay";
            // 
            // ProbarTicopayBtn
            // 
            this.ProbarTicopayBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.ProbarTicopayBtn.ForeColor = System.Drawing.Color.White;
            this.ProbarTicopayBtn.Image = global::UniversalTrayApp.Properties.Resources.conectar_24x24_ticopay_02;
            this.ProbarTicopayBtn.Location = new System.Drawing.Point(170, 158);
            this.ProbarTicopayBtn.Name = "ProbarTicopayBtn";
            this.ProbarTicopayBtn.Size = new System.Drawing.Size(150, 37);
            this.ProbarTicopayBtn.TabIndex = 14;
            this.ProbarTicopayBtn.Text = "Probar Credenciales";
            this.ProbarTicopayBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProbarTicopayBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ProbarTicopayBtn.UseVisualStyleBackColor = false;
            this.ProbarTicopayBtn.Click += new System.EventHandler(this.ProbarTicopayBtn_Click);
            // 
            // ClaveTxb
            // 
            this.ClaveTxb.Location = new System.Drawing.Point(18, 132);
            this.ClaveTxb.Name = "ClaveTxb";
            this.ClaveTxb.Size = new System.Drawing.Size(302, 20);
            this.ClaveTxb.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Sub Dominio o Tenant de Ticopay :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Usuario Correspondiente a ese Sub Dominio :";
            // 
            // UsuarioTbx
            // 
            this.UsuarioTbx.Location = new System.Drawing.Point(18, 88);
            this.UsuarioTbx.Name = "UsuarioTbx";
            this.UsuarioTbx.Size = new System.Drawing.Size(302, 20);
            this.UsuarioTbx.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Contraseña del Usuario :";
            // 
            // SubDominioTxb
            // 
            this.SubDominioTxb.Location = new System.Drawing.Point(18, 42);
            this.SubDominioTxb.Name = "SubDominioTxb";
            this.SubDominioTxb.Size = new System.Drawing.Size(302, 20);
            this.SubDominioTxb.TabIndex = 11;
            // 
            // DatosQuickbooksGB
            // 
            this.DatosQuickbooksGB.Controls.Add(this.SalesReceiptCheckBox);
            this.DatosQuickbooksGB.Controls.Add(this.ProbarConexionContaPymeBtn);
            this.DatosQuickbooksGB.Controls.Add(this.IdEmpresaQuickbooksTxb);
            this.DatosQuickbooksGB.Controls.Add(this.label4);
            this.DatosQuickbooksGB.Controls.Add(this.label9);
            this.DatosQuickbooksGB.Controls.Add(this.BuscarBDBtn);
            this.DatosQuickbooksGB.Controls.Add(this.DireccionArchivoCompaniaTxb);
            this.DatosQuickbooksGB.Location = new System.Drawing.Point(347, 12);
            this.DatosQuickbooksGB.Name = "DatosQuickbooksGB";
            this.DatosQuickbooksGB.Size = new System.Drawing.Size(441, 208);
            this.DatosQuickbooksGB.TabIndex = 1;
            this.DatosQuickbooksGB.TabStop = false;
            this.DatosQuickbooksGB.Text = "Datos de configuración de QuickBooks Enterprise Desktop";
            // 
            // ProbarConexionContaPymeBtn
            // 
            this.ProbarConexionContaPymeBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.ProbarConexionContaPymeBtn.ForeColor = System.Drawing.Color.White;
            this.ProbarConexionContaPymeBtn.Image = global::UniversalTrayApp.Properties.Resources.conectar_24x24_ticopay_02;
            this.ProbarConexionContaPymeBtn.Location = new System.Drawing.Point(302, 158);
            this.ProbarConexionContaPymeBtn.Name = "ProbarConexionContaPymeBtn";
            this.ProbarConexionContaPymeBtn.Size = new System.Drawing.Size(133, 37);
            this.ProbarConexionContaPymeBtn.TabIndex = 17;
            this.ProbarConexionContaPymeBtn.Text = "Probar Conexión";
            this.ProbarConexionContaPymeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProbarConexionContaPymeBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ProbarConexionContaPymeBtn.UseVisualStyleBackColor = false;
            this.ProbarConexionContaPymeBtn.Click += new System.EventHandler(this.ProbarConexionContaPymeBtn_Click);
            // 
            // IdEmpresaQuickbooksTxb
            // 
            this.IdEmpresaQuickbooksTxb.Location = new System.Drawing.Point(9, 88);
            this.IdEmpresaQuickbooksTxb.Name = "IdEmpresaQuickbooksTxb";
            this.IdEmpresaQuickbooksTxb.Size = new System.Drawing.Size(324, 20);
            this.IdEmpresaQuickbooksTxb.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(269, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Ubicación del archivo de la compañía de QuickBooks :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 72);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Empresa :";
            // 
            // BuscarBDBtn
            // 
            this.BuscarBDBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.BuscarBDBtn.ForeColor = System.Drawing.Color.White;
            this.BuscarBDBtn.Image = global::UniversalTrayApp.Properties.Resources.abrir_24x24_ticopay_02;
            this.BuscarBDBtn.Location = new System.Drawing.Point(344, 33);
            this.BuscarBDBtn.Name = "BuscarBDBtn";
            this.BuscarBDBtn.Size = new System.Drawing.Size(91, 37);
            this.BuscarBDBtn.TabIndex = 16;
            this.BuscarBDBtn.Text = "Buscar";
            this.BuscarBDBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BuscarBDBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BuscarBDBtn.UseVisualStyleBackColor = false;
            this.BuscarBDBtn.Click += new System.EventHandler(this.BuscarBDBtn_Click);
            // 
            // DireccionArchivoCompaniaTxb
            // 
            this.DireccionArchivoCompaniaTxb.Location = new System.Drawing.Point(9, 42);
            this.DireccionArchivoCompaniaTxb.Name = "DireccionArchivoCompaniaTxb";
            this.DireccionArchivoCompaniaTxb.ReadOnly = true;
            this.DireccionArchivoCompaniaTxb.Size = new System.Drawing.Size(324, 20);
            this.DireccionArchivoCompaniaTxb.TabIndex = 15;
            // 
            // AgregarBtn
            // 
            this.AgregarBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.AgregarBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AgregarBtn.ForeColor = System.Drawing.Color.White;
            this.AgregarBtn.Image = global::UniversalTrayApp.Properties.Resources.agregar_24x24_ticopay_02;
            this.AgregarBtn.Location = new System.Drawing.Point(584, 226);
            this.AgregarBtn.Name = "AgregarBtn";
            this.AgregarBtn.Size = new System.Drawing.Size(95, 37);
            this.AgregarBtn.TabIndex = 5;
            this.AgregarBtn.Text = "Agregar";
            this.AgregarBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AgregarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AgregarBtn.UseVisualStyleBackColor = false;
            this.AgregarBtn.Click += new System.EventHandler(this.AgregarBtn_Click);
            // 
            // CancelarBtn
            // 
            this.CancelarBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.CancelarBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelarBtn.ForeColor = System.Drawing.Color.White;
            this.CancelarBtn.Image = global::UniversalTrayApp.Properties.Resources.eliminar_24x24_ticopay_02;
            this.CancelarBtn.Location = new System.Drawing.Point(685, 226);
            this.CancelarBtn.Name = "CancelarBtn";
            this.CancelarBtn.Size = new System.Drawing.Size(97, 37);
            this.CancelarBtn.TabIndex = 4;
            this.CancelarBtn.Text = "Cancelar";
            this.CancelarBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.CancelarBtn.UseVisualStyleBackColor = false;
            this.CancelarBtn.Click += new System.EventHandler(this.CancelarBtn_Click);
            // 
            // ArchivoCompaniaOFD
            // 
            this.ArchivoCompaniaOFD.Filter = "QuickBooks Company File (*.qbw) | *.qbw";
            this.ArchivoCompaniaOFD.Title = "Archivo de la Compañía";
            // 
            // SalesReceiptCheckBox
            // 
            this.SalesReceiptCheckBox.AutoSize = true;
            this.SalesReceiptCheckBox.Location = new System.Drawing.Point(9, 116);
            this.SalesReceiptCheckBox.Name = "SalesReceiptCheckBox";
            this.SalesReceiptCheckBox.Size = new System.Drawing.Size(223, 17);
            this.SalesReceiptCheckBox.TabIndex = 21;
            this.SalesReceiptCheckBox.Text = "Crear los Sales Receipts como Tiquetes ?";
            this.SalesReceiptCheckBox.UseVisualStyleBackColor = true;
            // 
            // QuickbooksEnterpriseDesktop
            // 
            this.AcceptButton = this.AgregarBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelarBtn;
            this.ClientSize = new System.Drawing.Size(800, 270);
            this.Controls.Add(this.AgregarBtn);
            this.Controls.Add(this.CancelarBtn);
            this.Controls.Add(this.DatosQuickbooksGB);
            this.Controls.Add(this.TicopayDataGB);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 309);
            this.MinimumSize = new System.Drawing.Size(816, 309);
            this.Name = "QuickbooksEnterpriseDesktop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurar QuickBooks Enterprise Desktop";
            this.TicopayDataGB.ResumeLayout(false);
            this.TicopayDataGB.PerformLayout();
            this.DatosQuickbooksGB.ResumeLayout(false);
            this.DatosQuickbooksGB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox TicopayDataGB;
        private System.Windows.Forms.GroupBox DatosQuickbooksGB;
        private System.Windows.Forms.TextBox ClaveTxb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox UsuarioTbx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SubDominioTxb;
        private System.Windows.Forms.Button ProbarTicopayBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BuscarBDBtn;
        private System.Windows.Forms.TextBox DireccionArchivoCompaniaTxb;
        private System.Windows.Forms.TextBox IdEmpresaQuickbooksTxb;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button ProbarConexionContaPymeBtn;
        private System.Windows.Forms.Button AgregarBtn;
        private System.Windows.Forms.Button CancelarBtn;
        private System.Windows.Forms.OpenFileDialog ArchivoCompaniaOFD;
        private System.Windows.Forms.CheckBox SalesReceiptCheckBox;
    }
}