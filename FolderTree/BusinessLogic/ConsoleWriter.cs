using System;
using System.Collections.Generic;
using DomainModel;

namespace BusinessLogic
{
	public class ConsoleWriter
	{
		private readonly Tree _root;

		public ConsoleWriter(Tree root)
		{
			_root = root;
		}

		public void PrintTree()
		{
			var seenNodes = new HashSet<Tree>();
			Func<Tree, bool> onMeet = node => PrintNode(node, seenNodes);
			_root.TraverseTree(onMeet);
		}

		private bool PrintNode(Tree node, HashSet<Tree> seenNodes)
		{
			if (seenNodes.Contains(node))
				return true;

			seenNodes.Add(node);
			Console.WriteLine(node.Data.FullName);
			return false;
		}
	}
}