using System;
using System.Collections.Generic;

namespace Test_Quine_Mc_Clusskey
{
    class Quine_Mc_Clusky
    {
        Dictionary<int, List<string>> StartMinterms = new Dictionary<int, List<string>>();
        Dictionary<int, List<string>> NewMinterms = new Dictionary<int, List<string>>();
        List<string> EndMinterms = new List<string>();
        List<string> IndexNotEndMinterms = new List<string>();

        bool Error = false;

        string[,] MinimizationTable;

        byte bitDepth = 0;
        string funcLine = "";

        string coreResultLine = "";
        string funcResultLine = "";

        public void MainVoid(byte bitDepth, string funcLine, out string coreResultLineOut, out string funcResultLineOut, out string[,] minimizationTable, out bool error)
        {
            this.bitDepth = bitDepth;
            this.funcLine = funcLine;

            EndMinterms.Clear();
            IndexNotEndMinterms.Clear();
            funcResultLine = "";
            coreResultLine = "";

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

            coreResultLine = ConvertResult(coreResultLine);
            funcResultLine = ConvertResult(funcResultLine);

            coreResultLineOut = coreResultLine;
            funcResultLineOut = funcResultLine;
            minimizationTable = MinimizationTable;
            error = Error;
        }

        public Dictionary<int, List<string>> SortData()
        {
            StartMinterms = new Dictionary<int, List<string>>();
            string line;
            int a;

            for (int i = 0; i < Convert.ToInt32(bitDepth) + 1; i++)
            {
                StartMinterms.Add(i, new List<string>());
            }

            for (int i = 0; i < funcLine.Length; i++)
            {
                a = 0;
                if (Convert.ToInt32(funcLine[i].ToString()) == 1)
                {
                    line = ConvertToFormat(Convert.ToString(i, 2));

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

                        for (int x = 0; x < Convert.ToInt32(bitDepth); x++)
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

                            for (int z = 0; z < Convert.ToInt32(bitDepth); z++)
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
            for (int i = 0; i < StartMinterms.Count - 1; i++)
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

        public string ConvertToFormat(string line)
        {
            int LengthOfLine = bitDepth - line.Length;

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
                    for (int k = 0; k < Convert.ToInt32(bitDepth); k++)
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

                if (PlusCount == 1 && !funcResultLine.Contains(TmpLine))
                {
                    if (!CoreList.Contains(Index))
                    {
                        coreResultLine += TmpLine + " + ";
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
                funcResultLine += MinimizationTable[CoreList[i], 0] + " + ";
            }
        }

        public List<int> FindOptimalResult(List<string> lineCrossing, List<int> coreList)
        {
            List<string> LineCrossingSecond = lineCrossing;
            List<int> LineCrossingPlusCounter = new List<int>();
            Dictionary<int, List<string>> ResultMinimizations = new Dictionary<int, List<string>>();

            int max;
            int index = 0;

            ResultMinimizations.Add(0, new List<string>());

            for (int i = 1; i < MinimizationTable.GetLength(0); i++)
            {
                ResultMinimizations.Add(i, new List<string>());
                ResultMinimizations[i].Add("+");

                for (int j = 1; j < MinimizationTable.GetLength(1); j++)//Stolbec
                {
                    if (MinimizationTable[i, j] == "+")
                        ResultMinimizations[i].Add("+");
                    else
                        ResultMinimizations[i].Add("*");
                }
            }

            for (int x = 0; x < 10; x++)
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
                            if (ResultMinimizations[i][j] == "+" && lineCrossing[j] == "*")
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

                for (int i = 1; i < ResultMinimizations[index].Count; i++)
                {
                    if (ResultMinimizations[index][i] == "+")
                        LineCrossingSecond[i] = "+";
                }

                if (index != 0 && !coreList.Contains(index))
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

            for (int i = 0; i < funcLine.Length; i++)
            {
                DifferenceCount = 0;
                if (Convert.ToInt32(funcLine[i].ToString()) == 1)
                {
                    line = ConvertToFormat(Convert.ToString(i, 2));

                    for (int j = 0; j < CoreList.Count; j++)
                    {
                        for (int k = 0; k < line.Length; k++)
                        {
                            if (!(Convert.ToString(MinimizationTable[CoreList[j], 0][k]) == Convert.ToString(line[k]) || Convert.ToString(MinimizationTable[CoreList[j], 0][k]) == "x"))
                            {
                                DifferenceCount++;
                                break;
                            }
                        }
                    }

                    if (DifferenceCount == CoreList.Count)
                    {
                        Error = true;
                        break;
                    }
                }
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
                        line += "x" + Convert.ToString(Convert.ToInt32(bitDepth) - j - 1);
                    }
                    else if (SplitResult[i][j].ToString() == "0")
                    {
                        line += "~x" + Convert.ToString(Convert.ToInt32(bitDepth) - j - 1);
                    }
                    else if (SplitResult[i][j].ToString() == "+")
                    {
                        line += " + ";
                    }
                }
            }

            return line;
        }
    }
}
