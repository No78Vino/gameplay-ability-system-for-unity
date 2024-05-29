
#if UNITY_EDITOR
namespace UnityEditor.TreeDataModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	
	// The TreeModel is a utility class working on a list of serializable TreeElements where the order and the depth of each TreeElement define
	// the tree structure. Note that the TreeModel itself is not serializable (in Unity we are currently limited to serializing lists/arrays) but the 
	// input list is.
	// The tree representation (parent and children references) are then build internally using TreeElementUtility.ListToTree (using depth 
	// values of the elements). 
	// The first element of the input list is required to have depth == -1 (the hiddenroot) and the rest to have
	// depth >= 0 (otherwise an exception will be thrown)

	public class ExTreeModel<T> where T : ExTreeElement
	{
		IList<T> m_Data;
		int m_MaxID;
	
		public T Root { get; private set; }

		public event Action modelChanged;
		public int NumberOfDataElements => m_Data.Count;

		public ExTreeModel (IList<T> data)
		{
			SetData (data);
		}

		public T Find (int id)
		{
			return m_Data.FirstOrDefault (element => element.ID == id);
		}
	
		public void SetData (IList<T> data)
		{
			Init (data);
		}

		void Init (IList<T> data)
		{
			m_Data = data ?? throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");
			if (m_Data.Count > 0)
			{
				Root = ExTreeElementUtility.ListToTree(data);
			}
			else
			{
				T root = Activator.CreateInstance(typeof(T), "Root", -1, 0) as T;
				AddRoot(root);
				Root = root;
			}

			m_MaxID = m_Data.Max(e => e.ID);
		}

		public int GenerateUniqueID ()
		{
			return ++m_MaxID;
		}

		public IList<int> GetAncestors (int id)
		{
			var parents = new List<int>();
			ExTreeElement T = Find(id);
			if (T != null)
			{
				while (T.Parent != null)
				{
					parents.Add(T.Parent.ID);
					T = T.Parent;
				}
			}
			return parents;
		}

		public IList<int> GetDescendantsThatHaveChildren (int id)
		{
			T searchFromThis = Find(id);
			if (searchFromThis != null)
			{
				return GetParentsBelowStackBased(searchFromThis);
			}
			return new List<int>();
		}

		IList<int> GetParentsBelowStackBased(ExTreeElement searchFromThis)
		{
			Stack<ExTreeElement> stack = new Stack<ExTreeElement>();
			stack.Push(searchFromThis);

			var parentsBelow = new List<int>();
			while (stack.Count > 0)
			{
				ExTreeElement current = stack.Pop();
				if (current.HasChildren)
				{
					parentsBelow.Add(current.ID);
					foreach (var T in current.Children)
					{
						stack.Push(T);
					}
				}
			}

			return parentsBelow;
		}

		public void RemoveElements (IList<int> elementIDs)
		{
			IList<T> elements = m_Data.Where (element => elementIDs.Contains (element.ID)).ToArray ();
			RemoveElements (elements);
		}

		public void RemoveElements (IList<T> elements)
		{
			foreach (var element in elements)
				if (element == Root)
					throw new ArgumentException("It is not allowed to remove the root element");
		
			var commonAncestors = ExTreeElementUtility.FindCommonAncestorsWithinList (elements);

			foreach (var element in commonAncestors)
			{
				element.Parent.Children.Remove (element);
				element.Parent = null;
			}

			ExTreeElementUtility.TreeToList(Root, m_Data);

			Changed();
		}

		public void AddElements (IList<T> elements, ExTreeElement parent, int insertPosition)
		{
			if (elements == null)
				throw new ArgumentNullException("elements", "elements is null");
			if (elements.Count == 0)
				throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null");

			if (parent.Children == null)
				parent.Children = new List<ExTreeElement>();

			parent.Children.InsertRange(insertPosition, elements.Cast<ExTreeElement> ());
			foreach (var element in elements)
			{
				element.Parent = parent;
				element.Depth = parent.Depth + 1;
				ExTreeElementUtility.UpdateDepthValues(element);
			}

			ExTreeElementUtility.TreeToList(Root, m_Data);

			Changed();
		}

		public void AddRoot (T root)
		{
			if (root == null)
				throw new ArgumentNullException("root", "root is null");

			if (m_Data == null)
				throw new InvalidOperationException("Internal Error: data list is null");

			if (m_Data.Count != 0)
				throw new InvalidOperationException("AddRoot is only allowed on empty data list");

			root.Name = "Root";
			root.ID = GenerateUniqueID ();
			root.Depth = -1;
			m_Data.Add (root);
		}

		public void AddElement (T element, ExTreeElement parent, int insertPosition)
		{
			if (element == null)
				throw new ArgumentNullException("element", "element is null");
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null");
		
			if (parent.Children == null)
				parent.Children = new List<ExTreeElement> ();

			parent.Children.Insert (insertPosition, element);
			element.Parent = parent;

			ExTreeElementUtility.UpdateDepthValues(parent);
			ExTreeElementUtility.TreeToList(Root, m_Data);

			Changed ();
		}

		public void MoveElements(ExTreeElement parentElement, int insertionIndex, List<ExTreeElement> elements)
		{
			if (insertionIndex < 0)
				throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

			// Invalid reparenting input
			if (parentElement == null)
				return;

			// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
			if (insertionIndex > 0)
				insertionIndex -= parentElement.Children.GetRange(0, insertionIndex).Count(elements.Contains);

			// Remove draggedItems from their parents
			foreach (var draggedItem in elements)
			{
				draggedItem.Parent.Children.Remove(draggedItem);	// remove from old parent
				draggedItem.Parent = parentElement;					// set new parent
			} 

			if (parentElement.Children == null)
				parentElement.Children = new List<ExTreeElement>();

			// Insert dragged items under new parent
			parentElement.Children.InsertRange(insertionIndex, elements);

			ExTreeElementUtility.UpdateDepthValues (Root);
			ExTreeElementUtility.TreeToList (Root, m_Data);

			Changed ();
		}

		void Changed ()
		{
			if (modelChanged != null)
				modelChanged ();
		}
	}

}
#endif
