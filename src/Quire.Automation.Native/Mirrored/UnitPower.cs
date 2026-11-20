namespace Quire.Automation;

public partial struct UnitPower
{
    public string Name;
    public global::Quire.Mcd.Rational Power;

    public UnitPower(string name, global::Quire.Mcd.Rational power)
    {
        Name = name;
        Power = power;
    }
}
