// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.UserInterface.Graph.Interpolation;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph
{
    public readonly struct AnchorState
    {
        public readonly Vector2 Pos;

        public readonly IGraphInterpolator Interpolator;

        public AnchorState(Vector2 pos, IGraphInterpolator interpolator)
        {
            Pos = pos;
            Interpolator = interpolator;
        }
    }
}
