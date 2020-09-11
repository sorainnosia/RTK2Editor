using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RTK2Editor
{
    public partial class Form1 : Form
    {
        public List<byte> Datas = new List<byte>();
        public Dictionary<int, string> Addr = new Dictionary<int, string>();

        public Form1()
        {
            InitializeComponent();
            LoadSetting();
            if (string.IsNullOrEmpty(textBox1.Text) && File.Exists(textBox1.Text)) button1_Click(button1, EventArgs.Empty);
            LoadAddress();
        }

        private void LoadAddress()
        {
            int a = 11668;
            for (int i=0; i<49; i++)
            {
                Addr.Add(i + 1, (a + (i * 35)).ToString("X"));
            }
            //Addr.Add(3, "2dda");
            //Addr.Add(4, "2dfd");
            //Addr.Add(5, "2e20");
            //Addr.Add(7, "2e66");
            //Addr.Add(9, "2eac");
            //Addr.Add(10, "2ecf");
            //Addr.Add(11, "2ef2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Input a filename");
                return;
            }
            if (File.Exists(textBox1.Text) == false)
            {
                MessageBox.Show("File not found");
                return;
            }
            Datas = File.ReadAllBytes(textBox1.Text).ToList();
        }

        private void SaveSetting()
        {
            File.WriteAllText("filename.txt", textBox1.Text);
            File.WriteAllText("reinforce.txt", rtbReinforce.Text);
            File.WriteAllText("ruin.txt", rtbRuin.Text);
            File.WriteAllText("ruin1.txt", textBox2.Text);
            File.WriteAllText("ruin2.txt", textBox3.Text);
            File.WriteAllText("prosperity.txt", rtbProsperity.Text);
        }

        private void LoadSetting()
        {
            if (File.Exists("filename.txt")) textBox1.Text = File.ReadAllText("filename.txt");
            if (File.Exists("reinforce.txt")) rtbReinforce.Text = File.ReadAllText("reinforce.txt");
            if (File.Exists("ruin.txt")) rtbRuin.Text = File.ReadAllText("ruin.txt");
            if (File.Exists("prosperity.txt")) rtbProsperity.Text = File.ReadAllText("prosperity.txt");
            if (File.Exists("ruin1.txt")) textBox2.Text = File.ReadAllText("ruin1.txt");
            if (File.Exists("ruin2.txt")) textBox3.Text = File.ReadAllText("ruin2.txt");
        }

        private int FindIndex(string str)
        {
            for (int i=0; i<Datas.Count; i++)
            {
                int count = 0;
                for (int j=0;j<str.Length; j++)
                {
                    int ni = i + j;
                    if (ni >= Datas.Count) break;
                    if ((Datas[ni] >= 97 && Datas[ni] <= 122) || (Datas[ni] >= 65 && Datas[ni] <= 90) || (Datas[ni] == 32))
                    {
                        if (((char)Datas[ni]).ToString().ToUpper() == ((char)str[j]).ToString().ToUpper())
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (count == str.Length)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ApplySetting()
        {
            if (string.IsNullOrEmpty(rtbReinforce.Text) == false)
            {
                List<string> reinforces = rtbReinforce.Text.Split(new char[] { '\n' }).ToList();
                foreach (string reinforce in reinforces)
                {
                    string str = reinforce.Trim();
                    if (string.IsNullOrEmpty(str)) continue;
                    int index = FindIndex(str);
                    if (index <= -1) continue;
                    int first3 = index - 24;
                    Datas[first3] = 255;
                    Datas[first3 + 1] = 255;
                    Datas[first3 + 2] = 255;
                    Datas[first3 + 5] = 255;
                    Datas[first3 + 7] = 255; // loyalty

                    int last2 = index - 10; // army
                    if (last2 <= -1) continue;
                    Datas[last2] = 16;
                    Datas[last2 + 1] = 39;
                }
            }

            if (string.IsNullOrEmpty(rtbRuin.Text) == false)
            {
                int t1 = 0;
                int.TryParse(textBox2.Text, out t1);
                int t2 = 0;
                int.TryParse(textBox3.Text, out t2);
                if (t1 == 0) t1 = 1;
                if (t1 > 255) t1 = 255;
                if (t2 > 255) t2 = 255;
                if (t1 < 0) t1 = 0;
                if (t2 < 0) t2 = 0;
                List<string> ruins = rtbRuin.Text.Split(new char[] { '\n' }).ToList();
                foreach (string ruin in ruins)
                {
                    string str = ruin.Trim();
                    if (string.IsNullOrEmpty(str)) continue;
                    int index = FindIndex(str);
                    if (index <= -1) continue;

                    int last2 = index - 10; // army
                    if (last2 <= -1) continue;
                    Datas[last2] = (byte)t1;
                    Datas[last2 + 1] = (byte)t2;
                }
            }

            if (string.IsNullOrEmpty(rtbProsperity.Text) == false)
            {
                List<string> nos = rtbProsperity.Text.Split(new char[] { '\n' }).ToList();
                foreach(string no in nos)
                {
                    if (string.IsNullOrEmpty(no)) continue;
                    int n = 0;
                    int.TryParse(no, out n);
                    if (n == 0) continue;
                    if (Addr.ContainsKey(n) == false) continue;
                    string adr = Addr[n];
                    int loc = Convert.ToInt32(adr, 16);
                    Datas[loc] = 255;
                    Datas[loc+1] = 255;
                    Datas[loc+2] = 255;
                    Datas[loc+3] = 255;
                    Datas[loc+4] = 4;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveSetting();
            if (Datas.Count == 0)
            {
                MessageBox.Show("Load file first");
                return;
            }
            ApplySetting();
            File.WriteAllBytes(textBox1.Text, Datas.ToArray());
            MessageBox.Show("Successful");
        }
    }
}
