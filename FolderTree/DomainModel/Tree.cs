using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DomainModel
{
	public class Tree
	{
		public FileSystemInfo Data { get; set; }
		public Tree Root { get; set; }
		public List<Tree> Children { get; } = new List<Tree>();

		public bool IsFilled { get; set; } = false;

		public static Tree TreeFrom(string path)
		{
			Tree root = null;
			if (File.Exists(path))
				root = new Tree { Data = new FileInfo(path), IsFilled = true };

			if (Directory.Exists(path))
				root = new Tree { Data = new DirectoryInfo(path) };

			if (root != null)
			{
				root.Root = root;
				return root;
			}

			throw new ArgumentException($"Path [{path}] is not either a file nor a directory - cannot traverse it");
		}

		public void FillIn(Func<Tree, IEnumerable<Tree>> layerGenerator)
		{
			var layer = new Queue<Tree>();
			layer.Enqueue(Root);

			while (layer.Count != 0)
			{
				Thread.Sleep(20);
				var current = layer.Dequeue();
				lock (current)
				{
					current.IsFilled = true;

					var nextLayer = layerGenerator(current).ToList();
					foreach (var node in nextLayer)//.Where(n => !n.IsFilled))
					{
						layer.Enqueue(node);
					}

					Monitor.PulseAll(current);
				}
			}
		}

		public void TraverseTree(Func<Tree, bool> onMeet)
		{
			var path = new Stack<Tree>();
			path.Push(Root);

			while (path.Count != 0)
			{
				Thread.Sleep(20);
				var current = path.Peek();
				lock (current)
				{
					while (!current.IsFilled)
						Monitor.Wait(current);

					if (onMeet.Invoke(current))
					{
						path.Pop();
						continue;
					}

					foreach (var child in current.Children)
						path.Push(child);
				}
			}
		}
	}
}
