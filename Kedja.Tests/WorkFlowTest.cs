using System;
using System.Threading;
using System.Threading.Tasks;
using Kedja.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kedja.Tests {
    [TestClass]
    public class WorkFlowTest {
        private WorkFlow _instance;

        [TestInitialize]
        public void Initialize() {
            _instance = new WorkFlow();
        }

        [TestMethod]
        public void End_To_End() {
            var step2 = new GenericStep();
            var step3 = new GenericStep();
            _instance.AddStep<GenericStep, bool>(branch => {
                branch.When(x => x == true).AddStep(step2);
                branch.When(x => x == false).AddStep(step3);
            }).Execute();
            Assert.IsTrue(step2.Executed);
            Assert.IsFalse(step3.Executed);
        }

        [TestMethod]
        public void TypeFactory_Invoked_When_No_Instance_Given() {
            var step = new GenericStep();
            
            var typeFactory = Mock.Of<ITypeFactory>(tf => tf.Create<GenericStep>() == step);
            _instance.WithTypeFactory(typeFactory).AddStep<GenericStep>().Execute();
            
            Assert.IsTrue(step.Executed);
        }

        [TestMethod]
        public void Add_Instance_To_Workflow_Make_Sure_Executed() {
            var step1 = new GenericStep();
            _instance.AddStep(step1).Execute();
            Assert.IsTrue(step1.Executed);
        }

        [TestMethod]
        public void Add_Action_To_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep(() => executed = true).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Func_To_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep(() => {
                executed = true;
                return true;
            }, branch => {
            }).Execute();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Func_To_Sub_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep<GenericStep, bool>(branch => branch.When(x => x).AddStep(() => true, ibranch => ibranch.When(x => x).AddStep(() => executed = true))).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Action_To_Sub_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep<GenericStep, bool>(branch => branch.When(x => x).AddStep(() => executed = true)).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Two_Instance_To_Workflow_Make_Sure_Executed() {
            var step1 = new GenericStep();
            var step2 = new GenericStep();
            _instance
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
                _instance.Cancel();
            });

            _instance.AddStep(new CancelableStep()).Execute();
        }

        [TestMethod]
        public void Cancel_Make_Sure_Following_Steps_Not_Executed() {
            Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                _instance.Cancel();
            });

            var instance = new GenericStep();
            _instance.AddStep(new CancelableStep()).AddStep(instance).Execute();

            Assert.IsFalse(instance.Executed);
        }

        [TestMethod]
        public void Retry() {
            var step = new RetryableStep();
            _instance.AddStep<RetryableStep, bool>(step, branch => branch.When(x => x == false).Retry()).Execute();
            Assert.AreEqual(2, step.ExecutedCount);
        }

        [TestMethod]
        public void Break() {
            var step = new GenericStep();
            var subStep1 = new GenericStep();
            var subStep2 = new GenericStep();
            _instance.AddStep<GenericStep, bool>(step, branch => {
                branch.AddStep(subStep1);
                branch.When(r => r == true).Break();
                branch.AddStep(subStep2);
            }).Execute();

            Assert.IsTrue(step.Executed);
            Assert.IsTrue(subStep1.Executed);
            Assert.IsFalse(subStep2.Executed);
        }

        [TestMethod]
        public void Wait() {
            var step = new GenericStep();
            var beforeExecute = DateTime.UtcNow;
            _instance.AddStep<GenericStep>().Wait(100).Execute();

            Assert.IsTrue((DateTime.UtcNow - beforeExecute).TotalMilliseconds >= 100);
        }
    }

    public class CancelableStep : IStep {
        private readonly ManualResetEvent _sync = new ManualResetEvent(false);

        public void Cancel() {
            _sync.Set();
        }

        public void Execute() {
            _sync.WaitOne();
        }
    }

    public class GenericStep : IStep, IStep<bool> {
        public bool Executed { get; set; }

        protected int ExecutedCount { get; private set; }

        bool IStep<bool>.Execute() {
            Execute();
            return true;
        }

        public void Execute() {
            ExecutedCount += 1;
            Executed = true;
        }

        public void Cancel() {
            
        }
    }

    public class RetryableStep : IStep<bool> {
        public bool Executed { get; set; }

        public int ExecutedCount { get; private set; }

        public bool Execute() {
            ExecutedCount += 1;
            if(ExecutedCount == 1)
                return false;

            return true;
        }

        public void Cancel() {
            
        }
    }
}
