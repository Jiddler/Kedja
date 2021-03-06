using System;
using System.Collections.Generic;
using System.Linq;
using Kedja.Instruction;
using Kedja.Step;

namespace Kedja.Node {
    internal class BranchNode<TState, TReturn> : AbstractParentNode<TState, IBranchNode<TState, TReturn>>, IBranchNode<TState, TReturn> {
        private readonly Func<IStep<TState, TReturn>> _step;
        private readonly Action<IBranchNode<TState, TReturn>> _branch;
        private readonly Dictionary<INode, Func<TReturn, bool>> _conditions = new Dictionary<INode, Func<TReturn, bool>>();

        protected override IBranchNode<TState, TReturn> Self {
            get { return this; }
        }

        public TReturn Result { get; private set; }

        public BranchNode(AbstractNode<TState> parent, Func<IStep<TState, TReturn>> step, Action<IBranchNode<TState, TReturn>> branch) : base(parent) {
            _step = step;
            _branch = branch;
        }

        public IContainerNode<TState> When(Func<TReturn, bool> when) {
            var node = Nodes.AddContainerNode();
            _conditions.Add(node, when);
            return node;
        }

        public IContainerNode<TState> Otherwise() {
            var node = Nodes.AddContainerNode();

            var nodesWithConditions = Nodes.Intersect(_conditions.Keys).ToList();
            _conditions.Add(node, result => !nodesWithConditions.Any(n => _conditions[n](result)));
            
            return node;
        }
        
        public override void Execute() {
            _branch(this);

            do {
                WorkFlowContext.RemoveInstructions(this);

                var step = _step();
                Result = ExecuteStep(step);
                ExecuteNodes();
                
                WorkFlowContext.Path.RemoveLast();
            } while(WorkFlowContext.HasInstruction<RetryInstruction>(this) || WorkFlowContext.HasInstruction<RestartInstruction>(this));

            WorkFlowContext.RemoveInstructions(this);
        }

        private TReturn ExecuteStep(IStep<TState, TReturn> step) {
            var result = WorkFlowContext.Lock(() => WorkFlowContext.Path.AddLast(step));
            if(!result) {
                return default(TReturn);
            }

            return step.Execute(WorkFlowContext.State);
        }

        private void ExecuteNodes() {
            if(WorkFlowContext.Canceled)
                throw new WorkflowCanceledException();

            foreach(var node in Nodes) {
                if(!ShouldExecuteNode(Result, node))
                    continue;

                node.Execute();

                if(WorkFlowContext.HasInstruction<BreakInstruction>(this))
                    break;
            }
        }

        private bool ShouldExecuteNode(TReturn result, INode step) {
            Func<TReturn, bool> condition;
            return !_conditions.TryGetValue(step, out condition) || condition(result);
        }
    }
}