using System;
using System.Collections.Generic;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments;

namespace TerraJS.JSEngine.API.CommandArguments.SelectorArguments
{
    public sealed class SelectorRegistry<Target>
    {
        public static SelectorRegistry<Target> Instance { get; } = new();

        public Dictionary<string, SelectorEntry<Target>> Selectors { get; } = new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> Names => Selectors.Keys;

        public bool Register(string name, SelectorEntry<Target> entry)
        {
            if (string.IsNullOrWhiteSpace(name) || entry is null)
                return false;

            return Selectors.TryAdd(name, entry);
        }

        public bool TryFind(string name, out SelectorEntry<Target> entry) => Selectors.TryGetValue(name ?? string.Empty, out entry);
    }
}
