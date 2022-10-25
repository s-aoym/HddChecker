using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ServiceProcess;
using System.Management;

namespace HddChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.ContextMenuStrip = this.contextMenuStrip1;

            pictureBox1.Height = 10;
            pictureBox1.Location = new Point(0, 0);

            pictureBox2.Height = 10;
            pictureBox2.Location = new Point(0, 10);

            ////ディスプレイの高さ
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            this.Height = 10 * 2;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);

            checkHdd();
        }

        private void 終了EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkHdd();
        }

        private void checkHdd()
        {
            int free;
            int hdd;
            int par;
            //ディスプレイの高さ
            int h = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //ディスプレイの幅
            int w = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            string[] drives = Directory.GetLogicalDrives();
            foreach (string s in drives)
            {
                Hashtable data = new Hashtable();
                DriveInfo drive = new DriveInfo(s);

                if (drive.IsReady)
                {
                    if (s.ToString() == "C:\\")
                    {
                        free = ToGigaByte(Convert.ToDouble(drive.TotalFreeSpace));
                        hdd = ToGigaByte(Convert.ToDouble(drive.TotalSize));

                        toolTip1.SetToolTip(pictureBox1, "Cドライブ空き容量：" + free + "GB");

                        par = ((hdd - free) * 100) / hdd;
                        pictureBox1.Width = w * par / 100;
                        if (par > 80)
                        {
                            pictureBox1.BackColor = Color.Red;
                        }
                        else
                        {
                            pictureBox1.BackColor = SystemColors.MenuHighlight;
                        }
                    }
                    else if (s.ToString() == "D:\\")
                    {
                        free = ToGigaByte(Convert.ToDouble(drive.TotalFreeSpace));
                        hdd = ToGigaByte(Convert.ToDouble(drive.TotalSize));

                        toolTip1.SetToolTip(pictureBox2, "Dドライブ空き容量：" + free + "GB");

                        par = ((hdd - free) * 100) / hdd;
                        pictureBox2.Width = w * par / 100;
                        if (par > 80)
                        {
                            pictureBox2.BackColor = Color.Red;
                        }
                        else
                        {
                            pictureBox2.BackColor = SystemColors.MenuHighlight;
                        }
                    }

                }
            }
        }

        static int ToGigaByte(Double calcTarget)
        {
            Double i = calcTarget / 1024;
            i /= 1024;
            i /= 1024;
            i = Math.Ceiling(i);
            return Convert.ToInt16(i);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Left)
            {

            }
        }

        private void timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //int processId = GetProcessIdByServiceName("MaLionSPService");

            //if (processId!=0)
            //{
            //    // 第1引数がコマンド、第2引数がコマンドの引数
            //    ProcessStartInfo processStartInfo = new ProcessStartInfo("taskkill", "/pid " + processId + " /f");

            //    // 管理者としてコマンド実行
            //    processStartInfo.Verb = "RunAs";

            //    // ウィンドウを表示しない
            //    //processStartInfo.CreateNoWindow = true;
            //    //processStartInfo.UseShellExecute = false;

            //    // コマンド実行
            //    Process process = Process.Start(processStartInfo);

            //    // コマンド終了後にイベント発行させる
            //    process.EnableRaisingEvents = true;
            //    process.Exited += new EventHandler(process_Exited);
            //}

            

        }

        static void process_Exited(object sender, EventArgs e)
        {
            Process process = (Process)sender;
            // 終了処理
            //MessageBox.Show("終了しました。");
        }

        private static int GetProcessIdByServiceName(string serviceName)
        {
            string qry = $"SELECT PROCESSID FROM WIN32_SERVICE WHERE NAME = '{serviceName }'";
            var searcher = new ManagementObjectSearcher(qry);
            var managementObjects = new ManagementObjectSearcher(qry).Get();
            if (managementObjects.Count != 1)
                throw new InvalidOperationException($"In attempt to kill a service '{serviceName}', expected to find one process for service but found {managementObjects.Count}.");
            int processId = 0;
            foreach (ManagementObject mngntObj in managementObjects)
                processId = (int)(uint)mngntObj["PROCESSID"];
            //if (processId == 0)
                //throw new InvalidOperationException($"In attempt to kill a service '{serviceName}', process ID for service is 0. Possible reason is the service is already stopped.");
            return processId;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int processId = GetProcessIdByServiceName("MaLionSPService");

            if (processId != 0)
            {
                // 第1引数がコマンド、第2引数がコマンドの引数
                ProcessStartInfo processStartInfo = new ProcessStartInfo("taskkill", "/pid " + processId + " /f");

                // 管理者としてコマンド実行
                processStartInfo.Verb = "RunAs";

                // ウィンドウを表示しない
                //processStartInfo.CreateNoWindow = true;
                //processStartInfo.UseShellExecute = false;

                // コマンド実行
                Process process = Process.Start(processStartInfo);

                // コマンド終了後にイベント発行させる
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(process_Exited);
            }
        }
    }
}
