#nullable disable
using System.Collections.Generic;
using Quire;
using Quire.Matrices;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NestedMatrixFormats : SparseMatrix<MatrixFormat>
{
    [MirrorVerified("PtcControlsCoreNestedMatrixFormatsMechanicalDifferentialTests")]
    public NestedMatrixFormats() { }

    public MatrixFormatDictionary ItemMap
    {
        get
        {
            MatrixFormatDictionary matrixFormatDictionary = new MatrixFormatDictionary();
            foreach (KeyValuePair<TableCoordinate, MatrixFormat> item in _itemMap)
            {
                matrixFormatDictionary.Add(new MatrixFormatWithKey
                {
                    Key = item.Key,
                    Value = item.Value
                });
            }
            return matrixFormatDictionary;
        }
        set
        {
            foreach (MatrixFormatWithKey item in value)
            {
                _itemMap.Add(item.Key, item.Value);
            }
        }
    }
}
