using WorldWideAstronomy;
using static WorldWideAstronomy.WWA;

namespace AlignmentTest {
    public class EpochPositionParameters {
        private WWA.wwaASTROM astromParameters;
        private ITimeParameters timeParameters;
        private double equationOfOrigins = double.NaN;

        public EpochPositionParameters(LocationParameters location, HardwareParameters hardware, AlignmentParameters alignment, EpochTimeParameters time) {
            this.astromParameters = default(wwaASTROM);
            this.astromParameters.eb = new double[3];
            this.astromParameters.eh = new double[3];
            this.astromParameters.bpn = new double[3, 3];

            /* Star-independent astrometry parameters for CIRS->observed. */
            double xp = 0;
            double yp = 0;
            double tsl = 273.15; // Temperature at sea level. Taken from Astrophysical Quantities, C.W.Allen, 3rd edition, section 52
            // Formula from SOFA manual
            // phpa = 1013.25 * exp ( −hm / ( 29.3 * tsl ) )
            double phpa = 1013.25 * Math.Exp(-location.Elevation / (29.3 * tsl));
            double tc = 18.5; // Default ambient temperature at the observer, C
            double rh = 0.83; // Default relative humidity at the observer (0-1)
            double wl = 0.55; // Wavelength of light being refracted, in microns. This defaults to somewhere in the green spectrum
            var result = WWA.wwaApco13(
                time.UtcJd1,
                time.UtcJd2, 
                time.Dut,
                location.Longitude,
                location.Latitude,
                location.Elevation,
                xp, yp, 
                phpa, tc, rh, 
                wl, 
                ref this.astromParameters,
                ref this.equationOfOrigins);
            this.timeParameters = time;
        }

        public void UpdateAt(ITimeParameters time) {
            WWA.wwaAper13(
                time.Ut1Jd1,
                time.Ut1Jd2,
                ref this.astromParameters);
            this.timeParameters = time;
        }

        public EquatorialCoordinates ObservedToICRS(HorizonCoordinates observed) {
            double zenithDistance = (WWA.DPI / 2.0) - observed.Altitude;
            char typeCharH = 'A';

            double ri = double.NaN;
            double di = double.NaN;
            WWA.wwaAtoiq(ref typeCharH, observed.Azimuth, zenithDistance, ref this.astromParameters, ref ri, ref di);

            double ra = double.NaN;
            double dec = double.NaN;
            WWA.wwaAticq(ri, di, ref this.astromParameters, ref ra, ref dec);
            return new EquatorialCoordinates(ra, dec);
        }

        public HorizonCoordinates ICRSToObserved(EquatorialCoordinates equatorial) {
            double azimuth = double.NaN;
            double zenithDistance = double.NaN;
            double hob = double.NaN;
            double rob = double.NaN;
            double dob = double.NaN;

            // Annual proper motion: RA / Dec derivatives, epoch J2000.0
            double pr = Math.Atan2(-354.45e-3 * WWA.DAS2R, Math.Cos(equatorial.Declination));
            double pd = 595.35e-3 * WWA.DAS2R;

            // Parallax (arcsec) and recession speed (km/s).
            double px = 164.99e-3;
            double rv = 0.0;

            double ri = double.NaN;
            double di = double.NaN;
            WWA.wwaAtciq(equatorial.RightAscension, equatorial.Declination, pr, pd, px, rv, ref this.astromParameters, ref ri, ref di);
            WWA.wwaAtioq(ri, di, ref this.astromParameters, ref azimuth, ref zenithDistance, ref hob, ref dob, ref rob);
            double altitude = (WWA.DPI / 2.0) - zenithDistance;
            return new HorizonCoordinates(altitude, azimuth);
        }
    }
}
