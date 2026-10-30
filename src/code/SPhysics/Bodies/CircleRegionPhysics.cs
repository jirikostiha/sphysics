using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;

namespace SPhysics;

/// <summary> Physics of a disc of uniform density, a flat circular body, as members of <see cref="Circle.Region"/>. </summary>
public static class CircleRegionPhysics
{
    extension(Circle.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a disc. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a disc of the density and the radius. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N radius)
            where N : IFloatingPoint<N>
            => Mass.FromVolume(Circle.Region.Area.FromRadius(radius), density);
    }

    /// <summary> Moment of inertia of a disc about the axis through its centre, perpendicular to it. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> Half of the mass times the squared radius. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N radius)
            where N : INumberBase<N>
            => mass * radius * radius / N.CreateChecked(2);

        /// <summary> Moment of inertia of a disc of the density and the radius. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N radius)
            where N : IFloatingPoint<N>
            => FromMass(Circle.Region.Mass.FromDensity(density, radius), radius);
    }
}
