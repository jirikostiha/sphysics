using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid right circular cylinder of uniform density, as members of <see cref="Cylinder.Region"/>. </summary>
public static class CylinderRegionPhysics
{
    extension(Cylinder.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a solid cylinder. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a cylinder of the density, the radius and the height. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N radius, N height)
            where N : IFloatingPoint<N>
            => Mass.FromVolume(Cylinder.Region.Volume.FromRadius(radius, height), density);
    }

    /// <summary>
    /// Moments of inertia of a solid cylinder about its axis, and about a transverse axis through its centre,
    /// perpendicular to its axis.
    /// </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> About the axis, half of the mass times the squared radius; about the transverse axis, m * (3 r^2 + h^2) / 12. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N Axial, N Transverse) FromMass<N>(N mass, N radius, N height)
            where N : INumberBase<N>
        {
            N squaredRadius = radius * radius;
            return (mass * squaredRadius / N.CreateChecked(2),
                mass * (N.CreateChecked(3) * squaredRadius + height * height) / N.CreateChecked(12));
        }

        /// <summary> Moments of inertia of a cylinder of the density, the radius and the height. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N Axial, N Transverse) FromDensity<N>(N density, N radius, N height)
            where N : IFloatingPoint<N>
            => FromMass(Cylinder.Region.Mass.FromDensity(density, radius, height), radius, height);
    }
}
