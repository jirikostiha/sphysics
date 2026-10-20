namespace SPhysics;

/// <summary>
/// Drag coefficients of common shapes in real world, the input of <see cref="DragForce.InFluid{N}"/>.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Drag_coefficient">wikipedia</a>
/// </remarks>
public static class DragCoefficient
{
    /// <summary> Smooth sphere at the Reynolds number 10^6. </summary>
    public const double SmoothSphere = 0.1;

    /// <summary> Rough sphere at the Reynolds number 10^6. </summary>
    public const double RoughSphere = 0.47;
}
