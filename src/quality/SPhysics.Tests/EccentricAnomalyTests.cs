using SPhysics.CelestialMechanics;
using Xunit;

namespace SPhysics.Tests;

public class EccentricAnomalyTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(0.9)]
    [InlineData(0.999)]
    public void SolvesKeplersEquation(double eccentricity)
    {
        for (double meanAnomaly = -10.0; meanAnomaly <= 10.0; meanAnomaly += 0.37)
        {
            var eccentricAnomaly = EccentricAnomaly.FromMeanAnomaly(meanAnomaly, eccentricity);

            Assert.Equal(meanAnomaly, MeanAnomaly.Eval(eccentricAnomaly, eccentricity), 12);
        }
    }

    [Fact]
    public void EqualsMeanAnomalyOnCircularOrbit()
        => Assert.Equal(2.5, EccentricAnomaly.FromMeanAnomaly(2.5, 0.0));

    [Fact]
    public void KeepsTheApsides()
    {
        Assert.Equal(0.0, EccentricAnomaly.FromMeanAnomaly(0.0, 0.7), 12);
        Assert.Equal(Math.PI, EccentricAnomaly.FromMeanAnomaly(Math.PI, 0.7), 12);
        Assert.Equal(3.0 * Math.PI, EccentricAnomaly.FromMeanAnomaly(3.0 * Math.PI, 0.7), 12);
    }

    [Fact]
    public void WorksInSinglePrecision()
    {
        var eccentricAnomaly = EccentricAnomaly.FromMeanAnomaly(1.0f, 0.5f);

        Assert.Equal(1.0f, MeanAnomaly.Eval(eccentricAnomaly, 0.5f), 5);
    }
}
