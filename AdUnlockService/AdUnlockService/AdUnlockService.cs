using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdAspNetProvider.ActiveDirectory.Service;

namespace AdUnlockService
{
    public partial class AdUnlockService : ServiceBase
    {
        /// <summary>
        /// Store configuration settings.
        /// </summary>
        private ConfigSettings config = new ConfigSettings();

        /// <summary>
        /// Flags for service status.
        /// </summary>
        private ManualResetEvent stoppedEvent, stopping;

        public AdUnlockService()
        {
            // Standard service initialization component.
            InitializeComponent();

            // Initialize variables for state.
            this.stopping = new ManualResetEvent(true);
            this.stoppedEvent = new ManualResetEvent(false);

        }

        /// <summary>
        /// Method for starting service when debugging code.
        /// </summary>
        public void OnDebug()
        {
            this.OnStart(null);
        }

        /// <summary>
        /// Method to start service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // Write status message.
            Console.WriteLine("Starting AdUnlockService");

            // Initialize configuration.
            this.config.LoadSettings();

            // Set service status flag.
            this.stopping = new ManualResetEvent(false);

#if DEBUG
            // Unlock accounts.  Direct operation for debug mode.
            this.UnlockAccounts(null);
#else
            // Unlock accounts via worker thread for release mode.
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.UnlockAccounts));
#endif

        }

        /// <summary>
        /// Method to unlock loaded accounts.  Operates as worker thread.
        /// </summary>
        /// <param name="state">Unused paramter for transmissing state.</param>
        private void UnlockAccounts(object state)
        {
            // Configure access to AD.
            var adService = new AdService(new AdConfiguration { Server = this.config.Domain, Username = this.config.Username, Password = this.config.Password, IgnoreServerIpAddresses = this.config.IgnoreServerIpAddresses });

            // Loop execution.
            while (!this.stopping.WaitOne(0))
            {
                // Unlock accounts.
                foreach (var unlockAccount in this.config.UnlockAccounts)
                {
                    // Wrap into try-catch block so service continues to operate regardless of errors.
                    try
                    {
                        // Attempt to load user.
                        var account = adService.GetUser(unlockAccount);

                        // If account exists, try to unlock it.
                        if (account != null)
                        {
                            // See if account is locked out.
                            if (account.IsAccountLockedOut())
                            {
                                // Unlock account.
                                account.UnlockAccount();

                                Console.WriteLine(unlockAccount + " has been unlocked at " + DateTime.Now.ToString());
                            }
                            else
                            {
                                Console.WriteLine(unlockAccount + " was already unlocked at " + DateTime.Now.ToString());
                            }

                            // Dispose of account object.
                            account.Dispose();
                        }
                    }
                    catch { }
                }

                // Sleep for specified time or signal is received.
                this.stopping.WaitOne(this.config.UnlockFrequency * 1000);
            }

            // Signal processing has ended.
            this.stoppedEvent.Set();
        }

        /// <summary>
        /// Method to stop the service.
        /// </summary>
        protected override void OnStop()
        {
            // Write status message.
            Console.WriteLine("Stopping AdUnlockService");

            // Set service stopping flag.
            this.stopping.Set();

            // Wait for event to stop.
            this.stoppedEvent.WaitOne();
        }
    }
}
