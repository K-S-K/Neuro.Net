using System;
using System.Collections.Generic;

namespace NN.Solver
{
    public class NeuroNetwork
    {
        #region -> Events
        public event NeuroImpulseDelegate NeuroImpulse;
        #endregion


        #region -> Data
        private readonly List<NeuroLayer> layers;

        private readonly INeuroLayer layerFirst;
        private readonly INeuroLayer layerLast;
        #endregion


        #region -> Properties
        /// <summary>
        /// Dendrites terminal — feed your input values here
        /// </summary>
        public IEnumerable<INeuroTransmitter> Dendrites { get; private set; }

        /// <summary>
        /// Axon terminal — read the network's output values here
        /// </summary>
        public IEnumerable<INeuroTransmitter> SynapticKnobs { get; private set; }
        #endregion


        #region -> Methods
        /// <summary>
        /// Run a forward pass and fire NeuroImpulse with the result.
        /// </summary>
        public void ProcessData(IEnumerable<INeuroTransmitter> incoming)
        {
            layerFirst.ProcessData(incoming);
        }

        /// <summary>
        /// Run one training step: forward pass then backpropagation.
        /// </summary>
        /// <param name="inputs">Input signals</param>
        /// <param name="targets">Desired output values, one per output neuron</param>
        /// <param name="learningRate">Learning rate (default 0.3)</param>
        public void Train(IEnumerable<INeuroTransmitter> inputs, double[] targets, double learningRate = 0.3)
        {
            ProcessData(inputs);

            double[] errors = new double[targets.Length];
            int i = 0;
            foreach (INeuroTransmitter knob in layerLast.SynapticKnobs)
            {
                errors[i] = targets[i] - knob.Value;
                i++;
            }

            for (int idx = layers.Count - 1; idx >= 0; idx--)
                errors = layers[idx].Backpropagate(errors, learningRate);
        }

        /// <summary>
        /// Re-randomize all trainable weights using the given seed.
        /// Useful for reproducible experiments.
        /// </summary>
        public void Seed(int seed)
        {
            Random rnd = new Random(seed);
            foreach (NeuroLayer layer in layers)
                layer.RandomizeWeights(rnd);
        }
        #endregion


        #region -> Ctor
        /// <summary>
        /// Classic 3-3-3 network with the original hardcoded weights
        /// (kept so existing tests still pass).
        /// </summary>
        public NeuroNetwork() : this(3, 3, 3)
        {
            layers[1].SetWeights(new[,] {
                { 0.9, 0.3, 0.4 },
                { 0.2, 0.8, 0.2 },
                { 0.1, 0.5, 0.6 }
            });
            layers[2].SetWeights(new[,] {
                { 0.3, 0.7, 0.5 },
                { 0.6, 0.5, 0.2 },
                { 0.8, 0.1, 0.9 }
            });
        }

        /// <summary>
        /// Configurable network. Pass layer sizes from input to output,
        /// e.g. <c>new NeuroNetwork(2, 4, 1)</c> for a 2-input, 4-hidden, 1-output network.
        /// </summary>
        public NeuroNetwork(params int[] layerSizes)
        {
            if (layerSizes == null || layerSizes.Length < 2)
                throw new ArgumentException("Provide at least 2 layer sizes (input + output).");

            layers = new List<NeuroLayer>();

            for (int idx = 0; idx < layerSizes.Length; idx++)
            {
                int id = idx + 1;
                int countIn = idx == 0 ? layerSizes[0] : layerSizes[idx - 1];
                int countOut = layerSizes[idx];

                LayerRole role =
                    idx == 0 ? LayerRole.Input :
                    idx == layerSizes.Length - 1 ? LayerRole.Output :
                    LayerRole.Inner;

                NeuroLayer layer = new NeuroLayer(id, countIn, countOut, role);

                if (layers.Count > 0)
                    layers[layers.Count - 1].NeuroImpulse += layer.ProcessData;

                layers.Add(layer);
            }

            layerFirst = layers[0];
            layerLast = layers[layers.Count - 1];

            Dendrites = layerFirst.Dendrites;
            SynapticKnobs = layerLast.SynapticKnobs;

            layerLast.NeuroImpulse += knobs => NeuroImpulse?.Invoke(knobs);
        }
        #endregion
    }
}
