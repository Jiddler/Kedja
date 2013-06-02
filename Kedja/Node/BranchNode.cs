using System;
using System.Collections.Generic;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class BranchNode<TReturn> : AbstractNode, IBranchNode<TReturn> {
        private readonly IStep<TReturn> _step;
        private readonly Action<IBranchNode<TReturn>> _branch;
        private readonly Dictionary<INode, Func<TReturn, bool>> _conditions = new Dictionary<INode, Func<TReturn, bool>>();
        private readonly List<INode> _nodes = new List<INode>();
 
        public BranchNode(AbstractNode parent, IStep<TReturn> step, Action<IBranchNode<TReturn>> branch) : base(parent) {
            _step = step;
            _branch = branch;
        }

        public IContainerNode When(Func<TReturn, bool> when) {
            var node = new ContainerNode(this);
            _conditions.Add(node, when);
            _nodes.Add(node);
            return node;
        }

        public IBranchNode<TReturn> AddStep<T>() where T : IStep {
            return AddStep(WorkFlowContext.TypeFactory.Create<T>());
        }

        public IBranchNode<TReturn> AddStep(IStep instance) {
            var node = new LeafNode(instance, this);
            _nodes.Add(node);
            return this;
        }

        public IBranchNode<TReturn> AddStep<T, TXReturn>(Action<IBranchNode<TXReturn>> branch) where T : IStep<TXReturn> {
            return AddStep<T, TXReturn>(WorkFlowContext.TypeFactory.Create<T>(), branch);
        }

        public IBranchNode<TReturn> AddStep<T, TXReturn>(IStep<TXReturn> instance, Action<IBranchNode<TXReturn>> branch) {
            var node = new BranchNode<TXReturn>(this, instance, branch);
            _nodes.Add(node);
            return this;
        }

        public IBranchNode<TReturn> Wait(int ms) {
            _nodes.Add(new WaitNode(this, ms));
            return this;
        }

        public override void Execute() {
            _branch(this);

            do {
                WorkFlowContext.RemoveInstructions(this);

                var result = Execute(_step);
                ExecuteSubSteps(result);
            } while(WorkFlowContext.HasInstruction<RetryInstruction>(this));

            WorkFlowContext.RemoveInstructions(this);
        }

        private void ExecuteSubSteps(TReturn result) {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            foreach(var step in _nodes) {
                if(!ShouldRunStep(result, step))
                    continue;

                step.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this))
                    break;
            }
        }

        private bool ShouldRunStep(TReturn result, INode step) {
            return !_conditions.ContainsKey(step) || _conditions[step](result);
        }
    }
}