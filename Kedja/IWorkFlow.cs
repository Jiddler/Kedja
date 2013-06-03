﻿using System;
using Kedja.Step;

namespace Kedja {
    public interface IWorkFlow<out TState> {
        IWorkFlow<TState> WithTypeFactory(ITypeFactory typeFactory);

        IWorkFlow<TState> AddStep<TStep>() where TStep : IStep<TState>;
        IWorkFlow<TState> AddStep(IStep<TState> instance);
        IWorkFlow<TState> AddStep(Action<TState> perform);
        IWorkFlow<TState> AddStep<TStep, TReturn>(Action<IBranchNode<TState, TReturn>> branch) where TStep : IStep<TState, TReturn>;
        IWorkFlow<TState> AddStep<TStep, TReturn>(IStep<TState, TReturn> instance, Action<IBranchNode<TState, TReturn>> branch);
        IWorkFlow<TState> AddStep<TReturn>(Func<TState, TReturn> perform, Action<IBranchNode<TState, TReturn>> branch);
        
        IWorkFlow<TState> Wait(int ms);

        void Execute();
        void Cancel();
    }
}