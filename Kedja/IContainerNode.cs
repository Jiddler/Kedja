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

        IContainerNode<TState> AddLevel(Action<IContainerNode<TState>> branch);
        
        IContainerNode<TState> AddWorkFlow(IWorkFlowStep<TState> step);
        IContainerNode<TState> AddWorkFlow<T>() where T : IWorkFlowStep<TState>;

        IContainerNode<TState> Wait(int ms);

        void Retry(int maxRetries = 1);
        void Break(int levels = 1);
        void Stop();
        void Restart(int maxRestarts = -1);
    }
}