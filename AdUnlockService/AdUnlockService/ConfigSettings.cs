using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using log4net;
using log4net.Config;

namespace AdUnlockService
{
    internal class ConfigSettings
    {
        /// <summary>
        /// Log4Net instance.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConfigSettings()
        {
            // Initialize unlock frequency to 60 seconds.
            this.UnlockFrequency = 60;

            // Initialize lists.
            this.UnlockAccounts = new List<string>();
            this.IgnoreServerIpAddresses = new List<IPAddress>();
        }

        /// <summary>
        /// Load configuration settings into object.
        /// </summary>
        public void LoadSettings()
        {
            // Define variables.
            KeyValueConfigurationCollection appSettings;

            // Get appSettings
#if DEBUG
            // Load from load directory.
            try
            {
                log.Info("Loading configuration settings.");
                appSettings = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location).AppSettings.Settings;
            }
            catch
            {
                log.Error("Could not load configuration file.");
                throw new ConfigurationException("Could not load configuration file.");
            }
#else
            try
            {
                log.Info("Starting to load configuration settings.");
                appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
            }
            catch
            {
                log.Error("Could not load configuration file.");
                throw new ConfigurationException("Could not load configuration file.");
            }
#endif

            // Store domain.
            if ((appSettings["domain"] != null) && (!String.IsNullOrWhiteSpace(appSettings["domain"].Value)))
            {
                this.Domain = appSettings["domain"].Value;
            }
            else
            {
                log.Error("No domain specified.");
                throw new ConfigurationErrorsException("No domain specified.");
            }

            // Store username.
            if ((appSettings["username"] != null) && (!String.IsNullOrWhiteSpace(appSettings["username"].Value)))
            {
                this.Username = appSettings["username"].Value;
            }
            else
            {
                log.Error("No username specified.");
                throw new ConfigurationErrorsException("No username specified.");
            }

            // Store password.
            if ((appSettings["password"] != null) && (!String.IsNullOrWhiteSpace(appSettings["password"].Value)))
            {
                this.Password = appSettings["password"].Value;
            }
            else
            {
                log.Error("No password specified.");
                throw new ConfigurationErrorsException("No password specified.");
            }

            // Store unlock frequency.
            int unlockSeconds;
            if (int.TryParse(appSettings["unlockFrequency"].Value, out unlockSeconds))
            {
                this.UnlockFrequency = unlockSeconds;
            }

            // Store list of accounts to unlock.
            this.UnlockAccounts.Clear();
            if ((appSettings["unlockAccounts"] != null) && (!String.IsNullOrWhiteSpace(appSettings["unlockAccounts"].Value)))
            {
                this.UnlockAccounts.AddRange(appSettings["unlockAccounts"].Value.Split(',').Select(account => account.Trim()).ToList());
            }

            // Store list of server IP addresses to ignore.
            this.IgnoreServerIpAddresses.Clear();
            if ((appSettings["ignoreServerIpAddresses"] != null) && (!String.IsNullOrWhiteSpace(appSettings["ignoreServerIpAddresses"].Value)))
            {
                this.IgnoreServerIpAddresses.AddRange(appSettings["ignoreServerIpAddresses"].Value.Split(',').Select(ip => IPAddress.Parse(ip.Trim())));
            }

            log.Info("Finished loading configuration settings.");
            log.Info("Loaded " + this.UnlockAccounts.Count() + " accounts to unlock.");
        }

        /// <summary>
        /// Dns name for Active Directory domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Username for account to use in unlock process.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for account to use in unlock process.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Setting for how frequently server should attempt to unlock accounts.
        /// </summary>
        [DefaultSettingValue("60")]
        public int UnlockFrequency { get; set; }

        /// <summary>
        /// List of accounts to unlock.
        /// </summary>
        public List<string> UnlockAccounts { get; set; }

        /// <summary>
        /// List of server IP addresses to ignore.
        /// </summary>
        public List<IPAddress> IgnoreServerIpAddresses { get; set; }
    }
}
