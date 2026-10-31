using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry3D;

namespace SPhysics;

/// <summary> Physics of a solid cube of uniform density, as members of <see cref="Cube.Region"/>. It is a box with equal edges. </summary>
public static class CubeRegionPhysics
{
    extension(Cube.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a solid cube. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a cube of the density and the edge. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N edgeLength)
            where N : INumberBase<N>
            => Mass.FromVolume(Cube.Region.Volume.FromEdge(edgeLength), density);
    }

    /// <summary> Moment of inertia of a solid cube about any axis through its centre. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> A sixth of the mass times the squared edge, the moment of a box with equal edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N edgeLength)
            where N : INumberBase<N>
            => Cuboid.Region.MomentOfInertia.FromMass(mass, edgeLength, edgeLength, edgeLength).A;

        /// <summary> Moment of inertia of a cube of the density and the edge. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N edgeLength)
            where N : INumberBase<N>
            => FromMass(Cube.Region.Mass.FromDensity(density, edgeLength), edgeLength);
    }
}
