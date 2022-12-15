using System;
using System.Linq;

namespace Test_Quine_Mc_Clusskey
{
    internal class MainViewModel
    {
        Quine_Mc_Clusky quine = new Quine_Mc_Clusky();
        public string ConvertToBinFormat(bool isHex, string line)
        {
            if(isHex)
                line = Convert.ToString(Convert.ToInt32(line.ToLower(), 16), 2);

            int iter = (int)Math.Pow(2, Math.Ceiling(Math.Log(line.Length, 2))) - line.Length;

            return string.Join("", Enumerable.Repeat("0", iter)) + line;
        }

        public void MainCalc(bool isHex, string startFunc, out string finalFuncBin, out byte bitDepth, out string CoreResultLine, out string FuncResultLine, out string[,] MinimizationTable, out bool Error)
        {
            finalFuncBin = ConvertToBinFormat(isHex, startFunc);

            bitDepth = Convert.ToByte(Math.Log(finalFuncBin.Length, 2));

            quine.MainVoid(bitDepth, finalFuncBin, out CoreResultLine, out FuncResultLine, out MinimizationTable, out Error);
        }
    }
}
