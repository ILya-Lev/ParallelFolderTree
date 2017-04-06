using DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace BusinessLogic
{
	public class FileWriter : IDisposable
	{
		private readonly string _fileName;
		private readonly Tree _root;
		private readonly XmlTextWriter _xmlTextWriter;

		public FileWriter(string fileName, Tree root)
		{
			_fileName = fileName;
			_root = root;

			_xmlTextWriter = new XmlTextWriter(_fileName, Encoding.UTF8)
			{
				Formatting = Formatting.Indented,
				Indentation = 2,
			};
		}

		~FileWriter()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(false);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool fromFinalizer)
		{
			if (fromFinalizer) return;

			_xmlTextWriter.Flush();
			_xmlTextWriter.Close();
			_xmlTextWriter.Dispose();
		}

		private readonly HashSet<Tree> _seenItems = new HashSet<Tree>();

		public void StoreTree()
		{
			var branch = new Stack<Tree>();
			branch.Push(_root);

			while (branch.Count != 0)
			{
				Thread.Sleep(1);
				var current = branch.Peek();
				lock (current)
				{
					while (!current.IsEnumerated)
						Monitor.Wait(current);

					if (_seenItems.Contains(current))
					{
						branch.Pop();
						_xmlTextWriter.WriteEndElement();
						_xmlTextWriter.Flush(); //required to avoid data lossage
						continue;
					}
					WriteToFile(current.Data);
					_seenItems.Add(current);

					foreach (var node in current.Clidren)
					{
						branch.Push(node);
					}
				}
			}
		}

		private void WriteToFile(FileSystemInfo data)
		{
			if (data is FileInfo)
				_xmlTextWriter.WriteStartElement("File");
			else
				_xmlTextWriter.WriteStartElement("Dir");

			_xmlTextWriter.WriteAttributeString("Name", data.Name);
			_xmlTextWriter.WriteAttributeString("Created", data.CreationTime.ToString("F"));
		}
	}
}