// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation
{
    public interface IIntegrableInterpolator : IGraphInterpolator
    {
        /// <summary>
        /// Calculates the integral of the interpolator function between t1 and t2.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        double GetIntegral(double t1, double t2);
    }
}
