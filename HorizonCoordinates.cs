using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WorldWideAstronomy;

namespace AlignmentTest;

public record HorizonCoordinates {
    public HorizonCoordinates(double altitude, double azimuth) {
        this.Altitude = WWA.wwaAnpm(altitude);
        this.Azimuth = WWA.wwaAnp(azimuth);
    }

    public double Altitude { get; private set; }
    public double Azimuth { get; private set; }

    public override string ToString() {
        return $"Alt {AstroUtil.RadiansToDMS(Altitude)} Az {AstroUtil.RadiansToDMS(Azimuth)}";
    }

    public static HorizonCoordinates FromPolar(PolarCoordinates pc) {
        return new HorizonCoordinates(
            altitude: pc.Phi,
            azimuth: -pc.Theta);
    }

    public PolarCoordinates ToPolar() {
        return new PolarCoordinates(
            phi: Altitude,
            theta: -Azimuth);
    }

    /*
    public RectangularCoordinates ToApparentCoordinates(double latitude) {
        return this.
        .ToDirectionCosine().Rotate_Y(AstroUtil.HALF_PI - latitude);
    }
    */
}