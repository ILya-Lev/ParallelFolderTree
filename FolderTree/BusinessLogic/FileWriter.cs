using DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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


		public void StoreTree()
		{
			var seenItems = new HashSet<Tree>();
			Func<Tree, bool> onMeet = node => WriteToFile(node, seenItems);
			_root.TraverseTree(onMeet);
		}

		private bool WriteToFile(Tree current, HashSet<Tree> seenItems)
		{
			if (seenItems.Contains(current))
			{
				_xmlTextWriter.WriteEndElement();
				_xmlTextWriter.Flush(); //required to avoid data lossage
				return true;
			}

			FileSystemInfo data = current.Data;
			if (data is FileInfo)
				_xmlTextWriter.WriteStartElement("File");
			else
				_xmlTextWriter.WriteStartElement("Dir");

			_xmlTextWriter.WriteAttributeString("Name", data.Name);
			_xmlTextWriter.WriteAttributeString("Created", data.CreationTime.ToString("F"));
			seenItems.Add(current);
			return false;
		}
	}
}