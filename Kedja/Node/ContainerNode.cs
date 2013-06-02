using System;
using System.Collections.Generic;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class ContainerNode : AbstractNode, IContainerNode {
        protected readonly List<INode> _nodes = new List<INode>();

        internal ContainerNode(AbstractNode parent) : base(parent) {
        }

        internal ContainerNode(WorkFlowContext workFlowContext) : base(workFlowContext) {
        }

        public IContainerNode AddStep<T>() where T : IStep {
            return AddStep(WorkFlowContext.TypeFactory.Create<T>());
        }

        public IContainerNode AddStep(IStep instance) {
            var node = new LeafNode(instance, this);
            _nodes.Add(node);
            return this;
        }

        public IContainerNode AddStep<T, TReturn>(Action<IBranchNode<TReturn>> branch) where T : IStep<TReturn> {
            return AddStep<T, TReturn>(WorkFlowContext.TypeFactory.Create<T>(), branch);
        }

        public IContainerNode AddStep<T, TReturn>(IStep<TReturn> instance, Action<IBranchNode<TReturn>> branch) {
            var node = new BranchNode<TReturn>(this, instance, branch);
            _nodes.Add(node);
            return this;
        }

        public override void Execute() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            foreach(var step in _nodes) {
                step.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this)) {
                    break;
                }
            }

            WorkFlowContext.RemoveInstructions(this);
        }

        public void Retry(int maxRetries = 1) {
            _nodes.Add(new RetryNode(this, maxRetries));
        }

        public void Break(int levels = 1) {
            _nodes.Add(new BreakNode(this, levels));
        }

        public IContainerNode Wait(int ms) {
            _nodes.Add(new WaitNode(this, ms));
            return this;
        }
    }
}