namespace TicopayAdminConsole
{
    partial class PantallaPrincipal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PantallaPrincipal));
            this.pLogin = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.tbClave = new System.Windows.Forms.TextBox();
            this.tbUsuario = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pMonitor = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lDocumentosPendientes = new System.Windows.Forms.Label();
            this.lvEventos = new System.Windows.Forms.ListView();
            this.Descripcion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Tipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Fecha = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UpdateLogButton = new System.Windows.Forms.Button();
            this.lContaPyme = new System.Windows.Forms.GroupBox();
            this.SendDocumentsButton = new System.Windows.Forms.Button();
            this.tbIp = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbServerType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbBdContaPyme = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DisconnectBdButton = new System.Windows.Forms.Button();
            this.ConnectContaPymeButton = new System.Windows.Forms.Button();
            this.tbPasswordBd = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbUsuarioBD = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.OpenBdLocationButton = new System.Windows.Forms.Button();
            this.tbUbicacionBd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbContaPymeConectado = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Test = new System.Windows.Forms.Button();
            this.IniciarButton = new System.Windows.Forms.Button();
            this.DetenerServicioButton = new System.Windows.Forms.Button();
            this.cbServicioActivo = new System.Windows.Forms.CheckBox();
            this.ConfigurarServicioButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cbConfigurado = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OpenFileDialogBd = new System.Windows.Forms.OpenFileDialog();
            this.pLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pMonitor.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.lContaPyme.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pLogin
            // 
            this.pLogin.Controls.Add(this.pictureBox1);
            this.pLogin.Controls.Add(this.LoginButton);
            this.pLogin.Controls.Add(this.tbClave);
            this.pLogin.Controls.Add(this.tbUsuario);
            this.pLogin.Controls.Add(this.label2);
            this.pLogin.Controls.Add(this.label1);
            this.pLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pLogin.Location = new System.Drawing.Point(0, 0);
            this.pLogin.Name = "pLogin";
            this.pLogin.Size = new System.Drawing.Size(984, 511);
            this.pLogin.TabIndex = 0;
            this.pLogin.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TicopayAdminConsole.Properties.Resources.TicopayVertical;
            this.pictureBox1.Location = new System.Drawing.Point(357, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(270, 258);
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // LoginButton
            // 
            this.LoginButton.BackColor = System.Drawing.Color.ForestGreen;
            this.LoginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginButton.ForeColor = System.Drawing.Color.White;
            this.LoginButton.Location = new System.Drawing.Point(438, 418);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(108, 38);
            this.LoginButton.TabIndex = 10;
            this.LoginButton.Text = "Ingresar";
            this.LoginButton.UseVisualStyleBackColor = false;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // tbClave
            // 
            this.tbClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbClave.Location = new System.Drawing.Point(405, 375);
            this.tbClave.Name = "tbClave";
            this.tbClave.PasswordChar = '*';
            this.tbClave.Size = new System.Drawing.Size(174, 29);
            this.tbClave.TabIndex = 9;
            this.tbClave.Text = "P@ssw0rd";
            this.tbClave.UseSystemPasswordChar = true;
            // 
            // tbUsuario
            // 
            this.tbUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbUsuario.Location = new System.Drawing.Point(405, 316);
            this.tbUsuario.Name = "tbUsuario";
            this.tbUsuario.Size = new System.Drawing.Size(174, 29);
            this.tbUsuario.TabIndex = 8;
            this.tbUsuario.Text = "admin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(401, 348);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Contraseña";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(401, 289);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 22);
            this.label1.TabIndex = 6;
            this.label1.Text = "Usuario";
            // 
            // pMonitor
            // 
            this.pMonitor.Controls.Add(this.groupBox1);
            this.pMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMonitor.Location = new System.Drawing.Point(0, 0);
            this.pMonitor.Name = "pMonitor";
            this.pMonitor.Size = new System.Drawing.Size(984, 511);
            this.pMonitor.TabIndex = 1;
            this.pMonitor.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.lContaPyme);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(960, 490);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Estatus";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lDocumentosPendientes);
            this.groupBox3.Controls.Add(this.lvEventos);
            this.groupBox3.Controls.Add(this.UpdateLogButton);
            this.groupBox3.Location = new System.Drawing.Point(448, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(506, 453);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Registro de operacion TicopayMiddleService";
            // 
            // lDocumentosPendientes
            // 
            this.lDocumentosPendientes.AutoSize = true;
            this.lDocumentosPendientes.Location = new System.Drawing.Point(248, 33);
            this.lDocumentosPendientes.Name = "lDocumentosPendientes";
            this.lDocumentosPendientes.Size = new System.Drawing.Size(241, 17);
            this.lDocumentosPendientes.TabIndex = 2;
            this.lDocumentosPendientes.Text = "0 Documentos pendientes por enviar";
            // 
            // lvEventos
            // 
            this.lvEventos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Descripcion,
            this.Tipo,
            this.Fecha});
            this.lvEventos.FullRowSelect = true;
            this.lvEventos.GridLines = true;
            this.lvEventos.Location = new System.Drawing.Point(6, 76);
            this.lvEventos.Name = "lvEventos";
            this.lvEventos.Size = new System.Drawing.Size(494, 371);
            this.lvEventos.TabIndex = 1;
            this.lvEventos.UseCompatibleStateImageBehavior = false;
            this.lvEventos.View = System.Windows.Forms.View.Details;
            // 
            // Descripcion
            // 
            this.Descripcion.Text = "Descripción";
            this.Descripcion.Width = 300;
            // 
            // Tipo
            // 
            this.Tipo.Text = "Tipo";
            this.Tipo.Width = 98;
            // 
            // Fecha
            // 
            this.Fecha.Text = "Fecha";
            this.Fecha.Width = 91;
            // 
            // UpdateLogButton
            // 
            this.UpdateLogButton.BackColor = System.Drawing.Color.ForestGreen;
            this.UpdateLogButton.ForeColor = System.Drawing.Color.White;
            this.UpdateLogButton.Location = new System.Drawing.Point(6, 24);
            this.UpdateLogButton.Name = "UpdateLogButton";
            this.UpdateLogButton.Size = new System.Drawing.Size(218, 35);
            this.UpdateLogButton.TabIndex = 0;
            this.UpdateLogButton.Text = "Actualizar registro de eventos";
            this.UpdateLogButton.UseVisualStyleBackColor = false;
            this.UpdateLogButton.Click += new System.EventHandler(this.UpdateLogButton_Click);
            // 
            // lContaPyme
            // 
            this.lContaPyme.Controls.Add(this.SendDocumentsButton);
            this.lContaPyme.Controls.Add(this.tbIp);
            this.lContaPyme.Controls.Add(this.label11);
            this.lContaPyme.Controls.Add(this.cmbServerType);
            this.lContaPyme.Controls.Add(this.label10);
            this.lContaPyme.Controls.Add(this.cbBdContaPyme);
            this.lContaPyme.Controls.Add(this.label9);
            this.lContaPyme.Controls.Add(this.DisconnectBdButton);
            this.lContaPyme.Controls.Add(this.ConnectContaPymeButton);
            this.lContaPyme.Controls.Add(this.tbPasswordBd);
            this.lContaPyme.Controls.Add(this.label8);
            this.lContaPyme.Controls.Add(this.tbUsuarioBD);
            this.lContaPyme.Controls.Add(this.label7);
            this.lContaPyme.Controls.Add(this.OpenBdLocationButton);
            this.lContaPyme.Controls.Add(this.tbUbicacionBd);
            this.lContaPyme.Controls.Add(this.label6);
            this.lContaPyme.Controls.Add(this.cbContaPymeConectado);
            this.lContaPyme.Controls.Add(this.label5);
            this.lContaPyme.Location = new System.Drawing.Point(6, 144);
            this.lContaPyme.Name = "lContaPyme";
            this.lContaPyme.Size = new System.Drawing.Size(436, 334);
            this.lContaPyme.TabIndex = 5;
            this.lContaPyme.TabStop = false;
            this.lContaPyme.Text = "ContaPyme";
            // 
            // SendDocumentsButton
            // 
            this.SendDocumentsButton.BackColor = System.Drawing.Color.ForestGreen;
            this.SendDocumentsButton.Enabled = false;
            this.SendDocumentsButton.ForeColor = System.Drawing.Color.White;
            this.SendDocumentsButton.Location = new System.Drawing.Point(267, 290);
            this.SendDocumentsButton.Name = "SendDocumentsButton";
            this.SendDocumentsButton.Size = new System.Drawing.Size(163, 35);
            this.SendDocumentsButton.TabIndex = 14;
            this.SendDocumentsButton.Text = "Procesar Documentos";
            this.SendDocumentsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SendDocumentsButton.UseVisualStyleBackColor = false;
            this.SendDocumentsButton.Click += new System.EventHandler(this.SendDocumentsButton_Click);
            // 
            // tbIp
            // 
            this.tbIp.Location = new System.Drawing.Point(235, 261);
            this.tbIp.Name = "tbIp";
            this.tbIp.ReadOnly = true;
            this.tbIp.Size = new System.Drawing.Size(153, 23);
            this.tbIp.TabIndex = 13;
            this.tbIp.Text = "localhost";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 264);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(223, 17);
            this.label11.TabIndex = 12;
            this.label11.Text = "Dirección Ip Servidor ContaPyme :";
            // 
            // cmbServerType
            // 
            this.cmbServerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServerType.Enabled = false;
            this.cmbServerType.FormattingEnabled = true;
            this.cmbServerType.Items.AddRange(new object[] {
            "Mono Usuario",
            "Multi Usuario"});
            this.cmbServerType.Location = new System.Drawing.Point(185, 219);
            this.cmbServerType.Name = "cmbServerType";
            this.cmbServerType.Size = new System.Drawing.Size(203, 24);
            this.cmbServerType.TabIndex = 11;
            this.cmbServerType.SelectedValueChanged += new System.EventHandler(this.cmbServerType_SelectedValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(48, 222);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(135, 17);
            this.label10.TabIndex = 10;
            this.label10.Text = "Tipo de Instalación :";
            // 
            // cbBdContaPyme
            // 
            this.cbBdContaPyme.AutoSize = true;
            this.cbBdContaPyme.Enabled = false;
            this.cbBdContaPyme.Location = new System.Drawing.Point(299, 30);
            this.cbBdContaPyme.Name = "cbBdContaPyme";
            this.cbBdContaPyme.Size = new System.Drawing.Size(15, 14);
            this.cbBdContaPyme.TabIndex = 9;
            this.cbBdContaPyme.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(7, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(285, 17);
            this.label9.TabIndex = 5;
            this.label9.Text = "Credenciales BD ContaPyme Configuradas :";
            // 
            // DisconnectBdButton
            // 
            this.DisconnectBdButton.BackColor = System.Drawing.Color.ForestGreen;
            this.DisconnectBdButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DisconnectBdButton.Enabled = false;
            this.DisconnectBdButton.ForeColor = System.Drawing.Color.White;
            this.DisconnectBdButton.Image = global::TicopayAdminConsole.Properties.Resources.Delete16;
            this.DisconnectBdButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DisconnectBdButton.Location = new System.Drawing.Point(10, 290);
            this.DisconnectBdButton.Name = "DisconnectBdButton";
            this.DisconnectBdButton.Size = new System.Drawing.Size(153, 35);
            this.DisconnectBdButton.TabIndex = 8;
            this.DisconnectBdButton.Text = "Eliminar Conexión";
            this.DisconnectBdButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DisconnectBdButton.UseVisualStyleBackColor = false;
            this.DisconnectBdButton.Click += new System.EventHandler(this.DisconnectBdButton_Click);
            // 
            // ConnectContaPymeButton
            // 
            this.ConnectContaPymeButton.BackColor = System.Drawing.Color.ForestGreen;
            this.ConnectContaPymeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ConnectContaPymeButton.Enabled = false;
            this.ConnectContaPymeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectContaPymeButton.ForeColor = System.Drawing.Color.White;
            this.ConnectContaPymeButton.Image = global::TicopayAdminConsole.Properties.Resources.Db16;
            this.ConnectContaPymeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ConnectContaPymeButton.Location = new System.Drawing.Point(169, 290);
            this.ConnectContaPymeButton.Name = "ConnectContaPymeButton";
            this.ConnectContaPymeButton.Size = new System.Drawing.Size(92, 35);
            this.ConnectContaPymeButton.TabIndex = 5;
            this.ConnectContaPymeButton.Text = "Conectar";
            this.ConnectContaPymeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ConnectContaPymeButton.UseVisualStyleBackColor = false;
            this.ConnectContaPymeButton.Click += new System.EventHandler(this.ConnectContaPymeButton_Click);
            // 
            // tbPasswordBd
            // 
            this.tbPasswordBd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPasswordBd.Location = new System.Drawing.Point(185, 180);
            this.tbPasswordBd.Name = "tbPasswordBd";
            this.tbPasswordBd.ReadOnly = true;
            this.tbPasswordBd.Size = new System.Drawing.Size(203, 23);
            this.tbPasswordBd.TabIndex = 5;
            this.tbPasswordBd.Text = "masterkey";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(7, 183);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 17);
            this.label8.TabIndex = 5;
            this.label8.Text = "Password BD ContaPyme :";
            // 
            // tbUsuarioBD
            // 
            this.tbUsuarioBD.Location = new System.Drawing.Point(185, 142);
            this.tbUsuarioBD.Name = "tbUsuarioBD";
            this.tbUsuarioBD.ReadOnly = true;
            this.tbUsuarioBD.Size = new System.Drawing.Size(203, 23);
            this.tbUsuarioBD.TabIndex = 7;
            this.tbUsuarioBD.Text = "SYSDBA";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(162, 17);
            this.label7.TabIndex = 5;
            this.label7.Text = "Usuario Bd ContaPyme :";
            // 
            // OpenBdLocationButton
            // 
            this.OpenBdLocationButton.BackColor = System.Drawing.Color.ForestGreen;
            this.OpenBdLocationButton.Enabled = false;
            this.OpenBdLocationButton.ForeColor = System.Drawing.Color.White;
            this.OpenBdLocationButton.Image = global::TicopayAdminConsole.Properties.Resources.Open16;
            this.OpenBdLocationButton.Location = new System.Drawing.Point(394, 101);
            this.OpenBdLocationButton.Name = "OpenBdLocationButton";
            this.OpenBdLocationButton.Size = new System.Drawing.Size(36, 35);
            this.OpenBdLocationButton.TabIndex = 6;
            this.OpenBdLocationButton.UseVisualStyleBackColor = false;
            this.OpenBdLocationButton.Click += new System.EventHandler(this.OpenBdLocationButton_Click);
            // 
            // tbUbicacionBd
            // 
            this.tbUbicacionBd.Location = new System.Drawing.Point(9, 107);
            this.tbUbicacionBd.Name = "tbUbicacionBd";
            this.tbUbicacionBd.ReadOnly = true;
            this.tbUbicacionBd.Size = new System.Drawing.Size(379, 23);
            this.tbUbicacionBd.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(95, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(197, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Ubicación de BD ContaPyme :";
            // 
            // cbContaPymeConectado
            // 
            this.cbContaPymeConectado.AutoSize = true;
            this.cbContaPymeConectado.Enabled = false;
            this.cbContaPymeConectado.Location = new System.Drawing.Point(299, 61);
            this.cbContaPymeConectado.Name = "cbContaPymeConectado";
            this.cbContaPymeConectado.Size = new System.Drawing.Size(15, 14);
            this.cbContaPymeConectado.TabIndex = 5;
            this.cbContaPymeConectado.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(119, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(173, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Acceso a DB ContaPyme :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Test);
            this.groupBox2.Controls.Add(this.IniciarButton);
            this.groupBox2.Controls.Add(this.DetenerServicioButton);
            this.groupBox2.Controls.Add(this.cbServicioActivo);
            this.groupBox2.Controls.Add(this.ConfigurarServicioButton);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbConfigurado);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(6, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(436, 116);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ticopay";
            // 
            // Test
            // 
            this.Test.Location = new System.Drawing.Point(334, 68);
            this.Test.Name = "Test";
            this.Test.Size = new System.Drawing.Size(71, 35);
            this.Test.TabIndex = 6;
            this.Test.Text = "Test";
            this.Test.UseVisualStyleBackColor = true;
            this.Test.Click += new System.EventHandler(this.Test_Click);
            // 
            // IniciarButton
            // 
            this.IniciarButton.BackColor = System.Drawing.Color.ForestGreen;
            this.IniciarButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.IniciarButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IniciarButton.ForeColor = System.Drawing.Color.White;
            this.IniciarButton.Image = global::TicopayAdminConsole.Properties.Resources.Start16;
            this.IniciarButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IniciarButton.Location = new System.Drawing.Point(330, 24);
            this.IniciarButton.Name = "IniciarButton";
            this.IniciarButton.Size = new System.Drawing.Size(75, 35);
            this.IniciarButton.TabIndex = 5;
            this.IniciarButton.Text = "Iniciar";
            this.IniciarButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IniciarButton.UseVisualStyleBackColor = false;
            this.IniciarButton.Click += new System.EventHandler(this.IniciarButton_Click);
            // 
            // DetenerServicioButton
            // 
            this.DetenerServicioButton.BackColor = System.Drawing.Color.ForestGreen;
            this.DetenerServicioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DetenerServicioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DetenerServicioButton.ForeColor = System.Drawing.Color.White;
            this.DetenerServicioButton.Image = global::TicopayAdminConsole.Properties.Resources.Stop24;
            this.DetenerServicioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DetenerServicioButton.Location = new System.Drawing.Point(222, 24);
            this.DetenerServicioButton.Name = "DetenerServicioButton";
            this.DetenerServicioButton.Size = new System.Drawing.Size(92, 35);
            this.DetenerServicioButton.TabIndex = 5;
            this.DetenerServicioButton.Text = "Detener";
            this.DetenerServicioButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DetenerServicioButton.UseVisualStyleBackColor = false;
            this.DetenerServicioButton.Click += new System.EventHandler(this.DetenerServicioButton_Click);
            // 
            // cbServicioActivo
            // 
            this.cbServicioActivo.AutoSize = true;
            this.cbServicioActivo.Enabled = false;
            this.cbServicioActivo.Location = new System.Drawing.Point(186, 35);
            this.cbServicioActivo.Name = "cbServicioActivo";
            this.cbServicioActivo.Size = new System.Drawing.Size(15, 14);
            this.cbServicioActivo.TabIndex = 5;
            this.cbServicioActivo.UseVisualStyleBackColor = true;
            this.cbServicioActivo.CheckedChanged += new System.EventHandler(this.cbServicioActivo_CheckedChanged);
            // 
            // ConfigurarServicioButton
            // 
            this.ConfigurarServicioButton.BackColor = System.Drawing.Color.ForestGreen;
            this.ConfigurarServicioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ConfigurarServicioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConfigurarServicioButton.ForeColor = System.Drawing.Color.White;
            this.ConfigurarServicioButton.Image = global::TicopayAdminConsole.Properties.Resources.Config24;
            this.ConfigurarServicioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ConfigurarServicioButton.Location = new System.Drawing.Point(222, 68);
            this.ConfigurarServicioButton.Name = "ConfigurarServicioButton";
            this.ConfigurarServicioButton.Size = new System.Drawing.Size(106, 35);
            this.ConfigurarServicioButton.TabIndex = 2;
            this.ConfigurarServicioButton.Text = "Configurar";
            this.ConfigurarServicioButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ConfigurarServicioButton.UseVisualStyleBackColor = false;
            this.ConfigurarServicioButton.Click += new System.EventHandler(this.ConfigurarServicioButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Servicio de enlace Activo :";
            // 
            // cbConfigurado
            // 
            this.cbConfigurado.AutoSize = true;
            this.cbConfigurado.Enabled = false;
            this.cbConfigurado.Location = new System.Drawing.Point(185, 79);
            this.cbConfigurado.Name = "cbConfigurado";
            this.cbConfigurado.Size = new System.Drawing.Size(15, 14);
            this.cbConfigurado.TabIndex = 1;
            this.cbConfigurado.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Servicio Configurado :";
            // 
            // OpenFileDialogBd
            // 
            this.OpenFileDialogBd.DefaultExt = "fdb";
            this.OpenFileDialogBd.Filter = "FireBird Database files (*.fdb) | *.fdb";
            this.OpenFileDialogBd.RestoreDirectory = true;
            this.OpenFileDialogBd.Title = "Seleccione el archivo de BD de ContaPyme";
            // 
            // PantallaPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 511);
            this.Controls.Add(this.pMonitor);
            this.Controls.Add(this.pLogin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 550);
            this.MinimumSize = new System.Drawing.Size(1000, 550);
            this.Name = "PantallaPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consola de Administracion de Ticopay-ContaPyme Service";
            this.pLogin.ResumeLayout(false);
            this.pLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pMonitor.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.lContaPyme.ResumeLayout(false);
            this.lContaPyme.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pLogin;
        private System.Windows.Forms.Panel pMonitor;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.TextBox tbClave;
        private System.Windows.Forms.TextBox tbUsuario;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cbConfigurado;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ConfigurarServicioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox lContaPyme;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button IniciarButton;
        private System.Windows.Forms.Button DetenerServicioButton;
        private System.Windows.Forms.CheckBox cbServicioActivo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ConnectContaPymeButton;
        private System.Windows.Forms.TextBox tbPasswordBd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbUsuarioBD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button OpenBdLocationButton;
        private System.Windows.Forms.TextBox tbUbicacionBd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbContaPymeConectado;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button DisconnectBdButton;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogBd;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbBdContaPyme;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button UpdateLogButton;
        private System.Windows.Forms.ListView lvEventos;
        private System.Windows.Forms.ColumnHeader Descripcion;
        private System.Windows.Forms.ColumnHeader Tipo;
        private System.Windows.Forms.ColumnHeader Fecha;
        private System.Windows.Forms.TextBox tbIp;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbServerType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button SendDocumentsButton;
        private System.Windows.Forms.Label lDocumentosPendientes;
        private System.Windows.Forms.Button Test;
    }
}