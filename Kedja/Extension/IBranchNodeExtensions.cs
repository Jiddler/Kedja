using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kedja.Extension {
    public static class IBranchNodeExtensions {
        public static IContainerNode<T> When<T, TReturn>(this IBranchNode<T, TReturn> branchNode, TReturn value) {
            return branchNode.When(r => r.Equals(value));
        }

        public static IContainerNode<T> WhenNot<T, TReturn>(this IBranchNode<T, TReturn> branchNode, TReturn value) {
            return branchNode.When(r => !r.Equals(value));
        }
    }
}
