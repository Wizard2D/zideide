using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZIDE
{
    public partial class Workloads : Form
    {
        public Workloads()
        {
            InitializeComponent();
        }
        /*private void NotificationPopup(string title, string content, int timeMS)
        {
            //Timer time = new Timer();
            time.Interval = 2000;
            panel1.Visible = true;
            Title.Visible = true;
            textBox1.Visible = true;
            Title.Text = title;
            textBox1.Text = content;
            time.Tick += (s, e) =>
            {
                panel1.Visible = false;
                Title.Visible = false;
                textBox1.Visible = false;
            };
        }*/
        private void button1_Click(object sender, EventArgs e)
        {
            //NotificationPopup("Workflows", "Installing .NET Runtime...", 2500);
            WorkloadMan.InstallWorkload(WorkloadType.NET);
            
        }
    }
}
