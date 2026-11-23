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
/// <para>
/// <b>Cutoff.</b> The overloads with a cutoff skip every pair of bodies farther apart than it: neither pulls on the
/// other. The distance compared is the plain one, r, not the softened one, and a pair exactly at the cutoff still
/// pulls. Positive infinity skips no pair and gives the same accelerations as the overloads without a cutoff; 0 skips
/// every pair apart. In the vectorized kernel a pair beyond the cutoff costs as much as one within it, so the cutoff
/// limits the reach of gravity, not the time of an evaluation; the scalar kernel skips such a pair.
/// </para>
/// <para>
/// Gravity has no range in nature. The cutoff is a numerical tool, as the softening is, with which a simulation
/// limits the reach of gravity, as molecular dynamics cuts short-range forces off. It is the one exception to the rule
/// that this library holds only real physics, made so that a limited reach runs in the vectorized kernel too.
/// </para>
/// <para>
/// The pull drops to zero at the cutoff, so the plain potential energy of a pair would jump there. A simulation that
/// balances energy shifts the potential of every pair within the cutoff by its value at the cutoff: the shift leaves
/// the accelerations as they are and keeps the energy of a pair that crosses the cutoff.
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
        SystemSpans.RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
        Planar(positions, masses, gravitationalConstants, N.Zero, softening, Cutoff<N>.None, accelerations, perBody: true);
    }

    /// <summary>
    /// Computes gravitational accelerations for all bodies pairwise (O(n^2)), a gravitational constant per body,
    /// each body pulled only by the bodies within <paramref name="cutoff"/>.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Body positions. </param>
    /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
    /// <param name="gravitationalConstants"> Per-body gravitational constant (length must equal <paramref name="positions"/>). </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="cutoff">
    /// Farthest distance at which two bodies still pull on each other; a pair farther apart is skipped, positive
    /// infinity skips none. A numerical cutoff, not a law of physics: see the remarks of <see cref="NBodyGravity"/>.
    /// </param>
    /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="cutoff"/> is negative or NaN. </exception>
    public static void ComputeAccelerations<N>(
        ReadOnlySpan<(N X, N Y)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N softening,
        N cutoff,
        Span<(N X, N Y)> accelerations)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
        Planar(positions, masses, gravitationalConstants, N.Zero, softening, Cutoff<N>.Of(cutoff), accelerations, perBody: true);
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
        SystemSpans.RequireLengths(positions.Length, masses.Length, accelerations.Length);
        Planar(positions, masses, default, gravitationalConstant, softening, Cutoff<N>.None, accelerations, perBody: false);
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
        SystemSpans.RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
        Spatial(positions, masses, gravitationalConstants, N.Zero, softening, Cutoff<N>.None, accelerations, perBody: true);
    }

    /// <summary>
    /// Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)), a gravitational constant per body,
    /// each body pulled only by the bodies within <paramref name="cutoff"/>.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Body positions. </param>
    /// <param name="masses"> Body masses (length must equal <paramref name="positions"/>). </param>
    /// <param name="gravitationalConstants"> Per-body gravitational constant (length must equal <paramref name="positions"/>). </param>
    /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
    /// <param name="cutoff">
    /// Farthest distance at which two bodies still pull on each other; a pair farther apart is skipped, positive
    /// infinity skips none. A numerical cutoff, not a law of physics: see the remarks of <see cref="NBodyGravity"/>.
    /// </param>
    /// <param name="accelerations"> Output buffer (length must equal <paramref name="positions"/>). </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="cutoff"/> is negative or NaN. </exception>
    public static void ComputeAccelerations<N>(
        ReadOnlySpan<(N X, N Y, N Z)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N softening,
        N cutoff,
        Span<(N X, N Y, N Z)> accelerations)
        where N : IRootFunctions<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length, gravitationalConstants.Length, accelerations.Length);
        Spatial(positions, masses, gravitationalConstants, N.Zero, softening, Cutoff<N>.Of(cutoff), accelerations, perBody: true);
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
        SystemSpans.RequireLengths(positions.Length, masses.Length, accelerations.Length);
        Spatial(positions, masses, default, gravitationalConstant, softening, Cutoff<N>.None, accelerations, perBody: false);
    }

    // the components are copied into one pooled buffer, x, y, then the accelerations
    private static void Planar<N>(
        ReadOnlySpan<(N X, N Y)> positions,
        ReadOnlySpan<N> masses,
        ReadOnlySpan<N> gravitationalConstants,
        N gravitationalConstant,
        N softening,
        Cutoff<N> cutoff,
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

            Kernel<N>(x, y, default, masses, gravitationalConstants, gravitationalConstant, softening, cutoff, ax, ay, default, perBody, spatial: false);

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
        Cutoff<N> cutoff,
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

            Kernel<N>(x, y, z, masses, gravitationalConstants, gravitationalConstant, softening, cutoff, ax, ay, az, perBody, spatial: true);

            for (int i = 0; i < n; i++)
                accelerations[i] = (ax[i], ay[i], az[i]);
        }
        finally
        {
            ArrayPool<N>.Shared.Return(buffer, RuntimeHelpers.IsReferenceOrContainsReferences<N>());
        }
    }
}
