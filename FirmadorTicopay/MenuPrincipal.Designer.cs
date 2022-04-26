namespace FirmadorTicopay
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
            this.label4 = new System.Windows.Forms.Label();
            this.pFacturasEmitidas = new System.Windows.Forms.Panel();
            this.lTotalFacturas = new System.Windows.Forms.Label();
            this.BuscarButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.FirmarFacturasButton = new System.Windows.Forms.Button();
            this.dgFacturas = new System.Windows.Forms.DataGridView();
            this.pNotasEmitidas = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbNotasType = new System.Windows.Forms.ComboBox();
            this.lCantidadNotas = new System.Windows.Forms.Label();
            this.BuscarNotasButton = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.FirmarNotasButton = new System.Windows.Forms.Button();
            this.dgwNotas = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.MenuBarra = new System.Windows.Forms.Ribbon();
            this.DocumentosEmitidos = new System.Windows.Forms.RibbonTab();
            this.Facturas = new System.Windows.Forms.RibbonPanel();
            this.FirmarFacturas = new System.Windows.Forms.RibbonButton();
            this.Notas = new System.Windows.Forms.RibbonPanel();
            this.FirmarNotas = new System.Windows.Forms.RibbonButton();
            this.DocRecibidosTab = new System.Windows.Forms.RibbonTab();
            this.ComprobantesPanel = new System.Windows.Forms.RibbonPanel();
            this.VoucherSingButton = new System.Windows.Forms.RibbonButton();
            this.pVouchers = new System.Windows.Forms.Panel();
            this.lComprobantesPendientes = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FirmarComprobantesButton = new System.Windows.Forms.Button();
            this.dgvComprobantes = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.BuscarVouchersButton = new System.Windows.Forms.Button();
            this.pFacturasEmitidas.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFacturas)).BeginInit();
            this.pNotasEmitidas.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwNotas)).BeginInit();
            this.pVouchers.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComprobantes)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(219, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(440, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "Búsqueda de Facturas pendientes por Firma Digital";
            // 
            // pFacturasEmitidas
            // 
            this.pFacturasEmitidas.Controls.Add(this.lTotalFacturas);
            this.pFacturasEmitidas.Controls.Add(this.BuscarButton);
            this.pFacturasEmitidas.Controls.Add(this.groupBox4);
            this.pFacturasEmitidas.Controls.Add(this.label4);
            this.pFacturasEmitidas.Location = new System.Drawing.Point(0, 106);
            this.pFacturasEmitidas.Name = "pFacturasEmitidas";
            this.pFacturasEmitidas.Size = new System.Drawing.Size(884, 510);
            this.pFacturasEmitidas.TabIndex = 9;
            this.pFacturasEmitidas.Visible = false;
            // 
            // lTotalFacturas
            // 
            this.lTotalFacturas.AutoSize = true;
            this.lTotalFacturas.Location = new System.Drawing.Point(15, 96);
            this.lTotalFacturas.Name = "lTotalFacturas";
            this.lTotalFacturas.Size = new System.Drawing.Size(10, 13);
            this.lTotalFacturas.TabIndex = 9;
            this.lTotalFacturas.Text = " ";
            // 
            // BuscarButton
            // 
            this.BuscarButton.BackColor = System.Drawing.Color.ForestGreen;
            this.BuscarButton.ForeColor = System.Drawing.Color.White;
            this.BuscarButton.Location = new System.Drawing.Point(707, 80);
            this.BuscarButton.Name = "BuscarButton";
            this.BuscarButton.Size = new System.Drawing.Size(159, 33);
            this.BuscarButton.TabIndex = 8;
            this.BuscarButton.Text = "Buscar Facturas";
            this.BuscarButton.UseVisualStyleBackColor = false;
            this.BuscarButton.Click += new System.EventHandler(this.BuscarButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.FirmarFacturasButton);
            this.groupBox4.Controls.Add(this.dgFacturas);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(12, 119);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(860, 374);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Facturas Pendientes :";
            // 
            // FirmarFacturasButton
            // 
            this.FirmarFacturasButton.BackColor = System.Drawing.Color.ForestGreen;
            this.FirmarFacturasButton.ForeColor = System.Drawing.Color.White;
            this.FirmarFacturasButton.Location = new System.Drawing.Point(610, 331);
            this.FirmarFacturasButton.Name = "FirmarFacturasButton";
            this.FirmarFacturasButton.Size = new System.Drawing.Size(244, 37);
            this.FirmarFacturasButton.TabIndex = 1;
            this.FirmarFacturasButton.Text = "Firmar Facturas Seleccionadas";
            this.FirmarFacturasButton.UseVisualStyleBackColor = false;
            this.FirmarFacturasButton.Click += new System.EventHandler(this.FirmarFacturasButton_Click);
            // 
            // dgFacturas
            // 
            this.dgFacturas.AllowUserToAddRows = false;
            this.dgFacturas.AllowUserToDeleteRows = false;
            this.dgFacturas.AllowUserToResizeRows = false;
            this.dgFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFacturas.Location = new System.Drawing.Point(6, 25);
            this.dgFacturas.MultiSelect = false;
            this.dgFacturas.Name = "dgFacturas";
            this.dgFacturas.Size = new System.Drawing.Size(848, 300);
            this.dgFacturas.TabIndex = 0;
            this.dgFacturas.Paint += new System.Windows.Forms.PaintEventHandler(this.dgFacturas_Paint);
            // 
            // pNotasEmitidas
            // 
            this.pNotasEmitidas.Controls.Add(this.label1);
            this.pNotasEmitidas.Controls.Add(this.cbNotasType);
            this.pNotasEmitidas.Controls.Add(this.lCantidadNotas);
            this.pNotasEmitidas.Controls.Add(this.BuscarNotasButton);
            this.pNotasEmitidas.Controls.Add(this.groupBox8);
            this.pNotasEmitidas.Controls.Add(this.label9);
            this.pNotasEmitidas.Location = new System.Drawing.Point(0, 105);
            this.pNotasEmitidas.Name = "pNotasEmitidas";
            this.pNotasEmitidas.Size = new System.Drawing.Size(884, 510);
            this.pNotasEmitidas.TabIndex = 10;
            this.pNotasEmitidas.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(469, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Tipo de Notas :";
            // 
            // cbNotasType
            // 
            this.cbNotasType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNotasType.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbNotasType.FormattingEnabled = true;
            this.cbNotasType.Location = new System.Drawing.Point(580, 83);
            this.cbNotasType.Name = "cbNotasType";
            this.cbNotasType.Size = new System.Drawing.Size(121, 26);
            this.cbNotasType.TabIndex = 10;
            // 
            // lCantidadNotas
            // 
            this.lCantidadNotas.AutoSize = true;
            this.lCantidadNotas.Location = new System.Drawing.Point(15, 96);
            this.lCantidadNotas.Name = "lCantidadNotas";
            this.lCantidadNotas.Size = new System.Drawing.Size(0, 13);
            this.lCantidadNotas.TabIndex = 9;
            // 
            // BuscarNotasButton
            // 
            this.BuscarNotasButton.BackColor = System.Drawing.Color.ForestGreen;
            this.BuscarNotasButton.ForeColor = System.Drawing.Color.White;
            this.BuscarNotasButton.Location = new System.Drawing.Point(707, 80);
            this.BuscarNotasButton.Name = "BuscarNotasButton";
            this.BuscarNotasButton.Size = new System.Drawing.Size(159, 33);
            this.BuscarNotasButton.TabIndex = 8;
            this.BuscarNotasButton.Text = "Buscar Notas";
            this.BuscarNotasButton.UseVisualStyleBackColor = false;
            this.BuscarNotasButton.Click += new System.EventHandler(this.BuscarNotasButton_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.FirmarNotasButton);
            this.groupBox8.Controls.Add(this.dgwNotas);
            this.groupBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox8.Location = new System.Drawing.Point(12, 119);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(860, 374);
            this.groupBox8.TabIndex = 8;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Notas Pendientes :";
            // 
            // FirmarNotasButton
            // 
            this.FirmarNotasButton.BackColor = System.Drawing.Color.ForestGreen;
            this.FirmarNotasButton.ForeColor = System.Drawing.Color.White;
            this.FirmarNotasButton.Location = new System.Drawing.Point(610, 331);
            this.FirmarNotasButton.Name = "FirmarNotasButton";
            this.FirmarNotasButton.Size = new System.Drawing.Size(244, 37);
            this.FirmarNotasButton.TabIndex = 1;
            this.FirmarNotasButton.Text = "Firmar Notas Seleccionadas";
            this.FirmarNotasButton.UseVisualStyleBackColor = false;
            this.FirmarNotasButton.Click += new System.EventHandler(this.FirmarNotasButton_Click);
            // 
            // dgwNotas
            // 
            this.dgwNotas.AllowUserToAddRows = false;
            this.dgwNotas.AllowUserToDeleteRows = false;
            this.dgwNotas.AllowUserToResizeRows = false;
            this.dgwNotas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwNotas.Location = new System.Drawing.Point(6, 25);
            this.dgwNotas.MultiSelect = false;
            this.dgwNotas.Name = "dgwNotas";
            this.dgwNotas.Size = new System.Drawing.Size(848, 300);
            this.dgwNotas.TabIndex = 0;
            this.dgwNotas.Paint += new System.Windows.Forms.PaintEventHandler(this.dgwNotas_Paint);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(235, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(416, 24);
            this.label9.TabIndex = 6;
            this.label9.Text = "Búsqueda de Notas pendientes por Firma Digital";
            // 
            // MenuBarra
            // 
            this.MenuBarra.CaptionBarVisible = false;
            this.MenuBarra.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MenuBarra.Location = new System.Drawing.Point(0, 0);
            this.MenuBarra.Minimized = false;
            this.MenuBarra.Name = "MenuBarra";
            // 
            // 
            // 
            this.MenuBarra.OrbDropDown.BorderRoundness = 8;
            this.MenuBarra.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.MenuBarra.OrbDropDown.Name = "";
            this.MenuBarra.OrbDropDown.TabIndex = 0;
            this.MenuBarra.OrbImage = null;
            this.MenuBarra.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2013;
            this.MenuBarra.OrbVisible = false;
            this.MenuBarra.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
            this.MenuBarra.Size = new System.Drawing.Size(884, 107);
            this.MenuBarra.TabIndex = 11;
            this.MenuBarra.Tabs.Add(this.DocumentosEmitidos);
            this.MenuBarra.Tabs.Add(this.DocRecibidosTab);
            this.MenuBarra.TabsMargin = new System.Windows.Forms.Padding(12, 2, 20, 0);
            this.MenuBarra.ThemeColor = System.Windows.Forms.RibbonTheme.Green;
            // 
            // DocumentosEmitidos
            // 
            this.DocumentosEmitidos.Panels.Add(this.Facturas);
            this.DocumentosEmitidos.Panels.Add(this.Notas);
            this.DocumentosEmitidos.Text = "Documentos Emitidos";
            // 
            // Facturas
            // 
            this.Facturas.Items.Add(this.FirmarFacturas);
            this.Facturas.Text = "Facturas";
            // 
            // FirmarFacturas
            // 
            this.FirmarFacturas.Image = global::FirmadorTicopay.Properties.Resources.facturas_aplicativo_48;
            this.FirmarFacturas.SmallImage = ((System.Drawing.Image)(resources.GetObject("FirmarFacturas.SmallImage")));
            this.FirmarFacturas.Text = "";
            this.FirmarFacturas.TextAlignment = System.Windows.Forms.RibbonItem.RibbonItemTextAlignment.Center;
            this.FirmarFacturas.ToolTip = "Permite firmar las facturas emitidas desde Ticopay";
            this.FirmarFacturas.ToolTipTitle = "Firmar Facturas";
            this.FirmarFacturas.Click += new System.EventHandler(this.facturasEmitidasToolStripMenuItem_Click);
            // 
            // Notas
            // 
            this.Notas.Items.Add(this.FirmarNotas);
            this.Notas.Text = "Notas";
            // 
            // FirmarNotas
            // 
            this.FirmarNotas.Image = global::FirmadorTicopay.Properties.Resources.nota_de_debitoy_credito_aplicativo_48;
            this.FirmarNotas.SmallImage = ((System.Drawing.Image)(resources.GetObject("FirmarNotas.SmallImage")));
            this.FirmarNotas.Text = "";
            this.FirmarNotas.TextAlignment = System.Windows.Forms.RibbonItem.RibbonItemTextAlignment.Center;
            this.FirmarNotas.ToolTip = "Permite firmar las notas de Débito y Crédito elaboradas con Ticopay";
            this.FirmarNotas.ToolTipTitle = "Firmar Notas";
            this.FirmarNotas.Click += new System.EventHandler(this.notasEmitidasToolStripMenuItem_Click);
            // 
            // DocRecibidosTab
            // 
            this.DocRecibidosTab.Panels.Add(this.ComprobantesPanel);
            this.DocRecibidosTab.Text = "Documentos Recibidos";
            // 
            // ComprobantesPanel
            // 
            this.ComprobantesPanel.Items.Add(this.VoucherSingButton);
            this.ComprobantesPanel.Text = "Comprobantes Electrónicos";
            // 
            // VoucherSingButton
            // 
            this.VoucherSingButton.Image = global::FirmadorTicopay.Properties.Resources.confirmar_comprobantes_electronicos_aplicativo_03;
            this.VoucherSingButton.SmallImage = ((System.Drawing.Image)(resources.GetObject("VoucherSingButton.SmallImage")));
            this.VoucherSingButton.Text = "Firmar";
            this.VoucherSingButton.Click += new System.EventHandler(this.VoucherSingButton_Click);
            // 
            // pVouchers
            // 
            this.pVouchers.Controls.Add(this.lComprobantesPendientes);
            this.pVouchers.Controls.Add(this.groupBox1);
            this.pVouchers.Controls.Add(this.label2);
            this.pVouchers.Controls.Add(this.BuscarVouchersButton);
            this.pVouchers.Location = new System.Drawing.Point(0, 105);
            this.pVouchers.Name = "pVouchers";
            this.pVouchers.Size = new System.Drawing.Size(884, 510);
            this.pVouchers.TabIndex = 12;
            this.pVouchers.Visible = false;
            // 
            // lComprobantesPendientes
            // 
            this.lComprobantesPendientes.AutoSize = true;
            this.lComprobantesPendientes.Location = new System.Drawing.Point(21, 79);
            this.lComprobantesPendientes.Name = "lComprobantesPendientes";
            this.lComprobantesPendientes.Size = new System.Drawing.Size(146, 13);
            this.lComprobantesPendientes.TabIndex = 3;
            this.lComprobantesPendientes.Text = "0 Comprobantes encontrados";
            this.lComprobantesPendientes.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FirmarComprobantesButton);
            this.groupBox1.Controls.Add(this.dgvComprobantes);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(18, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(848, 396);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comprobantes Pendientes :";
            // 
            // FirmarComprobantesButton
            // 
            this.FirmarComprobantesButton.BackColor = System.Drawing.Color.ForestGreen;
            this.FirmarComprobantesButton.ForeColor = System.Drawing.Color.White;
            this.FirmarComprobantesButton.Location = new System.Drawing.Point(671, 352);
            this.FirmarComprobantesButton.Name = "FirmarComprobantesButton";
            this.FirmarComprobantesButton.Size = new System.Drawing.Size(171, 36);
            this.FirmarComprobantesButton.TabIndex = 1;
            this.FirmarComprobantesButton.Text = "Firmar Comprobantes";
            this.FirmarComprobantesButton.UseVisualStyleBackColor = false;
            this.FirmarComprobantesButton.Click += new System.EventHandler(this.FirmarComprobantesButton_Click);
            // 
            // dgvComprobantes
            // 
            this.dgvComprobantes.AllowUserToAddRows = false;
            this.dgvComprobantes.AllowUserToDeleteRows = false;
            this.dgvComprobantes.AllowUserToResizeRows = false;
            this.dgvComprobantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComprobantes.Location = new System.Drawing.Point(6, 26);
            this.dgvComprobantes.MultiSelect = false;
            this.dgvComprobantes.Name = "dgvComprobantes";
            this.dgvComprobantes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComprobantes.Size = new System.Drawing.Size(836, 320);
            this.dgvComprobantes.TabIndex = 0;
            this.dgvComprobantes.Paint += new System.Windows.Forms.PaintEventHandler(this.dgvComprobantes_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(195, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(481, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Búsqueda de Comprobantes pendientes por Firma Digital";
            // 
            // BuscarVouchersButton
            // 
            this.BuscarVouchersButton.BackColor = System.Drawing.Color.ForestGreen;
            this.BuscarVouchersButton.ForeColor = System.Drawing.Color.White;
            this.BuscarVouchersButton.Location = new System.Drawing.Point(738, 56);
            this.BuscarVouchersButton.Name = "BuscarVouchersButton";
            this.BuscarVouchersButton.Size = new System.Drawing.Size(128, 36);
            this.BuscarVouchersButton.TabIndex = 0;
            this.BuscarVouchersButton.Text = "Buscar Comprobantes";
            this.BuscarVouchersButton.UseVisualStyleBackColor = false;
            this.BuscarVouchersButton.Click += new System.EventHandler(this.BuscarVouchersButton_Click);
            // 
            // MenuPrincipal
            // 
            this.AcceptButton = this.FirmarFacturasButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 611);
            this.Controls.Add(this.MenuBarra);
            this.Controls.Add(this.pVouchers);
            this.Controls.Add(this.pNotasEmitidas);
            this.Controls.Add(this.pFacturasEmitidas);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 650);
            this.MinimumSize = new System.Drawing.Size(900, 650);
            this.Name = "MenuPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ticopay Firma";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MenuPrincipal_FormClosed);
            this.pFacturasEmitidas.ResumeLayout(false);
            this.pFacturasEmitidas.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFacturas)).EndInit();
            this.pNotasEmitidas.ResumeLayout(false);
            this.pNotasEmitidas.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwNotas)).EndInit();
            this.pVouchers.ResumeLayout(false);
            this.pVouchers.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComprobantes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pFacturasEmitidas;
        private System.Windows.Forms.Button BuscarButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button FirmarFacturasButton;
        private System.Windows.Forms.DataGridView dgFacturas;
        private System.Windows.Forms.Label lTotalFacturas;
        private System.Windows.Forms.Panel pNotasEmitidas;
        private System.Windows.Forms.Label lCantidadNotas;
        private System.Windows.Forms.Button BuscarNotasButton;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button FirmarNotasButton;
        private System.Windows.Forms.DataGridView dgwNotas;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbNotasType;
        private System.Windows.Forms.Ribbon MenuBarra;
        private System.Windows.Forms.RibbonTab DocumentosEmitidos;
        private System.Windows.Forms.RibbonPanel Facturas;
        private System.Windows.Forms.RibbonPanel Notas;
        private System.Windows.Forms.RibbonButton FirmarFacturas;
        private System.Windows.Forms.RibbonButton FirmarNotas;
        private System.Windows.Forms.RibbonTab DocRecibidosTab;
        private System.Windows.Forms.RibbonPanel ComprobantesPanel;
        private System.Windows.Forms.RibbonButton VoucherSingButton;
        private System.Windows.Forms.Panel pVouchers;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BuscarVouchersButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button FirmarComprobantesButton;
        private System.Windows.Forms.DataGridView dgvComprobantes;
        private System.Windows.Forms.Label lComprobantesPendientes;
    }
}