using Kedja.Step;

namespace Kedja.Tests.Step {
    public class GenericStep : IStep<object>, IStep<object, bool> {
        public bool Executed { get; set; }

        public int ExecutedCount { get; private set; }

        bool IStep<object, bool>.Execute(object state) {
            Execute(state);
            return true;
        }

        public void Execute(object state) {
            ExecutedCount += 1;
            Executed = true;
        }

        public void Cancel(object state) {
        }
    }
}