namespace Quire.Native.PtcEquationEditor
{
    internal partial interface b
    {
        global::Quire.EquationEditor.Tree.Navigation.NavigatorState a(global::Quire.EquationEditor.IMathNodeToVisualNodeMap A_0, global::Quire.EquationEditor.View.IVisualMathNode A_1, global::Quire.Direction A_2, global::Quire.Direction A_3, global::System.Nullable<int> A_4, double A_5, double A_6);
    }

    internal partial interface c
    {
        void b(global::Quire.EquationEditor.IMathNodeToVisualNodeMap A_0, global::Quire.EquationEditor.Tree.Navigation.NavigatorState A_1);

        void d(global::Quire.EquationEditor.IMathNodeToVisualNodeMap A_0, global::Quire.EquationEditor.TypeoverChangeEventArgs A_1);
    }

    internal partial interface h
    {
        void a(global::Quire.Delegate0 A_0);

        void b(global::Quire.Delegate0 A_0);

        void c(global::Quire.Delegate0 A_0);

        void d(global::Quire.Delegate0 A_0);

        void a();

        void b();
    }

    internal partial interface i
    {
        global::Quire.EquationEditor.View.IVisualTextNode a();

        global::Quire.EquationEditor.View.IVisualPlaceholderNode b();

        global::Quire.EquationEditor.View.IVisualOutfixNode c();

        global::Quire.EquationEditor.View.IVisualBinaryOperatorNode d();

        global::Quire.EquationEditor.View.IVisualBinaryOperatorNode e();

        global::Quire.EquationEditor.View.IVisualBinaryOperatorNode f();

        global::Quire.EquationEditor.View.IVisualBinaryOperatorNode g();

        global::Quire.EquationEditor.View.IVisualBinaryOperatorNode h();
    }
}
