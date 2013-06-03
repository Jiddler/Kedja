namespace Kedja {
    public interface IWorkFlowBuilder<in TState> {
        void Build(IContainerNode<TState> workFlow);
    }
}