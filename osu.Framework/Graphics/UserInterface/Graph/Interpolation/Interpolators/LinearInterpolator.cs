// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class LinearInterpolator : InterpolatorBase, IHasName, IDerivableInterpolator, IIntegrableInterpolator
    {
        public string Name => "Linear";

        public override double GetInterpolation(double t)
        {
            return t;
        }

        public double GetDerivative(double t)
        {
            return 1;
        }

        public double GetIntegral(double t1, double t2)
        {
            return 0.5 * t2 * t2 - 0.5 * t1 * t1;
        }
    }
}
