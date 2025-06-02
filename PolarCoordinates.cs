using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignmentTest;

public record PolarCoordinates {
    public PolarCoordinates(double phi, double theta) {
        this.Phi = phi;
        this.Theta = theta;
    }

    public double Phi { get; private set; } // Altitude

    public double Theta { get; private set; } // Azimuth

    public RectangularCoordinates ToDirectionCosine() {
        return new RectangularCoordinates() {
            X = Math.Cos(Phi) * Math.Cos(Theta),
            Y = Math.Cos(Phi) * Math.Sin(Theta),
            Z = Math.Sin(Phi)
        };
    }

    public LocalEquatorialCoordinates AsLocalEquatorial() {
        return LocalEquatorialCoordinates.FromPolar(this);
    }

    public HorizonCoordinates AsHorizon() {
        return HorizonCoordinates.FromPolar(this);
    }
}