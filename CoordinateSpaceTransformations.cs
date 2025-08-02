using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldWideAstronomy;

namespace AlignmentTest {
    public record HardwareParameters {
        public required int AltitudeSteps { get; init; }
        public required int AzimuthSteps { get; init; }
        public required double Latitude { get; init; }
        public required double Longitude { get; init; }
    }

    public record AlignmentParameters {
        public required double AltitudeOffset { get; init; }
        public required double AzimuthOffset { get; init; }
        public required double TiltAngle { get; init; }
        public required double TiltAmount { get; init; }
    }

    public record EncoderPosition {
        public required int AltitudePosition { get; init; }
        public required int AzimuthPosition { get; init; }
    }

    public static class CoordinateSpaceTransformations {
        public static HorizonCoordinates ToSkyHorizonCoordinates(
            EncoderPosition position,
            HardwareParameters hardware,
            AlignmentParameters alignment) {
            double absoluteAltitudeAngle = WWA.wwaAnpm(WWA.D2PI * (position.AltitudePosition + alignment.AltitudeOffset) / hardware.AltitudeSteps);
            double absoluteAzimuthAngle = WWA.wwaAnp(WWA.D2PI * (position.AzimuthPosition + alignment.AzimuthOffset) / hardware.AzimuthSteps);
            double[] telescopeDirectionCosine = new double[3];
            WWA.wwaS2c(theta: absoluteAzimuthAngle, phi: absoluteAltitudeAngle, telescopeDirectionCosine);

            double[,] tiltRotationMat = new double[3,3];
            WWA.wwaIr(tiltRotationMat);
            WWA.wwaRz(alignment.TiltAngle, tiltRotationMat);
            WWA.wwaRx(alignment.TiltAmount, tiltRotationMat);

            double[] skyDirectionCosine = new double[3];
            WWA.wwaRxp(tiltRotationMat, telescopeDirectionCosine, skyDirectionCosine);

            double skyTheta = double.NaN;
            double skyPhi = double.NaN;
            WWA.wwaC2s(skyDirectionCosine, ref skyTheta, ref skyPhi);
            var result = new HorizonCoordinates(azimuth: skyTheta, altitude: skyPhi);
            return result;
        }
    }
}