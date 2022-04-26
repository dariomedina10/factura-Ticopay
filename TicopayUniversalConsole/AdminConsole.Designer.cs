namespace TicopayUniversalConsole
{
    partial class AdminConsole
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminConsole));
            this.ConfiguracionBox = new System.Windows.Forms.GroupBox();
            this.EjecucionForzadaBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TiposDeConectorCb = new System.Windows.Forms.ComboBox();
            this.EliminarConectorBt = new System.Windows.Forms.Button();
            this.AgregarConectorBt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.DetenerServicioBt = new System.Windows.Forms.Button();
            this.IniciarServicioBt = new System.Windows.Forms.Button();
            this.ServicioActivo = new System.Windows.Forms.CheckBox();
            this.lvConfiguraciones = new System.Windows.Forms.ListView();
            this.JobId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SubDominio = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Usuario = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TipoConectorConfigurado = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EventosBox = new System.Windows.Forms.GroupBox();
            this.GenerarReportesButton = new System.Windows.Forms.Button();
            this.ActualizarEventosBt = new System.Windows.Forms.Button();
            this.lvEventos = new System.Windows.Forms.ListView();
            this.Descripcion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Tipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Fecha = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfiguracionBox.SuspendLayout();
            this.EventosBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConfiguracionBox
            // 
            this.ConfiguracionBox.Controls.Add(this.EjecucionForzadaBtn);
            this.ConfiguracionBox.Controls.Add(this.label2);
            this.ConfiguracionBox.Controls.Add(this.TiposDeConectorCb);
            this.ConfiguracionBox.Controls.Add(this.EliminarConectorBt);
            this.ConfiguracionBox.Controls.Add(this.AgregarConectorBt);
            this.ConfiguracionBox.Controls.Add(this.label1);
            this.ConfiguracionBox.Controls.Add(this.DetenerServicioBt);
            this.ConfiguracionBox.Controls.Add(this.IniciarServicioBt);
            this.ConfiguracionBox.Controls.Add(this.ServicioActivo);
            this.ConfiguracionBox.Controls.Add(this.lvConfiguraciones);
            this.ConfiguracionBox.Location = new System.Drawing.Point(16, 15);
            this.ConfiguracionBox.Margin = new System.Windows.Forms.Padding(4);
            this.ConfiguracionBox.Name = "ConfiguracionBox";
            this.ConfiguracionBox.Padding = new System.Windows.Forms.Padding(4);
            this.ConfiguracionBox.Size = new System.Drawing.Size(507, 661);
            this.ConfiguracionBox.TabIndex = 0;
            this.ConfiguracionBox.TabStop = false;
            this.ConfiguracionBox.Text = "Configuración";
            // 
            // EjecucionForzadaBtn
            // 
            this.EjecucionForzadaBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.EjecucionForzadaBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.EjecucionForzadaBtn.ForeColor = System.Drawing.Color.White;
            this.EjecucionForzadaBtn.Image = global::TicopayUniversalConsole.Properties.Resources.procesar_24x24_ticopay_02;
            this.EjecucionForzadaBtn.Location = new System.Drawing.Point(157, 556);
            this.EjecucionForzadaBtn.Margin = new System.Windows.Forms.Padding(4);
            this.EjecucionForzadaBtn.Name = "EjecucionForzadaBtn";
            this.EjecucionForzadaBtn.Size = new System.Drawing.Size(164, 46);
            this.EjecucionForzadaBtn.TabIndex = 2;
            this.EjecucionForzadaBtn.Text = "Forzar Ejecución";
            this.EjecucionForzadaBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EjecucionForzadaBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.EjecucionForzadaBtn.UseVisualStyleBackColor = false;
            this.EjecucionForzadaBtn.Visible = false;
            this.EjecucionForzadaBtn.Click += new System.EventHandler(this.EjecucionForzadaBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 624);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tipo de Conector :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TiposDeConectorCb
            // 
            this.TiposDeConectorCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TiposDeConectorCb.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.TiposDeConectorCb.FormattingEnabled = true;
            this.TiposDeConectorCb.Location = new System.Drawing.Point(145, 619);
            this.TiposDeConectorCb.Margin = new System.Windows.Forms.Padding(4);
            this.TiposDeConectorCb.MaxDropDownItems = 10;
            this.TiposDeConectorCb.Name = "TiposDeConectorCb";
            this.TiposDeConectorCb.Size = new System.Drawing.Size(227, 24);
            this.TiposDeConectorCb.TabIndex = 2;
            // 
            // EliminarConectorBt
            // 
            this.EliminarConectorBt.BackColor = System.Drawing.Color.RoyalBlue;
            this.EliminarConectorBt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.EliminarConectorBt.ForeColor = System.Drawing.Color.White;
            this.EliminarConectorBt.Image = global::TicopayUniversalConsole.Properties.Resources.eliminar_24x24_ticopay_02;
            this.EliminarConectorBt.Location = new System.Drawing.Point(329, 556);
            this.EliminarConectorBt.Margin = new System.Windows.Forms.Padding(4);
            this.EliminarConectorBt.Name = "EliminarConectorBt";
            this.EliminarConectorBt.Size = new System.Drawing.Size(169, 46);
            this.EliminarConectorBt.TabIndex = 7;
            this.EliminarConectorBt.Text = "Eliminar Conector";
            this.EliminarConectorBt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EliminarConectorBt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.EliminarConectorBt.UseVisualStyleBackColor = false;
            this.EliminarConectorBt.Click += new System.EventHandler(this.EliminarConectorBt_Click);
            // 
            // AgregarConectorBt
            // 
            this.AgregarConectorBt.BackColor = System.Drawing.Color.RoyalBlue;
            this.AgregarConectorBt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AgregarConectorBt.ForeColor = System.Drawing.Color.White;
            this.AgregarConectorBt.Image = global::TicopayUniversalConsole.Properties.Resources.agregar_24x24_ticopay_02;
            this.AgregarConectorBt.Location = new System.Drawing.Point(381, 609);
            this.AgregarConectorBt.Margin = new System.Windows.Forms.Padding(4);
            this.AgregarConectorBt.Name = "AgregarConectorBt";
            this.AgregarConectorBt.Size = new System.Drawing.Size(117, 46);
            this.AgregarConectorBt.TabIndex = 6;
            this.AgregarConectorBt.Text = "Agregar Conector";
            this.AgregarConectorBt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AgregarConectorBt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AgregarConectorBt.UseVisualStyleBackColor = false;
            this.AgregarConectorBt.Click += new System.EventHandler(this.AgregarConectorBt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 124);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Conectores Configurados :";
            // 
            // DetenerServicioBt
            // 
            this.DetenerServicioBt.BackColor = System.Drawing.Color.RoyalBlue;
            this.DetenerServicioBt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DetenerServicioBt.ForeColor = System.Drawing.Color.White;
            this.DetenerServicioBt.Image = global::TicopayUniversalConsole.Properties.Resources.detener_24x24_ticopay_02;
            this.DetenerServicioBt.Location = new System.Drawing.Point(329, 62);
            this.DetenerServicioBt.Margin = new System.Windows.Forms.Padding(4);
            this.DetenerServicioBt.Name = "DetenerServicioBt";
            this.DetenerServicioBt.Size = new System.Drawing.Size(169, 46);
            this.DetenerServicioBt.TabIndex = 4;
            this.DetenerServicioBt.Text = "Detener Servicio";
            this.DetenerServicioBt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DetenerServicioBt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.DetenerServicioBt.UseVisualStyleBackColor = false;
            this.DetenerServicioBt.Click += new System.EventHandler(this.DetenerServicioBt_Click);
            // 
            // IniciarServicioBt
            // 
            this.IniciarServicioBt.BackColor = System.Drawing.Color.RoyalBlue;
            this.IniciarServicioBt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IniciarServicioBt.ForeColor = System.Drawing.Color.White;
            this.IniciarServicioBt.Image = global::TicopayUniversalConsole.Properties.Resources.iniciar_24x24_ticopay_02;
            this.IniciarServicioBt.Location = new System.Drawing.Point(163, 62);
            this.IniciarServicioBt.Margin = new System.Windows.Forms.Padding(4);
            this.IniciarServicioBt.Name = "IniciarServicioBt";
            this.IniciarServicioBt.Size = new System.Drawing.Size(159, 46);
            this.IniciarServicioBt.TabIndex = 3;
            this.IniciarServicioBt.Text = "Iniciar Servicio";
            this.IniciarServicioBt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IniciarServicioBt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.IniciarServicioBt.UseVisualStyleBackColor = false;
            this.IniciarServicioBt.Click += new System.EventHandler(this.IniciarServicioBt_Click);
            // 
            // ServicioActivo
            // 
            this.ServicioActivo.AutoSize = true;
            this.ServicioActivo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ServicioActivo.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServicioActivo.Enabled = false;
            this.ServicioActivo.Location = new System.Drawing.Point(23, 33);
            this.ServicioActivo.Margin = new System.Windows.Forms.Padding(4);
            this.ServicioActivo.Name = "ServicioActivo";
            this.ServicioActivo.Size = new System.Drawing.Size(209, 21);
            this.ServicioActivo.TabIndex = 2;
            this.ServicioActivo.Text = "Servicio de conexión activo :";
            this.ServicioActivo.UseVisualStyleBackColor = true;
            this.ServicioActivo.CheckedChanged += new System.EventHandler(this.ServicioActivo_CheckedChanged);
            // 
            // lvConfiguraciones
            // 
            this.lvConfiguraciones.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.JobId,
            this.SubDominio,
            this.Usuario,
            this.TipoConectorConfigurado});
            this.lvConfiguraciones.Location = new System.Drawing.Point(8, 144);
            this.lvConfiguraciones.Margin = new System.Windows.Forms.Padding(4);
            this.lvConfiguraciones.MultiSelect = false;
            this.lvConfiguraciones.Name = "lvConfiguraciones";
            this.lvConfiguraciones.Size = new System.Drawing.Size(489, 404);
            this.lvConfiguraciones.TabIndex = 0;
            this.lvConfiguraciones.UseCompatibleStateImageBehavior = false;
            this.lvConfiguraciones.View = System.Windows.Forms.View.Details;
            // 
            // JobId
            // 
            this.JobId.Text = "JobId";
            this.JobId.Width = 38;
            // 
            // SubDominio
            // 
            this.SubDominio.Text = "Sub Dominio";
            this.SubDominio.Width = 106;
            // 
            // Usuario
            // 
            this.Usuario.Text = "Usuario";
            this.Usuario.Width = 100;
            // 
            // TipoConectorConfigurado
            // 
            this.TipoConectorConfigurado.Text = "Tipo de Conector";
            this.TipoConectorConfigurado.Width = 120;
            // 
            // EventosBox
            // 
            this.EventosBox.Controls.Add(this.GenerarReportesButton);
            this.EventosBox.Controls.Add(this.ActualizarEventosBt);
            this.EventosBox.Controls.Add(this.lvEventos);
            this.EventosBox.Location = new System.Drawing.Point(531, 15);
            this.EventosBox.Margin = new System.Windows.Forms.Padding(4);
            this.EventosBox.Name = "EventosBox";
            this.EventosBox.Padding = new System.Windows.Forms.Padding(4);
            this.EventosBox.Size = new System.Drawing.Size(797, 661);
            this.EventosBox.TabIndex = 1;
            this.EventosBox.TabStop = false;
            this.EventosBox.Text = "Registro de Eventos";
            // 
            // GenerarReportesButton
            // 
            this.GenerarReportesButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.GenerarReportesButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.GenerarReportesButton.ForeColor = System.Drawing.Color.White;
            this.GenerarReportesButton.Image = global::TicopayUniversalConsole.Properties.Resources.actualizar_24x24_ticopay_02;
            this.GenerarReportesButton.Location = new System.Drawing.Point(329, 608);
            this.GenerarReportesButton.Margin = new System.Windows.Forms.Padding(4);
            this.GenerarReportesButton.Name = "GenerarReportesButton";
            this.GenerarReportesButton.Size = new System.Drawing.Size(171, 46);
            this.GenerarReportesButton.TabIndex = 2;
            this.GenerarReportesButton.Text = "Generar Reportes";
            this.GenerarReportesButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GenerarReportesButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.GenerarReportesButton.UseVisualStyleBackColor = false;
            this.GenerarReportesButton.Click += new System.EventHandler(this.GenerarReportesButton_Click);
            // 
            // ActualizarEventosBt
            // 
            this.ActualizarEventosBt.BackColor = System.Drawing.Color.RoyalBlue;
            this.ActualizarEventosBt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ActualizarEventosBt.ForeColor = System.Drawing.Color.White;
            this.ActualizarEventosBt.Image = global::TicopayUniversalConsole.Properties.Resources.actualizar_24x24_ticopay_02;
            this.ActualizarEventosBt.Location = new System.Drawing.Point(508, 608);
            this.ActualizarEventosBt.Margin = new System.Windows.Forms.Padding(4);
            this.ActualizarEventosBt.Name = "ActualizarEventosBt";
            this.ActualizarEventosBt.Size = new System.Drawing.Size(281, 46);
            this.ActualizarEventosBt.TabIndex = 1;
            this.ActualizarEventosBt.Text = "Actualizar Registro de Eventos";
            this.ActualizarEventosBt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ActualizarEventosBt.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ActualizarEventosBt.UseVisualStyleBackColor = false;
            this.ActualizarEventosBt.Click += new System.EventHandler(this.ActualizarEventosBt_Click);
            // 
            // lvEventos
            // 
            this.lvEventos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Descripcion,
            this.Tipo,
            this.Fecha});
            this.lvEventos.Location = new System.Drawing.Point(8, 33);
            this.lvEventos.Margin = new System.Windows.Forms.Padding(4);
            this.lvEventos.Name = "lvEventos";
            this.lvEventos.Size = new System.Drawing.Size(780, 564);
            this.lvEventos.TabIndex = 0;
            this.lvEventos.UseCompatibleStateImageBehavior = false;
            this.lvEventos.View = System.Windows.Forms.View.Details;
            // 
            // Descripcion
            // 
            this.Descripcion.Text = "Descripción";
            this.Descripcion.Width = 400;
            // 
            // Tipo
            // 
            this.Tipo.Text = "Tipo";
            this.Tipo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Tipo.Width = 80;
            // 
            // Fecha
            // 
            this.Fecha.Text = "Fecha";
            this.Fecha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Fecha.Width = 100;
            // 
            // AdminConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1341, 681);
            this.Controls.Add(this.EventosBox);
            this.Controls.Add(this.ConfiguracionBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1359, 728);
            this.MinimumSize = new System.Drawing.Size(1359, 728);
            this.Name = "AdminConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consola de Administración de Conectores de Ticopay";
            this.ConfiguracionBox.ResumeLayout(false);
            this.ConfiguracionBox.PerformLayout();
            this.EventosBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ConfiguracionBox;
        private System.Windows.Forms.GroupBox EventosBox;
        private System.Windows.Forms.ListView lvConfiguraciones;
        private System.Windows.Forms.ListView lvEventos;
        private System.Windows.Forms.CheckBox ServicioActivo;
        private System.Windows.Forms.Button DetenerServicioBt;
        private System.Windows.Forms.Button IniciarServicioBt;
        private System.Windows.Forms.ColumnHeader Descripcion;
        private System.Windows.Forms.ColumnHeader Tipo;
        private System.Windows.Forms.ColumnHeader Fecha;
        private System.Windows.Forms.Button ActualizarEventosBt;
        private System.Windows.Forms.ComboBox TiposDeConectorCb;
        private System.Windows.Forms.Button EliminarConectorBt;
        private System.Windows.Forms.Button AgregarConectorBt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader JobId;
        private System.Windows.Forms.ColumnHeader SubDominio;
        private System.Windows.Forms.ColumnHeader Usuario;
        private System.Windows.Forms.ColumnHeader TipoConectorConfigurado;
        private System.Windows.Forms.Button EjecucionForzadaBtn;
        private System.Windows.Forms.Button GenerarReportesButton;
    }
}

