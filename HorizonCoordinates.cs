using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AlignmentTest;

public record HorizonCoordinates {
    public HorizonCoordinates(double altitude, double azimuth) {
        this.Altitude = AstroUtil.ClampPositiveRadians(altitude);
        this.Azimuth = AstroUtil.ClampHalfPositiveRadians(azimuth);
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
}