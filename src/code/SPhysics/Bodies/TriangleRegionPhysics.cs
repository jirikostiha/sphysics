using System.Numerics;
using System.Runtime.CompilerServices;
using SMath.Geometry2D;

namespace SPhysics;

/// <summary> Physics of a flat triangular plate of uniform density, given by its edges, as members of <see cref="Triangle.Region"/>. </summary>
public static class TriangleRegionPhysics
{
    extension(Triangle.Region)
    {
        /// <inheritdoc cref="MassFormulas"/>
        public static MassFormulas Mass => default;

        /// <inheritdoc cref="MomentOfInertiaFormulas"/>
        public static MomentOfInertiaFormulas MomentOfInertia => default;
    }

    /// <summary> Mass of a triangular plate. </summary>
    public readonly struct MassFormulas
    {
        /// <summary> Mass of a plate of the density and the edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N a, N b, N c)
            where N : IRootFunctions<N>
            => Mass.FromVolume(Triangle.Region.Area.FromEdges(a, b, c), density);
    }

    /// <summary> Moment of inertia of a triangular plate about the axis through its centroid, perpendicular to it. </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/List_of_moments_of_inertia">wikipedia</a></remarks>
    public readonly struct MomentOfInertiaFormulas
    {
        /// <summary> The mass times the sum of the squared edges, over thirty-six. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromMass<N>(N mass, N a, N b, N c)
            where N : INumberBase<N>
            => mass * (a * a + b * b + c * c) / N.CreateChecked(36);

        /// <summary> Moment of inertia of a plate of the density and the edges. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public N FromDensity<N>(N density, N a, N b, N c)
            where N : IRootFunctions<N>
            => FromMass(Triangle.Region.Mass.FromDensity(density, a, b, c), a, b, c);
    }
}
