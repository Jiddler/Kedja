using System;
using Kedja.Step;

namespace Kedja {
    public interface IBranchNode<out TState, out TReturn> {
        IContainerNode<TState> When(Func<TReturn, bool> when);
        
        IBranchNode<TState, TReturn> AddStep<T>() where T : IStep<TState>;
        IBranchNode<TState, TReturn> AddStep(IStep<TState> instance);
        IBranchNode<TState, TReturn> AddStep(Action<TState> perform);

        IBranchNode<TState, TReturn> AddStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStep<TState, TXReturn>;
        IBranchNode<TState, TReturn> AddStep<TXReturn>(Func<TState, TXReturn> perform, Action<IBranchNode<TState, TXReturn>> branch);
        IBranchNode<TState, TReturn> AddStep<T, TXReturn>(IStep<TState, TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch);

        IBranchNode<TState, TReturn> AddWorkFlow(IWorkFlowBuilder<TState> builder);
        IBranchNode<TState, TReturn> AddWorkFlow<T>() where T : IWorkFlowBuilder<TState>;

        IBranchNode<TState, TReturn> Wait(int ms);

        void Stop();
        void Execute();
    }
}