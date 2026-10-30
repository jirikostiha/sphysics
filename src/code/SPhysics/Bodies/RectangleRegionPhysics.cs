using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;

namespace SPhysics;

/// <summary> Physics of a flat rectangular plate of uniform density, as members of <see cref="Rectangle.Region"/>. </summary>
public static class RectangleRegionPhysics
{
    extension(Rectangle.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a rectangular plate. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a plate of the density and the edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N a, N b)
            where N : INumberBase<N>
            => Mass.FromVolume(Rectangle.Region.Area.FromEdges(a, b), density);
    }

    /// <summary> Moment of inertia of a rectangular plate about the axis through its centre, perpendicular to it. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> The mass times the sum of the squared edges, over twelve. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N a, N b)
            where N : INumberBase<N>
            => mass * (a * a + b * b) / N.CreateChecked(12);

        /// <summary> Moment of inertia of a plate of the density and the edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N a, N b)
            where N : INumberBase<N>
            => FromMass(Rectangle.Region.Mass.FromDensity(density, a, b), a, b);
    }
}
