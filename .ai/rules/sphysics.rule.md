---
name: sphysics
description: "SPhysics rules: static classes only, and empty readonly structs that group the formulas of a body. Always in force in this repository."
load: always
applyTo: "**"
---

# SPhysics Rules

Always in force in this repository.

## SPH-01 · Static classes only

This project can only contain static classes and no instance classes. The one exception is an empty
`readonly struct` that groups formulas of a body under a C# 14 static extension property, so that
`Sphere.Region.MomentOfInertia.FromMass(m, r)` reads like the geometry of SMath. Such a struct has no
fields and no state.

## SPH-02 · Formulas of a body belong to its shape

A formula that relates to a body is an extension of the SMath shape the body is made of, never a
class of its own: a solid ball extends `Sphere.Region`, a disc `Circle.Region`. Geometry, such as an
area or a volume, stays in SMath and is reused, not repeated.
