using System;
using System.Collections.Generic;
using System.IO;

namespace DomainModel
{
	public class Tree
	{
		public FileSystemInfo Data { get; set; }
		public Tree Root { get; set; }
		public List<Tree> Clidren { get; } = new List<Tree>();

		public bool IsEnumerated { get; set; } = false;

		public static Tree TreeFrom(string path)
		{
			if (File.Exists(path))
				return new Tree { Data = new FileInfo(path), IsEnumerated = true };

			if (Directory.Exists(path))
				return new Tree { Data = new DirectoryInfo(path) };

			throw new ArgumentException($"Path [{path}] is not either a file nor a directory - cannot traverse it");
		}
	}
}
