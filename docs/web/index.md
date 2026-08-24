# SPhysics Documentation

Physics for .NET, written against [generic math](https://learn.microsoft.com/en-us/dotnet/standard/generics/math).
Every formula is generic over the numeric type, so the same call site works for `double`, `float`, `decimal` or `Half` without overloads or casting. Builds on top of [SMath](https://github.com/jirikostiha/smath).

---

## Design Principles

- **Generic over the number type:** Constraints express the physics rather than a concrete type: a formula needing a square root asks for `IRootFunctions<N>`, one needing only addition asks for `INumberBase<N>`. Passing a type that cannot support the operation is a compile error.
- **Static and allocation free:** There are no wrapper structs for quantities or vectors. Values are plain scalars and tuples, so they stay on the stack and interoperate with any other library. All entry points are static.
- **Names that read as the formula:** Types are named after the quantity being computed, giving call sites like `GravitationalForce.Outside(m1, m2, r)` or `KineticEnergy.Linear(m, v)`.

---

## Installation

Install the NuGet package:

```shell
dotnet add package SPhysics
```

---

## Quick Navigation

- [API Reference](api/index.md) - Full reference documentation for all namespaces, types, and physical formulas.
- [GitHub Repository](https://github.com/jirikostiha/sphysics) - Source code, issues, and discussions.

---

## Overview of Modules

| Area | Types & Capabilities |
| :--- | :--- |
| **Base Quantities** | `Length`, `Mass`, `Time`, `ElectricCurrent`, `Temperature`, `AmountOfSubstance`, `LuminousIntensity` |
| **Mechanics** | `Velocity`, `Acceleration`, `Momentum`, `AngularMomentum`, `AngularVelocity`, `Impulse`, `Force`, `FrictionForce`, `DragForce`, `MomentOfInertia`, `CenterOfMass`, `CenterOfGravity`, `VelocityVerlet`, `ElasticCollision` |
| **Energy & Power** | `Energy`, `KineticEnergy`, `PotencialEnergy`, `GravitationalEnergy`, `Power` |
| **Gravitation** | `Gravity`, `GravitationalForce`, `GravitationalIntensity`, `GravitationalPotential`, `NBodyGravity` |
| **Celestial Mechanics** | `Barycenter`, `EscapeVelocity`, `MeanAnomaly`, `MeanMotion`, `OrbitalEccentricity`, `OrbitalPeriod`, `TrueAnomaly` |
| **Other Quantities** | `Density`, `Distance`, `Frequency`, `Pressure`, `AbsorbedDose` |
