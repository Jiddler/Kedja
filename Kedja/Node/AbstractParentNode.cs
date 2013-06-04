using System;
using Kedja.Step;

namespace Kedja.Node {
    internal abstract class AbstractParentNode<TState, TFluent> : AbstractNode<TState> {
        protected NodeCollection<TState> Nodes { get; private set; }
        protected abstract TFluent Self { get; }

        protected AbstractParentNode(WorkFlowContext<TState> workFlowContext) : base(workFlowContext) {
            Nodes = new NodeCollection<TState>(this);
        }

        protected AbstractParentNode(AbstractNode<TState> parent) : base(parent) {
            Nodes = new NodeCollection<TState>(this);
        }
        
        public TFluent AddStep<T>() where T : IStep<TState> {
            Nodes.AddLeafNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return Self;
        }

        public TFluent AddStep(Action<TState> perform) {
            Nodes.AddLeafNode(() => new DelegateStep<TState>(perform));
            return Self;
        }

        public TFluent AddStep(IStep<TState> instance) {
            Nodes.AddLeafNode(() => instance);
            return Self;
        }

        public TFluent AddStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStep<TState, TXReturn> {
            Nodes.AddBranchNode(() => WorkFlowContext.TypeFactory.Create<T>(), branch);
            return Self;
        }

        public TFluent AddStep<TXReturn>(Func<TState, TXReturn> perform, Action<IBranchNode<TState, TXReturn>> branch) {
            Nodes.AddBranchNode(() => new DelegateStep<TState, TXReturn>(perform), branch);
            return Self;
        }

        public TFluent AddStep<T, TXReturn>(IStep<TState, TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch) {
            Nodes.AddBranchNode(() => instance, branch);
            return Self;
        }

        public TFluent AddLevel(Action<IContainerNode<TState>> branch) {
            Nodes.AddLevelNode(branch);
            return Self;
        }

        public TFluent AddStatelessStep(IStatelessStep step) {
            Nodes.AddLeafNode(() => new StatelessStepWrapper<TState>(step));
            return Self;
        }

        public TFluent AddStatelessStep<T>() where T : IStatelessStep {
            Nodes.AddLeafNode(() => new StatelessStepWrapper<TState>(WorkFlowContext.TypeFactory.Create<T>()));
            return Self;
        }

        public TFluent AddStatelessStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStatelessStep<TXReturn> {
            Nodes.AddBranchNode(() => new StatelessStepWrapper<TState, TXReturn>(WorkFlowContext.TypeFactory.Create<T>()), branch);
            return Self;
        }

        public TFluent AddStatelessStep<T, TXReturn>(IStatelessStep<TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch) {
            Nodes.AddBranchNode(() => new StatelessStepWrapper<TState, TXReturn>(instance), branch);
            return Self;
        }

        public TFluent Wait(int ms) {
            Nodes.AddWait(ms);
            return Self;
        }

        public TFluent AddWorkFlow(IWorkFlowBuilder<TState> builder) {
            Nodes.AddWorkFlowNode(() => builder);
            return Self;
        }

        public TFluent AddWorkFlow<T>() where T : IWorkFlowBuilder<TState>{
            Nodes.AddWorkFlowNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return Self;
        }

        public void Retry(int maxRetries = 1) {
            Nodes.AddRetryNode(maxRetries);
        }

        public void Break(int levels = 1) {
            Nodes.AddBreakNode(levels);
        }

        public void Stop() {
            Nodes.AddStop();
        }
    }
}