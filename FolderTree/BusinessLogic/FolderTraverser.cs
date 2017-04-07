using DomainModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BusinessLogic
{
	public class FolderTraverser
	{
		private readonly Tree _root;

		public FolderTraverser(Tree root)
		{
			_root = root;
		}

		public void Traverse()
		{
			_root.FillIn(FolderTraversingLayerGenerator);
		}

		private static IEnumerable<Tree> FolderTraversingLayerGenerator(Tree localRoot)
		{
			var filesFirst = (localRoot.Data as DirectoryInfo)?.EnumerateFileSystemInfos()
										.ToList().OrderBy(f => f is FileInfo ? 0 : 1)
									?? Enumerable.Empty<FileSystemInfo>();
			foreach (var info in filesFirst)
			{
				var subNode = new Tree { Root = localRoot.Root, Data = info };
				localRoot.Children.Add(subNode);

				if (info is FileInfo)
					subNode.IsFilled = true;
				else
					yield return subNode;
			}
		}
	}
}
