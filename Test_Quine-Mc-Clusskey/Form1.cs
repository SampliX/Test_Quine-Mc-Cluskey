using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Test_Quine_Mc_Clusskey
{
    public partial class Form1 : Form
    {
        List<TextBox> textBoxes = new List<TextBox>();
        List<LowerMintermStruct> Minterms = new List<LowerMintermStruct>();
        List<LowerMintermStruct> NewMinterms = new List<LowerMintermStruct>();
        List<string> EndMinterms = new List<string>();
        List<string> IndexNotEndMinterms = new List<string>();

        string[,] MinimizationTable;

        string ResultLine = "";
        string Func;
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
                string TmpLine = "";
                string HexFunc = textBox1.Text;

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
            ClearPanel();

            textBox4.Clear();
            EndMinterms.Clear();
            IndexNotEndMinterms.Clear();
            ResultLine = "";

            LoadData(Convert.ToByte(textBox2.Text));

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;

            if (Func.Length == Math.Pow(2, Convert.ToByte(textBox2.Text)))
                PrintFunc();

            NewMinterms = DataSearch(SortData());

            NewMinterms = OptimizeMinerms(NewMinterms);

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

            ConvertResult();

            textBox3.Text = ResultLine;

            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
        }

        public List<LowerMintermStruct> SortData()
        {
            Minterms = new List<LowerMintermStruct>();
            string line;
            int a;

            for (int j = 0; j < Convert.ToInt32(textBox2.Text) + 1; j++)
            {
                Minterms.Add(new LowerMintermStruct() { LowerMinterm = new List<string>() });
            }

            for (int i = 0; i < Func.Length; i++)
            {
                a = 0;
                if (Convert.ToInt32(Func[i].ToString()) == 1)
                {
                    line = ConvertToFormat(Convert.ToString(i, 2), Convert.ToByte(textBox2.Text));

                    for (int k = 0; k < line.Length; k++)
                    {
                        a += Convert.ToInt32(line[k].ToString());
                    }

                    Minterms[a].LowerMinterm.Add(line);
                }
            }

            return Minterms;
        }

        public List<LowerMintermStruct> DataSearch(List<LowerMintermStruct> Minterms)
        {
            int LineDifferenceCounter;
            int DifferenceIndex;
            int OneCounter;
            bool HaveCrossing;

            List<LowerMintermStruct> NewMinterms = new List<LowerMintermStruct>();
            List<LowerMintermStruct> NullMintermList = new List<LowerMintermStruct>();

            for (int j = 0; j < Minterms.Count; j++)
            {
                NewMinterms.Add(new LowerMintermStruct() { LowerMinterm = new List<string>() });
                NullMintermList.Add(new LowerMintermStruct() { LowerMinterm = new List<string>() });
            }

            for (int i = 0; i < Minterms.Count - 1; i++)
            {
                for (int j = 0; j < Minterms[i].LowerMinterm.Count; j++)
                {
                    HaveCrossing = false;

                    for (int k = 0; k < Minterms[i + 1].LowerMinterm.Count; k++)
                    {
                        LineDifferenceCounter = 0;
                        DifferenceIndex = 0;
                        OneCounter = 0;

                        for (int x = 0; x < Convert.ToInt32(textBox2.Text); x++)
                        {
                            if (Convert.ToString(Minterms[i].LowerMinterm[j][x]) != Convert.ToString(Minterms[i + 1].LowerMinterm[k][x]))
                            {
                                LineDifferenceCounter++;
                                DifferenceIndex = x;
                            }

                            if ((Convert.ToString(Minterms[i].LowerMinterm[j][x]) != Convert.ToString(Minterms[i + 1].LowerMinterm[k][x])) && (Convert.ToString(Minterms[i].LowerMinterm[j][x]) == "x" || Convert.ToString(Minterms[i + 1].LowerMinterm[k][x]) == "x"))
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
                                    line += Convert.ToString(Minterms[i].LowerMinterm[j][z]);
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

                            NewMinterms[OneCounter].LowerMinterm.Add(line);

                            IndexNotEndMinterms.Add(Minterms[i + 1].LowerMinterm[k]);
                        }
                    }

                    if (!HaveCrossing)
                    {
                        EndMinterms.Add(Minterms[i].LowerMinterm[j]);
                    }
                }
            }

            if (CheckList(NewMinterms, NullMintermList))
                NewMinterms = Minterms;

            return NewMinterms;
        }

        public List<LowerMintermStruct> OptimizeMinerms(List<LowerMintermStruct> NewMinterms)
        {
            List<LowerMintermStruct> NewMint;
            List<LowerMintermStruct> OldMint = NewMinterms;

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

        public List<LowerMintermStruct> DeleteCopy(List<LowerMintermStruct> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].LowerMinterm.Count - 1; j++)
                {
                    for (int k = j + 1; k < list[i].LowerMinterm.Count; k++)
                    {
                        if (list[i].LowerMinterm[j] == list[i].LowerMinterm[k])
                            list[i].LowerMinterm.RemoveAt(k);
                    }
                }
            }

            return list;
        }

        public bool CheckList(List<LowerMintermStruct> list1, List<LowerMintermStruct> list2)
        {
            bool ListAreSame = true;

            if (list1.Count == list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i].LowerMinterm.Count != list2[i].LowerMinterm.Count)
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

            for (int i = 0; i < Minterms.Count; i++)
            {
                MintermCountSum += Minterms[i].LowerMinterm.Count;
            }

            MinimizationTable = new string[EndMinterms.Count + 1 /*column*/, MintermCountSum + 1 /*row*/];

            int x = 1;
            int y = 1;

            for (int i = 0; i < EndMinterms.Count; i++)
            {
                MinimizationTable[x, 0] = EndMinterms[i];
                x++;
            }

            for (int i = 0; i < Minterms.Count; i++)
            {
                for (int j = 0; j < Minterms[i].LowerMinterm.Count; j++)
                {
                    MinimizationTable[0, y] = Minterms[i].LowerMinterm[j];
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
            string LowResultLine = "";
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
                    LowResultLine += TmpLine + " + ";
                    CoreCount++;
                    TmpLine = "";
                    CoreList.Add(Index);
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

            ResultLine = LowResultLine;

            for(int i = 0 + CoreCount; i < CoreList.Count; i++)
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

            while (true)
            {
                for (int i = 0; i < MinimizationTable.GetLength(0); i++)
                {
                    ResultMinimizations.Add(new ResultMinimizationTableStruct { ColumnCrossing = new List<string>() });
                    LineCrossingPlusCounter.Add(0);
                }

                for (int i = 1; i < MinimizationTable.GetLength(0); i++)//Stroka
                {
                    if (!coreList.Contains(i))
                    {
                        for (int j = 1; j < MinimizationTable.GetLength(1); j++)//Stolbec
                        {
                            if (MinimizationTable[i, j] == "+")
                            {
                                ResultMinimizations[i].ColumnCrossing.Add("+");

                                if (lineCrossing[i] == "*")
                                    LineCrossingPlusCounter[i]++;
                            }
                            else
                            {
                                ResultMinimizations[i].ColumnCrossing.Add("*");
                            }
                        }
                    }
                }

                max = int.MinValue;
                index = 0;

                for (int i = 1; i < LineCrossingPlusCounter.Count; i++)
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

                if(CheckLine(LineCrossingSecond))
                {
                    break;
                }
                else
                {
                    coreList.Add(index);
                    LineCrossingPlusCounter.Clear();
                    ResultMinimizations.Clear();
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

        public void ConvertResult()
        {
            string line = "";

            string[] SplitResult = ResultLine.Split(new char[] { ' ' });

            for (int i = 0; i < SplitResult.Length - 2; i++)
            {
                for (int j = 0; j < SplitResult[i].Length; j++)
                {
                    if (SplitResult[i][j].ToString() == "1")
                    {
                        line += Convert.ToString(Convert.ToChar('a' + j));
                    }
                    else if (SplitResult[i][j].ToString() == "0")
                    {
                        line += "~" + Convert.ToString(Convert.ToChar('a' + j));
                    }
                    else if (SplitResult[i][j].ToString() == "+")
                    {
                        line += " + ";
                    }
                }
            }

            ResultLine = line;
        }

        public struct LowerMintermStruct
        {
            public List<string> LowerMinterm;
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
    }
}
