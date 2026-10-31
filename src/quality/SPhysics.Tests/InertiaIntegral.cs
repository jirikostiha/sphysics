namespace SPhysics.Tests;

/// <summary>
/// Volumes and second moments of uniform bodies integrated numerically from the definition, I = integral of r^2 dm,
/// so the closed forms are checked against an independent computation rather than against themselves.
/// </summary>
internal static class InertiaIntegral
{
    /// <summary>
    /// Body of revolution about the z axis, spanning z0..z1, whose cross section at z is the annulus between the radii.
    /// Midpoint rule in z and in rho with dV = 2 pi rho drho dz. The transverse integrand is about the x axis shifted
    /// to z = zc: x^2 averaged over the azimuth is rho^2 / 2.
    /// </summary>
    /// <returns> The volume and the integrals of rho^2 and of x^2 + (z - zc)^2 over it. </returns>
    public static (double Volume, double Axial, double Transverse) OfRevolution(
        double z0, double z1, Func<double, (double Inner, double Outer)> radii, double zc, int n = 2000)
    {
        double volume = 0, axial = 0, transverse = 0;
        double dz = (z1 - z0) / n;

        for (int i = 0; i < n; i++)
        {
            double z = z0 + ((i + 0.5) * dz);
            var (inner, outer) = radii(z);
            double drho = (outer - inner) / n;

            for (int j = 0; j < n; j++)
            {
                double rho = inner + ((j + 0.5) * drho);
                double dv = 2 * Math.PI * rho * drho * dz;

                volume += dv;
                axial += rho * rho * dv;
                transverse += ((rho * rho / 2) + ((z - zc) * (z - zc))) * dv;
            }
        }

        return (volume, axial, transverse);
    }

    /// <summary>
    /// Ellipsoid of semi-axes a, b, c centred in the origin. Midpoint rule over x and y, the z extent at (x, y)
    /// integrated exactly.
    /// </summary>
    /// <returns> The volume and the integrals of y^2 + z^2, x^2 + z^2 and x^2 + y^2 over it. </returns>
    public static (double Volume, double X, double Y, double Z) OfEllipsoid(double a, double b, double c, int n = 2000)
    {
        double volume = 0, xx = 0, yy = 0, zz = 0;
        double dx = 2 * a / n;
        double dy = 2 * b / n;

        for (int i = 0; i < n; i++)
        {
            double x = -a + ((i + 0.5) * dx);

            for (int j = 0; j < n; j++)
            {
                double y = -b + ((j + 0.5) * dy);
                double s = 1 - (x * x / (a * a)) - (y * y / (b * b));
                if (s <= 0)
                    continue;

                double halfHeight = c * Math.Sqrt(s);
                double dv = 2 * halfHeight * dx * dy;      // integral of dz over -h..h
                double z2 = 2 * halfHeight * halfHeight * halfHeight / 3 * dx * dy; // integral of z^2 dz over -h..h

                volume += dv;
                xx += (y * y * dv) + z2;
                yy += (x * x * dv) + z2;
                zz += ((x * x) + (y * y)) * dv;
            }
        }

        return (volume, xx, yy, zz);
    }

    /// <summary>
    /// Elliptical plate of semi-axes a, b centred in the origin. Midpoint rule over x, the y extent at x integrated exactly.
    /// </summary>
    /// <returns> The area and the integral of x^2 + y^2 over it. </returns>
    public static (double Area, double Polar) OfEllipse(double a, double b, int n = 200_000)
    {
        double area = 0, polar = 0;
        double dx = 2 * a / n;

        for (int i = 0; i < n; i++)
        {
            double x = -a + ((i + 0.5) * dx);
            double halfHeight = b * Math.Sqrt(1 - (x * x / (a * a)));

            area += 2 * halfHeight * dx;
            polar += ((2 * halfHeight * x * x) + (2 * halfHeight * halfHeight * halfHeight / 3)) * dx;
        }

        return (area, polar);
    }
}
