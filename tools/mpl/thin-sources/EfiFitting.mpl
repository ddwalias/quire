-- Authored Mpl source for the compiled EfiFitting module.
module EfiFitting (EfiFitting.mc_genfit,EfiFitting.mc_logfit,EfiFitting.mc_pwrfit,EfiFitting.mc_sinfit,EfiFitting.mc_expfit,EfiFitting.mc_lgsfit,EfiFitting.mc_ksmooth,EfiFitting.mc_line,EfiFitting.mc_loess,EfiFitting.mc_lspline,EfiFitting.mc_medfit,EfiFitting.mc_bspline,EfiFitting.mc_cspline,EfiFitting.mc_intercept,EfiFitting.mc_interp,EfiFitting.mc_linfit,EfiFitting.mc_linterp,EfiFitting.mc_lnfit,EfiFitting.mc_regress2,EfiFitting.mc_slope,EfiFitting.mc_supsmooth,EfiFitting.mc_root,EfiFitting.mc_root_bracket_,EfiFitting.mc_polyroots,EfiFitting.mc_polyroots0,EfiFitting.mc_polyroots_1,EfiFitting.mc_polyroots_2,EfiFitting.mc_medsmooth,EfiFitting.mc_pspline,EfiFitting.mc_regress) where
{
 mc_genfit :: McdRun.T->McdRun.I Mpl.I4->McdRun.V {}->McdRun.V {}->McdRun.V {}->(McdRun.V {}->a)->McdRun.V {} @ "EfiFitting.mc_genfit";
 mc_logfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::logfit";
 mc_pwrfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::pwrfit";
 mc_sinfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::sinfit";
 mc_expfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::expfit";
 mc_lgsfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::lgsfit";
 mc_ksmooth :: McdRun.V {a}->McdRun.V {b}->McdRun.V {a}->McdRun.V {b} @ "[efifitting_]Efi.Fitting::ksmooth";
 mc_line :: McdRun.V {}->McdRun.V {a}->McdRun.V {a} @ "[efifitting_]Efi.Fitting::line";
 mc_loess :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::loess";
 mc_lspline :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b a-2} @ "[efifitting_]Efi.Fitting::lspline";
 mc_medfit :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b a-1} @ "[efifitting_]Efi.Fitting::medfit";
 mc_bspline :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::bspline";
 mc_cspline :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b a-2} @ "[efifitting_]Efi.Fitting::cspline";
 mc_intercept :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b} @ "[efifitting_]Efi.Fitting::intercept";
 mc_interp :: McdRun.V {a}->McdRun.V {b}->McdRun.V {c}->McdRun.V {b}->McdRun.V {c} @ "[efifitting_]Efi.Fitting::interp";
 mc_linfit :: McdRun.V {}->McdRun.V {}->(McdRun.V {}->McdRun.V {})->McdRun.V {} @ "[efifitting_]Efi.Fitting::linfit";
 mc_linterp :: McdRun.V {a}->McdRun.V {b}->McdRun.V {a}->McdRun.V {b} @ "[efifitting_]Efi.Fitting::linterp";
 mc_lnfit :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::lnfit";
 mc_regress2 :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::regress2";
 mc_slope :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b a-1} @ "[efifitting_]Efi.Fitting::slope";
 mc_supsmooth :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b} @ "[efifitting_]Efi.Fitting::supsmooth";
 mc_root :: McdRun.T->(McdRun.V {a}->McdRun.V {b})->McdRun.V {a}->McdRun.V {a} @ "[efifitting_]Efi.Fitting::root";
 mc_root_bracket_ :: McdRun.T->(McdRun.V {a}->McdRun.V {b})->McdRun.V {a}->McdRun.V {a}->McdRun.V {a} @ "[efifitting_]Efi.Fitting::root_bracket";
 mc_polyroots :: McdRun.I Mpl.I4->McdRun.V {a}->McdRun.V {} @ "[efifitting_]Efi.Fitting::polyroots";
 mc_polyroots0 :: McdRun.V {a}->McdRun.V {} @ "[efifitting_]Efi.Fitting::polyroots_0";
 mc_polyroots_1 :: McdRun.V {a}->McdRun.V {} @ "[efifitting_]Efi.Fitting::polyroots_1";
 mc_polyroots_2 :: McdRun.V {a}->McdRun.V {} @ "[efifitting_]Efi.Fitting::polyroots_2";
 mc_medsmooth :: McdRun.V {a}->McdRun.D {}->McdRun.D {}->McdRun.V {a} @ "[efifitting_]Efi.Fitting::medsmooth";
 mc_pspline :: McdRun.V {a}->McdRun.V {b}->McdRun.V {b a-2} @ "[efifitting_]Efi.Fitting::pspline";
 mc_regress :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifitting_]Efi.Fitting::regress";
 import McdRun;
 import Mpl;
}
-- implementation
mc_genfit t selector x y z f = case selector of 0 -> tail Fitting.genfit2 t x y z f | _ -> tail Fitting.genfit t x y z f
