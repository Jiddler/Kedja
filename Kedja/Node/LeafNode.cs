using System;
using Kedja.Step;

namespace Kedja.Node {
    internal class LeafNode : AbstractNode {
        private readonly Func<IStep> _factory;

        public LeafNode(AbstractNode parent, Func<IStep> factory) : base(parent) {
            _factory = factory;
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            IStep step = _factory();

            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return;
            }

            step.Execute();
            WorkFlowContext.Path.RemoveLast();
        }
    }
}