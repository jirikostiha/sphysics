using System.Numerics;

namespace SPhysics;

/// <summary>
/// Moment of inertia quantity.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Moment_of_inertia">wikipedia</a>
/// <a href="https://mathworld.wolfram.com/MomentofInertia.html">wolfram</a>
/// <a href="https://en.wikipedia.org/wiki/Moment_of_inertia">wikipedia</a>
/// <a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a>
/// </remarks>
public static class MomentOfInertia
{
    public const string Name = "Moment of inertia";
    public const string DefaultSymbol = "I";
    public const string Dimension = Mass.Dimension + " " + Length.Dimension + "+2";

    public static N Eval<N>(N angularMomentum, N angularVelocity)
        where N : INumberBase<N>
        =>
        angularMomentum / angularVelocity;

    #region objects

    /// <summary> Point mass at a distance from the axis of rotation.. </summary>
    public static N Point<N>(N mass, N distanceRadius)
        where N : INumberBase<N>
        =>
        mass * (distanceRadius * distanceRadius);

    /// <summary> Two point masses at a distance from the axis of rotation.. </summary>
    public static N TwoPoints<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        (mass1 * mass2 / (mass1 + mass2)) * (distance * distance);

    /// <summary> Hollow sphere. </summary>
    //public static double Sphere(double radius, double mass) => SphereBody.MomentOfInertia(radius, mass);

    /// <summary> Solid sphere (ball). </summary>
    //public static double SolidSphere(double radius, double mass) => BallBody.MomentOfInertia(radius, mass);

    /// <summary>
    /// Sphere (shell) of radius r2 and mass m, with centered spherical cavity of radius r1.
    /// </summary>
  //  public static double SphereShell(double innerRadius, double outerRadius, double mass)
//        => SphericalShellBody.MomentOfInertia(innerRadius, outerRadius, mass);

    #endregion
}
