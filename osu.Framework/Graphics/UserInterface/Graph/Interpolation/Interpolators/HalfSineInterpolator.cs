// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Utils;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class HalfSineInterpolator : InterpolatorBase, IHasName, IDerivableInterpolator, IIntegrableInterpolator
    {
        private readonly LinearInterpolator linearDegenerate;

        public string Name => "Half sine";

        public HalfSineInterpolator()
        {
            linearDegenerate = new LinearInterpolator();
        }

        public override double GetInterpolation(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetInterpolation(t);
            }

            double p = MathHelper.Clamp(P, -1, 1);
            double b = 2 * Math.Acos(1 / (Math.Sqrt(2) * Math.Abs(p) - Math.Abs(p) + 1));
            return p < 0 ? 1 - f(1 - t, b) : f(t, b);
        }

        private static double f(double t, double k)
        {
            return Math.Sin(t * k) / Math.Sin(k);
        }

        private static double derivative(double t, double k)
        {
            return -(2 * k * Math.Sin(k) * Math.Cos(k * t)) / (Math.Cos(2 * k) - 1);
        }

        private static double primitive(double t, double p)
        {
            double b = 2 * Math.Acos(1 / (Math.Sqrt(2) * Math.Abs(p) - Math.Abs(p) + 1));
            return p > 0 ? (-(1 / Math.Sin(b) * (Math.Cos(b * t) - 1))) / b : 1 / Math.Sin(b) * (Math.Cos(b) - Math.Cos(b - b * t)) / b + t;
        }

        public double GetDerivative(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetDerivative(t);
            }

            double p = MathHelper.Clamp(P, -1, 1);
            double b = 2 * Math.Acos(1 / (Math.Sqrt(2) * Math.Abs(p) - Math.Abs(p) + 1));
            return p < 0 ? derivative(1 - t, b) : derivative(t, b);
        }

        public double GetIntegral(double t1, double t2)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetIntegral(t1, t2);
            }

            double p = MathHelper.Clamp(P, -1, 1);
            return primitive(t2, p) - primitive(t1, p);
        }
    }
}
