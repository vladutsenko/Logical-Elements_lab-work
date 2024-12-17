using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ILogicalScheme scheme = new LogicalScheme();
            PrintHelp();

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string command = parts[0].ToLower();

                try
                {
                    await ProcessCommandAsync(command, parts, scheme);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid format for command arguments.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("add {elemType} {name}: Add element of type {elemType}. Possible types: and, not, xor, or, in, out.");
            Console.WriteLine("connect {n}-{m}: Connect output of {n} to input of {m}.");
            Console.WriteLine("set {name} {value}: Set value for input element. {value} should be 'true' or 'false'.");
            Console.WriteLine("print: Display output value of the scheme.");
            Console.WriteLine("show {n}: Show information about logical element with ID {n}.");
        }

        static async Task ProcessCommandAsync(string command, string[] parts, ILogicalScheme scheme)
        {
            switch (command)
            {
                case "add":
                    if (parts.Length == 3)
                    {
                        string type = parts[1];
                        string name = parts[2];
                        await scheme.AddElementAsync(type, name);
                    }
                    else
                    {
                        Console.WriteLine("Usage: add {elemType} {name}");
                    }
                    break;

                case "connect":
                    if (parts.Length == 2 && parts[1].Contains('-'))
                    {
                        string[] connection = parts[1].Split('-');
                        if (connection.Length == 2)
                        {
                            await scheme.ConnectElementsAsync(connection[0], connection[1]);
                        }
                        else
                        {
                            Console.WriteLine("Invalid connection format. Use: connect {n}-{m}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Usage: connect {n}-{m}");
                    }
                    break;

                case "set":
                    if (parts.Length == 3 && bool.TryParse(parts[2], out bool value))
                    {
                        string name = parts[1];
                        await scheme.SetInputValueAsync(name, value);
                    }
                    else
                    {
                        Console.WriteLine("Usage: set {name} {true|false}");
                    }
                    break;

                case "print":
                    await scheme.PrintOutputAsync();
                    break;

                case "show":
                    if (parts.Length == 2)
                    {
                        await scheme.ShowElementAsync(parts[1]);
                    }
                    else
                    {
                        Console.WriteLine("Usage: show {n}");
                    }
                    break;

                default:
                    Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
                    break;
            }
        }
    }
}
