using Kedja.Step;

namespace Kedja.Tests.Step {
    public class RetryableStep : IStep<object, bool> {
        public bool Executed { get; set; }

        public int ExecutedCount { get; private set; }

        public bool Execute(object state) {
            ExecutedCount += 1;
            if(ExecutedCount == 1)
                return false;

            return true;
        }

        public void Cancel(object state) {
        }
    }
}