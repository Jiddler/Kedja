using System;
using System.Collections.Generic;
using System.Linq;
using Kedja.Instruction;
using Kedja.Node;
using Kedja.Step;
using Kedja.Utility;

namespace Kedja {
    internal class WorkFlowContext {
        private readonly DictionaryList<INode, IFlowInstruction> _instructions = new DictionaryList<INode,IFlowInstruction>();

        public WorkFlowContext() : this(new DefaultTypeFactory()) {
        }

        public WorkFlowContext(ITypeFactory typeFactory) {
            TypeFactory = typeFactory;
            Path = new LinkedList<ICancelableStep>();
        }

        public ITypeFactory TypeFactory { get; set; }

        public bool Canceled { get; set; }

        public LinkedList<ICancelableStep> Path { get; set; }
        
        public bool Lock(Action perform) {
            lock(this) {
                if(Canceled)
                    return false;

                perform();

                return true;
            }
        }

        public void AddInstruction(INode target, IFlowInstruction instruction) {
            _instructions.Add(target, instruction);
        }

        public T GetInstruction<T>(INode target) where T : IFlowInstruction {
            return (T)_instructions.Get(target).FirstOrDefault(i => i is T);
        }

        public bool HasInstruction<T>(INode target) where T : IFlowInstruction {
            return _instructions.Get(target).Any(i => i is T);
        }

        public void RemoveInstructions(INode node) {
            _instructions.Remove(node);
        }
    }
}