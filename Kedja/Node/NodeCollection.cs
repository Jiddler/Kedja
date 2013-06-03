using System;
using System.Collections;
using System.Collections.Generic;
using Kedja.Step;

namespace Kedja.Node {
    internal class NodeCollection : IEnumerable<INode> {
        private readonly AbstractNode _node;
        private readonly List<INode> _nodes = new List<INode>();

        public NodeCollection(AbstractNode node) {
            _node = node;
        }

        public WaitNode Wait(int ms) {
            var node = new WaitNode(_node, ms);
            _nodes.Add(node);
            return node;
        }

        public ContainerNode When(object when) {
            var node = new ContainerNode(_node);
            _nodes.Add(node);
            return node;
        }

        public LeafNode AddLeafNode(Func<IStep> step) {
            var node = new LeafNode(_node, step);
            _nodes.Add(node);
            return node;
        }

        public BranchNode<T> AddBranchNode<T>(Func<IStep<T>> func, Action<IBranchNode<T>> branch) {
            var node = new BranchNode<T>(_node, func, branch);
            _nodes.Add(node);
            return node;
        }

        public WaitNode AddWait(int ms) {
            var node = new WaitNode(_node, ms);
            _nodes.Add(node);
            return node;
        }

        public ContainerNode AddContainerNode() {
            var node = new ContainerNode(_node);
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