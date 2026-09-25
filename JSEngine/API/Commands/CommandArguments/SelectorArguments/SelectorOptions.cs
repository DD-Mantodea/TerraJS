using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments
{
    public enum SelectorSort
    {
        None,

        Nearest,

        Furthest,

        Random
    }

    public sealed class SelectorOptions
    {
        public bool LimitSet;

        public int Limit = 1;

        public bool SortSet;

        public SelectorSort Sort = SelectorSort.None;

        public bool OriginSet;

        public Vector2 Origin;

        public bool OriginXSet;

        public bool OriginYSet;

        public float OriginX;

        public float OriginY;

        public bool BoxSet;

        public bool BoxWidthSet;

        public bool BoxHeightSet;

        public Vector2 BoxSize;
    }

    public sealed class SelectorEntry<T>(Func<Selector, List<T>> candidates, SelectorSort sort, int limit)
    {
        public readonly Func<Selector, List<T>> Candidates = candidates;

        public readonly SelectorSort Sort = sort;

        public readonly int Limit = limit;
    }
}
