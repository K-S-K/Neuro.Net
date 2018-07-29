using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NN.Solver
{
    public class NeuroNetwork
    {
        #region -> Events
        public event NeuroImpulseDelegate NeuroImpulse;
        #endregion


        #region -> Data
        private Dictionary<int, INeuroLayer> data;

        private INeuroLayer layerFirst;
        private INeuroLayer layerLast;
        #endregion


        #region -> Properties
        /// <summary>
        /// Layer number stub
        /// </summary>
        public int LayerID => 0;

        /// <summary>
        /// Dendrites terminal
        /// </summary>
        /// <remarks>
        /// Must be filled from 
        /// previuus layer or input
        /// </remarks>
        public IEnumerable<INeuroTransmitter> Dendrites { get; private set; }

        /// <summary>
        /// Axon terminal
        /// </summary>
        /// <remarks>
        /// Must be filled from 
        /// nucleus activators
        /// </remarks>
        public IEnumerable<INeuroTransmitter> SynapticKnobs { get; private set; }
        #endregion


        #region -> Methods
        /// <summary>
        /// Process input data, 
        /// calculate output data, 
        /// fire output signal event.
        /// </summary>
        /// <param name="incoming">Incoming parameters set</param>
        public void ProcessData(IEnumerable<INeuroTransmitter> incoming)
        {
            layerFirst.ProcessData(incoming);
        }
        #endregion


        #region -> Ctor
        public NeuroNetwork()
        {
            data = new Dictionary<int, INeuroLayer>();

            NeuroLayer L1 = new NeuroLayer(1, 3, 3, LayerRole.Input);
            NeuroLayer L2 = new NeuroLayer(2, 3, 3, LayerRole.Inner);
            NeuroLayer L3 = new NeuroLayer(3, 3, 3, LayerRole.Output);

            L1.NeuroImpulse += L2.ProcessData;
            L2.NeuroImpulse += L3.ProcessData;

            data.Add(L1.LayerID, L1);
            data.Add(L2.LayerID, L2);
            data.Add(L3.LayerID, L3);


            layerFirst = L1;
            layerLast = L3;

            Dendrites = layerFirst.Dendrites;
            SynapticKnobs = layerLast.SynapticKnobs;

            layerLast.NeuroImpulse += (knobs) =>
            { NeuroImpulse?.Invoke(knobs); };
        }
        #endregion
    }
}
