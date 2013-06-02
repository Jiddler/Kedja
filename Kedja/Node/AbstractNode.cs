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

        protected WorkFlowContext WorkFlowContext {
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

        protected void Execute(IStep step) {
            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return;
            }

            step.Execute();
            WorkFlowContext.Path.RemoveLast();
        }

        protected TReturn Execute<TReturn>(IStep<TReturn> step) {
            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return default(TReturn);
            }

            var stepResult = step.Execute();
            WorkFlowContext.Path.RemoveLast();

            return stepResult;
        }

        public abstract void Execute();
    }
}