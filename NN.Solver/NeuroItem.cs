using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NN.Solver
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Contains:
    /// - Input request event
    /// - Activation function
    /// - Terminal vesicle
    /// 
    /// https://stackoverflow.com/questions/36384249/list-of-activation-functions-in-c-sharp
    /// http://makeyourownneuralnetwork.blogspot.ru/2016/07/error-backpropagation-revisted.html
    /// </remarks>
    public class NeuroItem
    {
        #region -> Events
        private event Action<NeuroSignal> NeedInput;
        #endregion


        #region -> Data
        private NeuroSignal dataOut;
        private NeuroSignal dataSum;
        #endregion


        #region -> Properties
        public int Number => dataOut.ItemID;
        public INeuroTransmitter Axon => dataOut;
        public ActivationFooType ActivationMethod { get; private set; }
        #endregion


        #region -> Methods
        public void ProcessData()
        {
            NeedInput?.Invoke(dataSum);

            ApplyActivationFoo();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("[{0}]: ", Number);
            sb.AppendFormat("{0}", dataSum.Value.ToString("0.##0"));
            sb.AppendFormat(" -> ");
            sb.AppendFormat("{0}", dataOut.Value.ToString("0.##0"));
            sb.AppendFormat(" ({0})", ActivationMethod);

            return sb.ToString();
        }
        #endregion


        #region -> Implementation
        private void ApplyActivationFoo()
        {
            if (ActivationMethod == ActivationFooType.Identity)
            {
                dataOut.Value = dataSum.Value;
            }
            else if (ActivationMethod == ActivationFooType.Sigmoid)
            {
                dataOut.Value = 1.0 / (1.0 + Math.Exp(-dataSum.Value));
            }
        }
        #endregion


        #region -> Ctor
        public NeuroItem(int num, Action<NeuroSignal> inputDataRequest, ActivationFooType activation)
        {
            ActivationMethod = activation;
            dataSum = new NeuroSignal(num);
            dataOut = new NeuroSignal(num);
            NeedInput = inputDataRequest;
        }
        #endregion
    }
}
