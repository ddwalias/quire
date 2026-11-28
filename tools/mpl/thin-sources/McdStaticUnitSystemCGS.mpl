-- CGS unit catalogue, normalized to the Mpl source forms.
module McdStaticUnitSystemCGS (McdStaticUnitSystemCGS.mc_statsiemens,McdStaticUnitSystemCGS.mc_stattesla,McdStaticUnitSystemCGS.mc_statvolt,McdStaticUnitSystemCGS.mc_statweber,McdStaticUnitSystemCGS.mc_volt,McdStaticUnitSystemCGS.system,McdStaticUnitSystemCGS.op_M,McdStaticUnitSystemCGS.op_L,McdStaticUnitSystemCGS.op_T,McdStaticUnitSystemCGS.op_Q,McdStaticUnitSystemCGS.op_K,McdStaticUnitSystemCGS.op_C,McdStaticUnitSystemCGS.op_S,McdStaticUnitSystemCGS.op_D,McdStaticUnitSystemCGS.mc_SIUnitsOf,McdStaticUnitSystemCGS.mc_UnitsOf,McdStaticUnitSystemCGS.mc_¤,McdStaticUnitSystemCGS.mc_μm,McdStaticUnitSystemCGS.mc_μs,McdStaticUnitSystemCGS.mc_Angstrom,McdStaticUnitSystemCGS.mc_Δ℉,McdStaticUnitSystemCGS.mc_GHz,McdStaticUnitSystemCGS.mc_GPa,McdStaticUnitSystemCGS.mc_Gy,McdStaticUnitSystemCGS.mc_Hz,McdStaticUnitSystemCGS.mc_Hza,McdStaticUnitSystemCGS.mc_IBTU,McdStaticUnitSystemCGS.mc_J,McdStaticUnitSystemCGS.mc_BTU,McdStaticUnitSystemCGS.mc_Btu,McdStaticUnitSystemCGS.mc_BTU15,McdStaticUnitSystemCGS.mc_Bq,McdStaticUnitSystemCGS.mc_CBTU,McdStaticUnitSystemCGS.mc_Δ°C,McdStaticUnitSystemCGS.mc_Δ°F,McdStaticUnitSystemCGS.mc_Δ℃,McdStaticUnitSystemCGS.mc_K,McdStaticUnitSystemCGS.mc_KHz,McdStaticUnitSystemCGS.mc_L,McdStaticUnitSystemCGS.mc_MHz,McdStaticUnitSystemCGS.mc_MPa,McdStaticUnitSystemCGS.mc_N,McdStaticUnitSystemCGS.mc_Pa,McdStaticUnitSystemCGS.mc_bhp,McdStaticUnitSystemCGS.mc_bohr,McdStaticUnitSystemCGS.mc_c,McdStaticUnitSystemCGS.mc_c_5F_,McdStaticUnitSystemCGS.mc_cal,McdStaticUnitSystemCGS.mc_cal15,McdStaticUnitSystemCGS.mc_cal20,McdStaticUnitSystemCGS.mc_R,McdStaticUnitSystemCGS.mc_°R,McdStaticUnitSystemCGS.mc_Sv,McdStaticUnitSystemCGS.mc_W,McdStaticUnitSystemCGS.mc_a_2E_0,McdStaticUnitSystemCGS.mc_acre,McdStaticUnitSystemCGS.mc_atm,McdStaticUnitSystemCGS.mc_barn,McdStaticUnitSystemCGS.mc_barye,McdStaticUnitSystemCGS.mc_ehp,McdStaticUnitSystemCGS.mc_erg,McdStaticUnitSystemCGS.mc_fl_5F_oz,McdStaticUnitSystemCGS.mc_fortnight,McdStaticUnitSystemCGS.mc_furlong,McdStaticUnitSystemCGS.mc_ft,McdStaticUnitSystemCGS.mc_g,McdStaticUnitSystemCGS.mc_gal,McdStaticUnitSystemCGS.mc_galUK,McdStaticUnitSystemCGS.mc_gm,McdStaticUnitSystemCGS.mc_gpm,McdStaticUnitSystemCGS.mc_hectare,McdStaticUnitSystemCGS.mc_hhp,McdStaticUnitSystemCGS.mc_hp,McdStaticUnitSystemCGS.mc_hpUK,McdStaticUnitSystemCGS.mc_hr,McdStaticUnitSystemCGS.mc_in,McdStaticUnitSystemCGS.mc_in_5F_Hg,McdStaticUnitSystemCGS.mc_joule,McdStaticUnitSystemCGS.mc_kHz,McdStaticUnitSystemCGS.mc_kN,McdStaticUnitSystemCGS.mc_kW,McdStaticUnitSystemCGS.mc_katal,McdStaticUnitSystemCGS.mc_kcal,McdStaticUnitSystemCGS.mc_cd,McdStaticUnitSystemCGS.mc_cm,McdStaticUnitSystemCGS.mc_cubit,McdStaticUnitSystemCGS.mc_day,McdStaticUnitSystemCGS.mc_dcal,McdStaticUnitSystemCGS.mc_deg,McdStaticUnitSystemCGS.mc_°,McdStaticUnitSystemCGS.mc_dyne,McdStaticUnitSystemCGS.mc_mL,McdStaticUnitSystemCGS.mc_mcal,McdStaticUnitSystemCGS.mc_mg,McdStaticUnitSystemCGS.mc_mW,McdStaticUnitSystemCGS.mc_mhp,McdStaticUnitSystemCGS.mc_mi,McdStaticUnitSystemCGS.mc_micron,McdStaticUnitSystemCGS.mc_lb,McdStaticUnitSystemCGS.mc_lbm,McdStaticUnitSystemCGS.mc_lbf,McdStaticUnitSystemCGS.mc_liter,McdStaticUnitSystemCGS.mc_lm,McdStaticUnitSystemCGS.mc_lx,McdStaticUnitSystemCGS.mc_m,McdStaticUnitSystemCGS.mc_mBTU,McdStaticUnitSystemCGS.mc_rad,McdStaticUnitSystemCGS.mc_sec,McdStaticUnitSystemCGS.mc_s,McdStaticUnitSystemCGS.mc_slug,McdStaticUnitSystemCGS.mc_sr,McdStaticUnitSystemCGS.mc_stokes,McdStaticUnitSystemCGS.mc_tBTU,McdStaticUnitSystemCGS.mc_tcal,McdStaticUnitSystemCGS.mc_kg,McdStaticUnitSystemCGS.mc_kgf,McdStaticUnitSystemCGS.mc_kip,McdStaticUnitSystemCGS.mc_km,McdStaticUnitSystemCGS.mc_kph,McdStaticUnitSystemCGS.mc_ksi,McdStaticUnitSystemCGS.mc_mil,McdStaticUnitSystemCGS.mc_mile,McdStaticUnitSystemCGS.mc_min,McdStaticUnitSystemCGS.mc_mm,McdStaticUnitSystemCGS.mc_mol,McdStaticUnitSystemCGS.mc_mole,McdStaticUnitSystemCGS.mc_mph,McdStaticUnitSystemCGS.mc_ms,McdStaticUnitSystemCGS.mc_newton,McdStaticUnitSystemCGS.mc_nm,McdStaticUnitSystemCGS.mc_nmi,McdStaticUnitSystemCGS.mc_oz,McdStaticUnitSystemCGS.mc_poise,McdStaticUnitSystemCGS.mc_psi,McdStaticUnitSystemCGS.mc_psf,McdStaticUnitSystemCGS.mc_μ_2E_0,McdStaticUnitSystemCGS.mc_ε_2E_0,McdStaticUnitSystemCGS.mc_Φ_2E_0,McdStaticUnitSystemCGS.mc_e_2E_c,McdStaticUnitSystemCGS.mc_μA,McdStaticUnitSystemCGS.mc_μF,McdStaticUnitSystemCGS.mc_μH,McdStaticUnitSystemCGS.mc_KA,McdStaticUnitSystemCGS.mc_KV,McdStaticUnitSystemCGS.mc_MΩ,McdStaticUnitSystemCGS.mc_MW,McdStaticUnitSystemCGS.mc_Mx,McdStaticUnitSystemCGS.mc_amp,McdStaticUnitSystemCGS.mc_coul,McdStaticUnitSystemCGS.mc_ton,McdStaticUnitSystemCGS.mc_tonne,McdStaticUnitSystemCGS.mc_torr,McdStaticUnitSystemCGS.mc_watt,McdStaticUnitSystemCGS.mc_yd,McdStaticUnitSystemCGS.mc_yr,McdStaticUnitSystemCGS.mc_dpi,McdStaticUnitSystemCGS.mc_rev,McdStaticUnitSystemCGS.mc_ft·lbf,McdStaticUnitSystemCGS.mc_in·lbf,McdStaticUnitSystemCGS.mc_in·ozf,McdStaticUnitSystemCGS.mc_N·m,McdStaticUnitSystemCGS.mc_N·cm,McdStaticUnitSystemCGS.mc_N·mm,McdStaticUnitSystemCGS.mc_Ω,McdStaticUnitSystemCGS.mc_A,McdStaticUnitSystemCGS.mc_C,McdStaticUnitSystemCGS.mc_F,McdStaticUnitSystemCGS.mc_H,McdStaticUnitSystemCGS.mc_S,McdStaticUnitSystemCGS.mc_V,McdStaticUnitSystemCGS.mc_KΩ,McdStaticUnitSystemCGS.mc_esu,McdStaticUnitSystemCGS.mc_farad,McdStaticUnitSystemCGS.mc_gauss,McdStaticUnitSystemCGS.mc_henry,McdStaticUnitSystemCGS.mc_kΩ,McdStaticUnitSystemCGS.mc_kA,McdStaticUnitSystemCGS.mc_kV,McdStaticUnitSystemCGS.mc_mA,McdStaticUnitSystemCGS.mc_pF,McdStaticUnitSystemCGS.mc_siemens,McdStaticUnitSystemCGS.mc_statamp,McdStaticUnitSystemCGS.mc_statcoul,McdStaticUnitSystemCGS.mc_statfarad,McdStaticUnitSystemCGS.mc_cgs_accel,McdStaticUnitSystemCGS.mc_stathenry,McdStaticUnitSystemCGS.mc_statohm,McdStaticUnitSystemCGS.mc_mH,McdStaticUnitSystemCGS.mc_mV,McdStaticUnitSystemCGS.mc_maxwell,McdStaticUnitSystemCGS.mc_mho,McdStaticUnitSystemCGS.mc_nF,McdStaticUnitSystemCGS.mc_oersted,McdStaticUnitSystemCGS.mc_ohm,McdStaticUnitSystem.Length(..),McdStaticUnitSystem.Mass(..),McdStaticUnitSystem.Time(..),McdStaticUnitSystem.Temperature(..),McdStaticUnitSystem.Luminosity(..),McdStaticUnitSystem.Substance(..),McdStaticUnitSystem.Money(..)) where
{
 mc_statsiemens :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_statsiemens";
 mc_stattesla :: Mpl.Value {McdStaticUnitSystem.Length-30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_stattesla";
 mc_statvolt :: Mpl.Value {McdStaticUnitSystem.Length 30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_statvolt";
 mc_statweber :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_statweber";
 mc_volt :: Mpl.Value {McdStaticUnitSystem.Length 30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_volt";
 system :: McdRun.UnitSystemGroup @ "McdStaticUnitSystemCGS.system";
 op_M :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.op_M";
 op_L :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.op_L";
 op_T :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.op_T";
 op_Q :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.op_Q";
 op_K :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.op_K";
 op_C :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemCGS.op_C";
 op_S :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Substance 60000} @ "McdStaticUnitSystemCGS.op_S";
 op_D :: Mpl.Value {a}->Mpl.Value {a McdStaticUnitSystem.Money 60000} @ "McdStaticUnitSystemCGS.op_D";
 mc_SIUnitsOf :: McdRun.V {a}->McdRun.V {a} @ "McdStaticUnitSystemCGS.mc_SIUnitsOf";
 mc_UnitsOf :: McdRun.V {a}->McdRun.V {a} @ "McdStaticUnitSystemCGS.mc_UnitsOf";
 mc_¤ :: Mpl.Value {McdStaticUnitSystem.Money 60000} @ "McdStaticUnitSystemCGS.mc_¤";
 mc_μm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_μm";
 mc_μs :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_μs";
 mc_Angstrom :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_Angstrom";
 mc_Δ℉ :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_Δ℉";
 mc_GHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_GHz";
 mc_GPa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_GPa";
 mc_Gy :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_Gy";
 mc_Hz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_Hz";
 mc_Hza :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_Hza";
 mc_IBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_IBTU";
 mc_J :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_J";
 mc_BTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_BTU";
 mc_Btu :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_Btu";
 mc_BTU15 :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_BTU15";
 mc_Bq :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_Bq";
 mc_CBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_CBTU";
 mc_Δ°C :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_Δ°C";
 mc_Δ°F :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_Δ°F";
 mc_Δ℃ :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_Δ℃";
 mc_K :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_K";
 mc_KHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_KHz";
 mc_L :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemCGS.mc_L";
 mc_MHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_MHz";
 mc_MPa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_MPa";
 mc_N :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_N";
 mc_Pa :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_Pa";
 mc_bhp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_bhp";
 mc_bohr :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_bohr";
 mc_c :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_c";
 mc_c_5F_ :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_c_5F_";
 mc_cal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_cal";
 mc_cal15 :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_cal15";
 mc_cal20 :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_cal20";
 mc_R :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_R";
 mc_°R :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemCGS.mc_°R";
 mc_Sv :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_Sv";
 mc_W :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_W";
 mc_a_2E_0 :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_a_2E_0";
 mc_acre :: Mpl.Value {McdStaticUnitSystem.Length 120000} @ "McdStaticUnitSystemCGS.mc_acre";
 mc_atm :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_atm";
 mc_barn :: Mpl.Value {McdStaticUnitSystem.Length 120000} @ "McdStaticUnitSystemCGS.mc_barn";
 mc_barye :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_barye";
 mc_ehp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_ehp";
 mc_erg :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_erg";
 mc_fl_5F_oz :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemCGS.mc_fl_5F_oz";
 mc_fortnight :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_fortnight";
 mc_furlong :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_furlong";
 mc_ft :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_ft";
 mc_g :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_g";
 mc_gal :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemCGS.mc_gal";
 mc_galUK :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemCGS.mc_galUK";
 mc_gm :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_gm";
 mc_gpm :: Mpl.Value {McdStaticUnitSystem.Length 180000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_gpm";
 mc_hectare :: Mpl.Value {McdStaticUnitSystem.Length 120000} @ "McdStaticUnitSystemCGS.mc_hectare";
 mc_hhp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_hhp";
 mc_hp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_hp";
 mc_hpUK :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_hpUK";
 mc_hr :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_hr";
 mc_in :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_in";
 mc_in_5F_Hg :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_in_5F_Hg";
 mc_joule :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_joule";
 mc_kHz :: Mpl.Value {McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_kHz";
 mc_kN :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_kN";
 mc_kW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_kW";
 mc_katal :: Mpl.Value {McdStaticUnitSystem.Substance 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_katal";
 mc_kcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_kcal";
 mc_cd :: Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemCGS.mc_cd";
 mc_cm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_cm";
 mc_cubit :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_cubit";
 mc_day :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_day";
 mc_dcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_dcal";
 mc_deg :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_deg";
 mc_° :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_°";
 mc_dyne :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_dyne";
 mc_mL :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemCGS.mc_mL";
 mc_mcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_mcal";
 mc_mg :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_mg";
 mc_mW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_mW";
 mc_mhp :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_mhp";
 mc_mi :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_mi";
 mc_micron :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_micron";
 mc_lb :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_lb";
 mc_lbm :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_lbm";
 mc_lbf :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_lbf";
 mc_liter :: Mpl.Value {McdStaticUnitSystem.Length 180000} @ "McdStaticUnitSystemCGS.mc_liter";
 mc_lm :: Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemCGS.mc_lm";
 mc_lx :: Mpl.Value {McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemCGS.mc_lx";
 mc_m :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_m";
 mc_mBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_mBTU";
 mc_rad :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_rad";
 mc_sec :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_sec";
 mc_s :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_s";
 mc_slug :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_slug";
 mc_sr :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_sr";
 mc_stokes :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_stokes";
 mc_tBTU :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_tBTU";
 mc_tcal :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_tcal";
 mc_kg :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_kg";
 mc_kgf :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_kgf";
 mc_kip :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_kip";
 mc_km :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_km";
 mc_kph :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_kph";
 mc_ksi :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_ksi";
 mc_mil :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_mil";
 mc_mile :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_mile";
 mc_min :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_min";
 mc_mm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_mm";
 mc_mol :: Mpl.Value {McdStaticUnitSystem.Substance 60000} @ "McdStaticUnitSystemCGS.mc_mol";
 mc_mole :: Mpl.Value {McdStaticUnitSystem.Substance 60000} @ "McdStaticUnitSystemCGS.mc_mole";
 mc_mph :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_mph";
 mc_ms :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_ms";
 mc_newton :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_newton";
 mc_nm :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_nm";
 mc_nmi :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_nmi";
 mc_oz :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_oz";
 mc_poise :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_poise";
 mc_psi :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_psi";
 mc_psf :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_psf";
 mc_μ_2E_0 :: Mpl.Value {McdStaticUnitSystem.Length-120000 McdStaticUnitSystem.Time 120000} @ "McdStaticUnitSystemCGS.mc_μ_2E_0";
 mc_ε_2E_0 :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_ε_2E_0";
 mc_Φ_2E_0 :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_Φ_2E_0";
 mc_e_2E_c :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_e_2E_c";
 mc_μA :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_μA";
 mc_μF :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_μF";
 mc_μH :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 120000} @ "McdStaticUnitSystemCGS.mc_μH";
 mc_KA :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_KA";
 mc_KV :: Mpl.Value {McdStaticUnitSystem.Length 30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_KV";
 mc_MΩ :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_MΩ";
 mc_MW :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_MW";
 mc_Mx :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_Mx";
 mc_amp :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_amp";
 mc_coul :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_coul";
 mc_ton :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_ton";
 mc_tonne :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemCGS.mc_tonne";
 mc_torr :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_torr";
 mc_watt :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-180000} @ "McdStaticUnitSystemCGS.mc_watt";
 mc_yd :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_yd";
 mc_yr :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_yr";
 mc_dpi :: Mpl.Value {McdStaticUnitSystem.Length-60000} @ "McdStaticUnitSystemCGS.mc_dpi";
 mc_rev :: Mpl.Value {} @ "McdStaticUnitSystemCGS.mc_rev";
 mc_ft·lbf :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_ft·lbf";
 mc_in·lbf :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_in·lbf";
 mc_in·ozf :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_in·ozf";
 mc_N·m :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_N·m";
 mc_N·cm :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_N·cm";
 mc_N·mm :: Mpl.Value {McdStaticUnitSystem.Length 120000 McdStaticUnitSystem.Mass 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_N·mm";
 mc_Ω :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_Ω";
 mc_A :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_A";
 mc_C :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_C";
 mc_F :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_F";
 mc_H :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 120000} @ "McdStaticUnitSystemCGS.mc_H";
 mc_S :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_S";
 mc_V :: Mpl.Value {McdStaticUnitSystem.Length 30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_V";
 mc_KΩ :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_KΩ";
 mc_esu :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_esu";
 mc_farad :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_farad";
 mc_gauss :: Mpl.Value {McdStaticUnitSystem.Length-30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_gauss";
 mc_henry :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 120000} @ "McdStaticUnitSystemCGS.mc_henry";
 mc_kΩ :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_kΩ";
 mc_kA :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_kA";
 mc_kV :: Mpl.Value {McdStaticUnitSystem.Length 30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_kV";
 mc_mA :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_mA";
 mc_pF :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_pF";
 mc_siemens :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_siemens";
 mc_statamp :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_statamp";
 mc_statcoul :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_statcoul";
 mc_statfarad :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_statfarad";
 mc_cgs_accel :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-120000} @ "McdStaticUnitSystemCGS.mc_cgs_accel";
 mc_stathenry :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 120000} @ "McdStaticUnitSystemCGS.mc_stathenry";
 mc_statohm :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_statohm";
 mc_mH :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 120000} @ "McdStaticUnitSystemCGS.mc_mH";
 mc_mV :: Mpl.Value {McdStaticUnitSystem.Length 30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_mV";
 mc_maxwell :: Mpl.Value {McdStaticUnitSystem.Length 90000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_maxwell";
 mc_mho :: Mpl.Value {McdStaticUnitSystem.Length 60000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_mho";
 mc_nF :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemCGS.mc_nF";
 mc_oersted :: Mpl.Value {McdStaticUnitSystem.Length-30000 McdStaticUnitSystem.Mass 30000 McdStaticUnitSystem.Time-60000} @ "McdStaticUnitSystemCGS.mc_oersted";
 mc_ohm :: Mpl.Value {McdStaticUnitSystem.Length-60000 McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemCGS.mc_ohm";
 import McdStaticUnitSystem;
 import McdRun;
 import Mpl;
 import MplPrelude;
}
-- implementation
mc_statvolt = McdRun.values.srv (McdStaticUnitSystem.values.statvolt)
mc_statohm = McdRun.values.srv (McdStaticUnitSystem.values.statohm)
mc_stathenry = McdRun.values.srv (McdStaticUnitSystem.values.stathenry)
mc_cgs_accel = McdRun.values.srv (McdStaticUnitSystem.values.cgs_accel)
mc_statcoul = McdRun.values.srv (McdStaticUnitSystem.values.statcoulomb)
mc_statamp = McdRun.values.srv (McdStaticUnitSystem.values.statampere)
mc_oersted = McdRun.values.srv (McdStaticUnitSystem.values.cgs_oersted)
mc_gauss = McdRun.values.srv (McdStaticUnitSystem.values.cgs_gauss)
mc_stokes = McdRun.values.srv (McdStaticUnitSystem.values.stokes)
mc_poise = McdRun.values.srv (McdStaticUnitSystem.values.poise)
mc_mL = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.liter))
mc_lx = McdRun.values.srv (McdStaticUnitSystem.values.lux)
mc_katal = McdRun.values.srv (McdStaticUnitSystem.values.katal)
mc_erg = McdRun.values.srv (McdStaticUnitSystem.values.erg)
mc_dyne = McdRun.values.srv (McdStaticUnitSystem.values.dyne)
mc_barye = McdRun.values.srv (McdStaticUnitSystem.values.barye)
mc_W = McdRun.values.srv (McdStaticUnitSystem.values.watt)
mc_Sv = McdRun.values.srv (McdStaticUnitSystem.values.sievert)
mc_¤ = McdRun.values.srv (McdStaticUnitSystem.values.basecurrency)
mc_mole = McdRun.values.srv (McdStaticUnitSystem.values.mole)
mc_cd = McdRun.values.srv (McdStaticUnitSystem.values.candela)
mc_K = McdRun.values.srv (McdStaticUnitSystem.values.kelvin)
mc_sec = McdRun.values.srv (McdStaticUnitSystem.values.second)
mc_cm = McdRun.values.srv (McdStaticUnitSystem.values.centi (McdStaticUnitSystem.values.meter))
mc_gm = McdRun.values.srv (McdStaticUnitSystem.values.gram)
system = McdRun.values.unit_system_group (McdRun.values.unit_system (["CGS", 2, "DDD31661-FF82-4A97-B5C7-0C754A775401", new Cons (McdRun.values.unit (["gram", "gm", "Mass", mc_gm, "Value {Mass 60000}"]), new Cons (McdRun.values.unit (["centimeter", "cm", "Length", mc_cm, "Value {Length 60000}"]), new Cons (McdRun.values.unit (["second", "sec", "Time", mc_sec, "Value {Time 60000}"]), new Cons (McdRun.values.unit (["kelvin", "K", "Temperature", mc_K, "Value {Temperature 60000}"]), new Cons (McdRun.values.unit (["candela", "cd", "Luminosity", mc_cd, "Value {Luminosity 60000}"]), new Cons (McdRun.values.unit (["mole", "mole", "Substance", mc_mole, "Value {Substance 60000}"]), new Cons (McdRun.values.unit (["base_currency", "¤", "Money", mc_¤, "Value {Money 60000}"]), null))))))), VecBuilder.AsList (VecBuilder.Push4 (VecBuilder.Push4 (VecBuilder.Push4 (VecBuilder.Push4 (VecBuilder.Push1 (VecBuilder.Push1 (VecBuilder.make 18, McdRun.values.unit (["barye", "barye", "Pressure", mc_barye, "Value {Length-60000 Mass 60000 Time-120000}"])), McdRun.values.unit (["dyne", "dyne", "Force", mc_dyne, "Value {Length 60000 Mass 60000 Time-120000}"])), McdRun.values.unit (["erg", "erg", "Energy", mc_erg, "Value {Length 120000 Mass 60000 Time-120000}"]), McdRun.values.unit (["gauss", "gauss", "Magnetic Flux Density", mc_gauss, "Value {Length-30000 Mass 30000 Time-60000}"]), McdRun.values.unit (["katal", "katal", "Catalytic Activity", mc_katal, "Value {Substance 60000 Time-60000}"]), McdRun.values.unit (["lux", "lx", "Illuminance", mc_lx, "Value {Length-120000 Luminosity 60000}"])), McdRun.values.unit (["milliliter", "mL", "Volume", mc_mL, "Value {Length 180000}"]), McdRun.values.unit (["oersted", "oersted", "Magnetic Field", mc_oersted, "Value {Length-30000 Mass 30000 Time-60000}"]), McdRun.values.unit (["poise", "poise", "Viscosity, dynamic", mc_poise, "Value {Length-60000 Mass 60000 Time-60000}"]), McdRun.values.unit (["sievert", "Sv", "Dose", mc_Sv, "Value {Length 120000 Time-120000}"])), McdRun.values.unit (["statampere", "statamp", "Current", mc_statamp, "Value {Length 90000 Mass 30000 Time-120000}"]), McdRun.values.unit (["statcoulomb", "statcoul", "Charge", mc_statcoul, "Value {Length 90000 Mass 30000 Time-60000}"]), McdRun.values.unit (["statohm", "statohm", "Resistance", mc_statohm, "Value {Length-60000 Time 60000}"]), McdRun.values.unit (["statvolt", "statvolt", "Potential", mc_statvolt, "Value {Length 30000 Mass 30000 Time-60000}"])), McdRun.values.unit (["acceleration", "NOSIMPLIFY", "Acceleration", mc_cgs_accel, "Value {Length 60000 Time-120000}"]), McdRun.values.unit (["stathenry", "stathenry", "Inductance", mc_stathenry, "Value {Length-60000 Time 120000}"]), McdRun.values.unit (["stokes", "stokes", "Viscosity, kinematic", mc_stokes, "Value {Length 120000 Time-60000}"]), McdRun.values.unit (["watt", "W", "Power", mc_W, "Value {Length 120000 Mass 60000 Time-180000}"])))]))
mc_volt = McdRun.values.srv (McdStaticUnitSystem.values.cgs_volt)
mc_statweber = McdRun.values.srv (McdStaticUnitSystem.values.statweber)
mc_stattesla = McdRun.values.srv (McdStaticUnitSystem.values.stattesla)
mc_statsiemens = McdRun.values.srv (McdStaticUnitSystem.values.statsiemen)
mc_statfarad = McdRun.values.srv (McdStaticUnitSystem.values.statfarad)
mc_siemens = McdRun.values.srv (McdStaticUnitSystem.values.cgs_siemen)
mc_pF = McdRun.values.srv (McdStaticUnitSystem.values.pico (McdStaticUnitSystem.values.cgs_farad))
mc_ohm = McdRun.values.srv (McdStaticUnitSystem.values.cgs_ohm)
mc_nF = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.cgs_farad))
mc_mho = McdRun.values.srv (McdStaticUnitSystem.values.cgs_siemen)
mc_maxwell = McdRun.values.srv (McdStaticUnitSystem.values.maxwell)
mc_mV = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.cgs_volt))
mc_mH = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.cgs_henry))
mc_mA = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.cgs_ampere))
mc_kV = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.cgs_volt))
mc_kA = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.cgs_ampere))
mc_kΩ = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.cgs_ohm))
mc_henry = McdRun.values.srv (McdStaticUnitSystem.values.cgs_henry)
mc_farad = McdRun.values.srv (McdStaticUnitSystem.values.cgs_farad)
mc_esu = McdRun.values.srv (McdStaticUnitSystem.values.statcoulomb)
mc_amp = McdRun.values.srv (McdStaticUnitSystem.values.cgs_ampere)
mc_Mx = McdRun.values.srv (McdStaticUnitSystem.values.maxwell)
mc_MW = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.watt))
mc_MΩ = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.cgs_ohm))
mc_KV = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.cgs_volt))
mc_KA = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.cgs_ampere))
mc_KΩ = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.cgs_ohm))
mc_V = McdRun.values.srv (McdStaticUnitSystem.values.cgs_volt)
mc_S = McdRun.values.srv (McdStaticUnitSystem.values.cgs_siemen)
mc_H = McdRun.values.srv (McdStaticUnitSystem.values.cgs_henry)
mc_F = McdRun.values.srv (McdStaticUnitSystem.values.cgs_farad)
mc_C = McdRun.values.srv (McdStaticUnitSystem.values.cgs_coulomb)
mc_A = McdRun.values.srv (McdStaticUnitSystem.values.cgs_ampere)
mc_Ω = McdRun.values.srv (McdStaticUnitSystem.values.cgs_ohm)
mc_μH = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.cgs_henry))
mc_μF = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.cgs_farad))
mc_μA = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.cgs_ampere))
mc_e_2E_c = McdRun.values.srv (McdStaticUnitSystem.values.cgs_e_c)
mc_Φ_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.cgs_phi_0)
mc_ε_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.cgs_epsilon0)
mc_μ_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.cgs_mu0)
mc_N·mm = McdRun.values.srv ((McdStaticUnitSystem.values.newton *. McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.meter)))
mc_N·cm = McdRun.values.srv ((McdStaticUnitSystem.values.newton *. McdStaticUnitSystem.values.centi (McdStaticUnitSystem.values.meter)))
mc_N·m = McdRun.values.srv ((McdStaticUnitSystem.values.newton *. McdStaticUnitSystem.values.meter))
mc_in·ozf = McdRun.values.srv ((McdStaticUnitSystem.values.inch *. McdStaticUnitSystem.values.ounce_force))
mc_in·lbf = McdRun.values.srv ((McdStaticUnitSystem.values.inch *. McdStaticUnitSystem.values.pound_force))
mc_ft·lbf = McdRun.values.srv ((McdStaticUnitSystem.values.foot *. McdStaticUnitSystem.values.pound_force))
mc_rev = McdRun.values.srv (McdStaticUnitSystem.values.revolution)
mc_dpi = McdRun.values.srv (McdStaticUnitSystem.values.dpi)
mc_yr = McdRun.values.srv (McdStaticUnitSystem.values.year)
mc_yd = McdRun.values.srv (McdStaticUnitSystem.values.yard)
mc_watt = McdRun.values.srv (McdStaticUnitSystem.values.watt)
mc_torr = McdRun.values.srv (McdStaticUnitSystem.values.torr)
mc_tonne = McdRun.values.srv (McdStaticUnitSystem.values.tonne)
mc_ton = McdRun.values.srv (McdStaticUnitSystem.values.ton)
mc_tcal = McdRun.values.srv (McdStaticUnitSystem.values.calorie_thermo)
mc_tBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_thermo)
mc_sr = McdRun.values.srv (McdStaticUnitSystem.values.steradian)
mc_slug = McdRun.values.srv (McdStaticUnitSystem.values.slug)
mc_s = McdRun.values.srv (McdStaticUnitSystem.values.second)
mc_rad = McdRun.values.srv (McdStaticUnitSystem.values.radian)
mc_psf = McdRun.values.srv ((McdStaticUnitSystem.values.pound_force /. McdStaticUnitSystem.values.square (McdStaticUnitSystem.values.foot)))
mc_psi = McdRun.values.srv (McdStaticUnitSystem.values.psi)
mc_oz = McdRun.values.srv (McdStaticUnitSystem.values.ounce)
mc_nmi = McdRun.values.srv (McdStaticUnitSystem.values.nautical_mile)
mc_nm = McdRun.values.srv (McdStaticUnitSystem.values.nano (McdStaticUnitSystem.values.meter))
mc_newton = McdRun.values.srv (McdStaticUnitSystem.values.newton)
mc_ms = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.second))
mc_mph = McdRun.values.srv ((McdStaticUnitSystem.values.mile /. McdStaticUnitSystem.values.hour))
mc_mol = McdRun.values.srv (McdStaticUnitSystem.values.mole)
mc_mm = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.meter))
mc_min = McdRun.values.srv (McdStaticUnitSystem.values.minute)
mc_mile = McdRun.values.srv (McdStaticUnitSystem.values.mile)
mc_mil = McdRun.values.srv (McdStaticUnitSystem.values.mil)
mc_micron = McdRun.values.srv (McdStaticUnitSystem.values.micron)
mc_mi = McdRun.values.srv (McdStaticUnitSystem.values.mile)
mc_mhp = McdRun.values.srv (McdStaticUnitSystem.values.hp_metric)
mc_mW = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.watt))
mc_mg = McdRun.values.srv (McdStaticUnitSystem.values.milli (McdStaticUnitSystem.values.gram))
mc_mcal = McdRun.values.srv (McdStaticUnitSystem.values.calorie_mean)
mc_mBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_mean)
mc_m = McdRun.values.srv (McdStaticUnitSystem.values.meter)
mc_lm = McdRun.values.srv (McdStaticUnitSystem.values.lumen)
mc_liter = McdRun.values.srv (McdStaticUnitSystem.values.liter)
mc_lbf = McdRun.values.srv (McdStaticUnitSystem.values.pound_force)
mc_lbm = McdRun.values.srv (McdStaticUnitSystem.values.pound)
mc_lb = McdRun.values.srv (McdStaticUnitSystem.values.pound)
mc_ksi = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force) /. McdStaticUnitSystem.values.square (McdStaticUnitSystem.values.inch)))
mc_kph = McdRun.values.srv ((McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.meter) /. McdStaticUnitSystem.values.hour))
mc_km = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.meter))
mc_kip = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.pound_force))
mc_kgf = McdRun.values.srv (McdStaticUnitSystem.values.kilogram_force)
mc_kg = McdRun.values.srv (McdStaticUnitSystem.values.kilogram)
mc_kcal = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.calorie))
mc_kW = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.watt))
mc_kN = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.newton))
mc_kHz = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.hertz))
mc_joule = McdRun.values.srv (McdStaticUnitSystem.values.joule)
mc_in_5F_Hg = McdRun.values.srv (McdStaticUnitSystem.values.inches_mercury)
mc_in = McdRun.values.srv (McdStaticUnitSystem.values.inch)
mc_hr = McdRun.values.srv (McdStaticUnitSystem.values.hour)
mc_hpUK = McdRun.values.srv (McdStaticUnitSystem.values.hp_uk)
mc_hp = McdRun.values.srv (McdStaticUnitSystem.values.hp)
mc_hhp = McdRun.values.srv (McdStaticUnitSystem.values.hp_water)
mc_hectare = McdRun.values.srv (McdStaticUnitSystem.values.hectare)
mc_gpm = McdRun.values.srv ((McdStaticUnitSystem.values.gallon /. McdStaticUnitSystem.values.minute))
mc_galUK = McdRun.values.srv (McdStaticUnitSystem.values.gallon_uk)
mc_gal = McdRun.values.srv (McdStaticUnitSystem.values.gallon)
mc_g = McdRun.values.srv (McdStaticUnitSystem.values.g)
mc_ft = McdRun.values.srv (McdStaticUnitSystem.values.foot)
mc_furlong = McdRun.values.srv (McdStaticUnitSystem.values.furlong)
mc_fortnight = McdRun.values.srv (McdStaticUnitSystem.values.fortnight)
mc_fl_5F_oz = McdRun.values.srv (McdStaticUnitSystem.values.fluid_ounce)
mc_ehp = McdRun.values.srv (McdStaticUnitSystem.values.hp_electric)
mc_° = McdRun.values.srv (McdStaticUnitSystem.values.degree)
mc_deg = McdRun.values.srv (McdStaticUnitSystem.values.degree)
mc_dcal = McdRun.values.srv (McdStaticUnitSystem.values.calorie_diet)
mc_day = McdRun.values.srv (McdStaticUnitSystem.values.day)
mc_cubit = McdRun.values.srv (McdStaticUnitSystem.values.cubit)
mc_cal20 = McdRun.values.srv (McdStaticUnitSystem.values.calorie_20C)
mc_cal15 = McdRun.values.srv (McdStaticUnitSystem.values.calorie_15C)
mc_cal = McdRun.values.srv (McdStaticUnitSystem.values.calorie)
mc_c_5F_ = McdRun.values.srv (McdStaticUnitSystem.values.c)
mc_c = McdRun.values.srv (McdStaticUnitSystem.values.c)
mc_bohr = McdRun.values.srv (McdStaticUnitSystem.values.bohr)
mc_bhp = McdRun.values.srv (McdStaticUnitSystem.values.hp_boiler)
mc_barn = McdRun.values.srv (McdStaticUnitSystem.values.barn)
mc_atm = McdRun.values.srv (McdStaticUnitSystem.values.atmosphere)
mc_acre = McdRun.values.srv (McdStaticUnitSystem.values.acre)
mc_a_2E_0 = McdRun.values.srv (McdStaticUnitSystem.values.bohr)
mc_°R = McdRun.values.srv (McdStaticUnitSystem.values.rankine)
mc_R = McdRun.values.srv (McdStaticUnitSystem.values.rankine)
mc_Pa = McdRun.values.srv (McdStaticUnitSystem.values.pascal)
mc_N = McdRun.values.srv (McdStaticUnitSystem.values.newton)
mc_MPa = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.pascal))
mc_MHz = McdRun.values.srv (McdStaticUnitSystem.values.mega (McdStaticUnitSystem.values.hertz))
mc_L = McdRun.values.srv (McdStaticUnitSystem.values.liter)
mc_KHz = McdRun.values.srv (McdStaticUnitSystem.values.kilo (McdStaticUnitSystem.values.hertz))
mc_J = McdRun.values.srv (McdStaticUnitSystem.values.joule)
mc_IBTU = McdRun.values.srv (McdStaticUnitSystem.values.btu_iso)
mc_Hza = McdRun.values.srv (McdStaticUnitSystem.values.angular_hertz)
mc_Hz = McdRun.values.srv (McdStaticUnitSystem.values.hertz)
mc_Gy = McdRun.values.srv (McdStaticUnitSystem.values.gray)
mc_GPa = McdRun.values.srv (McdStaticUnitSystem.values.giga (McdStaticUnitSystem.values.pascal))
mc_GHz = McdRun.values.srv (McdStaticUnitSystem.values.giga (McdStaticUnitSystem.values.hertz))
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
mc_μs = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.second))
mc_μm = McdRun.values.srv (McdStaticUnitSystem.values.micro (McdStaticUnitSystem.values.meter))
mc_coul = McdRun.values.srv (McdStaticUnitSystem.values.cgs_coulomb)
mc_UnitsOf value = tail AppException.Raise "unitsof is obsolete" 0
mc_SIUnitsOf value = Mcd.NumVal.One
op_D value = tail McdRun.values.multiply_by_unit (value, mc_¤)
op_S value = tail McdRun.values.multiply_by_unit (value, mc_mole)
op_C value = tail McdRun.values.multiply_by_unit (value, mc_cd)
op_K value = tail McdRun.values.multiply_by_unit (value, mc_K)
op_Q value = tail McdRun.values.multiply_by_unit (value, mc_coul)
op_T value = tail McdRun.values.multiply_by_unit (value, mc_sec)
op_L value = tail McdRun.values.multiply_by_unit (value, mc_cm)
op_M value = tail McdRun.values.multiply_by_unit (value, mc_gm)
