using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldWideAstronomy;

namespace AlignmentTest;

public record EquatorialCoordinates {
    public EquatorialCoordinates(double ra, double dec) {
        this.RightAscension = AstroUtil.ClampPositiveRadians(ra);
        this.Declination = AstroUtil.ClampHalfPositiveRadians(dec);
    }

    public double Declination { get; private set; }
    public double RightAscension { get; private set; }

    public override string ToString() {
        return $"RA {AstroUtil.RadiansToHMS(RightAscension)} Dec {AstroUtil.RadiansToDMS(Declination)}";
    }

    public LocalEquatorialCoordinates ToLocalEquatorial(DateTime dateTime, double longitude) {
        var siderealTime = AstroUtil.ToMeanSiderealTime(dateTime);
        var localSiderealTime = WWA.wwaAnp(siderealTime - longitude);
        var localHourAngle = localSiderealTime - this.RightAscension;
        return new LocalEquatorialCoordinates(lha: localHourAngle, dec: this.Declination);
    }
}