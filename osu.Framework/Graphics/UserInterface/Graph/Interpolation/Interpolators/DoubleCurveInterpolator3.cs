// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Utils;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class DoubleCurveInterpolator3 : InterpolatorBase, IHasName, IHasCustomDerivativeExtrema, IIntegrableInterpolator
    {
        private readonly LinearInterpolator linearDegenerate;

        public string Name => "Double curve 3";

        public double[] ExtremaPositions => new[] { 0, 0.5, 1 };

        public DoubleCurveInterpolator3()
        {
            linearDegenerate = new LinearInterpolator();
        }

        public override double GetInterpolation(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetInterpolation(t);
            }

            double p = MathHelper.Clamp(P, -1, 1) * 7;
            return t < 0.5 ? 0.5 * f(t * 2, p) : 0.5 + 0.5 * f(t * 2 - 1, -p);
        }

        private static double f(double t, double k)
        {
            return (Math.Exp(k) * t) / ((Math.Exp(k) - 1) * t + 1);
        }

        private static double derivative(double t, double p)
        {
            return Math.Exp(p) / Math.Pow(t * (Math.Exp(p) - 1) + 1, 2);
        }

        private static double primitive(double t, double p)
        {
            return t < 0.5
                ? (-(Math.Exp(p) * (Math.Log(2 * t * (Math.Exp(p) - 1) + 1) - 2 * t * (Math.Exp(p) - 1)))) /
                  (4 * Math.Pow(Math.Exp(p) - 1, 2))
                : (2 * t * (Math.Exp(p) - 2) * (Math.Exp(p) - 1) -
                      Math.Exp(p) * (Math.Log(-Math.Exp(-p) * (2 * t * Math.Exp(p) - 2 * Math.Exp(p) - 2 * t + 1)) -
                                     Math.Exp(p) - 2) - Math.Exp(2 * p) + Math.Exp(p) * -Math.Log(Math.Exp(p)) - 2) /
                  (4 * Math.Pow(Math.Exp(p) - 1, 2));
        }

        public double GetDerivative(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetDerivative(t);
            }

            double p = MathHelper.Clamp(P, -1, 1) * 7;
            return t < 0.5 ? derivative(2 * t, p) : derivative(2 - 2 * t, p);
        }

        public double GetIntegral(double t1, double t2)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetIntegral(t1, t2);
            }

            double p = MathHelper.Clamp(P, -1, 1) * 7;
            return primitive(t2, p) - primitive(t1, p);
        }
    }
}
