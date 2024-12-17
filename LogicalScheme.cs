using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1
{
    public interface ILogicalScheme
    {
        Task AddElementAsync(string type, string name);
        Task ConnectElementsAsync(string from, string to);
        Task SetInputValueAsync(string name, bool value);
        Task PrintOutputAsync();
        Task ShowElementAsync(string index);
    }

    public class LogicalScheme : ILogicalScheme
    {
        private readonly Dictionary<string, ILogicalElement> elements;
        private readonly List<string> names;

        public LogicalScheme()
        {
            elements = new Dictionary<string, ILogicalElement>();
            names = new List<string>();
        }

        public  Task AddElementAsync(string type, string name)
        {
            if (elements.ContainsKey(name))
            {
                throw new ArgumentException($"Element with name '{name}' already exists.");
            }

            ILogicalElement element = type.ToLower() switch
            {
                "and" => new AndElement(name),
                "or" => new OrElement(name),
                "not" => new NotElement(name),
                "xor" => new XorElement(name),
                "in" => new InputElement(name),
                "out" => new OutputElement(name),
                _ => throw new ArgumentException($"Invalid element type '{type}'.")
            };

            elements[name] = element;
            names.Add(name);
            Console.WriteLine($"created {name}:{type.ToLower()}");
            return Task.CompletedTask;
        }

        public async Task ConnectElementsAsync(string from, string to)
        {
            string fromName = FromIndexToName(int.Parse(from));
            string[] toParts = to.Split('[');
            string toName = FromIndexToName(int.Parse(toParts[0]));
            int toIndex = toParts.Length > 1 ? int.Parse(toParts[1].TrimEnd(']')) : 0;

            if (!elements.ContainsKey(fromName) || !elements.ContainsKey(toName))
            {
                throw new ArgumentException($"Cannot connect: element '{fromName}' or '{toName}' does not exist.");
            }

            var fromElement = elements[fromName];
            var toElement = elements[toName];

            await toElement.AddInputAsync(fromElement);
            Console.WriteLine($"Connected output of '{fromName}' to input {toIndex} of '{toName}'.");
        }

        public Task SetInputValueAsync(string name, bool value)
        {
            if (!elements.TryGetValue(name, out var element) || !(element is InputElement inputElement))
            {
                throw new ArgumentException($"Element '{name}' is not a valid input.");
            }

            inputElement.SetValue(value);

            return Task.CompletedTask;
        }

        public async Task PrintOutputAsync()
        {
            foreach (var element in elements.Values)
            {
                if (element is OutputElement outputElement)
                {
                    await outputElement.CalculateOutputAsync();
                    Console.WriteLine($"{outputElement.Name}: {outputElement.Output}");
                }
            }
        }

        public async Task ShowElementAsync(string index)
        {
            string name = FromIndexToName(int.Parse(index));
            if (!elements.ContainsKey(name))
            {
                throw new ArgumentException("Invalid element index.");
            }

            var element = elements[name];
            string inputsInfo = string.Join(", ",
                element.Inputs.Select((input, idx) =>
                    input == null
                        ? "null"
                        : input is InputElement
                            ? $"{FromNameToIndex(input.Name)}:{input.Name}"
                            : $"{FromNameToIndex(input.Name)}:{input.GetType().Name.Replace("Element", "").ToLower()}"));

            Console.WriteLine($"{index}:{element.GetType().Name.Replace("Element", "").ToLower()}({inputsInfo})");
        }

        public string FromIndexToName(int index)
        {
            if (index <= 0 || index > names.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Element index is out of range.");
            }
            return names[index - 1];
        }

        public int FromNameToIndex(string name)
        {
            if (!names.Contains(name))
            {
                throw new ArgumentException($"Element '{name}' does not exist.");
            }
            return names.IndexOf(name) + 1;
        }

        public ILogicalElement GetElementByName(string name)
        {
            if (elements.ContainsKey(name))
            {
                return elements[name];
            }
            else
            {
                throw new ArgumentException("Element not found.");
            }
        }

    }
}
