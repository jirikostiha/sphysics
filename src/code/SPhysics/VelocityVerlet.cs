using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Velocity Verlet integration primitives.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Verlet_integration#Velocity_Verlet">wikipedia</a></remarks>
public static partial class VelocityVerlet
{
    /// <summary>Half-kick: v += 0.5 * a * dt for a single body.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y) HalfKick<N>((N X, N Y) velocity, (N X, N Y) acceleration, N dt)
        where N : INumberBase<N>
    {
        N h = HalfStep(dt);
        return (velocity.X + acceleration.X * h, velocity.Y + acceleration.Y * h);
    }

    /// <summary>Drift: x += v * dt for a single body.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y) Drift<N>((N X, N Y) position, (N X, N Y) velocity, N dt)
        where N : INumberBase<N>
    {
        return (position.X + velocity.X * dt, position.Y + velocity.Y * dt);
    }

    /// <summary>Half-kick applied to every element: v[i] += 0.5 * a[i] * dt.</summary>
    public static void HalfKick<N>(Span<(N X, N Y)> velocities, ReadOnlySpan<(N X, N Y)> accelerations, N dt)
        where N : INumberBase<N>
    {
        if (accelerations.Length != velocities.Length)
            throw new ArgumentException("accelerations must have the same length as velocities.");
        N h = HalfStep(dt);
        for (int i = 0; i < velocities.Length; i++)
        {
            var v = velocities[i];
            var a = accelerations[i];
            velocities[i] = (v.X + a.X * h, v.Y + a.Y * h);
        }
    }

    /// <summary>Drift applied to every element: x[i] += v[i] * dt.</summary>
    public static void Drift<N>(Span<(N X, N Y)> positions, ReadOnlySpan<(N X, N Y)> velocities, N dt)
        where N : INumberBase<N>
    {
        if (velocities.Length != positions.Length)
            throw new ArgumentException("velocities must have the same length as positions.");
        for (int i = 0; i < positions.Length; i++)
            positions[i] = Drift(positions[i], velocities[i], dt);
    }

    /// <summary>
    /// One velocity-Verlet step for an N-body gravity system with softening.
    /// Computes a(x) once, then per sub-step: v += 0.5*a*h, x += v*h, compute a(x), v += 0.5*a*h.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Positions (modified in place). </param>
    /// <param name="velocities"> Velocities (modified in place). </param>
    /// <param name="accelerations"> Scratch acceleration buffer (must be same length as positions). </param>
    /// <param name="masses"> Body masses. </param>
    /// <param name="gravitationalConstant"> Gravitational constant. </param>
    /// <param name="softening"> Softening length. </param>
    /// <param name="timeDelta"> Total time delta to advance. </param>
    /// <param name="subSteps"> Number of equal-sized sub-steps to split the delta into. </param>
    public static void StepGravity<N>(
        Span<(N X, N Y)> positions,
        Span<(N X, N Y)> velocities,
        Span<(N X, N Y)> accelerations,
        ReadOnlySpan<N> masses,
        N gravitationalConstant,
        N softening,
        N timeDelta,
        int subSteps = 1)
        where N : IRootFunctions<N>
    {
        if (subSteps < 1)
            throw new ArgumentOutOfRangeException(nameof(subSteps), subSteps, "subSteps must be at least 1.");

        int n = positions.Length;
        if (n < 2)
            return;

        N h = timeDelta / N.CreateTruncating(subSteps);

        // The accelerations at the end of a sub-step are the ones the next sub-step starts from.
        NBodyGravity.ComputeAccelerations<N>(positions, masses, gravitationalConstant, softening, accelerations);
        for (int s = 0; s < subSteps; s++)
        {
            HalfKick(velocities, accelerations, h);
            Drift(positions, velocities, h);
            NBodyGravity.ComputeAccelerations<N>(positions, masses, gravitationalConstant, softening, accelerations);
            HalfKick(velocities, accelerations, h);
        }
    }

    /// <summary>Half-kick: v += 0.5 * a * dt for a single body in 3D.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) HalfKick<N>((N X, N Y, N Z) velocity, (N X, N Y, N Z) acceleration, N dt)
        where N : INumberBase<N>
    {
        N h = HalfStep(dt);
        return (velocity.X + acceleration.X * h, velocity.Y + acceleration.Y * h, velocity.Z + acceleration.Z * h);
    }

    /// <summary>Drift: x += v * dt for a single body in 3D.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) Drift<N>((N X, N Y, N Z) position, (N X, N Y, N Z) velocity, N dt)
        where N : INumberBase<N>
    {
        return (position.X + velocity.X * dt, position.Y + velocity.Y * dt, position.Z + velocity.Z * dt);
    }

    /// <summary>Half-kick applied to every element in 3D: v[i] += 0.5 * a[i] * dt.</summary>
    public static void HalfKick<N>(Span<(N X, N Y, N Z)> velocities, ReadOnlySpan<(N X, N Y, N Z)> accelerations, N dt)
        where N : INumberBase<N>
    {
        if (accelerations.Length != velocities.Length)
            throw new ArgumentException("accelerations must have the same length as velocities.");
        N h = HalfStep(dt);
        for (int i = 0; i < velocities.Length; i++)
        {
            var v = velocities[i];
            var a = accelerations[i];
            velocities[i] = (v.X + a.X * h, v.Y + a.Y * h, v.Z + a.Z * h);
        }
    }

    /// <summary>Drift applied to every element in 3D: x[i] += v[i] * dt.</summary>
    public static void Drift<N>(Span<(N X, N Y, N Z)> positions, ReadOnlySpan<(N X, N Y, N Z)> velocities, N dt)
        where N : INumberBase<N>
    {
        if (velocities.Length != positions.Length)
            throw new ArgumentException("velocities must have the same length as positions.");
        for (int i = 0; i < positions.Length; i++)
            positions[i] = Drift(positions[i], velocities[i], dt);
    }

    /// <summary>
    /// One velocity-Verlet step for an N-body gravity system in 3D with softening.
    /// Computes a(x) once, then per sub-step: v += 0.5*a*h, x += v*h, compute a(x), v += 0.5*a*h.
    /// </summary>
    /// <typeparam name="N"> Number type. </typeparam>
    /// <param name="positions"> Positions (modified in place). </param>
    /// <param name="velocities"> Velocities (modified in place). </param>
    /// <param name="accelerations"> Scratch acceleration buffer (must be same length as positions). </param>
    /// <param name="masses"> Body masses. </param>
    /// <param name="gravitationalConstant"> Gravitational constant. </param>
    /// <param name="softening"> Softening length. </param>
    /// <param name="timeDelta"> Total time delta to advance. </param>
    /// <param name="subSteps"> Number of equal-sized sub-steps to split the delta into. </param>
    public static void StepGravity<N>(
        Span<(N X, N Y, N Z)> positions,
        Span<(N X, N Y, N Z)> velocities,
        Span<(N X, N Y, N Z)> accelerations,
        ReadOnlySpan<N> masses,
        N gravitationalConstant,
        N softening,
        N timeDelta,
        int subSteps = 1)
        where N : IRootFunctions<N>
    {
        if (subSteps < 1)
            throw new ArgumentOutOfRangeException(nameof(subSteps), subSteps, "subSteps must be at least 1.");

        int n = positions.Length;
        if (n < 2)
            return;

        N h = timeDelta / N.CreateTruncating(subSteps);

        // The accelerations at the end of a sub-step are the ones the next sub-step starts from.
        NBodyGravity.ComputeAccelerations<N>(positions, masses, gravitationalConstant, softening, accelerations);
        for (int s = 0; s < subSteps; s++)
        {
            HalfKick(velocities, accelerations, h);
            Drift(positions, velocities, h);
            NBodyGravity.ComputeAccelerations<N>(positions, masses, gravitationalConstant, softening, accelerations);
            HalfKick(velocities, accelerations, h);
        }
    }

    // dt / 2 as 0.5 * dt, the one rounding every half-kick shares
    private static N HalfStep<N>(N dt)
        where N : INumberBase<N>
        => N.One / (N.One + N.One) * dt;
}
