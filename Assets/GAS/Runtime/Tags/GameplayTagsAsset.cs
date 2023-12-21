using System.Collections.Generic;
using GAS.Editor.Tags;
using UnityEditor.TreeDataModel;
using UnityEngine;

namespace GAS.Runtime.Tags
{
	
	[CreateAssetMenu (fileName = "GameplayTagsAsset", menuName = "GAS/GameplayTagsAsset ", order = 1)]
	public class GameplayTagsAsset : ScriptableObject
	{
		[SerializeField] public string GameplayTagSumCollectionGenPath = "Script/Gen/GameplayTagSumCollection.cs";
		
		[SerializeField] List<GameplayTagTreeElement> gameplayTagTreeElements = new();

		internal List<GameplayTagTreeElement> GameplayTagTreeElements => gameplayTagTreeElements;
		
		[SerializeField] public List<GameplayTag> Tags = new();


		public void CacheTags()
		{
			Tags.Clear();
			for (int i = 0; i < gameplayTagTreeElements.Count; i++)
			{
				TreeElement tag = gameplayTagTreeElements[i];
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
