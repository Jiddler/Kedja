using System;

namespace Kedja.Node {
    internal class WorkFlowNode<TState> : AbstractNode<TState> {
        private readonly Func<IWorkFlowBuilder<TState>> _builder;

        public WorkFlowNode(AbstractNode<TState> parent, Func<IWorkFlowBuilder<TState>> builder) : base(parent) {
            _builder = builder;
        }

        public override void Execute() {
            var node = new ContainerNode<TState>(this);
            _builder().Build(node);
            node.Execute();
        }
    }
}