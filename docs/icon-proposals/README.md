# Icon proposals

Draft proposals for a new SPhysics icon. The current icon
(`src/code/SPhysics/icon.png`) is an abstract geometric mark that does not
reference the library's content. These proposals instead reference the
macroscopic domain the library covers — mechanics, gravitation and celestial
mechanics — using a green palette on a rounded-square badge, in the style of
a package/app icon (NuGet, GitHub social preview, etc.).

Each proposal ships as a source `.svg` (256x256 viewBox, edit freely) plus
rendered `.png` exports at 512x512 and 128x128 (NuGet's recommended package
icon size).

| | File | Concept |
| --- | --- | --- |
| A | `icon-a-orbit-s.svg` | A single flowing orbit path shaped like the letter **S** (SPhysics), with a body at the inflection point and one at each end — reads as both a logotype and a two-body orbit. |
| B | `icon-b-kepler.svg` | A classic two-body system: a central mass, a dashed elliptical orbit, and a smaller orbiting body with a short motion trail — directly matches the `CelestialMechanics` / `Gravitation` areas. |
| C | `icon-c-apple.svg` | A falling apple on a curved dashed trajectory — a familiar shorthand for gravity/Newtonian mechanics (`Gravity`, `GravitationalForce`, `Force`). |
| D | `icon-d-vectors.svg` | A force vector resolved into its components, with a right-angle marker — references the `Mechanics` area (`Force`, `Momentum`, `Velocity`, `Acceleration`). |
| E | `icon-e-pendulum.svg` | A pendulum with a dashed swing arc — a classic mechanics/energy motif (`KineticEnergy`, `PotencialEnergy`, oscillatory motion). |

None of these replace the current icon automatically — pick one (or ask for
a variation) and it can be wired into `src/code/SPhysics/icon.png` /
the NuGet package metadata and the readme header.
