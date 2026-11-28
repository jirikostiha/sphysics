using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Reynolds number, the ratio of inertial to viscous forces in a flow. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Reynolds_number">wikipedia</a></remarks>
public static class ReynoldsNumber
{
    public const string Name = "Reynolds number";
    public const string DefaultSymbol = "Re";
    public const string Dimension = "1";

    /// <summary> Reynolds number <c>rho * v * L / mu</c>. </summary>
    /// <param name="fluidDensity"> Density of the fluid. </param>
    /// <param name="flowVelocity"> Velocity of the flow relative to the body. </param>
    /// <param name="characteristicLength"> Characteristic length, for example the diameter of a sphere or a pipe. </param>
    /// <param name="dynamicViscosity"> Dynamic viscosity of the fluid. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Eval<N>(N fluidDensity, N flowVelocity, N characteristicLength, N dynamicViscosity)
        where N : IMultiplyOperators<N, N, N>, IDivisionOperators<N, N, N>
        =>
        fluidDensity * flowVelocity * characteristicLength / dynamicViscosity;
}
