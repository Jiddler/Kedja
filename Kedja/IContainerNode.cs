using System;
using Kedja.Step;

namespace Kedja {
    public interface IContainerNode<out TState> {
        IContainerNode<TState> AddStep<T>() where T : IStep<TState>;
        IContainerNode<TState> AddStep(Action<TState> perform);
        IContainerNode<TState> AddStep(IStep<TState> instance);
        
        IContainerNode<TState> AddStep<T, TReturn>(Action<IBranchNode<TState, TReturn>> branch) where T : IStep<TState, TReturn>;
        IContainerNode<TState> AddStep<TReturn>(Func<TState, TReturn> perform, Action<IBranchNode<TState, TReturn>> branch);
        IContainerNode<TState> AddStep<T, TReturn>(IStep<TState, TReturn> instance, Action<IBranchNode<TState, TReturn>> branch);

        IContainerNode<TState> AddStatelessStep<T>() where T : IStatelessStep;
        IContainerNode<TState> AddStatelessStep<T, TReturn>(IStatelessStep<TReturn> instance, Action<IBranchNode<TState, TReturn>> branch);
        IContainerNode<TState> AddStatelessStep(IStatelessStep step);
        IContainerNode<TState> AddStatelessStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStatelessStep<TXReturn>;

        IContainerNode<TState> AddLevel(Action<IContainerNode<TState>> branch);
        
        IContainerNode<TState> AddWorkFlow(IWorkFlowBuilder<TState> builder);
        IContainerNode<TState> AddWorkFlow<T>() where T : IWorkFlowBuilder<TState>;

        IContainerNode<TState> Wait(int ms);

        void Retry(int maxRetries = 1);
        void Break(int levels = 1);
        void Stop();
    }
}