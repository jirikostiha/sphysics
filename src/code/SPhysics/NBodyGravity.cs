using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>N-body gravitational acceleration with Plummer-style softening.</summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/N-body_simulation">wikipedia</a>
/// <para>
/// The overloads over tuples copy the components into separate spans and run the vectorized kernel of <see cref="Soa"/>;
/// the copies cost O(n) against the O(n^2) pairs.
/// </para>
/// </remarks>
public static partial class NBodyGravity
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
        RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
        Planar(positions, masses, gravitationalConstants, N.Zero, softening, accelerations, perBody: true);
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
        RequireLengths(positions.Length, masses.Length, accelerations.Length);
        Planar(positions, masses, default, gravitationalConstant, softening, accelerations, perBody: false);
    }

    /// <summary>
    /// Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)).
    /// Uses Newton's third law so each pair is evaluated once.
    /// A per-body gravitational constant lets bodies in different regions/environments
    /// experience different G values (positional G-field).
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Body positions. </param>
    /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
    /// <param name="gravitationalConstants"> Per-body gravitational constant (length must equal <paramref name="positions"/>). </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
    public static void ComputeAccelerations<N>(
        ReadOnlySpan<(N X, N Y, N Z)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N softening,
        Span<(N X, N Y, N Z)> accelerations)
        where N : IRootFunctions<N>
    {
        RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
        Spatial(positions, masses, gravitationalConstants, N.Zero, softening, accelerations, perBody: true);
    }

    /// <summary>
    /// Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)) with a single
    /// gravitational constant shared by every body.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Body positions. </param>
    /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
    /// <param name="gravitationalConstant"> Gravitational constant. </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
    public static void ComputeAccelerations<N>(
        ReadOnlySpan<(N X, N Y, N Z)> positions,
        ReadOnlySpan<N> masses,
        N gravitationalConstant,
        N softening,
        Span<(N X, N Y, N Z)> accelerations)
        where N : IRootFunctions<N>
    {
        RequireLengths(positions.Length, masses.Length, accelerations.Length);
        Spatial(positions, masses, default, gravitationalConstant, softening, accelerations, perBody: false);
    }

    private static void RequireLengths(int positions, int masses, int gravitationalConstants, int accelerations)
    {
        if (masses != positions || gravitationalConstants != positions || accelerations != positions)
            throw new ArgumentException("masses, gravitationalConstants, and accelerations must have the same length as positions.");
    }

    private static void RequireLengths(int positions, int masses, int accelerations)
    {
        if (masses != positions || accelerations != positions)
            throw new ArgumentException("masses and accelerations must have the same length as positions.");
    }

    // the components are copied into one pooled buffer, x, y, then the accelerations
    private static void Planar<N>(
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
        N[] buffer = ArrayPool<N>.Shared.Rent(4 * n);
        try
        {
            var x = buffer.AsSpan(0, n);
            var y = buffer.AsSpan(n, n);
            var ax = buffer.AsSpan(2 * n, n);
            var ay = buffer.AsSpan(3 * n, n);
            for (int i = 0; i < n; i++)
                (x[i], y[i]) = positions[i];

            Kernel<N>(x, y, default, masses, gravitationalConstants, gravitationalConstant, softening, ax, ay, default, perBody, spatial: false);

            for (int i = 0; i < n; i++)
                accelerations[i] = (ax[i], ay[i]);
        }
        finally
        {
            ArrayPool<N>.Shared.Return(buffer, RuntimeHelpers.IsReferenceOrContainsReferences<N>());
        }
    }

    // the components are copied into one pooled buffer, x, y, z, then the accelerations
    private static void Spatial<N>(
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
        N[] buffer = ArrayPool<N>.Shared.Rent(6 * n);
        try
        {
            var x = buffer.AsSpan(0, n);
            var y = buffer.AsSpan(n, n);
            var z = buffer.AsSpan(2 * n, n);
            var ax = buffer.AsSpan(3 * n, n);
            var ay = buffer.AsSpan(4 * n, n);
            var az = buffer.AsSpan(5 * n, n);
            for (int i = 0; i < n; i++)
                (x[i], y[i], z[i]) = positions[i];

            Kernel<N>(x, y, z, masses, gravitationalConstants, gravitationalConstant, softening, ax, ay, az, perBody, spatial: true);

            for (int i = 0; i < n; i++)
                accelerations[i] = (ax[i], ay[i], az[i]);
        }
        finally
        {
            ArrayPool<N>.Shared.Return(buffer, RuntimeHelpers.IsReferenceOrContainsReferences<N>());
        }
    }
}
