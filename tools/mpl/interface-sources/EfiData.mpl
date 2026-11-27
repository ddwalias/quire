-- Interface-only module source: exported declarations define this module contract.
module EfiData (EfiData.mc_VSmooth,EfiData.mc_Thiele,EfiData.mc_Thielecoeff,EfiData.mc_localmin,EfiData.mc_localmax,EfiData.mc_trim,EfiData.mc_markNaN,EfiData.mc_percentile,EfiData.mc_polyiter,EfiData.mc_qqplot,EfiData.mc_Rank,EfiData.mc_order,EfiData.mc_Scale,EfiData.mc_Spline2,EfiData.mc_Binterp,EfiData.mc_DWS,EfiData.mc_matchNaN,EfiData.mc_filterNaN,EfiData.mc_confidence,EfiData.mc_rationalfit,EfiData.mc_rationalfitnp,EfiData.mc_LeastSquaresFit,EfiData.mc_Grubbs,EfiData.mc_ThreeSigma,EfiData.mc_GrubbsClassic,EfiData.mc_kendltau2,EfiData.mc_polycoeff,EfiData.mc_polyint,EfiData.mc_Nipals,EfiData.mc_Nipals2,EfiData.mc_loadings,EfiData.mc_scores,EfiData.mc_PCAeigenvals,EfiData.mc_PCAvariance,EfiData.mc_contingtbl,EfiData.mc_Ftest,EfiData.mc_kendltau,EfiData.mc_rationalint,EfiData.mc_Spear,EfiData.mc_Bicubic2D) where
{
 mc_VSmooth :: McdRun.T->McdRun.V {a}->McdRun.V {}->McdRun.V {a} @ "[efidata_]Efi.Data::VSmooth";
 mc_Thiele :: McdRun.V {a}->McdRun.V {}->McdRun.V {a}->McdRun.V {} @ "[efidata_]Efi.Data::Thiele";
 mc_Thielecoeff :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::Thielecoeff";
 mc_localmin :: McdRun.N (McdRun.V {})->McdRun.V {} @ "[efidata_]Efi.Data::localmin";
 mc_localmax :: McdRun.N (McdRun.V {})->McdRun.V {} @ "[efidata_]Efi.Data::localmax";
 mc_trim :: McdRun.O->McdRun.V {a}->McdRun.V {}->McdRun.V {a} @ "[efidata_]Efi.Data::trim";
 mc_markNaN :: McdRun.O->McdRun.V {a}->McdRun.V {}->McdRun.V {a} @ "[efidata_]Efi.Data::markNaN";
 mc_percentile :: McdRun.V {a}->McdRun.V {}->McdRun.V {a} @ "[efidata_]Efi.Data::percentile";
 mc_polyiter :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::polyiter";
 mc_qqplot :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efidata_]Efi.Data::qqplot";
 mc_Rank :: McdRun.V {a}->McdRun.V {} @ "[efidata_]Efi.Data::Rank";
 mc_order :: McdRun.O->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::order";
 mc_Scale :: McdRun.V {a}->McdRun.V {a}->McdRun.V {a}->McdRun.V {a} @ "[efidata_]Efi.Data::Scale";
 mc_Spline2 :: McdRun.N (McdRun.V {})->McdRun.V {} @ "[efidata_]Efi.Data::Spline2";
 mc_Binterp :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::Binterp";
 mc_DWS :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::DWS";
 mc_matchNaN :: McdRun.O->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::matchNaN";
 mc_filterNaN :: McdRun.V {a}->McdRun.V {a} @ "[efidata_]Efi.Data::filterNaN";
 mc_confidence :: McdRun.V {}->McdRun.V {}->(McdRun.V {}->a)->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::confidence";
 mc_rationalfit :: McdRun.N (McdRun.V {})->McdRun.V {} @ "[efidata_]Efi.Data::rationalfit";
 mc_rationalfitnp :: McdRun.N (McdRun.V {})->McdRun.V {} @ "[efidata_]Efi.Data::rationalfitnp";
 mc_LeastSquaresFit :: McdRun.V {}->McdRun.V {}->(McdRun.V {}->a)->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efidata_]Efi.Data::LeastSquaresFit";
 mc_Grubbs :: McdRun.O->McdRun.V {a}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::Grubbs";
 mc_ThreeSigma :: McdRun.O->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::ThreeSigma";
 mc_GrubbsClassic :: McdRun.O->McdRun.V {a}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::GrubbsClassic";
 mc_kendltau2 :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::kendltau2";
 mc_polycoeff :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::polycoeff";
 mc_polyint :: McdRun.V {a}->McdRun.V {b}->McdRun.V {a}->McdRun.V {b} @ "[efidata_]Efi.Data::polyint";
 mc_Nipals :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::Nipals";
 mc_Nipals2 :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::Nipals2";
 mc_loadings :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::loadings";
 mc_scores :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::scores";
 mc_PCAeigenvals :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::PCAeigenvals";
 mc_PCAvariance :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::PCAvariance";
 mc_contingtbl :: McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::contingtbl";
 mc_Ftest :: McdRun.V {a}->McdRun.V {a}->McdRun.V {} @ "[efidata_]Efi.Data::Ftest";
 mc_kendltau :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::kendltau";
 mc_rationalint :: McdRun.V {a}->McdRun.V {b}->McdRun.V {a}->McdRun.V {b} @ "[efidata_]Efi.Data::rationalint";
 mc_Spear :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efidata_]Efi.Data::Spear";
 mc_Bicubic2D :: McdRun.V {a}->McdRun.V {b}->McdRun.V {c}->McdRun.V {a}->McdRun.V {b}->McdRun.V {c} @ "[efidata_]Efi.Data::Bicubic2D";
 import McdRun;
 import Mpl;
}
