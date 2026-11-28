-- Authored Mpl source for the None system's dimension units.
module McdStaticUnitSystemNone (McdStaticUnitSystemNone.op_Q,McdStaticUnitSystemNone.op_K,McdStaticUnitSystemNone.op_C,McdStaticUnitSystemNone.op_S,McdStaticUnitSystemNone.op_D,McdStaticUnitSystemNone.mc_M,McdStaticUnitSystemNone.op_M,McdStaticUnitSystemNone.op_L,McdStaticUnitSystemNone.op_T,McdStaticUnitSystemNone.mc_L,McdStaticUnitSystemNone.mc_T,McdStaticUnitSystemNone.mc_Q,McdStaticUnitSystemNone.mc_K,McdStaticUnitSystemNone.mc_C,McdStaticUnitSystemNone.mc_S,McdStaticUnitSystemNone.mc_D,McdStaticUnitSystemNone.system,McdStaticUnitSystemNone.mc_UnitsOf,McdStaticUnitSystem.Time(..),McdStaticUnitSystem.Current(..),McdStaticUnitSystem.Temperature(..),McdStaticUnitSystem.Substance(..),McdStaticUnitSystem.Luminosity(..),McdStaticUnitSystem.Money(..),McdStaticUnitSystem.Length(..),McdStaticUnitSystem.Mass(..)) where
{
 op_Q :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Current 60000} @ 46;
 op_K :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ 46;
 op_C :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ 46;
 op_S :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Substance 60000} @ 46;
 op_D :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Money 60000} @ 46;
 mc_M :: Mpl.Value {McdStaticUnitSystem.Mass 60000} @ "McdStaticUnitSystemNone.mc_M";
 op_M :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Mass 60000} @ 46;
 op_L :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Length 60000} @ 46;
 op_T :: Mpl.Value {}->Mpl.Value {McdStaticUnitSystem.Time 60000} @ 46;
 mc_L :: Mpl.Value {McdStaticUnitSystem.Length 60000} @ "McdStaticUnitSystemNone.mc_L";
 mc_T :: Mpl.Value {McdStaticUnitSystem.Time 60000} @ "McdStaticUnitSystemNone.mc_T";
 mc_Q :: Mpl.Value {McdStaticUnitSystem.Current 60000} @ "McdStaticUnitSystemNone.mc_Q";
 mc_K :: Mpl.Value {McdStaticUnitSystem.Temperature 60000} @ "McdStaticUnitSystemNone.mc_K";
 mc_C :: Mpl.Value {McdStaticUnitSystem.Luminosity 60000} @ "McdStaticUnitSystemNone.mc_C";
 mc_S :: Mpl.Value {McdStaticUnitSystem.Substance 60000} @ "McdStaticUnitSystemNone.mc_S";
 mc_D :: Mpl.Value {McdStaticUnitSystem.Money 60000} @ "McdStaticUnitSystemNone.mc_D";
 system :: McdRun.UnitSystemGroup @ "McdStaticUnitSystemNone.system";
 mc_UnitsOf :: McdRun.V {a}->McdRun.V {a} @ "McdStaticUnitSystemNone.mc_UnitsOf";
 import McdStaticUnitSystem;
 import McdRun;
 import Mpl;
}
-- implementation
mc_D = McdRun.values.siv' 1
mc_S = McdRun.values.siv' 1
mc_C = McdRun.values.siv' 1
mc_K = McdRun.values.siv' 1
mc_Q = McdRun.values.siv' 1
mc_T = McdRun.values.siv' 1
mc_L = McdRun.values.siv' 1
mc_M = McdRun.values.siv' 1
system = McdRun.values.unit_system_group (McdRun.values.unit_system (["none", 4, "B2D057CB-916D-4BBF-BA8E-9D67051C7F35", new Cons (McdRun.values.unit (["M", "M", "M", mc_M, "Value {Mass 60000}"]), new Cons (McdRun.values.unit (["L", "L", "L", mc_L, "Value {Length 60000}"]), new Cons (McdRun.values.unit (["T", "T", "T", mc_T, "Value {Time 60000}"]), new Cons (McdRun.values.unit (["Q", "Q", "Q", mc_Q, "Value {Current 60000}"]), new Cons (McdRun.values.unit (["K", "K", "K", mc_K, "Value {Temperature 60000}"]), new Cons (McdRun.values.unit (["C", "C", "C", mc_C, "Value {Luminosity 60000}"]), new Cons (McdRun.values.unit (["S", "S", "S", mc_S, "Value {Substance 60000}"]), new Cons (McdRun.values.unit (["D", "D", "D", mc_D, "Value {Money 60000}"]), null)))))))), null]))
mc_UnitsOf value = NumVal.One
