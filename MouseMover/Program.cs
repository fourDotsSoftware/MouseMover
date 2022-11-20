using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MouseMover
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0 && args[0].StartsWith("/uninstall"))
            {
                //Module.DeleteApplicationSettingsFile();
                /*
                frmUninstallQuestionnaire fq = new frmUninstallQuestionnaire();
                fq.ShowDialog();
                */

                System.Diagnostics.Process.Start("https://www.4dots-software.com/support/bugfeature.php?uninstall=true&app=" + System.Web.HttpUtility.UrlEncode(Module.ShortApplicationTitle));

                return;
                Environment.Exit(0);
            }

            ExceptionHandlersHelper.AddUnhandledExceptionHandlers();

            if (args.Length>=1 && args[0].ToLower().Trim()=="/hide")
            {
                frmMain.DoHide = true;
            }

            Application.Run(new frmMain());
        }
    }
}
