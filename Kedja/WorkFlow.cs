using System;
using Kedja.Node;
using Kedja.Step;

namespace Kedja {
    public class WorkFlow : IWorkFlow {
        private readonly WorkFlowContext _workFlowContext = new WorkFlowContext();
        private readonly ContainerNode _rootNode;
        
        public WorkFlow() {
            _rootNode = new ContainerNode(_workFlowContext);
        }
       
        public IWorkFlow AddStep<TStep>() where TStep : IStep {
            _rootNode.AddStep<TStep>();
            return this;
        }

        public IWorkFlow AddStep(IStep instance) {
            _rootNode.AddStep(instance);
            return this;
        }

        public IWorkFlow AddStep(Action perform) {
            _rootNode.AddStep(perform);
            return this;
        }

        public IWorkFlow AddStep<TStep, TReturn>(Action<IBranchNode<TReturn>> branch) where TStep : IStep<TReturn> {
            _rootNode.AddStep<TStep, TReturn>(branch);
            return this;
        }

        public IWorkFlow AddStep<TStep, TReturn>(IStep<TReturn> instance, Action<IBranchNode<TReturn>> branch) {
            _rootNode.AddStep<TStep, TReturn>(instance, branch);
            return this;
        }

        public IWorkFlow AddStep<TReturn>(Func<TReturn> perform, Action<IBranchNode<TReturn>> branch) {
            _rootNode.AddStep(perform, branch);
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

        public IWorkFlow Wait(int ms) {
            _rootNode.Wait(ms);
            return this;
        }

        public IWorkFlow WithTypeFactory(ITypeFactory typeFactory) {
            _workFlowContext.TypeFactory = typeFactory;
            return this;
        }
    }
}

