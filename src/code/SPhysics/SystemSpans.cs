using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Checks and sums over the spans that describe the bodies of a system, one item per body. </summary>
internal static class SystemSpans
{
    /// <summary> Throws unless <paramref name="other"/> is as long as <paramref name="length"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RequireLengths(
        int length,
        int other,
        [CallerArgumentExpression(nameof(length))] string? lengthName = null,
        [CallerArgumentExpression(nameof(other))] string? otherName = null)
    {
        if (other != length)
            ThrowLengthMismatch(otherName, lengthName);
    }

    /// <summary> Throws unless <paramref name="other1"/> and <paramref name="other2"/> are as long as <paramref name="length"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RequireLengths(
        int length,
        int other1,
        int other2,
        [CallerArgumentExpression(nameof(length))] string? lengthName = null,
        [CallerArgumentExpression(nameof(other1))] string? other1Name = null,
        [CallerArgumentExpression(nameof(other2))] string? other2Name = null)
    {
        RequireLengths(length, other1, lengthName, other1Name);
        RequireLengths(length, other2, lengthName, other2Name);
    }

    /// <summary> Throws unless the three other spans are as long as <paramref name="length"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RequireLengths(
        int length,
        int other1,
        int other2,
        int other3,
        [CallerArgumentExpression(nameof(length))] string? lengthName = null,
        [CallerArgumentExpression(nameof(other1))] string? other1Name = null,
        [CallerArgumentExpression(nameof(other2))] string? other2Name = null,
        [CallerArgumentExpression(nameof(other3))] string? other3Name = null)
    {
        RequireLengths(length, other1, other2, lengthName, other1Name, other2Name);
        RequireLengths(length, other3, lengthName, other3Name);
    }

    /// <summary> Sum of the vectors weighted by the masses, <c>sum(m * v)</c>, and the sum of the masses. </summary>
    public static ((N X, N Y) Sum, N Mass) MassWeighted<N>(ReadOnlySpan<(N X, N Y)> vectors, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        RequireLengths(vectors.Length, masses.Length);

        N totalMass = N.Zero;
        N x = N.Zero;
        N y = N.Zero;
        for (int i = 0; i < vectors.Length; i++)
        {
            totalMass += masses[i];
            x += vectors[i].X * masses[i];
            y += vectors[i].Y * masses[i];
        }

        return ((x, y), totalMass);
    }

    /// <summary> Sum of the vectors weighted by the masses, <c>sum(m * v)</c>, and the sum of the masses. </summary>
    public static ((N X, N Y, N Z) Sum, N Mass) MassWeighted<N>(ReadOnlySpan<(N X, N Y, N Z)> vectors, ReadOnlySpan<N> masses)
        where N : INumberBase<N>
    {
        RequireLengths(vectors.Length, masses.Length);

        N totalMass = N.Zero;
        N x = N.Zero;
        N y = N.Zero;
        N z = N.Zero;
        for (int i = 0; i < vectors.Length; i++)
        {
            totalMass += masses[i];
            x += vectors[i].X * masses[i];
            y += vectors[i].Y * masses[i];
            z += vectors[i].Z * masses[i];
        }

        return ((x, y, z), totalMass);
    }

    // kept out of the callers, which the check is inlined into
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowLengthMismatch(string? otherName, string? lengthName)
        => throw new ArgumentException($"{otherName} must equal {lengthName}: every span of the bodies has one item per body.");
}
