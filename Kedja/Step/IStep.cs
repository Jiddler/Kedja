namespace Kedja.Step {
    public interface IStep<in TState> : ICancelableStep{
        void Execute(TState state);
    }

    public interface IStep<in TState, out TReturn> : ICancelableStep {
        TReturn Execute(TState state);
    }
}