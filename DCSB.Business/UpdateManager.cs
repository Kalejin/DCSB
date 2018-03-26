using Octokit;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DCSB.Business
{
    public class UpdateManager
    {
        private const string releasesUrl = "https://github.com/Kalejin/DCSB/releases";

        public async Task AutoUpdateCheck(Version currentVersion)
        {
            try
            {
                Version newVersion = await GetNewestVersion();
                if (newVersion > currentVersion)
                {
                    ShowUpdateDialog(newVersion);
                }
            }
            catch { }
        }

        public async Task ManualUpdateCheck(Version currentVersion)
        {
            try
            {
                Version newVersion = await GetNewestVersion();
                if (newVersion > currentVersion)
                {
                    ShowUpdateDialog(newVersion);
                }
                else
                {
                    MessageBox.Show("No update available.");
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(
                        $"{ex.Message}\nDo you want to open GitHub to check manually?",
                        "Update check failed",
                        MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(releasesUrl);
                }
            }
        }

        private void ShowUpdateDialog(Version newVersion)
        {
            MessageBoxResult result = MessageBox.Show(
                        $"Version {newVersion} is available at {releasesUrl}. \nDo you want to open this site?",
                        $"New version {newVersion}",
                        MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(releasesUrl);
            }
        }

        private async Task<Version> GetNewestVersion()
        {
            try
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("Kalejin-DCSB"));
                var releases = await client.Repository.Release.GetAll("Kalejin", "DCSB");
                Match match = Regex.Match(releases[0].TagName, @"\d+\.\d+\.\d+\.\d+");
                return Version.Parse(match.Value);
            }
            catch (Exception ex)
            {
                throw new ApiException("Unable to get newest version from GitHub.", ex);
            }
        }
    }
}
