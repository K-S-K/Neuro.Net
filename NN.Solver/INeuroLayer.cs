using System.Collections.Generic;

namespace NN.Solver
{
    public interface INeuroLayer
    {
        #region -> Events
        event NeuroImpulseDelegate NeuroImpulse;
        #endregion


        #region -> Properties
        int LayerID { get; }
        IEnumerable<INeuroTransmitter> Dendrites { get; }
        IEnumerable<INeuroTransmitter> SynapticKnobs { get; }
        #endregion


        #region -> Methods
        /// <summary>
        /// Process input data, 
        /// calculate output data, 
        /// fire output signal event.
        /// </summary>
        /// <param name="incoming">Incoming parameters set</param>
        void ProcessData(IEnumerable<INeuroTransmitter> incoming);
        #endregion
    }
}