using System.Numerics;
using System.Runtime.CompilerServices;

namespace SPhysics;

/// <summary> Precession, the turning of the axis of rotation of a spinning body. </summary>
/// <remarks><a href="https://en.wikipedia.org/wiki/Precession">wikipedia</a></remarks>
public static class Precession
{
    /// <summary>
    /// Angular velocity of the precession of a fast spinning top under a torque perpendicular to its spin, <c>Omega = tau / L</c>.
    /// </summary>
    /// <remarks>
    /// It holds while the angular momentum of the spin dominates the one of the precession itself.
    /// <a href="https://en.wikipedia.org/wiki/Precession#Torque-induced">wikipedia</a>
    /// </remarks>
    /// <param name="torque"> Torque perpendicular to the spin axis, for a top in gravity its weight times the lever arm. </param>
    /// <param name="spinAngularMomentum"> Angular momentum of the spin. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N Gyroscopic<N>(N torque, N spinAngularMomentum)
        where N : IDivisionOperators<N, N, N>
        =>
        torque / spinAngularMomentum;

    /// <summary>
    /// Angular velocity of the free precession of a symmetric body, seen in the frame of the body,
    /// <c>Omega = (I_3 - I_1) / I_1 * omega_3</c>.
    /// </summary>
    /// <remarks>
    /// Without a torque the angular velocity circles the symmetry axis at this rate; positive for an oblate body, in the
    /// sense of its spin. <a href="https://en.wikipedia.org/wiki/Euler%27s_equations_(rigid_body_dynamics)#Torque-free_solutions">wikipedia</a>
    /// </remarks>
    /// <param name="axialMoment"> Moment of inertia about the symmetry axis. </param>
    /// <param name="transverseMoment"> Moment of inertia about an axis perpendicular to the symmetry axis. </param>
    /// <param name="axialAngularVelocity"> Component of the angular velocity along the symmetry axis. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N TorqueFree<N>(N axialMoment, N transverseMoment, N axialAngularVelocity)
        where N : INumberBase<N>
        =>
        (axialMoment - transverseMoment) / transverseMoment * axialAngularVelocity;
}
