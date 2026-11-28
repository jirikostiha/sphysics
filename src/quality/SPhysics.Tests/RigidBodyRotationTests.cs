using SMath.Geometry3D;

using Xunit;

namespace SPhysics.Tests;

public class RigidBodyRotationTests
{
    private const int Precision = 12;

    private static readonly (double X, double Y, double Z)[] _positions = [(1, 2, 3), (-2, 0.5, 1), (0, -1, -2)];
    private static readonly double[] _masses = [2, 3, 1.5];
    private static readonly (double X, double Y, double Z) _angularVelocity = (0.3, -1.2, 0.7);

    private static void AssertClose((double X, double Y, double Z) expected, (double X, double Y, double Z) actual)
    {
        Assert.Equal(expected.X, actual.X, Precision);
        Assert.Equal(expected.Y, actual.Y, Precision);
        Assert.Equal(expected.Z, actual.Z, Precision);
    }

    private static void AssertClose(
        (double XX, double YY, double ZZ, double XY, double XZ, double YZ) expected,
        (double XX, double YY, double ZZ, double XY, double XZ, double YZ) actual)
    {
        AssertClose((expected.XX, expected.YY, expected.ZZ), (actual.XX, actual.YY, actual.ZZ));
        AssertClose((expected.XY, expected.XZ, expected.YZ), (actual.XY, actual.XZ, actual.YZ));
    }

    private static (double X, double Y, double Z)[] RigidVelocities() =>
        [.. _positions.Select(r => GeometricVector3.CrossProduct.FromCartesian(_angularVelocity, r))];

    [Fact]
    public void InertiaTensorPointHoldsTheMomentsAndTheNegativeProductsOfInertia()
    {
        Assert.Equal((26.0, 20.0, 10.0, -4.0, -6.0, -12.0), InertiaTensor.Point(2.0, (1.0, 2.0, 3.0)));
    }

    [Fact]
    public void MomentOfInertiaFromInertiaTensorIsTheMomentOfThePointsAtTheirDistancesFromTheAxis()
    {
        var tensor = InertiaTensor.Total<double>(_positions, _masses, (0, 0, 0));

        // distance of each point from the z axis is the length of its x and y
        double expected = _positions.Select((r, i) => MomentOfInertia.Point(_masses[i], Math.Sqrt((r.X * r.X) + (r.Y * r.Y)))).Sum();
        Assert.Equal(expected, MomentOfInertia.FromInertiaTensor(tensor, (0.0, 0.0, 1.0)), Precision);
    }

    [Fact]
    public void InertiaTensorTotalHasTwiceThePolarMomentAsItsTrace()
    {
        var origin = (0.5, -1.0, 2.0);
        var tensor = InertiaTensor.Total<double>(_positions, _masses, origin);

        Assert.Equal(2 * MomentOfInertia.Polar<double>(_positions, _masses, origin), tensor.XX + tensor.YY + tensor.ZZ, Precision);
    }

    [Fact]
    public void InertiaTensorParallelAxisShiftsTheTensorFromTheCenterOfMass()
    {
        var centerOfMass = CenterOfMass.Eval<double>(_positions, _masses);
        var aboutCenter = InertiaTensor.Total<double>(_positions, _masses, centerOfMass);

        var shifted = InertiaTensor.ParallelAxis(aboutCenter, _masses.Sum(), centerOfMass);

        var aboutOrigin = InertiaTensor.Total<double>(_positions, _masses, (0, 0, 0));
        AssertClose(aboutOrigin, shifted);
    }

    [Fact]
    public void InertiaTensorFromPrincipalGivesThePrincipalMomentAboutEachAxis()
    {
        var tensor = InertiaTensor.FromPrincipal(Cuboid.Region.MomentOfInertia.FromMass(12.0, 1.0, 2.0, 3.0));

        Assert.Equal(13.0, MomentOfInertia.FromInertiaTensor(tensor, (1.0, 0.0, 0.0)), Precision);
        Assert.Equal(10.0, MomentOfInertia.FromInertiaTensor(tensor, (0.0, 1.0, 0.0)), Precision);
        Assert.Equal(5.0, MomentOfInertia.FromInertiaTensor(tensor, (0.0, 0.0, 1.0)), Precision);
    }

    [Fact]
    public void AngularMomentumFromInertiaTensorIsTheAngularMomentumOfThePointsMovingRigidly()
    {
        var tensor = InertiaTensor.Total<double>(_positions, _masses, (0, 0, 0));

        var expected = AngularMomentum.Total<double>(_positions, RigidVelocities(), _masses, (0, 0, 0));
        AssertClose(expected, AngularMomentum.FromInertiaTensor(tensor, _angularVelocity));
    }

    [Fact]
    public void KineticEnergySpinningWithInertiaTensorIsTheKineticEnergyOfThePointsMovingRigidly()
    {
        var tensor = InertiaTensor.Total<double>(_positions, _masses, (0, 0, 0));

        double expected = KineticEnergy.Total<double>(RigidVelocities(), _masses);
        Assert.Equal(expected, KineticEnergy.Spinning(tensor, _angularVelocity), Precision);
    }

    [Fact]
    public void AngularAccelerationFromEulerEquationsAtRestIsTheTorqueOverEachMoment()
    {
        var alpha = AngularAcceleration.FromEulerEquations((2.0, 4.0, 5.0), (0.0, 0.0, 0.0), (1.0, 2.0, 10.0));

        AssertClose((0.5, 0.5, 2.0), alpha);
    }

    [Fact]
    public void AngularAccelerationFromEulerEquationsSpinAboutAPrincipalAxisKeepsWithoutATorque()
    {
        var alpha = AngularAcceleration.FromEulerEquations((2.0, 4.0, 5.0), (0.0, 3.0, 0.0), (0.0, 0.0, 0.0));

        AssertClose((0.0, 0.0, 0.0), alpha);
    }

    [Fact]
    public void AngularAccelerationFromEulerEquationsKeepsTheAngularMomentumOfAFreeBody()
    {
        // without a torque |L| stays, so the rate of L^2 = sum((I * omega)^2) is zero: sum(I^2 * omega * alpha) = 0
        var moments = (A: 2.0, B: 4.0, C: 5.0);
        var alpha = AngularAcceleration.FromEulerEquations(moments, _angularVelocity, (0.0, 0.0, 0.0));

        double rate = (moments.A * moments.A * _angularVelocity.X * alpha.X)
                      + (moments.B * moments.B * _angularVelocity.Y * alpha.Y)
                      + (moments.C * moments.C * _angularVelocity.Z * alpha.Z);
        Assert.Equal(0.0, rate, Precision);
    }

    [Fact]
    public void PrecessionTorqueFreeIsTheRateTheEulerEquationsTurnTheTransverseSpinAt()
    {
        // symmetric body spinning about its axis with a small transverse component along x: the component turns towards y
        const double transverse = 2.0, axial = 3.0, axialSpin = 4.0, transverseSpin = 0.1;
        var alpha = AngularAcceleration.FromEulerEquations((transverse, transverse, axial), (transverseSpin, 0.0, axialSpin), (0.0, 0.0, 0.0));

        Assert.Equal(alpha.Y / transverseSpin, Precession.TorqueFree(axial, transverse, axialSpin), Precision);
    }
}
