﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TicopayContaPymeMiddleService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void TicopayServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(TicopayServiceInstaller.ServiceName).Start();
        }
    }
}
