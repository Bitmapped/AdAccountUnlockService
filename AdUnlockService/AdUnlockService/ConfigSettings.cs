using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace AdUnlockService
{
    internal class ConfigSettings
    {
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
            // Get appSettings
#if DEBUG
            // Load from load directory.
            var appSettings = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location).AppSettings.Settings;
#else
            var appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
#endif

            // Store domain.
            if (!String.IsNullOrWhiteSpace(appSettings["domain"].Value))
            {
                this.Domain = appSettings["domain"].Value;
            }
            else
            {
                throw new ConfigurationErrorsException("No domain specified.");
            }

            // Store username.
            if (!String.IsNullOrWhiteSpace(appSettings["username"].Value))
            {
                this.Username = appSettings["username"].Value;
            }
            else
            {
                throw new ConfigurationErrorsException("No username specified.");
            }

            // Store password.
            if (!String.IsNullOrWhiteSpace(appSettings["password"].Value))
            {
                this.Password = appSettings["password"].Value;
            }
            else
            {
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
            if (!String.IsNullOrWhiteSpace(appSettings["unlockAccounts"].Value))
            {
                this.UnlockAccounts.AddRange(appSettings["unlockAccounts"].Value.Split(',').Select(account => account.Trim()).ToList());
            }

            // Store list of server IP addresses to ignore.
            this.IgnoreServerIpAddresses.Clear();
            if (!String.IsNullOrWhiteSpace(appSettings["ignoreServerIpAddresses"].Value))
            {
                this.IgnoreServerIpAddresses.AddRange(appSettings["ignoreServerIpAddresses"].Value.Split(',').Select(ip => IPAddress.Parse(ip.Trim())));
            }
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
