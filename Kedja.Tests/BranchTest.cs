using Kedja.Extension;
using Kedja.Tests.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kedja.Tests {
    [TestClass]
    public class BranchTest : AbstractWorkFlowTest {
        [TestMethod]
        public void When() {
            var step2 = new GenericStep();
            var step3 = new GenericStep();
            Instance.AddStep<GenericStep, bool>(branch => {
                                                    branch.When(true).AddStep(step2);
                                                    branch.When(false).AddStep(step3);
                                                }).Execute();
            Assert.IsTrue(step2.Executed);
            Assert.IsFalse(step3.Executed);
        }

        [TestMethod]
        public void Otherwise_Not_Matched() {
            var step2 = new GenericStep();
            var step3 = new GenericStep();
            Instance.AddStep<GenericStep, bool>(branch => {
                                                    branch.When(false).AddStep(step3);
                                                    branch.Otherwise().AddStep(step2);
                                                }).Execute();
            Assert.IsTrue(step2.Executed);
            Assert.IsFalse(step3.Executed);
        }

        [TestMethod]
        public void Otherwise_Matched() {
            var step2 = new GenericStep();
            var step3 = new GenericStep();
            Instance.AddStep<GenericStep, bool>(branch => {
                                                    branch.When(true).AddStep(step3);
                                                    branch.Otherwise().AddStep(step2);
                                                }).Execute();
            Assert.IsFalse(step2.Executed);
            Assert.IsTrue(step3.Executed);
        }
    }
}