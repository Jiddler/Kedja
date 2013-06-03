using System;
using System.Collections.Generic;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class BranchNode<TState, TReturn> : AbstractNode<TState>, IBranchNode<TState, TReturn> {
        private readonly Func<IStep<TState, TReturn>> _step;
        private readonly Action<IBranchNode<TState, TReturn>> _branch;
        private readonly Dictionary<INode, Func<TReturn, bool>> _conditions = new Dictionary<INode, Func<TReturn, bool>>();
        private readonly NodeCollection<TState> _nodes;
 
        public BranchNode(AbstractNode<TState> parent, Func<IStep<TState, TReturn>> step, Action<IBranchNode<TState, TReturn>> branch) : base(parent) {
            _nodes = new NodeCollection<TState>(this);
            _step = step;
            _branch = branch;
        }

        public IContainerNode<TState> When(Func<TReturn, bool> when) {
            var node = _nodes.AddContainerNode();
            _conditions.Add(node, when);
            return node;
        }

        public IBranchNode<TState, TReturn> AddStep<T>() where T : IStep<TState> {
            _nodes.AddLeafNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return this;
        }

        public IBranchNode<TState, TReturn> AddStep(Action<TState> perform) {
            _nodes.AddLeafNode(() => new DelegateStep<TState>(perform));
            return this;
        }

        public IBranchNode<TState, TReturn> AddStep(IStep<TState> instance) {
            _nodes.AddLeafNode(() => instance);
            return this;
        }

        public IBranchNode<TState, TReturn> AddStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStep<TState, TXReturn> {
            _nodes.AddBranchNode(() => WorkFlowContext.TypeFactory.Create<T>(), branch);
            return this;
        }

        public IBranchNode<TState, TReturn> AddStep<TXReturn>(Func<TState, TXReturn> perform, Action<IBranchNode<TState, TXReturn>> branch) {
            _nodes.AddBranchNode(() => new DelegateStep<TState, TXReturn>(perform), branch);
            return this;
        }

        public IBranchNode<TState, TReturn> AddStep<T, TXReturn>(IStep<TState, TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch) {
            _nodes.AddBranchNode(() => instance, branch);
            return this;
        }

        public IBranchNode<TState, TReturn> AddStatelessStep(IStatelessStep step) {
            _nodes.AddLeafNode(() => new StatelessStepWrapper<TState>(step));
            return this;
        }

        public IBranchNode<TState, TReturn> AddStatelessStep<T>() where T : IStatelessStep {
            _nodes.AddLeafNode(() => new StatelessStepWrapper<TState>(WorkFlowContext.TypeFactory.Create<T>()));
            return this;
        }

        public IBranchNode<TState, TReturn> AddStatelessStep<T, TXReturn>(Action<IBranchNode<TState, TXReturn>> branch) where T : IStatelessStep<TXReturn> {
            _nodes.AddBranchNode(() => new StatelessStepWrapper<TState, TXReturn>(WorkFlowContext.TypeFactory.Create<T>()), branch);
            return this;
        }

        public IBranchNode<TState, TReturn> AddStatelessStep<T, TXReturn>(IStatelessStep<TXReturn> instance, Action<IBranchNode<TState, TXReturn>> branch) {
            _nodes.AddBranchNode(() => new StatelessStepWrapper<TState, TXReturn>(instance), branch);
            return this;
        }

        public IBranchNode<TState, TReturn> AddWorkFlow(IWorkFlowBuilder<TState> builder) {
            _nodes.AddWorkFlowNode(() => builder);
            return this;
        }

        public IBranchNode<TState, TReturn> AddWorkFlow<T>() where T : IWorkFlowBuilder<TState>{
            _nodes.AddWorkFlowNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return this;
        }

        public IBranchNode<TState, TReturn> Wait(int ms) {
            _nodes.AddWait(ms);
            return this;
        }

        public void Stop() {
            _nodes.AddStop();
        }

        public override void Execute() {
            _branch(this);

            do {
                WorkFlowContext.RemoveInstructions(this);

                var step = _step();
                var result = ExecuteStep(step);
                ExecuteNodes(result);
            } while(WorkFlowContext.HasInstruction<RetryInstruction>(this));

            WorkFlowContext.RemoveInstructions(this);
        }

        private TReturn ExecuteStep(IStep<TState, TReturn> step) {
            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return default(TReturn);
            }

            var stepResult = step.Execute(WorkFlowContext.State);
            WorkFlowContext.Path.RemoveLast();

            return stepResult;
        }

        private void ExecuteNodes(TReturn result) {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            foreach(var node in _nodes) {
                if(!ShouldExecuteNode(result, node))
                    continue;

                node.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this))
                    break;
            }
        }

        private bool ShouldExecuteNode(TReturn result, INode step) {
            return !_conditions.ContainsKey(step) || _conditions[step](result);
        }
    }

    internal class StatelessStepWrapper<TState, TReturn> : IStep<TState, TReturn> {
        private readonly IStatelessStep<TReturn> _instance;

        public StatelessStepWrapper(IStatelessStep<TReturn> instance) {
            _instance = instance;
        }

        public TReturn Execute(TState state) {
            return _instance.Execute();
        }

        public void Cancel() {
            _instance.Cancel();
        }
    }

    internal class StatelessStepWrapper<TState> : IStep<TState> {
        private readonly IStatelessStep _instance;

        public StatelessStepWrapper(IStatelessStep instance) {
            _instance = instance;
        }

        public void Execute(TState state) {
            _instance.Execute();
        }

        public void Cancel() {
            _instance.Cancel();
        }
    }
}