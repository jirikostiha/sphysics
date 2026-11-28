namespace SPhysics;

/// <summary> Speed of light in vacuum, exact by the definition of the metre. </summary>
/// <remarks><a href="https://physics.nist.gov/cgi-bin/cuu/Value?c">CODATA 2018, NIST</a></remarks>
public static class SpeedOfLight
{
    public const string Name = "Speed of light";
    public const string DefaultSymbol = "c";
    public const string Dimension = Velocity.Dimension;

    /// <summary> Value in m s^-1. </summary>
    public const double Value = 299_792_458d;
}
