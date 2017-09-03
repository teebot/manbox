using ManBox.Business.Batch;
using ManBox.Common.Injection;
using ManBox.Common.Logging;
using ManBox.Common.Mail;
using ManBox.Common.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using System.Transactions;

namespace ManBox.Business.Tests
{
    [TestClass]
    public class ManBoxBusinessTestBase
    {
        TransactionScope _trans;

        [AssemblyInitialize()] // ran only once per test run
        public static void TestsInitialize(TestContext testContext)
        {
            ////setup dependency injection
            var container = DependencyContainer.GetInstance();
            container.Register<ILogger, MockLogger>();
            container.Register<IMailService, MockMailService>();

            container.Register<IBatchRepository, BatchRepository>();

            container.Verify();
        }

        [TestInitialize]
        public void Init()
        {
            // Setup transaction scope
            _trans = new TransactionScope();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _trans.Dispose();
        }
    }
}
