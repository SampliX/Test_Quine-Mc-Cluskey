using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Test_Quine_Mc_Clusskey
{
    public partial class Form1 : Form
    {
        MainViewModel mainViewModel = new MainViewModel();

        string[] vs = new string[] { "ae0fddd1", "3a7a6181", "c6c8f874", "31bd4051", "98fefd8f", "c3762eac", "7ff81a12",
            "d732c64c", "73f0fe13", "31ed8eb7", "52f8bab1", "853f4e38", "3a395e38", "c3893b88", "0a85771c", "67794325",
            "7e8bd7b8", "7a7d2069", "1bc28ab7", "e8d4f87d"};

        bool Error = false;
        bool isHex = true;

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

        private void button1_Click(object sender, EventArgs e)
        {
            originalFunc = textBox1.Text;

            mainViewModel.MainCalc(isHex, originalFunc, out finalFunc, out bitDepth, out CoreResultLine, out FuncResultLine, out MinimizationTable, out Error);

            if (originalFunc.Length != textBox1.Text.Length)
            {
                textBox1.Text = originalFunc;
                MessageBox.Show("Функция была приведена к формату - " + bitDepth.ToString() + " разрядов", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Error)
                MessageBox.Show("В ответе ошибки", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            GenerateTruthTable();
            WriteTable();
            GenerateMinimizationTable();

            textBox5.Text = CoreResultLine;
            textBox3.Text = FuncResultLine;
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
                tmp = string.Join("", Enumerable.Repeat("0", bitDepth - Convert.ToString(i, 2).Length)) + Convert.ToString(i, 2);

                for (int j = 0; j < bitDepth; j++)
                {
                    item.SubItems.Add(tmp[j].ToString());
                }

                item.SubItems.Add(finalFunc[i].ToString());

                TruthTableListView.Items.AddRange(new ListViewItem[] { item });
            }
        }
        
        public void GenerateMinimizationTable()
        {
            dataGridView1.ColumnCount = MinimizationTable.GetLength(0);//Добавляем столбцы, в количестве равным количеству элементов массива.
            dataGridView1.Rows.Add(MinimizationTable.GetLength(1));//Добавляем строки.

            for (int i = 0; i < MinimizationTable.GetLength(0); i++)
            {
                for (int j = 0; j < MinimizationTable.GetLength(1); j++)
                {
                    dataGridView1.Rows[j].Cells[i].Value = MinimizationTable[i,j];//Записываем каждый элемент в отдельную ячейку.
                }
            }
            this.dataGridView1.Columns[""].Frozen = true;
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
            }
        }


        private void HexRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (HexRadioButton.Checked)
            {
                BinRadioButton.Checked = false;
                isHex = true;
                textBox1.Text = "";
            }
        }

        private void BinRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (BinRadioButton.Checked)
            {
                HexRadioButton.Checked = false;
                isHex = false;
                textBox1.Text = "";
            }
        }
    }
}
