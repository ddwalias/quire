using System;

namespace Quire.Controls.Plots.Model;

public partial class MapperFactory
{
    public static IMapper CreateMapper(StyleType styleType, IMapper oldMapper = null)
    {
        IMapper mapper;
        if (styleType == StyleType.AlphaConstColor)
        {
            mapper = new AlphaConstColorMapper();
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return oldMapper;
        }

        if (oldMapper != null)
        {
            mapper.Color = oldMapper.Color;
            mapper.Alpha = oldMapper.Alpha;
            mapper.UseTraceInvalidStyle = oldMapper.UseTraceInvalidStyle;
        }
        return mapper;
    }

    public MapperFactory()
    {
    }
}
