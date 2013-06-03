using System;

namespace Kedja.Step {
    internal class DelegateStep : IStep {
        private readonly Action perform;

        public DelegateStep(Action perform) {
            this.perform = perform;
        }

        public void Execute() {
            perform();
        }

        public void Cancel() {}
    }

    internal class DelegateStep<TReturn> : IStep<TReturn> {
        private readonly Func<TReturn> perform;

        public DelegateStep(Func<TReturn> perform) {
            this.perform = perform;
        }

        public TReturn Execute() {
            return perform();
        }

        public void Cancel() {
        }
    }
}