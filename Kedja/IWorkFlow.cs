using System;
using Kedja.Step;

namespace Kedja {
    public interface IWorkFlow {
        IWorkFlow WithTypeFactory(ITypeFactory typeFactory);

        IWorkFlow AddStep<TStep>() where TStep : IStep;
        IWorkFlow AddStep(IStep instance);
        IWorkFlow AddStep<TStep, TReturn>(Action<IBranchNode<TReturn>> branch) where TStep : IStep<TReturn>;
        IWorkFlow AddStep<TStep, TReturn>(IStep<TReturn> instance, Action<IBranchNode<TReturn>> branch);
        IWorkFlow Wait(int ms);

        void Execute();
        void Cancel();
    }
}