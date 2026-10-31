using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;

namespace SPhysics;

/// <summary> Physics of a flat elliptical plate of uniform density, as members of <see cref="Ellipse.Region"/>. </summary>
public static class EllipseRegionPhysics
{
    extension(Ellipse.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of an elliptical plate. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a plate of the density and the semi-axes. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N majorRadius, N minorRadius)
            where N : IFloatingPoint<N>
            => Mass.FromVolume(Ellipse.Region.Area.FromRadius(majorRadius, minorRadius), density);
    }

    /// <summary> Moment of inertia of an elliptical plate about the axis through its centre, perpendicular to it. </summary>
    /// <remarks>
    /// The sum of the moments about the two semi-axes, m b^2 / 4 and m a^2 / 4, by the perpendicular axis theorem.
    /// <a href="https://phys.libretexts.org/Bookshelves/Classical_Mechanics/Classical_Mechanics_(Tatum)/02%3A_Moments_of_Inertia/2.20%3A_Ellipses_and_Ellipsoids">libretexts</a>
    /// </remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> The mass times the sum of the squared semi-axes, over four. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N majorRadius, N minorRadius)
            where N : INumberBase<N>
            => mass * (majorRadius * majorRadius + minorRadius * minorRadius) / N.CreateChecked(4);

        /// <summary> Moment of inertia of a plate of the density and the semi-axes. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N majorRadius, N minorRadius)
            where N : IFloatingPoint<N>
            => FromMass(Ellipse.Region.Mass.FromDensity(density, majorRadius, minorRadius), majorRadius, minorRadius);
    }
}
