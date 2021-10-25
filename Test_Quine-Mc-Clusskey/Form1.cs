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

        public Form1()
        {
            InitializeComponent();
        }

        public string ConvertToBin(string line)
        {
            string TmpLine = "";
            string HexFunc = line.ToLower();

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
                    default:
                        throw new Exception("Входная строка имела неверный формат.");
                        break;
                }
            }

            return TmpLine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                originalFunc = textBox1.Text;
                originalFunc = ConvertFuncToFormat(originalFunc);

                if (HexRadioButton.Checked)
                    finalFunc = ConvertToBin(originalFunc);
                else
                    finalFunc = originalFunc;

                bitDepth = Convert.ToByte(Math.Log(finalFunc.Length, 2));

                if (originalFunc.Length != textBox1.Text.Length)
                {
                    textBox1.Text = originalFunc;
                    MessageBox.Show("Функция была приведена к формату - " + bitDepth.ToString() + " разрядов", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                StartQuineMcClusky(bitDepth, finalFunc);

                if (Error)
                    MessageBox.Show("В ответе ошибки", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                GenerateTruthTable();
                WriteTable();

                textBox5.Text = CoreResultLine;
                textBox3.Text = FuncResultLine;
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        public string ConvertFuncToFormat(string func)
        {
            int iter;

            if (HexRadioButton.Checked)
            {
                iter = ((int)Math.Pow(2, Math.Ceiling(Math.Log(func.Length * 4, 2))) - func.Length * 4) / 4;
            }
            else
            {
                iter = (int)Math.Pow(2, Math.Ceiling(Math.Log(func.Length, 2))) - func.Length;
            }

            for (int i = 0; i < iter; i++)
                func = "0" + func;

            return func;
        }

        public void GenerateTruthTable()
        {
            TruthTableListView.Clear();

            string tmp;

            TruthTableListView.Columns.Add("", 0);

            for (int i = bitDepth - 1; i >= 0; i--)
            {
                TruthTableListView.Columns.Add("X" + i.ToString(), 30, HorizontalAlignment.Center);
            }

            TruthTableListView.Columns.Add("F", 30, HorizontalAlignment.Center);

            for (int i = 0; i < Math.Pow(2, bitDepth); i++)
            {
                ListViewItem item = new ListViewItem("", 0);
                tmp = quine.ConvertToFormat(Convert.ToString(i, 2), bitDepth);

                for(int j = 0; j < bitDepth; j++)
                {
                    item.SubItems.Add(tmp[j].ToString());
                }

                item.SubItems.Add(finalFunc[i].ToString());

                TruthTableListView.Items.AddRange(new ListViewItem[] { item });
            }
        }

        public void StartQuineMcClusky(byte bitDepth, string funcLine)
        {
            quine.MainVoid(bitDepth, funcLine, out CoreResultLine, out FuncResultLine, out MinimizationTable, out Error);
        }

        public void WriteTable()
        {
            textBox4.Clear();
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

        private void TestToolStripMenuItem_Click(object sender, EventArgs e)
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
