using Kedja.Instruction;

namespace Kedja.Node {
    internal class RestartNode<TState> : AbstractNode<TState> {
        private readonly INode _target;

        public RestartNode(AbstractNode<TState> parent, AbstractNode<TState> target) : base(parent) {
            _target = target;
        }

        public override void Execute() {
            AbstractNode<TState> current = this;
            do {
                current = current.Parent;
                WorkFlowContext.AddInstruction(current, new BreakInstruction());
            } while(_target != current);

            WorkFlowContext.AddInstruction(current, new RestartInstruction());
        }
    }
}