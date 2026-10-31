using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid right circular cone of uniform density, as members of <see cref="Cone.Region"/>. </summary>
public static class ConeRegionPhysics
{
    extension(Cone.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a solid cone. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a cone of the density, the base radius and the height. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N radius, N height)
            where N : IFloatingPoint<N>
            => Mass.FromVolume(Cone.Region.Volume.FromRadius(radius, height), density);
    }

    /// <summary>
    /// Moments of inertia of a solid cone about its axis, and about a transverse axis through its centre of mass,
    /// perpendicular to its axis. The centre of mass lies on the axis, a quarter of the height above the base.
    /// </summary>
    /// <remarks>
    /// About a transverse axis through the apex the moment is m * (3/20 r^2 + 3/5 h^2), about one through the base
    /// m * (3/20 r^2 + 1/10 h^2); both follow from the transverse moment by <see cref="SPhysics.MomentOfInertia.ParallelAxis{N}"/>.
    /// <a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a>
    /// </remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> About the axis, 3/10 m r^2; about the transverse axis, m * (3/20 r^2 + 3/80 h^2). </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N Axial, N Transverse) FromMass<N>(N mass, N radius, N height)
            where N : INumberBase<N>
        {
            N squaredRadius = radius * radius;
            return (N.CreateChecked(3) * mass * squaredRadius / N.CreateChecked(10),
                N.CreateChecked(3) * mass * (N.CreateChecked(4) * squaredRadius + height * height) / N.CreateChecked(80));
        }

        /// <summary> Moments of inertia of a cone of the density, the base radius and the height. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N Axial, N Transverse) FromDensity<N>(N density, N radius, N height)
            where N : IFloatingPoint<N>
            => FromMass(Cone.Region.Mass.FromDensity(density, radius, height), radius, height);
    }
}
