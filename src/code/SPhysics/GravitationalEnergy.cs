using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>
/// Gravitational potential energy
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Gravitational_energy">wikipedia</a>
/// <a href="https://www.calculatorsoup.com/calculators/physics/gravitational-potential.php">calculator</a>
/// <a href="https://cnx.org/contents/1AJmkLE0@2.9:nDPdI3O4@3/Gravitational-potential-energy"> cnx</a>
/// </remarks>
public static class GravitationalEnergy
{
    /// <summary>
    /// Evaluate gravitational energy of two bodies.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="mass1"> Mass of point 1 </param>
    /// <param name="mass2"> Mass of point 2 </param>
    /// <param name="distance"> Distance of points. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-6.67430e-11) * mass1 * mass2 / distance;

    /// <summary>
    /// Evaluate gravitational energy of two bodies.
    /// </summary>
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
    /// Total gravitational energy of the system.
    /// </summary>
    /// <typeparam name="N"> Number type </typeparam>
    /// <param name="points"> All points </param>
    /// /// <param name="gravitationConst"> constant in real world G = 6.67430e-11; // m^3 kg^-1 s^-2 </param>
    public static N Total<N>(Span<(N X, N Y, N Mass)> points, N gravitationConst)
        where N : INumberBase<N>, IRootFunctions<N>
    {
        N totalEnergy = N.Zero;

        for (int i = 0; i < points.Length; i++)
        {
            var (x0, y0, m0) = points[i]; // Mass and position of the first body

            for (int j = i + 1; j < points.Length; j++)
            {
                var (x1, y1, m1) = points[j]; // Mass and position of the second body

                N dx = x1 - x0; // X distance component
                N dy = y1 - y0; // Y distance component
                N r2 = dx * dx + dy * dy; // Squared distance between the two bodies (avoid sqrt)

                if (r2 == N.Zero) continue; // Skip if two bodies are at the same location (handle very close bodies)

                // Calculate the gravitational potential energy between the two bodies
                // Using r2 directly in the energy formula to avoid sqrt calculation
                N energy = Eval(m0, m1, r2, gravitationConst);

                totalEnergy += energy;
            }
        }

        return totalEnergy; // Return the total gravitational potential energy of the system
    }
}
