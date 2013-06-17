using System.Threading;
using System.Threading.Tasks;
using Kedja.Extension;
using Kedja.Tests.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kedja.Tests {
    [TestClass]
    public class WorkFlowTest : AbstractWorkFlowTest {
        [TestMethod]
        public void State_Object_Passed_On_Execution() {
            object calledWithState = null;
            Instance.AddStep(state => calledWithState = state).Execute();
            Assert.AreSame(State, calledWithState);
        }

        [TestMethod]
        public void TypeFactory_Invoked_When_No_Instance_Given() {
            var typeFactory = Mock.Of<ITypeFactory>();
            Instance
                .WithTypeFactory(typeFactory)
                .AddStep(state => false, branch => branch.When(true).AddStep<GenericStep>()).Execute();

            Mock.Get(typeFactory).Verify(tf => tf.Create<GenericStep>(), Times.Never());
        }

        [TestMethod]
        public void TypeFactory_Only_Invoked_When_Step_Executed() {
            var step = new GenericStep();

            var typeFactory = Mock.Of<ITypeFactory>(tf => tf.Create<GenericStep>() == step);
            Instance.WithTypeFactory(typeFactory).AddStep<GenericStep>().Execute();

            Assert.IsTrue(step.Executed);
        }

        [TestMethod]
        public void Add_Instance_To_Workflow_Make_Sure_Executed() {
            var step1 = new GenericStep();
            Instance.AddStep(step1).Execute();
            Assert.IsTrue(step1.Executed);
        }

        [TestMethod]
        public void Add_Action_To_Workflow_Make_Sure_Executed() {
            bool executed = false;
            Instance.AddStep(state => executed = true).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Func_To_Workflow_Make_Sure_Executed() {
            bool executed = false;
            Instance.AddStep(state => {
                executed = true;
                return true;
            }, branch => { }).Execute();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Func_To_Sub_Workflow_Make_Sure_Executed() {
            bool executed = false;
            Instance.AddStep<GenericStep, bool>(branch => branch.When(x => x).AddStep(state => true, ibranch => ibranch.When(x => x).AddStep(state => executed = true))).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Action_To_Sub_Workflow_Make_Sure_Executed() {
            bool executed = false;
            Instance.AddStep<GenericStep, bool>(branch => branch.When(x => x).AddStep(state => executed = true)).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Two_Instance_To_Workflow_Make_Sure_Executed() {
            var step1 = new GenericStep();
            var step2 = new GenericStep();
            Instance
                .AddStep(step1)
                .AddStep(step2)
                .Execute();
            Assert.IsTrue(step1.Executed);
            Assert.IsTrue(step2.Executed);
        }

        [TestMethod]
        public void Cancel_The_Flow() {
            Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                Instance.Cancel();
            });

            Instance.AddStep(new CancelableStep()).Execute();
        }

        [TestMethod]
        public void Cancel_Make_Sure_Following_Steps_Not_Executed() {
            Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                Instance.Cancel();
            });

            var instance = new GenericStep();
            Instance.AddStep(new CancelableStep()).AddStep(instance).Execute();

            Assert.IsFalse(instance.Executed);
        }
    }
}