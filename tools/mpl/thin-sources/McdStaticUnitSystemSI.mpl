-- SI unit catalogue, normalized to the Mpl source forms.
module McdStaticUnitSystemSI (McdStaticUnitSystemSI.mc_pF,McdStaticUnitSystemSI.mc_pcf,McdStaticUnitSystemSI.mc_pci,McdStaticUnitSystemSI.mc_poise,McdStaticUnitSystemSI.mc_psi,McdStaticUnitSystemSI.mc_psf,McdStaticUnitSystemSI.mc_pW,McdStaticUnitSystemSI.mc_mcal,McdStaticUnitSystemSI.mc_mg,McdStaticUnitSystemSI.mc_mho,McdStaticUnitSystemSI.mc_mhp,McdStaticUnitSystemSI.mc_micron,McdStaticUnitSystemSI.mc_mi,McdStaticUnitSystemSI.mc_mil,McdStaticUnitSystemSI.mc_newton,McdStaticUnitSystemSI.mc_nF,McdStaticUnitSystemSI.mc_nm,McdStaticUnitSystemSI.mc_nmi,McdStaticUnitSystemSI.mc_oersted,McdStaticUnitSystemSI.mc_ohm,McdStaticUnitSystemSI.mc_oz,McdStaticUnitSystemSI.mc_ozf,McdStaticUnitSystemSI.mc_mile,McdStaticUnitSystemSI.mc_min,McdStaticUnitSystemSI.mc_mm,McdStaticUnitSystemSI.mc_mol,McdStaticUnitSystemSI.mc_mole,McdStaticUnitSystemSI.mc_mph,McdStaticUnitSystemSI.mc_ms,McdStaticUnitSystemSI.mc_tonne,McdStaticUnitSystemSI.mc_torr,McdStaticUnitSystemSI.mc_volt,McdStaticUnitSystemSI.mc_watt,McdStaticUnitSystemSI.mc_weber,McdStaticUnitSystemSI.mc_yd,McdStaticUnitSystemSI.mc_yr,McdStaticUnitSystemSI.mc_dpi,McdStaticUnitSystemSI.mc_rad,McdStaticUnitSystemSI.mc_rev,McdStaticUnitSystemSI.mc_rpm,McdStaticUnitSystemSI.mc_s,McdStaticUnitSystemSI.mc_sec,McdStaticUnitSystemSI.mc_siemens,McdStaticUnitSystemSI.mc_slug,McdStaticUnitSystemSI.mc_e_2E_c,McdStaticUnitSystemSI.mc_h,McdStaticUnitSystemSI.mc_ℏ,McdStaticUnitSystemSI.mc_k,McdStaticUnitSystemSI.mc_m_2E_u,McdStaticUnitSystemSI.mc_N_2E_A,McdStaticUnitSystemSI.mc_R_2E_gas,McdStaticUnitSystemSI.mc_R_2E_∞,McdStaticUnitSystemSI.mc_sr,McdStaticUnitSystemSI.mc_stokes,McdStaticUnitSystemSI.mc_Smoot,McdStaticUnitSystemSI.mc_tBTU,McdStaticUnitSystemSI.mc_tcal,McdStaticUnitSystemSI.mc_tesla,McdStaticUnitSystemSI.mc_ton,McdStaticUnitSystemSI.mc_tonf,McdStaticUnitSystemSI.mc_α,McdStaticUnitSystemSI.mc_Φ_2E_0,McdStaticUnitSystemSI.mc_σ,McdStaticUnitSystemSI.system,McdStaticUnitSystemSI.op_M,McdStaticUnitSystemSI.op_L,McdStaticUnitSystemSI.op_T,McdStaticUnitSystemSI.op_Q,McdStaticUnitSystemSI.op_K,McdStaticUnitSystemSI.op_C,McdStaticUnitSystemSI.op_S,McdStaticUnitSystemSI.op_D,McdStaticUnitSystemSI.mc_¤,McdStaticUnitSystemSI.mc_μ_2E_0,McdStaticUnitSystemSI.mc_GPa,McdStaticUnitSystemSI.mc_Gy,McdStaticUnitSystemSI.mc_H,McdStaticUnitSystemSI.mc_Hz,McdStaticUnitSystemSI.mc_Hza,McdStaticUnitSystemSI.mc_IBTU,McdStaticUnitSystemSI.mc_J,McdStaticUnitSystemSI.mc_K,McdStaticUnitSystemSI.mc_CBTU,McdStaticUnitSystemSI.mc_Δ°C,McdStaticUnitSystemSI.mc_Δ°F,McdStaticUnitSystemSI.mc_Δ℃,McdStaticUnitSystemSI.mc_Δ℉,McdStaticUnitSystemSI.mc_F,McdStaticUnitSystemSI.mc_G,McdStaticUnitSystemSI.mc_GHz,McdStaticUnitSystemSI.mc_ε_2E_0,McdStaticUnitSystemSI.mc_A,McdStaticUnitSystemSI.mc_Angstrom,McdStaticUnitSystemSI.mc_BTU,McdStaticUnitSystemSI.mc_Btu,McdStaticUnitSystemSI.mc_BTU15,McdStaticUnitSystemSI.mc_Bq,McdStaticUnitSystemSI.mc_C,McdStaticUnitSystemSI.mc_KA,McdStaticUnitSystemSI.mc_KHz,McdStaticUnitSystemSI.mc_KV,McdStaticUnitSystemSI.mc_KΩ,McdStaticUnitSystemSI.mc_L,McdStaticUnitSystemSI.mc_MHz,McdStaticUnitSystemSI.mc_MΩ,McdStaticUnitSystemSI.mc_μH,McdStaticUnitSystemSI.mc_μm,McdStaticUnitSystemSI.mc_μs,McdStaticUnitSystemSI.mc_ns,McdStaticUnitSystemSI.mc_ps,McdStaticUnitSystemSI.mc_acre,McdStaticUnitSystemSI.mc_acre·ft,McdStaticUnitSystemSI.mc_W,McdStaticUnitSystemSI.mc_Wb,McdStaticUnitSystemSI.mc_Ω,McdStaticUnitSystemSI.mc_μA,McdStaticUnitSystemSI.mc_nA,McdStaticUnitSystemSI.mc_pA,McdStaticUnitSystemSI.mc_μF,McdStaticUnitSystemSI.mc_MW,McdStaticUnitSystemSI.mc_N,McdStaticUnitSystemSI.mc_N·m,McdStaticUnitSystemSI.mc_N·cm,McdStaticUnitSystemSI.mc_N·mm,McdStaticUnitSystemSI.mc_Oe,McdStaticUnitSystemSI.mc_Pa,McdStaticUnitSystemSI.mc_MPa,McdStaticUnitSystemSI.mc_R,McdStaticUnitSystemSI.mc_°R,McdStaticUnitSystemSI.mc_S,McdStaticUnitSystemSI.mc_Sv,McdStaticUnitSystemSI.mc_T,McdStaticUnitSystemSI.mc_V,McdStaticUnitSystemSI.mc_amp,McdStaticUnitSystemSI.mc_arcsec,McdStaticUnitSystemSI.mc_arcmin,McdStaticUnitSystemSI.mc_atm,McdStaticUnitSystemSI.mc_bar,McdStaticUnitSystemSI.mc_barn,McdStaticUnitSystemSI.mc_bhp,McdStaticUnitSystemSI.mc_bohr,McdStaticUnitSystemSI.mc_a_2E_0,McdStaticUnitSystemSI.mc_c,McdStaticUnitSystemSI.mc_cal,McdStaticUnitSystemSI.mc_cal15,McdStaticUnitSystemSI.mc_cal20,McdStaticUnitSystemSI.mc_cd,McdStaticUnitSystemSI.mc_cm,McdStaticUnitSystemSI.mc_coul,McdStaticUnitSystemSI.mc_deg,McdStaticUnitSystemSI.mc_°,McdStaticUnitSystemSI.mc_dyne,McdStaticUnitSystemSI.mc_ehp,McdStaticUnitSystemSI.mc_erg,McdStaticUnitSystemSI.mc_farad,McdStaticUnitSystemSI.mc_fl_5F_oz,McdStaticUnitSystemSI.mc_fortnight,McdStaticUnitSystemSI.mc_W·s,McdStaticUnitSystemSI.mc_W·hr,McdStaticUnitSystemSI.mc_kW·hr,McdStaticUnitSystemSI.mc_furlong,McdStaticUnitSystemSI.mc_g,McdStaticUnitSystemSI.mc_gal,McdStaticUnitSystemSI.mc_galUK,McdStaticUnitSystemSI.mc_mC,McdStaticUnitSystemSI.mc_μC,McdStaticUnitSystemSI.mc_nC,McdStaticUnitSystemSI.mc_pC,McdStaticUnitSystemSI.mc_cubit,McdStaticUnitSystemSI.mc_day,McdStaticUnitSystemSI.mc_dcal,McdStaticUnitSystemSI.mc_ft,McdStaticUnitSystemSI.mc_ft·lbf,McdStaticUnitSystemSI.mc_ft·kip,McdStaticUnitSystemSI.mc_in·lbf,McdStaticUnitSystemSI.mc_in·kip,McdStaticUnitSystemSI.mc_in·ozf,McdStaticUnitSystemSI.mc_henry,McdStaticUnitSystemSI.mc_hhp,McdStaticUnitSystemSI.mc_hp,McdStaticUnitSystemSI.mc_hpUK,McdStaticUnitSystemSI.mc_hr,McdStaticUnitSystemSI.mc_in,McdStaticUnitSystemSI.mc_in_5F_Hg,McdStaticUnitSystemSI.mc_joule,McdStaticUnitSystemSI.mc_kN·m,McdStaticUnitSystemSI.mc_kN·cm,McdStaticUnitSystemSI.mc_kN·mm,McdStaticUnitSystemSI.mc_kV,McdStaticUnitSystemSI.mc_kW,McdStaticUnitSystemSI.mc_kΩ,McdStaticUnitSystemSI.mc_kpi,McdStaticUnitSystemSI.mc_plf,McdStaticUnitSystemSI.mc_km,McdStaticUnitSystemSI.mc_knot,McdStaticUnitSystemSI.mc_kPa,McdStaticUnitSystemSI.mc_kph,McdStaticUnitSystemSI.mc_gpm,McdStaticUnitSystemSI.mc_cfm,McdStaticUnitSystemSI.mc_mgd,McdStaticUnitSystemSI.mc_gauss,McdStaticUnitSystemSI.mc_gm,McdStaticUnitSystemSI.mc_hectare,McdStaticUnitSystemSI.mc_kA,McdStaticUnitSystemSI.mc_kHz,McdStaticUnitSystemSI.mc_kN,McdStaticUnitSystemSI.mc_MN,McdStaticUnitSystemSI.mc_GN,McdStaticUnitSystemSI.mc_TN,McdStaticUnitSystemSI.mc_tonnef,McdStaticUnitSystemSI.mc_katal,McdStaticUnitSystemSI.mc_kcal,McdStaticUnitSystemSI.mc_kg,McdStaticUnitSystemSI.mc_kgf,McdStaticUnitSystemSI.mc_kip,McdStaticUnitSystemSI.mc_klf,McdStaticUnitSystemSI.mc_mV,McdStaticUnitSystemSI.mc_μV,McdStaticUnitSystemSI.mc_nV,McdStaticUnitSystemSI.mc_pV,McdStaticUnitSystemSI.mc_mW,McdStaticUnitSystemSI.mc_μW,McdStaticUnitSystemSI.mc_nW,McdStaticUnitSystemSI.mc_lm,McdStaticUnitSystemSI.mc_lx,McdStaticUnitSystemSI.mc_m,McdStaticUnitSystemSI.mc_mA,McdStaticUnitSystemSI.mc_mBTU,McdStaticUnitSystemSI.mc_mH,McdStaticUnitSystemSI.mc_mL,McdStaticUnitSystemSI.mc_ksf,McdStaticUnitSystemSI.mc_ksi,McdStaticUnitSystemSI.mc_lb,McdStaticUnitSystemSI.mc_lbm,McdStaticUnitSystemSI.mc_lbf,McdStaticUnitSystemSI.mc_liter,McdStaticUnitSystemSI.mc_l,McdStaticUnitSystemSI.mc_SIUnitsOf,McdStaticUnitSystemSI.mc_UnitsOf,McdStaticUnitSystem.Length(..),McdStaticUnitSystem.Time(..),McdStaticUnitSystem.Mass(..),McdStaticUnitSystem.Current(..),McdStaticUnitSystem.Temperature(..),McdStaticUnitSystem.Substance(..),McdStaticUnitSystem.Luminosity(..),McdStaticUnitSystem.Money(..)) where
{
 mc_pF :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 240000} @ "McdStaticUnitSystemSI.mc_pF";
 mc_pcf :: Mpl.Value {McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_pcf";
 mc_pci :: Mpl.Value {McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_pci";
 mc_poise :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_poise";
 mc_psi :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_psi";
 mc_psf :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_psf";
 mc_pW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_pW";
 mc_mcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_mcal";
 mc_mg :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_mg";
 mc_mho :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 180000} @ "McdStaticUnitSystemSI.mc_mho";
 mc_mhp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_mhp";
 mc_micron :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_micron";
 mc_mi :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_mi";
 mc_mil :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_mil";
 mc_newton :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_newton";
 mc_nF :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 240000} @ "McdStaticUnitSystemSI.mc_nF";
 mc_nm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_nm";
 mc_nmi :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_nmi";
 mc_oersted :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Length-60000} @ "McdStaticUnitSystemSI.mc_oersted";
 mc_ohm :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_ohm";
 mc_oz :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_oz";
 mc_ozf :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_ozf";
 mc_mile :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_mile";
 mc_min :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_min";
 mc_mm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_mm";
 mc_mol :: Mpl.Value {McdStaticUnitSystem.Substance 60000} @ "McdStaticUnitSystemSI.mc_mol";
 mc_mole :: Mpl.Value {McdStaticUnitSystem.Substance 60000} @ "McdStaticUnitSystemSI.mc_mole";
 mc_mph :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_mph";
 mc_ms :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_ms";
 mc_tonne :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_tonne";
 mc_torr :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_torr";
 mc_volt :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_volt";
 mc_watt :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_watt";
 mc_weber :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_weber";
 mc_yd :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_yd";
 mc_yr :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_yr";
 mc_dpi :: Mpl.Value {McdStaticUnitSystem.Length-60000} @ "McdStaticUnitSystemSI.mc_dpi";
 mc_rad :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_rad";
 mc_rev :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_rev";
 mc_rpm :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_rpm";
 mc_s :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_s";
 mc_sec :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_sec";
 mc_siemens :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 180000} @ "McdStaticUnitSystemSI.mc_siemens";
 mc_slug :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_slug";
 mc_e_2E_c :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_e_2E_c";
 mc_h :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_h";
 mc_ℏ :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_ℏ";
 mc_k :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Temperature-60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_k";
 mc_m_2E_u :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_m_2E_u";
 mc_N_2E_A :: Mpl.Value {McdStaticUnitSystem.Substance-60000} @ "McdStaticUnitSystemSI.mc_N_2E_A";
 mc_R_2E_gas :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Substance-60000 McdStaticUnitSystem.Temperature-60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_R_2E_gas";
 mc_R_2E_∞ :: Mpl.Value {McdStaticUnitSystem.Length-60000} @ "McdStaticUnitSystemSI.mc_R_2E_∞";
 mc_sr :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_sr";
 mc_stokes :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_stokes";
 mc_Smoot :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_Smoot";
 mc_tBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_tBTU";
 mc_tcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_tcal";
 mc_tesla :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_tesla";
 mc_ton :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_ton";
 mc_tonf :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_tonf";
 mc_α :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_α";
 mc_Φ_2E_0 :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_Φ_2E_0";
 mc_σ :: Mpl.Value {McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Temperature-240000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_σ";
 system :: McdRun.UnitSystemGroup @ "McdStaticUnitSystemSI.system";
 op_M :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Mass 60000} @ 46;
 op_L :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Length 60000} @ 46;
 op_T :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Time 60000} @ 46;
 op_Q :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Current 60000} @ 46;
 op_K :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ 46;
 op_C :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ 46;
 op_S :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Substance 60000} @ 46;
 op_D :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Money 60000} @ 46;
 mc_¤ :: Mpl.Value {McdStaticUnitSystem.Money 60000} @ "McdStaticUnitSystemSI.mc_¤";
 mc_μ_2E_0 :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_μ_2E_0";
 mc_GPa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_GPa";
 mc_Gy :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_Gy";
 mc_H :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_H";
 mc_Hz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_Hz";
 mc_Hza :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_Hza";
 mc_IBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_IBTU";
 mc_J :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_J";
 mc_K :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_K";
 mc_CBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_CBTU";
 mc_Δ°C :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_Δ°C";
 mc_Δ°F :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_Δ°F";
 mc_Δ℃ :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_Δ℃";
 mc_Δ℉ :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_Δ℉";
 mc_F :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 240000} @ "McdStaticUnitSystemSI.mc_F";
 mc_G :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_G";
 mc_GHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_GHz";
 mc_ε_2E_0 :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-180000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 240000} @ "McdStaticUnitSystemSI.mc_ε_2E_0";
 mc_A :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_A";
 mc_Angstrom :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_Angstrom";
 mc_BTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_BTU";
 mc_Btu :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_Btu";
 mc_BTU15 :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_BTU15";
 mc_Bq :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_Bq";
 mc_C :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_C";
 mc_KA :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_KA";
 mc_KHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_KHz";
 mc_KV :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_KV";
 mc_KΩ :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_KΩ";
 mc_L :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_L";
 mc_MHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_MHz";
 mc_MΩ :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_MΩ";
 mc_μH :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_μH";
 mc_μm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_μm";
 mc_μs :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_μs";
 mc_ns :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_ns";
 mc_ps :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_ps";
 mc_acre :: Mpl.Value {McdStaticUnitSystem.Length 120000} @ "McdStaticUnitSystemSI.mc_acre";
 mc_acre·ft :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_acre·ft";
 mc_W :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_W";
 mc_Wb :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_Wb";
 mc_Ω :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_Ω";
 mc_μA :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_μA";
 mc_nA :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_nA";
 mc_pA :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_pA";
 mc_μF :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 240000} @ "McdStaticUnitSystemSI.mc_μF";
 mc_MW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_MW";
 mc_N :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_N";
 mc_N·m :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_N·m";
 mc_N·cm :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_N·cm";
 mc_N·mm :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_N·mm";
 mc_Oe :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Length-60000} @ "McdStaticUnitSystemSI.mc_Oe";
 mc_Pa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_Pa";
 mc_MPa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_MPa";
 mc_R :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_R";
 mc_°R :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemSI.mc_°R";
 mc_S :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 180000} @ "McdStaticUnitSystemSI.mc_S";
 mc_Sv :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_Sv";
 mc_T :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_T";
 mc_V :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_V";
 mc_amp :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_amp";
 mc_arcsec :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_arcsec";
 mc_arcmin :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_arcmin";
 mc_atm :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_atm";
 mc_bar :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_bar";
 mc_barn :: Mpl.Value {McdStaticUnitSystem.Length 120000} @ "McdStaticUnitSystemSI.mc_barn";
 mc_bhp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_bhp";
 mc_bohr :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_bohr";
 mc_a_2E_0 :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_a_2E_0";
 mc_c :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_c";
 mc_cal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_cal";
 mc_cal15 :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_cal15";
 mc_cal20 :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_cal20";
 mc_cd :: Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemSI.mc_cd";
 mc_cm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_cm";
 mc_coul :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_coul";
 mc_deg :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_deg";
 mc_° :: Mpl.Value {} @ "McdStaticUnitSystemSI.mc_°";
 mc_dyne :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_dyne";
 mc_ehp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_ehp";
 mc_erg :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_erg";
 mc_farad :: Mpl.Value {McdStaticUnitSystem.Current 120000 McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Mass-60000 McdStaticUnitSystem.Time 240000} @ "McdStaticUnitSystemSI.mc_farad";
 mc_fl_5F_oz :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_fl_5F_oz";
 mc_fortnight :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_fortnight";
 mc_W·s :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_W·s";
 mc_W·hr :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_W·hr";
 mc_kW·hr :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kW·hr";
 mc_furlong :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_furlong";
 mc_g :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_g";
 mc_gal :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_gal";
 mc_galUK :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_galUK";
 mc_mC :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_mC";
 mc_μC :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_μC";
 mc_nC :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_nC";
 mc_pC :: Mpl.Value {McdStaticUnitSystem.Current 60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_pC";
 mc_cubit :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_cubit";
 mc_day :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_day";
 mc_dcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_dcal";
 mc_ft :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_ft";
 mc_ft·lbf :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_ft·lbf";
 mc_ft·kip :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_ft·kip";
 mc_in·lbf :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_in·lbf";
 mc_in·kip :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_in·kip";
 mc_in·ozf :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_in·ozf";
 mc_henry :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_henry";
 mc_hhp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_hhp";
 mc_hp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_hp";
 mc_hpUK :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_hpUK";
 mc_hr :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemSI.mc_hr";
 mc_in :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_in";
 mc_in_5F_Hg :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_in_5F_Hg";
 mc_joule :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_joule";
 mc_kN·m :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kN·m";
 mc_kN·cm :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kN·cm";
 mc_kN·mm :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kN·mm";
 mc_kV :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_kV";
 mc_kW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_kW";
 mc_kΩ :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_kΩ";
 mc_kpi :: Mpl.Value {McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kpi";
 mc_plf :: Mpl.Value {McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_plf";
 mc_km :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_km";
 mc_knot :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_knot";
 mc_kPa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kPa";
 mc_kph :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_kph";
 mc_gpm :: Mpl.Value {McdStaticUnitSystem.Length 180000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_gpm";
 mc_cfm :: Mpl.Value {McdStaticUnitSystem.Length 180000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_cfm";
 mc_mgd :: Mpl.Value {McdStaticUnitSystem.Length 180000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_mgd";
 mc_gauss :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_gauss";
 mc_gm :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_gm";
 mc_hectare :: Mpl.Value {McdStaticUnitSystem.Length 120000} @ "McdStaticUnitSystemSI.mc_hectare";
 mc_kA :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_kA";
 mc_kHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_kHz";
 mc_kN :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kN";
 mc_MN :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_MN";
 mc_GN :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_GN";
 mc_TN :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_TN";
 mc_tonnef :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_tonnef";
 mc_katal :: Mpl.Value {McdStaticUnitSystem.Substance 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemSI.mc_katal";
 mc_kcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kcal";
 mc_kg :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_kg";
 mc_kgf :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kgf";
 mc_kip :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_kip";
 mc_klf :: Mpl.Value {McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_klf";
 mc_mV :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_mV";
 mc_μV :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_μV";
 mc_nV :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_nV";
 mc_pV :: Mpl.Value {McdStaticUnitSystem.Current-60000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_pV";
 mc_mW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_mW";
 mc_μW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_μW";
 mc_nW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemSI.mc_nW";
 mc_lm :: Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemSI.mc_lm";
 mc_lx :: Mpl.Value {McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemSI.mc_lx";
 mc_m :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemSI.mc_m";
 mc_mA :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemSI.mc_mA";
 mc_mBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_mBTU";
 mc_mH :: Mpl.Value {McdStaticUnitSystem.Current-120000 McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_mH";
 mc_mL :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_mL";
 mc_ksf :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_ksf";
 mc_ksi :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_ksi";
 mc_lb :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_lb";
 mc_lbm :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemSI.mc_lbm";
 mc_lbf :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemSI.mc_lbf";
 mc_liter :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_liter";
 mc_l :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemSI.mc_l";
 mc_SIUnitsOf :: McdRun.V {a}->McdRun.V {a} @ "McdStaticUnitSystemSI.mc_SIUnitsOf";
 mc_UnitsOf :: McdRun.V {a}->McdRun.V {a} @ "McdStaticUnitSystemSI.mc_UnitsOf";
 import McdStaticUnitSystem;
 import McdRun;
 import Mpl;
 import MplPrelude;
}
-- implementation
mc_s = McdRun.values.srv (McdStaticUnitSystem.values.second)
mc_mole = McdRun.values.srv (McdStaticUnitSystem.values.mole)
mc_m = McdRun.values.srv (McdStaticUnitSystem.values.meter)
mc_lx = McdRun.values.srv (McdStaticUnitSystem.values.lux)
mc_kg = McdRun.values.srv (McdStaticUnitSystem.values.kilogram)
mc_cd = McdRun.values.srv (McdStaticUnitSystem.values.candela)
mc_Ω = McdRun.values.srv (McdStaticUnitSystem.values.ohm)
mc_Wb = McdRun.values.srv (McdStaticUnitSystem.values.weber)
mc_W = McdRun.values.srv (McdStaticUnitSystem.values.watt)
mc_V = McdRun.values.srv (McdStaticUnitSystem.values.volt)
mc_T = McdRun.values.srv (McdStaticUnitSystem.values.tesla)
mc_S = McdRun.values.srv (McdStaticUnitSystem.values.siemens)
mc_Pa = McdRun.values.srv (McdStaticUnitSystem.values.pascal)
mc_N = McdRun.values.srv (McdStaticUnitSystem.values.newton)
mc_L = McdRun.values.srv (McdStaticUnitSystem.values.liter)
mc_K = McdRun.values.srv (McdStaticUnitSystem.values.kelvin)
mc_J = McdRun.values.srv (McdStaticUnitSystem.values.joule)
mc_H = McdRun.values.srv (McdStaticUnitSystem.values.henry)
mc_F = McdRun.values.srv (McdStaticUnitSystem.values.farad)
mc_C = McdRun.values.srv (McdStaticUnitSystem.values.coulomb)
mc_A = McdRun.values.srv (McdStaticUnitSystem.values.ampere)
mc_¤ = McdRun.values.srv (McdStaticUnitSystem.values.basecurrency)
system = McdRun.values.unit_system_group (McdRun.values.unit_system (["SI", 0, "7FE6716D-2E40-43A6-9E41-0DF66E111D32", new Cons (McdRun.values.unit (["kilogram", "kg", "Mass", mc_kg, "Value {Mass 60000}"]), new Cons (McdRun.values.unit (["meter", "m", "Length", mc_m, "Value {Length 60000}"]), new Cons (McdRun.values.unit (["second", "s", "Time", mc_s, "Value {Time 60000}"]), new Cons (McdRun.values.unit (["ampere", "A", "Current", mc_A, "Value {Current 60000}"]), new Cons (McdRun.values.unit (["kelvin", "K", "Temperature", mc_K, "Value {Temperature 60000}"]), new Cons (McdRun.values.unit (["candela", "cd", "Luminosity", mc_cd, "Value {Luminosity 60000}"]), new Cons (McdRun.values.unit (["mole", "mol", "Substance", mc_mole, "Value {Substance 60000}"]), new Cons (McdRun.values.unit (["base_currency", "¤", "Money", mc_¤, "Value {Money 60000}"]), null)))))))), new Cons (McdRun.values.unit (["coulomb", "C", "Charge", mc_C, "Value {Current 60000 Time 60000}"]), new Cons (McdRun.values.unit (["farad", "F", "Capacitance", mc_F, "Value {Current 120000 Length-120000 Mass-60000 Time 240000}"]), new Cons (McdRun.values.unit (["henry", "H", "Inductance", mc_H, "Value {Current-120000 Length 120000 Mass 60000 Time-120000}"]), new Cons (McdRun.values.unit (["joule", "J", "Energy", mc_J, "Value {Length 120000 Mass 60000 Time-120000}"]), new Cons (McdRun.values.unit (["liter", "L", "Volume", mc_L, "Value {Length 180000}"]), new Cons (McdRun.values.unit (["lux", "lx", "Illuminance", mc_lx, "Value {Length-120000 Luminosity 60000}"]), new Cons (McdRun.values.unit (["newton", "N", "Force", mc_N, "Value {Length 60000 Mass 60000 Time-120000}"]), new Cons (McdRun.values.unit (["ohm", "Ω", "Resistance", mc_Ω, "Value {Current-120000 Length 120000 Mass 60000 Time-180000}"]), new Cons (McdRun.values.unit (["pascal", "Pa", "Pressure", mc_Pa, "Value {Length-60000 Mass 60000 Time-120000}"]), new Cons (McdRun.values.unit (["siemens", "S", "Conductance", mc_S, "Value {Current 120000 Length-120000 Mass-60000 Time 180000}"]), new Cons (McdRun.values.unit (["tesla", "T", "Magnetic Flux Density", mc_T, "Value {Current-60000 Mass 60000 Time-120000}"]), new Cons (McdRun.values.unit (["watt", "W", "Power", mc_W, "Value {Length 120000 Mass 60000 Time-180000}"]), new Cons (McdRun.values.unit (["weber", "Wb", "Magnetic Flux", mc_Wb, "Value {Current-60000 Length 120000 Mass 60000 Time-120000}"]), new Cons (McdRun.values.unit (["volt", "V", "Potential", mc_V, "Value {Current-60000 Length 120000 Mass 60000 Time-180000}"]), null))))))))))))))]))
mc_σ = McdRun.values.srv (McdStaticUnitSystem.values.sigma)
mc_Φ_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.phi_0)
mc_α = McdRun.values.srv (McdStaticUnitSystem.values.alpha)
mc_R_2E_∞ = McdRun.values.srv (McdStaticUnitSystem.values.rydberg_constant)
mc_R_2E_gas = McdRun.values.srv (McdStaticUnitSystem.values.molar_gas_constant)
mc_N_2E_A = McdRun.values.srv (McdStaticUnitSystem.values.avogadro_constant)
mc_m_2E_u = McdRun.values.srv (McdStaticUnitSystem.values.m_u)
mc_k = McdRun.values.srv (McdStaticUnitSystem.values.k)
mc_ℏ = McdRun.values.srv (McdStaticUnitSystem.values.h_bar)
mc_h = McdRun.values.srv (McdStaticUnitSystem.values.h)
mc_e_2E_c = McdRun.values.srv (McdStaticUnitSystem.values.e_c)
mc_dpi = McdRun.values.srv (McdStaticUnitSystem.values.dpi)
mc_yr = McdRun.values.srv (McdStaticUnitSystem.values.year)
mc_yd = McdRun.values.srv (McdStaticUnitSystem.values.yard)
mc_weber = McdRun.values.srv (McdStaticUnitSystem.values.weber)
mc_watt = McdRun.values.srv (McdStaticUnitSystem.values.watt)
mc_volt = McdRun.values.srv (McdStaticUnitSystem.values.volt)
mc_torr = McdRun.values.srv (McdStaticUnitSystem.values.torr)
mc_tonne = McdRun.values.srv (McdStaticUnitSystem.values.tonne)
mc_tonf = McdRun.values.srv ((2000.0 *. McdStaticUnitSystem.values.pound_force))
mc_ton = McdRun.values.srv (McdStaticUnitSystem.values.ton)
mc_tesla = McdRun.values.srv (McdStaticUnitSystem.values.tesla)
mc_tcal = McdRun.values.srv (McdStaticUnitSystem.values.calorie_thermo)
mc_tBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_thermo)
mc_Smoot = McdRun.values.srv (McdStaticUnitSystem.values.smoot)
mc_stokes = McdRun.values.srv (McdStaticUnitSystem.values.stokes)
mc_sr = McdRun.values.srv (McdStaticUnitSystem.values.steradian)
mc_slug = McdRun.values.srv (McdStaticUnitSystem.values.slug)
mc_siemens = McdRun.values.srv (McdStaticUnitSystem.values.siemens)
mc_sec = McdRun.values.srv (McdStaticUnitSystem.values.second)
mc_rpm = McdRun.values.srv ((McdStaticUnitSystem.values.revolution /. McdStaticUnitSystem.values.minute))
mc_rev = McdRun.values.srv (McdStaticUnitSystem.values.revolution)
mc_rad = McdRun.values.srv (McdStaticUnitSystem.values.radian)
mc_psf = McdRun.values.srv ((McdStaticUnitSystem.values.pound_force /. McdStaticUnitSystem.values.square (McdStaticUnitSystem.values.foot)))
mc_psi = McdRun.values.srv (McdStaticUnitSystem.values.psi)
mc_poise = McdRun.values.srv (McdStaticUnitSystem.values.poise)
mc_pci = McdRun.values.srv ((McdStaticUnitSystem.values.pound_force /. McdStaticUnitSystem.values.cubic (McdStaticUnitSystem.values.inch)))
mc_pcf = McdRun.values.srv ((McdStaticUnitSystem.values.pound_force /. McdStaticUnitSystem.values.cubic (McdStaticUnitSystem.values.foot)))
mc_pF = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.farad))
mc_ozf = McdRun.values.srv (McdStaticUnitSystem.values.ounce_force)
mc_oz = McdRun.values.srv (McdStaticUnitSystem.values.ounce)
mc_ohm = McdRun.values.srv (McdStaticUnitSystem.values.ohm)
mc_oersted = McdRun.values.srv (McdStaticUnitSystem.values.oersted)
mc_nmi = McdRun.values.srv (McdStaticUnitSystem.values.nautical_mile)
mc_nm = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.meter))
mc_nF = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.farad))
mc_newton = McdRun.values.srv (McdStaticUnitSystem.values.newton)
mc_ms = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.second))
mc_mph = McdRun.values.srv ((McdStaticUnitSystem.values.mile /. McdStaticUnitSystem.values.hour))
mc_mol = McdRun.values.srv (McdStaticUnitSystem.values.mole)
mc_mm = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.meter))
mc_min = McdRun.values.srv (McdStaticUnitSystem.values.minute)
mc_mile = McdRun.values.srv (McdStaticUnitSystem.values.mile)
mc_mil = McdRun.values.srv (McdStaticUnitSystem.values.mil)
mc_mi = McdRun.values.srv (McdStaticUnitSystem.values.mile)
mc_micron = McdRun.values.srv (McdStaticUnitSystem.values.micron)
mc_mhp = McdRun.values.srv (McdStaticUnitSystem.values.hp_metric)
mc_mho = McdRun.values.srv (McdStaticUnitSystem.values.siemens)
mc_mg = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.gram))
mc_mcal = McdRun.values.srv (McdStaticUnitSystem.values.calorie_mean)
mc_pW = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.watt))
mc_nW = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.watt))
mc_μW = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.watt))
mc_mW = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.watt))
mc_pV = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.volt))
mc_nV = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.volt))
mc_μV = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.volt))
mc_mV = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.volt))
mc_mL = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.liter))
mc_mH = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.henry))
mc_mBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_mean)
mc_mA = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.ampere))
mc_lm = McdRun.values.srv (McdStaticUnitSystem.values.lumen)
mc_l = McdRun.values.srv (McdStaticUnitSystem.values.liter)
mc_liter = McdRun.values.srv (McdStaticUnitSystem.values.liter)
mc_lbf = McdRun.values.srv (McdStaticUnitSystem.values.pound_force)
mc_lbm = McdRun.values.srv (McdStaticUnitSystem.values.pound)
mc_lb = McdRun.values.srv (McdStaticUnitSystem.values.pound)
mc_ksi = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force) /. McdStaticUnitSystem.values.square (McdStaticUnitSystem.values.inch)))
mc_ksf = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force) /. McdStaticUnitSystem.values.square (McdStaticUnitSystem.values.foot)))
mc_kph = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.meter) /. McdStaticUnitSystem.values.hour))
mc_kPa = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pascal))
mc_knot = McdRun.values.srv (McdStaticUnitSystem.values.knot)
mc_km = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.meter))
mc_plf = McdRun.values.srv ((McdStaticUnitSystem.values.pound_force /. McdStaticUnitSystem.values.foot))
mc_kpi = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force) /. McdStaticUnitSystem.values.inch))
mc_klf = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force) /. McdStaticUnitSystem.values.foot))
mc_kip = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force))
mc_kgf = McdRun.values.srv (McdStaticUnitSystem.values.kilogram_force)
mc_kcal = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.calorie))
mc_katal = McdRun.values.srv (McdStaticUnitSystem.values.katal)
mc_kΩ = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.ohm))
mc_kW = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.watt))
mc_kV = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.volt))
mc_kN·mm = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.newton) *. McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.meter)))
mc_kN·cm = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.newton) *. McdStaticUnitSystem.values.centi (McdStaticUnitSystem.values.meter)))
mc_kN·m = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.newton) *. McdStaticUnitSystem.values.meter))
mc_tonnef = McdRun.values.srv (McdStaticUnitSystem.values.tonne_force)
mc_TN = McdRun.values.srv (McdStaticUnitSystem.values.tera (McdStaticUnitSystem.values.newton))
mc_GN = McdRun.values.srv (McdStaticUnitSystem.values.giga (McdStaticUnitSystem.values.newton))
mc_MN = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.newton))
mc_kN = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.newton))
mc_kHz = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.hertz))
mc_kA = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.ampere))
mc_joule = McdRun.values.srv (McdStaticUnitSystem.values.joule)
mc_in_5F_Hg = McdRun.values.srv (McdStaticUnitSystem.values.inches_mercury)
mc_in = McdRun.values.srv (McdStaticUnitSystem.values.inch)
mc_hr = McdRun.values.srv (McdStaticUnitSystem.values.hour)
mc_hpUK = McdRun.values.srv (McdStaticUnitSystem.values.hp_uk)
mc_hp = McdRun.values.srv (McdStaticUnitSystem.values.hp)
mc_hhp = McdRun.values.srv (McdStaticUnitSystem.values.hp_water)
mc_henry = McdRun.values.srv (McdStaticUnitSystem.values.henry)
mc_hectare = McdRun.values.srv (McdStaticUnitSystem.values.hectare)
mc_gm = McdRun.values.srv (McdStaticUnitSystem.values.gram)
mc_gauss = McdRun.values.srv (McdStaticUnitSystem.values.gauss)
mc_mgd = McdRun.values.srv ((McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.gallon) /. McdStaticUnitSystem.values.day))
mc_cfm = McdRun.values.srv ((McdStaticUnitSystem.values.cubic (McdStaticUnitSystem.values.foot) /. McdStaticUnitSystem.values.minute))
mc_gpm = McdRun.values.srv ((McdStaticUnitSystem.values.gallon /. McdStaticUnitSystem.values.minute))
mc_galUK = McdRun.values.srv (McdStaticUnitSystem.values.gallon_uk)
mc_gal = McdRun.values.srv (McdStaticUnitSystem.values.gallon)
mc_g = McdRun.values.srv (McdStaticUnitSystem.values.g)
mc_furlong = McdRun.values.srv (McdStaticUnitSystem.values.furlong)
mc_kW·hr = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.watt) *. McdStaticUnitSystem.values.hour))
mc_W·hr = McdRun.values.srv ((McdStaticUnitSystem.values.watt *. McdStaticUnitSystem.values.hour))
mc_W·s = McdRun.values.srv ((McdStaticUnitSystem.values.watt *. McdStaticUnitSystem.values.second))
mc_in·ozf = McdRun.values.srv ((McdStaticUnitSystem.values.inch *. McdStaticUnitSystem.values.ounce_force))
mc_in·kip = McdRun.values.srv ((McdStaticUnitSystem.values.inch *. McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force)))
mc_in·lbf = McdRun.values.srv ((McdStaticUnitSystem.values.inch *. McdStaticUnitSystem.values.pound_force))
mc_ft·kip = McdRun.values.srv ((McdStaticUnitSystem.values.foot *. McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force)))
mc_ft·lbf = McdRun.values.srv ((McdStaticUnitSystem.values.foot *. McdStaticUnitSystem.values.pound_force))
mc_ft = McdRun.values.srv (McdStaticUnitSystem.values.foot)
mc_fortnight = McdRun.values.srv (McdStaticUnitSystem.values.fortnight)
mc_fl_5F_oz = McdRun.values.srv (McdStaticUnitSystem.values.fluid_ounce)
mc_farad = McdRun.values.srv (McdStaticUnitSystem.values.farad)
mc_erg = McdRun.values.srv (McdStaticUnitSystem.values.erg)
mc_ehp = McdRun.values.srv (McdStaticUnitSystem.values.hp_electric)
mc_dyne = McdRun.values.srv (McdStaticUnitSystem.values.dyne)
mc_° = McdRun.values.srv (McdStaticUnitSystem.values.degree)
mc_deg = McdRun.values.srv (McdStaticUnitSystem.values.degree)
mc_dcal = McdRun.values.srv (McdStaticUnitSystem.values.calorie_diet)
mc_day = McdRun.values.srv (McdStaticUnitSystem.values.day)
mc_cubit = McdRun.values.srv (McdStaticUnitSystem.values.cubit)
mc_pC = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.coulomb))
mc_nC = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.coulomb))
mc_μC = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.coulomb))
mc_mC = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.coulomb))
mc_coul = McdRun.values.srv (McdStaticUnitSystem.values.coulomb)
mc_cm = McdRun.values.srv (McdStaticUnitSystem.values.centi (McdStaticUnitSystem.values.meter))
mc_cal20 = McdRun.values.srv (McdStaticUnitSystem.values.calorie_20C)
mc_cal15 = McdRun.values.srv (McdStaticUnitSystem.values.calorie_15C)
mc_cal = McdRun.values.srv (McdStaticUnitSystem.values.calorie)
mc_c = McdRun.values.srv (McdStaticUnitSystem.values.c)
mc_a_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.bohr)
mc_bohr = McdRun.values.srv (McdStaticUnitSystem.values.bohr)
mc_bhp = McdRun.values.srv (McdStaticUnitSystem.values.hp_boiler)
mc_barn = McdRun.values.srv (McdStaticUnitSystem.values.barn)
mc_bar = McdRun.values.srv (McdStaticUnitSystem.values.bar)
mc_atm = McdRun.values.srv (McdStaticUnitSystem.values.atmosphere)
mc_arcmin = McdRun.values.srv (McdStaticUnitSystem.values.arcminute)
mc_arcsec = McdRun.values.srv (McdStaticUnitSystem.values.arcsecond)
mc_amp = McdRun.values.srv (McdStaticUnitSystem.values.ampere)
mc_acre·ft = McdRun.values.srv ((McdStaticUnitSystem.values.acre *. McdStaticUnitSystem.values.foot))
mc_acre = McdRun.values.srv (McdStaticUnitSystem.values.acre)
mc_ps = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.second))
mc_ns = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.second))
mc_μs = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.second))
mc_μm = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.meter))
mc_μH = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.henry))
mc_μF = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.farad))
mc_pA = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.ampere))
mc_nA = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.ampere))
mc_μA = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.ampere))
mc_Sv = McdRun.values.srv (McdStaticUnitSystem.values.sievert)
mc_°R = McdRun.values.srv (McdStaticUnitSystem.values.rankine)
mc_R = McdRun.values.srv (McdStaticUnitSystem.values.rankine)
mc_MPa = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.pascal))
mc_Oe = McdRun.values.srv (McdStaticUnitSystem.values.oersted)
mc_N·mm = McdRun.values.srv ((McdStaticUnitSystem.values.newton *. McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.meter)))
mc_N·cm = McdRun.values.srv ((McdStaticUnitSystem.values.newton *. McdStaticUnitSystem.values.centi (McdStaticUnitSystem.values.meter)))
mc_N·m = McdRun.values.srv ((McdStaticUnitSystem.values.newton *. McdStaticUnitSystem.values.meter))
mc_MW = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.watt))
mc_MΩ = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.ohm))
mc_MHz = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.hertz))
mc_KΩ = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.ohm))
mc_KV = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.volt))
mc_KHz = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.hertz))
mc_KA = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.ampere))
mc_IBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_iso)
mc_Hza = McdRun.values.srv (McdStaticUnitSystem.values.angular_hertz)
mc_Hz = McdRun.values.srv (McdStaticUnitSystem.values.hertz)
mc_Gy = McdRun.values.srv (McdStaticUnitSystem.values.gray)
mc_GPa = McdRun.values.srv (McdStaticUnitSystem.values.giga (McdStaticUnitSystem.values.pascal))
mc_GHz = McdRun.values.srv (McdStaticUnitSystem.values.giga (McdStaticUnitSystem.values.hertz))
mc_G = McdRun.values.srv (McdStaticUnitSystem.values.gauss)
mc_Δ℉ = McdRun.values.srv (McdStaticUnitSystem.values.rankine)
mc_Δ℃ = McdRun.values.srv (McdStaticUnitSystem.values.kelvin)
mc_Δ°F = McdRun.values.srv (McdStaticUnitSystem.values.rankine)
mc_Δ°C = McdRun.values.srv (McdStaticUnitSystem.values.kelvin)
mc_CBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_canadian)
mc_Bq = McdRun.values.srv (McdStaticUnitSystem.values.becquerel)
mc_BTU15 = McdRun.values.srv (McdStaticUnitSystem.values.btu_15C)
mc_Btu = McdRun.values.srv (McdStaticUnitSystem.values.btu)
mc_BTU = McdRun.values.srv (McdStaticUnitSystem.values.btu)
mc_Angstrom = McdRun.values.srv (McdStaticUnitSystem.values.angstrom)
mc_ε_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.epsilon0)
mc_μ_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.mu0)
mc_UnitsOf value = Mcd.NumVal.One
mc_SIUnitsOf value = Mcd.NumVal.One
