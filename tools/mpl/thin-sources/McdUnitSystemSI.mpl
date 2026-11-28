-- Authored Mpl source for SI unit values and temperature conversions.
module McdUnitSystemSI (McdUnitSystemSI.op_L,McdUnitSystemSI.op_T,McdUnitSystemSI.op_Q,McdUnitSystemSI.op_K,McdUnitSystemSI.op_C,McdUnitSystemSI.op_S,McdUnitSystemSI.op_D,McdUnitSystemSI.mc_°C,McdUnitSystemSI.mc_℃,McdUnitSystemSI.mc_°F,McdUnitSystemSI.mc_℉,McdUnitSystemSI.mc__2F_°C,McdUnitSystemSI.mc__2F_℃,McdUnitSystemSI.mc__2F_°F,McdUnitSystemSI.mc__2F_℉,McdUnitSystemSI.dyn,McdUnitSystemSI.system,McdUnitSystemSI.mc_Btu,McdUnitSystemSI.mc_BTU15,McdUnitSystemSI.mc_Bq,McdUnitSystemSI.mc_C,McdUnitSystemSI.mc_CBTU,McdUnitSystemSI.mc_Δ°C,McdUnitSystemSI.mc_Δ°F,McdUnitSystemSI.mc_Δ℃,McdUnitSystemSI.mc_¤,McdUnitSystemSI.mc_μ_2E_0#CONSTANT#UNIT,McdUnitSystemSI.mc_ε_2E_0#CONSTANT#UNIT,McdUnitSystemSI.mc_μ₊0₊#CONSTANT#UNIT,McdUnitSystemSI.mc_ε₊0₊#CONSTANT#UNIT,McdUnitSystemSI.mc_A,McdUnitSystemSI.mc_Angstrom,McdUnitSystemSI.mc_BTU,McdUnitSystemSI.mc_MPa,McdUnitSystemSI.mc_R,McdUnitSystemSI.mc_°R,McdUnitSystemSI.mc_S,McdUnitSystemSI.mc_Sv,McdUnitSystemSI.mc_T,McdUnitSystemSI.mc_V,McdUnitSystemSI.mc_W,McdUnitSystemSI.mc_Wb,McdUnitSystemSI.mc_Δ℉,McdUnitSystemSI.mc_F,McdUnitSystemSI.mc_G,McdUnitSystemSI.mc_GHz,McdUnitSystemSI.mc_GPa,McdUnitSystemSI.mc_Gy,McdUnitSystemSI.mc_H,McdUnitSystemSI.mc_Hz,McdUnitSystemSI.mc_Hza,McdUnitSystemSI.mc_IBTU,McdUnitSystemSI.mc_J,McdUnitSystemSI.mc_K,McdUnitSystemSI.mc_KA,McdUnitSystemSI.mc_KHz,McdUnitSystemSI.mc_KV,McdUnitSystemSI.mc_KΩ,McdUnitSystemSI.mc_L,McdUnitSystemSI.mc_MHz,McdUnitSystemSI.mc_MΩ,McdUnitSystemSI.mc_MW,McdUnitSystemSI.mc_N,McdUnitSystemSI.mc_N·m,McdUnitSystemSI.mc_N·cm,McdUnitSystemSI.mc_N·mm,McdUnitSystemSI.mc_Oe,McdUnitSystemSI.mc_Pa,McdUnitSystemSI.mc_barn,McdUnitSystemSI.mc_bhp,McdUnitSystemSI.mc_bohr,McdUnitSystemSI.mc_a_2E_0,McdUnitSystemSI.mc_a₊0₊,McdUnitSystemSI.mc_c#CONSTANT#UNIT,McdUnitSystemSI.mc_cal,McdUnitSystemSI.mc_cal15,McdUnitSystemSI.mc_cal20,McdUnitSystemSI.mc_ps,McdUnitSystemSI.mc_acre,McdUnitSystemSI.mc_acre·ft,McdUnitSystemSI.mc_amp,McdUnitSystemSI.mc_arcsec,McdUnitSystemSI.mc_arcmin,McdUnitSystemSI.mc_atm,McdUnitSystemSI.mc_bar,McdUnitSystemSI.mc_fortnight,McdUnitSystemSI.mc_ft,McdUnitSystemSI.mc_ft·lbf,McdUnitSystemSI.mc_ft·kip,McdUnitSystemSI.mc_in·lbf,McdUnitSystemSI.mc_in·kip,McdUnitSystemSI.mc_in·ozf,McdUnitSystemSI.mc_W·s,McdUnitSystemSI.mc_W·hr,McdUnitSystemSI.mc_Ω,McdUnitSystemSI.mc_μA,McdUnitSystemSI.mc_nA,McdUnitSystemSI.mc_pA,McdUnitSystemSI.mc_μF,McdUnitSystemSI.mc_μH,McdUnitSystemSI.mc_μm,McdUnitSystemSI.mc_μs,McdUnitSystemSI.mc_ns,McdUnitSystemSI.mc_cd,McdUnitSystemSI.mc_cm,McdUnitSystemSI.mc_coul,McdUnitSystemSI.mc_mC,McdUnitSystemSI.mc_μC,McdUnitSystemSI.mc_nC,McdUnitSystemSI.mc_pC,McdUnitSystemSI.mc_cubit,McdUnitSystemSI.mc_day,McdUnitSystemSI.mc_dcal,McdUnitSystemSI.mc_deg,McdUnitSystemSI.mc_°,McdUnitSystemSI.mc_dyne,McdUnitSystemSI.mc_ehp,McdUnitSystemSI.mc_erg,McdUnitSystemSI.mc_farad,McdUnitSystemSI.mc_fl_5F_oz,McdUnitSystemSI.mc_gm,McdUnitSystemSI.mc_hectare,McdUnitSystemSI.mc_henry,McdUnitSystemSI.mc_hhp,McdUnitSystemSI.mc_hp,McdUnitSystemSI.mc_hpUK,McdUnitSystemSI.mc_hr,McdUnitSystemSI.mc_in,McdUnitSystemSI.mc_kN·m,McdUnitSystemSI.mc_kN·cm,McdUnitSystemSI.mc_kN·mm,McdUnitSystemSI.mc_kV,McdUnitSystemSI.mc_kW,McdUnitSystemSI.mc_kΩ,McdUnitSystemSI.mc_katal,McdUnitSystemSI.mc_kcal,McdUnitSystemSI.mc_kg,McdUnitSystemSI.mc_kgf,McdUnitSystemSI.mc_kip,McdUnitSystemSI.mc_klf,McdUnitSystemSI.mc_kpi,McdUnitSystemSI.mc_plf,McdUnitSystemSI.mc_km,McdUnitSystemSI.mc_knot,McdUnitSystemSI.mc_kPa,McdUnitSystemSI.mc_kW·hr,McdUnitSystemSI.mc_furlong,McdUnitSystemSI.mc_g#CONSTANT#UNIT,McdUnitSystemSI.mc_gal,McdUnitSystemSI.mc_galUK,McdUnitSystemSI.mc_gpm,McdUnitSystemSI.mc_cfm,McdUnitSystemSI.mc_mgd,McdUnitSystemSI.mc_gauss,McdUnitSystemSI.mc_in_5F_Hg,McdUnitSystemSI.mc_joule,McdUnitSystemSI.mc_kA,McdUnitSystemSI.mc_kHz,McdUnitSystemSI.mc_kN,McdUnitSystemSI.mc_MN,McdUnitSystemSI.mc_GN,McdUnitSystemSI.mc_TN,McdUnitSystemSI.mc_tonnef,McdUnitSystemSI.mc_lx,McdUnitSystemSI.mc_m,McdUnitSystemSI.mc_mA,McdUnitSystemSI.mc_mBTU,McdUnitSystemSI.mc_mH,McdUnitSystemSI.mc_mL,McdUnitSystemSI.mc_mV,McdUnitSystemSI.mc_μV,McdUnitSystemSI.mc_nV,McdUnitSystemSI.mc_mhp,McdUnitSystemSI.mc_micron,McdUnitSystemSI.mc_mi,McdUnitSystemSI.mc_mil,McdUnitSystemSI.mc_mile,McdUnitSystemSI.mc_min,McdUnitSystemSI.mc_mm,McdUnitSystemSI.mc_mol,McdUnitSystemSI.mc_mole,McdUnitSystemSI.mc_kph,McdUnitSystemSI.mc_ksf,McdUnitSystemSI.mc_ksi,McdUnitSystemSI.mc_lb,McdUnitSystemSI.mc_lbm,McdUnitSystemSI.mc_lbf,McdUnitSystemSI.mc_liter,McdUnitSystemSI.mc_l,McdUnitSystemSI.mc_lm,McdUnitSystemSI.mc_pV,McdUnitSystemSI.mc_mW,McdUnitSystemSI.mc_μW,McdUnitSystemSI.mc_nW,McdUnitSystemSI.mc_pW,McdUnitSystemSI.mc_mcal,McdUnitSystemSI.mc_mg,McdUnitSystemSI.mc_mho,McdUnitSystemSI.mc_mph,McdUnitSystemSI.mc_ms,McdUnitSystemSI.mc_newton,McdUnitSystemSI.mc_nF,McdUnitSystemSI.mc_nm,McdUnitSystemSI.mc_nmi,McdUnitSystemSI.mc_oersted,McdUnitSystemSI.mc_ohm,McdUnitSystemSI.mc_oz,McdUnitSystemSI.mc_weber,McdUnitSystemSI.mc_yd,McdUnitSystemSI.mc_yr,McdUnitSystemSI.mc_dpi,McdUnitSystemSI.mc_e_2E_c#CONSTANT,McdUnitSystemSI.mc_e₊c₊#CONSTANT,McdUnitSystemSI.mc_h#CONSTANT,McdUnitSystemSI.mc_ℏ#CONSTANT,McdUnitSystemSI.mc_rev,McdUnitSystemSI.mc_rpm,McdUnitSystemSI.mc_s,McdUnitSystemSI.mc_sec,McdUnitSystemSI.mc_siemens,McdUnitSystemSI.mc_slug,McdUnitSystemSI.mc_sr,McdUnitSystemSI.mc_stokes,McdUnitSystemSI.mc_Smoot,McdUnitSystemSI.mc_tBTU,McdUnitSystemSI.mc_tcal,McdUnitSystemSI.mc_tesla,McdUnitSystemSI.mc_ton,McdUnitSystemSI.mc_tonf,McdUnitSystemSI.mc_tonne,McdUnitSystemSI.mc_torr,McdUnitSystemSI.mc_volt,McdUnitSystemSI.mc_watt,McdUnitSystemSI.mc_ozf,McdUnitSystemSI.mc_pF,McdUnitSystemSI.mc_pcf,McdUnitSystemSI.mc_pci,McdUnitSystemSI.mc_poise,McdUnitSystemSI.mc_psi,McdUnitSystemSI.mc_psf,McdUnitSystemSI.mc_rad,McdUnitSystemSI.mc_k#CONSTANT,McdUnitSystemSI.mc_m_2E_u#CONSTANT,McdUnitSystemSI.mc_N_2E_A#CONSTANT,McdUnitSystemSI.mc_m₊u₊#CONSTANT,McdUnitSystemSI.mc_N₊A₊#CONSTANT,McdUnitSystemSI.mc_R#CONSTANT,McdUnitSystemSI.mc_R_2E_∞#CONSTANT,McdUnitSystemSI.mc_R₊∞₊#CONSTANT,McdUnitSystemSI.mc_α#CONSTANT,McdUnitSystemSI.mc_Φ_2E_0#CONSTANT#UNIT,McdUnitSystemSI.mc_Φ₊0₊#CONSTANT#UNIT,McdUnitSystemSI.mc_σ#CONSTANT,McdUnitSystemSI.op_M,McdStaticUnitSystem.Time(..),McdStaticUnitSystem.Mass(..),McdStaticUnitSystem.Substance(..),McdStaticUnitSystem.Luminosity(..),McdStaticUnitSystem.Money(..),McdStaticUnitSystem.Current(..),McdStaticUnitSystem.Temperature(..),McdStaticUnitSystem.Length(..)) where
{
 op_L :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_L";
 op_T :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_T";
 op_Q :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_Q";
 op_K :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_K";
 op_C :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_C";
 op_S :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_S";
 op_D :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_D";
 mc_°C :: Mpl.Value {} @ "McdUnitSystemSI.mc_°C";
 mc_℃ :: Mpl.Value {} @ "McdUnitSystemSI.mc_℃";
 mc_°F :: Mpl.Value {} @ "McdUnitSystemSI.mc_°F";
 mc_℉ :: Mpl.Value {} @ "McdUnitSystemSI.mc_℉";
 mc__2F_°C :: Mpl.Value {} @ "McdUnitSystemSI.mc__2F_°C";
 mc__2F_℃ :: Mpl.Value {} @ "McdUnitSystemSI.mc__2F_℃";
 mc__2F_°F :: Mpl.Value {} @ "McdUnitSystemSI.mc__2F_°F";
 mc__2F_℉ :: Mpl.Value {} @ "McdUnitSystemSI.mc__2F_℉";
 inline 10 dyn;
 dyn :: McdRun.V {d}->McdRun.Dyn @ "McdUnitSystemSI.dyn" = (\z31_->McdRun.dnum z31_ "V {d}");
 system :: McdRun.UnitSystemGroup @ "McdUnitSystemSI.system";
 mc_Btu :: Mpl.Value {} @ "McdUnitSystemSI.mc_Btu";
 mc_BTU15 :: Mpl.Value {} @ "McdUnitSystemSI.mc_BTU15";
 mc_Bq :: Mpl.Value {} @ "McdUnitSystemSI.mc_Bq";
 mc_C :: Mpl.Value {} @ "McdUnitSystemSI.mc_C";
 mc_CBTU :: Mpl.Value {} @ "McdUnitSystemSI.mc_CBTU";
 mc_Δ°C :: Mpl.Value {} @ "McdUnitSystemSI.mc_Δ°C";
 mc_Δ°F :: Mpl.Value {} @ "McdUnitSystemSI.mc_Δ°F";
 mc_Δ℃ :: Mpl.Value {} @ "McdUnitSystemSI.mc_Δ℃";
 mc_¤ :: Mpl.Value {} @ "McdUnitSystemSI.mc_¤";
 mc_μ_2E_0#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_μ_2E_0#CONSTANT#UNIT";
 mc_ε_2E_0#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_ε_2E_0#CONSTANT#UNIT";
 mc_μ₊0₊#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_μ₊0₊#CONSTANT#UNIT";
 mc_ε₊0₊#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_ε₊0₊#CONSTANT#UNIT";
 mc_A :: Mpl.Value {} @ "McdUnitSystemSI.mc_A";
 mc_Angstrom :: Mpl.Value {} @ "McdUnitSystemSI.mc_Angstrom";
 mc_BTU :: Mpl.Value {} @ "McdUnitSystemSI.mc_BTU";
 mc_MPa :: Mpl.Value {} @ "McdUnitSystemSI.mc_MPa";
 mc_R :: Mpl.Value {} @ "McdUnitSystemSI.mc_R";
 mc_°R :: Mpl.Value {} @ "McdUnitSystemSI.mc_°R";
 mc_S :: Mpl.Value {} @ "McdUnitSystemSI.mc_S";
 mc_Sv :: Mpl.Value {} @ "McdUnitSystemSI.mc_Sv";
 mc_T :: Mpl.Value {} @ "McdUnitSystemSI.mc_T";
 mc_V :: Mpl.Value {} @ "McdUnitSystemSI.mc_V";
 mc_W :: Mpl.Value {} @ "McdUnitSystemSI.mc_W";
 mc_Wb :: Mpl.Value {} @ "McdUnitSystemSI.mc_Wb";
 mc_Δ℉ :: Mpl.Value {} @ "McdUnitSystemSI.mc_Δ℉";
 mc_F :: Mpl.Value {} @ "McdUnitSystemSI.mc_F";
 mc_G :: Mpl.Value {} @ "McdUnitSystemSI.mc_G";
 mc_GHz :: Mpl.Value {} @ "McdUnitSystemSI.mc_GHz";
 mc_GPa :: Mpl.Value {} @ "McdUnitSystemSI.mc_GPa";
 mc_Gy :: Mpl.Value {} @ "McdUnitSystemSI.mc_Gy";
 mc_H :: Mpl.Value {} @ "McdUnitSystemSI.mc_H";
 mc_Hz :: Mpl.Value {} @ "McdUnitSystemSI.mc_Hz";
 mc_Hza :: Mpl.Value {} @ "McdUnitSystemSI.mc_Hza";
 mc_IBTU :: Mpl.Value {} @ "McdUnitSystemSI.mc_IBTU";
 mc_J :: Mpl.Value {} @ "McdUnitSystemSI.mc_J";
 mc_K :: Mpl.Value {} @ "McdUnitSystemSI.mc_K";
 mc_KA :: Mpl.Value {} @ "McdUnitSystemSI.mc_KA";
 mc_KHz :: Mpl.Value {} @ "McdUnitSystemSI.mc_KHz";
 mc_KV :: Mpl.Value {} @ "McdUnitSystemSI.mc_KV";
 mc_KΩ :: Mpl.Value {} @ "McdUnitSystemSI.mc_KΩ";
 mc_L :: Mpl.Value {} @ "McdUnitSystemSI.mc_L";
 mc_MHz :: Mpl.Value {} @ "McdUnitSystemSI.mc_MHz";
 mc_MΩ :: Mpl.Value {} @ "McdUnitSystemSI.mc_MΩ";
 mc_MW :: Mpl.Value {} @ "McdUnitSystemSI.mc_MW";
 mc_N :: Mpl.Value {} @ "McdUnitSystemSI.mc_N";
 mc_N·m :: Mpl.Value {} @ "McdUnitSystemSI.mc_N·m";
 mc_N·cm :: Mpl.Value {} @ "McdUnitSystemSI.mc_N·cm";
 mc_N·mm :: Mpl.Value {} @ "McdUnitSystemSI.mc_N·mm";
 mc_Oe :: Mpl.Value {} @ "McdUnitSystemSI.mc_Oe";
 mc_Pa :: Mpl.Value {} @ "McdUnitSystemSI.mc_Pa";
 mc_barn :: Mpl.Value {} @ "McdUnitSystemSI.mc_barn";
 mc_bhp :: Mpl.Value {} @ "McdUnitSystemSI.mc_bhp";
 mc_bohr :: Mpl.Value {} @ "McdUnitSystemSI.mc_bohr";
 mc_a_2E_0 :: Mpl.Value {} @ "McdUnitSystemSI.mc_a_2E_0";
 mc_a₊0₊ :: Mpl.Value {} @ "McdUnitSystemSI.mc_a₊0₊";
 mc_c#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_c#CONSTANT#UNIT";
 mc_cal :: Mpl.Value {} @ "McdUnitSystemSI.mc_cal";
 mc_cal15 :: Mpl.Value {} @ "McdUnitSystemSI.mc_cal15";
 mc_cal20 :: Mpl.Value {} @ "McdUnitSystemSI.mc_cal20";
 mc_ps :: Mpl.Value {} @ "McdUnitSystemSI.mc_ps";
 mc_acre :: Mpl.Value {} @ "McdUnitSystemSI.mc_acre";
 mc_acre·ft :: Mpl.Value {} @ "McdUnitSystemSI.mc_acre·ft";
 mc_amp :: Mpl.Value {} @ "McdUnitSystemSI.mc_amp";
 mc_arcsec :: Mpl.Value {} @ "McdUnitSystemSI.mc_arcsec";
 mc_arcmin :: Mpl.Value {} @ "McdUnitSystemSI.mc_arcmin";
 mc_atm :: Mpl.Value {} @ "McdUnitSystemSI.mc_atm";
 mc_bar :: Mpl.Value {} @ "McdUnitSystemSI.mc_bar";
 mc_fortnight :: Mpl.Value {} @ "McdUnitSystemSI.mc_fortnight";
 mc_ft :: Mpl.Value {} @ "McdUnitSystemSI.mc_ft";
 mc_ft·lbf :: Mpl.Value {} @ "McdUnitSystemSI.mc_ft·lbf";
 mc_ft·kip :: Mpl.Value {} @ "McdUnitSystemSI.mc_ft·kip";
 mc_in·lbf :: Mpl.Value {} @ "McdUnitSystemSI.mc_in·lbf";
 mc_in·kip :: Mpl.Value {} @ "McdUnitSystemSI.mc_in·kip";
 mc_in·ozf :: Mpl.Value {} @ "McdUnitSystemSI.mc_in·ozf";
 mc_W·s :: Mpl.Value {} @ "McdUnitSystemSI.mc_W·s";
 mc_W·hr :: Mpl.Value {} @ "McdUnitSystemSI.mc_W·hr";
 mc_Ω :: Mpl.Value {} @ "McdUnitSystemSI.mc_Ω";
 mc_μA :: Mpl.Value {} @ "McdUnitSystemSI.mc_μA";
 mc_nA :: Mpl.Value {} @ "McdUnitSystemSI.mc_nA";
 mc_pA :: Mpl.Value {} @ "McdUnitSystemSI.mc_pA";
 mc_μF :: Mpl.Value {} @ "McdUnitSystemSI.mc_μF";
 mc_μH :: Mpl.Value {} @ "McdUnitSystemSI.mc_μH";
 mc_μm :: Mpl.Value {} @ "McdUnitSystemSI.mc_μm";
 mc_μs :: Mpl.Value {} @ "McdUnitSystemSI.mc_μs";
 mc_ns :: Mpl.Value {} @ "McdUnitSystemSI.mc_ns";
 mc_cd :: Mpl.Value {} @ "McdUnitSystemSI.mc_cd";
 mc_cm :: Mpl.Value {} @ "McdUnitSystemSI.mc_cm";
 mc_coul :: Mpl.Value {} @ "McdUnitSystemSI.mc_coul";
 mc_mC :: Mpl.Value {} @ "McdUnitSystemSI.mc_mC";
 mc_μC :: Mpl.Value {} @ "McdUnitSystemSI.mc_μC";
 mc_nC :: Mpl.Value {} @ "McdUnitSystemSI.mc_nC";
 mc_pC :: Mpl.Value {} @ "McdUnitSystemSI.mc_pC";
 mc_cubit :: Mpl.Value {} @ "McdUnitSystemSI.mc_cubit";
 mc_day :: Mpl.Value {} @ "McdUnitSystemSI.mc_day";
 mc_dcal :: Mpl.Value {} @ "McdUnitSystemSI.mc_dcal";
 mc_deg :: Mpl.Value {} @ "McdUnitSystemSI.mc_deg";
 mc_° :: Mpl.Value {} @ "McdUnitSystemSI.mc_°";
 mc_dyne :: Mpl.Value {} @ "McdUnitSystemSI.mc_dyne";
 mc_ehp :: Mpl.Value {} @ "McdUnitSystemSI.mc_ehp";
 mc_erg :: Mpl.Value {} @ "McdUnitSystemSI.mc_erg";
 mc_farad :: Mpl.Value {} @ "McdUnitSystemSI.mc_farad";
 mc_fl_5F_oz :: Mpl.Value {} @ "McdUnitSystemSI.mc_fl_5F_oz";
 mc_gm :: Mpl.Value {} @ "McdUnitSystemSI.mc_gm";
 mc_hectare :: Mpl.Value {} @ "McdUnitSystemSI.mc_hectare";
 mc_henry :: Mpl.Value {} @ "McdUnitSystemSI.mc_henry";
 mc_hhp :: Mpl.Value {} @ "McdUnitSystemSI.mc_hhp";
 mc_hp :: Mpl.Value {} @ "McdUnitSystemSI.mc_hp";
 mc_hpUK :: Mpl.Value {} @ "McdUnitSystemSI.mc_hpUK";
 mc_hr :: Mpl.Value {} @ "McdUnitSystemSI.mc_hr";
 mc_in :: Mpl.Value {} @ "McdUnitSystemSI.mc_in";
 mc_kN·m :: Mpl.Value {} @ "McdUnitSystemSI.mc_kN·m";
 mc_kN·cm :: Mpl.Value {} @ "McdUnitSystemSI.mc_kN·cm";
 mc_kN·mm :: Mpl.Value {} @ "McdUnitSystemSI.mc_kN·mm";
 mc_kV :: Mpl.Value {} @ "McdUnitSystemSI.mc_kV";
 mc_kW :: Mpl.Value {} @ "McdUnitSystemSI.mc_kW";
 mc_kΩ :: Mpl.Value {} @ "McdUnitSystemSI.mc_kΩ";
 mc_katal :: Mpl.Value {} @ "McdUnitSystemSI.mc_katal";
 mc_kcal :: Mpl.Value {} @ "McdUnitSystemSI.mc_kcal";
 mc_kg :: Mpl.Value {} @ "McdUnitSystemSI.mc_kg";
 mc_kgf :: Mpl.Value {} @ "McdUnitSystemSI.mc_kgf";
 mc_kip :: Mpl.Value {} @ "McdUnitSystemSI.mc_kip";
 mc_klf :: Mpl.Value {} @ "McdUnitSystemSI.mc_klf";
 mc_kpi :: Mpl.Value {} @ "McdUnitSystemSI.mc_kpi";
 mc_plf :: Mpl.Value {} @ "McdUnitSystemSI.mc_plf";
 mc_km :: Mpl.Value {} @ "McdUnitSystemSI.mc_km";
 mc_knot :: Mpl.Value {} @ "McdUnitSystemSI.mc_knot";
 mc_kPa :: Mpl.Value {} @ "McdUnitSystemSI.mc_kPa";
 mc_kW·hr :: Mpl.Value {} @ "McdUnitSystemSI.mc_kW·hr";
 mc_furlong :: Mpl.Value {} @ "McdUnitSystemSI.mc_furlong";
 mc_g#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_g#CONSTANT#UNIT";
 mc_gal :: Mpl.Value {} @ "McdUnitSystemSI.mc_gal";
 mc_galUK :: Mpl.Value {} @ "McdUnitSystemSI.mc_galUK";
 mc_gpm :: Mpl.Value {} @ "McdUnitSystemSI.mc_gpm";
 mc_cfm :: Mpl.Value {} @ "McdUnitSystemSI.mc_cfm";
 mc_mgd :: Mpl.Value {} @ "McdUnitSystemSI.mc_mgd";
 mc_gauss :: Mpl.Value {} @ "McdUnitSystemSI.mc_gauss";
 mc_in_5F_Hg :: Mpl.Value {} @ "McdUnitSystemSI.mc_in_5F_Hg";
 mc_joule :: Mpl.Value {} @ "McdUnitSystemSI.mc_joule";
 mc_kA :: Mpl.Value {} @ "McdUnitSystemSI.mc_kA";
 mc_kHz :: Mpl.Value {} @ "McdUnitSystemSI.mc_kHz";
 mc_kN :: Mpl.Value {} @ "McdUnitSystemSI.mc_kN";
 mc_MN :: Mpl.Value {} @ "McdUnitSystemSI.mc_MN";
 mc_GN :: Mpl.Value {} @ "McdUnitSystemSI.mc_GN";
 mc_TN :: Mpl.Value {} @ "McdUnitSystemSI.mc_TN";
 mc_tonnef :: Mpl.Value {} @ "McdUnitSystemSI.mc_tonnef";
 mc_lx :: Mpl.Value {} @ "McdUnitSystemSI.mc_lx";
 mc_m :: Mpl.Value {} @ "McdUnitSystemSI.mc_m";
 mc_mA :: Mpl.Value {} @ "McdUnitSystemSI.mc_mA";
 mc_mBTU :: Mpl.Value {} @ "McdUnitSystemSI.mc_mBTU";
 mc_mH :: Mpl.Value {} @ "McdUnitSystemSI.mc_mH";
 mc_mL :: Mpl.Value {} @ "McdUnitSystemSI.mc_mL";
 mc_mV :: Mpl.Value {} @ "McdUnitSystemSI.mc_mV";
 mc_μV :: Mpl.Value {} @ "McdUnitSystemSI.mc_μV";
 mc_nV :: Mpl.Value {} @ "McdUnitSystemSI.mc_nV";
 mc_mhp :: Mpl.Value {} @ "McdUnitSystemSI.mc_mhp";
 mc_micron :: Mpl.Value {} @ "McdUnitSystemSI.mc_micron";
 mc_mi :: Mpl.Value {} @ "McdUnitSystemSI.mc_mi";
 mc_mil :: Mpl.Value {} @ "McdUnitSystemSI.mc_mil";
 mc_mile :: Mpl.Value {} @ "McdUnitSystemSI.mc_mile";
 mc_min :: Mpl.Value {} @ "McdUnitSystemSI.mc_min";
 mc_mm :: Mpl.Value {} @ "McdUnitSystemSI.mc_mm";
 mc_mol :: Mpl.Value {} @ "McdUnitSystemSI.mc_mol";
 mc_mole :: Mpl.Value {} @ "McdUnitSystemSI.mc_mole";
 mc_kph :: Mpl.Value {} @ "McdUnitSystemSI.mc_kph";
 mc_ksf :: Mpl.Value {} @ "McdUnitSystemSI.mc_ksf";
 mc_ksi :: Mpl.Value {} @ "McdUnitSystemSI.mc_ksi";
 mc_lb :: Mpl.Value {} @ "McdUnitSystemSI.mc_lb";
 mc_lbm :: Mpl.Value {} @ "McdUnitSystemSI.mc_lbm";
 mc_lbf :: Mpl.Value {} @ "McdUnitSystemSI.mc_lbf";
 mc_liter :: Mpl.Value {} @ "McdUnitSystemSI.mc_liter";
 mc_l :: Mpl.Value {} @ "McdUnitSystemSI.mc_l";
 mc_lm :: Mpl.Value {} @ "McdUnitSystemSI.mc_lm";
 mc_pV :: Mpl.Value {} @ "McdUnitSystemSI.mc_pV";
 mc_mW :: Mpl.Value {} @ "McdUnitSystemSI.mc_mW";
 mc_μW :: Mpl.Value {} @ "McdUnitSystemSI.mc_μW";
 mc_nW :: Mpl.Value {} @ "McdUnitSystemSI.mc_nW";
 mc_pW :: Mpl.Value {} @ "McdUnitSystemSI.mc_pW";
 mc_mcal :: Mpl.Value {} @ "McdUnitSystemSI.mc_mcal";
 mc_mg :: Mpl.Value {} @ "McdUnitSystemSI.mc_mg";
 mc_mho :: Mpl.Value {} @ "McdUnitSystemSI.mc_mho";
 mc_mph :: Mpl.Value {} @ "McdUnitSystemSI.mc_mph";
 mc_ms :: Mpl.Value {} @ "McdUnitSystemSI.mc_ms";
 mc_newton :: Mpl.Value {} @ "McdUnitSystemSI.mc_newton";
 mc_nF :: Mpl.Value {} @ "McdUnitSystemSI.mc_nF";
 mc_nm :: Mpl.Value {} @ "McdUnitSystemSI.mc_nm";
 mc_nmi :: Mpl.Value {} @ "McdUnitSystemSI.mc_nmi";
 mc_oersted :: Mpl.Value {} @ "McdUnitSystemSI.mc_oersted";
 mc_ohm :: Mpl.Value {} @ "McdUnitSystemSI.mc_ohm";
 mc_oz :: Mpl.Value {} @ "McdUnitSystemSI.mc_oz";
 mc_weber :: Mpl.Value {} @ "McdUnitSystemSI.mc_weber";
 mc_yd :: Mpl.Value {} @ "McdUnitSystemSI.mc_yd";
 mc_yr :: Mpl.Value {} @ "McdUnitSystemSI.mc_yr";
 mc_dpi :: Mpl.Value {} @ "McdUnitSystemSI.mc_dpi";
 mc_e_2E_c#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_e_2E_c#CONSTANT";
 mc_e₊c₊#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_e₊c₊#CONSTANT";
 mc_h#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_h#CONSTANT";
 mc_ℏ#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_ℏ#CONSTANT";
 mc_rev :: Mpl.Value {} @ "McdUnitSystemSI.mc_rev";
 mc_rpm :: Mpl.Value {} @ "McdUnitSystemSI.mc_rpm";
 mc_s :: Mpl.Value {} @ "McdUnitSystemSI.mc_s";
 mc_sec :: Mpl.Value {} @ "McdUnitSystemSI.mc_sec";
 mc_siemens :: Mpl.Value {} @ "McdUnitSystemSI.mc_siemens";
 mc_slug :: Mpl.Value {} @ "McdUnitSystemSI.mc_slug";
 mc_sr :: Mpl.Value {} @ "McdUnitSystemSI.mc_sr";
 mc_stokes :: Mpl.Value {} @ "McdUnitSystemSI.mc_stokes";
 mc_Smoot :: Mpl.Value {} @ "McdUnitSystemSI.mc_Smoot";
 mc_tBTU :: Mpl.Value {} @ "McdUnitSystemSI.mc_tBTU";
 mc_tcal :: Mpl.Value {} @ "McdUnitSystemSI.mc_tcal";
 mc_tesla :: Mpl.Value {} @ "McdUnitSystemSI.mc_tesla";
 mc_ton :: Mpl.Value {} @ "McdUnitSystemSI.mc_ton";
 mc_tonf :: Mpl.Value {} @ "McdUnitSystemSI.mc_tonf";
 mc_tonne :: Mpl.Value {} @ "McdUnitSystemSI.mc_tonne";
 mc_torr :: Mpl.Value {} @ "McdUnitSystemSI.mc_torr";
 mc_volt :: Mpl.Value {} @ "McdUnitSystemSI.mc_volt";
 mc_watt :: Mpl.Value {} @ "McdUnitSystemSI.mc_watt";
 mc_ozf :: Mpl.Value {} @ "McdUnitSystemSI.mc_ozf";
 mc_pF :: Mpl.Value {} @ "McdUnitSystemSI.mc_pF";
 mc_pcf :: Mpl.Value {} @ "McdUnitSystemSI.mc_pcf";
 mc_pci :: Mpl.Value {} @ "McdUnitSystemSI.mc_pci";
 mc_poise :: Mpl.Value {} @ "McdUnitSystemSI.mc_poise";
 mc_psi :: Mpl.Value {} @ "McdUnitSystemSI.mc_psi";
 mc_psf :: Mpl.Value {} @ "McdUnitSystemSI.mc_psf";
 mc_rad :: Mpl.Value {} @ "McdUnitSystemSI.mc_rad";
 mc_k#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_k#CONSTANT";
 mc_m_2E_u#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_m_2E_u#CONSTANT";
 mc_N_2E_A#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_N_2E_A#CONSTANT";
 mc_m₊u₊#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_m₊u₊#CONSTANT";
 mc_N₊A₊#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_N₊A₊#CONSTANT";
 mc_R#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_R#CONSTANT";
 mc_R_2E_∞#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_R_2E_∞#CONSTANT";
 mc_R₊∞₊#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_R₊∞₊#CONSTANT";
 mc_α#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_α#CONSTANT";
 mc_Φ_2E_0#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_Φ_2E_0#CONSTANT#UNIT";
 mc_Φ₊0₊#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemSI.mc_Φ₊0₊#CONSTANT#UNIT";
 mc_σ#CONSTANT :: Mpl.Value {} @ "McdUnitSystemSI.mc_σ#CONSTANT";
 op_M :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemSI.op_M";
 import McdStaticUnitSystemSI;
 import McdRun;
 import Mpl;
 import MplPrelude;
 import McdStaticUnitSystem;
}
-- implementation

field mc_σ#CONSTANT
field mc_Φ₊0₊#CONSTANT#UNIT
field mc_Φ_2E_0#CONSTANT#UNIT
field mc_α#CONSTANT
field mc_R₊∞₊#CONSTANT
field mc_R_2E_∞#CONSTANT
field mc_R#CONSTANT
field mc_N₊A₊#CONSTANT
field mc_m₊u₊#CONSTANT
field mc_N_2E_A#CONSTANT
field mc_m_2E_u#CONSTANT
field mc_k#CONSTANT
field mc_ℏ#CONSTANT
field mc_h#CONSTANT
field mc_e₊c₊#CONSTANT
field mc_e_2E_c#CONSTANT
field mc_dpi
field mc_yr
field mc_yd
field mc_weber
field mc_watt
field mc_volt
field mc_torr
field mc_tonne
field mc_tonf
field mc_ton
field mc_tesla
field mc_tcal
field mc_tBTU
field mc_Smoot
field mc_stokes
field mc_sr
field mc_slug
field mc_siemens
field mc_s
field mc_rpm
field mc_rev
field mc_rad
field mc_psf
field mc_psi
field mc_poise
field mc_pci
field mc_pcf
field mc_pF
field mc_ozf
field mc_oz
field mc_ohm
field mc_oersted
field mc_nmi
field mc_nm
field mc_nF
field mc_newton
field mc_ms
field mc_mph
field mc_mol
field mc_mm
field mc_min
field mc_mile
field mc_mil
field mc_mi
field mc_micron
field mc_mhp
field mc_mho
field mc_mg
field mc_mcal
field mc_pW
field mc_nW
field mc_μW
field mc_mW
field mc_pV
field mc_nV
field mc_μV
field mc_mV
field mc_mL
field mc_mH
field mc_mBTU
field mc_mA
field mc_lx
field mc_lm
field mc_l
field mc_liter
field mc_lbf
field mc_lbm
field mc_lb
field mc_ksi
field mc_ksf
field mc_kph
field mc_kPa
field mc_knot
field mc_km
field mc_plf
field mc_kpi
field mc_klf
field mc_kip
field mc_kgf
field mc_kcal
field mc_katal
field mc_kΩ
field mc_kW
field mc_kV
field mc_kN·mm
field mc_kN·cm
field mc_kN·m
field mc_tonnef
field mc_TN
field mc_GN
field mc_MN
field mc_kN
field mc_kHz
field mc_kA
field mc_joule
field mc_in_5F_Hg
field mc_in
field mc_hr
field mc_hpUK
field mc_hp
field mc_hhp
field mc_henry
field mc_hectare
field mc_gm
field mc_gauss
field mc_mgd
field mc_cfm
field mc_gpm
field mc_galUK
field mc_gal
field mc_g#CONSTANT#UNIT
field mc_furlong
field mc_kW·hr
field mc_W·hr
field mc_W·s
field mc_in·ozf
field mc_in·kip
field mc_in·lbf
field mc_ft·kip
field mc_ft·lbf
field mc_ft
field mc_fortnight
field mc_fl_5F_oz
field mc_farad
field mc_erg
field mc_ehp
field mc_dyne
field mc_°
field mc_deg
field mc_dcal
field mc_day
field mc_cubit
field mc_pC
field mc_nC
field mc_μC
field mc_mC
field mc_coul
field mc_cm
field mc_cal20
field mc_cal15
field mc_cal
field mc_c#CONSTANT#UNIT
field mc_a₊0₊
field mc_a_2E_0
field mc_bohr
field mc_bhp
field mc_barn
field mc_bar
field mc_atm
field mc_arcmin
field mc_arcsec
field mc_acre·ft
field mc_acre
field mc_ps
field mc_ns
field mc_μs
field mc_μm
field mc_μH
field mc_μF
field mc_pA
field mc_nA
field mc_μA
field mc_Ω
field mc_Wb
field mc_W
field mc_V
field mc_T
field mc_Sv
field mc_S
field mc_°R
field mc_R
field mc_MPa
field mc_Pa
field mc_Oe
field mc_N·mm
field mc_N·cm
field mc_N·m
field mc_N
field mc_MW
field mc_MΩ
field mc_MHz
field mc_L
field mc_KΩ
field mc_KV
field mc_KHz
field mc_KA
field mc_J
field mc_IBTU
field mc_Hza
field mc_Hz
field mc_H
field mc_Gy
field mc_GPa
field mc_GHz
field mc_G
field mc_F
field mc_Δ℉
field mc_Δ℃
field mc_Δ°F
field mc_Δ°C
field mc_CBTU
field mc_C
field mc_Bq
field mc_BTU15
field mc_Btu
field mc_BTU
field mc_Angstrom
field mc_A
field mc_ε₊0₊#CONSTANT#UNIT
field mc_μ₊0₊#CONSTANT#UNIT
field mc_ε_2E_0#CONSTANT#UNIT
field mc_μ_2E_0#CONSTANT#UNIT
field mc_K
field $°F_846a964
field $°c_846ad88
field $°f_846b06c
field $$4m_84fe9b40
field $$4n_84fea701
field $kf_84665dc
field $fk_84663d4
field $$4k_84fb7b02
field $$4l_84fe8983
field $kc_8465840
field $ck_8469634
field mc__2F_℉
field mc__2F_°F
field mc__2F_℃
field mc__2F_°C
field mc_℉
field mc_°F
field mc_℃
field mc_°C
field mc_¤
field mc_mole
field mc_cd
field mc_amp
field mc_sec
field mc_m
field mc_kg
field system
static $$4m_84fe9b40 = Mpl.Run.DelegateFunction.make (delegate $$4m_84fe9b4) 1
static $$4n_84fea701 = Mpl.Run.DelegateFunction.make (delegate $$4n_84fea70) 1
static $$4k_84fb7b02 = Mpl.Run.DelegateFunction.make (delegate $$4k_84fb7b0) 1
static $$4l_84fe8983 = Mpl.Run.DelegateFunction.make (delegate $$4l_84fe898) 1
update = mc_σ#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_σ "Value {Mass 60000 Temperature-240000 Time-180000}";
    mc_Φ₊0₊#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_Φ_2E_0 "Value {Current-60000 Length 120000 Mass 60000 Time-120000}";
    mc_Φ_2E_0#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_Φ_2E_0 "Value {Current-60000 Length 120000 Mass 60000 Time-120000}";
    mc_α#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_α "Value {}";
    mc_R₊∞₊#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_R_2E_∞ "Value {Length-60000}";
    mc_R_2E_∞#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_R_2E_∞ "Value {Length-60000}";
    mc_R#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_R_2E_gas "Value {Length 120000 Mass 60000 Substance-60000 Temperature-60000 Time-120000}";
    mc_N₊A₊#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_N_2E_A "Value {Substance-60000}";
    mc_m₊u₊#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_m_2E_u "Value {Mass 60000}";
    mc_N_2E_A#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_N_2E_A "Value {Substance-60000}";
    mc_m_2E_u#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_m_2E_u "Value {Mass 60000}";
    mc_k#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_k "Value {Length 120000 Mass 60000 Temperature-60000 Time-120000}";
    mc_ℏ#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_ℏ "Value {Length 120000 Mass 60000 Time-60000}";
    mc_h#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_h "Value {Length 120000 Mass 60000 Time-60000}";
    mc_e₊c₊#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_e_2E_c "Value {Current 60000 Time 60000}";
    mc_e_2E_c#CONSTANT := McdRun.values.dnum McdStaticUnitSystemSI.mc_e_2E_c "Value {Current 60000 Time 60000}";
    mc_dpi := McdRun.values.dnum McdStaticUnitSystemSI.mc_dpi "Value {Length-60000}";
    mc_yr := McdRun.values.dnum McdStaticUnitSystemSI.mc_yr "Value {Time 60000}";
    mc_yd := McdRun.values.dnum McdStaticUnitSystemSI.mc_yd "Value {Length 60000}";
    mc_weber := McdRun.values.dnum McdStaticUnitSystemSI.mc_weber "Value {Current-60000 Length 120000 Mass 60000 Time-120000}";
    mc_watt := McdRun.values.dnum McdStaticUnitSystemSI.mc_watt "Value {Length 120000 Mass 60000 Time-180000}";
    mc_volt := McdRun.values.dnum McdStaticUnitSystemSI.mc_volt "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_torr := McdRun.values.dnum McdStaticUnitSystemSI.mc_torr "Value {Length-60000 Mass 60000 Time-120000}";
    mc_tonne := McdRun.values.dnum McdStaticUnitSystemSI.mc_tonne "Value {Mass 60000}";
    mc_tonf := McdRun.values.dnum McdStaticUnitSystemSI.mc_tonf "Value {Length 60000 Mass 60000 Time-120000}";
    mc_ton := McdRun.values.dnum McdStaticUnitSystemSI.mc_ton "Value {Mass 60000}";
    mc_tesla := McdRun.values.dnum McdStaticUnitSystemSI.mc_tesla "Value {Current-60000 Mass 60000 Time-120000}";
    mc_tcal := McdRun.values.dnum McdStaticUnitSystemSI.mc_tcal "Value {Length 120000 Mass 60000 Time-120000}";
    mc_tBTU := McdRun.values.dnum McdStaticUnitSystemSI.mc_tBTU "Value {Length 120000 Mass 60000 Time-120000}";
    mc_Smoot := McdRun.values.dnum McdStaticUnitSystemSI.mc_Smoot "Value {Length 60000}";
    mc_stokes := McdRun.values.dnum McdStaticUnitSystemSI.mc_stokes "Value {Length 120000 Time-60000}";
    mc_sr := McdRun.values.dnum McdStaticUnitSystemSI.mc_sr "Value {}";
    mc_slug := McdRun.values.dnum McdStaticUnitSystemSI.mc_slug "Value {Mass 60000}";
    mc_siemens := McdRun.values.dnum McdStaticUnitSystemSI.mc_siemens "Value {Current 120000 Length-120000 Mass-60000 Time 180000}";
    mc_s := McdRun.values.dnum McdStaticUnitSystemSI.mc_s "Value {Time 60000}";
    mc_rpm := McdRun.values.dnum McdStaticUnitSystemSI.mc_rpm "Value {Time-60000}";
    mc_rev := McdRun.values.dnum McdStaticUnitSystemSI.mc_rev "Value {}";
    mc_rad := McdRun.values.dnum McdStaticUnitSystemSI.mc_rad "Value {}";
    mc_psf := McdRun.values.dnum McdStaticUnitSystemSI.mc_psf "Value {Length-60000 Mass 60000 Time-120000}";
    mc_psi := McdRun.values.dnum McdStaticUnitSystemSI.mc_psi "Value {Length-60000 Mass 60000 Time-120000}";
    mc_poise := McdRun.values.dnum McdStaticUnitSystemSI.mc_poise "Value {Length-60000 Mass 60000 Time-60000}";
    mc_pci := McdRun.values.dnum McdStaticUnitSystemSI.mc_pci "Value {Length-120000 Mass 60000 Time-120000}";
    mc_pcf := McdRun.values.dnum McdStaticUnitSystemSI.mc_pcf "Value {Length-120000 Mass 60000 Time-120000}";
    mc_pF := McdRun.values.dnum McdStaticUnitSystemSI.mc_pF "Value {Current 120000 Length-120000 Mass-60000 Time 240000}";
    mc_ozf := McdRun.values.dnum McdStaticUnitSystemSI.mc_ozf "Value {Length 60000 Mass 60000 Time-120000}";
    mc_oz := McdRun.values.dnum McdStaticUnitSystemSI.mc_oz "Value {Mass 60000}";
    mc_ohm := McdRun.values.dnum McdStaticUnitSystemSI.mc_ohm "Value {Current-120000 Length 120000 Mass 60000 Time-180000}";
    mc_oersted := McdRun.values.dnum McdStaticUnitSystemSI.mc_oersted "Value {Current 60000 Length-60000}";
    mc_nmi := McdRun.values.dnum McdStaticUnitSystemSI.mc_nmi "Value {Length 60000}";
    mc_nm := McdRun.values.dnum McdStaticUnitSystemSI.mc_nm "Value {Length 60000}";
    mc_nF := McdRun.values.dnum McdStaticUnitSystemSI.mc_nF "Value {Current 120000 Length-120000 Mass-60000 Time 240000}";
    mc_newton := McdRun.values.dnum McdStaticUnitSystemSI.mc_newton "Value {Length 60000 Mass 60000 Time-120000}";
    mc_ms := McdRun.values.dnum McdStaticUnitSystemSI.mc_ms "Value {Time 60000}";
    mc_mph := McdRun.values.dnum McdStaticUnitSystemSI.mc_mph "Value {Length 60000 Time-60000}";
    mc_mol := McdRun.values.dnum McdStaticUnitSystemSI.mc_mol "Value {Substance 60000}";
    mc_mm := McdRun.values.dnum McdStaticUnitSystemSI.mc_mm "Value {Length 60000}";
    mc_min := McdRun.values.dnum McdStaticUnitSystemSI.mc_min "Value {Time 60000}";
    mc_mile := McdRun.values.dnum McdStaticUnitSystemSI.mc_mile "Value {Length 60000}";
    mc_mil := McdRun.values.dnum McdStaticUnitSystemSI.mc_mil "Value {Length 60000}";
    mc_mi := McdRun.values.dnum McdStaticUnitSystemSI.mc_mi "Value {Length 60000}";
    mc_micron := McdRun.values.dnum McdStaticUnitSystemSI.mc_micron "Value {Length 60000}";
    mc_mhp := McdRun.values.dnum McdStaticUnitSystemSI.mc_mhp "Value {Length 120000 Mass 60000 Time-180000}";
    mc_mho := McdRun.values.dnum McdStaticUnitSystemSI.mc_mho "Value {Current 120000 Length-120000 Mass-60000 Time 180000}";
    mc_mg := McdRun.values.dnum McdStaticUnitSystemSI.mc_mg "Value {Mass 60000}";
    mc_mcal := McdRun.values.dnum McdStaticUnitSystemSI.mc_mcal "Value {Length 120000 Mass 60000 Time-120000}";
    mc_pW := McdRun.values.dnum McdStaticUnitSystemSI.mc_pW "Value {Length 120000 Mass 60000 Time-180000}";
    mc_nW := McdRun.values.dnum McdStaticUnitSystemSI.mc_nW "Value {Length 120000 Mass 60000 Time-180000}";
    mc_μW := McdRun.values.dnum McdStaticUnitSystemSI.mc_μW "Value {Length 120000 Mass 60000 Time-180000}";
    mc_mW := McdRun.values.dnum McdStaticUnitSystemSI.mc_mW "Value {Length 120000 Mass 60000 Time-180000}";
    mc_pV := McdRun.values.dnum McdStaticUnitSystemSI.mc_pV "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_nV := McdRun.values.dnum McdStaticUnitSystemSI.mc_nV "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_μV := McdRun.values.dnum McdStaticUnitSystemSI.mc_μV "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_mV := McdRun.values.dnum McdStaticUnitSystemSI.mc_mV "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_mL := McdRun.values.dnum McdStaticUnitSystemSI.mc_mL "Value {Length 180000}";
    mc_mH := McdRun.values.dnum McdStaticUnitSystemSI.mc_mH "Value {Current-120000 Length 120000 Mass 60000 Time-120000}";
    mc_mBTU := McdRun.values.dnum McdStaticUnitSystemSI.mc_mBTU "Value {Length 120000 Mass 60000 Time-120000}";
    mc_mA := McdRun.values.dnum McdStaticUnitSystemSI.mc_mA "Value {Current 60000}";
    mc_lx := McdRun.values.dnum McdStaticUnitSystemSI.mc_lx "Value {Length-120000 Luminosity 60000}";
    mc_lm := McdRun.values.dnum McdStaticUnitSystemSI.mc_lm "Value {Luminosity 60000}";
    mc_l := McdRun.values.dnum McdStaticUnitSystemSI.mc_l "Value {Length 180000}";
    mc_liter := McdRun.values.dnum McdStaticUnitSystemSI.mc_liter "Value {Length 180000}";
    mc_lbf := McdRun.values.dnum McdStaticUnitSystemSI.mc_lbf "Value {Length 60000 Mass 60000 Time-120000}";
    mc_lbm := McdRun.values.dnum McdStaticUnitSystemSI.mc_lbm "Value {Mass 60000}";
    mc_lb := McdRun.values.dnum McdStaticUnitSystemSI.mc_lb "Value {Mass 60000}";
    mc_ksi := McdRun.values.dnum McdStaticUnitSystemSI.mc_ksi "Value {Length-60000 Mass 60000 Time-120000}";
    mc_ksf := McdRun.values.dnum McdStaticUnitSystemSI.mc_ksf "Value {Length-60000 Mass 60000 Time-120000}";
    mc_kph := McdRun.values.dnum McdStaticUnitSystemSI.mc_kph "Value {Length 60000 Time-60000}";
    mc_kPa := McdRun.values.dnum McdStaticUnitSystemSI.mc_kPa "Value {Length-60000 Mass 60000 Time-120000}";
    mc_knot := McdRun.values.dnum McdStaticUnitSystemSI.mc_knot "Value {Length 60000 Time-60000}";
    mc_km := McdRun.values.dnum McdStaticUnitSystemSI.mc_km "Value {Length 60000}";
    mc_plf := McdRun.values.dnum McdStaticUnitSystemSI.mc_plf "Value {Mass 60000 Time-120000}";
    mc_kpi := McdRun.values.dnum McdStaticUnitSystemSI.mc_kpi "Value {Mass 60000 Time-120000}";
    mc_klf := McdRun.values.dnum McdStaticUnitSystemSI.mc_klf "Value {Mass 60000 Time-120000}";
    mc_kip := McdRun.values.dnum McdStaticUnitSystemSI.mc_kip "Value {Length 60000 Mass 60000 Time-120000}";
    mc_kgf := McdRun.values.dnum McdStaticUnitSystemSI.mc_kgf "Value {Length 60000 Mass 60000 Time-120000}";
    mc_kcal := McdRun.values.dnum McdStaticUnitSystemSI.mc_kcal "Value {Length 120000 Mass 60000 Time-120000}";
    mc_katal := McdRun.values.dnum McdStaticUnitSystemSI.mc_katal "Value {Substance 60000 Time-60000}";
    mc_kΩ := McdRun.values.dnum McdStaticUnitSystemSI.mc_kΩ "Value {Current-120000 Length 120000 Mass 60000 Time-180000}";
    mc_kW := McdRun.values.dnum McdStaticUnitSystemSI.mc_kW "Value {Length 120000 Mass 60000 Time-180000}";
    mc_kV := McdRun.values.dnum McdStaticUnitSystemSI.mc_kV "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_kN·mm := McdRun.values.dnum McdStaticUnitSystemSI.mc_kN·mm "Value {Length 120000 Mass 60000 Time-120000}";
    mc_kN·cm := McdRun.values.dnum McdStaticUnitSystemSI.mc_kN·cm "Value {Length 120000 Mass 60000 Time-120000}";
    mc_kN·m := McdRun.values.dnum McdStaticUnitSystemSI.mc_kN·m "Value {Length 120000 Mass 60000 Time-120000}";
    mc_tonnef := McdRun.values.dnum McdStaticUnitSystemSI.mc_tonnef "Value {Length 60000 Mass 60000 Time-120000}";
    mc_TN := McdRun.values.dnum McdStaticUnitSystemSI.mc_TN "Value {Length 60000 Mass 60000 Time-120000}";
    mc_GN := McdRun.values.dnum McdStaticUnitSystemSI.mc_GN "Value {Length 60000 Mass 60000 Time-120000}";
    mc_MN := McdRun.values.dnum McdStaticUnitSystemSI.mc_MN "Value {Length 60000 Mass 60000 Time-120000}";
    mc_kN := McdRun.values.dnum McdStaticUnitSystemSI.mc_kN "Value {Length 60000 Mass 60000 Time-120000}";
    mc_kHz := McdRun.values.dnum McdStaticUnitSystemSI.mc_kHz "Value {Time-60000}";
    mc_kA := McdRun.values.dnum McdStaticUnitSystemSI.mc_kA "Value {Current 60000}";
    mc_joule := McdRun.values.dnum McdStaticUnitSystemSI.mc_joule "Value {Length 120000 Mass 60000 Time-120000}";
    mc_in_5F_Hg := McdRun.values.dnum McdStaticUnitSystemSI.mc_in_5F_Hg "Value {Length-60000 Mass 60000 Time-120000}";
    mc_in := McdRun.values.dnum McdStaticUnitSystemSI.mc_in "Value {Length 60000}";
    mc_hr := McdRun.values.dnum McdStaticUnitSystemSI.mc_hr "Value {Time 60000}";
    mc_hpUK := McdRun.values.dnum McdStaticUnitSystemSI.mc_hpUK "Value {Length 120000 Mass 60000 Time-180000}";
    mc_hp := McdRun.values.dnum McdStaticUnitSystemSI.mc_hp "Value {Length 120000 Mass 60000 Time-180000}";
    mc_hhp := McdRun.values.dnum McdStaticUnitSystemSI.mc_hhp "Value {Length 120000 Mass 60000 Time-180000}";
    mc_henry := McdRun.values.dnum McdStaticUnitSystemSI.mc_henry "Value {Current-120000 Length 120000 Mass 60000 Time-120000}";
    mc_hectare := McdRun.values.dnum McdStaticUnitSystemSI.mc_hectare "Value {Length 120000}";
    mc_gm := McdRun.values.dnum McdStaticUnitSystemSI.mc_gm "Value {Mass 60000}";
    mc_gauss := McdRun.values.dnum McdStaticUnitSystemSI.mc_gauss "Value {Current-60000 Mass 60000 Time-120000}";
    mc_mgd := McdRun.values.dnum McdStaticUnitSystemSI.mc_mgd "Value {Length 180000 Time-60000}";
    mc_cfm := McdRun.values.dnum McdStaticUnitSystemSI.mc_cfm "Value {Length 180000 Time-60000}";
    mc_gpm := McdRun.values.dnum McdStaticUnitSystemSI.mc_gpm "Value {Length 180000 Time-60000}";
    mc_galUK := McdRun.values.dnum McdStaticUnitSystemSI.mc_galUK "Value {Length 180000}";
    mc_gal := McdRun.values.dnum McdStaticUnitSystemSI.mc_gal "Value {Length 180000}";
    mc_g#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_g "Value {Length 60000 Time-120000}";
    mc_furlong := McdRun.values.dnum McdStaticUnitSystemSI.mc_furlong "Value {Length 60000}";
    mc_kW·hr := McdRun.values.dnum McdStaticUnitSystemSI.mc_kW·hr "Value {Length 120000 Mass 60000 Time-120000}";
    mc_W·hr := McdRun.values.dnum McdStaticUnitSystemSI.mc_W·hr "Value {Length 120000 Mass 60000 Time-120000}";
    mc_W·s := McdRun.values.dnum McdStaticUnitSystemSI.mc_W·s "Value {Length 120000 Mass 60000 Time-120000}";
    mc_in·ozf := McdRun.values.dnum McdStaticUnitSystemSI.mc_in·ozf "Value {Length 120000 Mass 60000 Time-120000}";
    mc_in·kip := McdRun.values.dnum McdStaticUnitSystemSI.mc_in·kip "Value {Length 120000 Mass 60000 Time-120000}";
    mc_in·lbf := McdRun.values.dnum McdStaticUnitSystemSI.mc_in·lbf "Value {Length 120000 Mass 60000 Time-120000}";
    mc_ft·kip := McdRun.values.dnum McdStaticUnitSystemSI.mc_ft·kip "Value {Length 120000 Mass 60000 Time-120000}";
    mc_ft·lbf := McdRun.values.dnum McdStaticUnitSystemSI.mc_ft·lbf "Value {Length 120000 Mass 60000 Time-120000}";
    mc_ft := McdRun.values.dnum McdStaticUnitSystemSI.mc_ft "Value {Length 60000}";
    mc_fortnight := McdRun.values.dnum McdStaticUnitSystemSI.mc_fortnight "Value {Time 60000}";
    mc_fl_5F_oz := McdRun.values.dnum McdStaticUnitSystemSI.mc_fl_5F_oz "Value {Length 180000}";
    mc_farad := McdRun.values.dnum McdStaticUnitSystemSI.mc_farad "Value {Current 120000 Length-120000 Mass-60000 Time 240000}";
    mc_erg := McdRun.values.dnum McdStaticUnitSystemSI.mc_erg "Value {Length 120000 Mass 60000 Time-120000}";
    mc_ehp := McdRun.values.dnum McdStaticUnitSystemSI.mc_ehp "Value {Length 120000 Mass 60000 Time-180000}";
    mc_dyne := McdRun.values.dnum McdStaticUnitSystemSI.mc_dyne "Value {Length 60000 Mass 60000 Time-120000}";
    mc_° := McdRun.values.dnum McdStaticUnitSystemSI.mc_° "Value {}";
    mc_deg := McdRun.values.dnum McdStaticUnitSystemSI.mc_deg "Value {}";
    mc_dcal := McdRun.values.dnum McdStaticUnitSystemSI.mc_dcal "Value {Length 120000 Mass 60000 Time-120000}";
    mc_day := McdRun.values.dnum McdStaticUnitSystemSI.mc_day "Value {Time 60000}";
    mc_cubit := McdRun.values.dnum McdStaticUnitSystemSI.mc_cubit "Value {Length 60000}";
    mc_pC := McdRun.values.dnum McdStaticUnitSystemSI.mc_pC "Value {Current 60000 Time 60000}";
    mc_nC := McdRun.values.dnum McdStaticUnitSystemSI.mc_nC "Value {Current 60000 Time 60000}";
    mc_μC := McdRun.values.dnum McdStaticUnitSystemSI.mc_μC "Value {Current 60000 Time 60000}";
    mc_mC := McdRun.values.dnum McdStaticUnitSystemSI.mc_mC "Value {Current 60000 Time 60000}";
    mc_coul := McdRun.values.dnum McdStaticUnitSystemSI.mc_coul "Value {Current 60000 Time 60000}";
    mc_cm := McdRun.values.dnum McdStaticUnitSystemSI.mc_cm "Value {Length 60000}";
    mc_cal20 := McdRun.values.dnum McdStaticUnitSystemSI.mc_cal20 "Value {Length 120000 Mass 60000 Time-120000}";
    mc_cal15 := McdRun.values.dnum McdStaticUnitSystemSI.mc_cal15 "Value {Length 120000 Mass 60000 Time-120000}";
    mc_cal := McdRun.values.dnum McdStaticUnitSystemSI.mc_cal "Value {Length 120000 Mass 60000 Time-120000}";
    mc_c#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_c "Value {Length 60000 Time-60000}";
    mc_a₊0₊ := McdRun.values.dnum McdStaticUnitSystemSI.mc_a_2E_0 "Value {Length 60000}";
    mc_a_2E_0 := McdRun.values.dnum McdStaticUnitSystemSI.mc_a_2E_0 "Value {Length 60000}";
    mc_bohr := McdRun.values.dnum McdStaticUnitSystemSI.mc_bohr "Value {Length 60000}";
    mc_bhp := McdRun.values.dnum McdStaticUnitSystemSI.mc_bhp "Value {Length 120000 Mass 60000 Time-180000}";
    mc_barn := McdRun.values.dnum McdStaticUnitSystemSI.mc_barn "Value {Length 120000}";
    mc_bar := McdRun.values.dnum McdStaticUnitSystemSI.mc_bar "Value {Length-60000 Mass 60000 Time-120000}";
    mc_atm := McdRun.values.dnum McdStaticUnitSystemSI.mc_atm "Value {Length-60000 Mass 60000 Time-120000}";
    mc_arcmin := McdRun.values.dnum McdStaticUnitSystemSI.mc_arcmin "Value {}";
    mc_arcsec := McdRun.values.dnum McdStaticUnitSystemSI.mc_arcsec "Value {}";
    mc_acre·ft := McdRun.values.dnum McdStaticUnitSystemSI.mc_acre·ft "Value {Length 180000}";
    mc_acre := McdRun.values.dnum McdStaticUnitSystemSI.mc_acre "Value {Length 120000}";
    mc_ps := McdRun.values.dnum McdStaticUnitSystemSI.mc_ps "Value {Time 60000}";
    mc_ns := McdRun.values.dnum McdStaticUnitSystemSI.mc_ns "Value {Time 60000}";
    mc_μs := McdRun.values.dnum McdStaticUnitSystemSI.mc_μs "Value {Time 60000}";
    mc_μm := McdRun.values.dnum McdStaticUnitSystemSI.mc_μm "Value {Length 60000}";
    mc_μH := McdRun.values.dnum McdStaticUnitSystemSI.mc_μH "Value {Current-120000 Length 120000 Mass 60000 Time-120000}";
    mc_μF := McdRun.values.dnum McdStaticUnitSystemSI.mc_μF "Value {Current 120000 Length-120000 Mass-60000 Time 240000}";
    mc_pA := McdRun.values.dnum McdStaticUnitSystemSI.mc_pA "Value {Current 60000}";
    mc_nA := McdRun.values.dnum McdStaticUnitSystemSI.mc_nA "Value {Current 60000}";
    mc_μA := McdRun.values.dnum McdStaticUnitSystemSI.mc_μA "Value {Current 60000}";
    mc_Ω := McdRun.values.dnum McdStaticUnitSystemSI.mc_Ω "Value {Current-120000 Length 120000 Mass 60000 Time-180000}";
    mc_Wb := McdRun.values.dnum McdStaticUnitSystemSI.mc_Wb "Value {Current-60000 Length 120000 Mass 60000 Time-120000}";
    mc_W := McdRun.values.dnum McdStaticUnitSystemSI.mc_W "Value {Length 120000 Mass 60000 Time-180000}";
    mc_V := McdRun.values.dnum McdStaticUnitSystemSI.mc_V "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_T := McdRun.values.dnum McdStaticUnitSystemSI.mc_T "Value {Current-60000 Mass 60000 Time-120000}";
    mc_Sv := McdRun.values.dnum McdStaticUnitSystemSI.mc_Sv "Value {Length 120000 Time-120000}";
    mc_S := McdRun.values.dnum McdStaticUnitSystemSI.mc_S "Value {Current 120000 Length-120000 Mass-60000 Time 180000}";
    mc_°R := McdRun.values.dnum McdStaticUnitSystemSI.mc_°R "Value {Temperature 60000}";
    mc_R := McdRun.values.dnum McdStaticUnitSystemSI.mc_R "Value {Temperature 60000}";
    mc_MPa := McdRun.values.dnum McdStaticUnitSystemSI.mc_MPa "Value {Length-60000 Mass 60000 Time-120000}";
    mc_Pa := McdRun.values.dnum McdStaticUnitSystemSI.mc_Pa "Value {Length-60000 Mass 60000 Time-120000}";
    mc_Oe := McdRun.values.dnum McdStaticUnitSystemSI.mc_Oe "Value {Current 60000 Length-60000}";
    mc_N·mm := McdRun.values.dnum McdStaticUnitSystemSI.mc_N·mm "Value {Length 120000 Mass 60000 Time-120000}";
    mc_N·cm := McdRun.values.dnum McdStaticUnitSystemSI.mc_N·cm "Value {Length 120000 Mass 60000 Time-120000}";
    mc_N·m := McdRun.values.dnum McdStaticUnitSystemSI.mc_N·m "Value {Length 120000 Mass 60000 Time-120000}";
    mc_N := McdRun.values.dnum McdStaticUnitSystemSI.mc_N "Value {Length 60000 Mass 60000 Time-120000}";
    mc_MW := McdRun.values.dnum McdStaticUnitSystemSI.mc_MW "Value {Length 120000 Mass 60000 Time-180000}";
    mc_MΩ := McdRun.values.dnum McdStaticUnitSystemSI.mc_MΩ "Value {Current-120000 Length 120000 Mass 60000 Time-180000}";
    mc_MHz := McdRun.values.dnum McdStaticUnitSystemSI.mc_MHz "Value {Time-60000}";
    mc_L := McdRun.values.dnum McdStaticUnitSystemSI.mc_L "Value {Length 180000}";
    mc_KΩ := McdRun.values.dnum McdStaticUnitSystemSI.mc_KΩ "Value {Current-120000 Length 120000 Mass 60000 Time-180000}";
    mc_KV := McdRun.values.dnum McdStaticUnitSystemSI.mc_KV "Value {Current-60000 Length 120000 Mass 60000 Time-180000}";
    mc_KHz := McdRun.values.dnum McdStaticUnitSystemSI.mc_KHz "Value {Time-60000}";
    mc_KA := McdRun.values.dnum McdStaticUnitSystemSI.mc_KA "Value {Current 60000}";
    mc_J := McdRun.values.dnum McdStaticUnitSystemSI.mc_J "Value {Length 120000 Mass 60000 Time-120000}";
    mc_IBTU := McdRun.values.dnum McdStaticUnitSystemSI.mc_IBTU "Value {Length 120000 Mass 60000 Time-120000}";
    mc_Hza := McdRun.values.dnum McdStaticUnitSystemSI.mc_Hza "Value {Time-60000}";
    mc_Hz := McdRun.values.dnum McdStaticUnitSystemSI.mc_Hz "Value {Time-60000}";
    mc_H := McdRun.values.dnum McdStaticUnitSystemSI.mc_H "Value {Current-120000 Length 120000 Mass 60000 Time-120000}";
    mc_Gy := McdRun.values.dnum McdStaticUnitSystemSI.mc_Gy "Value {Length 120000 Time-120000}";
    mc_GPa := McdRun.values.dnum McdStaticUnitSystemSI.mc_GPa "Value {Length-60000 Mass 60000 Time-120000}";
    mc_GHz := McdRun.values.dnum McdStaticUnitSystemSI.mc_GHz "Value {Time-60000}";
    mc_G := McdRun.values.dnum McdStaticUnitSystemSI.mc_G "Value {Current-60000 Mass 60000 Time-120000}";
    mc_F := McdRun.values.dnum McdStaticUnitSystemSI.mc_F "Value {Current 120000 Length-120000 Mass-60000 Time 240000}";
    mc_Δ℉ := McdRun.values.dnum McdStaticUnitSystemSI.mc_Δ°F "Value {Temperature 60000}";
    mc_Δ℃ := McdRun.values.dnum McdStaticUnitSystemSI.mc_Δ°C "Value {Temperature 60000}";
    mc_Δ°F := McdRun.values.dnum McdStaticUnitSystemSI.mc_Δ°F "Value {Temperature 60000}";
    mc_Δ°C := McdRun.values.dnum McdStaticUnitSystemSI.mc_Δ°C "Value {Temperature 60000}";
    mc_CBTU := McdRun.values.dnum McdStaticUnitSystemSI.mc_CBTU "Value {Length 120000 Mass 60000 Time-120000}";
    mc_C := McdRun.values.dnum McdStaticUnitSystemSI.mc_C "Value {Current 60000 Time 60000}";
    mc_Bq := McdRun.values.dnum McdStaticUnitSystemSI.mc_Bq "Value {Time-60000}";
    mc_BTU15 := McdRun.values.dnum McdStaticUnitSystemSI.mc_BTU15 "Value {Length 120000 Mass 60000 Time-120000}";
    mc_Btu := McdRun.values.dnum McdStaticUnitSystemSI.mc_Btu "Value {Length 120000 Mass 60000 Time-120000}";
    mc_BTU := McdRun.values.dnum McdStaticUnitSystemSI.mc_BTU "Value {Length 120000 Mass 60000 Time-120000}";
    mc_Angstrom := McdRun.values.dnum McdStaticUnitSystemSI.mc_Angstrom "Value {Length 60000}";
    mc_A := McdRun.values.dnum McdStaticUnitSystemSI.mc_A "Value {Current 60000}";
    mc_ε₊0₊#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_ε_2E_0 "Value {Current 120000 Length-180000 Mass-60000 Time 240000}";
    mc_μ₊0₊#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_μ_2E_0 "Value {Current-120000 Length 60000 Mass 60000 Time-120000}";
    mc_ε_2E_0#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_ε_2E_0 "Value {Current 120000 Length-180000 Mass-60000 Time 240000}";
    mc_μ_2E_0#CONSTANT#UNIT := McdRun.values.dnum McdStaticUnitSystemSI.mc_μ_2E_0 "Value {Current-120000 Length 60000 Mass 60000 Time-120000}";
    mc_K := McdRun.values.dnum McdStaticUnitSystemSI.mc_K "Value {Temperature 60000}";
    $°F_846a964 := Efi.Matrix.mult (McdRun.values.rv (div 5.0 9.0)) mc_K;
    $°c_846ad88 := Efi.Matrix.mult (McdRun.values.rv 273.15) mc_K;
    $°f_846b06c := Efi.Core.sub $°c_846ad88 (Efi.Matrix.mult (McdRun.values.rv 32.0) $°F_846a964);
    (let inverse0 : ObjectArray = McdRun.values.declare_inverse $$4m_84fe9b40 $$4n_84fea701 in $kf_84665dc := McdRun.values.stdfunc 0 (inverse0[1]);
    $fk_84663d4 := McdRun.values.stdfunc 0 (inverse0[0]);
    (let inverse1 : ObjectArray = McdRun.values.declare_inverse $$4k_84fb7b02 $$4l_84fe8983 in $kc_8465840 := McdRun.values.stdfunc 0 (inverse1[1]);
    $ck_8469634 := McdRun.values.stdfunc 0 (inverse1[0]);
    mc__2F_℉ := $kf_84665dc;
    mc__2F_°F := $kf_84665dc;
    mc__2F_℃ := $kc_8465840;
    mc__2F_°C := $kc_8465840;
    mc_℉ := $fk_84663d4;
    mc_°F := $fk_84663d4;
    mc_℃ := $ck_8469634;
    mc_°C := $ck_8469634;
    mc_¤ := McdRun.values.dnum McdStaticUnitSystemSI.mc_¤ "Value {Money 60000}";
    mc_mole := McdRun.values.dnum McdStaticUnitSystemSI.mc_mole "Value {Substance 60000}";
    mc_cd := McdRun.values.dnum McdStaticUnitSystemSI.mc_cd "Value {Luminosity 60000}";
    mc_amp := McdRun.values.dnum McdStaticUnitSystemSI.mc_amp "Value {Current 60000}";
    mc_sec := McdRun.values.dnum McdStaticUnitSystemSI.mc_sec "Value {Time 60000}";
    mc_m := McdRun.values.dnum McdStaticUnitSystemSI.mc_m "Value {Length 60000}";
    mc_kg := McdRun.values.dnum McdStaticUnitSystemSI.mc_kg "Value {Mass 60000}";
    system := McdStaticUnitSystemSI.system))
$$4m_84fe9b4 :: McdRun.V {}->McdRun.V {};
$$4n_84fea70 :: McdRun.V {}->McdRun.V {};
$$4k_84fb7b0 :: McdRun.V {}->McdRun.V {};
$$4l_84fe898 :: McdRun.V {}->McdRun.V {};
dyn value = tail McdRun.values.dnum value "V {d}"
$$4m_84fe9b4 value = tail Efi.Core.add (Efi.Matrix.mult value $°F_846a964) $°f_846b06c
$$4n_84fea70 value = tail Efi.Matrix.div (Efi.Core.sub value $°f_846b06c) $°F_846a964
$$4k_84fb7b0 value = tail Efi.Core.add (Efi.Matrix.mult value mc_K) $°c_846ad88
$$4l_84fe898 value = tail Efi.Matrix.div (Efi.Core.sub value $°c_846ad88) mc_K
op_D value = tail McdRun.values.times_unit value mc_¤
op_S value = tail McdRun.values.times_unit value mc_mole
op_C value = tail McdRun.values.times_unit value mc_cd
op_K value = tail McdRun.values.times_unit value mc_K
op_Q value = tail McdRun.values.times_unit value mc_amp
op_T value = tail McdRun.values.times_unit value mc_sec
op_L value = tail McdRun.values.times_unit value mc_m
op_M value = tail McdRun.values.times_unit value mc_kg
