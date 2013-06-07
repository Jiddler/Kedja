using System;
using System.Collections;
using System.Collections.Generic;
using Kedja.Step;

namespace Kedja.Node {
    internal class NodeCollection<TState> : IEnumerable<INode> {
        private readonly AbstractNode<TState> _node;
        private readonly List<INode> _nodes = new List<INode>();

        public NodeCollection(AbstractNode<TState> node) {
            _node = node;
        }

        private T AddNode<T>(T node) where T : INode {
            _nodes.Add(node);
            return node;            
        }

        public WaitNode<TState> Wait(int ms) {
            return AddNode(new WaitNode<TState>(_node, ms));
        }

        public LeafNode<TState> AddLeafNode(Func<IStep<TState>> step) {
            return AddNode(new LeafNode<TState>(_node, step));
        }

        public BranchNode<TState, T> AddBranchNode<T>(Func<IStep<TState, T>> func, Action<IBranchNode<TState, T>> branch) {
            return AddNode(new BranchNode<TState, T>(_node, func, branch));
        }

        public WaitNode<TState> AddWait(int ms) {
            return AddNode(new WaitNode<TState>(_node, ms));
        }

        public ContainerNode<TState> AddContainerNode() {
            return AddNode(new ContainerNode<TState>(_node));
        }

        public IContainerNode<TState> AddLevelNode(Action<IContainerNode<TState>> branch) {
            return AddNode(new LevelNode<TState>(_node, branch));
        }

        public RetryNode<TState> AddRetryNode(int maxRetries) {
            return AddNode(new RetryNode<TState>(_node, maxRetries));
        }

        public BreakNode<TState> AddBreakNode(int levels) {
            return AddNode(new BreakNode<TState>(_node, levels));
        }

        public StopNode<TState> AddStop() {
            return AddNode(new StopNode<TState>(_node));
        }

        public WorkFlowNode<TState> AddWorkFlowNode(Func<IWorkFlowBuilder<TState>> builder) {
            return AddNode(new WorkFlowNode<TState>(_node, builder));
        }

        public IEnumerator<INode> GetEnumerator() {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Clear() {
            _nodes.Clear();
        }

        public void AddRestart(AbstractNode<TState> target) {
            AddNode(new RestartNode<TState>(_node, target));
        }
    }
}