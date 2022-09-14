// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation
{
    public interface IInvertibleInterpolator : IGraphInterpolator
    {
        /// <summary>
        /// Calculates all X values that correspond to specified Y value.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        IEnumerable<double> GetInverse(double y);
    }
}
