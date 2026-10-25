#nullable disable
using System;
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial class MathFormatForPaste : IDisposable
{
    private static MathFormatForPaste _instance;

    public static MathFormatForPaste Instance => _instance;

    public IMathFormat DocumentMathFormat { get; private set; }

    public IDictionary<IdLabels, ExtendedFont> DocumentLabeledIdFormat { get; private set; }

    private MathFormatForPaste()
    {
    }

    public static MathFormatForPaste Generate(IMathFormatDeserializationProvider propertiesSource)
    {
        if (DebugTools.AssertNotNull(propertiesSource, null).Failed)
        {
            return _instance;
        }
        return Generate(propertiesSource.TargetMathFormat, propertiesSource.TargetLabeledIdFormat);
    }

    public static MathFormatForPaste Generate(IMathFormat targetMathFormat, IDictionary<IdLabels, ExtendedFont> targetLabeledIdFormat)
    {
        DebugTools.AssertNotNull(targetMathFormat, null);
        DebugTools.AssertNotNull(targetLabeledIdFormat, null);
        try
        {
            if (DebugTools.DevAssert(_instance == null, null).Passed)
            {
                _instance = new MathFormatForPaste
                {
                    DocumentMathFormat = targetMathFormat,
                    DocumentLabeledIdFormat = targetLabeledIdFormat
                };
            }
        }
        catch (DevAssertionException)
        {
            _instance = null;
            throw;
        }
        return _instance;
    }

    public static MathFormatForPaste SetDefault()
    {
        StaticMathFormat targetMathFormat = new StaticMathFormat();
        Dictionary<IdLabels, ExtendedFont> targetLabeledIdFormat = new Dictionary<IdLabels, ExtendedFont>();
        return Generate(targetMathFormat, targetLabeledIdFormat);
    }

    public void Dispose()
    {
        DocumentMathFormat = null;
        DocumentLabeledIdFormat = null;
        if (DebugTools.DevAssert(this == _instance, null).Failed)
        {
            return;
        }
        _instance = null;
    }
}
