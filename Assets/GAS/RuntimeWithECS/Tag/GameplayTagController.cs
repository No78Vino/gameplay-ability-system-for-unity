using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag
{
    public class GameplayTagController
    {
        private readonly Entity _asc;
        private static EntityManager GasEntityManager => GASManager.EntityManager;
        private DynamicBuffer<BuffElemFixedTag> DynamicBufferFixedTags => GasEntityManager.GetBuffer<BuffElemFixedTag>(_asc);
        private DynamicBuffer<BuffElemTemporaryTag> DynamicBufferTemporaryTags => GasEntityManager.GetBuffer<BuffElemTemporaryTag>(_asc);
        public GameplayTagController(Entity asc)
        {
            _asc = asc;
            GasEntityManager.AddBuffer<BuffElemFixedTag>(_asc);
            GasEntityManager.AddBuffer<BuffElemTemporaryTag>(_asc);
        }

        private bool HasFixedTag(int tag)
        {
            var fixedTags = DynamicBufferFixedTags;
            foreach (var t in fixedTags)
                if (t.tag == tag)
                    return true;
            return false;
        }
        
        private bool HasTemporaryTag(int tag)
        {
            var temporaryTags = DynamicBufferTemporaryTags;
            foreach (var t in temporaryTags)
                if (t.tag == tag)
                    return true;
            return false;
        }
        
        public bool HasTag(int tag)
        {
            bool hasFixedTag = HasFixedTag(tag);
            if (hasFixedTag) return true;

            bool hasTemporary = HasTemporaryTag(tag);
            return hasTemporary;
        }
        
        public void AddFixedTag(int tag)
        {
            bool contain = HasFixedTag(tag);
            if(!contain)
                DynamicBufferFixedTags.Add(new BuffElemFixedTag { tag = tag });
            
            //
        }

        public void AddFixedTags(int[] tags)
        {
            foreach (var tag in tags)
                AddFixedTag(tag);
        }

        public void RemoveFixedTag(int tag)
        {
            
        }

        public void RemoveTags(int[] tags)
        {
            
        }
    }
}