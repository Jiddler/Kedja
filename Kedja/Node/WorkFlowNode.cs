using System;

namespace Kedja.Node {
    internal class WorkFlowNode<TState> : AbstractNode<TState> {
        private readonly Func<IWorkFlowStep<TState>> _builder;

        public WorkFlowNode(AbstractNode<TState> parent, Func<IWorkFlowStep<TState>> builder) : base(parent) {
            _builder = builder;
        }

        public override void Execute() {
            var node = new ContainerNode<TState>(this);

            var step = _builder();

            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return;
            }

            step.Execute(WorkFlowContext.State, node);
            WorkFlowContext.Path.RemoveLast();

            node.Execute();
        }
    }
}