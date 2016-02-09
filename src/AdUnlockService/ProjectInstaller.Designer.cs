namespace AdUnlockService
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AdUnlockServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AdUnlockServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AdUnlockServiceProcessInstaller
            // 
            this.AdUnlockServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.AdUnlockServiceProcessInstaller.Password = null;
            this.AdUnlockServiceProcessInstaller.Username = null;
            // 
            // AdUnlockServiceInstaller
            // 
            this.AdUnlockServiceInstaller.ServiceName = "AdUnlockService";
            this.AdUnlockServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AdUnlockServiceProcessInstaller,
            this.AdUnlockServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AdUnlockServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller AdUnlockServiceInstaller;
    }
}