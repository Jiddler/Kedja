namespace Kedja.Tests.Step {
    public class WorkFlowStep : IWorkFlowStep<object> {
        private readonly GenericStep _step;

        public WorkFlowStep() {
            _step = new GenericStep();
        }

        public bool ExecutedStep {
            get { return _step.Executed; }
        }

        public void Execute(object state, IContainerNode<object> workFlow) {
            workFlow.AddStep(_step);
        }

        public void Cancel(object state) {
        }
    }
}