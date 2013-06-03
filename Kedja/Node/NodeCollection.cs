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

        public WaitNode<TState> Wait(int ms) {
            var node = new WaitNode<TState>(_node, ms);
            _nodes.Add(node);
            return node;
        }

        public ContainerNode<TState> When(object when) {
            var node = new ContainerNode<TState>(_node);
            _nodes.Add(node);
            return node;
        }

        public LeafNode<TState> AddLeafNode(Func<IStep<TState>> step) {
            var node = new LeafNode<TState>(_node, step);
            _nodes.Add(node);
            return node;
        }

        public BranchNode<TState, T> AddBranchNode<T>(Func<IStep<TState, T>> func, Action<IBranchNode<TState, T>> branch) {
            var node = new BranchNode<TState, T>(_node, func, branch);
            _nodes.Add(node);
            return node;
        }

        public WaitNode<TState> AddWait(int ms) {
            var node = new WaitNode<TState>(_node, ms);
            _nodes.Add(node);
            return node;
        }

        public ContainerNode<TState> AddContainerNode() {
            var node = new ContainerNode<TState>(_node);
            _nodes.Add(node);
            return node;
        }

        public RetryNode<TState> AddRetryNode(int maxRetries) {
            var node = new RetryNode<TState>(_node, maxRetries);
            _nodes.Add(node);
            return node;
        }

        public BreakNode<TState> AddBreakNode(int levels) {
            var node = new BreakNode<TState>(_node, levels);
            _nodes.Add(node);
            return node;
        }

        public IEnumerator<INode> GetEnumerator() {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}