namespace Kedja.Step {
    public interface IStatelessStep : ICancelableStep{
        void Execute();
    }

    public interface IStatelessStep<out TReturn> : ICancelableStep {
        TReturn Execute();
    }
}