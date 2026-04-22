using System;
using System.Text;
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

        // XOR truth table
        private static readonly double[][] XorInputs = {
            new[] { 0.0, 0.0 },
            new[] { 0.0, 1.0 },
            new[] { 1.0, 0.0 },
            new[] { 1.0, 1.0 }
        };
        private static readonly double[][] XorTargets = {
            new[] { 0.0 },
            new[] { 1.0 },
            new[] { 1.0 },
            new[] { 0.0 }
        };

        private static List<NeuroSignal> MakeSignals(double[] values)
        {
            var list = new List<NeuroSignal>();
            for (int i = 0; i < values.Length; i++)
                list.Add(new NeuroSignal(i, values[i]));
            return list;
        }

        [TestMethod]
        public void XorTrain_LossDecreases()
        {
            NeuroNetwork n = new NeuroNetwork(2, 4, 1);
            n.Seed(0);

            double InitialError()
            {
                double total = 0;
                for (int i = 0; i < XorInputs.Length; i++)
                {
                    n.ProcessData(MakeSignals(XorInputs[i]));
                    double err = 0;
                    int j = 0;
                    foreach (INeuroTransmitter knob in n.SynapticKnobs)
                        err += Math.Pow(XorTargets[i][j++] - knob.Value, 2);
                    total += err;
                }
                return total;
            }

            double before = InitialError();

            Random rng = new Random(0);
            for (int epoch = 0; epoch < 5000; epoch++)
            {
                int i = rng.Next(XorInputs.Length);
                n.Train(MakeSignals(XorInputs[i]), XorTargets[i], learningRate: 0.5);
            }

            double after = InitialError();

            Assert.IsTrue(after < before,
                $"Training did not reduce error. Before: {before:F3}, After: {after:F3}");
        }

        [TestMethod]
        public void XorTrain_Converges()
        {
            NeuroNetwork n = new NeuroNetwork(2, 4, 1);
            n.Seed(42);

            Random rng = new Random(42);
            for (int epoch = 0; epoch < 30000; epoch++)
            {
                int i = rng.Next(XorInputs.Length);
                n.Train(MakeSignals(XorInputs[i]), XorTargets[i], learningRate: 0.5);
            }

            for (int i = 0; i < XorInputs.Length; i++)
            {
                n.ProcessData(MakeSignals(XorInputs[i]));
                double actual = 0;
                foreach (INeuroTransmitter knob in n.SynapticKnobs)
                    actual = knob.Value;

                double expected = XorTargets[i][0];
                Assert.IsTrue(Math.Abs(actual - expected) < 0.1,
                    $"XOR[{XorInputs[i][0]},{XorInputs[i][1]}]: expected {expected}, got {actual:F3}");
            }
        }

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
