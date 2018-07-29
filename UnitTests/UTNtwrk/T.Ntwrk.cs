using System;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTests.Common;

using NN.Solver;

namespace UnitTests.UTNtwrk
{
    [TestClass]
    public class UT_Ntwrk : TestBase
    {
        private const string subDirName = "Ntwrk";

        [TestMethod]
        public void NtwrkSolve()
        {
            StringBuilder sb = new StringBuilder();

            NeuroNetwork n = new NeuroNetwork();

            Dictionary<int, NeuroSignal> input = new Dictionary<int, NeuroSignal>();
            Dictionary<int, NeuroSignal> output = new Dictionary<int, NeuroSignal>();

            input.Add(0, new NeuroSignal(0, 0.9)); output.Add(0, new NeuroSignal(0));
            input.Add(1, new NeuroSignal(1, 0.1)); output.Add(1, new NeuroSignal(1));
            input.Add(2, new NeuroSignal(2, 0.8)); output.Add(2, new NeuroSignal(2));

            #region -> Log input data
            {
                sb.Append("Input data:   {");
                foreach (INeuroTransmitter val in input.Values)
                {
                    sb.AppendFormat("  {0}", val);
                }
                sb.Append("  }");
                sb.AppendLine();
            }
            #endregion

            n.ProcessData(input.Values);

            #region -> Log output data
            {
                sb.Append("Iteration #1: {");
                foreach (INeuroTransmitter val in n.SynapticKnobs)
                {
                    sb.AppendFormat("  {0}", val);
                }
                sb.Append("  }");
                sb.AppendLine();
            }
            #endregion

            string txActual = sb.ToString();
            string txExpctd = Data.CalculationLog;

            CompareResult(
                txActual, txExpctd, subDirName,
                "Calculation", "Calculation log changed");
        }
    }
}
