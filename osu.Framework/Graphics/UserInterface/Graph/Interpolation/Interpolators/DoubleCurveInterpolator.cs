// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Utils;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class DoubleCurveInterpolator : InterpolatorBase, IHasName, IHasCustomDerivativeExtrema, IIntegrableInterpolator
    {
        private readonly LinearInterpolator linearDegenerate;

        public string Name => "Double curve";

        public double[] ExtremaPositions => new[] { 0, 0.5, 1 };

        public DoubleCurveInterpolator()
        {
            linearDegenerate = new LinearInterpolator();
        }

        public override double GetInterpolation(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetInterpolation(t);
            }

            double p = -MathHelper.Clamp(P, -1, 1) * 10;
            return t < 0.5 ? 0.5 * f(t * 2, p) : 0.5 + 0.5 * f(t * 2 - 1, -p);
        }

        private static double f(double t, double p)
        {
            return (Math.Exp(p * t) - 1) / (Math.Exp(p) - 1);
        }

        private static double derivative(double t, double p)
        {
            return Math.Exp(p * t) * p / (Math.Exp(p) - 1);
        }

        private static double primitive(double t, double p)
        {
            return t < 0.5
                ? (2 * p * t - Math.Exp(2 * p * t)) / (4 * p - 4 * Math.Exp(p) * p)
                : (2 * p * ((2 * Math.Exp(p) - 1) * t - Math.Exp(p)) + Math.Exp(p * (2 - 2 * t))) /
                  (4 * (Math.Exp(p) - 1) * p);
        }

        public double GetDerivative(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetDerivative(t);
            }

            double p = -MathHelper.Clamp(P, -1, 1) * 10;
            return t < 0.5 ? derivative(2 * t, p) : derivative(2 - 2 * t, p);
        }

        public double GetIntegral(double t1, double t2)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetIntegral(t1, t2);
            }

            double p = -MathHelper.Clamp(P, -1, 1) * 10;
            return primitive(t2, p) - primitive(t1, p);
        }
    }
}
