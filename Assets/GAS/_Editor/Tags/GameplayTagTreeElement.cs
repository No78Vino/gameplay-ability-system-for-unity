using System;
using UnityEditor.TreeDataModel;

namespace GAS.Editor.Tags
{

	[Serializable]
	internal class GameplayTagTreeElement : TreeElement
	{
		public GameplayTagTreeElement (string name, int depth, int id) : base (name, depth, id)
		{
		}
	}
}
