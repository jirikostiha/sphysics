namespace SPhysics;

/// <summary>
/// Drag coefficient.
/// </summary>
/// <remarks>
/// <a href="https://en.wikipedia.org/wiki/Drag_coefficient">wikipedia</a>
/// </remarks>
public static class DragCoefficient
{
    /// <summary> Smooth sphere with Reynolds number = 10^6 </summary>
    public static double SmoothSphere_Re10p6 => 0.1;

    /// <summary> Smooth sphere with Reynolds number = 10^5 </summary>
    public static double SmoothSphere_Re10p5 => 0.47;

}
