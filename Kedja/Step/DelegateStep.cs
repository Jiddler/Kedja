using System;

namespace Kedja.Step {
    internal class DelegateStep<TState> : IStep<TState> {
        private readonly Action<TState> _perform;

        public DelegateStep(Action<TState> perform) {
            _perform = perform;
        }

        public void Execute(TState state) {
            _perform(state);
        }

        public void Cancel() {}
    }

    internal class DelegateStep<TState, TReturn> : IStep<TState, TReturn> {
        private readonly Func<TState, TReturn> _perform;

        public DelegateStep(Func<TState, TReturn> perform) {
            _perform = perform;
        }

        public TReturn Execute(TState state) {
            return _perform(state);
        }

        public void Cancel() {
        }
    }
}