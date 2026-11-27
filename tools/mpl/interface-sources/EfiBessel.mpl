-- Interface-only module source: exported declarations define this module contract.
module EfiBessel (EfiBessel.mc_J1_2E_sc,EfiBessel.mc_Jn_2E_sc,EfiBessel.mc_Y0,EfiBessel.mc_Y1,EfiBessel.mc_Yn,EfiBessel.mc_In,EfiBessel.mc_I0_2E_sc,EfiBessel.mc_I1_2E_sc,EfiBessel.mc_In_2E_sc,EfiBessel.mc_J0,EfiBessel.mc_J1,EfiBessel.mc_Jn,EfiBessel.mc_J0_2E_sc,EfiBessel.mc_Y0_2E_sc,EfiBessel.mc_Y1_2E_sc,EfiBessel.mc_Yn_2E_sc,EfiBessel.mc_I0,EfiBessel.mc_I1,EfiBessel.mc_Kn_2E_sc,EfiBessel.mc_H1,EfiBessel.mc_H2,EfiBessel.mc_H1_2E_sc,EfiBessel.mc_Bi,EfiBessel.mc_DBi,EfiBessel.mc_Bi_2E_sc,EfiBessel.mc_DBi_2E_sc,EfiBessel.mc_js,EfiBessel.mc_K0,EfiBessel.mc_K1,EfiBessel.mc_Kn,EfiBessel.mc_K0_2E_sc,EfiBessel.mc_K1_2E_sc,EfiBessel.mc_H2_2E_sc,EfiBessel.mc_Ai,EfiBessel.mc_DAi,EfiBessel.mc_Ai_2E_sc,EfiBessel.mc_DAi_2E_sc,EfiBessel.mc_ys) where
{
 mc_J1_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZJ1";
 mc_Jn_2E_sc :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZJn";
 mc_Y0 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZY0";
 mc_Y1 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZY1";
 mc_Yn :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZYn";
 mc_In :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZIn";
 mc_I0_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZI0";
 mc_I1_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZI1";
 mc_In_2E_sc :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZIn";
 mc_J0 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZJ0";
 mc_J1 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZJ1";
 mc_Jn :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZJn";
 mc_J0_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZJ0";
 mc_Y0_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZY0";
 mc_Y1_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZY1";
 mc_Yn_2E_sc :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZYn";
 mc_I0 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZI0";
 mc_I1 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZI1";
 mc_Kn_2E_sc :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZKn";
 mc_H1 :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZH1";
 mc_H2 :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZH2";
 mc_H1_2E_sc :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZH1";
 mc_Bi :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZBi";
 mc_DBi :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZdBi";
 mc_Bi_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZBi";
 mc_DBi_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZdBi";
 mc_js :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::Zjs";
 mc_K0 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZK0";
 mc_K1 :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZK1";
 mc_Kn :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZKn";
 mc_K0_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZK0";
 mc_K1_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZK1";
 mc_H2_2E_sc :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZH2";
 mc_Ai :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZAi";
 mc_DAi :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::ZdAi";
 mc_Ai_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZAi";
 mc_DAi_2E_sc :: McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::EZdAi";
 mc_ys :: McdRun.V {}->McdRun.V {}->McdRun.V {} @ "[efibessel_]Efi.Bessel::Zys";
 import McdRun;
 import Mpl;
}
