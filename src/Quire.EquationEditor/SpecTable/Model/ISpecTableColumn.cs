namespace Quire.EquationEditor.SpecTable.Model
{
    public partial interface ISpecTableColumn
    {
        global::Quire.EquationEditor.IEquationModel Header { get; }

        global::Quire.EquationEditor.IEquationModel ScalingFactor { get; }

        global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.IEquationModel> Data { get; }

        int CellCount { get; }

        string Rl { get; }

        string SExpression { get; }

        global::System.Xml.XmlNode EquationXmlNode { get; set; }

        int DataCellCount { get; }

        global::Quire.EquationEditor.IEquationModel ErrorCell { get; }

        bool IsExpressionValueModified { get; }

        bool IsEmpty { get; }

        string GetAllValuesScaledSExpression { get; }

        global::Quire.EquationEditor.IEquationModel GetCell(int index);

        string GetHeaderSExpression();

        string GetScaleSExpression();

        string GetDataSExpression();

        void AddCell();

        void InsertDataCell(int indexToInsert);

        void RemoveDataCell(int index);

        global::Quire.EquationEditor.IEquationModel GetDataCell(int index);

        void ClearIsExpressionValueModified();

        void HandleError(global::Quire.Calculation.Core.EngineError arg);

        void SetRestrictedReadonly(bool isInput);

        IColumnMemento CreateMemento();

        void RestoreMemento(ColumnMemento columnMemento);
    }
}
