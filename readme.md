<p align="center">
  <img src="src/code/SPhysics/icon.png" alt="SPhysics" width="50"/>
</p>

# SPhysics

![GitHub repo size](https://img.shields.io/github/repo-size/jirikostiha/sphysics)
![GitHub code size](https://img.shields.io/github/languages/code-size/jirikostiha/sphysics)
![Nuget](https://img.shields.io/nuget/dt/SPhysics)  
[![Build](https://github.com/jirikostiha/sphysics/actions/workflows/build.yml/badge.svg)](https://github.com/jirikostiha/sphysics/actions/workflows/build.yml)
[![Code Analysis](https://github.com/jirikostiha/sphysics/actions/workflows/code-analysis.yml/badge.svg)](https://github.com/jirikostiha/sphysics/actions/workflows/code-analysis.yml)
[![Code Lint](https://github.com/jirikostiha/sphysics/actions/workflows/lint-code.yml/badge.svg)](https://github.com/jirikostiha/sphysics/actions/workflows/lint-code.yml)
[![Documentation](https://img.shields.io/badge/docs-DocFX-blue.svg)](https://jirikostiha.github.io/sphysics/)

Physics for .NET, written against
[generic math](https://learn.microsoft.com/en-us/dotnet/standard/generics/math).
Every formula is generic over the numeric type, so the same call site works for `double`,
`float`, `decimal` or `Half` without overloads or casting. Builds on top of
[SMath](https://github.com/jirikostiha/smath).

## Documentation

Full API reference, guides, and performance benchmarks are available on the [Documentation Website](https://jirikostiha.github.io/sphysics/).

## Design

**Generic over the number type.** Constraints express the physics rather than a concrete
type: a formula needing a square root asks for `IRootFunctions<N>`, one needing only
addition asks for `INumberBase<N>`. Passing a type that cannot support the operation is a
compile error.

**Static and allocation free.** There are no wrapper structs for quantities or vectors.
Values are plain scalars and tuples, so they stay on the stack and interoperate with any
other library. All entry points are static.

**Names that read as the formula.** Types are named after the quantity being computed,
giving call sites like `GravitationalForce.Outside(m1, m2, r)` or
`KineticEnergy.Linear(m, v)`.

## Contents

| Area | Types |
| --- | --- |
| Base quantities | `Length`, `Mass`, `Time`, `ElectricCurrent`, `Temperature`, `AmountOfSubstance`, `LuminousIntensity` |
| Mechanics | `Velocity`, `Acceleration`, `Jerk`, `Momentum`, `AngularMomentum`, `AngularVelocity`, `Impulse`, `Force`, `FrictionForce`, `DragForce`, `DragCoefficient`, `MomentOfInertia`, `CenterOfMass`, `CenterOfGravity`, `ReducedMass`, `VelocityVerlet`, `ElasticCollision` |
| Energy & Power | `Energy`, `KineticEnergy`, `PotentialEnergy`, `GravitationalEnergy`, `Power`, `ThermalRadiation` |
| Gravitation | `Gravity`, `GravitationalForce`, `GravitationalIntensity`, `GravitationalPotential`, `NBodyGravity` |
| Celestial mechanics | `Barycenter`, `EscapeVelocity`, `MeanAnomaly`, `MeanMotion`, `OrbitalEccentricity`, `OrbitalPeriod`, `SemiMajorAxis`, `TrueAnomaly`, `OrbitalVelocity`, `HillSphere`, `RocheLimit`, `EccentricAnomaly`, `OrbitalStateVectors`, `MeanLongitude` |
| Other | `Density`, `Distance`, `PlaneAngle`, `SolidAngle`, `Frequency`, `Pressure`, `AbsorbedDose` |

## Setup

```xml
<PackageReference Include="SPhysics" Version="X.X.X" />
```

Replace `X.X.X` with the current version from [NuGet](https://www.nuget.org/packages/SPhysics).
The package targets `net8.0` and runs on any newer runtime.

## Usage

Gravitation, with the numeric type inferred from the arguments:

```cs
using SPhysics;
using SPhysics.CelestialMechanics;

const double G = 6.67430e-11;

// Newton's law of universal gravitation between two point bodies
var force = GravitationalForce.Outside(mass1: 5.972e24, mass2: 7.342e22, distance: 3.844e8);

// Escape velocity from a spherically symmetric body of given mass and radius
var vEsc = EscapeVelocity.Eval(mass: 5.972e24, distance: 6.371e6, gravitationalConstant: G);
```

Energy and motion:

```cs
using SPhysics;

// Linear kinetic energy
var keLinear = KineticEnergy.Linear(mass: 1.5, velocity: 12.0);

// Spinning kinetic energy
var keSpin = KineticEnergy.Spinning(momentOfInertia: 0.4, angularVelocity: 3.0);

// Gravitational potential energy between two bodies
var epg = GravitationalEnergy.Eval(mass1: 5.972e24, mass2: 1.5, distance: 6.371e6);
```

N-body force accumulation into a stack allocated span:

```cs
using SPhysics;

const double G = 6.67430e-11;

Span<(double X, double Y, double Mass)> bodies = stackalloc (double, double, double)[]
{
    (0, 0, 5.972e24),
    (3.844e8, 0, 7.342e22),
    (1.496e11, 0, 1.989e30),
};

var (fx, fy) = GravitationalForce.Eval<double>(bodies, index: 0, gravitationConst: G);
```

## Performance

`NBodyGravity` evaluates all pairs, O(n^2), in one of two kernels:

- **Vectorized** across the pairs: one `Vector<T>` evaluates 8 float or 4 double pairs at once with AVX2.
  The overloads over tuples copy the components into pooled spans first; `NBodyGravity.Soa` takes them in separate
  spans (structure of arrays) and skips the copies.
- **Scalar**, `NBodyGravity.Scalar`: pair by pair on the tuples, no copies.

Time of one evaluation, float bodies, a gravitational constant per body
([NBodyGravityBenchmark](src/quality/SPhysics.Benchmarks/NBodyGravityBenchmark.cs), x64 with AVX2):

| Overloads | 4 bodies | 16 | 64 | 512 | 3000 |
| --- | ---: | ---: | ---: | ---: | ---: |
| `NBodyGravity.Scalar`, 2D | 55 ns | 0.72 µs | 11.9 µs | 0.76 ms | 26.0 ms |
| `NBodyGravity`, 2D | 123 ns | 0.78 µs | 4.6 µs | 0.12 ms | 3.6 ms |
| `NBodyGravity.Soa`, 2D | 87 ns | 0.74 µs | 4.2 µs | 0.12 ms | 3.5 ms |
| `NBodyGravity.Scalar`, 3D | 71 ns | 0.98 µs | 16.2 µs | 1.04 ms | 35.4 ms |
| `NBodyGravity`, 3D | 169 ns | 0.94 µs | 5.9 µs | 0.17 ms | 5.0 ms |
| `NBodyGravity.Soa`, 3D | 103 ns | 0.84 µs | 5.6 µs | 0.17 ms | 5.0 ms |

- The scalar kernel is faster below about 16 bodies; from there the vectorized one wins, 7x for thousands of bodies.
  The caller picks the kernel for its body count.
- The kernels differ in rounding. The vectorized one sums the pulls on a body in several lanes, so its rounding
  depends on the vector width: reproducible on one machine, not across machines with different vector widths.
  The scalar one is reproducible everywhere.
- `VelocityVerlet.Soa` advances the structure of arrays, bitwise identical to the tuple steps.

## Contributing

Ideas, bug reports and pull requests are welcome. Open an
[issue](https://github.com/jirikostiha/sphysics/issues/new/choose) to propose a change, or send a
[pull request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request)
directly.

## License

Project is under [MIT](./LICENSE) license.
