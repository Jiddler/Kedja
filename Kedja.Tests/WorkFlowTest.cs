using System;
using System.Threading;
using System.Threading.Tasks;
using Kedja.Extension;
using Kedja.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kedja.Tests {
    [TestClass]
    public class WorkFlowTest {
        private WorkFlow<object> _instance;
        private readonly object _state = new object();

        [TestInitialize]
        public void Initialize() {
            _instance = new WorkFlow<object>(_state);
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
        public void Max_Restarts() {
            var step1 = new GenericStep();
            _instance.AddLevel(branch => {
                                   branch.AddStep(step1);
                                   branch.Restart(3);
                               }).Execute();
            Assert.AreEqual(3, step1.ExecutedCount);
        }

        [TestMethod]
        public void State_Object_Passed_On_Execution() {
            object calledWithState = null;
            _instance.AddStep(state => calledWithState = state).Execute();
            Assert.AreSame(_state, calledWithState);
        }

        [TestMethod]
        public void TypeFactory_Invoked_When_No_Instance_Given() {
            var typeFactory = Mock.Of<ITypeFactory>();
            _instance
                .WithTypeFactory(typeFactory)
                .AddStep<bool>(state => false, branch => branch.When(x => x == true).AddStep<GenericStep>()).Execute();
            
            Mock.Get(typeFactory).Verify(tf => tf.Create<GenericStep>(), Times.Never());
        }

        [TestMethod]
        public void TypeFactory_Only_Invoked_When_Step_Executed() {
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
            _instance.AddStep(state => executed = true).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Func_To_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep(state => {
                executed = true;
                return true;
            }, branch => {
            }).Execute();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Func_To_Sub_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep<GenericStep, bool>(branch => branch.When(x => x).AddStep(state => true, ibranch => ibranch.When(x => x).AddStep(state => executed = true))).Execute();
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Add_Action_To_Sub_Workflow_Make_Sure_Executed() {
            var executed = false;
            _instance.AddStep<GenericStep, bool>(branch => branch.When(x => x).AddStep(state => executed = true)).Execute();
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
        public void Break_Out_Of_Level() {
            var subStep1 = new GenericStep();
            var subStep2 = new GenericStep();
            _instance.AddLevel(branch => {
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
            var step = new GenericStep();
            var beforeExecute = DateTime.UtcNow;
            _instance.AddStep<GenericStep>().Wait(100).Execute();

            Assert.IsTrue((DateTime.UtcNow - beforeExecute).TotalMilliseconds >= 100);
        }

        [TestMethod]
        public void Stop() {
            var step = new GenericStep();
            var subStep1 = new GenericStep();
            var subStep2 = new GenericStep();
            _instance.AddStep<GenericStep, bool>(step, branch => {
                branch.Stop();
                branch.AddStep(subStep1);
            }).AddStep(subStep2).Execute();

            Assert.IsTrue(step.Executed);
            Assert.IsFalse(subStep1.Executed);
            Assert.IsFalse(subStep2.Executed);
        }
        
        [TestMethod]
        public void SubWorkFlow() {
            var workFlowBuilder = new WorkFlowStep();
            _instance.AddWorkFlow(workFlowBuilder).Execute();
            Assert.IsTrue(workFlowBuilder.ExecutedStep);
        }

        [TestMethod]
        public void Cancel_SubWorkFlow() {
            Task.Factory.StartNew(() => {
                Thread.Sleep(100);
                _instance.Cancel();
            });

            _instance.AddWorkFlow(new CancelableWorkFlowStep()).Execute();
        }

        [TestMethod]
        public void Restart_In_Container() {
            var runs = 0;
            _instance
                .AddStep(state => runs++)
                .AddStep(state => runs == 3, node => node.When(true).Stop())
                .Restart()
                .Execute();

            Assert.AreEqual(3, runs);
        }

        [TestMethod]
        public void Restart_In_Branch() {
            var runs = 0;
            _instance
                .AddStep(state => runs++)
                .AddStep(state => runs == 3,
                    node => {
                        node.When(true).Stop();
                        node.Restart();
                    })
                .Execute();

            Assert.AreEqual(3, runs);
        }
    }

    public class WorkFlowStep : IWorkFlowStep<object> {
        private readonly GenericStep _step;

        public bool ExecutedStep { get { return _step.Executed; } }

        public WorkFlowStep() {
            _step = new GenericStep();
        }

        public void Execute(object state, IContainerNode<object> workFlow) {
            workFlow.AddStep(_step);
        }

        public void Cancel(object state) {
        }
    }

    public class CancelableWorkFlowStep : IWorkFlowStep<object> {
        private readonly GenericStep _step;
        private readonly ManualResetEvent _sync = new ManualResetEvent(false);

        public bool ExecutedStep { get { return _step.Executed; } }

        public CancelableWorkFlowStep() {
            _step = new GenericStep();
        }

        public void Execute(object state, IContainerNode<object> workFlow) {
            workFlow.AddStep(_step);
            _sync.WaitOne();
        }

        public void Cancel(object state) {
            _sync.Set();
        }
    }

    public class CancelableStep : IStep<object> {
        private readonly ManualResetEvent _sync = new ManualResetEvent(false);

        public void Cancel(object state) {
            _sync.Set();
        }

        public void Execute(object state) {
            _sync.WaitOne();
        }
    }

    public class GenericStep : IStep<object>, IStep<object, bool> {
        public bool Executed { get; set; }

        public int ExecutedCount { get; private set; }

        bool IStep<object, bool>.Execute(object state) {
            Execute(state);
            return true;
        }

        public void Execute(object state) {
            ExecutedCount += 1;
            Executed = true;
        }

        public void Cancel(object state) {
            
        }
    }

    public class RetryableStep : IStep<object, bool> {
        public bool Executed { get; set; }

        public int ExecutedCount { get; private set; }

        public bool Execute(object state) {
            ExecutedCount += 1;
            if(ExecutedCount == 1)
                return false;

            return true;
        }

        public void Cancel(object state) {
            
        }
    }
}
