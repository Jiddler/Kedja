using System;
using Kedja.Node;
using Kedja.Step;

namespace Kedja {
    public class WorkFlow<TState> : IWorkFlow<TState> {
        private readonly WorkFlowContext<TState> _workFlowContext = new WorkFlowContext<TState>();
        private readonly ContainerNode<TState> _rootNode;
        
        public WorkFlow() {
            _rootNode = new ContainerNode<TState>(_workFlowContext);
        }

        public WorkFlow(TState state) : this() {
            _workFlowContext.State = state;
        }

        public IWorkFlow<TState> AddLevel(Action<IContainerNode<TState>> branch) {
            _rootNode.AddLevel(branch);
            return this;
        } 

        public IWorkFlow<TState> AddStep<TStep>() where TStep : IStep<TState> {
            _rootNode.AddStep<TStep>();
            return this;
        }

        public IWorkFlow<TState> AddStep(IStep<TState> instance) {
            _rootNode.AddStep(instance);
            return this;
        }

        public IWorkFlow<TState> AddStep(Action<TState> perform) {
            _rootNode.AddStep(perform);
            return this;
        }

        public IWorkFlow<TState> AddStep<TStep, TReturn>(Action<IBranchNode<TState, TReturn>> branch) where TStep : IStep<TState, TReturn> {
            _rootNode.AddStep<TStep, TReturn>(branch);
            return this;
        }

        public IWorkFlow<TState> AddStep<TStep, TReturn>(IStep<TState, TReturn> instance, Action<IBranchNode<TState, TReturn>> branch) {
            _rootNode.AddStep<TStep, TReturn>(instance, branch);
            return this;
        }

        public IWorkFlow<TState> AddStep<TReturn>(Func<TState, TReturn> perform, Action<IBranchNode<TState, TReturn>> branch) {
            _rootNode.AddStep(perform, branch);
            return this;
        }
        
        public IWorkFlow<TState> AddWorkFlow(IWorkFlowStep<TState> step) {
            _rootNode.AddWorkFlow(step);
            return this;
        }

        public IWorkFlow<TState> AddWorkFlow<T>() where T : IWorkFlowStep<TState>{
            _rootNode.AddWorkFlow<T>();
            return this;
        }

        public void Execute() {
            try {
                _rootNode.Execute();
            } catch(WorkflowCanceledException) {
            }
        }

        public void Cancel() {
            _rootNode.Cancel();
        }

        public IWorkFlow<TState> Restart() {
            _rootNode.Restart();
            return this;
        }

        public IWorkFlow<TState> Wait(int ms) {
            _rootNode.Wait(ms);
            return this;
        }

        public IWorkFlow<TState> WithTypeFactory(ITypeFactory typeFactory) {
            _workFlowContext.TypeFactory = typeFactory;
            return this;
        }
    }
}

