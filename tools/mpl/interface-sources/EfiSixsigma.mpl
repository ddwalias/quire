-- Interface-only module source: exported declarations define this module contract.
module EfiSixsigma (EfiSixsigma.mc_fullfact,EfiSixsigma.mc_fractfact,EfiSixsigma.mc_fractresol,EfiSixsigma.mc_fractalias,EfiSixsigma.mc_boxwilson,EfiSixsigma.mc_plackettburman,EfiSixsigma.mc_boxplot,EfiSixsigma.mc_boxplotgraph,EfiSixsigma.mc_pareto,EfiSixsigma.mc_block,EfiSixsigma.mc_anova,EfiSixsigma.mc_boxbehnken,EfiSixsigma.mc_fractruns,EfiSixsigma.mc_mtaguchi,EfiSixsigma.mc_randomize,EfiSixsigma.mc_taguchi,EfiSixsigma.mc_quickscreen,EfiSixsigma.mc_effects,EfiSixsigma.mc_effectsgraph,EfiSixsigma.mc_polyfitstat,EfiSixsigma.mc_polylmfit,EfiSixsigma.mc_polyfit,EfiSixsigma.mc_polyfitc,EfiSixsigma.mc_montecarlo,EfiSixsigma.mc_doelabel,EfiSixsigma.mc_foldover,EfiSixsigma.mc_fullfact2,EfiSixsigma.mc_plotfilter,EfiSixsigma.mc_plot3d,EfiSixsigma.mc_fractfact2,EfiSixsigma.mc_boxwilson2,EfiSixsigma.mc_contour,EfiSixsigma.mc_polylmfitc,EfiSixsigma.mc_multidfit) where
{
 mc_fullfact :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fullfact";
 mc_fractfact :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fractfact";
 mc_fractresol :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fractresoln";
 mc_fractalias :: McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fractalias";
 mc_boxwilson :: McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::BoxWilson";
 mc_plackettburman :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::PlackettBurman";
 mc_boxplot :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::boxplot";
 mc_boxplotgraph :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::boxplotgraph";
 mc_pareto :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Pareto";
 mc_block :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Block";
 mc_anova :: McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Anova";
 mc_boxbehnken :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::BoxBehnken";
 mc_fractruns :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fractruns";
 mc_mtaguchi :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::MixedTaguchi";
 mc_randomize :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::randomize";
 mc_taguchi :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Taguchi";
 mc_quickscreen :: McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::QuickScreen";
 mc_effects :: McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Effects";
 mc_effectsgraph :: McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::effectsgraph";
 mc_polyfitstat :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::PolyfitStat";
 mc_polylmfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::PolyLMfit";
 mc_polyfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Polyfit";
 mc_polyfitc :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Polyfitc";
 mc_montecarlo :: (McdRun.V {}->a)->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::MonteCarlo";
 mc_doelabel :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::DoeLabel";
 mc_foldover :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::foldover";
 mc_fullfact2 :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fullfact2";
 mc_plotfilter :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efiplot_]Efi.Plot::ReduceData";
 mc_plot3d :: McdRun.O->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiplot_]Efi.Plot::plot3d";
 mc_fractfact2 :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::fractfact2";
 mc_boxwilson2 :: McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::BoxWilson2";
 mc_contour :: McdRun.O->McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efiplot_]Efi.Plot::contour";
 mc_polylmfitc :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::PolyLMfitc";
 mc_multidfit :: McdRun.T->McdRun.V {}->McdRun.V {}->McdRun.V {}->(McdRun.V {}->a)->McdRun.V {} @ "[efisixsigma_]Efi.Sixsigma::Multidfit";
 import McdRun;
 import Mpl;
}
