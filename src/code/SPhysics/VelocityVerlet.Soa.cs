using System.Numerics;
using System.Runtime.InteropServices;

namespace SPhysics;

public static partial class VelocityVerlet
{
    /// <summary>
    /// Velocity Verlet steps over one component of all bodies at once (structure of arrays),
    /// the counterpart of <see cref="NBodyGravity.Soa"/>. Call them for every axis.
    /// </summary>
    /// <remarks>
    /// Vectorized by <see cref="Vector{T}"/>; every element gets the same operations as in the tuple steps,
    /// so the results are bitwise identical to them.
    /// </remarks>
    public static class Soa
    {
        /// <summary>Half-kick of one component: v[i] += 0.5 * a[i] * dt.</summary>
        /// <exception cref="ArgumentException"> The spans differ in length. </exception>
        public static void HalfKick<N>(Span<N> velocities, ReadOnlySpan<N> accelerations, N dt)
            where N : INumberBase<N>
            => AddScaled(velocities, accelerations, HalfStep(dt));

        /// <summary>Drift of one component: x[i] += v[i] * dt.</summary>
        /// <exception cref="ArgumentException"> The spans differ in length. </exception>
        public static void Drift<N>(Span<N> positions, ReadOnlySpan<N> velocities, N dt)
            where N : INumberBase<N>
            => AddScaled(positions, velocities, dt);

        // target[i] += source[i] * factor
        private static void AddScaled<N>(Span<N> target, ReadOnlySpan<N> source, N factor)
            where N : INumberBase<N>
        {
            if (source.Length != target.Length)
                throw new ArgumentException("Both spans must have the same length.");

            int i = 0;
            if (Vector.IsHardwareAccelerated && Vector<N>.IsSupported)
            {
                ref N target0 = ref MemoryMarshal.GetReference(target);
                ref N source0 = ref MemoryMarshal.GetReference(source);
                var vectorFactor = new Vector<N>(factor);
                for (; i + Vector<N>.Count <= target.Length; i += Vector<N>.Count)
                    (Vector.LoadUnsafe(ref target0, (nuint)i) + Vector.LoadUnsafe(ref source0, (nuint)i) * vectorFactor).StoreUnsafe(ref target0, (nuint)i);
            }

            for (; i < target.Length; i++)
                target[i] += source[i] * factor;
        }
    }
}
