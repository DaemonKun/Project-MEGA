﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Diagnostics;
using System.Security.Principal;
using MetroFramework;
using strData;

namespace Desktop_IDE
{
    public partial class Main : MetroForm,IMain
    {
        Process newprocess = new Process();
        

        public Main()
        {
            //hotspot
            newprocess.StartInfo.UseShellExecute = false;
            newprocess.StartInfo.CreateNoWindow = true;
            newprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            InitializeComponent();
            strData.strData.main = this;
            this.StyleManager = msmMain;
            tabMenu.SelectedTab = tabQuestion;
            cbNumber.SelectedIndex = 0;
            butPrevious.Enabled = false;
                                            
        }

        #region Hotspot
        
        public void Process_1()
        {
            pb.Value = 0;
            pb.Increment(25);
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan stop hostednetwork";
            try
            {
                using(Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    pb.Increment(25);
                    Process_2();
                }
            }
            catch
            {
                //Nothing
            }
        }
        public void Process_2()
        {
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan set hostednetwork mode=allow ssid=MEGA key=x8743k0J";
            try
            {
                using (Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    pb.Increment(25);
                    Process_3();

                }
            }
            catch
            {
                //Nothing
            }
        }
        public void Process_3()
        {
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan start hostednetwork";
            try
            {
                using (Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    pb.Increment(25);
                    butHotspot.Text = "Stop Hotspot";
                    
                }
            }
            catch
            {
                //Nothing
            }
        }
        public void Process_stop()
        {
            pb.Value = 0;
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan stop hostednetwork";
            try
            {
                pb.Increment(50);
                using (Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    pb.Increment(50);
                    butHotspot.Text = "Start Hotspot";
                    
                }
            }
            catch
            {
                //Nothing
            }
        }
        private void clickHotspot(object sender, EventArgs e)
        {
            if (butHotspot.Text == "Start Hotspot")
            {
                Process_1();
            }
            else if(butHotspot.Text == "Stop Hotspot")
            {
                Process_stop();
            }
                
        }
        #endregion

        private void checkTheme(object sender, EventArgs e)
        {
            if (chkTheme.Checked)
            {
                msmMain.Theme = MetroFramework.MetroThemeStyle.Dark;
                msmMain.Style = MetroFramework.MetroColorStyle.Green;
                
            }
            else
            {
                msmMain.Style = MetroFramework.MetroColorStyle.Blue;
                msmMain.Theme = MetroFramework.MetroThemeStyle.Light;
               
                
            }
        }

        private void changeNum(object sender, EventArgs e)
        {
            lblQuestion.Text = "Question " + cbNumber.SelectedItem;
            strData.strData.dispAnswer();
            strData.strData.dispQuestion();
                
            if (cbNumber.SelectedIndex == 0)
            {
                butPrevious.Enabled = false;
                butNext.Enabled = true;
            }
            else if (cbNumber.SelectedIndex == (cbNumber.Items.Count - 1))
            {
                butNext.Enabled = false;
                butPrevious.Enabled = true;
            }
            else
            {
                butPrevious.Enabled = true;
                butNext.Enabled = true;
            }
        }
        
        #region Click Events
        private void clickPrevious(object sender, EventArgs e)
        {
            try
            {
                cbNumber.SelectedIndex -= 1;
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, ex.Message);
            }
            
        }
        private void clickNext(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Question) || string.IsNullOrWhiteSpace(AnswerA) || 
                    string.IsNullOrWhiteSpace(AnswerB) || string.IsNullOrWhiteSpace(AnswerC) || 
                    string.IsNullOrWhiteSpace(AnswerD))
                {
                    MetroMessageBox.Show(this, "You must fill the fields before pressing next", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    strData.strData.getQuestion();
                    strData.strData.getAnswer();
                    cbNumber.SelectedIndex += 1;
                }
            }

            catch (Exception ex)
            {
                MetroMessageBox.Show(this, ex.Message);
            }
        }
        private void clickAdd(object sender, EventArgs e)
        {
            cbNumber.Items.Add(cbNumber.Items.Count + 1);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            int prevIndex = cbNumber.SelectedIndex;
            int prevLastIndex = cbNumber.Items.Count - 1;
            int count = cbNumber.Items.Count;
            if (count == 1)
            {
                butDelete.Enabled = false;
                return;
                
            }
            else
            {
                cbNumber.Items.RemoveAt(cbNumber.SelectedIndex);
            }

            cbNumber.Items.Clear();
            for(int i = 1; i<=(count-1);i++)
            {
                cbNumber.Items.Add(i);
            }
            if ((prevIndex != 0) && (prevIndex != prevLastIndex))
                cbNumber.SelectedIndex = prevIndex;
            else if (prevIndex == prevLastIndex)
                cbNumber.SelectedIndex = prevIndex - 1;
            else
                cbNumber.SelectedIndex = 0;

        }
        private void clickClear(object sender, EventArgs e)
        {
            Question = null;
            AnswerA = null;
            AnswerB = null;
            AnswerC = null;
            AnswerD = null;
            strData.strData.getAnswer();
            strData.strData.getQuestion();
        }
        #endregion

        #region DLL Variable Declarations
        public string Question
        {
            get { return txtQuestion.Text; }
            set { txtQuestion.Text = value; }
        }
        public string AnswerA
        {
            get { return txtA.Text; }
            set { txtA.Text = value; }
        }
        public string AnswerB
        {
            get { return txtB.Text; }
            set { txtB.Text = value; }
        }
        public string AnswerC
        {
            get { return txtC.Text; }
            set { txtC.Text = value; }
        }
        public string AnswerD
        {
            get { return txtD.Text; }
            set { txtD.Text = value; }
        }
        public int Index
        {
            get { return cbNumber.SelectedIndex; }
            set { cbNumber.SelectedIndex = value; }
        }
        public bool A
        {
            get { return rbA.Checked; }
            set{rbA.Checked = value;}
        }
        public bool B
        {
            get { return rbB.Checked; }
            set { rbB.Checked = value; }
        }
        public bool C
        {
            get { return rbC.Checked; }
            set { rbC.Checked = value; }
        }
        public bool D
        {
            get { return rbD.Checked; }
            set { rbD.Checked = value; }
        }
        #endregion

        

        

                        
    }
}
