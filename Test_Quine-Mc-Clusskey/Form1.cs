using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Test_Quine_Mc_Clusskey
{
    public partial class Form1 : Form
    {
        List<TextBox> textBoxes = new List<TextBox>();
        Quine_Mc_Clusky quine = new Quine_Mc_Clusky();

        string[] vs = new string[] { "ae0fddd1", "3a7a6181", "c6c8f874", "31bd4051", "98fefd8f", "c3762eac", "7ff81a12",
            "d732c64c", "73f0fe13", "31ed8eb7", "52f8bab1", "853f4e38", "3a395e38", "c3893b88", "0a85771c", "67794325",
            "7e8bd7b8", "7a7d2069", "1bc28ab7", "e8d4f87d"};

        bool Error = false;

        string originalFunc;
        byte bitDepth;

        string[,] MinimizationTable;
        string CoreResultLine;
        string FuncResultLine;

        string finalFunc;
        int LastX;
        int LastY;

        public Form1()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            int period;
            int countPeriod;
            bool tmp;
            int b;
            int counter;

            if (HexRadioButton.Checked)
            {
                finalFunc = ConvertToBin(originalFunc);
            }
            else if (BinRadioButton.Checked)
            {
                finalFunc = originalFunc;
            }

            for (byte i = 0; i < bitDepth; i++)
            {
                int a = (bitDepth - i);

                LastX = 3 + 25 * i;

                textBoxes.Add(new TextBox { Name = (bitDepth - 1 - i).ToString(), Text = "X" + (bitDepth - 1 - i).ToString(), ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, 3), Parent = panel1 });

                counter = 1;

                period = (int)Math.Pow(2, a) / 2;

                for (int k = 0; k < Math.Pow(2, i); k++)
                {
                    tmp = false;
                    countPeriod = 0;
                    b = 0;

                    for (int j = 0; j < Math.Pow(2, a); j++)
                    {
                        if (countPeriod == period)
                        {
                            if (tmp)
                            {
                                tmp = false;
                                b = 0;
                            }
                            else
                            {
                                tmp = true;
                                b = 1;
                            }

                            countPeriod = 0;
                        }

                        LastY = 3 + 25 * counter;

                        textBoxes.Add(new TextBox { Name = "X" + (bitDepth - 1 - i).ToString() + "_" + counter.ToString(), Text = b.ToString(), ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, LastY), Parent = panel1 });

                        countPeriod++;
                        counter++;
                    }
                }
            }
        }

        public string ConvertToBin(string line)
        {
            string TmpLine = "";
            string HexFunc = line;

            for (int i = 0; i < HexFunc.Length; i++)
            {
                switch (Convert.ToString(HexFunc[i]))
                {
                    case ("0"):
                        TmpLine += "0000";
                        break;
                    case ("1"):
                        TmpLine += "0001";
                        break;
                    case ("2"):
                        TmpLine += "0010";
                        break;
                    case ("3"):
                        TmpLine += "0011";
                        break;
                    case ("4"):
                        TmpLine += "0100";
                        break;
                    case ("5"):
                        TmpLine += "0101";
                        break;
                    case ("6"):
                        TmpLine += "0110";
                        break;
                    case ("7"):
                        TmpLine += "0111";
                        break;
                    case ("8"):
                        TmpLine += "1000";
                        break;
                    case ("9"):
                        TmpLine += "1001";
                        break;
                    case ("a"):
                        TmpLine += "1010";
                        break;
                    case ("b"):
                        TmpLine += "1011";
                        break;
                    case ("c"):
                        TmpLine += "1100";
                        break;
                    case ("d"):
                        TmpLine += "1101";
                        break;
                    case ("e"):
                        TmpLine += "1110";
                        break;
                    case ("f"):
                        TmpLine += "1111";
                        break;
                }
            }

            return TmpLine;
        }

        public void PrintFunc()
        {
            Panel BlackLine = new Panel { Location = new Point(LastX + 25, 3), Size = new Size(5, LastY + 18), BackColor = Color.FromArgb(125, 125, 125), Parent = panel1 };

            LastX += 35;

            textBoxes.Add(new TextBox { Name = "F", Text = "F", ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, 3), Parent = panel1 });

            for (int i = 0; i < Math.Pow(2, Convert.ToByte(textBox2.Text)); i++)
            {
                textBoxes.Add(new TextBox { Name = "F_" + i.ToString(), Text = finalFunc[i].ToString(), ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, 3 + 25 * (i + 1)), Parent = panel1 });
            }
        }

        public void ClearPanel()
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < this.panel1.Controls.Count; i++)
                {
                    panel1.Controls.Remove(panel1.Controls[i]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveExternalData();

            ClearPanel();

            LoadData();

            textBox4.Clear();

            if (finalFunc.Length == Math.Pow(2, bitDepth))
                PrintFunc();

            StartQuineMcClusky(bitDepth, finalFunc);

            if (!Error)
            {
                WriteTable();
                textBox5.Text = CoreResultLine;
                textBox3.Text = FuncResultLine;
            }
            else
            {
                MessageBox.Show("В ответе ошибки");
            }
        }

        public void SaveExternalData()
        {
            originalFunc = textBox1.Text;
            bitDepth = Convert.ToByte(textBox2.Text);
        }

        public void StartQuineMcClusky(byte bitDepth, string funcLine)
        {
            quine.MainVoid(bitDepth, funcLine, out CoreResultLine, out FuncResultLine, out MinimizationTable, out Error);
        }

        public void WriteTable()
        {
            string TmpLine;
            string RowTable = "";

            for (int i = 0; i < MinimizationTable.GetLength(0); i++)
            {
                RowTable = "";
                for (int j = 0; j < MinimizationTable.GetLength(1); j++)
                {
                    TmpLine = "";

                    if (MinimizationTable[i, j] == null)
                    {
                        while (TmpLine.Length != 2 * bitDepth)
                        {
                            TmpLine += " ";
                        }

                        RowTable += TmpLine;
                    }
                    else if (MinimizationTable[i, j].Length < bitDepth)
                    {
                        while (MinimizationTable[i, j].Length + TmpLine.Length != bitDepth)
                        {
                            TmpLine += "+";
                        }

                        RowTable += MinimizationTable[i, j] + TmpLine;
                    }
                    else
                    {
                        RowTable += MinimizationTable[i, j];
                    }

                    RowTable += "|";
                }

                if (i == 0)
                {
                    textBox4.Size = new Size(RowTable.Length * 6, MinimizationTable.GetLength(0) * 16);
                }
                textBox4.Text += RowTable + "\r\n";
            }
        }


        private void HexRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (HexRadioButton.Checked)
                BinRadioButton.Checked = false;

            textBox1.Text = "";
        }

        private void BinRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (BinRadioButton.Checked)
                HexRadioButton.Checked = false;

            textBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string endFunc;
            Stopwatch stopwatch = new Stopwatch();
            textBox4.Clear();

            for (int i = 0; i < vs.Length; i++)
            {
                endFunc = ConvertToBin(vs[i]);

                stopwatch.Restart();
                StartQuineMcClusky(5, endFunc);
                stopwatch.Stop();

                textBox4.Text += vs[i] + " - " + Convert.ToString(stopwatch.Elapsed) + CoreResultLine + " + " + FuncResultLine + "\r\n";
            }
        }
    }
}
