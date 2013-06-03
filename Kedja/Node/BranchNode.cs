using System;
using System.Collections.Generic;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class BranchNode<TReturn> : AbstractNode, IBranchNode<TReturn> {
        private readonly Func<IStep<TReturn>> _step;
        private readonly Action<IBranchNode<TReturn>> _branch;
        private readonly Dictionary<INode, Func<TReturn, bool>> _conditions = new Dictionary<INode, Func<TReturn, bool>>();
        private readonly NodeCollection _nodes;
 
        public BranchNode(AbstractNode parent, Func<IStep<TReturn>> step, Action<IBranchNode<TReturn>> branch) : base(parent) {
            _nodes = new NodeCollection(this);
            _step = step;
            _branch = branch;
        }

        public IContainerNode When(Func<TReturn, bool> when) {
            var node = _nodes.AddContainerNode();
            _conditions.Add(node, when);
            return node;
        }

        public IBranchNode<TReturn> AddStep<T>() where T : IStep {
            _nodes.AddLeafNode(() => WorkFlowContext.TypeFactory.Create<T>());
            return this;
        }

        public IBranchNode<TReturn> AddStep(Action perform) {
            _nodes.AddLeafNode(() => new DelegateStep(perform));
            return this;
        }

        public IBranchNode<TReturn> AddStep(IStep instance) {
            _nodes.AddLeafNode(() => instance);
            return this;
        }

        public IBranchNode<TReturn> AddStep<T, TXReturn>(Action<IBranchNode<TXReturn>> branch) where T : IStep<TXReturn> {
            _nodes.AddBranchNode(() => WorkFlowContext.TypeFactory.Create<T>(), branch);
            return this;
        }

        public IBranchNode<TReturn> AddStep<TXReturn>(Func<TXReturn> perform, Action<IBranchNode<TXReturn>> branch) {
            _nodes.AddBranchNode(() => new DelegateStep<TXReturn>(perform), branch);
            return this;
        }

        public IBranchNode<TReturn> AddStep<T, TXReturn>(IStep<TXReturn> instance, Action<IBranchNode<TXReturn>> branch) {
            _nodes.AddBranchNode(() => instance, branch);
            return this;
        }

        public IBranchNode<TReturn> Wait(int ms) {
            _nodes.AddWait(ms);
            return this;
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

        private TReturn ExecuteStep(IStep<TReturn> step) {
            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return default(TReturn);
            }

            var stepResult = step.Execute();
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
}