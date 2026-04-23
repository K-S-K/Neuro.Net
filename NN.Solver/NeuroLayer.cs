using System;
using System.Text;
using System.Collections.Generic;

namespace NN.Solver;

public class NeuroLayer : INeuroLayer
{
    #region -> Events
    public event NeuroImpulseDelegate NeuroImpulse;
    #endregion


    #region -> Data
    private readonly object sync;
    private readonly Matrix.Matrix weights;
    private readonly Dictionary<int, NeuroItem> nucleus;
    private readonly Dictionary<int, NeuroSignal> input;
    private readonly Dictionary<int, NeuroSignal> output;
    #endregion


    #region -> Properties
    /// <summary>
    /// Layer number
    /// </summary>
    public int LayerID { get; }

    public LayerRole Role { get; }

    /// <summary>
    /// Dendrites terminal
    /// </summary>
    /// <remarks>
    /// Must be filled from 
    /// pervious layer or input
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

    /// <summary>
    /// Backpropagate errors through this layer and update weights.
    /// Returns the error signal for the previous layer.
    /// </summary>
    /// <param name="errors">Error for each output neuron of this layer</param>
    /// <param name="learningRate">Learning rate</param>
    public double[] Backpropagate(double[] errors, double learningRate)
    {
        int outCount = output.Count;
        int inCount = input.Count;

        // delta[i] = error[i] * activation'(output[i])
        double[] deltas = new double[outCount];
        for (int i = 0; i < outCount; i++)
        {
            double o = output[i].Value;
            double deriv = Role == LayerRole.Input ? 1.0 : o * (1.0 - o);
            deltas[i] = errors[i] * deriv;
        }

        // propagate error back through weight matrix: W^T * delta
        double[] prevErrors = new double[inCount];
        for (int j = 0; j < inCount; j++)
            for (int i = 0; i < outCount; i++)
                prevErrors[j] += weights[i, j] * deltas[i];

        // update weights (input layer is a passthrough — nothing to train)
        if (Role != LayerRole.Input)
            for (int i = 0; i < outCount; i++)
                for (int j = 0; j < inCount; j++)
                    weights[i, j] += learningRate * deltas[i] * input[j].Value;

        return prevErrors;
    }

    /// <summary>
    /// Overwrite the weight matrix from a 2-D array [outputNeuron, inputNeuron].
    /// Dimensions must match the layer topology.
    /// </summary>
    public void SetWeights(double[,] w)
    {
        for (int i = 0; i < w.GetLength(0); i++)
            for (int j = 0; j < w.GetLength(1); j++)
                weights[i, j] = w[i, j];
    }

    /// <summary>
    /// Re-initialize all trainable weights with uniform random values in [-0.5, 0.5].
    /// The input layer is a passthrough and is not affected.
    /// </summary>
    public void RandomizeWeights(Random rnd)
    {
        if (Role == LayerRole.Input) return;
        for (int i = 0; i < weights.Row; i++)
            for (int j = 0; j < weights.Col; j++)
                weights[i, j] = rnd.NextDouble() - 0.5;
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
        // 1) In the beginning we must accept incoming data values
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
                throw new Exception($"Can't found input[{dataSum.ItemID}] in the layer {this}");
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

        nucleus = [];
        input = [];
        output = [];

        // weights[outputNeuron, inputNeuron] — dimensions are [countOut x countIn]
        weights = Role == LayerRole.Input
            ? Matrix.Matrix.One(countOut, countIn)
            : Matrix.Matrix.Random(countOut, countIn);

        ActivationFooType activation =
            Role == LayerRole.Input ? ActivationFooType.Identity : ActivationFooType.Sigmoid;

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
