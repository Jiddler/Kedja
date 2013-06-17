using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kedja.Tests {
    public abstract class AbstractWorkFlowTest {
        protected readonly object State = new object();
        protected WorkFlow<object> Instance;

        [TestInitialize]
        public void Initialize() {
            Instance = new WorkFlow<object>(State);
        }
    }
}