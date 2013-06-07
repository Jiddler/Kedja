namespace Kedja.Step {
    public interface IStep<in TState> : ICancelableStep<TState> {
        void Execute(TState state);
    }

    public interface IStep<in TState, out TReturn> : ICancelableStep<TState> {
        TReturn Execute(TState state);
    }
}