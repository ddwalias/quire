-- Authored Mpl source for SI unit scales and derived units.
module MplUnitSystemSI (MplUnitSystemSI.second,MplUnitSystemSI.ampere,MplUnitSystemSI.kelvin,MplUnitSystemSI.mole,MplUnitSystemSI.candela,MplUnitSystemSI.radian,MplUnitSystemSI.steradian,MplUnitSystemSI.hertz,MplUnitSystemSI.L(..),MplUnitSystemSI.M(..),MplUnitSystemSI.T(..),MplUnitSystemSI.C(..),MplUnitSystemSI.K(..),MplUnitSystemSI.S(..),MplUnitSystemSI.I(..),MplUnitSystemSI.meter,MplUnitSystemSI.kilogram,MplUnitSystemSI.newton,MplUnitSystemSI.pascal,MplUnitSystemSI.joule,MplUnitSystemSI.watt,MplUnitSystemSI.coulomb,MplUnitSystemSI.volt,MplUnitSystemSI.farad,MplUnitSystemSI.ohm,MplUnitSystemSI.siemens,MplUnitSystemSI.weber,MplUnitSystemSI.tesla,MplUnitSystemSI.henry,MplUnitSystemSI.celsius,MplUnitSystemSI.lumen,MplUnitSystemSI.giga,MplUnitSystemSI.mega,MplUnitSystemSI.kilo,MplUnitSystemSI.hecto,MplUnitSystemSI.deka,MplUnitSystemSI.deci,MplUnitSystemSI.centi,MplUnitSystemSI.milli,MplUnitSystemSI.micro,MplUnitSystemSI.lux,MplUnitSystemSI.becquerel,MplUnitSystemSI.gray,MplUnitSystemSI.sievert,MplUnitSystemSI.katal,MplUnitSystemSI.minute,MplUnitSystemSI.hour,MplUnitSystemSI.day,MplUnitSystemSI.degree,MplUnitSystemSI.arcminute,MplUnitSystemSI.arcsecond,MplUnitSystemSI.liter,MplUnitSystemSI.tonne,MplUnitSystemSI.electronvolt,MplUnitSystemSI.atomicmass,MplUnitSystemSI.astronomical,MplUnitSystemSI.yotta,MplUnitSystemSI.zetta,MplUnitSystemSI.exa,MplUnitSystemSI.peta,MplUnitSystemSI.tera,MplUnitSystemSI.nano,MplUnitSystemSI.pico,MplUnitSystemSI.femto,MplUnitSystemSI.atto,MplUnitSystemSI.zepto,MplUnitSystemSI.yocto) where
{
 second :: Mpl.Real {MplUnitSystemSI.T} @ "MplUnitSystemSI.second";
 ampere :: Mpl.Real {MplUnitSystemSI.C} @ "MplUnitSystemSI.ampere";
 kelvin :: Mpl.Real {MplUnitSystemSI.K} @ "MplUnitSystemSI.kelvin";
 mole :: Mpl.Real {MplUnitSystemSI.S} @ "MplUnitSystemSI.mole";
 candela :: Mpl.Real {MplUnitSystemSI.I} @ "MplUnitSystemSI.candela";
 radian :: Mpl.Real {} @ "MplUnitSystemSI.radian";
 steradian :: Mpl.Real {} @ "MplUnitSystemSI.steradian";
 hertz :: Mpl.Real {MplUnitSystemSI.T-1} @ "MplUnitSystemSI.hertz";
 dimension L;
 dimension M;
 dimension T;
 dimension C;
 dimension K;
 dimension S;
 dimension I;
 meter :: Mpl.Real {MplUnitSystemSI.L} @ "MplUnitSystemSI.meter";
 kilogram :: Mpl.Real {MplUnitSystemSI.M} @ "MplUnitSystemSI.kilogram";
 newton :: Mpl.Real {MplUnitSystemSI.L MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.newton";
 pascal :: Mpl.Real {MplUnitSystemSI.L-1 MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.pascal";
 joule :: Mpl.Real {MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.joule";
 watt :: Mpl.Real {MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-3} @ "MplUnitSystemSI.watt";
 coulomb :: Mpl.Real {MplUnitSystemSI.C MplUnitSystemSI.T} @ "MplUnitSystemSI.coulomb";
 volt :: Mpl.Real {MplUnitSystemSI.C-1 MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-3} @ "MplUnitSystemSI.volt";
 farad :: Mpl.Real {MplUnitSystemSI.C 2 MplUnitSystemSI.L-2 MplUnitSystemSI.M-1 MplUnitSystemSI.T 4} @ "MplUnitSystemSI.farad";
 ohm :: Mpl.Real {MplUnitSystemSI.C-2 MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-3} @ "MplUnitSystemSI.ohm";
 siemens :: Mpl.Real {MplUnitSystemSI.C 2 MplUnitSystemSI.L-2 MplUnitSystemSI.M-1 MplUnitSystemSI.T 3} @ "MplUnitSystemSI.siemens";
 weber :: Mpl.Real {MplUnitSystemSI.C-1 MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.weber";
 tesla :: Mpl.Real {MplUnitSystemSI.C-1 MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.tesla";
 henry :: Mpl.Real {MplUnitSystemSI.C-2 MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.henry";
 celsius :: Mpl.Real {MplUnitSystemSI.K} @ "MplUnitSystemSI.celsius";
 lumen :: Mpl.Real {MplUnitSystemSI.I} @ "MplUnitSystemSI.lumen";
 giga :: Mpl.Real {} @ "MplUnitSystemSI.giga";
 mega :: Mpl.Real {} @ "MplUnitSystemSI.mega";
 kilo :: Mpl.Real {} @ "MplUnitSystemSI.kilo";
 hecto :: Mpl.Real {} @ "MplUnitSystemSI.hecto";
 deka :: Mpl.Real {} @ "MplUnitSystemSI.deka";
 deci :: Mpl.Real {} @ "MplUnitSystemSI.deci";
 centi :: Mpl.Real {} @ "MplUnitSystemSI.centi";
 milli :: Mpl.Real {} @ "MplUnitSystemSI.milli";
 micro :: Mpl.Real {} @ "MplUnitSystemSI.micro";
 lux :: Mpl.Real {MplUnitSystemSI.I MplUnitSystemSI.L-2} @ "MplUnitSystemSI.lux";
 becquerel :: Mpl.Real {MplUnitSystemSI.T-1} @ "MplUnitSystemSI.becquerel";
 gray :: Mpl.Real {MplUnitSystemSI.L 2 MplUnitSystemSI.T-2} @ "MplUnitSystemSI.gray";
 sievert :: Mpl.Real {MplUnitSystemSI.L 2 MplUnitSystemSI.T-2} @ "MplUnitSystemSI.sievert";
 katal :: Mpl.Real {MplUnitSystemSI.S MplUnitSystemSI.T-1} @ "MplUnitSystemSI.katal";
 minute :: Mpl.Real {MplUnitSystemSI.T} @ "MplUnitSystemSI.minute";
 hour :: Mpl.Real {MplUnitSystemSI.T} @ "MplUnitSystemSI.hour";
 day :: Mpl.Real {MplUnitSystemSI.T} @ "MplUnitSystemSI.day";
 degree :: Mpl.Real {} @ "MplUnitSystemSI.degree";
 arcminute :: Mpl.Real {} @ "MplUnitSystemSI.arcminute";
 arcsecond :: Mpl.Real {} @ "MplUnitSystemSI.arcsecond";
 liter :: Mpl.Real {MplUnitSystemSI.L 3} @ "MplUnitSystemSI.liter";
 tonne :: Mpl.Real {MplUnitSystemSI.M} @ "MplUnitSystemSI.tonne";
 electronvolt :: Mpl.Real {MplUnitSystemSI.L 2 MplUnitSystemSI.M MplUnitSystemSI.T-2} @ "MplUnitSystemSI.electronvolt";
 atomicmass :: Mpl.Real {MplUnitSystemSI.M} @ "MplUnitSystemSI.atomicmass";
 astronomical :: Mpl.Real {MplUnitSystemSI.L} @ "MplUnitSystemSI.astronomical";
 yotta :: Mpl.Real {} @ "MplUnitSystemSI.yotta";
 zetta :: Mpl.Real {} @ "MplUnitSystemSI.zetta";
 exa :: Mpl.Real {} @ "MplUnitSystemSI.exa";
 peta :: Mpl.Real {} @ "MplUnitSystemSI.peta";
 tera :: Mpl.Real {} @ "MplUnitSystemSI.tera";
 nano :: Mpl.Real {} @ "MplUnitSystemSI.nano";
 pico :: Mpl.Real {} @ "MplUnitSystemSI.pico";
 femto :: Mpl.Real {} @ "MplUnitSystemSI.femto";
 atto :: Mpl.Real {} @ "MplUnitSystemSI.atto";
 zepto :: Mpl.Real {} @ "MplUnitSystemSI.zepto";
 yocto :: Mpl.Real {} @ "MplUnitSystemSI.yocto";
 import MplPrelude;
 import MplConstants;
 import Mpl;
}
-- implementation
-- Authored Mpl source for SI unit scales and derived units.
yocto = 1e-24
zepto = 1e-21
atto = 1e-18
femto = 1e-15
pico = 1e-12
nano = 1e-9
micro = 1e-6
milli = 0.001
centi = 0.01
deci = 0.1
deka = 10.0
hecto = 100.0
kilo = 1000.0
mega = 1000000.0
giga = 1000000000.0
tera = 1000000000000.0
peta = 1000000000000000.0
exa = 1e18
zetta = 1e21
yotta = 1e24
meter = 1.0
astronomical = 149598000000.0 *. meter
kilogram = 1.0
atomicmass = 1.66054e-27 *. kilogram
second = 1.0
newton = meter *. kilogram /. second /. second
joule = newton *. meter
electronvolt = 1.60218e-19 *. joule
tonne = 1000.0 *. kilogram
liter = 0.001 *. meter *. meter *. meter
radian = 1.0
degree = (MplConstants.pi /. 180.0) *. radian
arcsecond = degree /. 360.0
arcminute = degree /. 60.0
minute = 60.0 *. second
hour = 60.0 *. minute
day = 24.0 *. hour
mole = 1.0
katal = mole /. second
sievert = joule /. kilogram
gray = joule /. kilogram
becquerel = 1.0 /. second
steradian = 1.0
candela = 1.0
lumen = candela *. steradian
lux = (lumen /. meter) /. meter
kelvin = 1.0
celsius = kelvin
watt = joule /. second
ampere = 1.0
volt = watt /. ampere
weber = volt *. second
henry = weber /. ampere
tesla = (weber /. meter) /. meter
siemens = ampere /. volt
ohm = volt /. ampere
coulomb = second *. ampere
farad = coulomb /. volt
pascal = (newton /. meter) /. meter
hertz = 1.0 /. second
