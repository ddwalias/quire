-- CGS derived units, expressed with the Mpl module's unit conversion primitive.
module McdUnitSystemCGS (McdUnitSystemCGS.mc_¤,McdUnitSystemCGS.system,McdUnitSystemCGS.mc_IBTU,McdUnitSystemCGS.mc_J,McdUnitSystemCGS.mc_K,McdUnitSystemCGS.mc_KHz,McdUnitSystemCGS.mc_L,McdUnitSystemCGS.mc_MHz,McdUnitSystemCGS.mc_MPa,McdUnitSystemCGS.mc_N,McdUnitSystemCGS.mc_Pa,McdUnitSystemCGS.mc_R,McdUnitSystemCGS.mc_°R,McdUnitSystemCGS.mc_Sv,McdUnitSystemCGS.mc_W,McdUnitSystemCGS.mc_a_2E_0,McdUnitSystemCGS.mc_a₊0₊,McdUnitSystemCGS.mc_acre,McdUnitSystemCGS.mc_atm,McdUnitSystemCGS.mc_barn,McdUnitSystemCGS.mc_μm,McdUnitSystemCGS.mc_μs,McdUnitSystemCGS.mc_Angstrom,McdUnitSystemCGS.mc_BTU,McdUnitSystemCGS.mc_Btu,McdUnitSystemCGS.mc_BTU15,McdUnitSystemCGS.mc_Bq,McdUnitSystemCGS.mc_CBTU,McdUnitSystemCGS.mc_Δ°C,McdUnitSystemCGS.mc_Δ°F,McdUnitSystemCGS.mc_Δ℃,McdUnitSystemCGS.mc_Δ℉,McdUnitSystemCGS.mc_GHz,McdUnitSystemCGS.mc_GPa,McdUnitSystemCGS.mc_Gy,McdUnitSystemCGS.mc_Hz,McdUnitSystemCGS.mc_Hza,McdUnitSystemCGS.mc_cd,McdUnitSystemCGS.mc_cm,McdUnitSystemCGS.mc_cubit,McdUnitSystemCGS.mc_day,McdUnitSystemCGS.mc_dcal,McdUnitSystemCGS.mc_deg,McdUnitSystemCGS.mc_°,McdUnitSystemCGS.mc_dyne,McdUnitSystemCGS.mc_ehp,McdUnitSystemCGS.mc_joule,McdUnitSystemCGS.mc_kHz,McdUnitSystemCGS.mc_kN,McdUnitSystemCGS.mc_kW,McdUnitSystemCGS.mc_katal,McdUnitSystemCGS.mc_kcal,McdUnitSystemCGS.mc_kg,McdUnitSystemCGS.mc_kgf,McdUnitSystemCGS.mc_kip,McdUnitSystemCGS.mc_erg,McdUnitSystemCGS.mc_fl_5F_oz,McdUnitSystemCGS.mc_fortnight,McdUnitSystemCGS.mc_furlong,McdUnitSystemCGS.mc_ft,McdUnitSystemCGS.mc_g,McdUnitSystemCGS.mc_gal,McdUnitSystemCGS.mc_galUK,McdUnitSystemCGS.mc_gm,McdUnitSystemCGS.mc_barye,McdUnitSystemCGS.mc_bhp,McdUnitSystemCGS.mc_bohr,McdUnitSystemCGS.mc_c#CONSTANT#UNIT,McdUnitSystemCGS.mc_c_5F_,McdUnitSystemCGS.mc_cal,McdUnitSystemCGS.mc_cal15,McdUnitSystemCGS.mc_cal20,McdUnitSystemCGS.mc_gpm,McdUnitSystemCGS.mc_hectare,McdUnitSystemCGS.mc_hhp,McdUnitSystemCGS.mc_hp,McdUnitSystemCGS.mc_hpUK,McdUnitSystemCGS.mc_hr,McdUnitSystemCGS.mc_in,McdUnitSystemCGS.mc_in_5F_Hg,McdUnitSystemCGS.mc_m,McdUnitSystemCGS.mc_mBTU,McdUnitSystemCGS.mc_mL,McdUnitSystemCGS.mc_mcal,McdUnitSystemCGS.mc_mg,McdUnitSystemCGS.mc_mW,McdUnitSystemCGS.mc_mhp,McdUnitSystemCGS.mc_mi,McdUnitSystemCGS.mc_km,McdUnitSystemCGS.mc_kph,McdUnitSystemCGS.mc_ksi,McdUnitSystemCGS.mc_lb,McdUnitSystemCGS.mc_lbm,McdUnitSystemCGS.mc_lbf,McdUnitSystemCGS.mc_liter,McdUnitSystemCGS.mc_lm,McdUnitSystemCGS.mc_lx,McdUnitSystemCGS.mc_newton,McdUnitSystemCGS.mc_nm,McdUnitSystemCGS.mc_nmi,McdUnitSystemCGS.mc_oz,McdUnitSystemCGS.mc_poise,McdUnitSystemCGS.mc_psi,McdUnitSystemCGS.mc_psf,McdUnitSystemCGS.mc_rad,McdUnitSystemCGS.mc_sec,McdUnitSystemCGS.mc_micron,McdUnitSystemCGS.mc_mil,McdUnitSystemCGS.mc_mile,McdUnitSystemCGS.mc_min,McdUnitSystemCGS.mc_mm,McdUnitSystemCGS.mc_mol,McdUnitSystemCGS.mc_mole,McdUnitSystemCGS.mc_mph,McdUnitSystemCGS.mc_ms,McdUnitSystemCGS.mc_e_2E_c#CONSTANT,McdUnitSystemCGS.mc_e₊c₊#CONSTANT,McdUnitSystemCGS.mc_μA,McdUnitSystemCGS.mc_μF,McdUnitSystemCGS.mc_μH,McdUnitSystemCGS.mc_Ω,McdUnitSystemCGS.mc_A,McdUnitSystemCGS.mc_C,McdUnitSystemCGS.mc_torr,McdUnitSystemCGS.mc_watt,McdUnitSystemCGS.mc_yd,McdUnitSystemCGS.mc_yr,McdUnitSystemCGS.mc_dpi,McdUnitSystemCGS.mc_rev,McdUnitSystemCGS.mc_ft·lbf,McdUnitSystemCGS.mc_in·lbf,McdUnitSystemCGS.mc_in·ozf,McdUnitSystemCGS.mc_s,McdUnitSystemCGS.mc_slug,McdUnitSystemCGS.mc_sr,McdUnitSystemCGS.mc_stokes,McdUnitSystemCGS.mc_tBTU,McdUnitSystemCGS.mc_tcal,McdUnitSystemCGS.mc_ton,McdUnitSystemCGS.mc_tonne,McdUnitSystemCGS.mc_N·m,McdUnitSystemCGS.mc_N·cm,McdUnitSystemCGS.mc_N·mm,McdUnitSystemCGS.mc_μ_2E_0#CONSTANT#UNIT,McdUnitSystemCGS.mc_ε_2E_0#CONSTANT#UNIT,McdUnitSystemCGS.mc_μ₊0₊#CONSTANT#UNIT,McdUnitSystemCGS.mc_ε₊0₊#CONSTANT#UNIT,McdUnitSystemCGS.mc_Φ_2E_0#CONSTANT#UNIT,McdUnitSystemCGS.mc_Φ₊0₊#CONSTANT#UNIT,McdUnitSystemCGS.mc_F,McdUnitSystemCGS.mc_H,McdUnitSystemCGS.mc_S,McdUnitSystemCGS.mc_V,McdUnitSystemCGS.mc_KΩ,McdUnitSystemCGS.mc_KA,McdUnitSystemCGS.mc_KV,McdUnitSystemCGS.mc_MΩ,McdUnitSystemCGS.mc_MW,McdUnitSystemCGS.mc_ohm,McdUnitSystemCGS.mc_pF,McdUnitSystemCGS.mc_siemens,McdUnitSystemCGS.mc_statamp,McdUnitSystemCGS.mc_statcoul,McdUnitSystemCGS.mc_statfarad,McdUnitSystemCGS.mc_stathenry,McdUnitSystemCGS.mc_statohm,McdUnitSystemCGS.mc_statsiemens,McdUnitSystemCGS.mc_kV,McdUnitSystemCGS.mc_mA,McdUnitSystemCGS.mc_mH,McdUnitSystemCGS.mc_mV,McdUnitSystemCGS.mc_maxwell,McdUnitSystemCGS.mc_mho,McdUnitSystemCGS.mc_nF,McdUnitSystemCGS.mc_oersted,McdUnitSystemCGS.mc_Mx,McdUnitSystemCGS.mc_amp,McdUnitSystemCGS.mc_coul,McdUnitSystemCGS.mc_esu,McdUnitSystemCGS.mc_farad,McdUnitSystemCGS.mc_gauss,McdUnitSystemCGS.mc_henry,McdUnitSystemCGS.mc_kΩ,McdUnitSystemCGS.mc_kA,McdUnitSystemCGS.op_M,McdUnitSystemCGS.op_L,McdUnitSystemCGS.op_T,McdUnitSystemCGS.op_Q,McdUnitSystemCGS.op_K,McdUnitSystemCGS.op_C,McdUnitSystemCGS.op_S,McdUnitSystemCGS.op_D,McdUnitSystemCGS.dyn,McdUnitSystemCGS.mc_stattesla,McdUnitSystemCGS.mc_statvolt,McdUnitSystemCGS.mc_statweber,McdUnitSystemCGS.mc_volt,McdUnitSystemSI.mc__2F_℃,McdUnitSystemSI.mc__2F_°F,McdUnitSystemSI.mc__2F_℉,McdUnitSystemSI.mc_°C,McdUnitSystemSI.mc_℃,McdUnitSystemSI.mc_°F,McdUnitSystemSI.mc_℉,McdUnitSystemSI.mc_σ#CONSTANT,McdUnitSystemSI.mc_α#CONSTANT,McdUnitSystemSI.mc__2F_°C,McdUnitSystemSI.mc_h#CONSTANT,McdUnitSystemSI.mc_m₊u₊#CONSTANT,McdUnitSystemSI.mc_N₊A₊#CONSTANT,McdUnitSystemSI.mc_m_2E_u#CONSTANT,McdUnitSystemSI.mc_N_2E_A#CONSTANT,McdUnitSystemSI.mc_k#CONSTANT,McdUnitSystemSI.mc_R#CONSTANT,McdUnitSystemSI.mc_R_2E_∞#CONSTANT,McdUnitSystemSI.mc_R₊∞₊#CONSTANT,McdUnitSystemSI.mc_ℏ#CONSTANT) where
{
 mc_¤ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_¤";
 system :: McdRun.UnitSystemGroup @ "McdUnitSystemCGS.system";
 mc_IBTU :: Mpl.Value {} @ "McdUnitSystemCGS.mc_IBTU";
 mc_J :: Mpl.Value {} @ "McdUnitSystemCGS.mc_J";
 mc_K :: Mpl.Value {} @ "McdUnitSystemCGS.mc_K";
 mc_KHz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_KHz";
 mc_L :: Mpl.Value {} @ "McdUnitSystemCGS.mc_L";
 mc_MHz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_MHz";
 mc_MPa :: Mpl.Value {} @ "McdUnitSystemCGS.mc_MPa";
 mc_N :: Mpl.Value {} @ "McdUnitSystemCGS.mc_N";
 mc_Pa :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Pa";
 mc_R :: Mpl.Value {} @ "McdUnitSystemCGS.mc_R";
 mc_°R :: Mpl.Value {} @ "McdUnitSystemCGS.mc_°R";
 mc_Sv :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Sv";
 mc_W :: Mpl.Value {} @ "McdUnitSystemCGS.mc_W";
 mc_a_2E_0 :: Mpl.Value {} @ "McdUnitSystemCGS.mc_a_2E_0";
 mc_a₊0₊ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_a₊0₊";
 mc_acre :: Mpl.Value {} @ "McdUnitSystemCGS.mc_acre";
 mc_atm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_atm";
 mc_barn :: Mpl.Value {} @ "McdUnitSystemCGS.mc_barn";
 mc_μm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μm";
 mc_μs :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μs";
 mc_Angstrom :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Angstrom";
 mc_BTU :: Mpl.Value {} @ "McdUnitSystemCGS.mc_BTU";
 mc_Btu :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Btu";
 mc_BTU15 :: Mpl.Value {} @ "McdUnitSystemCGS.mc_BTU15";
 mc_Bq :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Bq";
 mc_CBTU :: Mpl.Value {} @ "McdUnitSystemCGS.mc_CBTU";
 mc_Δ°C :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Δ°C";
 mc_Δ°F :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Δ°F";
 mc_Δ℃ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Δ℃";
 mc_Δ℉ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Δ℉";
 mc_GHz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_GHz";
 mc_GPa :: Mpl.Value {} @ "McdUnitSystemCGS.mc_GPa";
 mc_Gy :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Gy";
 mc_Hz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Hz";
 mc_Hza :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Hza";
 mc_cd :: Mpl.Value {} @ "McdUnitSystemCGS.mc_cd";
 mc_cm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_cm";
 mc_cubit :: Mpl.Value {} @ "McdUnitSystemCGS.mc_cubit";
 mc_day :: Mpl.Value {} @ "McdUnitSystemCGS.mc_day";
 mc_dcal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_dcal";
 mc_deg :: Mpl.Value {} @ "McdUnitSystemCGS.mc_deg";
 mc_° :: Mpl.Value {} @ "McdUnitSystemCGS.mc_°";
 mc_dyne :: Mpl.Value {} @ "McdUnitSystemCGS.mc_dyne";
 mc_ehp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ehp";
 mc_joule :: Mpl.Value {} @ "McdUnitSystemCGS.mc_joule";
 mc_kHz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kHz";
 mc_kN :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kN";
 mc_kW :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kW";
 mc_katal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_katal";
 mc_kcal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kcal";
 mc_kg :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kg";
 mc_kgf :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kgf";
 mc_kip :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kip";
 mc_erg :: Mpl.Value {} @ "McdUnitSystemCGS.mc_erg";
 mc_fl_5F_oz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_fl_5F_oz";
 mc_fortnight :: Mpl.Value {} @ "McdUnitSystemCGS.mc_fortnight";
 mc_furlong :: Mpl.Value {} @ "McdUnitSystemCGS.mc_furlong";
 mc_ft :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ft";
 mc_g :: Mpl.Value {} @ "McdUnitSystemCGS.mc_g";
 mc_gal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_gal";
 mc_galUK :: Mpl.Value {} @ "McdUnitSystemCGS.mc_galUK";
 mc_gm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_gm";
 mc_barye :: Mpl.Value {} @ "McdUnitSystemCGS.mc_barye";
 mc_bhp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_bhp";
 mc_bohr :: Mpl.Value {} @ "McdUnitSystemCGS.mc_bohr";
 mc_c#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_c#CONSTANT#UNIT";
 mc_c_5F_ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_c_5F_";
 mc_cal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_cal";
 mc_cal15 :: Mpl.Value {} @ "McdUnitSystemCGS.mc_cal15";
 mc_cal20 :: Mpl.Value {} @ "McdUnitSystemCGS.mc_cal20";
 mc_gpm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_gpm";
 mc_hectare :: Mpl.Value {} @ "McdUnitSystemCGS.mc_hectare";
 mc_hhp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_hhp";
 mc_hp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_hp";
 mc_hpUK :: Mpl.Value {} @ "McdUnitSystemCGS.mc_hpUK";
 mc_hr :: Mpl.Value {} @ "McdUnitSystemCGS.mc_hr";
 mc_in :: Mpl.Value {} @ "McdUnitSystemCGS.mc_in";
 mc_in_5F_Hg :: Mpl.Value {} @ "McdUnitSystemCGS.mc_in_5F_Hg";
 mc_m :: Mpl.Value {} @ "McdUnitSystemCGS.mc_m";
 mc_mBTU :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mBTU";
 mc_mL :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mL";
 mc_mcal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mcal";
 mc_mg :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mg";
 mc_mW :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mW";
 mc_mhp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mhp";
 mc_mi :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mi";
 mc_km :: Mpl.Value {} @ "McdUnitSystemCGS.mc_km";
 mc_kph :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kph";
 mc_ksi :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ksi";
 mc_lb :: Mpl.Value {} @ "McdUnitSystemCGS.mc_lb";
 mc_lbm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_lbm";
 mc_lbf :: Mpl.Value {} @ "McdUnitSystemCGS.mc_lbf";
 mc_liter :: Mpl.Value {} @ "McdUnitSystemCGS.mc_liter";
 mc_lm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_lm";
 mc_lx :: Mpl.Value {} @ "McdUnitSystemCGS.mc_lx";
 mc_newton :: Mpl.Value {} @ "McdUnitSystemCGS.mc_newton";
 mc_nm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_nm";
 mc_nmi :: Mpl.Value {} @ "McdUnitSystemCGS.mc_nmi";
 mc_oz :: Mpl.Value {} @ "McdUnitSystemCGS.mc_oz";
 mc_poise :: Mpl.Value {} @ "McdUnitSystemCGS.mc_poise";
 mc_psi :: Mpl.Value {} @ "McdUnitSystemCGS.mc_psi";
 mc_psf :: Mpl.Value {} @ "McdUnitSystemCGS.mc_psf";
 mc_rad :: Mpl.Value {} @ "McdUnitSystemCGS.mc_rad";
 mc_sec :: Mpl.Value {} @ "McdUnitSystemCGS.mc_sec";
 mc_micron :: Mpl.Value {} @ "McdUnitSystemCGS.mc_micron";
 mc_mil :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mil";
 mc_mile :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mile";
 mc_min :: Mpl.Value {} @ "McdUnitSystemCGS.mc_min";
 mc_mm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mm";
 mc_mol :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mol";
 mc_mole :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mole";
 mc_mph :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mph";
 mc_ms :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ms";
 mc_e_2E_c#CONSTANT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_e_2E_c#CONSTANT";
 mc_e₊c₊#CONSTANT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_e₊c₊#CONSTANT";
 mc_μA :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μA";
 mc_μF :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μF";
 mc_μH :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μH";
 mc_Ω :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Ω";
 mc_A :: Mpl.Value {} @ "McdUnitSystemCGS.mc_A";
 mc_C :: Mpl.Value {} @ "McdUnitSystemCGS.mc_C";
 mc_torr :: Mpl.Value {} @ "McdUnitSystemCGS.mc_torr";
 mc_watt :: Mpl.Value {} @ "McdUnitSystemCGS.mc_watt";
 mc_yd :: Mpl.Value {} @ "McdUnitSystemCGS.mc_yd";
 mc_yr :: Mpl.Value {} @ "McdUnitSystemCGS.mc_yr";
 mc_dpi :: Mpl.Value {} @ "McdUnitSystemCGS.mc_dpi";
 mc_rev :: Mpl.Value {} @ "McdUnitSystemCGS.mc_rev";
 mc_ft·lbf :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ft·lbf";
 mc_in·lbf :: Mpl.Value {} @ "McdUnitSystemCGS.mc_in·lbf";
 mc_in·ozf :: Mpl.Value {} @ "McdUnitSystemCGS.mc_in·ozf";
 mc_s :: Mpl.Value {} @ "McdUnitSystemCGS.mc_s";
 mc_slug :: Mpl.Value {} @ "McdUnitSystemCGS.mc_slug";
 mc_sr :: Mpl.Value {} @ "McdUnitSystemCGS.mc_sr";
 mc_stokes :: Mpl.Value {} @ "McdUnitSystemCGS.mc_stokes";
 mc_tBTU :: Mpl.Value {} @ "McdUnitSystemCGS.mc_tBTU";
 mc_tcal :: Mpl.Value {} @ "McdUnitSystemCGS.mc_tcal";
 mc_ton :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ton";
 mc_tonne :: Mpl.Value {} @ "McdUnitSystemCGS.mc_tonne";
 mc_N·m :: Mpl.Value {} @ "McdUnitSystemCGS.mc_N·m";
 mc_N·cm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_N·cm";
 mc_N·mm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_N·mm";
 mc_μ_2E_0#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μ_2E_0#CONSTANT#UNIT";
 mc_ε_2E_0#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ε_2E_0#CONSTANT#UNIT";
 mc_μ₊0₊#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_μ₊0₊#CONSTANT#UNIT";
 mc_ε₊0₊#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ε₊0₊#CONSTANT#UNIT";
 mc_Φ_2E_0#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Φ_2E_0#CONSTANT#UNIT";
 mc_Φ₊0₊#CONSTANT#UNIT :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Φ₊0₊#CONSTANT#UNIT";
 mc_F :: Mpl.Value {} @ "McdUnitSystemCGS.mc_F";
 mc_H :: Mpl.Value {} @ "McdUnitSystemCGS.mc_H";
 mc_S :: Mpl.Value {} @ "McdUnitSystemCGS.mc_S";
 mc_V :: Mpl.Value {} @ "McdUnitSystemCGS.mc_V";
 mc_KΩ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_KΩ";
 mc_KA :: Mpl.Value {} @ "McdUnitSystemCGS.mc_KA";
 mc_KV :: Mpl.Value {} @ "McdUnitSystemCGS.mc_KV";
 mc_MΩ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_MΩ";
 mc_MW :: Mpl.Value {} @ "McdUnitSystemCGS.mc_MW";
 mc_ohm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_ohm";
 mc_pF :: Mpl.Value {} @ "McdUnitSystemCGS.mc_pF";
 mc_siemens :: Mpl.Value {} @ "McdUnitSystemCGS.mc_siemens";
 mc_statamp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statamp";
 mc_statcoul :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statcoul";
 mc_statfarad :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statfarad";
 mc_stathenry :: Mpl.Value {} @ "McdUnitSystemCGS.mc_stathenry";
 mc_statohm :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statohm";
 mc_statsiemens :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statsiemens";
 mc_kV :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kV";
 mc_mA :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mA";
 mc_mH :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mH";
 mc_mV :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mV";
 mc_maxwell :: Mpl.Value {} @ "McdUnitSystemCGS.mc_maxwell";
 mc_mho :: Mpl.Value {} @ "McdUnitSystemCGS.mc_mho";
 mc_nF :: Mpl.Value {} @ "McdUnitSystemCGS.mc_nF";
 mc_oersted :: Mpl.Value {} @ "McdUnitSystemCGS.mc_oersted";
 mc_Mx :: Mpl.Value {} @ "McdUnitSystemCGS.mc_Mx";
 mc_amp :: Mpl.Value {} @ "McdUnitSystemCGS.mc_amp";
 mc_coul :: Mpl.Value {} @ "McdUnitSystemCGS.mc_coul";
 mc_esu :: Mpl.Value {} @ "McdUnitSystemCGS.mc_esu";
 mc_farad :: Mpl.Value {} @ "McdUnitSystemCGS.mc_farad";
 mc_gauss :: Mpl.Value {} @ "McdUnitSystemCGS.mc_gauss";
 mc_henry :: Mpl.Value {} @ "McdUnitSystemCGS.mc_henry";
 mc_kΩ :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kΩ";
 mc_kA :: Mpl.Value {} @ "McdUnitSystemCGS.mc_kA";
 op_M :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_M";
 op_L :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_L";
 op_T :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_T";
 op_Q :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_Q";
 op_K :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_K";
 op_C :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_C";
 op_S :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_S";
 op_D :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemCGS.op_D";
 inline 10 dyn;
 dyn :: McdRun.V {d}->McdRun.Dyn @ "McdUnitSystemCGS.dyn" = (\z10_->McdRun.dnum z10_ "V {d}");
 mc_stattesla :: Mpl.Value {} @ "McdUnitSystemCGS.mc_stattesla";
 mc_statvolt :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statvolt";
 mc_statweber :: Mpl.Value {} @ "McdUnitSystemCGS.mc_statweber";
 mc_volt :: Mpl.Value {} @ "McdUnitSystemCGS.mc_volt";
 import McdStaticUnitSystemCGS;
 import McdUnitSystemSI;
 import McdRun;
 import Mpl;
}
-- implementation
mc_volt = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_volt "Value {Length 30000 Mass 30000 Time-60000}"
mc_statweber = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statweber "Value {Length 90000 Mass 30000 Time-60000}"
mc_statvolt = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statvolt "Value {Length 30000 Mass 30000 Time-60000}"
mc_stattesla = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_stattesla "Value {Length-30000 Mass 30000 Time-60000}"
mc_statsiemens = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statsiemens "Value {Length 60000 Time-60000}"
mc_statohm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statohm "Value {Length-60000 Time 60000}"
mc_stathenry = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_stathenry "Value {Length-60000 Time 120000}"
mc_statfarad = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statfarad "Value {Length 60000}"
mc_statamp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statamp "Value {Length 90000 Mass 30000 Time-120000}"
mc_siemens = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_siemens "Value {Length 60000 Time-60000}"
mc_pF = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_pF "Value {Length 60000}"
mc_ohm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ohm "Value {Length-60000 Time 60000}"
mc_oersted = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_oersted "Value {Length-30000 Mass 30000 Time-60000}"
mc_nF = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_nF "Value {Length 60000}"
mc_mho = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mho "Value {Length 60000 Time-60000}"
mc_maxwell = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_maxwell "Value {Length 90000 Mass 30000 Time-60000}"
mc_mV = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mV "Value {Length 30000 Mass 30000 Time-60000}"
mc_mH = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mH "Value {Length-60000 Time 120000}"
mc_mA = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mA "Value {Length 90000 Mass 30000 Time-120000}"
mc_kV = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kV "Value {Length 30000 Mass 30000 Time-60000}"
mc_kA = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kA "Value {Length 90000 Mass 30000 Time-120000}"
mc_kΩ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kΩ "Value {Length-60000 Time 60000}"
mc_henry = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_henry "Value {Length-60000 Time 120000}"
mc_gauss = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_gauss "Value {Length-30000 Mass 30000 Time-60000}"
mc_farad = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_farad "Value {Length 60000}"
mc_esu = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_esu "Value {Length 90000 Mass 30000 Time-60000}"
mc_coul = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_coul "Value {Length 90000 Mass 30000 Time-60000}"
mc_amp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_amp "Value {Length 90000 Mass 30000 Time-120000}"
mc_Mx = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Mx "Value {Length 90000 Mass 30000 Time-60000}"
mc_MW = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_MW "Value {Length 120000 Mass 60000 Time-180000}"
mc_MΩ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_MΩ "Value {Length-60000 Time 60000}"
mc_KV = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_KV "Value {Length 30000 Mass 30000 Time-60000}"
mc_KA = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_KA "Value {Length 90000 Mass 30000 Time-120000}"
mc_KΩ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_KΩ "Value {Length-60000 Time 60000}"
mc_V = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_V "Value {Length 30000 Mass 30000 Time-60000}"
mc_S = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_S "Value {Length 60000 Time-60000}"
mc_H = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_H "Value {Length-60000 Time 120000}"
mc_F = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_F "Value {Length 60000}"
mc_C = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_C "Value {Length 90000 Mass 30000 Time-60000}"
mc_A = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_A "Value {Length 90000 Mass 30000 Time-120000}"
mc_Ω = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Ω "Value {Length-60000 Time 60000}"
mc_μH = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μH "Value {Length-60000 Time 120000}"
mc_μF = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μF "Value {Length 60000}"
mc_μA = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μA "Value {Length 90000 Mass 30000 Time-120000}"
mc_e₊c₊#CONSTANT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_e_2E_c "Value {Length 90000 Mass 30000 Time-60000}"
mc_e_2E_c#CONSTANT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_e_2E_c "Value {Length 90000 Mass 30000 Time-60000}"
mc_Φ₊0₊#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Φ_2E_0 "Value {Length 90000 Mass 30000 Time-60000}"
mc_Φ_2E_0#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Φ_2E_0 "Value {Length 90000 Mass 30000 Time-60000}"
mc_ε₊0₊#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ε_2E_0 "Value {}"
mc_μ₊0₊#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μ_2E_0 "Value {Length-120000 Time 120000}"
mc_ε_2E_0#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ε_2E_0 "Value {}"
mc_μ_2E_0#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μ_2E_0 "Value {Length-120000 Time 120000}"
mc_N·mm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_N·mm "Value {Length 120000 Mass 60000 Time-120000}"
mc_N·cm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_N·cm "Value {Length 120000 Mass 60000 Time-120000}"
mc_N·m = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_N·m "Value {Length 120000 Mass 60000 Time-120000}"
mc_in·ozf = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_in·ozf "Value {Length 120000 Mass 60000 Time-120000}"
mc_in·lbf = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_in·lbf "Value {Length 120000 Mass 60000 Time-120000}"
mc_ft·lbf = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ft·lbf "Value {Length 120000 Mass 60000 Time-120000}"
mc_rev = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_rev "Value {}"
mc_dpi = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_dpi "Value {Length-60000}"
mc_yr = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_yr "Value {Time 60000}"
mc_yd = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_yd "Value {Length 60000}"
mc_watt = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_watt "Value {Length 120000 Mass 60000 Time-180000}"
mc_torr = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_torr "Value {Length-60000 Mass 60000 Time-120000}"
mc_tonne = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_tonne "Value {Mass 60000}"
mc_ton = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ton "Value {Mass 60000}"
mc_tcal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_tcal "Value {Length 120000 Mass 60000 Time-120000}"
mc_tBTU = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_tBTU "Value {Length 120000 Mass 60000 Time-120000}"
mc_stokes = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_stokes "Value {Length 120000 Time-60000}"
mc_sr = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_sr "Value {}"
mc_slug = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_slug "Value {Mass 60000}"
mc_s = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_s "Value {Time 60000}"
mc_rad = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_rad "Value {}"
mc_psf = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_psf "Value {Length-60000 Mass 60000 Time-120000}"
mc_psi = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_psi "Value {Length-60000 Mass 60000 Time-120000}"
mc_poise = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_poise "Value {Length-60000 Mass 60000 Time-60000}"
mc_oz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_oz "Value {Mass 60000}"
mc_nmi = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_nmi "Value {Length 60000}"
mc_nm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_nm "Value {Length 60000}"
mc_newton = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_newton "Value {Length 60000 Mass 60000 Time-120000}"
mc_ms = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ms "Value {Time 60000}"
mc_mph = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mph "Value {Length 60000 Time-60000}"
mc_mol = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mol "Value {Substance 60000}"
mc_mm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mm "Value {Length 60000}"
mc_min = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_min "Value {Time 60000}"
mc_mile = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mile "Value {Length 60000}"
mc_mil = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mil "Value {Length 60000}"
mc_micron = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_micron "Value {Length 60000}"
mc_mi = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mi "Value {Length 60000}"
mc_mhp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mhp "Value {Length 120000 Mass 60000 Time-180000}"
mc_mW = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mW "Value {Length 120000 Mass 60000 Time-180000}"
mc_mg = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mg "Value {Mass 60000}"
mc_mcal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mcal "Value {Length 120000 Mass 60000 Time-120000}"
mc_mL = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mL "Value {Length 180000}"
mc_mBTU = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mBTU "Value {Length 120000 Mass 60000 Time-120000}"
mc_m = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_m "Value {Length 60000}"
mc_lx = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_lx "Value {Length-120000 Luminosity 60000}"
mc_lm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_lm "Value {Luminosity 60000}"
mc_liter = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_liter "Value {Length 180000}"
mc_lbf = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_lbf "Value {Length 60000 Mass 60000 Time-120000}"
mc_lbm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_lbm "Value {Mass 60000}"
mc_lb = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_lb "Value {Mass 60000}"
mc_ksi = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ksi "Value {Length-60000 Mass 60000 Time-120000}"
mc_kph = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kph "Value {Length 60000 Time-60000}"
mc_km = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_km "Value {Length 60000}"
mc_kip = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kip "Value {Length 60000 Mass 60000 Time-120000}"
mc_kgf = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kgf "Value {Length 60000 Mass 60000 Time-120000}"
mc_kg = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kg "Value {Mass 60000}"
mc_kcal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kcal "Value {Length 120000 Mass 60000 Time-120000}"
mc_katal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_katal "Value {Substance 60000 Time-60000}"
mc_kW = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kW "Value {Length 120000 Mass 60000 Time-180000}"
mc_kN = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kN "Value {Length 60000 Mass 60000 Time-120000}"
mc_kHz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_kHz "Value {Time-60000}"
mc_joule = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_joule "Value {Length 120000 Mass 60000 Time-120000}"
mc_in_5F_Hg = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_in_5F_Hg "Value {Length-60000 Mass 60000 Time-120000}"
mc_in = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_in "Value {Length 60000}"
mc_hr = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_hr "Value {Time 60000}"
mc_hpUK = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_hpUK "Value {Length 120000 Mass 60000 Time-180000}"
mc_hp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_hp "Value {Length 120000 Mass 60000 Time-180000}"
mc_hhp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_hhp "Value {Length 120000 Mass 60000 Time-180000}"
mc_hectare = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_hectare "Value {Length 120000}"
mc_gpm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_gpm "Value {Length 180000 Time-60000}"
mc_galUK = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_galUK "Value {Length 180000}"
mc_gal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_gal "Value {Length 180000}"
mc_g = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_g "Value {Length 60000 Time-120000}"
mc_ft = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ft "Value {Length 60000}"
mc_furlong = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_furlong "Value {Length 60000}"
mc_fortnight = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_fortnight "Value {Time 60000}"
mc_fl_5F_oz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_fl_5F_oz "Value {Length 180000}"
mc_erg = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_erg "Value {Length 120000 Mass 60000 Time-120000}"
mc_ehp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_ehp "Value {Length 120000 Mass 60000 Time-180000}"
mc_dyne = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_dyne "Value {Length 60000 Mass 60000 Time-120000}"
mc_° = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_° "Value {}"
mc_deg = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_deg "Value {}"
mc_dcal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_dcal "Value {Length 120000 Mass 60000 Time-120000}"
mc_day = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_day "Value {Time 60000}"
mc_cubit = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_cubit "Value {Length 60000}"
mc_cal20 = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_cal20 "Value {Length 120000 Mass 60000 Time-120000}"
mc_cal15 = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_cal15 "Value {Length 120000 Mass 60000 Time-120000}"
mc_cal = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_cal "Value {Length 120000 Mass 60000 Time-120000}"
mc_c_5F_ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_c_5F_ "Value {}"
mc_c#CONSTANT#UNIT = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_c "Value {Length 60000 Time-60000}"
mc_bohr = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_bohr "Value {Length 60000}"
mc_bhp = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_bhp "Value {Length 120000 Mass 60000 Time-180000}"
mc_barye = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_barye "Value {Length-60000 Mass 60000 Time-120000}"
mc_barn = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_barn "Value {Length 120000}"
mc_atm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_atm "Value {Length-60000 Mass 60000 Time-120000}"
mc_acre = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_acre "Value {Length 120000}"
mc_a₊0₊ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_a_2E_0 "Value {Length 60000}"
mc_a_2E_0 = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_a_2E_0 "Value {Length 60000}"
mc_W = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_W "Value {Length 120000 Mass 60000 Time-180000}"
mc_Sv = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Sv "Value {Length 120000 Time-120000}"
mc_°R = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_°R "Value {Temperature 60000}"
mc_R = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_R "Value {Temperature 60000}"
mc_Pa = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Pa "Value {Length-60000 Mass 60000 Time-120000}"
mc_N = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_N "Value {Length 60000 Mass 60000 Time-120000}"
mc_MPa = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_MPa "Value {Length-60000 Mass 60000 Time-120000}"
mc_MHz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_MHz "Value {Time-60000}"
mc_L = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_L "Value {Length 180000}"
mc_KHz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_KHz "Value {Time-60000}"
mc_J = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_J "Value {Length 120000 Mass 60000 Time-120000}"
mc_IBTU = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_IBTU "Value {Length 120000 Mass 60000 Time-120000}"
mc_Hza = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Hza "Value {Time-60000}"
mc_Hz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Hz "Value {Time-60000}"
mc_Gy = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Gy "Value {Length 120000 Time-120000}"
mc_GPa = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_GPa "Value {Length-60000 Mass 60000 Time-120000}"
mc_GHz = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_GHz "Value {Time-60000}"
mc_Δ℉ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Δ℉ "Value {Temperature 60000}"
mc_Δ℃ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Δ℃ "Value {Temperature 60000}"
mc_Δ°F = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Δ°F "Value {Temperature 60000}"
mc_Δ°C = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Δ°C "Value {Temperature 60000}"
mc_CBTU = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_CBTU "Value {Length 120000 Mass 60000 Time-120000}"
mc_Bq = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Bq "Value {Time-60000}"
mc_BTU15 = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_BTU15 "Value {Length 120000 Mass 60000 Time-120000}"
mc_Btu = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Btu "Value {Length 120000 Mass 60000 Time-120000}"
mc_BTU = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_BTU "Value {Length 120000 Mass 60000 Time-120000}"
mc_Angstrom = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_Angstrom "Value {Length 60000}"
mc_μs = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μs "Value {Time 60000}"
mc_μm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_μm "Value {Length 60000}"
mc_¤ = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_¤ "Value {Money 60000}"
mc_mole = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_mole "Value {Substance 60000}"
mc_cd = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_cd "Value {Luminosity 60000}"
mc_K = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_K "Value {Temperature 60000}"
mc_statcoul = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_statcoul "Value {Length 90000 Mass 30000 Time-60000}"
mc_sec = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_sec "Value {Time 60000}"
mc_cm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_cm "Value {Length 60000}"
mc_gm = McdRun.values.dnum McdStaticUnitSystemCGS.values.mc_gm "Value {Mass 60000}"
system = McdStaticUnitSystemCGS.system
dyn value = tail McdRun.values.dnum value "V {d}"
op_D value = tail McdRun.values.times_unit value mc_¤
op_S value = tail McdRun.values.times_unit value mc_mole
op_C value = tail McdRun.values.times_unit value mc_cd
op_K value = tail McdRun.values.times_unit value mc_K
op_Q value = tail McdRun.values.times_unit value mc_statcoul
op_T value = tail McdRun.values.times_unit value mc_sec
op_L value = tail McdRun.values.times_unit value mc_cm
op_M value = tail McdRun.values.times_unit value mc_gm
