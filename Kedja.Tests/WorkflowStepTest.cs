using System.Threading;
using System.Threading.Tasks;
using Kedja.Tests.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kedja.Tests {
    [TestClass]
    public class WorkflowStepTest : AbstractWorkFlowTest {
        [TestMethod]
        public void SubWorkFlow() {
            var workFlowBuilder = new WorkFlowStep();
            Instance.AddWorkFlow(workFlowBuilder).Execute();
            Assert.IsTrue(workFlowBuilder.ExecutedStep);
        }

        [TestMethod]
        public void Cancel_SubWorkFlow() {
            Task.Factory.StartNew(() => {
                                      Thread.Sleep(100);
                                      Instance.Cancel();
                                  });

            Instance.AddWorkFlow(new CancelableWorkFlowStep()).Execute();
        }
    }
}