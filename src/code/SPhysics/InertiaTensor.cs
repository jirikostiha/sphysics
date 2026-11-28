using System.Numerics;
using System.Runtime.CompilerServices;

using SMath.Geometry3D;

namespace SPhysics;

/// <summary>
/// Inertia tensor, the moment of inertia of a body in space about every axis through a point at once.
/// </summary>
/// <remarks>
/// The tensor is symmetric, so it is held by its six independent elements. The off-diagonal elements are the elements
/// of the tensor, <c>I_xy = -sum(m * x * y)</c>, not the products of inertia, which have the opposite sign.
/// <a href="https://en.wikipedia.org/wiki/Moment_of_inertia#Inertia_tensor">wikipedia</a>
/// </remarks>
public static class InertiaTensor
{
    /// <summary> Point mass at a position relative to the point the tensor is taken about, <c>m * (|r|^2 * E - r r^T)</c>. </summary>
    /// <param name="mass"> Mass of the point. </param>
    /// <param name="position"> Position of the point relative to the point the tensor is taken about. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N XX, N YY, N ZZ, N XY, N XZ, N YZ) Point<N>(N mass, (N X, N Y, N Z) position)
        where N : INumberBase<N>
    {
        var (x, y, z) = position;
        N xx = x * x, yy = y * y, zz = z * z;
        N mx = mass * x, my = mass * y;
        return (mass * (yy + zz), mass * (xx + zz), mass * (xx + yy), -(mx * y), -(mx * z), -(my * z));
    }

    /// <summary> Point masses in space about the point <paramref name="origin"/>, the sum of the tensors of the points. </summary>
    /// <param name="positions"> Positions of the points. </param>
    /// <param name="masses"> Masses of the points, as many as there are positions. </param>
    /// <param name="origin"> Point the tensor is taken about. </param>
    public static (N XX, N YY, N ZZ, N XY, N XZ, N YZ) Total<N>(
        ReadOnlySpan<(N X, N Y, N Z)> positions, ReadOnlySpan<N> masses, (N X, N Y, N Z) origin)
        where N : INumberBase<N>
    {
        SystemSpans.RequireLengths(positions.Length, masses.Length);

        var total = (XX: N.Zero, YY: N.Zero, ZZ: N.Zero, XY: N.Zero, XZ: N.Zero, YZ: N.Zero);
        for (int i = 0; i < positions.Length; i++)
        {
            // a point has no tensor of its own about its centre, so it adds just its parallel axis term
            total = ParallelAxis(total, masses[i], GeometricVector3.Direction.FromCartesian(origin, positions[i]));
        }

        return total;
    }

    /// <summary>
    /// Tensor of the principal moments of inertia, in the frame of the principal axes, where the off-diagonal elements are zero.
    /// </summary>
    /// <param name="principalMoments"> Moments of inertia about the principal axes, as the bodies of SMath shapes give them. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N XX, N YY, N ZZ, N XY, N XZ, N YZ) FromPrincipal<N>((N A, N B, N C) principalMoments)
        where N : INumberBase<N>
        =>
        (principalMoments.A, principalMoments.B, principalMoments.C, N.Zero, N.Zero, N.Zero);

    /// <summary>
    /// Tensor about a point offset from the centre of mass, the parallel axis (Steiner) theorem in space,
    /// <c>I = I_cm + m * (|d|^2 * E - d d^T)</c>.
    /// </summary>
    /// <remarks><a href="https://en.wikipedia.org/wiki/Parallel_axis_theorem#Tensor_generalization">wikipedia</a></remarks>
    /// <param name="centerOfMassTensor"> Tensor about the centre of mass. </param>
    /// <param name="mass"> Mass of the body. </param>
    /// <param name="offset"> Position of the centre of mass relative to the new point. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (N XX, N YY, N ZZ, N XY, N XZ, N YZ) ParallelAxis<N>(
        (N XX, N YY, N ZZ, N XY, N XZ, N YZ) centerOfMassTensor, N mass, (N X, N Y, N Z) offset)
        where N : INumberBase<N>
    {
        var shift = Point(mass, offset);
        return (centerOfMassTensor.XX + shift.XX, centerOfMassTensor.YY + shift.YY, centerOfMassTensor.ZZ + shift.ZZ,
                centerOfMassTensor.XY + shift.XY, centerOfMassTensor.XZ + shift.XZ, centerOfMassTensor.YZ + shift.YZ);
    }

    /// <summary> Product of the tensor and a vector, <c>I * v</c>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (N X, N Y, N Z) Apply<N>((N XX, N YY, N ZZ, N XY, N XZ, N YZ) tensor, (N X, N Y, N Z) vector)
        where N : INumberBase<N>
        =>
        ((tensor.XX * vector.X) + (tensor.XY * vector.Y) + (tensor.XZ * vector.Z),
         (tensor.XY * vector.X) + (tensor.YY * vector.Y) + (tensor.YZ * vector.Z),
         (tensor.XZ * vector.X) + (tensor.YZ * vector.Y) + (tensor.ZZ * vector.Z));
}
