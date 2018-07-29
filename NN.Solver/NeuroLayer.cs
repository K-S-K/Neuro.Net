using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace NN.Solver
{
    public class NeuroLayer : INeuroLayer
    {
        #region -> Events
        public event NeuroImpulseDelegate NeuroImpulse;
        #endregion


        #region -> Data
        private object sync;
        private Matrix.Matrix weights;
        private Dictionary<int, NeuroItem> nucleus;
        private Dictionary<int, NeuroSignal> input;
        private Dictionary<int, NeuroSignal> output;
        #endregion


        #region -> Properties
        /// <summary>
        /// Layer number
        /// </summary>
        public int LayerID { get; private set; }

        public LayerRole Role { get; private set; }

        /// <summary>
        /// Dendrites terminal
        /// </summary>
        /// <remarks>
        /// Must be filled from 
        /// previuus layer or input
        /// </remarks>
        public IEnumerable<INeuroTransmitter> Dendrites => input.Values;

        /// <summary>
        /// Axon terminal
        /// </summary>
        /// <remarks>
        /// Must be filled from 
        /// nucleus activators
        /// </remarks>
        public IEnumerable<INeuroTransmitter> SynapticKnobs => output.Values;
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
            lock (sync) { ProcessInputData(incoming); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("[{0}]:", LayerID);
            sb.AppendFormat(" {0}", Role);

            return sb.ToString();
        }
        #endregion


        #region -> Implementation
        private void ProcessInputData(IEnumerable<INeuroTransmitter> incoming)
        {
            // 1) In the begining we must accept incoming data values
            foreach (INeuroTransmitter item in incoming)
            {
                if (input.TryGetValue(item.ItemID, out NeuroSignal signalIn))
                {
                    signalIn.Value = item.Value;
                }
            }

            // 2) Next, we must calculate outputs
            foreach (NeuroItem core in nucleus.Values)
            {
                core.ProcessData();

                if (output.TryGetValue(core.Number, out NeuroSignal signalOut))
                {
                    signalOut.Value = core.Axon.Value;
                }

            }

            // 3) Then, we must notify next level or user class
            NeuroImpulse?.Invoke(SynapticKnobs);
        }

        private void TransmitInput(NeuroSignal dataSum)
        {
            double val = 0.0;

            if (Role == LayerRole.Input)
            {
                if (input.TryGetValue(dataSum.ItemID, out NeuroSignal incoming))
                {
                    val = incoming.Value;
                }
                else
                {
                    throw new Exception(string.Format(
                        "Can't found input[{0}] in the layer {1}", dataSum.ItemID, this));
                }
            }

            else
            {
                int row = dataSum.ItemID;

                // Calculate neuron input according weight matrix data
                foreach (INeuroTransmitter item in Dendrites)
                {
                    int col = item.ItemID;

                    val += item.Value * weights[row, col];
                }
            }


            dataSum.Value = val;
        }
        #endregion


        #region -> Ctor
        public NeuroLayer(int id, int countIn, int countOut, LayerRole role)
        {
            sync = new object();
            Role = role;
            LayerID = id;

            nucleus = new Dictionary<int, NeuroItem>();
            input = new Dictionary<int, NeuroSignal>();
            output = new Dictionary<int, NeuroSignal>();

            weights =
                Role == LayerRole.Input ? Matrix.Matrix.One(countIn, countOut) :
                Role == LayerRole.Output ? Matrix.Matrix.Half(countIn, countOut) :
                 Matrix.Matrix.Half(countIn, countOut);

            ActivationFooType activation =
                Role == LayerRole.Input ? ActivationFooType.Identity : ActivationFooType.Sigmoid;

            #region -> TMP
            if (id == 2)
            {
                weights[0, 0] = 0.9; weights[0, 1] = 0.3; weights[0, 2] = 0.4;
                weights[1, 0] = 0.2; weights[1, 1] = 0.8; weights[1, 2] = 0.2;
                weights[2, 0] = 0.1; weights[2, 1] = 0.5; weights[2, 2] = 0.6;
            }
            if (id == 3)
            {
                weights[0, 0] = 0.3; weights[0, 1] = 0.7; weights[0, 2] = 0.5;
                weights[1, 0] = 0.6; weights[1, 1] = 0.5; weights[1, 2] = 0.2;
                weights[2, 0] = 0.8; weights[2, 1] = 0.1; weights[2, 2] = 0.9;
            }
            #endregion

            for (int num = 0; num < countIn; num++)
            {
                input.Add(num, new NeuroSignal(num));
            }

            for (int num = 0; num < countOut; num++)
            {
                NeuroItem ni =
                    new NeuroItem(num, TransmitInput, activation);

                nucleus.Add(num, ni);
                output.Add(num, new NeuroSignal(num));
            }
        }
        #endregion
    }
}
