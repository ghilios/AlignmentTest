using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignmentTest;

public record LocalEquatorialCoordinates {
    public LocalEquatorialCoordinates(double lha, double dec) {
        this.LocalHourAngle = AstroUtil.ClampPositiveRadians(lha);
        this.Declination = AstroUtil.ClampHalfPositiveRadians(dec);
    }

    public double Declination { get; private set; }
    public double LocalHourAngle { get; private set; }

    public override string ToString() {
        return $"LHA {AstroUtil.RadiansToHMS(LocalHourAngle)} Dec {AstroUtil.RadiansToDMS(Declination)}";
    }

    public static LocalEquatorialCoordinates FromPolar(PolarCoordinates pc) {
        return new LocalEquatorialCoordinates(
            lha: Math.PI - pc.Theta,
            dec: pc.Phi);
    }

    public PolarCoordinates ToPolar() {
        return new PolarCoordinates(
            phi: Declination,
            theta: Math.PI - LocalHourAngle);
    }
}