using Kedja.Instruction;

namespace Kedja.Node {
    internal class StopNode<TState> : AbstractNode<TState> {
        private readonly int _levels;

        public StopNode(AbstractNode<TState> parent) : base(parent) {
        }

        public override void Execute() {
            var current = Parent;
            while(current != null) {
                WorkFlowContext.AddInstruction(current, new BreakInstruction());
                current = current.Parent;                
            }
        }
    }
}