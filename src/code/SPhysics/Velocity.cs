using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary>Velocity quantity.</summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Velocity">wikipedia</a></remarks>
public static class Velocity
{
    public const string Name = "velocity";
    public const string DefaultSymbol = "v";
    public const string Dimension = Length.Dimension + " " + Time.Dimension + "-1";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAcceleration<N>(N acceleration, N time)
        where N : IMultiplyOperators<N, N, N>
        =>
        acceleration * time;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromAngularVelocity<N>(N angularVelocity, N radius)
        where N : IMultiplyOperators<N, N, N>
        =>
        angularVelocity * radius;

    /// <summary>
    /// Share of its velocity a body of the given mass keeps after a time under linear drag, the force <c>-b * v</c>:
    /// <c>exp(-b * t / m)</c>, the exact solution of <c>m * dv/dt = -b * v</c>.
    /// </summary>
    /// <remarks>
    /// <a href="https://en.wikipedia.org/wiki/Drag_(physics)#Very_low_Reynolds_numbers:_Stokes'_drag">wikipedia</a>.
    /// Exact for any time, so a step of any length neither overshoots nor reverses the motion.
    /// </remarks>
    /// <param name="dragCoefficient"> Linear drag coefficient <c>b</c>, force per velocity. </param>
    /// <param name="mass"> Mass of the body. </param>
    /// <param name="time"> Time spent under the drag. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N RetainedUnderLinearDrag<N>(N dragCoefficient, N mass, N time)
        where N : IExponentialFunctions<N>
        =>
        N.Exp(-(dragCoefficient * time / mass));

    /// <summary> Average velocity over a distance travelled in a time span. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N distance, N time)
        where N : IDivisionOperators<N, N, N>
        =>
        distance / time;

    /// <summary> Velocity of a body of the given mass carrying the given momentum. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromMomentum<N>(N momentum, N mass)
        where N : IDivisionOperators<N, N, N>
        =>
        momentum / mass;

    /// <summary> Velocity in a plane of a body of the given mass carrying the given momentum, <c>p / m</c>. </summary>
    /// <remarks> Momentum is conserved, so a body that ejects a momentum recoils with the velocity of its negation. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y) FromMomentum<N>((N X, N Y) momentum, N mass)
        where N : IDivisionOperators<N, N, N>
        =>
        (momentum.X / mass, momentum.Y / mass);

    /// <summary> Velocity in space of a body of the given mass carrying the given momentum, <c>p / m</c>. </summary>
    /// <remarks> Momentum is conserved, so a body that ejects a momentum recoils with the velocity of its negation. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N X, N Y, N Z) FromMomentum<N>((N X, N Y, N Z) momentum, N mass)
        where N : IDivisionOperators<N, N, N>
        =>
        (momentum.X / mass, momentum.Y / mass, momentum.Z / mass);

    /// <summary> Speed of a body of the given mass carrying the given kinetic energy, <c>sqrt(2 * E / m)</c>. </summary>
    /// <remarks> The inverse of <see cref="KineticEnergy.Linear{N}"/>; a negative energy has no real speed and gives NaN. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N FromKineticEnergy<N>(N kineticEnergy, N mass)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(N.CreateChecked(2) * kineticEnergy / mass);

    /// <summary>
    /// Terminal velocity of a body falling through a fluid, where the drag of <see cref="DragForce.InFluid{N}"/> balances the weight:
    /// <c>sqrt(2 * m * g / (rho * A * Cd))</c>.
    /// </summary>
    /// <remarks>
    /// <a href="https://en.wikipedia.org/wiki/Terminal_velocity">wikipedia</a>.
    /// Neglects buoyancy; to include it, pass the mass of the body less the mass of the fluid it displaces.
    /// </remarks>
    /// <param name="mass"> Mass of the body. </param>
    /// <param name="gravity"> Gravitational acceleration. </param>
    /// <param name="fluidDensity"> Density of the fluid. </param>
    /// <param name="area"> Projected area of the body. </param>
    /// <param name="dragCoef"> Drag coefficient. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Terminal<N>(N mass, N gravity, N fluidDensity, N area, N dragCoef)
        where N : IRootFunctions<N>
        =>
        N.Sqrt(N.CreateChecked(2) * mass * gravity / (fluidDensity * area * dragCoef));
}
