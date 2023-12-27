using System.Collections.Generic;
using GAS.Runtime.Component;

namespace GAS.Runtime.Tags
{
    public class GameplayTagAggregator
    {
        AbilitySystemComponent _owner;

        public delegate void tagIsDirty();

        private event tagIsDirty OnTagIsDirty;

        public GameplayTagAggregator(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        public void RegisterOnTagIsDirty(tagIsDirty tagIsDirty)
        {
            OnTagIsDirty += tagIsDirty;
        }
        
        public void UnregisterOnTagIsDirty(tagIsDirty tagIsDirty)
        {
            OnTagIsDirty -= tagIsDirty;
        }
        
        public void TagIsDirty(GameplayTagSet tags)
        {
            if (!tags.Empty) OnTagIsDirty?.Invoke();
        }
    }
}