namespace TicopayContaPymeMiddleService
{
    partial class ProjectInstaller
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

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.TicopayserviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.TicopayServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // TicopayserviceProcessInstaller
            // 
            this.TicopayserviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.TicopayserviceProcessInstaller.Password = null;
            this.TicopayserviceProcessInstaller.Username = null;
            // 
            // TicopayServiceInstaller
            // 
            this.TicopayServiceInstaller.Description = "Servicio que comunica el Sistema de Ticopay Con ContaPyme";
            this.TicopayServiceInstaller.DisplayName = "Ticopay - ContaPyme Service";
            this.TicopayServiceInstaller.ServiceName = "Ticopay - ContaPyme Service";
            this.TicopayServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.TicopayServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.TicopayServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.TicopayserviceProcessInstaller,
            this.TicopayServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller TicopayserviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller TicopayServiceInstaller;
    }
}