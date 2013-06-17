using System;
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
            do {
                if(WorkFlowContext.Canceled)
                    throw new WorkflowCanceledException();

                WorkFlowContext.RemoveInstructions(this);

                foreach(var step in Nodes) {
                    step.Execute();

                    if(WorkFlowContext.HasInstruction<BreakInstruction>(this)) {
                        break;
                    }
                }
            } while(WorkFlowContext.HasInstruction<RestartInstruction>(this));

            WorkFlowContext.RemoveInstructions(this);
        }
    }
}