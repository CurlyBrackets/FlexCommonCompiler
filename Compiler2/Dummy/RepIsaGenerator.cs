using Compiler2.Compiler;
using Compiler2.Compiler.RepresentationalISA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Function = System.Collections.Generic.IList<Compiler2.Compiler.RepresentationalISA.RepresentationalBase<Compiler2.Compiler.RepresentationalISA.Amd64Operation>>;

namespace Compiler2.Dummy
{
    /*
      0000000140001000: 48 83 EC 38        sub         rsp,38h
  0000000140001004: 48 C7 44 24 20 00  mov         qword ptr [rsp+20h],0
                    00 00 00
  000000014000100D: 48 8D 44 24 20     lea         rax,[rsp+20h]
  0000000140001012: 48 89 44 24 28     mov         qword ptr [rsp+28h],rax
  0000000140001017: 48 8B 4C 24 28     mov         rcx,qword ptr [rsp+28h]
  000000014000101C: E8 0F 00 00 00     call        0000000140001030
  0000000140001021: 8B C8              mov         ecx,eax
  0000000140001023: E8 4A 00 00 00     call        0000000140001072
  0000000140001028: 48 83 C4 38        add         rsp,38h
  000000014000102C: C3                 ret
  000000014000102D: CC                 int         3
  000000014000102E: CC                 int         3
  000000014000102F: CC                 int         3
  0000000140001030: 48 89 4C 24 08     mov         qword ptr [rsp+8],rcx
  0000000140001035: 48 83 EC 48        sub         rsp,48h
  0000000140001039: B9 F5 FF FF FF     mov         ecx,0FFFFFFF5h
  000000014000103E: E8 35 00 00 00     call        0000000140001078
  0000000140001043: 48 89 44 24 30     mov         qword ptr [rsp+30h],rax
  0000000140001048: 48 C7 44 24 20 00  mov         qword ptr [rsp+20h],0
                    00 00 00
  0000000140001051: 45 33 C9           xor         r9d,r9d
  0000000140001054: 41 B8 0D 00 00 00  mov         r8d,0Dh
  000000014000105A: 48 8D 15 9F 1F 00  lea         rdx,[0000000140003000h]
                    00
  0000000140001061: 48 8B 4C 24 30     mov         rcx,qword ptr [rsp+30h]
  0000000140001066: E8 13 00 00 00     call        000000014000107E
  000000014000106B: 33 C0              xor         eax,eax
  000000014000106D: 48 83 C4 48        add         rsp,48h
  0000000140001071: C3                 ret
  0000000140001072: FF 25 88 0F 00 00  jmp         qword ptr [0000000140002000h]
  0000000140001078: FF 25 8A 0F 00 00  jmp         qword ptr [0000000140002008h]
  000000014000107E: FF 25 8C 0F 00 00  jmp         qword ptr [0000000140002010h]
    */
    class RepIsaGenerator : CompileStage<object, IntermediateProgram<Amd64Operation>>
    {
        public RepIsaGenerator(CompilerSettings settings)
            : base(settings)
        {

        }

        protected override IntermediateProgram<Amd64Operation> ProcessCore(object ignore)
        {
            var ret = new IntermediateProgram<Amd64Operation>();

            ret.Functions.Add("_start", GenerateStart());
            ret.Functions.Add("Program::main", GenerateMain());

            return ret;
        }

        private Function GenerateStart()
        {
            return new List<RepresentationalBase<Amd64Operation>>()
            {
                // subq rsp, 38h
                new BinaryOperation<Amd64Operation>(
                    Amd64Operation.LargeOperand | Amd64Operation.Subtract,
                    new RegisterOperand(OperandType.StackRegister, OperandSize.QWord),
                    new ImmediateOperand(OperandSize.Byte, 0x38)),

                // movq [rsp+20h], 0
                new BinaryOperation<Amd64Operation>(
                    Amd64Operation.LargeOperand | Amd64Operation.Move,
                    new MemoryOperand(OperandSize.QWord,
                        new OffsetOperand(
                            new RegisterOperand(OperandType.StackRegister, OperandSize.QWord),
                            new ImmediateOperand(OperandSize.Byte, 0x20))),
                    new ImmediateOperand(OperandSize.DWord, 0)),

                // leaq rax, [rsp+20h]
                new BinaryOperation<Amd64Operation>(
                    Amd64Operation.LargeOperand | Amd64Operation.LoadAddress,
                    new RegisterOperand(OperandType.ReturnRegister, OperandSize.QWord),
                    new MemoryOperand(OperandSize.QWord,
                        new OffsetOperand(
                            new RegisterOperand(OperandType.StackRegister, OperandSize.QWord),
                            new ImmediateOperand(OperandSize.Byte, 0x20)))),
                
                // movq [rsp+0x28], rax

            };
        }

        private Function GenerateMain()
        {
            return null;
        }
    }
}
