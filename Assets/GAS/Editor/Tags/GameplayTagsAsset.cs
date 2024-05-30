#if UNITY_EDITOR
namespace GAS.Editor
{
	using GAS.Runtime;
	using System.Collections.Generic;
	using UnityEditor.TreeDataModel;
	using UnityEngine;
	
	[FilePath(GasDefine.GAS_TAGS_MANAGER_ASSET_PATH)]
	public class GameplayTagsAsset : ScriptableSingleton<GameplayTagsAsset>
	{
		[SerializeField] List<GameplayTagTreeElement> gameplayTagTreeElements = new List<GameplayTagTreeElement>();

		internal List<GameplayTagTreeElement> GameplayTagTreeElements => gameplayTagTreeElements;
		
		[SerializeField] public List<GameplayTag> Tags = new List<GameplayTag>();


		public void CacheTags()
		{
			Tags.Clear();
			for (int i = 0; i < gameplayTagTreeElements.Count; i++)
			{
				ExTreeElement tag = gameplayTagTreeElements[i];
				if (tag.Depth == -1) continue;
				string tagName = tag.Name;
				while (tag.Parent.Depth >=0)
				{
					tagName = tag.Parent.Name + "." + tagName;
					tag = tag.Parent;
				}

				Tags.Add(new GameplayTag(tagName));
			}
		}
	}
}
#endif
