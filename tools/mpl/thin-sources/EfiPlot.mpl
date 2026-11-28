-- Authored Mpl source for EfiPlot.
module EfiPlot (EfiPlot.op_eval_xy_plot,EfiPlot.op_format_xy_plot,EfiPlot.op_parametric_plot,EfiPlot.op_parametric_quickplot,EfiPlot.Trace(..),EfiPlot.op_contour,EfiPlot.op_plot3d,EfiPlot.op_range_eval,EfiPlot.validate_plot_limit,EfiPlot.maketrace,EfiPlot.vector_trace,EfiPlot.range_trace,EfiPlot.quick_trace,EfiPlot.op_scale_xy_plot,EfiPlot.op_xy_plot,EfiPlot.map_scale,EfiPlot.op_xy_plot2,EfiPlot.cplot,EfiPlot.mc_logspace,EfiPlot.mc_logpts) where
{
 op_eval_xy_plot :: Mpl.I4->McdRun.Dyn->McdRun.Dyn->McdRun.N McdRun.Dyn->McdRun.N McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::eval_xy_plot";
 op_format_xy_plot :: McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::format_xy_plot";
 op_parametric_plot :: McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::parametric_plot";
 op_parametric_quickplot :: McdRun.N McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::parametric_quickplot(System.Object[])";
 data Trace;
 op_contour :: Mpl.I4->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->Mpl.Vector McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::contour";
 op_plot3d :: Mpl.I4->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->McdRun.Dyn->Mpl.Vector McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::plot3d_";
 op_range_eval :: (a->b)->a->Mpl.Matrix b @ "[efiplot_]Efi.Plot::range_eval";
 validate_plot_limit :: McdRun.Dyn->McdRun.Dyn @ "[efiplot_]Efi.Plot::validate_plot_limit";
 maketrace :: Mpl.I4->a->(b->c)->(d->e)->Mpl.Int->Mpl.Int->EfiPlot.Trace @ "[efiplot_]Efi.Plot::maketrace";
 vector_trace :: a->(b->c)->(d->e)->Mpl.Int->Mpl.Int->EfiPlot.Trace @ "EfiPlot.vector_trace";
 range_trace :: a->(b->c)->(d->e)->Mpl.Int->Mpl.Int->EfiPlot.Trace @ "EfiPlot.range_trace";
 quick_trace :: a->(b->c)->(d->e)->Mpl.Int->Mpl.Int->EfiPlot.Trace @ "EfiPlot.quick_trace";
 op_scale_xy_plot :: Mpl.Matrix McdRun.Dyn->a->b->Mpl.Value {} @ "EfiPlot.op_scale_xy_plot";
 op_xy_plot :: Mpl.I4->McdRun.Dyn->McdRun.Dyn->a->b->McdRun.N McdRun.Dyn->McdRun.N McdRun.Dyn->Mpl.Value {} @ "EfiPlot.op_xy_plot";
 map_scale :: McdRun.Dyn->a->Mpl.Value {} @ "EfiPlot.map_scale";
 op_xy_plot2 :: Mpl.I4->McdRun.Dyn->McdRun.Dyn->a->b->McdRun.N McdRun.Dyn->McdRun.N McdRun.Dyn->Mpl.Matrix c @ "EfiPlot.op_xy_plot2";
 cplot :: McdRun.O->Mpl.I4->Mpl.I4->Mpl.I4->Mpl.I4->[Mpl.Int]->[Mpl.Int]->[EfiPlot.Trace]->[EfiPlot.Trace]->[(McdRun.Dyn,Mpl.Int)]->[(McdRun.Dyn,Mpl.Int)]->[(McdRun.Dyn,Mpl.Int)]->Mpl.Value {} @ "EfiPlot.cplot";
 mc_logspace :: McdRun.V {a}->McdRun.V {a}->McdRun.V {}->McdRun.V {} @ "[efiplot_]Efi.Plot::logspace";
 mc_logpts :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efiplot_]Efi.Plot::logpts";
 import McdRun;
 import EfiMatrix;
 import MplPrelude;
 import Mpl;
}
-- implementation
-- Authored Mpl source for plot trace mapping and XY plot assembly.
static maketrace0 : System.Object = DelegateFunction.make (delegate Plot.maketrace) 6
quick_trace = Function.Call_1 maketrace0 2
range_trace = Function.Call_1 maketrace0 1
vector_trace = Function.Call_1 maketrace0 0
static $$2h_85810701 : System.Delegate = delegate $$2h_8581070
cplot out flags xmode ymode style xgrid ygrid xvalues yvalues trace1 trace2 trace3 = tail Plot.cplot out flags xmode ymode style (i4_array (MplPrelude.vector_i4 xgrid)) (i4_array (MplPrelude.vector_i4 ygrid)) (object_array (MplPrelude.vector xvalues)) (object_array (MplPrelude.vector yvalues)) (object_array (MplPrelude.vector trace1)) (object_array (MplPrelude.vector trace2)) (object_array (MplPrelude.vector trace3))
map_scale matrix scale = let savedScale = scale in tail EfiMatrix.values.map_scalar (Function.closure $$2h_85810701 savedScale 2) matrix
$$2h_8581070 :: Mpl.Object->Mpl.Object->Mpl.Object;
$$2h_8581070 first second = tail EfiMatrix.values.op_scale second first
op_scale_xy_plot matrix xscale yscale = tail Plot.format_xy_plot (DimVal.matrix_get_1 matrix 0) (EfiMatrix.values.map_scale_inverse (DimVal.matrix_get_1 matrix 1) xscale) (EfiMatrix.values.map_scale_inverse (DimVal.matrix_get_1 matrix 2) yscale)
op_xy_plot2 flags x y xscale yscale xvalues yvalues = let result = op_scale_xy_plot (Plot.eval_xy_plot flags x y (object_array xvalues) (object_array yvalues)) xscale yscale in let yscaleValue = DimVal.matrix_get_1 result 7 in let xscaleValue = DimVal.matrix_get_1 result 4 in tail MatVal.SetShape (MatVal.PushRaw (MatVal.PushRaw (MatVal.PushRaw (MatVal.PushRaw (MatVal.PushRaw (MatVal.PushRaw (MatVal.PushRaw (MatVal.reserve 7 1) (map_scale (DimVal.matrix_get_1 result 0) xscaleValue)) (map_scale (DimVal.matrix_get_1 result 1) yscaleValue)) (map_scale (DimVal.matrix_get_1 result 2) xscaleValue)) (map_scale (DimVal.matrix_get_1 result 3) xscaleValue)) (map_scale (DimVal.matrix_get_1 result 5) yscaleValue)) (map_scale (DimVal.matrix_get_1 result 6) yscaleValue)) (DimVal.matrix_get_1 result 8))
op_xy_plot flags x y xscale yscale xvalues yvalues = tail op_scale_xy_plot (Plot.eval_xy_plot flags x y (object_array xvalues) (object_array yvalues)) xscale yscale
