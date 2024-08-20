using System.Collections.Generic;
using System.Linq;

namespace GAS.Runtime
{
    public static class TagHelper
    {
        public static GameplayTag[] Sort(IEnumerable<GameplayTag> tags) => tags.OrderBy(tag => tag.Name).ToArray();
    }
}