using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class MetadataHelper
{
    internal static MethodDefinitionHandle AddConstructorDefintionNoBodyWithOffset(MetadataBuilder metadata, MetadataReferenceTracker referenceTracker, TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle, ConstructorBuilder ctrBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder, int methodBodyOffset)
    {
        BlobBuilder value = SignatureHelper.MethodSignatureEnconder(ctrBuilder._parameters, ctrBuilder._returnType, !ctrBuilder.IsStatic);
        return metadata.AddMethodDefinition(ctrBuilder.Attributes, MethodImplAttributes.IL, metadata.GetOrAddString(ctrBuilder.Name), metadata.GetOrAddBlob(value), methodBodyOffset, default);
    }

    internal static int AddConstructorDefintionBody(MetadataBuilder metadata, MetadataReferenceTracker referenceTracker, TypeBuilder typeBuilder, TypeDefinitionHandle typeHandle, ConstructorBuilder ctrBuilder, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        BlobBuilder codeBuilder = new BlobBuilder();
        ControlFlowBuilder controlFlowBuilder = new ControlFlowBuilder();
        InstructionEncoder instructionEncoder = new InstructionEncoder(codeBuilder, controlFlowBuilder);
        CommonMemberData commonMemberData = CommonMemberData.FromConstructorBuilder(ctrBuilder);
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
