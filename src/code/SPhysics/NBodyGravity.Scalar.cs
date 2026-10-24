using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

public static partial class NBodyGravity
{
    /// <summary>Gravitational accelerations of bodies in tuples, evaluated pair by pair in scalar code, without copies.</summary>
    /// <remarks>
    /// Faster than the vectorized overloads of <see cref="NBodyGravity"/> for a few bodies, where copying the
    /// components costs more than the vectors save; slower for many bodies. The results differ from them in rounding.
    /// The sums run in a fixed order, so the results are reproducible on every machine.
    /// </remarks>
    public static class Scalar
    {
        /// <summary>Computes gravitational accelerations for all bodies in 2D pairwise (O(n^2)), a gravitational constant per body.</summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="positions"> Body positions. </param>
        /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
        /// <param name="gravitationalConstants"> Per-body gravitational constant (length must equal <paramref name="positions"/>). </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="positions"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<(N X, N Y)> positions,
            ReadOnlySpan<N> masses,
            ReadOnlySpan<N> gravitationalConstants,
            N softening,
            Span<(N X, N Y)> accelerations)
            where N : IRootFunctions<N>
        {
            RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
            PlanarScalar(positions, masses, gravitationalConstants, N.Zero, softening, accelerations, perBody: true);
        }

        /// <summary>Computes gravitational accelerations for all bodies in 2D pairwise (O(n^2)), one gravitational constant shared by every body.</summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="positions"> Body positions. </param>
        /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
        /// <param name="gravitationalConstant"> Gravitational constant. </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="positions"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<(N X, N Y)> positions,
            ReadOnlySpan<N> masses,
            N gravitationalConstant,
            N softening,
            Span<(N X, N Y)> accelerations)
            where N : IRootFunctions<N>
        {
            RequireLengths(positions.Length, masses.Length, accelerations.Length);
            PlanarScalar(positions, masses, default, gravitationalConstant, softening, accelerations, perBody: false);
        }

        /// <summary>Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)), a gravitational constant per body.</summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="positions"> Body positions. </param>
        /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
        /// <param name="gravitationalConstants"> Per-body gravitational constant (length must equal <paramref name="positions"/>). </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="positions"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<(N X, N Y, N Z)> positions,
            ReadOnlySpan<N> masses,
            ReadOnlySpan<N> gravitationalConstants,
            N softening,
            Span<(N X, N Y, N Z)> accelerations)
            where N : IRootFunctions<N>
        {
            RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
            SpatialScalar(positions, masses, gravitationalConstants, N.Zero, softening, accelerations, perBody: true);
        }

        /// <summary>Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)), one gravitational constant shared by every body.</summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="positions"> Body positions. </param>
        /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
        /// <param name="gravitationalConstant"> Gravitational constant. </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="positions"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<(N X, N Y, N Z)> positions,
            ReadOnlySpan<N> masses,
            N gravitationalConstant,
            N softening,
            Span<(N X, N Y, N Z)> accelerations)
            where N : IRootFunctions<N>
        {
            RequireLengths(positions.Length, masses.Length, accelerations.Length);
            SpatialScalar(positions, masses, default, gravitationalConstant, softening, accelerations, perBody: false);
        }
    }

    // 1 / r^3 from the softened squared distance
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static N InverseCube<N>(N squaredDistance)
        where N : IRootFunctions<N>
    {
        N invR = N.One / N.Sqrt(squaredDistance);
        return invR * invR * invR;
    }

    private static void PlanarScalar<N>(
        ReadOnlySpan<(N X, N Y)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N gravitationalConstant,
        N softening,
        Span<(N X, N Y)> accelerations,
        bool perBody)
        where N : IRootFunctions<N>
    {
        int n = positions.Length;
        N soft2 = softening * softening;

        accelerations.Clear();

        for (int i = 0; i < n; i++)
        {
            var pi = positions[i];
            N gI = perBody ? gravitationalConstants[i] : gravitationalConstant;
            N mI = masses[i];
            var (aix, aiy) = accelerations[i];

            for (int j = i + 1; j < n; j++)
            {
                var pj = positions[j];
                N rx = pj.X - pi.X;
                N ry = pj.Y - pi.Y;
                N invR3 = InverseCube(rx * rx + ry * ry + soft2);

                N kI = gI * masses[j] * invR3;
                aix += rx * kI;
                aiy += ry * kI;

                N kJ = (perBody ? gravitationalConstants[j] : gravitationalConstant) * mI * invR3;
                ref var aj = ref accelerations[j];
                aj.X -= rx * kJ;
                aj.Y -= ry * kJ;
            }

            accelerations[i] = (aix, aiy);
        }
    }

    private static void SpatialScalar<N>(
        ReadOnlySpan<(N X, N Y, N Z)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N gravitationalConstant,
        N softening,
        Span<(N X, N Y, N Z)> accelerations,
        bool perBody)
        where N : IRootFunctions<N>
    {
        int n = positions.Length;
        N soft2 = softening * softening;

        accelerations.Clear();

        for (int i = 0; i < n; i++)
        {
            var pi = positions[i];
            N gI = perBody ? gravitationalConstants[i] : gravitationalConstant;
            N mI = masses[i];
            var (aix, aiy, aiz) = accelerations[i];

            for (int j = i + 1; j < n; j++)
            {
                var pj = positions[j];
                N rx = pj.X - pi.X;
                N ry = pj.Y - pi.Y;
                N rz = pj.Z - pi.Z;
                N invR3 = InverseCube(rx * rx + ry * ry + rz * rz + soft2);

                N kI = gI * masses[j] * invR3;
                aix += rx * kI;
                aiy += ry * kI;
                aiz += rz * kI;

                N kJ = (perBody ? gravitationalConstants[j] : gravitationalConstant) * mI * invR3;
                ref var aj = ref accelerations[j];
                aj.X -= rx * kJ;
                aj.Y -= ry * kJ;
                aj.Z -= rz * kJ;
            }

            accelerations[i] = (aix, aiy, aiz);
        }
    }
}
