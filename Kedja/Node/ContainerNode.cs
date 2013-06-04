using Kedja.Instruction;

namespace Kedja.Node {
    internal class ContainerNode<TState> : AbstractParentNode<TState, IContainerNode<TState>>, IContainerNode<TState> {
        protected override IContainerNode<TState> Self {
            get { return this; }
        }

        internal ContainerNode(AbstractNode<TState> parent) : base(parent) {
        }

        internal ContainerNode(WorkFlowContext<TState> workFlowContext) : base(workFlowContext) {
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            foreach(var step in Nodes) {
                step.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this)) {
                    break;
                }
            }

            WorkFlowContext.RemoveInstructions(this);
        }
    }
}