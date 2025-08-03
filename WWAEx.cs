using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WorldWideAstronomy.WWA;

namespace WorldWideAstronomy {
    public static class WWAEx {
        /// <summary>
        /// For a terrestrial observer, prepare star-independent astrometry
        /// parameters for transformations between CIRS and observed
        /// coordinates.  The caller supplies the Earth orientation information
        /// and the refraction constants as well as the site coordinates.
        /// </summary>
        /// 
        /// <remarks>
        /// World Wide Astronomy - WWA
        /// Set of C# algorithms and procedures that implement standard models used in fundamental astronomy.
        /// 
        /// This program is derived from the International Astronomical Union's
        /// SOFA (Standards of Fundamental Astronomy) software collection.
        /// http://www.iausofa.org
        /// 
        /// The WWA code does not itself constitute software provided by and/or endorsed by SOFA.
        /// This version is intended to retain identical functionality to the SOFA library, but
        /// made distinct through different function names (prefixes) and C# language specific
        /// modifications in code.
        /// 
        /// Contributor
        /// Attila Abrudán
        /// 
        /// Please read the ReadMe.1st text file for more information.
        /// </remarks>
        /// <param name="rc"></param>
        /// <param name="dc"></param>
        /// <param name="pr"></param>
        /// <param name="pd"></param>
        /// <param name="px"></param>
        /// <param name="rv"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="ra"></param>
        /// <param name="da"></param>
        public static void wwaAtcc13(double rc, double dc,
               double pr, double pd, double px, double rv,
               double date1, double date2,
               ref double ra, ref double da) {
            /* Star-independent astrometry parameters */
            WWA.wwaASTROM astrom = new WWA.wwaASTROM();

            double w = 0;

            /* The transformation parameters. */
            WWA.wwaApci13(date1, date2, ref astrom, ref w);

            /* Catalog ICRS (epoch J2000.0) to astrometric. */
            WWAEx.wwaAtccq(rc, dc, pr, pd, px, rv, ref astrom, ref ra, ref da);

            /* Finished. */
        }
        /// <summary>
        /// For a geocentric observer, prepare star-independent astrometry
        /// parameters for transformations between ICRS and GCRS coordinates.
        /// The caller supplies the date, and SOFA models are used to predict
        /// the Earth ephemeris.
        /// The parameters produced by this function are required in the
        /// parallax, light deflection and aberration parts of the astrometric
        /// transformation chain.
        /// </summary>
        /// 
        /// <remarks>
        /// World Wide Astronomy - WWA
        /// Set of C# algorithms and procedures that implement standard models used in fundamental astronomy.
        /// 
        /// This program is derived from the International Astronomical Union's
        /// SOFA (Standards of Fundamental Astronomy) software collection.
        /// http://www.iausofa.org
        /// 
        /// The WWA code does not itself constitute software provided by and/or endorsed by SOFA.
        /// This version is intended to retain identical functionality to the SOFA library, but
        /// made distinct through different function names (prefixes) and C# language specific
        /// modifications in code.
        /// 
        /// Contributor
        /// Attila Abrudán
        /// 
        /// Please read the ReadMe.1st text file for more information.
        /// </remarks>
        /// <param name="rc"></param>
        /// <param name="dc"></param>
        /// <param name="pr"></param>
        /// <param name="pd"></param>
        /// <param name="px"></param>
        /// <param name="rv"></param>
        /// <param name="astrom"></param>
        /// <param name="ra"></param>
        /// <param name="da"></param>
        public static void wwaAtccq(double rc, double dc,
              double pr, double pd, double px, double rv,
              ref wwaASTROM astrom, ref double ra, ref double da) {
            double[] p = new double[3];
            double w = 0;

            /* Proper motion and parallax, giving BCRS coordinate direction. */
            wwaPmpx(rc, dc, pr, pd, px, rv, astrom.pmt, astrom.eb, p);

            /* ICRS astrometric RA,Dec. */
            wwaC2s(p, ref w, ref da);
            ra = wwaAnp(w);

            /* Finished. */
        }
    }
}
