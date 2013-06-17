using System.Threading;

namespace Kedja.Tests.Step {
    public class CancelableWorkFlowStep : IWorkFlowStep<object> {
        private readonly GenericStep _step;
        private readonly ManualResetEvent _sync = new ManualResetEvent(false);

        public CancelableWorkFlowStep() {
            _step = new GenericStep();
        }

        public bool ExecutedStep {
            get { return _step.Executed; }
        }

        public void Execute(object state, IContainerNode<object> workFlow) {
            workFlow.AddStep(_step);
            _sync.WaitOne();
        }

        public void Cancel(object state) {
            _sync.Set();
        }
    }
}