using System.Numerics;

namespace SPhysics;

/// <summary>Drag force</summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Drag_(physics)">wikipedia</a>
/// <a href="https://en.wikipedia.org/wiki/Drag_equation">wikipedia</a>
/// </remarks>
public static class DragForce
{
    public static N InFluid<N>(N fluidDensity, N flowVelocity, N area, N dragCoef)
        where N : INumberBase<N>
        =>
        (N.One / N.CreateTruncating(2)) * fluidDensity * (flowVelocity * flowVelocity) * area * dragCoef;
}
