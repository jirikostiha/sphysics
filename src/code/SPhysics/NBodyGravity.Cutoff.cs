using System.Numerics;

namespace SPhysics;

public static partial class NBodyGravity
{
    /// <summary> Cutoff of the kernels as they compare it: squared, as they have the squared distance of a pair. </summary>
    /// <param name="Squared"> Square of the cutoff distance. </param>
    /// <param name="Limited"> Whether the cutoff can skip a pair at all; an unlimited one leaves the kernels as they are without it. </param>
    private readonly record struct Cutoff<N>(N Squared, bool Limited)
        where N : IRootFunctions<N>
    {
        /// <summary> No cutoff: every pair pulls. </summary>
        public static Cutoff<N> None => new(N.Zero, false);

        /// <summary> Cutoff at <paramref name="distance"/>; positive infinity is none. </summary>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="distance"/> is negative or NaN. </exception>
        public static Cutoff<N> Of(N distance)
        {
            if (N.IsNaN(distance) || N.IsNegative(distance))
                throw new ArgumentOutOfRangeException(nameof(distance), distance, "A cutoff is a distance, not negative.");

            return N.IsPositiveInfinity(distance) ? None : new(distance * distance, true);
        }

        /// <summary> Whether a pair at the squared distance <paramref name="r2"/> is skipped; a pair at the cutoff still pulls. </summary>
        // the sign of the difference, as the number types of the kernels need not be ordered; it is exact, zero only when equal
        public bool Skips(N r2) => Limited && N.IsNegative(Squared - r2);
    }
}
