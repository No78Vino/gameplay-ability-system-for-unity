using System.Collections.Generic;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag
{
    public class GameplayTagController
    {
        private readonly Entity _asc;
        private static EntityManager GasEntityManager => GASManager.EntityManager;
        private DynamicBuffer<BuffElemFixedTag> DynamicBufferFixedTags => GasEntityManager.GetBuffer<BuffElemFixedTag>(_asc);
        private DynamicBuffer<BuffElemTemporaryTag> DynamicBufferTemporaryTags => GasEntityManager.GetBuffer<BuffElemTemporaryTag>(_asc);

#if UNITY_EDITOR
        public int[] FixedTags()
        {
            int[] tags = new int[DynamicBufferFixedTags.Length];
            for (int i = 0; i < DynamicBufferFixedTags.Length; i++)
                tags[i] = DynamicBufferFixedTags[i].tag;
            return tags;
        }
#endif
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
                if (GameplayTagHub.HasTag(t.tag, tag))
                    return true;
            return false;
        }

        private bool HasTemporaryTag(int tag)
        {
            var temporaryTags = DynamicBufferTemporaryTags;
            foreach (var t in temporaryTags)
                if (GameplayTagHub.HasTag(t.tag, tag))
                    return true;
            return false;
        }

        private void KillFixedTag(int tag)
        {
            var fixedTags = DynamicBufferFixedTags;
            int index = -1;
            for (int i = 0; i < fixedTags.Length; i++)
            {
                if (fixedTags[i].tag != tag) continue;
                index = i;
                break;
            }
            if(index>=0) fixedTags.RemoveAt(index);
        }

        private void KillTemporaryTag(int tag)
        {
            var tempTags = DynamicBufferTemporaryTags;
            List<int> removedIndexList = new List<int>();
            for (int i = 0; i < tempTags.Length; i++)
            {
                if (tempTags[i].tag == tag)
                {
                    removedIndexList.Add(i);
                }
            }

            int offset = 0;
            foreach (var index in removedIndexList)
            {
                tempTags.RemoveAt(index-offset);
                offset++;
            }
        }
        
        public bool HasTag(int tag)
        {
            bool hasFixedTag = HasFixedTag(tag);
            if (hasFixedTag) return true;

            bool hasTemporary = HasTemporaryTag(tag);
            return hasTemporary;
        }


        /// <summary>
        /// 固有Tag添加逻辑：固有tag已经存在则不添加；若临时tag中存在，则在固有tag中添加完后，还要移除临时tag中的这个tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>dirty：tag合集是否产生变化</returns>
        private bool AddFixedTag(int tag)
        {
            if (HasFixedTag(tag)) return false;
            
            DynamicBufferFixedTags.Add(new BuffElemFixedTag { tag = tag });
            bool containTemporary = HasTemporaryTag(tag);
            // 从临时tag中剔除
            if (containTemporary) KillTemporaryTag(tag);
            return !containTemporary;
        }

        public bool AddFixedTags(int[] tags)
        {
            bool dirty = false;
            foreach (var tag in tags)
                dirty |= AddFixedTag(tag);
            return dirty;
        }

        public void RemoveFixedTag(int tag)
        {
            
        }

        public void RemoveTags(int[] tags)
        {
            
        }
    }
}