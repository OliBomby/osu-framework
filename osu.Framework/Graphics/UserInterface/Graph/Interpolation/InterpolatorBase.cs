// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation
{
    public abstract class InterpolatorBase : IParameterizedGraphInterpolator
    {
        public double P { get; set; } = 0;

        public abstract double GetInterpolation(double t);
    }
}
