namespace Kedja.Step {
    internal class StatelessStepWrapper<TState> : IStep<TState> {
        private readonly IStatelessStep _instance;

        public StatelessStepWrapper(IStatelessStep instance) {
            _instance = instance;
        }

        public void Execute(TState state) {
            _instance.Execute();
        }

        public void Cancel() {
            _instance.Cancel();
        }
    }

    internal class StatelessStepWrapper<TState, TReturn> : IStep<TState, TReturn> {
        private readonly IStatelessStep<TReturn> _instance;

        public StatelessStepWrapper(IStatelessStep<TReturn> instance) {
            _instance = instance;
        }

        public TReturn Execute(TState state) {
            return _instance.Execute();
        }

        public void Cancel() {
            _instance.Cancel();
        }
    }
}