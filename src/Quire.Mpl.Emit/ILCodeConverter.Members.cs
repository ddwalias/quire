using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class ILCodeConverter
{
    private Stack<TryCatchBlockInfo> _tryCatchStack = new Stack<TryCatchBlockInfo>();

    public ILCodeConverter(MetadataBuilder metadata, MetadataReferenceTracker referencesTracker, TypeBuilder typeBuilder, CommonMemberData commonMemberData)
    {
        _metadata = metadata;
        _referencesTracker = referencesTracker;
        _typeBuilder = typeBuilder;
        _codeStore = (commonMemberData.codeStore as ILGeneratorStore) ?? throw new InvalidEnumArgumentException("codeStore must be ILGeneratorStore");
        _labelManager = commonMemberData.labelManager;
        _moduleBuilder = typeBuilder.Module;
    }

    public void Convert(InstructionEncoder instructionEncoder)
    {
        foreach (ILInstruction instruction in _codeStore.Instructions)
        {
            if (instruction.CallCommand == ILCallsEncoding.eEmit)
            {
                ConvertEmit(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
            else if (instruction.CallCommand == ILCallsEncoding.eEmitCall)
            {
                ConvertEmitCall(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
            else if (instruction.CallCommand == ILCallsEncoding.eDeclareLabel)
            {
                ConvertDeclareLabel(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
            else if (instruction.CallCommand == ILCallsEncoding.eMarkLabel)
            {
                ConvertMarkLabel(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
            else if (instruction.CallCommand == ILCallsEncoding.eBeginExceptionBlock)
            {
                ConvertBeginExceptionBlock(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
            else if (instruction.CallCommand == ILCallsEncoding.eBeginCatchBlock)
            {
                ConvertBeginCatchBlock(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
            else if (instruction.CallCommand == ILCallsEncoding.eEndExceptionBlock)
            {
                ConvertEndExceptionBlock(_metadata, _typeBuilder, instruction, instructionEncoder);
            }
        }
    }

    private void ConvertEmit(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        if (inst.OpCode == OpCodes.Ldc_R8)
        {
            double value = (double)inst.Operand;
            instructionEncoder.OpCode(ILOpCode.Ldc_r8);
            instructionEncoder.CodeBuilder.WriteUInt64(BitConverter.DoubleToUInt64Bits(value));
        }
        else if (inst.OpCode == OpCodes.Ldc_I4)
        {
            int intOperand = (int)inst.Operand;
            instructionEncoder.OpCode(ILOpCode.Ldc_i4);
            instructionEncoder.CodeBuilder.WriteInt32(intOperand);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_S)
        {
            byte byteOperand = (byte)inst.Operand;
            instructionEncoder.OpCode(ILOpCode.Ldc_i4_s);
            instructionEncoder.CodeBuilder.WriteByte(byteOperand);
        }
        else if (inst.OpCode == OpCodes.Ldstr)
        {
            string stringOperand = (inst.Operand as string) ?? throw new InvalidOperationException("Operand must be a String");
            UserStringHandle orAddUserString = _metadata.GetOrAddUserString(stringOperand);
            instructionEncoder.LoadString(orAddUserString);
        }
        else if (inst.OpCode == OpCodes.Ldarg_S)
        {
            if (inst.decalaredType == typeof(byte))
            {
                if (inst.Operand == null)
                {
                    throw new ArgumentNullException("Object is null.");
                }
                byte byteOperand = (byte)inst.Operand;
                instructionEncoder.OpCode(ILOpCode.Ldarg_s);
                instructionEncoder.CodeBuilder.WriteByte(byteOperand);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        else if (inst.OpCode == OpCodes.Box)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            Type type = (inst.Operand as Type) ?? throw new InvalidOperationException("Operand must be a Type");
            TypeHandleResolver.ResolveHandleEmit(_typeBuilder, type, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.TypeDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Box);
                    instructionEncoder.Token((TypeDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.TypeReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Box);
                    instructionEncoder.Token((TypeReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a TypeDefinitionHandle or TypeReferenceHandle");
                }
            }, (Type unresolvedType) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedType.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Unbox)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            Type targetType = (inst.Operand as Type) ?? throw new InvalidOperationException("Operand must be a Type");
            TypeHandleResolver.ResolveHandleEmit(_typeBuilder, targetType, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.TypeDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Unbox);
                    instructionEncoder.Token((TypeDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.TypeReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Unbox);
                    instructionEncoder.Token((TypeReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a TypeDefinitionHandle or TypeReferenceHandle");
                }
            }, (Type unresolvedType) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedType.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Ldobj)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            Type targetType = (inst.Operand as Type) ?? throw new InvalidOperationException("Operand must be a Type");
            TypeHandleResolver.ResolveHandleEmit(_typeBuilder, targetType, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.TypeDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Ldobj);
                    instructionEncoder.Token((TypeDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.TypeReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Ldobj);
                    instructionEncoder.Token((TypeReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a TypeDefinitionHandle or TypeReferenceHandle");
                }
            }, (Type unresolvedType) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedType.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Isinst)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            Type targetType = (inst.Operand as Type) ?? throw new InvalidOperationException("Operand must be a Type");
            TypeHandleResolver.ResolveHandleEmit(_typeBuilder, targetType, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.TypeDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Isinst);
                    instructionEncoder.Token((TypeDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.TypeReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Isinst);
                    instructionEncoder.Token((TypeReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a TypeDefinitionHandle or TypeReferenceHandle");
                }
            }, (Type unresolvedType) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedType.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Newarr)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            Type targetType = (inst.Operand as Type) ?? throw new InvalidOperationException("Operand must be a Type");
            TypeHandleResolver.ResolveHandleEmit(_typeBuilder, targetType, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.TypeDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Newarr);
                    instructionEncoder.Token((TypeDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.TypeReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Newarr);
                    instructionEncoder.Token((TypeReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a TypeDefinitionHandle or TypeReferenceHandle");
                }
            }, (Type unresolvedType) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedType.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Stfld || inst.OpCode == OpCodes.Stsfld)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            ILOpCode code = ((inst.OpCode == OpCodes.Stfld) ? ILOpCode.Stfld : ILOpCode.Stsfld);
            FieldInfo field = (inst.Operand as FieldInfo) ?? throw new InvalidOperationException("Operand must be a FieldInfo");
            FieldHandleResolver.ResolveHandleEmit(_typeBuilder, field, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.FieldDefinition)
                {
                    instructionEncoder.OpCode(code);
                    instructionEncoder.Token((FieldDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.MemberReference)
                {
                    instructionEncoder.OpCode(code);
                    instructionEncoder.Token((MemberReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a FieldDefinitionHandle or MemberReferenceHandle");
                }
            }, (FieldInfo fieldInfo) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for fiedl " + fieldInfo.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Ldfld || inst.OpCode == OpCodes.Ldsfld)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            ILOpCode code = ((inst.OpCode == OpCodes.Ldfld) ? ILOpCode.Ldfld : ILOpCode.Ldsfld);
            FieldInfo fieldOperand = (inst.Operand as FieldInfo) ?? throw new InvalidOperationException("Operand must be a FieldInfo");
            FieldHandleResolver.ResolveHandleEmit(_typeBuilder, fieldOperand, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.FieldDefinition)
                {
                    instructionEncoder.OpCode(code);
                    instructionEncoder.Token((FieldDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.MemberReference)
                {
                    instructionEncoder.OpCode(code);
                    instructionEncoder.Token((MemberReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a FieldDefinitionHandle or MemberReferenceHandle");
                }
            }, (FieldInfo fieldInfo) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for fiedl " + fieldInfo.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Callvirt)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            MethodBase methodBase = (inst.Operand as MethodBase) ?? throw new InvalidOperationException("Operand type must be MethodBase");
            MethodHandleResolver.ResolveHandleEmit(typeBuilder, methodBase, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.MethodDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Callvirt);
                    instructionEncoder.Token((MethodDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.MemberReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Callvirt);
                    instructionEncoder.Token((MemberReferenceHandle)handle);
                }
                else if (handle.Kind == HandleKind.MethodSpecification)
                {
                    instructionEncoder.OpCode(ILOpCode.Callvirt);
                    instructionEncoder.Token((MethodSpecificationHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a MethodDefinitionHandle, MethodSpecificationHandle or MemberReferenceHandle");
                }
            }, (MethodBase unresolvedMethod) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedMethod.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Ldftn)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            MethodBase methodOperand = (inst.Operand as MethodBase) ?? throw new InvalidOperationException("Operand type must be MethodBase");
            MethodHandleResolver.ResolveHandleEmit(typeBuilder, methodOperand, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.MethodDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Ldftn);
                    instructionEncoder.Token((MethodDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.MemberReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Ldftn);
                    instructionEncoder.Token((MemberReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a MethodDefinitionHandle or MemberReferenceHandle");
                }
            }, (MethodBase unresolvedMethod) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedMethod.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Call)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            MethodBase methodOperand = (inst.Operand as MethodBase) ?? throw new InvalidOperationException("Operand type must be MethodBase");
            MethodHandleResolver.ResolveHandleEmit(typeBuilder, methodOperand, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.MethodDefinition)
                {
                    instructionEncoder.Call((MethodDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.MemberReference)
                {
                    instructionEncoder.Call((MemberReferenceHandle)handle);
                }
                else if (handle.Kind == HandleKind.MethodSpecification)
                {
                    instructionEncoder.Call((MethodSpecificationHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a MethodDefinitionHandle, MethodSpecificationHandle or MemberReferenceHandle");
                }
            }, (MethodBase unresolvedMethod) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedMethod.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Newobj)
        {
            if (inst.decalaredType is null)
            {
                throw new InvalidOperationException("Declared type must be not null");
            }
            MethodBase methodOperand = (inst.Operand as MethodBase) ?? throw new InvalidOperationException("Operand type must be MethodBase");
            MethodHandleResolver.ResolveHandleEmit(typeBuilder, methodOperand, (Handle handle) =>
            {
                if (handle.Kind == HandleKind.MethodDefinition)
                {
                    instructionEncoder.OpCode(ILOpCode.Newobj);
                    instructionEncoder.Token((MethodDefinitionHandle)handle);
                }
                else if (handle.Kind == HandleKind.MemberReference)
                {
                    instructionEncoder.OpCode(ILOpCode.Newobj);
                    instructionEncoder.Token((MemberReferenceHandle)handle);
                }
                else
                {
                    throw new InvalidOperationException("Operand must be a MethodDefinitionHandle or MemberReferenceHandle");
                }
            }, (MethodBase unresolvedMethod) =>
            {
                throw new InvalidOperationException("Cannot resolve the handle for method " + unresolvedMethod.Name);
            });
        }
        else if (inst.OpCode == OpCodes.Leave)
        {
            if (inst.decalaredType == typeof(Label))
            {
                if (inst.Operand == null)
                {
                    throw new ArgumentNullException("Object is null.");
                }
                instructionEncoder.Branch(ILOpCode.Leave, _labelManager.GetLabelHandle((Label)inst.Operand));
            }
            else
            {
                throw new ArgumentException("The 'Operand' must be 'Label' here");
            }
        }
        else if (inst.OpCode == OpCodes.Stloc)
        {
            LocalBuilder localBuilder = (inst.Operand as LocalBuilder) ?? throw new InvalidOperationException("Operand must be a LocalBuilder");
            instructionEncoder.StoreLocal(localBuilder.LocalIndex);
        }
        else if (inst.OpCode == OpCodes.Ldloc)
        {
            LocalBuilder localOperand = (inst.Operand as LocalBuilder) ?? throw new InvalidOperationException("Operand must be a LocalBuilder");
            instructionEncoder.LoadLocal(localOperand.LocalIndex);
        }
        else if (inst.OpCode == OpCodes.Br)
        {
            if (inst.Operand == null)
            {
                throw new InvalidOperationException("Operand must be not null");
            }
            if (inst.Operand.GetType() != typeof(Label))
            {
                throw new InvalidOperationException("Operand type must be Label");
            }
            Label label = (Label)inst.Operand;
            instructionEncoder.Branch(ILOpCode.Br, _labelManager.GetLabelHandle(label));
        }
        else if (inst.OpCode == OpCodes.Brfalse)
        {
            if (inst.Operand == null)
            {
                throw new InvalidOperationException("Operand must be not null");
            }
            if (inst.Operand.GetType() != typeof(Label))
            {
                throw new InvalidOperationException("Operand type must be Label");
            }
            Label targetLabel = (Label)inst.Operand;
            instructionEncoder.Branch(ILOpCode.Brfalse, _labelManager.GetLabelHandle(targetLabel));
        }
        else if (inst.OpCode == OpCodes.Brtrue)
        {
            if (inst.Operand == null)
            {
                throw new InvalidOperationException("Operand must be not null");
            }
            if (inst.Operand.GetType() != typeof(Label))
            {
                throw new InvalidOperationException("Operand type must be Label");
            }
            Label targetLabel = (Label)inst.Operand;
            instructionEncoder.Branch(ILOpCode.Brtrue, _labelManager.GetLabelHandle(targetLabel));
        }
        else if (inst.OpCode == OpCodes.Bne_Un)
        {
            if (inst.Operand == null)
            {
                throw new InvalidOperationException("Operand must be not null");
            }
            if (inst.Operand.GetType() != typeof(Label))
            {
                throw new InvalidOperationException("Operand type must be Label");
            }
            Label targetLabel = (Label)inst.Operand;
            instructionEncoder.Branch(ILOpCode.Bne_un, _labelManager.GetLabelHandle(targetLabel));
        }
        else if (inst.OpCode == OpCodes.Switch)
        {
            if (inst.Operand == null)
            {
                throw new InvalidOperationException("Operand must be not null");
            }
            if (inst.Operand.GetType() != typeof(Label[]))
            {
                throw new InvalidOperationException("Operand type must be Label[]");
            }
            LabelHandle[] array = ((Label[])inst.Operand).Select((Label l) => _labelManager.GetLabelHandle(l)).ToArray();
            SwitchInstructionEncoder switchInstructionEncoder = instructionEncoder.Switch(array.Length);
            LabelHandle[] labelHandles = array;
            foreach (LabelHandle switchLabel in labelHandles)
            {
                switchInstructionEncoder.Branch(switchLabel);
            }
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_0)
        {
            instructionEncoder.LoadConstantI4(0);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_1)
        {
            instructionEncoder.LoadConstantI4(1);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_2)
        {
            instructionEncoder.LoadConstantI4(2);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_3)
        {
            instructionEncoder.LoadConstantI4(3);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_4)
        {
            instructionEncoder.LoadConstantI4(4);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_5)
        {
            instructionEncoder.LoadConstantI4(5);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_6)
        {
            instructionEncoder.LoadConstantI4(6);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_7)
        {
            instructionEncoder.LoadConstantI4(7);
        }
        else if (inst.OpCode == OpCodes.Ldc_I4_8)
        {
            instructionEncoder.LoadConstantI4(8);
        }
        else if (inst.OpCode == OpCodes.Dup)
        {
            instructionEncoder.OpCode(ILOpCode.Dup);
        }
        else if (inst.OpCode == OpCodes.Ldnull)
        {
            instructionEncoder.OpCode(ILOpCode.Ldnull);
        }
        else if (inst.OpCode == OpCodes.Stelem_Ref)
        {
            instructionEncoder.OpCode(ILOpCode.Stelem_ref);
        }
        else if (inst.OpCode == OpCodes.Stelem_R8)
        {
            instructionEncoder.OpCode(ILOpCode.Stelem_r8);
        }
        else if (inst.OpCode == OpCodes.Stelem_I4)
        {
            instructionEncoder.OpCode(ILOpCode.Stelem_i4);
        }
        else if (inst.OpCode == OpCodes.Ldelem_Ref)
        {
            instructionEncoder.OpCode(ILOpCode.Ldelem_ref);
        }
        else if (inst.OpCode == OpCodes.Ret)
        {
            instructionEncoder.OpCode(ILOpCode.Ret);
        }
        else if (inst.OpCode == OpCodes.Ldarg_0)
        {
            instructionEncoder.OpCode(ILOpCode.Ldarg_0);
        }
        else if (inst.OpCode == OpCodes.Ldarg_1)
        {
            instructionEncoder.OpCode(ILOpCode.Ldarg_1);
        }
        else if (inst.OpCode == OpCodes.Ldarg_2)
        {
            instructionEncoder.OpCode(ILOpCode.Ldarg_2);
        }
        else if (inst.OpCode == OpCodes.Ldarg_3)
        {
            instructionEncoder.OpCode(ILOpCode.Ldarg_3);
        }
        else if (inst.OpCode == OpCodes.Ldlen)
        {
            instructionEncoder.OpCode(ILOpCode.Ldlen);
        }
        else if (inst.OpCode == OpCodes.Ceq)
        {
            instructionEncoder.OpCode(ILOpCode.Ceq);
        }
        else if (inst.OpCode == OpCodes.Clt)
        {
            instructionEncoder.OpCode(ILOpCode.Clt);
        }
        else if (inst.OpCode == OpCodes.Clt_Un)
        {
            instructionEncoder.OpCode(ILOpCode.Clt_un);
        }
        else if (inst.OpCode == OpCodes.Cgt)
        {
            instructionEncoder.OpCode(ILOpCode.Cgt);
        }
        else if (inst.OpCode == OpCodes.Cgt_Un)
        {
            instructionEncoder.OpCode(ILOpCode.Cgt_un);
        }
        else if (inst.OpCode == OpCodes.Pop)
        {
            instructionEncoder.OpCode(ILOpCode.Pop);
        }
        else if (inst.OpCode == OpCodes.Add)
        {
            instructionEncoder.OpCode(ILOpCode.Add);
        }
        else if (inst.OpCode == OpCodes.Sub)
        {
            instructionEncoder.OpCode(ILOpCode.Sub);
        }
        else if (inst.OpCode == OpCodes.Mul)
        {
            instructionEncoder.OpCode(ILOpCode.Mul);
        }
        else if (inst.OpCode == OpCodes.Div)
        {
            instructionEncoder.OpCode(ILOpCode.Div);
        }
        else if (inst.OpCode == OpCodes.Or)
        {
            instructionEncoder.OpCode(ILOpCode.Or);
        }
        else if (inst.OpCode == OpCodes.And)
        {
            instructionEncoder.OpCode(ILOpCode.And);
        }
        else if (inst.OpCode == OpCodes.Xor)
        {
            instructionEncoder.OpCode(ILOpCode.Xor);
        }
        else if (inst.OpCode == OpCodes.Throw)
        {
            instructionEncoder.OpCode(ILOpCode.Throw);
        }
        else if (inst.OpCode == OpCodes.Tailcall)
        {
            instructionEncoder.OpCode(ILOpCode.Tail);
        }
        else if (inst.OpCode == OpCodes.Conv_Ovf_I4)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_ovf_i4);
        }
        else if (inst.OpCode == OpCodes.Conv_R4)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_r4);
        }
        else if (inst.OpCode == OpCodes.Conv_R8)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_r8);
        }
        else if (inst.OpCode == OpCodes.Conv_I4)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_i4);
        }
        else if (inst.OpCode == OpCodes.Conv_I8)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_i8);
        }
        else if (inst.OpCode == OpCodes.Conv_U4)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_u4);
        }
        else if (inst.OpCode == OpCodes.Conv_U8)
        {
            instructionEncoder.OpCode(ILOpCode.Conv_u8);
        }
        else if (inst.OpCode == OpCodes.Neg)
        {
            instructionEncoder.OpCode(ILOpCode.Neg);
        }
        else if (inst.OpCode == OpCodes.Rem)
        {
            instructionEncoder.OpCode(ILOpCode.Rem);
        }
        else if (inst.OpCode == OpCodes.Ckfinite)
        {
            instructionEncoder.OpCode(ILOpCode.Ckfinite);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ConvertEmitCall(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        if (inst.OpCode == OpCodes.Call)
        {
            MethodInfo method = (inst.Operand as MethodInfo) ?? throw new InvalidOperationException("Operand must be a MethodInfo");
            MethodDefinitionHandle methodHandle = _moduleBuilder._definedMethods[new EntityWrappers.MethodReferenceWrapper(method)];
            instructionEncoder.Call(methodHandle);
            return;
        }
        throw new NotImplementedException();
    }

    private void ConvertDeclareLabel(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        if (inst.decalaredType == typeof(Label))
        {
            if (inst.Operand == null)
            {
                throw new ArgumentNullException("Object is null.");
            }
            Label label = (Label)inst.Operand;
            LabelHandle labelHandle = instructionEncoder.DefineLabel();
            _labelManager.AddLabelHandle(label, labelHandle);
            return;
        }
        throw new NotImplementedException();
    }

    private void ConvertMarkLabel(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        if (inst.decalaredType == typeof(Label))
        {
            if (inst.Operand == null)
            {
                throw new ArgumentNullException("Object is null.");
            }
            Label label = (Label)inst.Operand;
            LabelHandle labelHandle = _labelManager.GetLabelHandle(label);
            instructionEncoder.MarkLabel(labelHandle);
            return;
        }
        throw new NotImplementedException();
    }

    private void ConvertBeginExceptionBlock(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        TryCatchBlockInfo tryCatchBlockInfo = new TryCatchBlockInfo();
        if (inst.decalaredType == typeof(Label))
        {
            if (inst.Operand == null)
            {
                throw new ArgumentNullException("Object is null.");
            }
            Label label = (Label)inst.Operand;
            LabelHandle labelHandle = instructionEncoder.DefineLabel();
            _labelManager.AddLabelHandle(label, labelHandle);
            tryCatchBlockInfo.endExceptionBlockLabel = labelHandle;
        }
        else
        {
            throw new ArgumentException("The 'Operand' must be 'Label' here");
        }
        tryCatchBlockInfo.labelTryStart = instructionEncoder.DefineLabel();
        tryCatchBlockInfo.labelTryEnd = instructionEncoder.DefineLabel();
        tryCatchBlockInfo.labelCatchStart = instructionEncoder.DefineLabel();
        tryCatchBlockInfo.labelCatchEnd = instructionEncoder.DefineLabel();
        instructionEncoder.MarkLabel(tryCatchBlockInfo.labelTryStart);
        _tryCatchStack.Push(tryCatchBlockInfo);
    }

    private void ConvertBeginCatchBlock(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        TryCatchBlockInfo tryCatchInfo = _tryCatchStack.Peek();
        if (inst.decalaredType is null)
        {
            throw new InvalidOperationException("Declared type must be not null");
        }
        Type type = (inst.Operand as Type) ?? throw new InvalidOperationException("Operand must be a Type");
        TypeHandleResolver.ResolveHandleEmit(_typeBuilder, type, (Handle handle) =>
        {
            if (handle.Kind == HandleKind.TypeDefinition)
            {
                tryCatchInfo.exceptionToken = (TypeDefinitionHandle)handle;
            }
            else if (handle.Kind == HandleKind.TypeReference)
            {
                tryCatchInfo.exceptionToken = (TypeReferenceHandle)handle;
            }
            else
            {
                throw new InvalidOperationException("Operand must be a TypeDefinitionHandle or TypeReferenceHandle");
            }
        }, (Type unresolvedType) =>
        {
            throw new InvalidOperationException("Cannot resolve the handle for " + unresolvedType.Name);
        });
        if (tryCatchInfo.endExceptionBlockLabel.GetType() == typeof(LabelHandle))
        {
            instructionEncoder.Branch(ILOpCode.Leave, tryCatchInfo.endExceptionBlockLabel);
        }
        else
        {
            throw new ArgumentException("The 'Operand' must be 'LabelHandle' here");
        }
        instructionEncoder.MarkLabel(tryCatchInfo.labelTryEnd);
        instructionEncoder.MarkLabel(tryCatchInfo.labelCatchStart);
    }

    private void ConvertEndExceptionBlock(MetadataBuilder metadata, TypeBuilder typeBuilder, ILInstruction inst, InstructionEncoder instructionEncoder)
    {
        TryCatchBlockInfo tryCatchBlockInfo = _tryCatchStack.Pop();
        if (tryCatchBlockInfo.endExceptionBlockLabel.GetType() == typeof(LabelHandle))
        {
            LabelHandle endExceptionBlockLabel = tryCatchBlockInfo.endExceptionBlockLabel;
            instructionEncoder.Branch(ILOpCode.Leave, endExceptionBlockLabel);
            instructionEncoder.MarkLabel(tryCatchBlockInfo.labelCatchEnd);
            instructionEncoder.MarkLabel(endExceptionBlockLabel);
        }
        else
        {
            throw new ArgumentException("The 'exception block' label must be 'LabelHandle'");
        }
        (instructionEncoder.ControlFlowBuilder ?? throw new ArgumentNullException("ControlFlowBuilder must be not null")).AddCatchRegion(tryCatchBlockInfo.labelTryStart, tryCatchBlockInfo.labelTryEnd, tryCatchBlockInfo.labelCatchStart, tryCatchBlockInfo.labelCatchEnd, tryCatchBlockInfo.exceptionToken);
    }
}
