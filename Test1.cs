using Lab1;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public async Task AddElement_CreatesAndElement()
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("and", "1");

            Assert.AreEqual("1", scheme.FromIndexToName(1));
            Assert.IsInstanceOfType(scheme.GetElementByName("1"), typeof(AndElement));
        }

        [TestMethod]
        public async Task ConnectElements_ValidConnection()
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("and", "1");
            await scheme.AddElementAsync("in", "A");
            await scheme.ConnectElementsAsync("2", "1[0]");

            var andElement = (AndElement)scheme.GetElementByName("1");
            Assert.IsNotNull(andElement.Inputs[0]);
            Assert.IsInstanceOfType(andElement.Inputs[0], typeof(InputElement));
        }

        [TestMethod]
        public async Task CalculateOutput_AndGate()
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("and", "1");
            await scheme.AddElementAsync("in", "A");
            await scheme.AddElementAsync("in", "B");
            await scheme.SetInputValueAsync("A", true);
            await scheme.SetInputValueAsync("B", false);
            await scheme.ConnectElementsAsync("2", "1[0]");
            await scheme.ConnectElementsAsync("3", "1[1]");

            var andElement = (AndElement)scheme.GetElementByName("1");
            await andElement.CalculateOutputAsync();

            Assert.IsFalse(andElement.Output.Value);
        }

        [TestMethod]
        public async Task CalculateOutput_OrGate()
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("or", "1");
            await scheme.AddElementAsync("in", "A");
            await scheme.AddElementAsync("in", "B");
            await scheme.SetInputValueAsync("A", true);
            await scheme.SetInputValueAsync("B", false);
            await scheme.ConnectElementsAsync("2", "1[0]");
            await scheme.ConnectElementsAsync("3", "1[1]");

            var orElement = (OrElement)scheme.GetElementByName("1");
            await orElement.CalculateOutputAsync();

            Assert.IsTrue(orElement.Output.Value);
        }

        [TestMethod]
        public async Task CalculateOutput_NotGate()
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("not", "1");
            await scheme.AddElementAsync("in", "A");
            await scheme.SetInputValueAsync("A", true);
            await scheme.ConnectElementsAsync("2", "1[0]");

            var notElement = (NotElement)scheme.GetElementByName("1");
            await notElement.CalculateOutputAsync();

            Assert.IsFalse(notElement.Output.Value);
        }

        [TestMethod]
        public async Task CalculateOutput_XorGate()
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("xor", "1");
            await scheme.AddElementAsync("in", "A");
            await scheme.AddElementAsync("in", "B");
            await scheme.SetInputValueAsync("A", true);
            await scheme.SetInputValueAsync("B", false);
            await scheme.ConnectElementsAsync("2", "1[0]");
            await scheme.ConnectElementsAsync("3", "1[1]");

            var xorElement = (XorElement)scheme.GetElementByName("1");
            await xorElement.CalculateOutputAsync();

            Assert.IsTrue(xorElement.Output.Value);
        }

        [TestMethod]
        [DataRow(true, false, false)]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, true, true)]
        public async Task CalculateOutput_ComplexCases(bool inputA, bool inputB, bool expectedOutput)
        {
            var scheme = new LogicalScheme();
            await scheme.AddElementAsync("or", "1");
            await scheme.AddElementAsync("not", "2");
            await scheme.AddElementAsync("xor", "M");
            await scheme.AddElementAsync("in", "A");
            await scheme.AddElementAsync("in", "B");
            await scheme.AddElementAsync("out", "R");

            await scheme.ConnectElementsAsync("4", "1[0]");
            await scheme.ConnectElementsAsync("5", "1[1]");
            await scheme.ConnectElementsAsync("1", "2");
            await scheme.ConnectElementsAsync("2", "3[0]");
            await scheme.ConnectElementsAsync("5", "3[1]");
            await scheme.ConnectElementsAsync("3", "6");

            await scheme.SetInputValueAsync("A", inputA);
            await scheme.SetInputValueAsync("B", inputB);

            var OutputElement = (OutputElement)scheme.GetElementByName("R");
            await OutputElement.CalculateOutputAsync();

            Assert.AreEqual(expectedOutput, OutputElement.Output.Value);
        }
    }
}
