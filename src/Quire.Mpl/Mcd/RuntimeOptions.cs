#nullable disable
using Quire.Mirror;

namespace Quire.Mcd;

/// <summary>
/// The unit systems of one worksheet, keyed by runtime-options number; the system it was created with has key 0
/// (docs/mirror/evidence/Mcd.RuntimeOptions.md).
/// </summary>
public partial class UnitSystemGroup : MarshalByRefObject
{
    [MirrorTested("Mcd.RuntimeOptions")]
    private Dictionary<int, UnitSystem> m_unitsys_list;
    [MirrorTested("Mcd.RuntimeOptions")]
    private readonly int init_id;
    [MirrorTested("Mcd.RuntimeOptions")]
    private string m_name;

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public UnitSystemGroup(UnitSystem unit_system)
    {
        m_unitsys_list = new Dictionary<int, UnitSystem> { { init_id, unit_system } };
        m_name = unit_system.Name;
    }

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public string Name => m_name;

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public UnitSystem findMember(int num)
    {
        if (!m_unitsys_list.ContainsKey(num))
        {
            return null;
        }
        return m_unitsys_list[num];
    }

    /// <summary>Replaces the basis and defaults of member <paramref name="num"/>, if there is one.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public void changeMember(int num, Unit[] basis, Unit[] defaults)
    {
        UnitSystem unitSystem = findMember(num);
        if (unitSystem != null)
        {
            unitSystem.Basis = basis;
            unitSystem.Defaults = defaults;
        }
    }

    /// <summary>Adds the system under its <see cref="UnitSystem.RO_number"/> unless that key is taken.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public void addMember(UnitSystem unit_system)
    {
        if (!m_unitsys_list.ContainsKey(unit_system.RO_number))
        {
            m_unitsys_list[unit_system.RO_number] = unit_system;
        }
    }

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public void removeMember(int num)
    {
        if (m_unitsys_list.ContainsKey(num))
        {
            m_unitsys_list.Remove(num);
        }
    }

    /// <summary>Gives member <paramref name="num"/> (never the initial one) the initial member's current basis and
    /// defaults, when both exist.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public void resetMember(int num)
    {
        if (num != 0 && m_unitsys_list.ContainsKey(num))
        {
            UnitSystem member = m_unitsys_list[num];
            if (m_unitsys_list.ContainsKey(init_id))
            {
                UnitSystem initial = m_unitsys_list[init_id];
                member.Basis = initial.Basis;
                member.Defaults = initial.Defaults;
            }
        }
    }

    /// <summary>Resets member <paramref name="num"/>, or else the initial member, to its construction-time units
    /// and returns it; null when neither exists.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public UnitSystem Reset(int num)
    {
        if (m_unitsys_list.ContainsKey(num))
        {
            return m_unitsys_list[num].Reset();
        }
        if (m_unitsys_list.ContainsKey(init_id))
        {
            return m_unitsys_list[init_id].Reset();
        }
        return null;
    }

    /// <summary>Member <paramref name="num"/>; if absent, a new copy of the initial member's name, code, guid and
    /// current units, numbered <paramref name="num"/> and added. Null when there is no initial member.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public UnitSystem addCopyWithRONumber(int num)
    {
        if (m_unitsys_list.ContainsKey(num))
        {
            return m_unitsys_list[num];
        }
        UnitSystem initial = findMember(init_id);
        if (initial != null)
        {
            UnitSystem copy = new UnitSystem(initial.Name, initial.Code, initial.Guid, initial.Basis, initial.Defaults)
            {
                RO_number = num
            };
            addMember(copy);
            return copy;
        }
        return null;
    }
}

/// <summary>
/// Calculation options of one worksheet. An instance holds them; <see cref="SyncToThread"/> copies them into
/// thread-static fields, which the runtime reads (the unit system included). The type's initialisation syncs a
/// default instance to the thread that first touches it; other threads have no unit system until something syncs
/// options to them.
/// </summary>
public partial class RuntimeOptions
{
    private partial class RuntimeOptionsInitializer
    {
        [MirrorTested("Mcd.RuntimeOptions")]
        public RuntimeOptionsInitializer()
        {
            new RuntimeOptions().SyncToThread();
            s_ro_counter++;
        }
    }

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public static bool s_restricted_mode = false;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public static int s_ro_counter = 0;

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_strict_comparisons;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_strict_singularity_checking = true;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_strings_indexed_from_origin = true;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_zero_over_zero_is_zero;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_debugging_enabled;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_labels_in_symbolics;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_units_in_symbolics;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public bool m_constants_in_symbolics;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public MangledIdOptions m_mangled_identifiers;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public UnitSystem m_unit_system = s_default_unit_system;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public UnitSystemGroup m_unit_system_group = new(s_default_unit_system);
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public int m_ro_number = s_ro_counter + 1;

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool strict_comparisons;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool strict_singularity_checking;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool strings_indexed_from_origin;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool zero_over_zero_is_zero;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool debugging_enabled;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool labels_in_symbolics;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool units_in_symbolics;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static bool constants_in_symbolics;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static int the_ro_number;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static MangledIdOptions mangled_identifiers;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static UnitSystem the_unit_system;
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    [ThreadStatic] public static UnitSystemGroup the_unit_system_group;

    /// <summary>Mathcad's built-in SI: eight base units (money included) and fourteen derived defaults.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public static UnitSystem s_default_unit_system = new("SI", 0u, "7F1921F3-0A1A-11E0-8A9E-11F8E2A8C36C",
    [
        new Unit("kilogram", "kg", "Mass", 1.0, "V{Mass         60000}"),
        new Unit("meter", "m", "Length", 1.0, "V{Length       60000}"),
        new Unit("second", "s", "Time", 1.0, "V{Time         60000}"),
        new Unit("ampere", "A", "Current", 1.0, "V{Current      60000}"),
        new Unit("kelvin", "K", "Temperature", 1.0, "V{Temperature  60000}"),
        new Unit("candela", "cd", "Luminosity", 1.0, "V{Luminosity   60000}"),
        new Unit("mole", "mol", "Substance", 1.0, "V{Substance    60000}"),
        new Unit("base_currency", "¤", "Money", 1.0, "V{Money        60000}"),
    ],
    [
        new Unit("coulomb", "C", "Charge", 1.0, "V{Current 60000 Time 60000}"),
        new Unit("farad", "F", "Capacitance", 1.0, "V{Current 120000 Length-120000 Mass-60000 Time 240000}"),
        new Unit("henry", "H", "Inductance", 1.0, "V{Current-120000 Length 120000 Mass 60000 Time-120000}"),
        new Unit("joule", "J", "Energy", 1.0, "V{Length 120000 Mass 60000 Time-120000}"),
        new Unit("liter", "L", "Volume", 1.0, "V{Length 180000}"),
        new Unit("lux", "lx", "Illuminance", 1.0, "V{Length-120000 Luminosity 60000}"),
        new Unit("newton", "N", "Force", 1.0, "V{Length 60000 Mass 60000 Time-120000}"),
        new Unit("ohm", "Ω", "Resistance", 1.0, "V{Current-120000 Length 120000 Mass 60000 Time-180000}"),
        new Unit("pascal", "Pa", "Pressure", 1.0, "V{Length-60000 Mass 60000 Time-120000}"),
        new Unit("siemens", "S", "Conductance", 1.0, "V{Current 120000 Length-120000 Mass-60000 Time 180000}"),
        new Unit("tesla", "T", "Magnetic Flux Density", 1.0, "V{Current-60000 Mass 60000 Time-120000}"),
        new Unit("watt", "W", "Power", 1.0, "V{Length 120000 Mass 60000 Time-180000}"),
        new Unit("weber", "Wb", "Magnetic Flux", 1.0, "V{Current-60000 Length 120000 Mass 60000 Time-120000}"),
        new Unit("volt", "V", "Potential", 1.0, "V{Current-60000 Length 120000 Mass 60000 Time-180000}"),
    ]);

    // Declared last: initialising it runs a constructor that reads the fields above.
    [MirrorTested("Mcd.RuntimeOptions")]
    private static object s_init = new RuntimeOptionsInitializer();

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public RuntimeOptions() { }

    /// <summary>Copies the options into the calling thread's fields (under a lock on this instance) and gives the
    /// thread its dimension scratch vector.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public void SyncToThread()
    {
        lock (this)
        {
            strict_comparisons = m_strict_comparisons;
            strict_singularity_checking = m_strict_singularity_checking;
            strings_indexed_from_origin = m_strings_indexed_from_origin;
            zero_over_zero_is_zero = m_zero_over_zero_is_zero;
            debugging_enabled = m_debugging_enabled;
            labels_in_symbolics = m_labels_in_symbolics;
            units_in_symbolics = m_units_in_symbolics;
            constants_in_symbolics = m_constants_in_symbolics;
            the_unit_system = m_unit_system;
            mangled_identifiers = m_mangled_identifiers;
            the_ro_number = m_ro_number;
            the_unit_system_group = m_unit_system_group;
            Dim.ThreadInit();
        }
    }

    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public void RemoveUnitSystem() => m_unit_system_group.removeMember(m_ro_number);

    /// <summary>A new instance holding the calling thread's current options.</summary>
    [MirrorVerified("RuntimeOptionsDifferentialTests")]
    public static RuntimeOptions SaveThreadOptions() => new()
    {
        m_strict_comparisons = strict_comparisons,
        m_strict_singularity_checking = strict_singularity_checking,
        m_strings_indexed_from_origin = strings_indexed_from_origin,
        m_zero_over_zero_is_zero = zero_over_zero_is_zero,
        m_debugging_enabled = debugging_enabled,
        m_labels_in_symbolics = labels_in_symbolics,
        m_units_in_symbolics = units_in_symbolics,
        m_constants_in_symbolics = constants_in_symbolics,
        m_unit_system = the_unit_system,
        m_mangled_identifiers = mangled_identifiers,
        m_ro_number = the_ro_number,
        m_unit_system_group = the_unit_system_group,
    };
}
