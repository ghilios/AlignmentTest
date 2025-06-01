using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignmentTest;

public record RectangularCoordinates {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public PolarCoordinates ToPolarCoordinates() {
        return new PolarCoordinates(phi: Math.Asin(Z), theta: Math.Atan2(Y, X));
    }

    public override string ToString() {
        return $"{X} {Y} {Z}";
    }

    public RectangularCoordinates Rotate_X(double angle) {
        var sa = Math.Sin(angle);
        var ca = Math.Cos(angle);
        return new RectangularCoordinates() {
            X = X,
            Y = ca * Y - sa * Z,
            Z = sa * Y + ca * Z
        };
    }

    public RectangularCoordinates Rotate_Y(double angle) {
        var sa = Math.Sin(angle);
        var ca = Math.Cos(angle);
        return new RectangularCoordinates() {
            X = ca * X + sa * Z,
            Y = Y,
            Z = -sa * X + ca * Z
        };
    }

    public RectangularCoordinates Rotate_Z(double angle) {
        var sa = Math.Sin(angle);
        var ca = Math.Cos(angle);
        return new RectangularCoordinates() {
            X = ca * X - sa * Y,
            Y = sa * X + ca * Y,
            Z = Z
        };
    }

    public RectangularCoordinates Tilt(double tiltAngle, double tiltAmount) {
        return Rotate_Z(tiltAngle).Rotate_Y(tiltAmount);
    }
}