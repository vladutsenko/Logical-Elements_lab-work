using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab1
{
    public interface ILogicalElement
    {
        string Name { get; }
        List<ILogicalElement> Inputs { get; }
        bool? Output { get; }
        Task AddInputAsync(ILogicalElement input);
        Task CalculateOutputAsync();
    }

    public abstract class LogicalElement : ILogicalElement
    {
        public string Name { get; }
        public List<ILogicalElement> Inputs { get; }
        public bool? Output { get; protected set; }

        protected abstract int ExpectedInputCount { get; } // Ожидаемое количество входов

        protected LogicalElement(string name, int length)
        {
            Name = name;
            Inputs = new List<ILogicalElement>(new ILogicalElement[length]);
        }

        public async Task AddInputAsync(ILogicalElement input)
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (Inputs[i] == null)
                {
                    Inputs[i] = input;
                    return;
                }
            }
            throw new InvalidOperationException($"All inputs of element '{Name}' are already occupied.");
        }

        public async Task CalculateOutputAsync()
        {
            await ValidateInputsAsync();
            Calculate();
        }

        protected abstract void Calculate();

        protected async Task ValidateInputsAsync()
        {
            if (this is InputElement)
            {
                // InputElement не требует проверки входов
                return;
            }

            if (Inputs.Count != ExpectedInputCount)
            {
                throw new InvalidOperationException($"Element '{Name}' requires {ExpectedInputCount} inputs, but {Inputs.Count} provided.");
            }

            foreach (var input in Inputs)
            {
                if (input == null)
                {
                    throw new InvalidOperationException($"Element '{Name}' has unconnected inputs.");
                }
                await input.CalculateOutputAsync();
            }
        }
    }

    public class AndElement : LogicalElement
    {
        protected override int ExpectedInputCount => 2;

        public AndElement(string name) : base(name, 2) { }

        protected override void Calculate()
        {
            Output = Inputs[0].Output.Value && Inputs[1].Output.Value;
        }
    }

    public class OrElement : LogicalElement
    {
        protected override int ExpectedInputCount => 2;

        public OrElement(string name) : base(name, 2) { }

        protected override void Calculate()
        {
            Output = Inputs[0].Output.Value || Inputs[1].Output.Value;
        }
    }

    public class NotElement : LogicalElement
    {
        protected override int ExpectedInputCount => 1;

        public NotElement(string name) : base(name, 1) { }

        protected override void Calculate()
        {
            Output = !Inputs[0].Output.Value;
        }
    }

    public class XorElement : LogicalElement
    {
        protected override int ExpectedInputCount => 2;

        public XorElement(string name) : base(name, 2) { }

        protected override void Calculate()
        {
            Output = Inputs[0].Output.Value ^ Inputs[1].Output.Value;
        }
    }

    public class InputElement : LogicalElement
    {
        protected override int ExpectedInputCount => 0; // Входы не требуются

        public InputElement(string name) : base(name, 1) { }

        public void SetValue(bool value)
        {
            Output = value;
        }

        protected override void Calculate()
        {
            // Нет необходимости в вычислениях
        }
    }

    public class OutputElement : LogicalElement
    {
        protected override int ExpectedInputCount => 1;

        public OutputElement(string name) : base(name, 1) { }

        protected override void Calculate()
        {
            Output = Inputs[0].Output;
        }
    }
}
