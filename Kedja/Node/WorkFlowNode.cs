using System;

namespace Kedja.Node {
    internal class WorkFlowNode<TState> : AbstractNode<TState> {
        private readonly Func<IWorkFlowStep<TState>> _builder;

        public WorkFlowNode(AbstractNode<TState> parent, Func<IWorkFlowStep<TState>> builder) : base(parent) {
            _builder = builder;
        }

        public override void Execute() {
            var node = new ContainerNode<TState>(this);
            _builder().Execute(WorkFlowContext.State, node);
            node.Execute();
        }
    }
}