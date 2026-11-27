-- Interface-only module source: exported declarations define this module contract.
module EfiFinance (EfiFinance.mc_cnper,EfiFinance.mc_crate,EfiFinance.mc_cumint,EfiFinance.mc_cumprn,EfiFinance.mc_fvc,EfiFinance.mc_ipmt,EfiFinance.mc_eff,EfiFinance.mc_fv,EfiFinance.mc_fvadj,EfiFinance.mc_pmt,EfiFinance.mc_ppmt,EfiFinance.mc_nper,EfiFinance.mc_npv,EfiFinance.mc_irr,EfiFinance.mc_mirr,EfiFinance.mc_nom,EfiFinance.mc_pv,EfiFinance.mc_rate,EfiFinance.mc_time) where
{
 mc_cnper :: McdRun.V {}->McdRun.V {a}->McdRun.V {a}->McdRun.V {} @ "[efifinance_]Efi.Finance::cnper";
 mc_crate :: McdRun.V {}->McdRun.V {a}->McdRun.V {a}->McdRun.V {} @ "[efifinance_]Efi.Finance::crate";
 mc_cumint :: McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::cumint";
 mc_cumprn :: McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.V {}->McdRun.V {}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::cumprn";
 mc_fvc :: McdRun.V {}->McdRun.V {a}->McdRun.V {a} @ "[efifinance_]Efi.Finance::fvc";
 mc_ipmt :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::ipmt";
 mc_eff :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifinance_]Efi.Finance::eff";
 mc_fv :: McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::fv";
 mc_fvadj :: McdRun.V {a}->McdRun.V {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::fvadj";
 mc_pmt :: McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::pmt";
 mc_ppmt :: McdRun.V {}->McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::ppmt";
 mc_nper :: McdRun.V {}->McdRun.V {a}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.V {} @ "[efifinance_]Efi.Finance::nper";
 mc_npv :: McdRun.V {}->McdRun.V {a}->McdRun.V {a} @ "[efifinance_]Efi.Finance::npv";
 mc_irr :: McdRun.V {a}->McdRun.D {}->McdRun.V {} @ "[efifinance_]Efi.Finance::irr";
 mc_mirr :: McdRun.V {a}->McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifinance_]Efi.Finance::mirr";
 mc_nom :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efifinance_]Efi.Finance::nom";
 mc_pv :: McdRun.V {}->McdRun.V {}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.V {a} @ "[efifinance_]Efi.Finance::pv";
 mc_rate :: McdRun.V {}->McdRun.V {a}->McdRun.V {a}->McdRun.D {a}->McdRun.D {}->McdRun.D {}->McdRun.V {} @ "[efifinance_]Efi.Finance::rate";
 mc_time :: McdRun.V {}->McdRun.V {} @ "[efifinance_]Efi.Finance::time";
 import McdRun;
 import Mpl;
}
