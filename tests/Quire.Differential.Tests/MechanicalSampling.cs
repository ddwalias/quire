namespace Quire.Differential.Tests;

/// <summary>
/// The sample values Mechanical gives generated comparisons: distinct across rounds, slots (arguments and the target)
/// and the values within a slot, so a body that reads the wrong field or argument sees a different value.
/// </summary>
public class MechanicalSamplingTests
{
    [Fact]
    public void Sample_numbers_never_repeat_across_slots_or_rounds()
    {
        var seen = new HashSet<int>();
        for (int round = 1; round <= 2; round++)
            for (int index = 0; index < 4; index++)
                foreach (int value in new[] { 0, 1, 49, 50, 51, 999, 9_999 })
                    Assert.True(seen.Add(Mechanical.Number(round, index, value)), $"round {round}, slot {index}, value {value}");
    }

    /// <summary>A slot never runs into the next one's numbers: past its capacity it fails instead.</summary>
    [Fact]
    public void A_slot_with_too_many_values_fails()
    {
        Assert.NotEqual(Mechanical.Number(1, 1, 0), Mechanical.Number(1, 0, 50));
        Assert.Throws<HarnessException>(() => Mechanical.Number(1, 0, 10_000));
    }

    /// <summary>Two booleans (or enum fields) of one slot differ, as two numbers of one slot do.</summary>
    [Fact]
    public void Choices_alternate_within_a_slot()
    {
        Assert.NotEqual(Mechanical.Choice(1, 0, 0, 2), Mechanical.Choice(1, 0, 1, 2));
        Assert.NotEqual(Mechanical.Choice(1, 2, 0, 3), Mechanical.Choice(1, 2, 1, 3));
        Assert.NotEqual(Mechanical.Choice(1, 0, 0, 2), Mechanical.Choice(2, 0, 0, 2));
    }
}
