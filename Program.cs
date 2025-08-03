using AlignmentTest.Solver;
using WorldWideAstronomy;

namespace AlignmentTest;

public class Program
{
    static void Main(string[] args) {
        double latitude = double.NaN;
        double longitude = double.NaN;
        WWA.wwaAf2a('+', 41, 17, 29.44, ref latitude);
        WWA.wwaAf2a('-', 74, 22, 42.72, ref longitude);

        LocationParameters location = new LocationParameters() { Elevation = 300, Latitude = latitude, Longitude = longitude };
        var encoderPosition = new EncoderPosition() { AltitudePosition = 0, AzimuthPosition = -8192 };
        var hardwareParameters = new HardwareParameters() { AltitudeSteps = 32768, AzimuthSteps = 32768 };
        var alignmentParameters = new AlignmentParameters() { AltitudeOffset = 1500, AzimuthOffset = 1000, TiltAmount = AstroUtil.ToRadians(3.0), TiltAngle = AstroUtil.ToRadians(46) };
        EpochTimeParameters epochParameters = EpochTimeParameters.CalculateAt(location);

        // Alphard
        /*
        double ra = double.NaN;
        double dec = double.NaN;
        WWA.wwaTf2a('+', 9, 27, 33.94, ref ra);
        WWA.wwaAf2a('-', 8, 39, 25.5, ref dec);
        var equatorial = new EquatorialCoordinates(ra, dec);
        var epochPosition = new EpochPositionParameters(location, hardwareParameters, alignmentParameters, epochParameters);
        while (true) {
            EpochTimeParameters updatedParameters = EpochTimeParameters.CalculateAt(location);
            epochPosition.UpdateAt(updatedParameters);
            var observed = epochPosition.ICRSToObserved(equatorial);
            System.Console.Write($"\r{observed}        ");
            Thread.Sleep(250);
        }
        */

        var encoderPosition1 = new EncoderPosition() { AltitudePosition = 9000, AzimuthPosition = 8000 };
        var encoderPosition2 = new EncoderPosition() { AltitudePosition = 16450, AzimuthPosition = 13400 };
        var skyPosition1 = CoordinateSpaceTransformations.ToSkyHorizonCoordinates(encoderPosition1, hardwareParameters, alignmentParameters);
        var skyPosition2 = CoordinateSpaceTransformations.ToSkyHorizonCoordinates(encoderPosition2, hardwareParameters, alignmentParameters);

        var dataPoints = new List<AlignmentDataPoint>() {
            new AlignmentDataPoint(encoderPosition1, skyPosition1),
            new AlignmentDataPoint(encoderPosition2, skyPosition2)
        };
        var alglibAPI = new AlglibAPI();
        var solver = new AlignmentSolver(
                    dataPoints: dataPoints,
                    hardware: hardwareParameters);
        var nlSolver = new NonLinearLeastSquaresSolver<AlignmentSolver, AlignmentDataPoint, AlignmentModel>(alglibAPI);
        var solution = nlSolver.Solve(solver, ct: CancellationToken.None, tolerance: 1E-15);

        var optimalParameters = new AlignmentModel(alignmentParameters);
        var value1 = solver.Value(solution.ToArray(), dataPoints[0].ToInput());
        var value2 = solver.Value(solution.ToArray(), dataPoints[1].ToInput());
        var optimalValue1 = solver.Value(optimalParameters.ToArray(), dataPoints[0].ToInput());
        var optimalValue2 = solver.Value(optimalParameters.ToArray(), dataPoints[1].ToInput());

        System.Console.WriteLine();

        /*
        var observed = epochPosition.ToObserved(equatorial);
        var calculatedEquatorial = epochPosition.ToEquatorial(observed);
        */
        Console.WriteLine();

        /*
        while (true) {
            EpochTimeParameters updatedParameters = EpochTimeParameters.CalculateAt(location);
            ITimeParameters approximatedParameters = epochParameters.ApproximateAt(updatedParameters.Epoch);
            Console.Write($"\rLMST: {AstroUtil.RadiansToHMS(updatedParameters.Lmst)}, Approximated: {AstroUtil.RadiansToHMS(approximatedParameters.Lmst)}, Difference: {updatedParameters.Lmst - approximatedParameters.Lmst}         ");
            Thread.Sleep(250);
        }
        */

        DateTime now = DateTime.UtcNow;
        double gmst = AstroUtil.ToMeanSiderealTime(now);
        double lmst = gmst + longitude;
        Console.WriteLine($"GMST: {AstroUtil.RadiansToHMS(gmst)}");
        Console.WriteLine($"LMST: {AstroUtil.RadiansToHMS(lmst)}");

        // var horizonCoordinates = CoordinateSpaceTransformations.ToSkyHorizonCoordinates(encoderPosition, hardwareParameters, alignmentParameters);


        return;

        // Right Ascension (RA) 6 hr 45 min 09 sec, Declination (Dec) -16 deg 42 min 58 sec
        // Latitude 41.
        /*
        Angle latitude = Angle.ByDegree(41.27d);
        PolarCoordinates siriusCoordinates = new PolarCoordinates() {
            rightAscension = Angle.ByDegree(AstroUtil.HoursToDegrees(6 + 45 / 60d + 9 / 3600d)),
            declination = Angle.ByDegree(-(16d + 42 / 60d + 58 / 3600d))
        };
        */


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
