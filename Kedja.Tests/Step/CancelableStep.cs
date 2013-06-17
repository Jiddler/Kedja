using System.Threading;
using Kedja.Step;

namespace Kedja.Tests.Step {
    public class CancelableStep : IStep<object> {
        private readonly ManualResetEvent _sync = new ManualResetEvent(false);

        public void Cancel(object state) {
            _sync.Set();
        }

        public void Execute(object state) {
            _sync.WaitOne();
        }
    }
}