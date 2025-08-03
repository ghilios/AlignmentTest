using WorldWideAstronomy;

namespace AlignmentTest {
    public interface ITimeParameters {
        DateTime At { get; }
        DateTime Epoch { get; }
        double DeltaT { get; }
        double UtcJd1 { get; }
        double UtcJd2 { get; }
        double Dut { get; }
        double Ut1Jd1 { get; }
        double Ut1Jd2 { get; }
        double TaiJd1 { get; }
        double TaiJd2 { get; }
        double TtJd1 { get; }
        double TtJd2 { get; }
        double TdbMinusTt { get; }
        double U { get; }
        double V { get; }
        double Gmst { get; }
        double Lmst { get; }
    }

    public record EpochTimeParameters : ITimeParameters {
        public required DateTime Epoch { get; init; }
        public DateTime At => Epoch;
        public required LocationParameters Location { get; init; }
        public required double DeltaT { get; init; }
        public required double UtcJd1 { get; init; }
        public required double UtcJd2 { get; init; }
        public required double Dut { get; init; }
        public required double Ut1Jd1 { get; init; }
        public required double Ut1Jd2 { get; init; }
        public required double TaiJd1 { get; init; }
        public required double TaiJd2 { get; init; }
        public required double TtJd1 { get; init; }
        public required double TtJd2 { get; init; }
        public required double TdbMinusTt { get; init; }
        public required double U { get; init; }
        public required double V { get; init; }
        public required double Gmst { get; init; }
        public required double Lmst { get; init; }
        public static EpochTimeParameters CalculateAt(LocationParameters location) {
            DateTime epochTime = DateTime.UtcNow;

            double deltaT = double.NaN;
            var res2 = WWA.wwaDat(epochTime.Year, epochTime.Month, epochTime.Day, 0.0d, ref deltaT);

            double utcJd1 = double.NaN;
            double utcJd2 = double.NaN;
            var res = WWA.wwaCal2jd(epochTime.Year, epochTime.Month, epochTime.Day, ref utcJd1, ref utcJd2);

            utcJd2 += epochTime.TimeOfDay.TotalSeconds / WWA.DAYSEC;
            double dut = 0.0d;
            double ut1Jd1 = double.NaN;
            double ut1Jd2 = double.NaN;
            WWA.wwaUtcut1(utcJd1, utcJd2, dut, ref ut1Jd1, ref ut1Jd2);

            double taiJd1 = utcJd1;
            double taiJd2 = utcJd2 + deltaT / WWA.DAYSEC;
            double ttJd1 = double.NaN;
            double ttJd2 = double.NaN;
            WWA.wwaTaitt(taiJd1, taiJd2, ref ttJd1, ref ttJd2);

            double[] geocentricPosition = new double[3];
            WWA.wwaGd2gc(1, location.Longitude, location.Latitude, location.Elevation, geocentricPosition);
            double u = Math.Sqrt(geocentricPosition[0] * geocentricPosition[0] + geocentricPosition[1] * geocentricPosition[1]);
            double v = geocentricPosition[2];

            double ut = Math.IEEERemainder(Math.IEEERemainder(ut1Jd1, 1.0) + Math.IEEERemainder(ut1Jd2, 1.0), 1.0) + 0.5;
            double tdbMinusTt = WWA.wwaDtdb(ttJd1, ttJd2, ut, location.Longitude, u / 1000, v / 1000);

            double gmst = WWA.wwaGmst06(ut1Jd1, ut1Jd2, ttJd1, ttJd2);
            double lmst = gmst + location.Longitude;
            return new EpochTimeParameters() {
                Epoch = epochTime,
                Location = location,
                DeltaT = deltaT,
                UtcJd1 = utcJd1,
                UtcJd2 = utcJd2,
                Dut = dut,
                Ut1Jd1 = ut1Jd1,
                Ut1Jd2 = ut1Jd2,
                TaiJd1 = taiJd1,
                TaiJd2 = taiJd2,
                TtJd1 = ttJd1,
                TtJd2 = ttJd2,
                TdbMinusTt = tdbMinusTt,
                U = u,
                V = v,
                Gmst = gmst,
                Lmst = lmst,
            };
        }

        public ITimeParameters ApproximateAt(DateTime at) {
            return new ApproximatedTimeParameters(this, at);
        }
    }

    public class ApproximatedTimeParameters : ITimeParameters {
        private EpochTimeParameters epochParameters;
        private double deltaSeconds;
        private double deltaDays;

        public ApproximatedTimeParameters(EpochTimeParameters epochParameters, DateTime at) {
            this.At = at.ToUniversalTime();
            this.epochParameters = epochParameters;
            this.deltaSeconds = (this.At - epochParameters.Epoch).TotalSeconds;
            this.deltaDays = this.deltaSeconds / WWA.DAYSEC;
        }

        public DateTime At { get; private set; }

        public DateTime Epoch => this.epochParameters.Epoch;

        public double DeltaT => this.epochParameters.DeltaT;

        public double UtcJd1 => this.epochParameters.UtcJd1;

        public double UtcJd2 => this.epochParameters.UtcJd2 + this.deltaDays;

        public double Dut => this.epochParameters.Dut;

        public double Ut1Jd1 => this.epochParameters.Ut1Jd1;

        public double Ut1Jd2 => this.epochParameters.Ut1Jd2 + this.deltaDays;

        public double TaiJd1 => this.epochParameters.TaiJd1;

        public double TaiJd2 => this.epochParameters.TaiJd2 + this.deltaDays;

        public double TtJd1 => this.epochParameters.TtJd1;

        public double TtJd2 => this.epochParameters.TtJd2 + this.deltaDays;

        public double TdbMinusTt {
            get {
                double ut = Math.IEEERemainder(Math.IEEERemainder(this.Ut1Jd1, 1.0) + Math.IEEERemainder(this.Ut1Jd2, 1.0), 1.0) + 0.5;
                return WWA.wwaDtdb(this.TtJd1, TtJd2, ut, this.epochParameters.Location.Longitude, this.U / 1000, this.V / 1000);
            }
        }

        public double U => this.epochParameters.U;

        public double V => this.epochParameters.V;

        public double Gmst => this.epochParameters.Gmst + this.deltaSeconds * AstroUtil.SiderealSecondsPerSecond * WWA.DS2R;

        public double Lmst => this.epochParameters.Lmst + this.deltaSeconds * AstroUtil.SiderealSecondsPerSecond * WWA.DS2R;
    }
}
