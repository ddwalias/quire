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

}
