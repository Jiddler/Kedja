using Kedja.Instruction;

namespace Kedja.Node {
    internal class RestartNode<TState> : AbstractNode<TState> {
        private readonly INode _target;
        private readonly int _maxRestart;
        private decimal _restarts;

        public RestartNode(AbstractNode<TState> parent, AbstractNode<TState> target, int maxRestart) : base(parent) {
            _target = target;
            _maxRestart = maxRestart;
        }

        public override void Execute() {
            _restarts++;
            if(_maxRestart >= 0 && _maxRestart <= _restarts) {
                return;
            }

            AbstractNode<TState> current = this;
            do {
                current = current.Parent;
                WorkFlowContext.AddInstruction(current, new BreakInstruction());
            } while(_target != current);

            WorkFlowContext.AddInstruction(current, new RestartInstruction());
        }
    }
}