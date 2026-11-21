extern alias InteropMathcad;
using System;
using Quire.Automation;
using Quire.Mcd;
using IMatrixValue = InteropMathcad::Mathcad.IMatrixValue;

namespace Quire.Components;

public partial class VSValueFactory : ValueFactory
{
    public new static VSValueFactory Instance => new VSValueFactory();

    public override object AutomationToVSData(object obj)
    {
        if (!(obj is Value value))
        {
            return null;
        }
        MatVal matVal = value.Core as MatVal;
        if (matVal == null)
        {
            matVal = new MatVal(1, 1);
            matVal.set_2(0, 0, value.Core);
        }
        return new VSDataValue(matVal);
    }

    public VSValueFactory()
    {
    }
}
