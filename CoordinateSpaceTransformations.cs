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
        public required int AltitudeOffset { get; init; }
        public required int AzimuthOffset { get; init; }
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
            int absoluteAltitudePosition = (position.AltitudePosition + alignment.AltitudeOffset) % hardware.AltitudeSteps;
            int absoluteAzimuthPosition = (position.AzimuthPosition + alignment.AzimuthOffset) % hardware.AzimuthSteps;
            double absoluteAltitudeAngle = WWA.wwaAnpm(WWA.D2PI * absoluteAltitudePosition / hardware.AltitudeSteps);
            double absoluteAzimuthAngle = WWA.wwaAnp(WWA.D2PI * absoluteAzimuthPosition / hardware.AzimuthSteps);
            var telescopeCoordinates = new HorizonCoordinates(altitude: absoluteAltitudeAngle, azimuth: absoluteAltitudeAngle);
            return telescopeCoordinates
                .ToPolar()
                .ToDirectionCosine()
                .Tilt(tiltAngle: alignment.TiltAngle, tiltAmount: alignment.TiltAmount)
                .ToPolarCoordinates()
                .AsHorizon();
        }
    }
}
