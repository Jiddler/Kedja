namespace Kedja {
    public interface IWorkFlowStep<in TState> {
        void Execute(TState state, IContainerNode<TState> workFlow);
    }
}