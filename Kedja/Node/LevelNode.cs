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
            do {
                if(WorkFlowContext.Canceled)
                    throw new WorkflowCanceledException();

                WorkFlowContext.RemoveInstructions(this);

                Nodes.Clear();
                _branch(this);

                foreach(var step in Nodes) {
                    step.Execute();

                    if(WorkFlowContext.HasInstruction<BreakInstruction>(this)) {
                        break;
                    }
                }
            } while(WorkFlowContext.HasInstruction<RestartInstruction>(this));

            WorkFlowContext.RemoveInstructions(this);
        }

        public void Restart() {
            Nodes.AddRestart(this);
        }
    }
}