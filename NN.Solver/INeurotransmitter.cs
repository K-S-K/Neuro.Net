using System.Collections.Generic;

namespace NN.Solver
{
    public interface INeuroTransmitter
    {
        int ItemID { get; }
        double Value { get; }

        string ToString();
    }

    public delegate void NeuroImpulseDelegate(IEnumerable<INeuroTransmitter> transmitters);
}