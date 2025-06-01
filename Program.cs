using WorldWideAstronomy;

namespace AlignmentTest;

public class Program
{
    static void Main(string[] args)
    {
        var d = 2458484.833333d - 2451545.0d;
        // var sofaJd = WWA.wwaGmst06(2451545.0d, 6935.5d + (8.0d/24.0d), 0.0d, 0.0d);
        var sofaJd = WWA.wwaGmst06(2458484.833333d, 0.0d, 2458484.833333d, 0.0d);
        string hms = AstroUtil.RadiansToHMS(sofaJd);
        Console.WriteLine(hms);

        // Right Ascension (RA) 6 hr 45 min 09 sec, Declination (Dec) -16 deg 42 min 58 sec
        // Latitude 41.
        /*
        Angle latitude = Angle.ByDegree(41.27d);
        PolarCoordinates siriusCoordinates = new PolarCoordinates() {
            rightAscension = Angle.ByDegree(AstroUtil.HoursToDegrees(6 + 45 / 60d + 9 / 3600d)),
            declination = Angle.ByDegree(-(16d + 42 / 60d + 58 / 3600d))
        };
        */

        double latitude = 0.88660302d;
        System.Console.WriteLine($"Latitude: {AstroUtil.RadiansToDMS(latitude)}");
        EquatorialCoordinates siriusCoordinates = new EquatorialCoordinates(
            ra: -0.69112174d,
            dec: 0.14718022d);
        System.Console.WriteLine(siriusCoordinates.ToPolar().ToDirectionCosine());

        HorizonCoordinates siriusHorizonCoordinates_sofa = EquatorialToHorizon_sofa(siriusCoordinates, latitude);
        HorizonCoordinates siriusHorizonCoordinates_direct = EquatorialToHorizon_direct(siriusCoordinates, latitude);
        EquatorialCoordinates siriusEquatorialCoordinates_sofa = HorizonToEquatorial_sofa(siriusHorizonCoordinates_sofa, latitude);
        EquatorialCoordinates siriusEquatorialCoordinates_direct = HorizonToEquatorial_direct(siriusHorizonCoordinates_direct, latitude);
        System.Console.WriteLine($"Sirius Original: {siriusCoordinates}");
        System.Console.WriteLine($"Sirius Horizon (SOFA): {siriusHorizonCoordinates_sofa}");
        System.Console.WriteLine($"Sirius Horizon (direct): {siriusHorizonCoordinates_direct}");
        System.Console.WriteLine($"Sirius Equatorial (SOFA): {siriusEquatorialCoordinates_sofa}");
        System.Console.WriteLine($"Sirius Equatorial (direct): {siriusEquatorialCoordinates_direct}");
    }

    public static EquatorialCoordinates HorizonToEquatorial_sofa(HorizonCoordinates horizon, double latitude) {
        double ha = double.NaN;
        double dec = double.NaN;
        WWA.wwaAe2hd(az: horizon.Azimuth, el: horizon.Altitude, phi: latitude, ha: ref ha, dec: ref dec);
        return new EquatorialCoordinates(
            ra: ha,
            dec: dec);
    }
    public static EquatorialCoordinates HorizonToEquatorial_direct(HorizonCoordinates horizon, double latitude) {
        var horizonRect = horizon.ToPolar().ToDirectionCosine().Rotate_Y(latitude - AstroUtil.HALF_PI);
        var pc = horizonRect.ToPolarCoordinates().AsEquatorial();
        return pc;
    }
    public static HorizonCoordinates EquatorialToHorizon_direct(EquatorialCoordinates equatorial, double latitude) {
        var horizonRect = equatorial.ToPolar().ToDirectionCosine().Rotate_Y(AstroUtil.HALF_PI - latitude);
        var pc = horizonRect.ToPolarCoordinates().AsHorizon();
        return pc;
    }

    public static HorizonCoordinates EquatorialToHorizon_sofa(EquatorialCoordinates equatorial, double latitude) {
        double alt = double.NaN;
        double az = double.NaN;
        WWA.wwaHd2ae(ha: equatorial.RightAscension, dec: equatorial.Declination, phi: latitude, az: ref az, el: ref alt);
        return new HorizonCoordinates(
            azimuth: az,
            altitude: alt);
    }
}
