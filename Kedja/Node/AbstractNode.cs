using System;
using Kedja.Step;

namespace Kedja.Node {
    internal abstract class AbstractNode : INode {
        private readonly AbstractNode _parent;
        private readonly WorkFlowContext _workFlowContext;

        protected AbstractNode( WorkFlowContext workFlowContext) {
            _workFlowContext = workFlowContext;
        }

        protected AbstractNode(AbstractNode parent) {
            _parent = parent;
            _workFlowContext = parent.WorkFlowContext;
        }

        public AbstractNode Parent {
            get { return _parent; }
        }

        public WorkFlowContext WorkFlowContext {
            get { return _workFlowContext; }
        }

        public void Cancel() {
            WorkFlowContext.Lock(InternalCancel);
        }

        private void InternalCancel() {
            WorkFlowContext.Canceled = true;

            var current = WorkFlowContext.Path.Last;
            if(current != null) {
                current.Value.Cancel();
            }
        }

        public abstract void Execute();
    }
}