using Kedja.Instruction;

namespace Kedja.Node {
    internal class RetryNode : AbstractNode {
        private readonly int _maxRetries;
        private int _retries;

        public RetryNode(AbstractNode parent, int maxRetries) : base(parent) {
            _maxRetries = maxRetries;
        }

        public override void Execute() {
            var containerNode = Parent;
            var branchNode = containerNode.Parent;

            WorkFlowContext.AddInstruction(containerNode, new BreakInstruction());
            WorkFlowContext.AddInstruction(branchNode, new BreakInstruction());

            if(_retries++ < _maxRetries) {
                WorkFlowContext.AddInstruction(branchNode, new RetryInstruction());
            }
        }
    }
}