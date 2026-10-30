using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid box of uniform density, as members of <see cref="Cuboid.Region"/>. </summary>
public static class CuboidRegionPhysics
{
    extension(Cuboid.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a solid box. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a box of the density and the edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N a, N b, N c)
            where N : INumberBase<N>
            => Mass.FromVolume(Cuboid.Region.Volume.FromEdges(a, b, c), density);
    }

    /// <summary> Moments of inertia of a solid box about the axes through its centre, parallel to the edges a, b and c. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> About each axis, the mass times the sum of the other two squared edges, over twelve. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N A, N B, N C) FromMass<N>(N mass, N a, N b, N c)
            where N : INumberBase<N>
        {
            N twelfth = mass / N.CreateChecked(12);
            return (twelfth * (b * b + c * c), twelfth * (a * a + c * c), twelfth * (a * a + b * b));
        }

        /// <summary> Moments of inertia of a box of the density and the edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (N A, N B, N C) FromDensity<N>(N density, N a, N b, N c)
            where N : INumberBase<N>
            => FromMass(Cuboid.Region.Mass.FromDensity(density, a, b, c), a, b, c);
    }
}
