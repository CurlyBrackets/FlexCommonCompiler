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

            ret.Functions.Add("Program::exit", GenerateExit());
            ret.Functions.Add("Program::GetStdHandle", GenerateGetStdHandle());
            ret.Functions.Add("Program::WriteFile", GenerateWriteFile());

            ret.Constants.Add("Constant::String::0", "Hello world!\n");

            return ret;
        }

        private Function GenerateStart()
        {
            return new List<RepresentationalBase<Amd64Operation>>()
            {
                // subq rsp, 38h
                 OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Subtract,
                    OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                    OperandFactory.Instance.Immediate(OperandSize.Byte, 0x38)),

                // movq [rsp+20h], 0
                 OperationFactory<Amd64Operation>.Instance.Binary(
                   Amd64Operation.Move,
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Offset(
                            OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                            OperandFactory.Instance.Immediate(OperandSize.Byte, 0x20))),
                    OperandFactory.Instance.Immediate(OperandSize.DWord, 0)),

                // leaq rax, [rsp+20h]
                 OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.LoadAddress,
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.QWord),
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Offset(
                            OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                            OperandFactory.Instance.Immediate(OperandSize.Byte, 0x20)))),
                
                // movq [rsp+0x28], rax
                 OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Offset(
                            OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                            OperandFactory.Instance.Immediate(OperandSize.Byte, 0x28))),
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.QWord)),

                // movq rcx, rax
                 OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.QWord, 0),
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.QWord)),

                // call main
                OperationFactory<Amd64Operation>.Instance.Unary(
                    Amd64Operation.Call,
                    OperandFactory.Instance.Address(OperandSize.DWord, "Program::main")),

                // mov ecx, eax (pass return value to exit)
                 OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.DWord),
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.DWord)),

                // call exit
                OperationFactory<Amd64Operation>.Instance.Unary(
                    Amd64Operation.Call,
                    OperandFactory.Instance.Address(OperandSize.DWord, "Program::exit")),

                // add rsp,0x38
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Add,
                    OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                    OperandFactory.Instance.Immediate(OperandSize.Byte, 0x38)),

                // ret
                OperationFactory<Amd64Operation>.Instance.Nullary(
                    Amd64Operation.Return)
            };
        }

        private Function GenerateMain()
        {
            return new List<RepresentationalBase<Amd64Operation>>()
            { 
                // mov [rsp + 8], rcx
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Offset(
                            OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                            OperandFactory.Instance.Immediate(OperandSize.Byte, 0x08))),
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.QWord, 0)),

                // sub rsp, 0x48
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Subtract,
                    OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                    OperandFactory.Instance.Immediate(OperandSize.Byte, 0x48)),

                // mov ecx, -11
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.DWord, 0),
                    OperandFactory.Instance.Immediate(OperandSize.DWord, -11)),

                // call GetStdHandle
                OperationFactory<Amd64Operation>.Instance.Unary(
                    Amd64Operation.Call,
                    OperandFactory.Instance.Address(OperandSize.DWord, "Program::GetStdHandle")),

                // mov [rsp + 30h], rax
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Offset(
                            OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                            OperandFactory.Instance.Immediate(OperandSize.Byte, 0x30))),
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.QWord)),

                // mov [rsp + 20h], 0
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Offset(
                            OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                            OperandFactory.Instance.Immediate(OperandSize.Byte, 0x20))),
                    OperandFactory.Instance.Immediate(OperandSize.DWord, 0)),

                // xor r9d, r9d
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.ExclusiveOr,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.DWord, 3),
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.DWord, 3)),

                // mov r8d, 0xd
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.DWord, 2),
                    OperandFactory.Instance.Immediate(OperandSize.DWord, 0x0D)),

                // lea rdx, [string]
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.LoadAddress,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.QWord, 1),
                    OperandFactory.Instance.Memory(OperandSize.QWord,
                        OperandFactory.Instance.Address(OperandSize.DWord, "Constant::String::0"))),

                // mov rcx, rax
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Move,
                    OperandFactory.Instance.Register(OperandType.ArgumentRegister, OperandSize.QWord, 0),
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.QWord)),

                // call WriteFile
                OperationFactory<Amd64Operation>.Instance.Unary(
                    Amd64Operation.Call,
                    OperandFactory.Instance.Address(OperandSize.DWord, "Program::WriteFile")),

                // xor eax, eax
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.ExclusiveOr,
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.DWord),
                    OperandFactory.Instance.Register(OperandType.ReturnRegister, OperandSize.DWord)),

                // add rsp, 48h
                OperationFactory<Amd64Operation>.Instance.Binary(
                    Amd64Operation.Add,
                    OperandFactory.Instance.Register(OperandType.StackRegister, OperandSize.QWord),
                    OperandFactory.Instance.Immediate(OperandSize.Byte, 0x48)),

                // ret
                OperationFactory<Amd64Operation>.Instance.Nullary(Amd64Operation.Return)
            };
        }

        private Function GenerateExit()
        {
            return new List<RepresentationalBase<Amd64Operation>>()
            {
                new ExternalCallOperation<Amd64Operation>(Amd64Operation.Special | Amd64Operation.Call, "ExitProcess", "KERNEL32.dll")
            };
        }

        private Function GenerateGetStdHandle()
        {
            return new List<RepresentationalBase<Amd64Operation>>()
            {
                new ExternalCallOperation<Amd64Operation>(Amd64Operation.Special | Amd64Operation.Call, "GetStdHandle", "KERNEL32.dll")
            };
        }

        private Function GenerateWriteFile()
        {
            return new List<RepresentationalBase<Amd64Operation>>()
            {
                new ExternalCallOperation<Amd64Operation>(Amd64Operation.Special | Amd64Operation.Call, "WriteFile", "KERNEL32.dll")
            };
        }
    }
}
