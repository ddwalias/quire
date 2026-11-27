-- Interface-only module source: exported declarations define this module contract.
module EfiSpecial (EfiSpecial.mc_ibeta,EfiSpecial.mc_Jac,EfiSpecial.mc_Her,EfiSpecial.mc_bei,EfiSpecial.mc_ber,EfiSpecial.mc_mhyper,EfiSpecial.mc_Lag,EfiSpecial.mc_Leg,EfiSpecial.mc_Tcheb,EfiSpecial.mc_Ucheb,EfiSpecial.mc_Γ,EfiSpecial.mc_lnΓ,EfiSpecial.mc_Psi,EfiSpecial.mc_fhyper) where
{
 mc_ibeta :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Ibeta";
 mc_Jac :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Jac";
 mc_Her :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Her";
 mc_bei :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Bei";
 mc_ber :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Ber";
 mc_mhyper :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Mhyper";
 mc_Lag :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Lag";
 mc_Leg :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Leg";
 mc_Tcheb :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Tcheb";
 mc_Ucheb :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Ucheb";
 mc_Γ :: McdRun.V {}->McdRun.D {}->McdRun.V {} @ "[efispecial_]Efi.Special::Gamma";
 mc_lnΓ :: McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::LnGamma";
 mc_Psi :: McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Psi";
 mc_fhyper :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efispecial_]Efi.Special::Fhyper";
 import McdRun;
 import Mpl;
}
