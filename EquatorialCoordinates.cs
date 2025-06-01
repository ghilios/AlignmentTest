using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignmentTest;

public record EquatorialCoordinates {
    public EquatorialCoordinates(double ra, double dec) {
        this.RightAscension = AstroUtil.ClampPositiveRadians(ra);
        this.Declination = AstroUtil.ClampHalfPositiveRadians(dec);
    }

    public double Declination { get; private set; }
    public double RightAscension { get; private set; }

    public override string ToString() {
        return $"RA {AstroUtil.RadiansToDMS(RightAscension)} Dec {AstroUtil.RadiansToDMS(Declination)}";
    }

    public static EquatorialCoordinates FromPolar(PolarCoordinates pc) {
        return new EquatorialCoordinates(
            ra: Math.PI - pc.Theta,
            dec: pc.Phi);
    }

    public PolarCoordinates ToPolar() {
        return new PolarCoordinates(
            phi: Declination,
            theta: Math.PI - RightAscension);
    }
}