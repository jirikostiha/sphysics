using System.Numerics;

namespace SPhysics;

/// <summary>
/// N-body gravitational acceleration with Plummer-style softening.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/N-body_simulation">wikipedia</a>
/// </remarks>
public static class NBodyGravity
{
    /// <summary>
    /// Computes gravitational accelerations for all bodies pairwise (O(n^2)).
    /// Uses Newton's third law so each pair is evaluated once.
    /// A per-body gravitational constant lets bodies in different regions/environments
    /// experience different G values (positional G-field), matching CpuGravitySolver semantics.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Body positions. </param>
    /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
    /// <param name="gravitationalConstants"> Per-body gravitational constant (length must equal <paramref name="positions"/>). </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
    public static void ComputeAccelerations<N>(
        ReadOnlySpan<(N X, N Y)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N softening,
        Span<(N X, N Y)> accelerations)
        where N : IRootFunctions<N>
    {
        int n = positions.Length;
        if (masses.Length != n || gravitationalConstants.Length != n || accelerations.Length != n)
            throw new ArgumentException("masses, gravitationalConstants, and accelerations must have the same length as positions.");

        N soft2 = softening * softening;

        for (int i = 0; i < n; i++)
            accelerations[i] = (N.Zero, N.Zero);

        for (int i = 0; i < n; i++)
        {
            var pi = positions[i];
            N gI = gravitationalConstants[i];
            var (aix, aiy) = accelerations[i];

            for (int j = i + 1; j < n; j++)
            {
                var pj = positions[j];
                N rx = pj.X - pi.X;
                N ry = pj.Y - pi.Y;
                N d2 = rx * rx + ry * ry + soft2;
                N invR = N.One / N.Sqrt(d2);
                N invR3 = invR * invR * invR;

                N kI = gI * masses[j] * invR3;
                aix += rx * kI;
                aiy += ry * kI;

                N kJ = gravitationalConstants[j] * masses[i] * invR3;
                var aj = accelerations[j];
                accelerations[j] = (aj.X - rx * kJ, aj.Y - ry * kJ);
            }

            accelerations[i] = (aix, aiy);
        }
    }

    /// <summary>
    /// Computes gravitational accelerations for all bodies pairwise (O(n^2)) with a single
    /// gravitational constant shared by every body.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Body positions. </param>
    /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
    /// <param name="gravitationalConstant"> Gravitational constant. </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
    public static void ComputeAccelerations<N>(
        ReadOnlySpan<(N X, N Y)> positions,
        ReadOnlySpan<N> masses,
        N gravitationalConstant,
        N softening,
        Span<(N X, N Y)> accelerations)
        where N : IRootFunctions<N>
    {
        int n = positions.Length;
        if (masses.Length != n || accelerations.Length != n)
            throw new ArgumentException("masses and accelerations must have the same length as positions.");

        N soft2 = softening * softening;

        for (int i = 0; i < n; i++)
            accelerations[i] = (N.Zero, N.Zero);

        for (int i = 0; i < n; i++)
        {
            var pi = positions[i];
            var (aix, aiy) = accelerations[i];

            for (int j = i + 1; j < n; j++)
            {
                var pj = positions[j];
                N rx = pj.X - pi.X;
                N ry = pj.Y - pi.Y;
                N d2 = rx * rx + ry * ry + soft2;
                N invR = N.One / N.Sqrt(d2);
                N invR3 = invR * invR * invR;

                N kI = gravitationalConstant * masses[j] * invR3;
                aix += rx * kI;
                aiy += ry * kI;

                N kJ = gravitationalConstant * masses[i] * invR3;
                var aj = accelerations[j];
                accelerations[j] = (aj.X - rx * kJ, aj.Y - ry * kJ);
            }

            accelerations[i] = (aix, aiy);
        }
    }
}
