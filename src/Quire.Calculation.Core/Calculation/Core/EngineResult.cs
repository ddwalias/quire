#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial struct EngineResult
{
    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public IResultNode ResultNode { get; private set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineErrorCollection Errors { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public EngineRedefInfo[] Warnings { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public SynchronizationStatus SynchronizationStatus { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineResultMechanicalDifferentialTests")]
    public string SerializationTag { get; set; }

    public string ResultXml
    {
        get
        {
            try
            {
                return ResultNodeToXmcdTransform.Transform(ResultNode);
            }
            catch (Exception ex)
            {
                DebugTools.DevAssert(ex is OutOfMemoryException, null);
                return ResultNodeToXmcdTransform.Transform(null);
            }
        }
        set
        {
            XmcdToIResultNodeTransform xmcdToIResultNodeTransform = new XmcdToIResultNodeTransform(new SimpleResultNodeFactory());
            try
            {
                ResultNode = xmcdToIResultNodeTransform.Transform(value);
            }
            catch (Exception)
            {
                ResultNode = null;
            }
        }
    }

    public EngineResult(EngineResult result, SynchronizationStatus synchronizationStatus)
    {
        this = default;
        ResultNode = result.ResultNode ?? new EmptySimpleResultNode();
        Errors = result.Errors;
        Warnings = result.Warnings;
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = synchronizationStatus;
    }

    public EngineResult(EngineResult result, IResultNode resultNode)
    {
        this = default;
        ResultNode = resultNode ?? new EmptySimpleResultNode();
        Errors = null;
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = result.SynchronizationStatus;
    }

    public EngineResult(EngineResult result, EngineError error)
    {
        this = default;
        if (error == null)
        {
            ResultNode = result.ResultNode;
            Warnings = result.Warnings;
            Errors = null;
        }
        else
        {
            ResultNode = new EmptySimpleResultNode();
            Errors = new EngineErrorCollection { error };
            Warnings = null;
        }
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = result.SynchronizationStatus;
    }

    public EngineResult(IResultNode resultNode, EngineErrorCollection errorCollection, SynchronizationStatus synchronizationStatus)
    {
        this = default;
        if (resultNode != null && errorCollection != null && !(resultNode is IEmptyResultNode) && errorCollection.Count != 0)
        {
            throw new InvalidOperationException("logic error: An EngineResult may not have both a calculated result value and an error collection.");
        }
        ResultNode = resultNode ?? new EmptySimpleResultNode();
        Errors = errorCollection;
        Warnings = null;
        SynchronizationStatus = synchronizationStatus;
    }

    public EngineResult(IResultNode resultNode, EngineErrorCollection errorCollection, IEnumerable<EngineRedefInfo> warnings, SynchronizationStatus synchronizationStatus)
    {
        this = default;
        if (resultNode != null && errorCollection != null && !(resultNode is IEmptyResultNode) && errorCollection.Count != 0)
        {
            throw new InvalidOperationException("logic error: An EngineResult may not have both a calculated result value and an error collection.");
        }
        DebugTools.DevAssert(errorCollection.IsNullOrEmpty() || warnings.IsNullOrEmpty(), null);
        ResultNode = resultNode ?? new EmptySimpleResultNode();
        Errors = errorCollection;
        Warnings = warnings?.ToArray();
        SynchronizationStatus = synchronizationStatus;
    }

    public EngineResult(EngineResult result, IEnumerable<EngineRedefInfo> warnings)
    {
        this = default;
        ResultNode = result.ResultNode;
        Errors = result.Errors;
        Warnings = warnings?.ToArray();
        SerializationTag = result.SerializationTag;
        SynchronizationStatus = result.SynchronizationStatus;
    }

    public bool IsEmpty()
    {
        if ((ResultNode == null || ResultNode is IEmptyResultNode) && (Errors == null || Errors.Count == 0))
        {
            return Warnings.IsNullOrEmpty();
        }
        return false;
    }
}
