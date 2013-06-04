using System;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class LevelNode<TState> : AbstractParentNode<TState, IContainerNode<TState>>, IContainerNode<TState> {
        private readonly Action<IContainerNode<TState>> _branch;

        protected override IContainerNode<TState> Self {
            get { return this; }
        }

        public LevelNode(AbstractNode<TState> workFlowContext, Action<IContainerNode<TState>> branch) : base(workFlowContext) {
            _branch = branch;
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            Nodes.Clear();
            _branch(this);

            foreach(var step in Nodes) {
                step.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this)) {
                    break;
                }
            }

            WorkFlowContext.RemoveInstructions(this);
        }

        public IContainerNode<TState> AddLevel(Action<IContainerNode<TState>> branch) {
            Nodes.AddLevelNode(branch);
            return this;
        }
    }
}