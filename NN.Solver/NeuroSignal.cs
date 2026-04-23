using System.Globalization;
using System.Text;

namespace NN.Solver;

/// <summary>
/// Neurotransmitter
/// </summary>
/// <remarks>
/// It moves signal from pre-synaptic 
/// vesicles to postsynaptic vesicles
/// </remarks>
public class NeuroSignal : INeuroTransmitter
{
    #region -> Properties
    public int ItemID { get; }
    public double Value { get; internal set; }
    #endregion


    #region -> Methods
    public override string ToString()
    {
        StringBuilder sb = new ();

        sb.Append($"[{ItemID}]:");
        sb.Append($" {Value.ToString("0.##0", CultureInfo.InvariantCulture)}");

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
