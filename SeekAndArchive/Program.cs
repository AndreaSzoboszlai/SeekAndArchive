using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekAndArchive
{
    class Program
    {
        private static List<FileInfo> FoundFiles;
        private static List<FileSystemWatcher> watchers;
        private static List<DirectoryInfo> archiveDirs;

        static void Main(string[] args)
        {
            string fileName = args[0];
            string directoryName = args[1]; 
            Console.WriteLine(args[0]);
            Console.WriteLine(args[1]);
            Console.ReadLine();
            String fileToSearch = "CreateClass.sln";
            DirectoryInfo di = new DirectoryInfo("C:\\Users\\szobo\\Documents\\.NET Projects");
            DirectoryInfo rootDir = new DirectoryInfo(directoryName);
            if (!rootDir.Exists)
            { 
                {
                    Console.WriteLine("The specified directory does not exist.");
                    return;
                }
            }
            FoundFiles = new List<FileInfo>();
            watchers = new List<FileSystemWatcher>();

            foreach (FileInfo fil in FoundFiles)
            {
                FileSystemWatcher newWatcher = new FileSystemWatcher(fil.DirectoryName, fil.Name);
                newWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                newWatcher.EnableRaisingEvents = true;
                watchers.Add(newWatcher);
            }

            RecursiveSearch(FoundFiles, fileToSearch, rootDir);

            Console.WriteLine("Found {0} files.", FoundFiles.Count);
            foreach (FileInfo file in FoundFiles)
            {
                Console.WriteLine("Searched file: {0}", file.FullName);
            }
            Console.ReadKey();

            archiveDirs = new List<DirectoryInfo>();
            for (int i = 0; i < FoundFiles.Count; i++)
            {
                archiveDirs.Add(Directory.CreateDirectory("archive" + i.ToString()));
            }


            Console.ReadLine();
        }
        public static void RecursiveSearch(List<FileInfo> foundFiles, string fileName, DirectoryInfo currentDirectory)
        {
            foreach (FileInfo file in currentDirectory.GetFiles())
            {
                if (file.Name == fileName)
                {
                    foundFiles.Add(file);
                }
            }

            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                RecursiveSearch(foundFiles, fileName, dir);
            }
        }

        private static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed!", e.FullPath);
                FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
                int index = watchers.IndexOf(senderWatcher, 0);
                ArchiveFile(archiveDirs[index], FoundFiles[index]);
            }
        }

        private static void ArchiveFile(DirectoryInfo archiveDir, FileInfo fileToArchive)
        {
            FileStream input = fileToArchive.OpenRead(); FileStream output = File.Create(archiveDir.FullName + @"" + fileToArchive.Name + ".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte(); while (b != -1)
            {
                Compressor.WriteByte((byte)b);

                b = input.ReadByte();
            }
            Compressor.Close(); input.Close(); output.Close();
        }

        }
    }
