---
name: verify
description: Verify a change of SPhysics at its surface, the NuGet package, by a sample app that references the packed package.
---

# Verify SPhysics through the package

SPhysics is a library, its surface is the package boundary, not `src/`.

1. Pack with a unique suffix, so no cached package is reused:
   `dotnet pack src/code/SPhysics -c Release -o <scratch>/feed -p:Version=<x>-v<HHmmss>`
   (the version is the `Version` element of `SPhysics.csproj`). Keep `<scratch>` outside the
   repository, e.g. `%TEMP%`, so `src/Directory.*.props` do not apply to the sample app.
2. In `<scratch>/app` keep a `nuget.config` with `<clear />`, the local feed (absolute path) and
   nuget.org, and a **net10.0** console project (the package targets net10.0; the formulas of a
   body are C# 14 extension members) with
   `<PackageReference Include="SPhysics" Version="$(SPhysicsVersion)" />`.
3. Restore with that config only, then run without restoring again:
   `dotnet restore -p:SPhysicsVersion=<x>-v<HHmmss> --configfile nuget.config`,
   `dotnet run -c Release --no-restore -p:SPhysicsVersion=<x>-v<HHmmss>`.
   Check in `obj/project.assets.json` that the packed version was restored.
4. Drive the changed public API from `Program.cs`, then probe the edges: zero, NaN, infinity,
   the limiting cases of a formula, and the numeric types `float`, `decimal` and `Half`.
5. Delete `<scratch>`.

Gotchas:
- `--configfile` keeps the machine-level NuGet config out: a source disabled or mapped there
  fails the restore with NU1100 even when the package is in the local feed. Do not combine
  `--source` with package source mapping; mapping then ignores the given sources.
- The repository restore fails the same way when nuget.org is disabled on the machine:
  `dotnet restore src/sphysics.slnx --source https://api.nuget.org/v3/index.json`.
- SPhysics references a released SMath package; pack SMath into the same feed with
  `-p:VersionSuffix=v<HHmmss>` and reference it too to check SPhysics against an unreleased SMath.
- `cmd/Publish-Release.ps1` needs `-VersionFile src/code/SPhysics/SPhysics.csproj`.
