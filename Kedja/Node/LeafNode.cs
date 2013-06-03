using System;
using Kedja.Step;

namespace Kedja.Node {
    internal class LeafNode<TState> : AbstractNode<TState> {
        private readonly Func<IStep<TState>> _factory;

        public LeafNode(AbstractNode<TState> parent, Func<IStep<TState>> factory) : base(parent) {
            _factory = factory;
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            var step = _factory();

            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return;
            }

            step.Execute(WorkFlowContext.State);
            WorkFlowContext.Path.RemoveLast();
        }
    }
}