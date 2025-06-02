using WorldWideAstronomy;

namespace AlignmentTest;

public class Program
{
    static void Main(string[] args)
    {
        DateTime now = DateTime.UtcNow;
        double gmst = AstroUtil.ToMeanSiderealTime(now);
        Console.WriteLine($"GMST: {AstroUtil.RadiansToHMS(gmst)}");

        // Right Ascension (RA) 6 hr 45 min 09 sec, Declination (Dec) -16 deg 42 min 58 sec
        // Latitude 41.
        /*
        Angle latitude = Angle.ByDegree(41.27d);
        PolarCoordinates siriusCoordinates = new PolarCoordinates() {
            rightAscension = Angle.ByDegree(AstroUtil.HoursToDegrees(6 + 45 / 60d + 9 / 3600d)),
            declination = Angle.ByDegree(-(16d + 42 / 60d + 58 / 3600d))
        };
        */

        double latitude = AstroUtil.ToRadians(41.2915117d); // 41 17 29.4426 N
        double longitude = AstroUtil.ToRadians(-74.3785292d); // 74 22 42.7044 W
        System.Console.WriteLine($"Latitude: {AstroUtil.RadiansToDMS(latitude)}");
        System.Console.WriteLine($"Longitude: {AstroUtil.RadiansToDMS(longitude)}");
        EquatorialCoordinates siriusCoordinates = new EquatorialCoordinates(
            ra: -0.69112174d,
            dec: 0.14718022d);
        LocalEquatorialCoordinates localSiriusCoordinates = siriusCoordinates.ToLocalEquatorial(now, longitude);
        HorizonCoordinates siriusHorizonCoordinates_sofa = EquatorialToHorizon_sofa(localSiriusCoordinates, latitude);
        HorizonCoordinates siriusHorizonCoordinates_direct = EquatorialToHorizon_direct(localSiriusCoordinates, latitude);
        LocalEquatorialCoordinates siriusEquatorialCoordinates_sofa = HorizonToEquatorial_sofa(siriusHorizonCoordinates_sofa, latitude);
        LocalEquatorialCoordinates siriusEquatorialCoordinates_direct = HorizonToEquatorial_direct(siriusHorizonCoordinates_direct, latitude);
        System.Console.WriteLine($"Sirius Original: {siriusCoordinates}");
        System.Console.WriteLine($"Sirius Horizon (SOFA): {siriusHorizonCoordinates_sofa}");
        System.Console.WriteLine($"Sirius Horizon (direct): {siriusHorizonCoordinates_direct}");
        System.Console.WriteLine($"Sirius Equatorial (SOFA): {siriusEquatorialCoordinates_sofa}");
        System.Console.WriteLine($"Sirius Equatorial (direct): {siriusEquatorialCoordinates_direct}");
    }

    public static LocalEquatorialCoordinates HorizonToEquatorial_sofa(HorizonCoordinates horizon, double latitude) {
        double ha = double.NaN;
        double dec = double.NaN;
        WWA.wwaAe2hd(az: horizon.Azimuth, el: horizon.Altitude, phi: latitude, ha: ref ha, dec: ref dec);
        return new LocalEquatorialCoordinates(
            lha: ha,
            dec: dec);
    }
    public static LocalEquatorialCoordinates HorizonToEquatorial_direct(HorizonCoordinates horizon, double latitude) {
        var horizonRect = horizon.ToPolar().ToDirectionCosine().Rotate_Y(latitude - AstroUtil.HALF_PI);
        var pc = horizonRect.ToPolarCoordinates().AsLocalEquatorial();
        return pc;
    }
    public static HorizonCoordinates EquatorialToHorizon_direct(LocalEquatorialCoordinates equatorial, double latitude) {
        var horizonRect = equatorial.ToPolar().ToDirectionCosine().Rotate_Y(AstroUtil.HALF_PI - latitude);
        var pc = horizonRect.ToPolarCoordinates().AsHorizon();
        return pc;
    }

    public static HorizonCoordinates EquatorialToHorizon_sofa(LocalEquatorialCoordinates equatorial, double latitude) {
        double alt = double.NaN;
        double az = double.NaN;
        WWA.wwaHd2ae(ha: equatorial.LocalHourAngle, dec: equatorial.Declination, phi: latitude, az: ref az, el: ref alt);
        return new HorizonCoordinates(
            azimuth: az,
            altitude: alt);
    }
}
