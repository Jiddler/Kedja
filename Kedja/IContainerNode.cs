using System;
using Kedja.Step;

namespace Kedja {
    public interface IContainerNode {
        IContainerNode AddStep<T>() where T : IStep;
        IContainerNode AddStep(IStep instance);
        IContainerNode AddStep<T, TReturn>(Action<IBranchNode<TReturn>> branch) where T : IStep<TReturn>;
        IContainerNode AddStep<T, TReturn>(IStep<TReturn> instance, Action<IBranchNode<TReturn>> branch);
        void Retry(int maxRetries = 1);
        void Break(int levels = 1);
    }
}