using AlignmentTest.Solver;
using WorldWideAstronomy;

namespace AlignmentTest {
    public class AlignmentDataPoint : INonLinearLeastSquaresDataPoint {
        private readonly EncoderPosition encoderPosition;
        private readonly HorizonCoordinates skyCoordinates;
        public AlignmentDataPoint(EncoderPosition encoderPosition, HorizonCoordinates skyCoordinates) {
            this.encoderPosition = encoderPosition;
            this.skyCoordinates = skyCoordinates;
        }

        public double[] ToInput() {
            return new double[] {
                this.encoderPosition.AltitudePosition,
                this.encoderPosition.AzimuthPosition,
                this.skyCoordinates.Altitude,
                this.skyCoordinates.Azimuth,
            };
        }

        public double ToOutput() {
            // Target angle separation
            return 0.0d;
        }
    }

    public class AlignmentModel : INonLinearLeastSquaresParameters {
        public AlignmentModel(AlignmentParameters alignment) {
            this.TiltAngle = alignment.TiltAngle;
            this.TiltAmount = alignment.TiltAmount;
            this.AltitudeOffset = alignment.AltitudeOffset;
            this.AzimuthOffset = alignment.AzimuthOffset;
        }

        public AlignmentModel() {
            this.TiltAngle = 0;
            this.TiltAmount = 0;
            this.AltitudeOffset = 0;
            this.AzimuthOffset = 0;
        }

        public void FromArray(double[] parameters) {
            if (parameters == null || parameters.Length != 4) {
                throw new ArgumentException($"Expected a 4-element array of parameters");
            }

            this.TiltAngle = parameters[0];
            this.TiltAmount = parameters[1];
            this.AltitudeOffset = parameters[2];
            this.AzimuthOffset = parameters[3];
        }

        public double[] ToArray() {
            return new double[] {
                this.TiltAngle,
                this.TiltAmount,
                this.AltitudeOffset,
                this.AzimuthOffset,
            };
        }

        public double TiltAngle { get; private set; }
        public double TiltAmount { get; private set; }
        public double AltitudeOffset { get; private set; }
        public double AzimuthOffset { get; private set; }
    }

    public class AlignmentSolver : NonLinearLeastSquaresSolverBase<AlignmentDataPoint, AlignmentModel> {
        private readonly HardwareParameters hardware;

        public AlignmentSolver(
            List<AlignmentDataPoint> dataPoints,
            HardwareParameters hardware) : base(dataPoints, 4) {
            this.hardware = hardware;
        }

        public override double NumericIntegrationIntervalSize => 1E-9;


        public override void SetBounds(double[] lowerBounds, double[] upperBounds) {
            lowerBounds[0] = 0.0;
            upperBounds[0] = WWA.D2PI;
            lowerBounds[1] = 1E-5; // Perfection is impossible, so ensure at least some minimal amount of tilt so a solution can converge
            WWA.wwaAf2a('+', 10, 0, 0, ref upperBounds[1]);
            lowerBounds[2] = 0;
            upperBounds[2] = this.hardware.AltitudeSteps;
            lowerBounds[3] = 0;
            upperBounds[3] = this.hardware.AzimuthSteps;
        }

        public override void SetInitialGuess(double[] initialGuess) {
            initialGuess[0] = 0.0;
            initialGuess[1] = 1E-5;
            initialGuess[2] = 0.0; // TODO: Be better about an initial guess
            initialGuess[3] = 0.0;
        }

        public override void SetScale(double[] scales) {
            scales[0] = WWA.D2PI;
            WWA.wwaAf2a('+', 10, 0, 0, ref scales[1]);
            scales[2] = this.hardware.AltitudeSteps;
            scales[3] = this.hardware.AzimuthSteps;
        }

        public override double Value(double[] parameters, double[] input) {
            double altitudePosition = input[0];
            double azimuthPosition = input[1];
            double skyAltitude = input[2];
            double skyAzimuth = input[3];
            double tiltAngle = parameters[0];
            double tiltAmount = parameters[1];
            double altitudeOffset = parameters[2];
            double azimuthOffset = parameters[3];
            var encoderPosition = new EncoderPosition() { AltitudePosition = (int)altitudePosition, AzimuthPosition = (int)azimuthPosition };
            var alignment = new AlignmentParameters() { AltitudeOffset = altitudeOffset, AzimuthOffset = azimuthOffset, TiltAmount = tiltAmount, TiltAngle = tiltAngle };
            var modeledSkyCoordinates = CoordinateSpaceTransformations.ToSkyHorizonCoordinates(encoderPosition, hardware, alignment);
            var result = WWA.wwaSeps(modeledSkyCoordinates.Azimuth, modeledSkyCoordinates.Altitude, skyAzimuth, skyAltitude);
            return result;
        }
    }
}
