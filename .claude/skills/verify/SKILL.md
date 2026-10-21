---
name: verify
description: Verify a change of SPhysics at its surface, the NuGet package, by a sample app that references the packed package.
---

# Verify SPhysics through the package

SPhysics is a library, its surface is the package boundary, not `src/`.

1. Pack with a unique suffix, so no cached package is reused:
   `dotnet pack src/code/SPhysics -c Release -o <scratch>/feed -p:Version=<x>-v<HHmmss>`
   (the version is the `Version` element of `SPhysics.csproj`)
2. In `<scratch>/app` keep a `nuget.config` with `<clear />`, the local feed mapped to
   `SMath` (and `SPhysics`) and nuget.org for the rest, and a net8.0 console project with
   `<PackageReference Include="SPhysics" Version="$(SPhysicsVersion)" />`.
3. Run `dotnet run -c Release -p:SPhysicsVersion=<x>-v<HHmmss>` and check in
   `obj/project.assets.json` that the packed version was restored.
4. Drive the changed public API from `Program.cs`, then probe the edges (zero, NaN,
   int.MinValue, grid boundary, corner cases).

Gotchas: SPhysics references a released SMath package; pack SMath into the same feed with
`-p:VersionSuffix=v<HHmmss>` and reference it too to check SPhysics against an unreleased SMath. `cmd/Publish-Release.ps1` needs `-VersionFile src/code/SPhysics/SPhysics.csproj`.
