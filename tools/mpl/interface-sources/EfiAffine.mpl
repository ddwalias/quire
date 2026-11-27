-- Interface-only module source: exported declarations define this module contract.
module EfiAffine (EfiAffine.mc__2F_FIF,EfiAffine.mc_hhmmss,EfiAffine.mc__2F_hhmmss,EfiAffine.mc_DMS,EfiAffine.mc__2F_DMS,EfiAffine.mc_FIF) where
{
 mc__2F_FIF :: McdRun.V {McdStaticUnitSystem.Length 60000}->McdRun.V {} @ "[efimatrix_]Efi.Affine::FIF_inverse";
 mc_hhmmss :: McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.V {McdStaticUnitSystem.Time 60000} @ "[efimatrix_]Efi.Affine::HMS";
 mc__2F_hhmmss :: McdRun.V {McdStaticUnitSystem.Time 60000}->McdRun.V {} @ "[efimatrix_]Efi.Affine::HMS_inverse";
 mc_DMS :: McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efimatrix_]Efi.Affine::DMS";
 mc__2F_DMS :: McdRun.V {}->McdRun.V {} @ "[efimatrix_]Efi.Affine::DMS_inverse";
 mc_FIF :: McdRun.V {}->McdRun.D {}->McdRun.D {}->McdRun.V {McdStaticUnitSystem.Length 60000} @ "[efimatrix_]Efi.Affine::FIF";
 import McdRun;
 import Mpl;
 import McdStaticUnitSystem;
}
