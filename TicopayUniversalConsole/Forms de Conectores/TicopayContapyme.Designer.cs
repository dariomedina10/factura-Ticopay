namespace TicopayUniversalConsole.Forms_de_Conectores
{
    partial class TicopayContaPyme
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TicopayContaPyme));
            this.DatosTicopay = new System.Windows.Forms.GroupBox();
            this.ProbarTicopayBtn = new System.Windows.Forms.Button();
            this.ClaveTxb = new System.Windows.Forms.TextBox();
            this.UsuarioTbx = new System.Windows.Forms.TextBox();
            this.SubDominioTxb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DatosContaPyme = new System.Windows.Forms.GroupBox();
            this.TBIdApp = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ProbarConexionContaPymeBtn = new System.Windows.Forms.Button();
            this.IpServidorContaPymeTxb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ClaveBdContaPymeTxb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.UsuarioBdContaPymeTxb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ArchivoBdContaPyme = new System.Windows.Forms.OpenFileDialog();
            this.CancelarBtn = new System.Windows.Forms.Button();
            this.AgregarBtn = new System.Windows.Forms.Button();
            this.TBIdMaquina = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.TBIdEmpresa = new System.Windows.Forms.TextBox();
            this.TBCodigoCentroCostos = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.TBCodigoLiquidacionIva = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.TBCodigoCuentaCaja = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.TBCodigoCuentaBanco = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.TBCodigoCuentasXCobrar = new System.Windows.Forms.TextBox();
            this.DatosTicopay.SuspendLayout();
            this.DatosContaPyme.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.DatosTicopay.Location = new System.Drawing.Point(16, 15);
            this.DatosTicopay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatosTicopay.Name = "DatosTicopay";
            this.DatosTicopay.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatosTicopay.Size = new System.Drawing.Size(445, 265);
            this.DatosTicopay.TabIndex = 0;
            this.DatosTicopay.TabStop = false;
            this.DatosTicopay.Text = "Datos de Configuración Ticopay";
            // 
            // ProbarTicopayBtn
            // 
            this.ProbarTicopayBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.ProbarTicopayBtn.ForeColor = System.Drawing.Color.White;
            this.ProbarTicopayBtn.Image = global::TicopayUniversalConsole.Properties.Resources.conectar_24x24_ticopay_02;
            this.ProbarTicopayBtn.Location = new System.Drawing.Point(237, 193);
            this.ProbarTicopayBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ProbarTicopayBtn.Name = "ProbarTicopayBtn";
            this.ProbarTicopayBtn.Size = new System.Drawing.Size(200, 46);
            this.ProbarTicopayBtn.TabIndex = 8;
            this.ProbarTicopayBtn.Text = "Probar Credenciales";
            this.ProbarTicopayBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProbarTicopayBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ProbarTicopayBtn.UseVisualStyleBackColor = false;
            this.ProbarTicopayBtn.Click += new System.EventHandler(this.ProbarTicopayBtn_Click);
            // 
            // ClaveTxb
            // 
            this.ClaveTxb.Location = new System.Drawing.Point(12, 161);
            this.ClaveTxb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ClaveTxb.Name = "ClaveTxb";
            this.ClaveTxb.Size = new System.Drawing.Size(424, 22);
            this.ClaveTxb.TabIndex = 7;
            // 
            // UsuarioTbx
            // 
            this.UsuarioTbx.Location = new System.Drawing.Point(12, 107);
            this.UsuarioTbx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UsuarioTbx.Name = "UsuarioTbx";
            this.UsuarioTbx.Size = new System.Drawing.Size(424, 22);
            this.UsuarioTbx.TabIndex = 6;
            // 
            // SubDominioTxb
            // 
            this.SubDominioTxb.Location = new System.Drawing.Point(12, 50);
            this.SubDominioTxb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SubDominioTxb.Name = "SubDominioTxb";
            this.SubDominioTxb.Size = new System.Drawing.Size(424, 22);
            this.SubDominioTxb.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 142);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(165, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Contraseña del Usuario :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(297, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Usuario Correspondiente a ese Sub Dominio :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Sub Dominio o Tenant de Ticopay :";
            // 
            // DatosContaPyme
            // 
            this.DatosContaPyme.Controls.Add(this.TBIdEmpresa);
            this.DatosContaPyme.Controls.Add(this.label10);
            this.DatosContaPyme.Controls.Add(this.TBIdMaquina);
            this.DatosContaPyme.Controls.Add(this.TBIdApp);
            this.DatosContaPyme.Controls.Add(this.label9);
            this.DatosContaPyme.Controls.Add(this.ProbarConexionContaPymeBtn);
            this.DatosContaPyme.Controls.Add(this.IpServidorContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.label8);
            this.DatosContaPyme.Controls.Add(this.label7);
            this.DatosContaPyme.Controls.Add(this.ClaveBdContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.label6);
            this.DatosContaPyme.Controls.Add(this.UsuarioBdContaPymeTxb);
            this.DatosContaPyme.Controls.Add(this.label5);
            this.DatosContaPyme.Location = new System.Drawing.Point(469, 15);
            this.DatosContaPyme.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatosContaPyme.Name = "DatosContaPyme";
            this.DatosContaPyme.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatosContaPyme.Size = new System.Drawing.Size(457, 441);
            this.DatosContaPyme.TabIndex = 1;
            this.DatosContaPyme.TabStop = false;
            this.DatosContaPyme.Text = "Datos de Configuración del Api ContaPyme";
            // 
            // TBIdApp
            // 
            this.TBIdApp.Location = new System.Drawing.Point(11, 272);
            this.TBIdApp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TBIdApp.Name = "TBIdApp";
            this.TBIdApp.Size = new System.Drawing.Size(431, 22);
            this.TBIdApp.TabIndex = 13;
            this.TBIdApp.Text = "1001";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 251);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 17);
            this.label9.TabIndex = 12;
            this.label9.Text = "Id de la Aplicacion :";
            // 
            // ProbarConexionContaPymeBtn
            // 
            this.ProbarConexionContaPymeBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.ProbarConexionContaPymeBtn.ForeColor = System.Drawing.Color.White;
            this.ProbarConexionContaPymeBtn.Image = global::TicopayUniversalConsole.Properties.Resources.conectar_24x24_ticopay_02;
            this.ProbarConexionContaPymeBtn.Location = new System.Drawing.Point(266, 372);
            this.ProbarConexionContaPymeBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ProbarConexionContaPymeBtn.Name = "ProbarConexionContaPymeBtn";
            this.ProbarConexionContaPymeBtn.Size = new System.Drawing.Size(177, 46);
            this.ProbarConexionContaPymeBtn.TabIndex = 11;
            this.ProbarConexionContaPymeBtn.Text = "Probar Conexión";
            this.ProbarConexionContaPymeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ProbarConexionContaPymeBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ProbarConexionContaPymeBtn.UseVisualStyleBackColor = false;
            this.ProbarConexionContaPymeBtn.Click += new System.EventHandler(this.ProbarConexionContaPymeBtn_Click);
            // 
            // IpServidorContaPymeTxb
            // 
            this.IpServidorContaPymeTxb.Location = new System.Drawing.Point(12, 51);
            this.IpServidorContaPymeTxb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.IpServidorContaPymeTxb.Name = "IpServidorContaPymeTxb";
            this.IpServidorContaPymeTxb.Size = new System.Drawing.Size(431, 22);
            this.IpServidorContaPymeTxb.TabIndex = 10;
            this.IpServidorContaPymeTxb.Text = "http://localhost:9000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 31);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(265, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "Dirección IP del servidor de ContaPyme :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 197);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 17);
            this.label7.TabIndex = 7;
            this.label7.Text = "Id de la maquina :";
            // 
            // ClaveBdContaPymeTxb
            // 
            this.ClaveBdContaPymeTxb.Location = new System.Drawing.Point(12, 161);
            this.ClaveBdContaPymeTxb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ClaveBdContaPymeTxb.Name = "ClaveBdContaPymeTxb";
            this.ClaveBdContaPymeTxb.Size = new System.Drawing.Size(431, 22);
            this.ClaveBdContaPymeTxb.TabIndex = 6;
            this.ClaveBdContaPymeTxb.Text = "123";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 142);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(210, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Contraseña del usuario del Api :";
            // 
            // UsuarioBdContaPymeTxb
            // 
            this.UsuarioBdContaPymeTxb.Location = new System.Drawing.Point(12, 107);
            this.UsuarioBdContaPymeTxb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UsuarioBdContaPymeTxb.Name = "UsuarioBdContaPymeTxb";
            this.UsuarioBdContaPymeTxb.Size = new System.Drawing.Size(431, 22);
            this.UsuarioBdContaPymeTxb.TabIndex = 4;
            this.UsuarioBdContaPymeTxb.Text = "desarrollo@asadacloud.com";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 87);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(168, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "Email de usuario del Api :";
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
            this.CancelarBtn.Location = new System.Drawing.Point(782, 532);
            this.CancelarBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CancelarBtn.Name = "CancelarBtn";
            this.CancelarBtn.Size = new System.Drawing.Size(129, 46);
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
            this.AgregarBtn.Location = new System.Drawing.Point(647, 534);
            this.AgregarBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AgregarBtn.Name = "AgregarBtn";
            this.AgregarBtn.Size = new System.Drawing.Size(127, 46);
            this.AgregarBtn.TabIndex = 3;
            this.AgregarBtn.Text = "Agregar";
            this.AgregarBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AgregarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AgregarBtn.UseVisualStyleBackColor = false;
            this.AgregarBtn.Click += new System.EventHandler(this.AgregarBtn_Click);
            // 
            // TBIdMaquina
            // 
            this.TBIdMaquina.Location = new System.Drawing.Point(12, 217);
            this.TBIdMaquina.Name = "TBIdMaquina";
            this.TBIdMaquina.Size = new System.Drawing.Size(431, 22);
            this.TBIdMaquina.TabIndex = 14;
            this.TBIdMaquina.Text = "//";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TBCodigoCuentasXCobrar);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.TBCodigoCuentaBanco);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.TBCodigoCuentaCaja);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.TBCodigoLiquidacionIva);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.TBCodigoCentroCostos);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(16, 287);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(445, 302);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Datos de Cuentas Contables de Contapyme :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(263, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Codigo de cuenta del Centro de Costos :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 311);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 17);
            this.label10.TabIndex = 15;
            this.label10.Text = "Id de Empresa :";
            // 
            // TBIdEmpresa
            // 
            this.TBIdEmpresa.Location = new System.Drawing.Point(12, 331);
            this.TBIdEmpresa.Name = "TBIdEmpresa";
            this.TBIdEmpresa.Size = new System.Drawing.Size(430, 22);
            this.TBIdEmpresa.TabIndex = 16;
            this.TBIdEmpresa.Text = "1";
            // 
            // TBCodigoCentroCostos
            // 
            this.TBCodigoCentroCostos.Location = new System.Drawing.Point(12, 59);
            this.TBCodigoCentroCostos.Name = "TBCodigoCentroCostos";
            this.TBCodigoCentroCostos.Size = new System.Drawing.Size(424, 22);
            this.TBCodigoCentroCostos.TabIndex = 1;
            this.TBCodigoCentroCostos.Text = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(293, 17);
            this.label11.TabIndex = 2;
            this.label11.Text = "Codigo de cuenta de liquidacion Impuesto Iva";
            // 
            // TBCodigoLiquidacionIva
            // 
            this.TBCodigoLiquidacionIva.Location = new System.Drawing.Point(11, 112);
            this.TBCodigoLiquidacionIva.Name = "TBCodigoLiquidacionIva";
            this.TBCodigoLiquidacionIva.Size = new System.Drawing.Size(425, 22);
            this.TBCodigoLiquidacionIva.TabIndex = 3;
            this.TBCodigoLiquidacionIva.Text = "2408";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 144);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(306, 17);
            this.label12.TabIndex = 4;
            this.label12.Text = "Codigo de cuenta de Caja (Pagos en efectivo) :";
            // 
            // TBCodigoCuentaCaja
            // 
            this.TBCodigoCuentaCaja.Location = new System.Drawing.Point(12, 164);
            this.TBCodigoCuentaCaja.Name = "TBCodigoCuentaCaja";
            this.TBCodigoCuentaCaja.Size = new System.Drawing.Size(425, 22);
            this.TBCodigoCuentaCaja.TabIndex = 5;
            this.TBCodigoCuentaCaja.Text = "1105";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 193);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(426, 17);
            this.label13.TabIndex = 6;
            this.label13.Text = "Codigo de cuenta de Banco (Pagos en Cheq, Dep , Trans , Tarj ) :";
            // 
            // TBCodigoCuentaBanco
            // 
            this.TBCodigoCuentaBanco.Location = new System.Drawing.Point(12, 213);
            this.TBCodigoCuentaBanco.Name = "TBCodigoCuentaBanco";
            this.TBCodigoCuentaBanco.Size = new System.Drawing.Size(425, 22);
            this.TBCodigoCuentaBanco.TabIndex = 7;
            this.TBCodigoCuentaBanco.Text = "1110";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 245);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(314, 17);
            this.label14.TabIndex = 8;
            this.label14.Text = "Codigo de cuenta Credito (Cuentas por Cobrar) :";
            // 
            // TBCodigoCuentasXCobrar
            // 
            this.TBCodigoCuentasXCobrar.Location = new System.Drawing.Point(12, 265);
            this.TBCodigoCuentasXCobrar.Name = "TBCodigoCuentasXCobrar";
            this.TBCodigoCuentasXCobrar.Size = new System.Drawing.Size(423, 22);
            this.TBCodigoCuentasXCobrar.TabIndex = 9;
            this.TBCodigoCuentasXCobrar.Text = "1305";
            // 
            // TicopayContaPyme
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 593);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.AgregarBtn);
            this.Controls.Add(this.CancelarBtn);
            this.Controls.Add(this.DatosContaPyme);
            this.Controls.Add(this.DatosTicopay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TicopayContaPyme";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurar Conexión de ContaPyme";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ContaPyme_FormClosed);
            this.DatosTicopay.ResumeLayout(false);
            this.DatosTicopay.PerformLayout();
            this.DatosContaPyme.ResumeLayout(false);
            this.DatosContaPyme.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog ArchivoBdContaPyme;
        private System.Windows.Forms.TextBox ClaveBdContaPymeTxb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox UsuarioBdContaPymeTxb;
        private System.Windows.Forms.TextBox IpServidorContaPymeTxb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ProbarConexionContaPymeBtn;
        private System.Windows.Forms.Button CancelarBtn;
        private System.Windows.Forms.Button AgregarBtn;
        private System.Windows.Forms.TextBox TBIdApp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TBIdMaquina;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TBIdEmpresa;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox TBCodigoCentroCostos;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TBCodigoLiquidacionIva;
        private System.Windows.Forms.TextBox TBCodigoCuentaCaja;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox TBCodigoCuentaBanco;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox TBCodigoCuentasXCobrar;
        private System.Windows.Forms.Label label14;
    }
}