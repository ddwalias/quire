-- Authored Mpl source for the dimensionless (None) unit-system view.
module McdUnitSystemNone (McdUnitSystemNone.mc_M,McdUnitSystemNone.mc_L,McdUnitSystemNone.mc_T,McdUnitSystemNone.mc_Q,McdUnitSystemNone.mc_K,McdUnitSystemNone.mc_C,McdUnitSystemNone.mc_S,McdUnitSystemNone.mc_D,McdUnitSystemNone.system,McdUnitSystemNone.op_M,McdUnitSystemNone.op_L,McdUnitSystemNone.op_T,McdUnitSystemNone.op_Q,McdUnitSystemNone.op_K,McdUnitSystemNone.op_C,McdUnitSystemNone.op_S,McdUnitSystemNone.op_D,McdUnitSystemNone.dyn,McdStaticUnitSystem.Mass(..),McdStaticUnitSystem.Time(..),McdStaticUnitSystem.Current(..),McdStaticUnitSystem.Temperature(..),McdStaticUnitSystem.Length(..),McdStaticUnitSystem.Luminosity(..),McdStaticUnitSystem.Substance(..),McdStaticUnitSystem.Money(..)) where
{
 mc_M :: Mpl.Value {} @ "McdUnitSystemNone.mc_M";
 mc_L :: Mpl.Value {} @ "McdUnitSystemNone.mc_L";
 mc_T :: Mpl.Value {} @ "McdUnitSystemNone.mc_T";
 mc_Q :: Mpl.Value {} @ "McdUnitSystemNone.mc_Q";
 mc_K :: Mpl.Value {} @ "McdUnitSystemNone.mc_K";
 mc_C :: Mpl.Value {} @ "McdUnitSystemNone.mc_C";
 mc_S :: Mpl.Value {} @ "McdUnitSystemNone.mc_S";
 mc_D :: Mpl.Value {} @ "McdUnitSystemNone.mc_D";
 system :: McdRun.UnitSystemGroup @ "McdUnitSystemNone.system";
 op_M :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_M";
 op_L :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_L";
 op_T :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_T";
 op_Q :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_Q";
 op_K :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_K";
 op_C :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_C";
 op_S :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_S";
 op_D :: Mpl.Value {}->Mpl.Value {} @ "McdUnitSystemNone.op_D";
 inline 10 dyn;
 dyn :: McdRun.V {d}->McdRun.Dyn @ "McdUnitSystemNone.dyn" = (\z10_->McdRun.dnum z10_ "V {d}");
 import McdStaticUnitSystemNone;
 import McdRun;
 import McdStaticUnitSystem;
 import Mpl;
}
-- implementation
-- Bind values from the selected static system and apply units through McdRun.
system = McdStaticUnitSystemNone.system
mc_D = McdRun.values.dnum McdStaticUnitSystemNone.mc_D "Value {Money 60000}"
mc_S = McdRun.values.dnum McdStaticUnitSystemNone.mc_S "Value {Substance 60000}"
mc_C = McdRun.values.dnum McdStaticUnitSystemNone.mc_C "Value {Luminosity 60000}"
mc_K = McdRun.values.dnum McdStaticUnitSystemNone.mc_K "Value {Temperature 60000}"
mc_Q = McdRun.values.dnum McdStaticUnitSystemNone.mc_Q "Value {Current 60000}"
mc_T = McdRun.values.dnum McdStaticUnitSystemNone.mc_T "Value {Time 60000}"
mc_L = McdRun.values.dnum McdStaticUnitSystemNone.mc_L "Value {Length 60000}"
mc_M = McdRun.values.dnum McdStaticUnitSystemNone.mc_M "Value {Mass 60000}"
dyn value = tail McdRun.values.dnum value "V {d}"
op_D value = tail McdRun.values.times_unit value mc_D
op_S value = tail McdRun.values.times_unit value mc_S
op_C value = tail McdRun.values.times_unit value mc_C
op_K value = tail McdRun.values.times_unit value mc_K
op_Q value = tail McdRun.values.times_unit value mc_Q
op_T value = tail McdRun.values.times_unit value mc_T
op_L value = tail McdRun.values.times_unit value mc_L
op_M value = tail McdRun.values.times_unit value mc_M

