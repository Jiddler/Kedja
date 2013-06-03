using System;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class ContainerNode<TState> : AbstractNode<TState>, IContainerNode<TState> {
        private readonly Action<IContainerNode<TState>> _branch;
        private readonly NodeCollection<TState> _nodes;

        internal ContainerNode(AbstractNode<TState> parent) : base(parent) {
            _nodes = new NodeCollection<TState>(this);
        }

        internal ContainerNode(WorkFlowContext<TState> workFlowContext) : base(workFlowContext) {
            _nodes = new NodeCollection<TState>(this);
        }

        public ContainerNode(AbstractNode<TState> workFlowContext, Action<IContainerNode<TState>> branch) : base(workFlowContext) {
            _nodes = new NodeCollection<TState>(this);
            _branch = branch;
        }

        public IContainerNode<TState> AddStep<T>() where T : IStep<TState> {
            _nodes.AddLeafNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return this;
        }

        public IContainerNode<TState> AddStep(Action<TState> perform) {
            _nodes.AddLeafNode(() => new DelegateStep<TState>(perform));
            return this;
        }

        public IContainerNode<TState> AddStep(IStep<TState> instance) {
            _nodes.AddLeafNode(() => instance);
            return this;
        }

        public IContainerNode<TState> AddStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStep<TState, TXReturn> {
            _nodes.AddBranchNode(() => WorkFlowContext.TypeFactory.Create<T>(), branch);
            return this;
        }

        public IContainerNode<TState> AddStep<TXReturn>(Func<TState, TXReturn> perform, Action<IBranchNode<TState, TXReturn>> branch) {
            _nodes.AddBranchNode(() => new DelegateStep<TState, TXReturn>(perform), branch);
            return this;
        }

        public IContainerNode<TState> AddStep<T, TXReturn>(IStep<TState, TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch) {
            _nodes.AddBranchNode(() => instance, branch);
            return this;
        }

        public IContainerNode<TState> AddLevel(Action<IContainerNode<TState>> branch) {
            _nodes.AddLevelNode(branch);
            return this;
        }

        public IContainerNode<TState> AddStatelessStep(IStatelessStep step) {
            _nodes.AddLeafNode(() => new StatelessStepWrapper<TState>(step));
            return this;
        }

        public IContainerNode<TState> AddStatelessStep<T>() where T : IStatelessStep {
            _nodes.AddLeafNode(() => new StatelessStepWrapper<TState>(WorkFlowContext.TypeFactory.Create<T>()));
            return this;
        }

        public IContainerNode<TState> AddStatelessStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStatelessStep<TXReturn> {
            _nodes.AddBranchNode(() => new StatelessStepWrapper<TState, TXReturn>(WorkFlowContext.TypeFactory.Create<T>()), branch);
            return this;
        }

        public IContainerNode<TState> AddStatelessStep<T, TXReturn>(IStatelessStep<TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch) {
            _nodes.AddBranchNode(() => new StatelessStepWrapper<TState, TXReturn>(instance), branch);
            return this;
        }

        public IContainerNode<TState> Wait(int ms) {
            _nodes.AddWait(ms);
            return this;
        }

        public IContainerNode<TState> AddWorkFlow(IWorkFlowBuilder<TState> builder) {
            _nodes.AddWorkFlowNode(() => builder);
            return this;
        }

        public IContainerNode<TState> AddWorkFlow<T>() where T : IWorkFlowBuilder<TState>{
            _nodes.AddWorkFlowNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return this;
        }

        public void Retry(int maxRetries = 1) {
            _nodes.AddRetryNode(maxRetries);
        }

        public void Break(int levels = 1) {
            _nodes.AddBreakNode(levels);
        }

        public void Stop() {
            _nodes.AddStop();
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            if(_branch != null) {
                _nodes.Clear();
                _branch(this);
            }

            foreach(var step in _nodes) {
                step.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this)) {
                    break;
                }
            }

            WorkFlowContext.RemoveInstructions(this);
        }
    }
}