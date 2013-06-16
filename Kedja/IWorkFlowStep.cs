using Kedja.Step;

namespace Kedja {
    public interface IWorkFlowStep<in TState> : ICancelableStep<TState> {
        void Execute(TState state, IContainerNode<TState> workFlow);
    }
}