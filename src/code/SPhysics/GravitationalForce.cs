using System.Numerics;
using System.Runtime.CompilerServices;

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
    /// Evaluate gravitational force of two bodies above surfaces.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass1"> Mass of point 1 </param>
    /// <param name="mass2"> Mass of point 2 </param>
    /// <param name="distance"> Distance of points. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Outside<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-6.67430e-11) * mass1 * mass2 / (distance * distance);

    /// <summary>
    /// Evaluate gravitational force of two bodies above surfaces.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass1"> Mass of point 1 </param>
    /// <param name="mass2"> Mass of point 2 </param>
    /// <param name="distance"> Distance of points. </param>
    /// <param name="gravitationConst"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Outside<N>(N mass1, N mass2, N distance, N gravitationConst)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>, IUnaryNegationOperators<N, N>
        =>
        -gravitationConst * mass1 * mass2 / (distance * distance);

    ///// <summary>
    ///// Under the surface of homogenous body.
    ///// Neni presny, pze r2 a mass2 se nebere v potaz a pocita se jako bod asi se stejnou hustotou.
    ///// </summary>
    ///// <remarks>
    ///// <a href="https://profoundphysics.com/gravity-underground/">source</a>
    ///// </remarks>
    //public static N Inside<N>(N distance, N density, N gravitationConst)
    //    where N : ITrigonometricFunctions<N>
    //    => N.CreateTruncating(4 / 3) * N.Pi * gravitationConst * (density * distance); //todo test

    ///// <summary>
    ///// Outside and inside. Inside is counted as homogenous.
    ///// Neni presny, pze r2 se nebere v potaz a pocita se jako hmotny bod.
    ///// </summary>
    //public static N Anywhere<N>(N mass1, N mass2, N radius1, N radius2, N distance, N gravitationConst)
    //    where N : INumberBase<N>
    //    =>
    //    distance > Max(radius1, radius2)
    //        ? Outside(mass1, mass2, distance, gravitationConst)
    //        : mass1 >= mass2 //take more masive body as primary
    //            ? Inside(distance, Density.Average(Ball.Volume(radius1), mass1), gravitationConst)
    //            : Inside(distance, Density.Average(Ball.Volume(radius2), mass2), gravitationConst); // je spravne?


    /// <summary>
    /// Evaluate gravitational force to target point defined by index in collection of other points.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="points"> All points. </param>
    /// <param name="index"> Target point index. </param>
    /// <param name="gravitationConst"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    public static (N Fx, N Fy) Eval<N>(Span<(N X, N Y, N Mass)> points, int index, N gravitationConst)
        where N : INumberBase<N>
    {
        N Fx = N.Zero, Fy = N.Zero;

        var (x0, y0, m0) = points[index];
        N Gm0 = gravitationConst * m0; // Precompute constant G * m0 for optimization

        for (int i = 0; i < points.Length; i++)
        {
            if (i == index) continue; // Skip the body itself (no self-gravity)

            var (xi, yi, mi) = points[i];

            N dx = xi - x0;
            N dy = yi - y0;
            N r2 = dx * dx + dy * dy;

            if (r2 == N.Zero) continue; // Avoid division by zero (in case two bodies are at the same position)

            N invR2 = N.One / r2; // Inverse square of the distance for efficiency
            N F_over_r = Gm0 * mi * invR2; // Compute gravitational force divided by distance (no need for square root)

            Fx += F_over_r * dx; // Calculate the x-component of the force
            Fy += F_over_r * dy; // Calculate the y-component of the force
        }

        return (Fx, Fy); // Return the total gravitational force components
    }

    /// <summary>
    /// Evaluate gravitational force to target point from other points.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="targetPoint"> Target point </param>
    /// <param name="points"> All points </param>
    /// <param name="gravitationConst"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    public static (N Fx, N Fy) Eval<N>((N X, N Y, N Mass) targetPoint, Span<(N X, N Y, N Mass)> points, N gravitationConst)
        where N : INumberBase<N>
    {
        N Fx = N.Zero, Fy = N.Zero;

        var (xTarget, yTarget, massTarget) = targetPoint;
        N GmTarget = gravitationConst * massTarget; // Precompute constant G * mTarget for optimization

        for (int i = 0; i < points.Length; i++)
        {
            var (xOther, yOther, massOther) = points[i];

            if (xTarget == xOther && yTarget == yOther) continue; // Skip the target point (no self-gravity)

            N dx = xOther - xTarget;
            N dy = yOther - yTarget;
            N r2 = dx * dx + dy * dy;

            if (r2 == N.Zero) continue; // Avoid division by zero (in case two points are at the same position)

            N invR2 = N.One / r2; // Inverse square of the distance for efficiency
            N F_over_r = GmTarget * massOther * invR2; // Compute gravitational force divided by distance (no need for square root)

            Fx += F_over_r * dx; // Calculate the x-component of the force
            Fy += F_over_r * dy; // Calculate the y-component of the force
        }

        return (Fx, Fy); // Return the total gravitational force components
    }
}
