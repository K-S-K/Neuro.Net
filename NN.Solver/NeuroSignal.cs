using System;
using System.Linq;
using System.Text;

namespace NN.Solver
{
    /// <summary>
    /// Neurotransmitter
    /// </summary>
    /// <remarks>
    /// It moves signal from presynaptic 
    /// vesicles to postsynaptic vesicles
    /// </remarks>
    public class NeuroSignal : INeuroTransmitter
    {
        #region -> Properties
        public int ItemID { get; private set; }
        public double Value { get; internal set; }
        #endregion


        #region -> Methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("[{0}]:", ItemID);
            sb.AppendFormat(" {0}", Value.ToString("0.##0"));

            return sb.ToString();
        }
        #endregion


        #region -> Ctor
        public NeuroSignal(int id)
        {
            ItemID = id;
            Value = 0.0;
        }

        public NeuroSignal(int id, double val) : this(id)
        {
            Value = val;
        }
        #endregion
    }
}
