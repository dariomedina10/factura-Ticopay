using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TicopayUniversalConnectorService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void TicopayUniversalServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(TicopayUniversalServiceInstaller.ServiceName).Start();
        }
    }
}
