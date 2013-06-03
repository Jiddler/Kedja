using System.Threading;

namespace Kedja.Node {
    internal class WaitNode<TState> : AbstractNode<TState> {
        private readonly int _ms;

        public WaitNode(AbstractNode<TState> parent, int ms) : base(parent) {
            _ms = ms;
        }

        public override void Execute() {
            Thread.Sleep(_ms);
        }
    }
}