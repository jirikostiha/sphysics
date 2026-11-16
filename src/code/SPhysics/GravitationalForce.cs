using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Attraction force</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Gravity">wikipedia</a></remarks>
public static class GravitationalForce
{
    /// <summary>Evaluate gravitational force of two bodies above surfaces.</summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass1"> Mass of point 1 </param>
    /// <param name="mass2"> Mass of point 2 </param>
    /// <param name="distance"> Distance of points. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Outside<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-6.67430e-11) * mass1 * mass2 / (distance * distance);

    /// <summary>Evaluate gravitational force of two bodies above surfaces.</summary>
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

    /// <summary> Gravitational force on the body at <paramref name="index"/> from all the other bodies in a plane. </summary>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="index"> Index of the body the force acts on. </param>
    /// <param name="gravitationConst"> Gravitational constant, in the real world 6.67430e-11 m^3 kg^-1 s^-2. </param>
    /// <remarks> A body at the same place as the target, the target itself included, exerts no force. </remarks>
    public static (N Fx, N Fy) Eval<N>(ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses, int index, N gravitationConst)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        return Eval(positions[index], masses[index], positions, masses, gravitationConst);
    }

    /// <summary> Gravitational force on a target body from the bodies in a plane. </summary>
    /// <param name="target"> Position of the body the force acts on. </param>
    /// <param name="targetMass"> Mass of the body the force acts on. </param>
    /// <param name="positions"> Positions of the attracting bodies. </param>
    /// <param name="masses"> Masses of the attracting bodies, as many as there are positions. </param>
    /// <param name="gravitationConst"> Gravitational constant, in the real world 6.67430e-11 m^3 kg^-1 s^-2. </param>
    /// <remarks> A body at the same place as the target exerts no force, which leaves the target itself out if it is among them. </remarks>
    public static (N Fx, N Fy) Eval<N>((N X, N Y) target, N targetMass, ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses, N gravitationConst)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N Fx = N.Zero, Fy = N.Zero;
        N GmTarget = gravitationConst * targetMass; // Precompute constant G * mTarget for optimization

        for (int i = 0; i < positions.Length; i++)
        {
            N dx = positions[i].X - target.X;
            N dy = positions[i].Y - target.Y;
            N r2 = dx * dx + dy * dy;

            if (r2 == N.Zero) continue; // Avoid division by zero (the target itself, or a body at its place)

            N invR = N.One / N.Sqrt(r2);
            N invR3 = invR * invR * invR;
            N F_over_r = GmTarget * masses[i] * invR3; // Force magnitude divided by the distance, so multiplying by dx/dy gives the components

            Fx += F_over_r * dx; // Calculate the x-component of the force
            Fy += F_over_r * dy; // Calculate the y-component of the force
        }

        return (Fx, Fy); // Return the total gravitational force components
    }
}
