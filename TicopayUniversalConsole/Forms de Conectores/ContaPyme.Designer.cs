namespace TicopayUniversalConsole.Forms_de_Conectores
{
    partial class ContaPyme
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContaPyme));
            this.DatosTicopay = new System.Windows.Forms.GroupBox();
            this.ProbarTicopayBtn = new System.Windows.Forms.Button();
            this.ClaveTxb = new System.Windows.Forms.TextBox();
            this.UsuarioTbx = new System.Windows.Forms.TextBox();
            this.SubDominioTxb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DatosContaPyme = new System.Windows.Forms.GroupBox();
            this.IdEmpresaContaPymeTxb = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ProbarConexionContaPymeBtn = new System.Windows.Forms.Button();
            this.IpServidorContaPymeTxb = new System.Windows.Forms.TextBox();
            this.TipoInstalacionContaPymeCmb = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ClaveBdContaPymeTxb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.UsuarioBdContaPymeTxb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BuscarBDBtn = new System.Windows.Forms.Button();
            this.DireccionBdTxb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ArchivoBdContaPyme = new System.Windows.Forms.OpenFileDialog();
            this.CancelarBtn = new System.Windows.Forms.Button();
            this.AgregarBtn = new System.Windows.Forms.Button();
            this.DatosTicopay.SuspendLayout();
            this.DatosContaPyme.SuspendLayout();
            this.SuspendLayout();
            // 
            // DatosTicopay
            // 
            this.DatosTicopay.Controls.Add(this.ProbarTicopayBtn);
            this.DatosTicopay.Controls.Add(this.ClaveTxb);
            this.DatosTicopay.Controls.Add(this.UsuarioTbx);
            this.DatosTicopay.Controls.Add(this.SubDominioTxb);
            this.DatosTicopay.Controls.Add(this.label3);
            this.DatosTicopay.Controls.Add(this.label2);
            this.DatosTicopay.Controls.Add(this.label1);
            this.DatosTicopay.Location = new System.Drawing.Point(12, 12);
            this.DatosTicopay.Name = "DatosTicopay";
            this.DatosTicopay.Size = new System.Drawing.Size(334, 337);
            this.DatosTicopay.TabIndex = 0;
            this.DatosTicopay.TabStop = false;
            this.DatosTicopay.Text = "Datos de Configuración Ticopay";
            // 
            // ProbarTicopayBtn
            // 
            this.ProbarTicopayBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.ProbarTicopayBtn.ForeColor = System.Drawing.Color.White;
            this.ProbarTicopayBtn.Image = global::TicopayUniversalConsole.Properties.Resources.conectar_24x24_ticopay_02;
            this.ProbarTicopayBtn.Location = new System.Drawing.Point(178, 157);
            this.ProbarTicopayBtn.Name = "ProbarTicopayBtn";
            this.ProbarTicopayBtn.Size = new System.Drawing.Size(150, 37);
            this.ProbarTicopayBtn.TabIndex = 8;
            this.ProbarTicopayBtn.Text = "Probar Credenciales";
            this.ProbarTicopayBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProbarTicopayBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ProbarTicopayBtn.UseVisualStyleBackColor = false;
            this.ProbarTicopayBtn.Click += new System.EventHandler(this.ProbarTicopayBtn_Click);
            // 
            // ClaveTxb
            // 
            this.ClaveTxb.Location = new System.Drawing.Point(9, 131);
            this.ClaveTxb.Name = "ClaveTxb";
            this.ClaveTxb.Size = new System.Drawing.Size(319, 20);
            this.ClaveTxb.TabIndex = 7;
            // 
            // UsuarioTbx
            // 
            this.UsuarioTbx.Location = new System.Drawing.Point(9, 87);
            this.UsuarioTbx.Name = "UsuarioTbx";
            this.UsuarioTbx.Size = new System.Drawing.Size(319, 20);
            this.UsuarioTbx.TabIndex = 6;
            // 
            // SubDominioTxb
            // 
            this.SubDominioTxb.Location = new System.Drawing.Point(9, 41);
            this.SubDominioTxb.Name = "SubDominioTxb";
            this.SubDominioTxb.Size = new System.Drawing.Size(319, 20);
            this.SubDominioTxb.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Contraseña del Usuario :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Usuario Correspondiente a ese Sub Dominio :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Sub Dominio o Tenant de Ticopay :";
            // 
            // DatosContaPyme
            // 
            this.DatosContaPyme.Controls.Add(this.IdEmpresaContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.label9);
            this.DatosContaPyme.Controls.Add(this.ProbarConexionContaPymeBtn);
            this.DatosContaPyme.Controls.Add(this.IpServidorContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.TipoInstalacionContaPymeCmb);
            this.DatosContaPyme.Controls.Add(this.label8);
            this.DatosContaPyme.Controls.Add(this.label7);
            this.DatosContaPyme.Controls.Add(this.ClaveBdContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.label6);
            this.DatosContaPyme.Controls.Add(this.UsuarioBdContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.label5);
            this.DatosContaPyme.Controls.Add(this.BuscarBDBtn);
            this.DatosContaPyme.Controls.Add(this.DireccionBdTxb);
            this.DatosContaPyme.Controls.Add(this.label4);
            this.DatosContaPyme.Location = new System.Drawing.Point(352, 12);
            this.DatosContaPyme.Name = "DatosContaPyme";
            this.DatosContaPyme.Size = new System.Drawing.Size(436, 337);
            this.DatosContaPyme.TabIndex = 1;
            this.DatosContaPyme.TabStop = false;
            this.DatosContaPyme.Text = "Datos de Configuración de ContaPyme";
            // 
            // IdEmpresaContaPymeTxb
            // 
            this.IdEmpresaContaPymeTxb.Location = new System.Drawing.Point(9, 269);
            this.IdEmpresaContaPymeTxb.Name = "IdEmpresaContaPymeTxb";
            this.IdEmpresaContaPymeTxb.Size = new System.Drawing.Size(324, 20);
            this.IdEmpresaContaPymeTxb.TabIndex = 13;
            this.IdEmpresaContaPymeTxb.Text = "1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 253);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(154, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "ID de empresa en ContaPyme :";
            // 
            // ProbarConexionContaPymeBtn
            // 
            this.ProbarConexionContaPymeBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.ProbarConexionContaPymeBtn.ForeColor = System.Drawing.Color.White;
            this.ProbarConexionContaPymeBtn.Image = global::TicopayUniversalConsole.Properties.Resources.conectar_24x24_ticopay_02;
            this.ProbarConexionContaPymeBtn.Location = new System.Drawing.Point(297, 294);
            this.ProbarConexionContaPymeBtn.Name = "ProbarConexionContaPymeBtn";
            this.ProbarConexionContaPymeBtn.Size = new System.Drawing.Size(133, 37);
            this.ProbarConexionContaPymeBtn.TabIndex = 11;
            this.ProbarConexionContaPymeBtn.Text = "Probar Conexión";
            this.ProbarConexionContaPymeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProbarConexionContaPymeBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ProbarConexionContaPymeBtn.UseVisualStyleBackColor = false;
            this.ProbarConexionContaPymeBtn.Click += new System.EventHandler(this.ProbarConexionContaPymeBtn_Click);
            // 
            // IpServidorContaPymeTxb
            // 
            this.IpServidorContaPymeTxb.Location = new System.Drawing.Point(9, 226);
            this.IpServidorContaPymeTxb.Name = "IpServidorContaPymeTxb";
            this.IpServidorContaPymeTxb.Size = new System.Drawing.Size(324, 20);
            this.IpServidorContaPymeTxb.TabIndex = 10;
            this.IpServidorContaPymeTxb.Text = "localhost";
            // 
            // TipoInstalacionContaPymeCmb
            // 
            this.TipoInstalacionContaPymeCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TipoInstalacionContaPymeCmb.FormattingEnabled = true;
            this.TipoInstalacionContaPymeCmb.Location = new System.Drawing.Point(9, 176);
            this.TipoInstalacionContaPymeCmb.Name = "TipoInstalacionContaPymeCmb";
            this.TipoInstalacionContaPymeCmb.Size = new System.Drawing.Size(324, 21);
            this.TipoInstalacionContaPymeCmb.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(200, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Dirección IP del servidor de ContaPyme :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 160);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(174, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Tipo de instalación de ContaPyme :";
            // 
            // ClaveBdContaPymeTxb
            // 
            this.ClaveBdContaPymeTxb.Location = new System.Drawing.Point(9, 131);
            this.ClaveBdContaPymeTxb.Name = "ClaveBdContaPymeTxb";
            this.ClaveBdContaPymeTxb.Size = new System.Drawing.Size(324, 20);
            this.ClaveBdContaPymeTxb.TabIndex = 6;
            this.ClaveBdContaPymeTxb.Text = "masterkey";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(289, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Contraseña del usuario de la base de datos de ContaPyme :";
            // 
            // UsuarioBdContaPymeTxb
            // 
            this.UsuarioBdContaPymeTxb.Location = new System.Drawing.Point(9, 87);
            this.UsuarioBdContaPymeTxb.Name = "UsuarioBdContaPymeTxb";
            this.UsuarioBdContaPymeTxb.Size = new System.Drawing.Size(324, 20);
            this.UsuarioBdContaPymeTxb.TabIndex = 4;
            this.UsuarioBdContaPymeTxb.Text = "SYSDBA";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(217, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Usuario de la base de datos de ContaPyme :";
            // 
            // BuscarBDBtn
            // 
            this.BuscarBDBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.BuscarBDBtn.ForeColor = System.Drawing.Color.White;
            this.BuscarBDBtn.Image = global::TicopayUniversalConsole.Properties.Resources.abrir_24x24_ticopay_02;
            this.BuscarBDBtn.Location = new System.Drawing.Point(339, 32);
            this.BuscarBDBtn.Name = "BuscarBDBtn";
            this.BuscarBDBtn.Size = new System.Drawing.Size(91, 37);
            this.BuscarBDBtn.TabIndex = 2;
            this.BuscarBDBtn.Text = "Buscar";
            this.BuscarBDBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BuscarBDBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BuscarBDBtn.UseVisualStyleBackColor = false;
            this.BuscarBDBtn.Click += new System.EventHandler(this.BuscarBDBtn_Click);
            // 
            // DireccionBdTxb
            // 
            this.DireccionBdTxb.Location = new System.Drawing.Point(9, 41);
            this.DireccionBdTxb.Name = "DireccionBdTxb";
            this.DireccionBdTxb.ReadOnly = true;
            this.DireccionBdTxb.Size = new System.Drawing.Size(324, 20);
            this.DireccionBdTxb.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(273, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Ubicación del archivo de base de datos de ContaPyme :";
            // 
            // ArchivoBdContaPyme
            // 
            this.ArchivoBdContaPyme.Filter = "FireBird Database files (*.fdb) | *.fdb";
            this.ArchivoBdContaPyme.Title = "Seleccione el archivo de Base de Datos de ContaPyme";
            // 
            // CancelarBtn
            // 
            this.CancelarBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.CancelarBtn.ForeColor = System.Drawing.Color.White;
            this.CancelarBtn.Image = global::TicopayUniversalConsole.Properties.Resources.eliminar_24x24_ticopay_02;
            this.CancelarBtn.Location = new System.Drawing.Point(691, 355);
            this.CancelarBtn.Name = "CancelarBtn";
            this.CancelarBtn.Size = new System.Drawing.Size(97, 37);
            this.CancelarBtn.TabIndex = 2;
            this.CancelarBtn.Text = "Cancelar";
            this.CancelarBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.CancelarBtn.UseVisualStyleBackColor = false;
            this.CancelarBtn.Click += new System.EventHandler(this.CancelarBtn_Click);
            // 
            // AgregarBtn
            // 
            this.AgregarBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.AgregarBtn.ForeColor = System.Drawing.Color.White;
            this.AgregarBtn.Image = global::TicopayUniversalConsole.Properties.Resources.agregar_24x24_ticopay_02;
            this.AgregarBtn.Location = new System.Drawing.Point(590, 355);
            this.AgregarBtn.Name = "AgregarBtn";
            this.AgregarBtn.Size = new System.Drawing.Size(95, 37);
            this.AgregarBtn.TabIndex = 3;
            this.AgregarBtn.Text = "Agregar";
            this.AgregarBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AgregarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AgregarBtn.UseVisualStyleBackColor = false;
            this.AgregarBtn.Click += new System.EventHandler(this.AgregarBtn_Click);
            // 
            // ContaPyme
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 404);
            this.Controls.Add(this.AgregarBtn);
            this.Controls.Add(this.CancelarBtn);
            this.Controls.Add(this.DatosContaPyme);
            this.Controls.Add(this.DatosTicopay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ContaPyme";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurar Conexión de ContaPyme";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ContaPyme_FormClosed);
            this.DatosTicopay.ResumeLayout(false);
            this.DatosTicopay.PerformLayout();
            this.DatosContaPyme.ResumeLayout(false);
            this.DatosContaPyme.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox DatosTicopay;
        private System.Windows.Forms.GroupBox DatosContaPyme;
        private System.Windows.Forms.Button ProbarTicopayBtn;
        private System.Windows.Forms.TextBox ClaveTxb;
        private System.Windows.Forms.TextBox UsuarioTbx;
        private System.Windows.Forms.TextBox SubDominioTxb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BuscarBDBtn;
        private System.Windows.Forms.TextBox DireccionBdTxb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog ArchivoBdContaPyme;
        private System.Windows.Forms.TextBox ClaveBdContaPymeTxb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox UsuarioBdContaPymeTxb;
        private System.Windows.Forms.TextBox IpServidorContaPymeTxb;
        private System.Windows.Forms.ComboBox TipoInstalacionContaPymeCmb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ProbarConexionContaPymeBtn;
        private System.Windows.Forms.Button CancelarBtn;
        private System.Windows.Forms.Button AgregarBtn;
        private System.Windows.Forms.TextBox IdEmpresaContaPymeTxb;
        private System.Windows.Forms.Label label9;
    }
}