#if UNITY_EDITOR
namespace GAS.Editor
{
	using System;
	using UnityEditor.TreeDataModel;
	
	[Serializable]
	public class GameplayTagTreeElement : ExTreeElement
	{
		public GameplayTagTreeElement (string name, int depth, int id) : base (name, depth, id)
		{
		}
	}
}
#endif
