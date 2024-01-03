using System;
using System.Collections.Generic;
using GAS.Runtime.Component;

namespace GAS.Runtime.Tags
{
    public class GameplayTagAggregator
    {
        AbilitySystemComponent _owner;

        private event Action OnTagIsDirty;

        public GameplayTagAggregator(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        public void RegisterOnTagIsDirty(Action tagIsDirty)
        {
            OnTagIsDirty += tagIsDirty;
        }
        
        public void UnregisterOnTagIsDirty(Action tagIsDirty)
        {
            OnTagIsDirty -= tagIsDirty;
        }
        
        public void TagIsDirty(GameplayTagSet tags)
        {
            if (!tags.Empty) OnTagIsDirty?.Invoke();
        }
    }
}