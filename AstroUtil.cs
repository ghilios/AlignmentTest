namespace AlignmentTest {

    public class AstroUtil {

        private const double DegreeToRadiansFactor = Math.PI / 180d;
        private const double RadiansToDegreeFactor = 180d / Math.PI;
        private const double RadianstoHourFactor = 12d / Math.PI;
        private const double DaysToSecondsFactor = 60d * 60d * 24d;
        private const double SecondsToDaysFactor = 1.0 / (60d * 60d * 24d);
        public const double TWO_PI = 2d * Math.PI;
        public const double HALF_PI = Math.PI / 2d;

        public static double ToRadians(double val) {
            return DegreeToRadiansFactor * val;
        }

        public static double ToDegree(double angle) {
            return angle * RadiansToDegreeFactor;
        }

        public static double RadianToHour(double radian) {
            return radian * RadianstoHourFactor;
        }

        public static double DegreeToArcmin(double degree) {
            return degree * 60d;
        }

        public static double DegreeToArcsec(double degree) {
            return degree * 60d * 60d;
        }

        public static double ArcminToArcsec(double arcmin) {
            return arcmin * 60d;
        }

        public static double ArcminToDegree(double arcmin) {
            return arcmin / 60d;
        }

        public static double ArcsecToArcmin(double arcsec) {
            return arcsec / 60d;
        }

        public static double ArcsecToDegree(double arcsec) {
            return arcsec / 60d / 60d;
        }

        public static double HoursToDegrees(double hours) {
            return hours * 15d;
        }

        public static double DegreesToHours(double deg) {
            return deg / 15d;
        }
        public static string DegreesToDMS(double value) {
            return DegreesToDMS(value, "{0:00}° {1:00}' {2:00}\"");
        }

        public static string RadiansToDMS(double value) {
            return DegreesToDMS(ToDegree(value));
        }

        public static double ClampPositiveRadians(double value) {
            var clamped = value % TWO_PI;
            if (clamped < 0d) {
                clamped += TWO_PI;
            }
            return clamped;
        }

        public static double ClampHalfPositiveRadians(double value) {
            var clamped = ClampPositiveRadians(value);
            if (clamped > Math.PI) {
                clamped -= TWO_PI;
            }
            return clamped;
        }

        private static string DegreesToDMS(double value, string pattern) {
            bool negative = false;
            if (value < 0) {
                negative = true;
                value = -value;
            }
            if (negative) {
                pattern = "-" + pattern;
            }

            var degree = Math.Floor(value);
            var arcmin = Math.Floor(DegreeToArcmin(value - degree));
            var arcminDeg = ArcminToDegree(arcmin);

            var arcsec = Math.Floor(DegreeToArcsec(value - degree - arcminDeg));
            if (arcsec == 60) {
                /* If arcsec got rounded to 60 add to arcmin instead */
                arcsec = 0;
                arcmin += 1;

                if (arcmin == 60) {
                    /* If arcmin got rounded to 60 add to degree instead */
                    arcmin = 0;
                    degree += 1;
                }
            }
            var arcms = (value - degree - arcmin / 60.0d - arcsec / 3600.0d) * 3600000.0d;

            // Prevent "-0" when using ToString
            if (arcms == 0) { arcms = 0; }
            if (arcsec == 0) { arcsec = 0; }
            if (arcmin == 0) { arcmin = 0; }
            if (degree == 0) { degree = 0; }

            return string.Format(pattern, degree, arcmin, arcsec, arcms);
        }

        public static string DegreesToHMS(double deg) {
            return DegreesToDMS(DegreesToHours(deg), "{0:00}:{1:00}:{2:00}.{3:000}");
        }
        
        public static string RadiansToHMS(double rad) {
            return DegreesToHMS(ToDegree(rad));
        }

        public static double SecondsToDays(double seconds) {
            return seconds * SecondsToDaysFactor;
        }

        public static double DaysToSeconds(double days) {
            return days * DaysToSecondsFactor;
        }

        public static float EuclidianModulus(float x, float y) {
            return (float)EuclidianModulus((double)x, (double)y);
        }

        public static double EuclidianModulus(double x, double y) {
            if (y > 0) {
                double r = x % y;
                if (r < 0) {
                    return r + y;
                } else {
                    return r;
                }
            } else if (y < 0) {
                return -1 * EuclidianModulus(-1 * x, -1 * y);
            } else {
                return double.NaN;
            }
        }
    }
}