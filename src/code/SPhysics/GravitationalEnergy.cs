using SMath.GeometryD2;
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
    public static N Eval<N>(N mass1, N mass2, N distance)
        where N : INumberBase<N>
        =>
        //const double G = 6.67430e-11; // m^3 kg^-1 s^-2
        N.CreateTruncating(-6.67430e-11) * mass1 * mass2 / distance;

    public static N Eval<N>(N mass1, N mass2, N distance, N gravitationalConst)
        where N : INumberBase<N>
        =>
        N.CreateTruncating(-gravitationalConst) * mass1 * mass2 / distance;

    public static N Total<N>((N x, N y, N m)[] points)
        where N : IRootFunctions<N>
    {
        int n = points.Length;
        N totalEnergy = N.Zero;

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                N dx = points[i].x - points[j].x;
                N dy = points[i].y - points[j].y;
                N distance = Point2.Distance((dx, dy));

                totalEnergy += Eval(points[i].m, points[j].m, distance);
            }
        }

        return totalEnergy;
    }
}
