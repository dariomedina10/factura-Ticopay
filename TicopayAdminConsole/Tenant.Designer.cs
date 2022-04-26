namespace TicopayAdminConsole
{
    partial class Tenant
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tenant));
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbTenant = new System.Windows.Forms.TextBox();
            this.ContinuarButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(120, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bienvenido a Ticopay Firma";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(66, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(364, 17);
            this.label6.TabIndex = 8;
            this.label6.Text = "Para iniciar debe especificar el Url de su cuenta Ticopay";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 22);
            this.label4.TabIndex = 9;
            this.label4.Text = "Https://";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(239, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 22);
            this.label5.TabIndex = 10;
            this.label5.Text = ".ticopays.com";
            // 
            // tbTenant
            // 
            this.tbTenant.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTenant.Location = new System.Drawing.Point(67, 75);
            this.tbTenant.Name = "tbTenant";
            this.tbTenant.Size = new System.Drawing.Size(175, 27);
            this.tbTenant.TabIndex = 11;
            this.tbTenant.Text = "ticopay";
            // 
            // ContinuarButton
            // 
            this.ContinuarButton.BackColor = System.Drawing.Color.ForestGreen;
            this.ContinuarButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContinuarButton.ForeColor = System.Drawing.Color.White;
            this.ContinuarButton.Location = new System.Drawing.Point(363, 68);
            this.ContinuarButton.Name = "ContinuarButton";
            this.ContinuarButton.Size = new System.Drawing.Size(111, 41);
            this.ContinuarButton.TabIndex = 12;
            this.ContinuarButton.Text = "Continuar";
            this.ContinuarButton.UseVisualStyleBackColor = false;
            this.ContinuarButton.Click += new System.EventHandler(this.ContinuarButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(36, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(419, 34);
            this.label2.TabIndex = 13;
            this.label2.Text = "Normalmente esta es la parte principal de su nombre de dominio.\r\n\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(64, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(359, 17);
            this.label3.TabIndex = 14;
            this.label3.Text = " Revise su correo electrónico de bienvenida si lo olvidó.";
            // 
            // Tenant
            // 
            this.AcceptButton = this.ContinuarButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 161);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ContinuarButton);
            this.Controls.Add(this.tbTenant);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(502, 200);
            this.MinimumSize = new System.Drawing.Size(502, 200);
            this.Name = "Tenant";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bienvenido a la Consola de Administracion de Ticopay - ContaPyme";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbTenant;
        private System.Windows.Forms.Button ContinuarButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

