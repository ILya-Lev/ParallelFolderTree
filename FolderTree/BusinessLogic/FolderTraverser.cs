using DomainModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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
			var layer = new Queue<Tree>();
			layer.Enqueue(_root);

			while (layer.Count != 0)
			{
				Thread.Sleep(100);
				var current = layer.Dequeue();
				lock (current)
				{
					current.IsEnumerated = true;

					var filesFirst = (current.Data as DirectoryInfo)?.EnumerateFileSystemInfos()
										.ToList().OrderBy(f => f is FileInfo ? 0 : 1)
									?? Enumerable.Empty<FileSystemInfo>();
					foreach (var info in filesFirst)
					{
						var subNode = new Tree { Root = current.Root, Data = info };
						current.Clidren.Add(subNode);

						if (info is FileInfo)
							subNode.IsEnumerated = true;
						else
							layer.Enqueue(subNode);
					}

					Monitor.PulseAll(current);
				}
			}
		}
	}
}
