# Neuro.Net

A handmade feedforward neural network in C#, built from scratch as a learning project.
Supports configurable topology, sigmoid activation, and gradient-descent backpropagation.

---

## Architecture

```text
Inputs ──► [ Layer 1: Input ] ──► [ Layer 2..N-1: Hidden ] ──► [ Layer N: Output ] ──► Outputs
```

| Class | Role |
| --- | --- |
| `NeuroNetwork` | Wires layers together; exposes `ProcessData` and `Train` |
| `NeuroLayer` | One layer: weight matrix, input/output signals, forward+backward pass |
| `NeuroItem` | One neuron: weighted sum → activation function → output signal |
| `Matrix` | 2-D double matrix with multiply, transpose, XML serialization |

Layers are chained via C# events (`NeuroImpulse`): each layer fires an event when it
finishes its forward pass, and the next layer's `ProcessData` is subscribed to that event.

---

## Forward pass

For every non-input neuron `i` in a layer, the **weighted sum** of its inputs is:

```text
net_i = Σ_j ( w[i,j] * input_j )
```

Then the **sigmoid activation** is applied:

```text
output_i = 1 / (1 + e^(-net_i))
```

The input layer is a passthrough (identity activation, weight matrix = identity).

---

## Training: backpropagation

Training adjusts the weights to minimise the squared error between the network's
output and a desired target, using the **gradient-descent** rule.

### One training step

**1. Forward pass** — run the network normally to get the current output.

**2. Output error** — compute how far off each output neuron is:

```text
error_i = target_i - output_i
```

**3. Output-layer delta** — scale by the sigmoid derivative `σ'(o) = o(1-o)`:

```text
delta_i = error_i * output_i * (1 - output_i)
```

**4. Weight update** — nudge each weight in the direction that reduces the error:

```text
w[i,j]  +=  learningRate * delta_i * input_j
```

**5. Propagate error to the previous layer** — use the transposed weight matrix
so each input node receives the sum of all deltas it contributed to:

```text
prevError_j = Σ_i ( w[i,j] * delta_i )        (= W^T · delta)
```

Repeat steps 3–5 back through every layer.

### Learning rate

A typical value is `0.1`–`0.5`. Too high → weights overshoot and oscillate.
Too low → training is slow. The default is `0.3`.

---

## Usage

### 1. Create a network

```csharp
// Topology: 2 inputs → 4 hidden neurons → 1 output
NeuroNetwork n = new NeuroNetwork(2, 4, 1);

// Optional: fix the random seed for reproducible experiments
n.Seed(42);
```

### 2. Prepare signals

```csharp
var inputs = new List<NeuroSignal> {
    new NeuroSignal(0, 0.9),
    new NeuroSignal(1, 0.1),
};
```

### 3. Train

```csharp
double[] target = { 1.0 };   // desired output

for (int epoch = 0; epoch < 10_000; epoch++)
    n.Train(inputs, target, learningRate: 0.5);
```

### 4. Predict

```csharp
n.ProcessData(inputs);

foreach (INeuroTransmitter knob in n.SynapticKnobs)
    Console.WriteLine($"Output: {knob.Value:F3}");
```

---

## XOR example

XOR is a classic benchmark: it is not linearly separable, so a single-layer network
cannot solve it — a hidden layer is required.

| x₀ | x₁ | x₀ XOR x₁ |
| --- | --- | --- |
| 0 | 0 | 0 |
| 0 | 1 | 1 |
| 1 | 0 | 1 |
| 1 | 1 | 0 |

The unit test `XorTrain_Converges` trains a `2 → 4 → 1` network for 30,000 steps
and verifies that all four cases are answered correctly (within 0.1 of the target).
It converges in under 50 ms.

---

## Build & test

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build
dotnet test
```

Expected output:

```text
Passed  MatrixFill
Passed  MatrixProduct
Passed  MatrixTranspose
Passed  XorTrain_LossDecreases
Passed  XorTrain_Converges
Passed  NtwrkSolve
```

---

## References

- *Make Your Own Neural Network* — Tariq Rashid
  (the blog posts at `makeyourownneuralnetwork.blogspot.com` were the original
  inspiration for this project)
- [Activation functions in C#](https://stackoverflow.com/questions/36384249/list-of-activation-functions-in-c-sharp)
- [Matrix multiplication](https://en.wikipedia.org/wiki/Matrix_multiplication)
