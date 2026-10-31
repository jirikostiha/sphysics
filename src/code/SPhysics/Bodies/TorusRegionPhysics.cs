using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid ring torus of uniform density, as members of <see cref="Torus.Region"/>. </summary>
public static class TorusRegionPhysics
{
    extension(Torus.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a solid torus. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a torus of the density, the major radius R of the tube centre circle and the minor radius r of the tube. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N majorRadius, N minorRadius)
            where N : IFloatingPointConstants<N>
            => Mass.FromVolume(Torus.Region.Volume.FromRadii(majorRadius, minorRadius), density);
    }

    /// <summary>
    /// Moments of inertia of a solid torus about its axis of symmetry, and about a diameter, an axis in its equatorial
    /// plane through its centre.
    /// </summary>
    /// <remarks>
    /// <a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a>
    /// <a href="https://scienceworld.wolfram.com/physics/MomentofInertiaRing.html">wolfram</a>
    /// </remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> About the axis, m * (4 R^2 + 3 r^2) / 4; about a diameter, m * (4 R^2 + 5 r^2) / 8. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N Axial, N Diametral) FromMass<N>(N mass, N majorRadius, N minorRadius)
            where N : INumberBase<N>
        {
            N fourMajorSquared = N.CreateChecked(4) * majorRadius * majorRadius;
            N minorSquared = minorRadius * minorRadius;
            return (mass * (fourMajorSquared + N.CreateChecked(3) * minorSquared) / N.CreateChecked(4),
                mass * (fourMajorSquared + N.CreateChecked(5) * minorSquared) / N.CreateChecked(8));
        }

        /// <summary> Moments of inertia of a torus of the density and the radii. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N Axial, N Diametral) FromDensity<N>(N density, N majorRadius, N minorRadius)
            where N : IFloatingPointConstants<N>
            => FromMass(Torus.Region.Mass.FromDensity(density, majorRadius, minorRadius), majorRadius, minorRadius);
    }
}
