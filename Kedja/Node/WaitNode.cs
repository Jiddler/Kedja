using System.Threading;

namespace Kedja.Node {
    internal class WaitNode : AbstractNode {
        private readonly int _ms;
        private int _retries;

        public WaitNode(AbstractNode parent, int ms) : base(parent) {
            _ms = ms;
        }

        public override void Execute() {
            Thread.Sleep(_ms);
        }
    }
}