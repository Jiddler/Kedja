using System;
using Kedja.Extension;
using Kedja.Tests.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kedja.Tests {
    [TestClass]
    public class FlowNodeTest : AbstractWorkFlowTest {
        [TestMethod]
        public void Retry() {
            var step = new RetryableStep();
            Instance.AddStep<RetryableStep, bool>(step, branch => branch.When(false).Retry()).Execute();
            Assert.AreEqual(2, step.ExecutedCount);
        }

        [TestMethod]
        public void Break_Out_Of_Level() {
            var subStep1 = new GenericStep();
            var subStep2 = new GenericStep();
            Instance.AddLevel(branch => {
                                  branch.AddStep(subStep1);
                                  branch.Break(2);
                              })
                .AddStep(subStep2)
                .Execute();

            Assert.IsTrue(subStep1.Executed);
            Assert.IsFalse(subStep2.Executed);
        }

        [TestMethod]
        public void Wait() {
            DateTime beforeExecute = DateTime.UtcNow;
            Instance.AddStep<GenericStep>().Wait(100).Execute();

            Assert.IsTrue((DateTime.UtcNow - beforeExecute).TotalMilliseconds >= 100);
        }

        [TestMethod]
        public void Max_Restarts() {
            var step1 = new GenericStep();
            Instance.AddLevel(branch => {
                                  branch.AddStep(step1);
                                  branch.Restart(3);
                              }).Execute();
            Assert.AreEqual(3, step1.ExecutedCount);
        }
    }
}