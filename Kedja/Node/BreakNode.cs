using Kedja.Instruction;

namespace Kedja.Node {
    internal class BreakNode<TState> : AbstractNode<TState> {
        private readonly int _levels;

        public BreakNode(AbstractNode<TState> parent, int levels) : base(parent) {
            _levels = levels + 1;
        }

        public override void Execute() {
            var current = Parent;
            for(int i = 0; i < _levels && current != null; i++) {
                WorkFlowContext.AddInstruction(current, new BreakInstruction());
                current = current.Parent;
            }
        }
    }
}