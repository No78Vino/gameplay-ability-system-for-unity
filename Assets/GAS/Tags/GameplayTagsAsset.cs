using System;
using System.Collections.Generic;
using GAS.Runtime.Tags;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Editor.Tags
{
	
	[CreateAssetMenu (fileName = "GameplayTagsAsset", menuName = "GAS/GameplayTagsAsset ", order = 1)]
	public class GameplayTagsAsset : ScriptableObject
	{
		[SerializeField] List<GameplayTagTreeElement> _gameplayTagTreeElements = new();

		internal List<GameplayTagTreeElement> TreeElements => _gameplayTagTreeElements;
		
		[SerializeField] public List<GameplayTag> Tags = new();
	}
}
