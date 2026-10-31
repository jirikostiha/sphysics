using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;

namespace SPhysics;

/// <summary> Physics of a flat square plate of uniform density, as members of <see cref="Square.Region"/>. It is a rectangular plate with equal edges. </summary>
public static class SquareRegionPhysics
{
    extension(Square.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a square plate. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a plate of the density and the edge. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N edgeLength)
            where N : INumberBase<N>
            => Mass.FromVolume(Square.Region.Area.FromEdge(edgeLength), density);
    }

    /// <summary> Moment of inertia of a square plate about the axis through its centre, perpendicular to it. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> A sixth of the mass times the squared edge, the moment of a rectangular plate with equal edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N edgeLength)
            where N : INumberBase<N>
            => Rectangle.Region.MomentOfInertia.FromMass(mass, edgeLength, edgeLength);

        /// <summary> Moment of inertia of a plate of the density and the edge. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N edgeLength)
            where N : INumberBase<N>
            => FromMass(Square.Region.Mass.FromDensity(density, edgeLength), edgeLength);
    }
}
