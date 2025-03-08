namespace SPhysics;

/// <summary>
/// Pressure quantity
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Pressure">wikipedia</a>
/// </remarks>
public static class Pressure
{
    public const string Name = "pressure";
    public const string DefaultSymbol = "p";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "-1 " + Time.Dimension + "-2";
}
