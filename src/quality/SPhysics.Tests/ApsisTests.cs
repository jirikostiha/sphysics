using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class ApsisTests
{
    [Fact]
    public void ApsidesOfAnEllipseSpanItsMajorAxis()
    {
        Assert.Equal(2.0, Apsis.Pericenter(5.0, 0.6), 12);
        Assert.Equal(8.0, Apsis.Apocenter(5.0, 0.6), 12);
    }

    [Fact]
    public void CircleHasBothApsidesAtItsRadius()
    {
        Assert.Equal(5.0, Apsis.Pericenter(5.0, 0.0));
        Assert.Equal(5.0, Apsis.Apocenter(5.0, 0.0));
    }

    [Fact]
    public void PericenterOfAHyperbolaIsPositive()
        => Assert.Equal(2.0, Apsis.Pericenter(-2.0, 2.0), 12);
}
