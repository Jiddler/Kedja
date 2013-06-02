using Kedja.Step;

namespace Kedja.Node {
    internal class LeafNode : AbstractNode {
        private readonly IStep _step;

        internal LeafNode(IStep step, AbstractNode parent) : base(parent) {
            _step = step;
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            Execute(_step);
        }
    }
}