namespace Kedja.Step {
    public interface ICancelableStep<in TState> {
        void Cancel(TState state);
    }
}