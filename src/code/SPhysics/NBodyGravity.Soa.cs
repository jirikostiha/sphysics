using System.Numerics;
using System.Runtime.InteropServices;

namespace SPhysics;

public static partial class NBodyGravity
{
    /// <summary>
    /// Gravitational accelerations of bodies whose components are stored in separate spans (structure of arrays),
    /// vectorized across the pairs of bodies.
    /// </summary>
    /// <remarks>
    /// One <see cref="Vector{T}"/> evaluates as many pairs as it has lanes, 8 for float and 4 for double with AVX2.
    /// The sum of the pulls on a body is split into the lanes, so the rounding depends on the vector width of the
    /// machine: the results are reproducible on one machine, not across machines with different vector widths.
    /// A number type without hardware vectors runs the same kernel in scalar code.
    /// The kernels over tuples copy the components into spans and run this kernel.
    /// </remarks>
    public static class Soa
    {
        /// <summary>
        /// Computes gravitational accelerations for all bodies in 2D pairwise (O(n^2)), a gravitational constant per body.
        /// </summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="x"> X components of the positions. </param>
        /// <param name="y"> Y components of the positions. </param>
        /// <param name="masses"> Body masses. </param>
        /// <param name="gravitationalConstants"> Per-body gravitational constant. </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerationsX"> Output buffer of the x components. </param>
        /// <param name="accelerationsY"> Output buffer of the y components. </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="x"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<N> x, ReadOnlySpan<N> y,
            ReadOnlySpan<N> masses, ReadOnlySpan<N> gravitationalConstants, N softening,
            Span<N> accelerationsX, Span<N> accelerationsY)
            where N : IRootFunctions<N>
            => Kernel(x, y, default, masses, gravitationalConstants, N.Zero, softening, accelerationsX, accelerationsY, default, perBody: true, spatial: false);

        /// <summary>
        /// Computes gravitational accelerations for all bodies in 2D pairwise (O(n^2)), one gravitational constant shared by every body.
        /// </summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="x"> X components of the positions. </param>
        /// <param name="y"> Y components of the positions. </param>
        /// <param name="masses"> Body masses. </param>
        /// <param name="gravitationalConstant"> Gravitational constant. </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerationsX"> Output buffer of the x components. </param>
        /// <param name="accelerationsY"> Output buffer of the y components. </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="x"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<N> x, ReadOnlySpan<N> y,
            ReadOnlySpan<N> masses, N gravitationalConstant, N softening,
            Span<N> accelerationsX, Span<N> accelerationsY)
            where N : IRootFunctions<N>
            => Kernel(x, y, default, masses, default, gravitationalConstant, softening, accelerationsX, accelerationsY, default, perBody: false, spatial: false);

        /// <summary>
        /// Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)), a gravitational constant per body.
        /// </summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="x"> X components of the positions. </param>
        /// <param name="y"> Y components of the positions. </param>
        /// <param name="z"> Z components of the positions. </param>
        /// <param name="masses"> Body masses. </param>
        /// <param name="gravitationalConstants"> Per-body gravitational constant. </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerationsX"> Output buffer of the x components. </param>
        /// <param name="accelerationsY"> Output buffer of the y components. </param>
        /// <param name="accelerationsZ"> Output buffer of the z components. </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="x"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<N> x, ReadOnlySpan<N> y, ReadOnlySpan<N> z,
            ReadOnlySpan<N> masses, ReadOnlySpan<N> gravitationalConstants, N softening,
            Span<N> accelerationsX, Span<N> accelerationsY, Span<N> accelerationsZ)
            where N : IRootFunctions<N>
            => Kernel(x, y, z, masses, gravitationalConstants, N.Zero, softening, accelerationsX, accelerationsY, accelerationsZ, perBody: true, spatial: true);

        /// <summary>
        /// Computes gravitational accelerations for all bodies in 3D pairwise (O(n^2)), one gravitational constant shared by every body.
        /// </summary>
        /// <typeparam name="N"> Number type. </typeparam>
        /// <param name="x"> X components of the positions. </param>
        /// <param name="y"> Y components of the positions. </param>
        /// <param name="z"> Z components of the positions. </param>
        /// <param name="masses"> Body masses. </param>
        /// <param name="gravitationalConstant"> Gravitational constant. </param>
        /// <param name="softening"> Softening length; the effective squared distance is r^2 + softening^2. </param>
        /// <param name="accelerationsX"> Output buffer of the x components. </param>
        /// <param name="accelerationsY"> Output buffer of the y components. </param>
        /// <param name="accelerationsZ"> Output buffer of the z components. </param>
        /// <exception cref="ArgumentException"> A span differs in length from <paramref name="x"/>. </exception>
        public static void ComputeAccelerations<N>(
            ReadOnlySpan<N> x, ReadOnlySpan<N> y, ReadOnlySpan<N> z,
            ReadOnlySpan<N> masses, N gravitationalConstant, N softening,
            Span<N> accelerationsX, Span<N> accelerationsY, Span<N> accelerationsZ)
            where N : IRootFunctions<N>
            => Kernel(x, y, z, masses, default, gravitationalConstant, softening, accelerationsX, accelerationsY, accelerationsZ, perBody: false, spatial: true);
    }

    // z and az are read only when spatial, the gravitational constants only when perBody
    private static void Kernel<N>(
        ReadOnlySpan<N> x, ReadOnlySpan<N> y, ReadOnlySpan<N> z,
        ReadOnlySpan<N> masses, ReadOnlySpan<N> gravitationalConstants, N gravitationalConstant, N softening,
        Span<N> ax, Span<N> ay, Span<N> az,
        bool perBody, bool spatial)
        where N : IRootFunctions<N>
    {
        int n = x.Length;
        if (y.Length != n || masses.Length != n || ax.Length != n || ay.Length != n
            || (perBody && gravitationalConstants.Length != n)
            || (spatial && (z.Length != n || az.Length != n)))
            throw new ArgumentException("Every span must have the same length as the x components.");

        N soft2 = softening * softening;

        ax.Clear();
        ay.Clear();
        if (spatial)
            az.Clear();

        for (int i = 0; i < n; i++)
        {
            N xi = x[i];
            N yi = y[i];
            N zi = spatial ? z[i] : N.Zero;
            N gI = perBody ? gravitationalConstants[i] : gravitationalConstant;
            N mI = masses[i];

            int j = i + 1;
            var (sx, sy, sz) = (N.Zero, N.Zero, N.Zero);
            // fewer pairs than lanes are left to the scalar loop, the vector setup would cost more than they do
            if (Vector.IsHardwareAccelerated && Vector<N>.IsSupported && n - j >= Vector<N>.Count)
                (j, sx, sy, sz) = VectorPairs(i, x, y, z, masses, gravitationalConstants, gI, soft2, ax, ay, az, perBody, spatial);

            for (; j < n; j++)
            {
                N rx = x[j] - xi;
                N ry = y[j] - yi;
                N rz = spatial ? z[j] - zi : N.Zero;
                N invR = N.One / N.Sqrt(rx * rx + ry * ry + rz * rz + soft2);
                N invR3 = invR * invR * invR;

                N kI = gI * masses[j] * invR3;
                sx += rx * kI;
                sy += ry * kI;
                sz += rz * kI;

                N kJ = (perBody ? gravitationalConstants[j] : gravitationalConstant) * mI * invR3;
                ax[j] -= rx * kJ;
                ay[j] -= ry * kJ;
                if (spatial)
                    az[j] -= rz * kJ;
            }

            ax[i] += sx;
            ay[i] += sy;
            if (spatial)
                az[i] += sz;
        }
    }

    // the pairs of body i with the following bodies in whole vectors; returns the first body left to the scalar loop
    // and the pulls on body i. The loads and stores stay within the spans, as j + width <= n.
    // Without per-body constants gI is the shared constant, so it serves body j too.
    private static (int J, N X, N Y, N Z) VectorPairs<N>(
        int i,
        ReadOnlySpan<N> x, ReadOnlySpan<N> y, ReadOnlySpan<N> z,
        ReadOnlySpan<N> masses, ReadOnlySpan<N> gravitationalConstants, N gI, N soft2,
        Span<N> ax, Span<N> ay, Span<N> az,
        bool perBody, bool spatial)
        where N : IRootFunctions<N>
    {
        int n = x.Length;
        int width = Vector<N>.Count;

        ref N x0 = ref MemoryMarshal.GetReference(x);
        ref N y0 = ref MemoryMarshal.GetReference(y);
        ref N z0 = ref MemoryMarshal.GetReference(z);
        ref N m0 = ref MemoryMarshal.GetReference(masses);
        ref N g0 = ref MemoryMarshal.GetReference(gravitationalConstants);
        ref N ax0 = ref MemoryMarshal.GetReference(ax);
        ref N ay0 = ref MemoryMarshal.GetReference(ay);
        ref N az0 = ref MemoryMarshal.GetReference(az);

        var xi = new Vector<N>(x[i]);
        var yi = new Vector<N>(y[i]);
        var zi = new Vector<N>(spatial ? z[i] : N.Zero);
        var vgI = new Vector<N>(gI);
        var mI = new Vector<N>(masses[i]);
        var vSoft2 = new Vector<N>(soft2);
        var sx = Vector<N>.Zero;
        var sy = Vector<N>.Zero;
        var sz = Vector<N>.Zero;

        // separate loops, the 2D one skips the z lanes: one loop testing spatial per pair ran twice as long
        int j = i + 1;
        if (spatial)
        {
            for (; j + width <= n; j += width)
            {
                nuint at = (nuint)j;
                var rx = Vector.LoadUnsafe(ref x0, at) - xi;
                var ry = Vector.LoadUnsafe(ref y0, at) - yi;
                var rz = Vector.LoadUnsafe(ref z0, at) - zi;
                var invR = Vector<N>.One / Vector.SquareRoot(rx * rx + ry * ry + rz * rz + vSoft2);
                var invR3 = invR * invR * invR;

                var kI = vgI * Vector.LoadUnsafe(ref m0, at) * invR3;
                sx += rx * kI;
                sy += ry * kI;
                sz += rz * kI;

                var kJ = (perBody ? Vector.LoadUnsafe(ref g0, at) : vgI) * mI * invR3;
                (Vector.LoadUnsafe(ref ax0, at) - rx * kJ).StoreUnsafe(ref ax0, at);
                (Vector.LoadUnsafe(ref ay0, at) - ry * kJ).StoreUnsafe(ref ay0, at);
                (Vector.LoadUnsafe(ref az0, at) - rz * kJ).StoreUnsafe(ref az0, at);
            }
        }
        else
        {
            for (; j + width <= n; j += width)
            {
                nuint at = (nuint)j;
                var rx = Vector.LoadUnsafe(ref x0, at) - xi;
                var ry = Vector.LoadUnsafe(ref y0, at) - yi;
                var invR = Vector<N>.One / Vector.SquareRoot(rx * rx + ry * ry + vSoft2);
                var invR3 = invR * invR * invR;

                var kI = vgI * Vector.LoadUnsafe(ref m0, at) * invR3;
                sx += rx * kI;
                sy += ry * kI;

                var kJ = (perBody ? Vector.LoadUnsafe(ref g0, at) : vgI) * mI * invR3;
                (Vector.LoadUnsafe(ref ax0, at) - rx * kJ).StoreUnsafe(ref ax0, at);
                (Vector.LoadUnsafe(ref ay0, at) - ry * kJ).StoreUnsafe(ref ay0, at);
            }
        }

        return (j, Vector.Sum(sx), Vector.Sum(sy), Vector.Sum(sz));
    }
}
