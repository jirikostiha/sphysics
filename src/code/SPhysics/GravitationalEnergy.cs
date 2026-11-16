using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Gravitational potential energy</summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Gravitational_energy">wikipedia</a>
/// <a href="https://www.calculatorsoup.com/calculators/physics/gravitational-potential.php">calculator</a>
/// <a href="https://cnx.org/contents/1AJmkLE0@2.9:nDPdI3O4@3/Gravitational-potential-energy"> cnx</a>
/// </remarks>
public static class GravitationalEnergy
{
    /// <summary>Evaluate gravitational energy of two bodies.</summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass1"> Mass of point 1 </param>
    /// <param name="mass2"> Mass of point 2 </param>
    /// <param name="distance"> Distance of points. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-GravitationalConstant.Value) * mass1 * mass2 / distance;

    /// <summary>Evaluate gravitational energy of two bodies.</summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass1"> Mass of point 1 </param>
    /// <param name="mass2"> Mass of point 2 </param>
    /// <param name="distance"> Distance of points. </param>
    /// /// <param name="gravitationConst"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass1, N mass2, N distance, N gravitationConst)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>, IUnaryNegationOperators<N, N>
        =>
        -gravitationConst * mass1 * mass2 / distance;

    /// <summary>
    /// Softened gravitational energy of two bodies, <c>-G * m1 * m2 / sqrt(r^2 + softening^2)</c>, the energy of the
    /// accelerations <see cref="NBodyGravity"/> computes with the same softening.
    /// </summary>
    /// <param name="mass1"> Mass of body 1. </param>
    /// <param name="mass2"> Mass of body 2. </param>
    /// <param name="distanceSquared"> Squared distance of the bodies, so callers that already have it skip a square root. </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="gravitationConst"> Gravitational constant. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Softened<N>(N mass1, N mass2, N distanceSquared, N softening, N gravitationConst)
        where N : IRootFunctions<N>
        =>
        mass1 * GravitationalPotential.Softened(mass2, distanceSquared, softening, gravitationConst);

    /// <summary> Total softened gravitational energy of every pair of bodies in a plane (O(n^2)). </summary>
    /// <remarks> Two bodies at the same place without softening hold no finite energy and are left out. </remarks>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="gravitationConst"> Gravitational constant, in the real world 6.67430e-11 m^3 kg^-1 s^-2. </param>
    /// <param name="softening"> Softening length, zero for the plain Newtonian energy. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses, N gravitationConst, N softening)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N softeningSquared = softening * softening;
        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = i + 1; j < positions.Length; j++)
            {
                N dx = positions[j].X - positions[i].X;
                N dy = positions[j].Y - positions[i].Y;
                N r2 = (dx * dx) + (dy * dy);
                if (r2 + softeningSquared != N.Zero)
                    total += Softened(masses[i], masses[j], r2, softening, gravitationConst);
            }
        }

        return total;
    }

    /// <summary> Total softened gravitational energy of every pair of bodies in space (O(n^2)). </summary>
    /// <remarks> Two bodies at the same place without softening hold no finite energy and are left out. </remarks>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="gravitationConst"> Gravitational constant, in the real world 6.67430e-11 m^3 kg^-1 s^-2. </param>
    /// <param name="softening"> Softening length, zero for the plain Newtonian energy. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y, N Z)> positions, ReadOnlySpan<N> masses, N gravitationConst, N softening)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N softeningSquared = softening * softening;
        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            for (int j = i + 1; j < positions.Length; j++)
            {
                N dx = positions[j].X - positions[i].X;
                N dy = positions[j].Y - positions[i].Y;
                N dz = positions[j].Z - positions[i].Z;
                N r2 = (dx * dx) + (dy * dy) + (dz * dz);
                if (r2 + softeningSquared != N.Zero)
                    total += Softened(masses[i], masses[j], r2, softening, gravitationConst);
            }
        }

        return total;
    }
}
