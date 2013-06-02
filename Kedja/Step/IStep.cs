namespace Kedja.Step {
    public interface IStep : ICancelableStep{
        void Execute();
    }

    public interface IStep<out TReturn> : ICancelableStep {
        TReturn Execute();
    }
}