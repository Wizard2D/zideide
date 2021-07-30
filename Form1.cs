using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace ZIDE
{
    public partial class Form1 : Form
    {
        SyntaxBox sbox = new SyntaxBox();
        public Form1()
        {
            InitializeComponent();
        }
        private void InitCustomFontTextBox(RichTextBox box)
        {
            //Create your private font collection object.
            PrivateFontCollection pfc = new PrivateFontCollection();

            //Select your font from the resources.
            //My font here is "Digireu.ttf"
           
            int fontLength = Resources.Monaco.Length;

            // create a buffer to read in to
            byte[] fontdata = Resources.Monaco;

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);
        }
        private void ApplyMonacoBox(RichTextBox box, int size)//--RICHTEXTBOX.
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("Monaco.ttf");
            box.Font = new Font(pfc.Families[0], size);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            sbox.Location = richTextBox1.Location;
            sbox.Size = richTextBox1.Size;
            sbox.BackColor = richTextBox1.BackColor;
            sbox.ForeColor = richTextBox1.ForeColor;
            sbox.Text = richTextBox1.Text;
            sbox.BorderStyle = BorderStyle.FixedSingle;
            sbox.Settings.Comment = "//";
            sbox.Settings.EnableComments = false;
            sbox.Settings.Keywords = new List<string> { "public", "private", "void", "int", "short", "float", "ushort", "uint", "long", "ulong", "protected", "virtual", "return", "this", "class", "enum", "struct", "string", "object", "byte", "new", "using", "bool", "static", "abstract" };
            sbox.CompileKeywords();
            this.Controls.Add(sbox);
            ApplyMonacoBox(sbox, 9);

            RichTextBoxExtensions.SetInnerMargins(sbox, 10, 7, 0, 0);
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new TestColorTable());
            //FIRST RUN SETUP:
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Zide")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Zide"));
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Zide\\Projects"));
                FileStream f = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Zide", "InstalledWorkloads.json"));
                f.Close();
                DialogResult res = MessageBox.Show("Install some workloads to be able to compile and run your code!","TIP", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    new Workloads().ShowDialog();
                }
            }
            
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewProject p = new NewProject();
            p.ProjectLoaded += projectLoaded;
            p.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Directory.Delete(Directory.GetCurrentDirectory() + "\\ZIDEUTIL");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //QUICKBOARD RUN
            if(ZIDESettings.currentOpenDir != null)
            {
                string path = ZIDESettings.currentOpenDir;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "dotnet.exe";
                startInfo.WorkingDirectory = path;
                startInfo.Arguments = "build";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                System.Diagnostics.Process processs = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startsInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startsInfo.CreateNoWindow = false;
                startsInfo.FileName = "dotnet.exe";
                startsInfo.WorkingDirectory = path;
                startsInfo.Arguments = "run";
                processs.StartInfo = startsInfo;
                processs.Start();
            }
            else
            {
                MessageBox.Show("Please Open or Create a new Project to run.");
            }
        }
      
        Dictionary<string, string> filedb = new Dictionary<string, string>();
        public void projectLoaded()
        {
            TreeNode root = new TreeNode(ZIDESettings.crProjN);
            treeView1.Nodes.Add(root);
            string targetDirectory = ZIDESettings.currentOpenDir;
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                filedb.Add(Path.GetFileName(fileName), fileName);
                root.Nodes.Add(Path.GetFileName(fileName));
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node.Text != ZIDESettings.crProjN)
            {
                ZIDESettings.currentOpenFIle = filedb[e.Node.Text];
                sbox.Text = "";
                sbox.Text = File.ReadAllText(filedb[e.Node.Text]);
                sbox.ProcessAllLines();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = folderBrowserDialog1.ShowDialog();
            if(res == DialogResult.OK)
            {
                ZIDESettings.currentOpenDir = folderBrowserDialog1.SelectedPath;
                ZIDESettings.crProjN = File.ReadAllText(folderBrowserDialog1.SelectedPath + "\\projdetails.zicf");
                projectLoaded();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllText(ZIDESettings.currentOpenFIle, sbox.Text);
            
        }

        private void richTextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }


    }
    public static class RichTextBoxExtensions
    {
        public static void SetInnerMargins(this TextBoxBase textBox, int left, int top, int right, int bottom)
        {
            var rect = textBox.GetFormattingRect();

            var newRect = new Rectangle(left, top, rect.Width - left - right, rect.Height - top - bottom);
            textBox.SetFormattingRect(newRect);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }
        }

        [DllImport(@"User32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessageRefRect(IntPtr hWnd, uint msg, int wParam, ref RECT rect);

        [DllImport(@"user32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, ref Rectangle lParam);

        private const int EmGetrect = 0xB2;
        private const int EmSetrect = 0xB3;

        private static void SetFormattingRect(this TextBoxBase textbox, Rectangle rect)
        {
            var rc = new RECT(rect);
            SendMessageRefRect(textbox.Handle, EmSetrect, 0, ref rc);
        }

        private static Rectangle GetFormattingRect(this TextBoxBase textbox)
        {
            var rect = new Rectangle();
            SendMessage(textbox.Handle, EmGetrect, (IntPtr)0, ref rect);
            return rect;
        }
    }
    public class TestColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.Black; }
        }

        public override Color MenuBorder  //Change color according your Need
        {
            get { return Color.Black; }
        }
        public override Color ToolStripBorder => Color.Black;
        public override Color MenuItemBorder => Color.White;
        public override Color ToolStripDropDownBackground => Color.DarkGray;
        public override Color ButtonSelectedBorder => Color.DarkGray;
        public override Color SeparatorDark => Color.White;
    }
}
