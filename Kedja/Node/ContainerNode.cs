using System;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class ContainerNode<TState> : AbstractNode<TState>, IContainerNode<TState> {
        private readonly NodeCollection<TState> _nodes;

        internal ContainerNode(AbstractNode<TState> parent) : base(parent) {
            _nodes = new NodeCollection<TState>(this);
        }

        internal ContainerNode(WorkFlowContext<TState> workFlowContext) : base(workFlowContext) {
            _nodes = new NodeCollection<TState>(this);
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

        public IContainerNode<TState> Wait(int ms) {
            _nodes.AddWait(ms);
            return this;
        }

        public void Retry(int maxRetries = 1) {
            _nodes.AddRetryNode(maxRetries);
        }

        public void Break(int levels = 1) {
            _nodes.AddBreakNode(levels);
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

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