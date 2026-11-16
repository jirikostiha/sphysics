using System.Numerics;
using System.Runtime.CompilerServices;

using SMath.Geometry2D;
using SMath.Geometry3D;

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
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass1, N mass2, N distance, N gravitationalConstant)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>, IUnaryNegationOperators<N, N>
        =>
        -gravitationalConstant * mass1 * mass2 / distance;

    /// <summary>
    /// Softened gravitational energy of two bodies, <c>-G * m1 * m2 / sqrt(r^2 + softening^2)</c>, the energy of the
    /// accelerations <see cref="NBodyGravity"/> computes with the same softening.
    /// </summary>
    /// <param name="mass1"> Mass of body 1. </param>
    /// <param name="mass2"> Mass of body 2. </param>
    /// <param name="distanceSquared"> Squared distance of the bodies, so callers that already have it skip a square root. </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="gravitationalConstant"> Gravitational constant. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Softened<N>(N mass1, N mass2, N distanceSquared, N softening, N gravitationalConstant)
        where N : IRootFunctions<N>
        =>
        mass1 * GravitationalPotential.Softened(mass2, distanceSquared, softening, gravitationalConstant);

    /// <summary> Total softened gravitational energy of every pair of bodies in a plane (O(n^2)). </summary>
    /// <remarks> Two bodies at the same place without softening hold no finite energy and are left out. </remarks>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    /// <param name="softening"> Softening length, zero for the plain Newtonian energy. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y)> positions, ReadOnlySpan<N> masses, N gravitationalConstant, N softening)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            // potential of the later bodies at body i, so its mass multiplies once
            N potential = N.Zero;
            for (int j = i + 1; j < positions.Length; j++)
            {
                var separation = GeometricVector2.Direction.FromCartesian(positions[i], positions[j]);
                N r2 = GeometricVector2.DotProduct.FromCartesian(separation, separation);
                if (r2 != N.Zero || softening != N.Zero)
                    potential += GravitationalPotential.Softened(masses[j], r2, softening, gravitationalConstant);
            }

            total += masses[i] * potential;
        }

        return total;
    }

    /// <summary> Total softened gravitational energy of every pair of bodies in space (O(n^2)). </summary>
    /// <remarks> Two bodies at the same place without softening hold no finite energy and are left out. </remarks>
    /// <param name="positions"> Positions of the bodies. </param>
    /// <param name="masses"> Masses of the bodies, as many as there are positions. </param>
    /// <param name="gravitationalConstant"> Gravitational constant, in the real world <see cref="GravitationalConstant.Value"/>. </param>
    /// <param name="softening"> Softening length, zero for the plain Newtonian energy. </param>
    public static N Total<N>(ReadOnlySpan<(N X, N Y, N Z)> positions, ReadOnlySpan<N> masses, N gravitationalConstant, N softening)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        N total = N.Zero;
        for (int i = 0; i < positions.Length; i++)
        {
            // potential of the later bodies at body i, so its mass multiplies once
            N potential = N.Zero;
            for (int j = i + 1; j < positions.Length; j++)
            {
                var separation = GeometricVector3.Direction.FromCartesian(positions[i], positions[j]);
                N r2 = GeometricVector3.DotProduct.FromCartesian(separation, separation);
                if (r2 != N.Zero || softening != N.Zero)
                    potential += GravitationalPotential.Softened(masses[j], r2, softening, gravitationalConstant);
            }

            total += masses[i] * potential;
        }

        return total;
    }
}
