using System;
using Kedja.Step;

namespace Kedja {
    public interface IBranchNode<out TReturn> {
        IContainerNode When(Func<TReturn, bool> when);
        
        IBranchNode<TReturn> AddStep<T>() where T : IStep;
        IBranchNode<TReturn> AddStep(IStep instance);
        IBranchNode<TReturn> AddStep(Action perform);
        IBranchNode<TReturn> AddStep<T, TXReturn>(Action<IBranchNode<TXReturn>> branch) where T : IStep<TXReturn>;
        IBranchNode<TReturn> AddStep<TXReturn>(Func<TXReturn> perform, Action<IBranchNode<TXReturn>> branch);
        IBranchNode<TReturn> AddStep<T, TXReturn>(IStep<TXReturn> instance, Action<IBranchNode<TXReturn>> branch);

        IBranchNode<TReturn> Wait(int ms);
    }
}