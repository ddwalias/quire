using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class MetadataHelper
{
    internal static int AddMethodDefintionBody(MetadataBuilder metadata, MetadataReferenceTracker referenceTracker, TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle, MethodBuilder methodBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        BlobBuilder codeBuilder = new BlobBuilder();
        ControlFlowBuilder controlFlowBuilder = new ControlFlowBuilder();
        InstructionEncoder instructionEncoder = new InstructionEncoder(codeBuilder, controlFlowBuilder);
        CommonMemberData commonMemberData = CommonMemberData.FromMethodBuilder(methodBuilder);
        ILCodeConverter iLCodeConverter = new ILCodeConverter(metadata, referenceTracker, typeBuilder, commonMemberData);
        int maxStack = StackDepthCalculator.CalculateMaxStackDepth(((commonMemberData.codeStore as ILGeneratorStore) ?? throw new ArgumentException("The type ILGeneratorStore should be here")).Instructions);
        int num = 0;
        if (commonMemberData.localVars.Count > 0 && !typeHandle.IsNil)
        {
            StandaloneSignatureHandle localVariablesSignature = commonMemberData.localVars.CreateSignature(metadata, typeBuilder, typeHandle);
            iLCodeConverter.Convert(instructionEncoder);
            num = methodBodyStreamEncoder.AddMethodBody(instructionEncoder, maxStack, localVariablesSignature, MethodBodyAttributes.InitLocals);
        }
        else
        {
            iLCodeConverter.Convert(instructionEncoder);
            num = methodBodyStreamEncoder.AddMethodBody(instructionEncoder, maxStack);
        }
        return num;
    }
}
