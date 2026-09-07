using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class TrueAnomalyTests
{
    [Fact]
    public void EqualsEccentricAnomalyOnCircularOrbit()
    {
        Assert.Equal(1.0, TrueAnomaly.FromEccentricAnomaly(1.0, 0.0), 10);
        Assert.Equal(-2.0, TrueAnomaly.FromEccentricAnomaly(-2.0, 0.0), 10);
    }

    [Fact]
    public void IsZeroAtPeriapsisAndPiAtApoapsis()
    {
        Assert.Equal(0.0, TrueAnomaly.FromEccentricAnomaly(0.0, 0.5), 10);
        Assert.Equal(Math.PI, TrueAnomaly.FromEccentricAnomaly(Math.PI, 0.5), 10);
    }

    [Fact]
    public void MatchesTheCosineRelation()
    {
        // cos(v) = (cos(E) - e) / (1 - e * cos(E))
        const double e = 0.6;
        const double eccentricAnomaly = 1.2;

        var trueAnomaly = TrueAnomaly.FromEccentricAnomaly(eccentricAnomaly, e);
        var expected = (Math.Cos(eccentricAnomaly) - e) / (1 - e * Math.Cos(eccentricAnomaly));

        Assert.Equal(expected, Math.Cos(trueAnomaly), 10);
    }
}
