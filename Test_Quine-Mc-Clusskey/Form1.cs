﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Test_Quine_Mc_Clusskey
{
    public partial class Form1 : Form
    {
        List<TextBox> textBoxes = new List<TextBox>();
        Dictionary<int, List<string>> StartMinterms = new Dictionary<int, List<string>>();
        Dictionary<int, List<string>> NewMinterms = new Dictionary<int, List<string>>();
        List<string> EndMinterms = new List<string>();
        List<string> IndexNotEndMinterms = new List<string>();

        string[] vs = new string[] { "ae0fddd1", "3a7a6181", "c6c8f874", "31bd4051", "98fefd8f", "c3762eac", "7ff81a12", 
            "d732c64c", "73f0fe13", "31ed8eb7", "52f8bab1", "853f4e38", "3a395e38", "c3893b88", "0a85771c", "67794325", 
            "7e8bd7b8", "7a7d2069", "1bc28ab7", "e8d4f87d"};

        List<string> errors = new List<string>();

        string[,] MinimizationTable;

        string ResultLine = "";
        string CoreResultLine = "";
        string Func;
        string FuncHex;
        int LastX;
        int LastY;

        public Form1()
        {
            InitializeComponent();
        }

        public void LoadData(byte BitDepth)
        {
            int period;
            int countPeriod;
            bool tmp;
            int b;
            int counter;

            if (HexRadioButton.Checked)
            {
                ConvertToBin(textBox1.Text);
            }
            else if (BinRadioButton.Checked)
            {
                Func = textBox1.Text;
            }

            for (byte i = 0; i < BitDepth; i++)
            {
                int a = (BitDepth - i);

                LastX = 3 + 25 * i;

                textBoxes.Add(new TextBox { Name = (BitDepth - 1 - i).ToString(), Text = "X" + (BitDepth - 1 - i).ToString(), ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, 3), Parent = panel1 });

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

                        textBoxes.Add(new TextBox { Name = "X" + (BitDepth - 1 - i).ToString() + "_" + counter.ToString(), Text = b.ToString(), ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, LastY), Parent = panel1 });

                        countPeriod++;
                        counter++;
                    }
                }
            }
        }

        public void ConvertToBin(string line)
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

            Func = TmpLine;
        }

        public void PrintFunc()
        {
            Panel BlackLine = new Panel { Location = new Point(LastX + 25, 3), Size = new Size(5, LastY + 18), BackColor = Color.FromArgb(125, 125, 125), Parent = panel1 };

            LastX += 35;

            textBoxes.Add(new TextBox { Name = "F", Text = "F", ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, 3), Parent = panel1 });

            for (int i = 0; i < Math.Pow(2, Convert.ToByte(textBox2.Text)); i++)
            {
                textBoxes.Add(new TextBox { Name = "F_" + i.ToString(), Text = Func[i].ToString(), ReadOnly = true, Size = new Size(20, 20), Location = new Point(LastX, 3 + 25 * (i + 1)), Parent = panel1 });
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
            Stopwatch stopwatch = new Stopwatch();

            ClearPanel();

            textBox4.Clear();
            EndMinterms.Clear();
            IndexNotEndMinterms.Clear();
            ResultLine = "";
            CoreResultLine = "";

            LoadData(Convert.ToByte(textBox2.Text));

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;

            if (Func.Length == Math.Pow(2, Convert.ToByte(textBox2.Text)))
                PrintFunc();

            stopwatch.Start();

            NewMinterms = DataSearch(SortData());

            NewMinterms = OptimizeMinerms(NewMinterms);

            AddNotAddedMinterms();

            for (int i = 0; i < EndMinterms.Count - 1; i++)
            {
                for (int j = i + 1; j < EndMinterms.Count; j++)
                {
                    if (EndMinterms[i] == EndMinterms[j])
                    {
                        EndMinterms.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < IndexNotEndMinterms.Count; i++)
            {
                for (int j = 0; j < EndMinterms.Count; j++)
                {
                    if (EndMinterms[j] == IndexNotEndMinterms[i])
                        EndMinterms.RemoveAt(j);
                }
            }

            CalcTable();

            WriteTable();

            CoreResultLine = ConvertResult(CoreResultLine);
            ResultLine = ConvertResult(ResultLine);

            stopwatch.Stop();

            MessageBox.Show(FuncHex + " - " + stopwatch.ElapsedMilliseconds.ToString());

            textBox5.Text = CoreResultLine;
            textBox3.Text = ResultLine;

            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
        }

        public Dictionary<int, List<string>> SortData()
        {
            StartMinterms = new Dictionary<int, List<string>>();
            string line;
            int a;

            for (int i = 0; i < Convert.ToInt32(textBox2.Text) + 1; i++)
            {
                StartMinterms.Add(i, new List<string>());
            }

            for (int i = 0; i < Func.Length; i++)
            {
                a = 0;
                if (Convert.ToInt32(Func[i].ToString()) == 1)
                {
                    line = ConvertToFormat(Convert.ToString(i, 2), Convert.ToByte(textBox2.Text));

                    for (int k = 0; k < line.Length; k++)
                    {
                        a += Convert.ToByte(line[k].ToString());
                    }

                    StartMinterms[a].Add(line);
                }
            }

            return StartMinterms;
        }

        public Dictionary<int, List<string>> DataSearch(Dictionary<int, List<string>> Minterms)
        {
            int LineDifferenceCounter;
            int DifferenceIndex;
            int OneCounter;
            bool HaveCrossing;

            Dictionary<int, List<string>> NewMinterms = new Dictionary<int, List<string>>();
            Dictionary<int, List<string>> NullMintermList = new Dictionary<int, List<string>>();

            for (int i = 0; i < Minterms.Count; i++)
            {
                NewMinterms.Add(i, new List<string>());
                NullMintermList.Add(i, new List<string>());
            }

            for (int i = 0; i < Minterms.Count - 1; i++)
            {
                for (int j = 0; j < Minterms[i].Count; j++)
                {
                    HaveCrossing = false;

                    for (int k = 0; k < Minterms[i + 1].Count; k++)
                    {
                        LineDifferenceCounter = 0;
                        DifferenceIndex = 0;
                        OneCounter = 0;

                        for (int x = 0; x < Convert.ToInt32(textBox2.Text); x++)
                        {
                            if (Convert.ToString(Minterms[i][j][x]) != Convert.ToString(Minterms[i + 1][k][x]))
                            {
                                LineDifferenceCounter++;
                                DifferenceIndex = x;
                            }

                            if ((Convert.ToString(Minterms[i][j][x]) != Convert.ToString(Minterms[i + 1][k][x])) && (Convert.ToString(Minterms[i][j][x]) == "x" || Convert.ToString(Minterms[i + 1][k][x]) == "x"))
                            {
                                LineDifferenceCounter = 2;
                                break;
                            }
                        }

                        if (LineDifferenceCounter == 1)
                        {
                            HaveCrossing = true;
                            string line = "";

                            for (int z = 0; z < Convert.ToInt32(textBox2.Text); z++)
                            {
                                if (z != DifferenceIndex)
                                {
                                    line += Convert.ToString(Minterms[i][j][z]);
                                }
                                else
                                {
                                    line += "x";
                                }
                            }

                            for (int x = 0; x < line.Length; x++)
                            {
                                if (Convert.ToString(line[x]) == "1")
                                    OneCounter++;
                            }

                            NewMinterms[OneCounter].Add(line);

                            IndexNotEndMinterms.Add(Minterms[i + 1][k]);
                        }
                    }

                    if (!HaveCrossing)
                    {
                        EndMinterms.Add(Minterms[i][j]);
                    }
                }
            }

            if (CheckList(NewMinterms, NullMintermList))
                NewMinterms = Minterms;

            return NewMinterms;
        }

        public void AddNotAddedMinterms()
        {
            for(int i = 0; i < StartMinterms.Count - 1; i++)
            {
                if (StartMinterms[i].Count == 0 && StartMinterms[i + 1].Count != 0)
                {
                    for (int j = 0; j < StartMinterms[i + 1].Count; j++)
                    {
                        EndMinterms.Add(StartMinterms[i + 1][j]);
                    }
                }
                else if (StartMinterms[i].Count != 0 && StartMinterms[i + 1].Count == 0)
                {
                    for (int j = 0; j < StartMinterms[i + 1].Count; j++)
                    {
                        EndMinterms.Add(StartMinterms[i + 1][j]);
                    }
                }
            }
        }

        public Dictionary<int, List<string>> OptimizeMinerms(Dictionary<int, List<string>> NewMinterms)
        {
            Dictionary<int, List<string>> NewMint;
            Dictionary<int, List<string>> OldMint = NewMinterms;

            while (true)
            {
                NewMint = DataSearch(OldMint);

                NewMint = DeleteCopy(NewMint);

                if (CheckList(OldMint, NewMint))
                    break;
                else
                    OldMint = NewMint;
            }

            return NewMint;
        }

        public Dictionary<int, List<string>> DeleteCopy(Dictionary<int, List<string>> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count - 1; j++)
                {
                    for (int k = j + 1; k < list[i].Count; k++)
                    {
                        if (list[i][j] == list[i][k])
                            list[i].RemoveAt(k);
                    }
                }
            }

            return list;
        }

        public bool CheckList(Dictionary<int, List<string>> list1, Dictionary<int, List<string>> list2)
        {
            bool ListAreSame = true;

            if (list1.Count == list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i].Count != list2[i].Count)
                    {
                        ListAreSame = false;
                        break;
                    }
                }
            }

            return ListAreSame;
        }

        public string ConvertToFormat(string line, byte BitDepth)
        {
            int LengthOfLine = BitDepth - line.Length;

            for (int i = 0; i < LengthOfLine; i++)
                line = "0" + line;

            return line;
        }

        public void CalcTable()
        {
            int MintermCountSum = 0;

            for (int i = 0; i < StartMinterms.Count; i++)
            {
                MintermCountSum += StartMinterms[i].Count;
            }

            MinimizationTable = new string[EndMinterms.Count + 1 /*column*/, MintermCountSum + 1 /*row*/];

            int x = 1;
            int y = 1;

            for (int i = 0; i < EndMinterms.Count; i++)
            {
                MinimizationTable[x, 0] = EndMinterms[i];
                x++;
            }

            for (int i = 0; i < StartMinterms.Count; i++)
            {
                for (int j = 0; j < StartMinterms[i].Count; j++)
                {
                    MinimizationTable[0, y] = StartMinterms[i][j];
                    y++;
                }
            }

            int Difference;

            for (int i = 1; i < MinimizationTable.GetLength(0); i++)
            {
                for (int j = 1; j < MinimizationTable.GetLength(1); j++)
                {
                    Difference = 0;
                    for (int k = 0; k < Convert.ToInt32(textBox2.Text); k++)
                    {
                        if (Convert.ToString(MinimizationTable[i, 0][k]) != "x")
                        {
                            if (MinimizationTable[i, 0][k].ToString() != MinimizationTable[0, j][k].ToString())
                            {
                                Difference++;
                                break;
                            }
                        }
                    }

                    if (Difference == 0)
                    {
                        MinimizationTable[i, j] = "+";
                    }
                }
            }

            List<int> CoreList = new List<int>();

            int PlusCount = 0;
            string TmpLine = "";
            int CoreCount = 0;
            int Index = 0;

            for (int j = 1; j < MinimizationTable.GetLength(1); j++)//Stolbec
            {
                PlusCount = 0;
                Index = 0;
                for (int i = 1; i < MinimizationTable.GetLength(0); i++)//Stroka
                {
                    if (MinimizationTable[i, j] == "+")
                    {
                        PlusCount++;
                        TmpLine = MinimizationTable[i, 0];
                        Index = i;
                    }
                }

                if (PlusCount == 1 && !ResultLine.Contains(TmpLine))
                {
                    if (!CoreList.Contains(Index))
                    {
                        CoreResultLine += TmpLine + " + ";
                        CoreCount++;
                        TmpLine = "";

                        CoreList.Add(Index);
                    }
                }
            }

            CoreList.Sort();

            List<string> LineCrossing = new List<string>();
            LineCrossing.Add("+");

            for (int i = 0; i < CoreList.Count; i++)
            {
                for (int j = 1; j < MinimizationTable.GetLength(1); j++)
                {
                    if (i == 0)
                    {
                        if (MinimizationTable[CoreList[i], j] == "+")
                            LineCrossing.Add("+");
                        else
                            LineCrossing.Add("*");
                    }
                    else
                    {
                        if (MinimizationTable[CoreList[i], j] == "+")
                            LineCrossing[j] = "+";
                    }
                }
            }

            CoreList = FindOptimalResult(LineCrossing, CoreList);
            CheckAnswer(CoreList);

            for (int i = 0 + CoreCount; i < CoreList.Count; i++)
            {
                ResultLine += MinimizationTable[CoreList[i], 0] + " + ";
            }
        }

        public List<int> FindOptimalResult(List<string> lineCrossing, List<int> coreList)
        {
            List<string> LineCrossingSecond = lineCrossing;
            List<int> LineCrossingPlusCounter = new List<int>();
            List<ResultMinimizationTableStruct> ResultMinimizations = new List<ResultMinimizationTableStruct>();

            int max;
            int index = 0;

            ResultMinimizations.Add(new ResultMinimizationTableStruct { ColumnCrossing = new List<string>() });

            for (int i = 1; i < MinimizationTable.GetLength(0); i++)
            {
                ResultMinimizations.Add(new ResultMinimizationTableStruct { ColumnCrossing = new List<string>() });
                ResultMinimizations[i].ColumnCrossing.Add("+");

                for (int j = 1; j < MinimizationTable.GetLength(1); j++)//Stolbec
                {
                    if (MinimizationTable[i, j] == "+")
                        ResultMinimizations[i].ColumnCrossing.Add("+");
                    else
                        ResultMinimizations[i].ColumnCrossing.Add("*");
                }
            }

            for(int x = 0; x < 10; x++)
            {
                for (int i = 0; i < MinimizationTable.GetLength(0); i++)
                {
                    LineCrossingPlusCounter.Add(0);
                }

                for (int i = 1; i < MinimizationTable.GetLength(0); i++)//Stroka
                {
                    if (!coreList.Contains(i))
                    {
                        for (int j = 1; j < MinimizationTable.GetLength(1); j++)//Stolbec
                        {
                            if (ResultMinimizations[i].ColumnCrossing[j] == "+" && lineCrossing[j] == "*")
                            {
                                LineCrossingPlusCounter[i]++;
                            }
                        }
                    }
                }

                max = 0;
                index = 0;

                for (int i = 0; i < LineCrossingPlusCounter.Count; i++)
                {
                    if (!coreList.Contains(i))
                    {
                        if (max < LineCrossingPlusCounter[i])
                        {
                            max = LineCrossingPlusCounter[i];
                            index = i;
                        }
                    }
                }

                for (int i = 1; i < ResultMinimizations[index].ColumnCrossing.Count; i++)
                {
                    if (ResultMinimizations[index].ColumnCrossing[i] == "+")
                        LineCrossingSecond[i] = "+";
                }

                if(index != 0 && !coreList.Contains(index))
                    coreList.Add(index);

                if (CheckLine(LineCrossingSecond))
                {
                    break;
                }
                else
                {
                    LineCrossingPlusCounter.Clear();
                }
            }

            return coreList;
        }

        public bool CheckLine(List<string> list)
        {
            bool HaveAllPlus = true;

            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] != "+")
                {
                    HaveAllPlus = false;
                    break;
                }
            }

            return HaveAllPlus;
        }

        public void CheckAnswer(List<int> CoreList)
        {
            string line;
            int DifferenceCount;

            for (int i = 0; i < Func.Length; i++)
            {
                DifferenceCount = 0;
                if (Convert.ToInt32(Func[i].ToString()) == 1)
                {
                    line = ConvertToFormat(Convert.ToString(i, 2), Convert.ToByte(textBox2.Text));

                    for(int j = 0; j < CoreList.Count; j++)
                    {
                        for (int k = 0; k < line.Length; k++)
                        {
                            if(!(Convert.ToString(MinimizationTable[CoreList[j], 0][k]) == Convert.ToString(line[k]) || Convert.ToString(MinimizationTable[CoreList[j], 0][k]) == "x"))
                            {
                                DifferenceCount++;
                                break;
                            }
                        }
                    }

                    if(DifferenceCount == CoreList.Count)
                    {
                        MessageBox.Show("Ответ не сходится");
                        break;
                    }
                }
            }
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
                        while (TmpLine.Length != 2 * Convert.ToInt32(textBox2.Text))
                        {
                            TmpLine += " ";
                        }

                        RowTable += TmpLine;
                    }
                    else if (MinimizationTable[i, j].Length < Convert.ToInt32(textBox2.Text))
                    {
                        while (MinimizationTable[i, j].Length + TmpLine.Length != Convert.ToInt32(textBox2.Text))
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

        public string ConvertResult(string lineResult)
        {
            string line = "";
            string[] SplitResult = lineResult.Split(new char[] { ' ' });

            for (int i = 0; i < SplitResult.Length - 2; i++)
            {
                for (int j = 0; j < SplitResult[i].Length; j++)
                {
                    if (SplitResult[i][j].ToString() == "1")
                    {
                        line += "x" + Convert.ToString(Convert.ToInt32(textBox2.Text) - j - 1);
                    }
                    else if (SplitResult[i][j].ToString() == "0")
                    {
                        line += "~x" + Convert.ToString(Convert.ToInt32(textBox2.Text) - j - 1);
                    }
                    else if (SplitResult[i][j].ToString() == "+")
                    {
                        line += " + ";
                    }
                }
            }

            return line;
        }

        public struct ResultMinimizationTableStruct
        {
            public List<string> ColumnCrossing;
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
    }
}
