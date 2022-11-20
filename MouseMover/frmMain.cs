using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace MouseMover
{
    public partial class frmMain : CustomForm
    {
        public frmMain()
        {
            InitializeComponent();

            if (!DoShow && Properties.Settings.Default.ConfiguredOnce && Properties.Settings.Default.ShowIcon)
            {
                this.Visible = false;
                this.Hide();                
            }            
        }

        private bool DoShow = false;
        public static bool DoHide = false;

        Point LastMousePosition = new Point(0, 0);
        DateTime LastMousePositionDateTime = DateTime.Now;
        TimeSpan tsMaxIdle = new TimeSpan(0, 2, 0);
        private int PosX = 100;

        private bool InTimMonitor = false;

        private void timMonitor_Tick(object sender, EventArgs e)
        {
            if (InTimMonitor) return;

            try
            {
                InTimMonitor = true;

                if ((System.Windows.Forms.Cursor.Position.X == LastMousePosition.X) &&
                    (System.Windows.Forms.Cursor.Position.Y == LastMousePosition.Y))
                {
                    TimeSpan ts = DateTime.Now - LastMousePositionDateTime;

                    if (ts >= tsMaxIdle)
                    {
                        tsMoveMouse.Enabled = true;
                        timMonitor.Enabled = false;

                        System.Windows.Forms.Cursor.Position = new Point(PosX, 100);

                        System.Threading.Thread.Sleep(300);

                        //while (System.Windows.Forms.Cursor.Position.Y == LastMousePosition.Y)
                        while (System.Windows.Forms.Cursor.Position.Y == 100)
                        {
                            Application.DoEvents();
                        }

                        tsMoveMouse.Enabled = false;

                        LastMousePosition = System.Windows.Forms.Cursor.Position;
                        LastMousePositionDateTime = DateTime.Now;
                        timMonitor.Enabled = true;
                    }
                }
                else
                {
                    LastMousePosition = System.Windows.Forms.Cursor.Position;
                    LastMousePositionDateTime = DateTime.Now;
                }

            }
            finally
            {
                InTimMonitor = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            tsMaxIdle = new TimeSpan(0, (int)nudMaxMouseIdle.Value, 0);

            timMonitor.Enabled = true;

            Properties.Settings.Default.PreventSleep = chkPreventSleep.Checked;

            Properties.Settings.Default.MaxIdleMinutes = (int)nudMaxMouseIdle.Value;

            Properties.Settings.Default.ShowIcon = chkShowIcon.Checked;

            Properties.Settings.Default.ConfiguredOnce = true;

            Properties.Settings.Default.Save();

            this.notifyIcon1.Visible = chkShowIcon.Checked;

            if (chkPreventSleep.Checked)
            {
                SleepPreventer.PreventSleep();
            }
            else
            {
                SleepPreventer.EnableSleep();
            }


            RegistryKey key = Registry.CurrentUser;

            try
            {
                key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (key == null)
                {
                    Module.ShowMessage("Error. Could not Save if Application will start automatically with Windows");
                    return;
                }

                if (chkWindowsStartup.Checked)
                {
                    if (key.GetValue("MouseMover") == null)
                    {
                        key.SetValue("MouseMover", "\"" + Application.StartupPath + "\\MouseMover.exe\" /hide");
                    }
                }
                else
                {
                    if (key.GetValue("MouseMover") != null)
                    {
                        key.DeleteValue("MouseMover");
                    }
                }

            }
            catch (Exception ex)
            {
                Module.ShowMessage("Error. Could not Save if Application will start automatically with Windows");
                return;
            }

            onToolStripMenuItem.Checked = true;

            this.Visible = false;
        }


        private void tsMoveMouse_Tick(object sender, EventArgs e)
        {            
            if (PosX>300)
            {
                PosX = 100;
            }

            if (System.Windows.Forms.Cursor.Position.Y == 100)
            {
                System.Windows.Forms.Cursor.Position = new Point(PosX, 100);

                PosX++;
            }
            else
            {
                tsMoveMouse.Enabled = false;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {            
            this.Text = Module.ApplicationTitle;

            if (Properties.Settings.Default.CheckWeek)
            {
                UpdateHelper.InitializeCheckVersionWeek();
            }

            checkForNewVersionEachWeekToolStripMenuItem.Checked = Properties.Settings.Default.CheckWeek;

            chkPreventSleep.Checked = Properties.Settings.Default.PreventSleep;

            nudMaxMouseIdle.Value = (int)Properties.Settings.Default.MaxIdleMinutes;

            chkShowIcon.Checked = Properties.Settings.Default.ShowIcon;

            RegistryKey key = Registry.CurrentUser;

            try
            {
                key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);

                if (key != null)
                {
                    if (key.GetValue("MouseMover") != null)
                    {
                        chkWindowsStartup.Checked = true;
                    }
                    else
                    {
                        chkWindowsStartup.Checked = false;
                    }
                }
                else
                {
                    chkWindowsStartup.Checked = false;
                }
            }
            catch (Exception ex)
            {
                chkWindowsStartup.Checked = false;
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }

            if (!Properties.Settings.Default.ConfiguredOnce)
            {
                Module.ShowMessage("The first time Mouse Mover application is being run this configuration screen will open.\n\nAfterwards, you can access it via a double click on the Mouse Mover Windows System Tray Icon or right click.");
            }

            if (!DoShow && Properties.Settings.Default.ConfiguredOnce && Properties.Settings.Default.ShowIcon)
            {
                this.Visible = false;
                this.Hide();
                btnOK_Click(null, null);
            }

            if (DoHide)
            {
                this.Visible = false;
                this.Hide();
                btnOK_Click(null, null);
            }
        }

        #region Help

        private void helpGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(Application.StartupPath + "\\Video Cutter Joiner Expert - User's Manual.chm");
            System.Diagnostics.Process.Start(Module.HelpURL);
        }

        private void pleaseDonateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.4dots-software.com/donate.php");
        }

        private void dotsSoftwarePRODUCTCATALOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.4dots-software.com/downloads/4dots-Software-PRODUCT-CATALOG.pdf");
        }
        
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            frmAbout f = new frmAbout();
            f.ShowDialog();
        }

        private void tiHelpFeedback_Click(object sender, EventArgs e)
        {
            /*
            frmUninstallQuestionnaire f = new frmUninstallQuestionnaire(false);
            f.ShowDialog();
            */

            System.Diagnostics.Process.Start("https://www.4dots-software.com/support/bugfeature.php?app=" + System.Web.HttpUtility.UrlEncode(Module.ShortApplicationTitle));
        }

        private void followUsOnTwitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.twitter.com/4dotsSoftware");
        }

        private void visit4dotsSoftwareWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.4dots-software.com");
        }

        private void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHelper.CheckVersion(false);
        }

        private void youtubeChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            FileShower fs = new FileShower(@"c:\1\03\con.a.jpg");
            fs.Show();

            Module.ShowError(fs.Err);
            */

            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UCovA-lld9Q79l08K-V1QEng");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch { }
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.CheckWeek = checkForNewVersionEachWeekToolStripMenuItem.Checked;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            offToolStripMenuItem.Checked = false;
            btnOK_Click(null, null);
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onToolStripMenuItem.Checked = false;
            offToolStripMenuItem.Checked = true;
            timMonitor.Enabled = false;
            tsMoveMouse.Enabled = false;
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DoShow = true;
                this.Visible = true;
                this.Show();
                this.CenterToScreen();
                this.BringToFront();
                timMonitor.Enabled = false;
                tsMoveMouse.Enabled = false;
            }
            finally
            {
                DoShow = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            timMonitor.Enabled = false;
            tsMoveMouse.Enabled = false;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DoShow = true;
                this.Visible = true;
                this.Show();
                this.CenterToScreen();
                this.BringToFront();
                timMonitor.Enabled = false;
                tsMoveMouse.Enabled = false;
            }
            finally
            {
                DoShow = false;
            }
        }

        private void btnOK_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void frmMain_VisibleChanged(object sender, EventArgs e)
        {
            if (!DoShow && Properties.Settings.Default.ConfiguredOnce && Properties.Settings.Default.ShowIcon)
            {
                this.Visible = false;
                this.Hide();
            }
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            if (!DoShow && Properties.Settings.Default.ConfiguredOnce && Properties.Settings.Default.ShowIcon)
            {
                this.Visible = false;
                this.Hide();
            }

            if (DoHide)
            {
                DoHide = false;
                this.Visible = false;
                this.Hide();
                btnOK_Click(null, null);
            }
        }
    }
}
