using System;

namespace Lab1
{
    public class Connection
    {
        public LogicalElement From { get; }
        public LogicalElement To { get; }
        public int ToIndex { get; }

        public Connection(LogicalElement from, LogicalElement to, int toIndex)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from), "Source element cannot be null.");

            if (to == null)
                throw new ArgumentNullException(nameof(to), "Destination element cannot be null.");

            if (toIndex < 0 || toIndex >= to.Inputs.Count)
                throw new ArgumentOutOfRangeException(nameof(toIndex), $"Input index {toIndex} is out of range for element '{to.Name}'.");

            if (to.Inputs[toIndex] != null)
                throw new InvalidOperationException($"Input {toIndex} of element '{to.Name}' is already occupied.");

            From = from;
            To = to;
            ToIndex = toIndex;

            to.Inputs[toIndex] = from;
        }

        public void Validate()
        {
            if (From.Output == null)
                throw new InvalidOperationException($"Source element '{From.Name}' has no calculated output.");

            if (To.Inputs[ToIndex] != From)
                throw new InvalidOperationException($"Connection to input {ToIndex} of '{To.Name}' is not properly established.");
        }

        public override string ToString()
        {
            return $"Connection: {From.Name} -> {To.Name}[{ToIndex}]";
        }
    }
}
