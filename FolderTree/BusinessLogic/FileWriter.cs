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

		private readonly HashSet<Tree> _seenItems = new HashSet<Tree>();

		public void StoreTree()
		{
			_root.TraverseTree(OnMeetNode);
		}

		private bool OnMeetNode(Tree current)
		{
			if (_seenItems.Contains(current))
			{
				_xmlTextWriter.WriteEndElement();
				_xmlTextWriter.Flush(); //required to avoid data lossage
				return true;
			}

			WriteToFile(current.Data);
			_seenItems.Add(current);
			return false;
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