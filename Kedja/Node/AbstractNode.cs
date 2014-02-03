using System;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal abstract class AbstractNode<TState> : INode {
        private readonly AbstractNode<TState> _parent;
        private readonly WorkFlowContext<TState> _workFlowContext;

        protected AbstractNode(WorkFlowContext<TState> workFlowContext) {
            _workFlowContext = workFlowContext;
        }

        protected AbstractNode(AbstractNode<TState> parent) {
            _parent = parent;
            _workFlowContext = parent.WorkFlowContext;
        }

        public AbstractNode<TState> Parent {
            get { return _parent; }
        }

        public WorkFlowContext<TState> WorkFlowContext {
            get { return _workFlowContext; }
        }

        public void Cancel() {
            WorkFlowContext.Lock(InternalCancel);
        }

        private void InternalCancel() {
            WorkFlowContext.Canceled = true;

            var current = WorkFlowContext.Path.Last;
            while(current != null) {
                current.Value.Cancel(WorkFlowContext.State);
                current = current.Previous;
            }
        }

        public abstract void Execute();
    }
}