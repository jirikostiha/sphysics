using System.Numerics;

namespace SPhysics;

/// <summary>
/// Attraction force
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Gravity">wikipedia</a>
/// </remarks>
public static class GravitationalForce
{
    /// <summary>
    /// Attraction force of two bodies above surface.
    /// </summary>
    public static N Outside<N>(N mass1, N mass2, N distance, N gravitationConst)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        mass1 * mass2 * gravitationConst / (distance * distance);

    /// <summary>
    /// Under the surface of homogenous body.
    /// Neni presny, pze r2 a mass2 se nebere v potaz a pocita se jako bod asi se stejnou hustotou.
    /// </summary>
    /// <remarks>
    /// <a href="https://profoundphysics.com/gravity-underground/">source</a>
    /// </remarks>
    public static N Inside<N>(N distance, N density, N gravitationConst)
        where N : ITrigonometricFunctions<N>
        => N.CreateTruncating(4 / 3) * N.Pi * gravitationConst * (density * distance); //todo test

    /// <summary>
    /// Outside and inside. Inside is counted as homogenous.
    /// Neni presny, pze r2 se nebere v potaz a pocita se jako hmotny bod.
    /// </summary>
    //public static N Anywhere<N>(N mass1, N mass2, N radius1, N radius2, N distance, N gravitationConst)
    //    where N : INumberBase<N>
    //    =>
    //    distance > Max(radius1, radius2)
    //        ? Outside(mass1, mass2, distance, gravitationConst)
    //        : mass1 >= mass2 //take more masive body as primary
    //            ? Inside(distance, Density.Average(Ball.Volume(radius1), mass1), gravitationConst)
    //            : Inside(distance, Density.Average(Ball.Volume(radius2), mass2), gravitationConst); // je spravne?
}
