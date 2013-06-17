using Kedja.Extension;
using Kedja.Tests.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kedja.Tests {
    [TestClass]
    public class InstructionTest : AbstractWorkFlowTest {
        [TestMethod]
        public void Restart_In_Container() {
            int runs = 0;
            Instance
                .AddStep(state => runs++)
                .AddStep(state => runs == 3, node => node.When(true).Stop())
                .Restart()
                .Execute();

            Assert.AreEqual(3, runs);
        }

        [TestMethod]
        public void Restart_In_Branch() {
            int runs = 0;
            Instance
                .AddStep(state => runs++)
                .AddStep(state => runs == 3,
                    node => {
                        node.When(true).Stop();
                        node.Restart();
                    })
                .Execute();

            Assert.AreEqual(3, runs);
        }

        [TestMethod]
        public void Break() {
            var step = new GenericStep();
            var subStep1 = new GenericStep();
            var subStep2 = new GenericStep();
            Instance.AddStep<GenericStep, bool>(step, branch => {
                branch.AddStep(subStep1);
                branch.When(true).Break();
                branch.AddStep(subStep2);
            }).Execute();

            Assert.IsTrue(step.Executed);
            Assert.IsTrue(subStep1.Executed);
            Assert.IsFalse(subStep2.Executed);
        }

        [TestMethod]
        public void Stop() {
            var step = new GenericStep();
            var subStep1 = new GenericStep();
            var subStep2 = new GenericStep();
            Instance.AddStep<GenericStep, bool>(step, branch => {
                branch.Stop();
                branch.AddStep(subStep1);
            }).AddStep(subStep2).Execute();

            Assert.IsTrue(step.Executed);
            Assert.IsFalse(subStep1.Executed);
            Assert.IsFalse(subStep2.Executed);
        }
    }
}