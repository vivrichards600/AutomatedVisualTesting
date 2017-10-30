using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomatedVisualTesting.Utilities
{
    [TestClass]
    public class InitializeTestContext
    {
        public static TestContext TestContext { get; private set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            TestContext = context;
        }
    }
}