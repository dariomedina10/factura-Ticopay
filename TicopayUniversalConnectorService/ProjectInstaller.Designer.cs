namespace TicopayUniversalConnectorService
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
            this.TicopayUSProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.TicopayUniversalServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // TicopayUSProcessInstaller
            // 
            this.TicopayUSProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.TicopayUSProcessInstaller.Password = null;
            this.TicopayUSProcessInstaller.Username = null;
            // 
            // TicopayUniversalServiceInstaller
            // 
            this.TicopayUniversalServiceInstaller.Description = "Servicio de conexion universal de Ticopay";
            this.TicopayUniversalServiceInstaller.DisplayName = "Ticopay Universal Connector Service";
            this.TicopayUniversalServiceInstaller.ServiceName = "UniversalConnectorService";
            this.TicopayUniversalServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.TicopayUniversalServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.TicopayUniversalServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.TicopayUSProcessInstaller,
            this.TicopayUniversalServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller TicopayUSProcessInstaller;
        private System.ServiceProcess.ServiceInstaller TicopayUniversalServiceInstaller;
    }
}