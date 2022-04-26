namespace TicopayAdminConsole
{
    partial class AddTenants
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTenants));
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbTenant = new System.Windows.Forms.TextBox();
            this.tbCompany = new System.Windows.Forms.TextBox();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.lvTenants = new System.Windows.Forms.ListView();
            this.id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tenant = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.user = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.company = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // tbPassword
            // 
            this.tbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPassword.Location = new System.Drawing.Point(234, 320);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(146, 23);
            this.tbPassword.TabIndex = 0;
            // 
            // tbTenant
            // 
            this.tbTenant.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTenant.Location = new System.Drawing.Point(234, 262);
            this.tbTenant.Name = "tbTenant";
            this.tbTenant.Size = new System.Drawing.Size(146, 23);
            this.tbTenant.TabIndex = 1;
            // 
            // tbCompany
            // 
            this.tbCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCompany.Location = new System.Drawing.Point(234, 350);
            this.tbCompany.Name = "tbCompany";
            this.tbCompany.Size = new System.Drawing.Size(146, 23);
            this.tbCompany.TabIndex = 2;
            // 
            // tbUser
            // 
            this.tbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbUser.Location = new System.Drawing.Point(234, 291);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(146, 23);
            this.tbUser.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Listado de Tenants configurados :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(71, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Tenant o Sub Dominio :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(163, 294);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Usuario :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(139, 323);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Contraseña :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 353);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(216, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Codigo Empresa en ContaPyme :";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(434, 40);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 10;
            this.addButton.Text = "Agregar";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(434, 69);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 11;
            this.deleteButton.Text = "Eliminar";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(434, 350);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 13;
            this.backButton.Text = "Aceptar";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // lvTenants
            // 
            this.lvTenants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.id,
            this.tenant,
            this.user,
            this.company});
            this.lvTenants.Location = new System.Drawing.Point(15, 40);
            this.lvTenants.MultiSelect = false;
            this.lvTenants.Name = "lvTenants";
            this.lvTenants.Size = new System.Drawing.Size(365, 216);
            this.lvTenants.TabIndex = 14;
            this.lvTenants.UseCompatibleStateImageBehavior = false;
            this.lvTenants.View = System.Windows.Forms.View.Details;
            // 
            // id
            // 
            this.id.Text = "Id";
            this.id.Width = 120;
            // 
            // tenant
            // 
            this.tenant.Text = "Tenant";
            this.tenant.Width = 100;
            // 
            // user
            // 
            this.user.Text = "User";
            this.user.Width = 80;
            // 
            // company
            // 
            this.company.Text = "Company";
            // 
            // AddTenants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 394);
            this.Controls.Add(this.lvTenants);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbUser);
            this.Controls.Add(this.tbCompany);
            this.Controls.Add(this.tbTenant);
            this.Controls.Add(this.tbPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddTenants";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tenants en Ticopay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddTenants_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbTenant;
        private System.Windows.Forms.TextBox tbCompany;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.ListView lvTenants;
        private System.Windows.Forms.ColumnHeader id;
        private System.Windows.Forms.ColumnHeader tenant;
        private System.Windows.Forms.ColumnHeader user;
        private System.Windows.Forms.ColumnHeader company;
    }
}