// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class ParabolaInterpolator : InterpolatorBase, IHasName, IDerivableInterpolator, IIntegrableInterpolator
    {
        public string Name => "Parabola";

        public override double GetInterpolation(double t)
        {
            double p = MathHelper.Clamp(P, -1, 1);
            return -p * Math.Pow(t, 2) + (p + 1) * t;
        }

        public double GetDerivative(double t)
        {
            double p = MathHelper.Clamp(P, -1, 1);
            return -2 * p * t + p + 1;
        }

        public double GetIntegral(double t1, double t2)
        {
            return primitive(t2) - primitive(t1);
        }

        private double primitive(double t)
        {
            double p = MathHelper.Clamp(P, -1, 1);
            return 1d / 3 * -p * Math.Pow(t, 3) + 0.5 * (p + 1) * Math.Pow(t, 2);
        }
    }
}
