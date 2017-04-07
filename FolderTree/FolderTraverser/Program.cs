using BusinessLogic;
using DomainModel;
using System.Threading;

namespace FolderTraverser
{
	class Program
	{
		static void Main(string[] args)
		{
			var rootPath = @".\tree";
			var outputFilePath = @".\output\tree.xml";

			var root = Tree.TreeFrom(rootPath);

			var traverser = new BusinessLogic.FolderTraverser(root);
			var fileWriter = new FileWriter(outputFilePath, root);
			var consoleWriter = new ConsoleWriter(root);

			var traverserThread = new Thread(traverser.Traverse);
			var consoleWriterThread = new Thread(consoleWriter.PrintTree);
			var writerThread = new Thread(fileWriter.StoreTree);

			writerThread.Start();
			consoleWriterThread.Start();

			Thread.Sleep(1000);

			traverserThread.Start();

			traverserThread.Join();
			writerThread.Join();
			consoleWriterThread.Join();
		}
	}
}
