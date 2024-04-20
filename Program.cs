using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace lab7
{
    public class FileSystemEntity
    {
        public string Name { get; set; }
        public long Size { get; set; }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: lab7 <directory>");
                return;
            }

            var directory = new DirectoryInfo(Path.GetFullPath(args[0]));
            if (!directory.Exists)
            {
                Console.Error.WriteLine("Directory does not exist: {0}", directory.FullName);
                return;
            }

            PrintDirectoryFiles(directory);

            var files = directory.GetFiles().Select(f =>
                new FileSystemEntity { Name = f.Name, Size = f.Length }).ToArray();
            var directories = directory.GetDirectories().Select(d => {
                var total = d.GetFiles().Length + d.GetDirectories().Length;
                return new FileSystemEntity { Name = d.Name, Size = total };
            }).ToArray();
            
            var collection = files.Concat(directories)
                .OrderBy(o => o.Name.Length)
                .ThenBy(o => o.Name)
                .ToDictionary(o => o.Name, o => o.Size);

            Console.WriteLine();
            Console.WriteLine($"The eldest file: {GetEldestFile(directory).CreationTime}");
            Console.WriteLine();
            
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, collection);
                ms.Position = 0;
                
                var deserialized = (Dictionary<string, long>)bf.Deserialize(ms);
                foreach (var item in deserialized)
                {
                    Console.WriteLine($"{item.Key} -> {item.Value}");
                }
            }
        }

        private static void PrintDirectoryFiles(DirectoryInfo directory, string prefix = "")
        {
            Console.WriteLine($"{prefix}{directory.Name} ({directory.GetDirectories().Length + directory.GetFiles().Length})");            
            foreach (var subdirectory in directory.GetDirectories())
            {
                PrintDirectoryFiles(subdirectory, prefix + "\t");
            }
            foreach (var file in directory.GetFiles())
            {
                Console.WriteLine($"{prefix}\t{file.Name}\t{GetDosAttributes(file)}\t{file.Length} bytes");
            }
        }

        public static string GetDosAttributes(FileSystemInfo fileSystemInfo)
        {   
            var attributes = fileSystemInfo.Attributes;
            var result = new StringBuilder();
            result.Append((attributes & FileAttributes.ReadOnly) != 0 ? 'R' : '-');
            result.Append((attributes & FileAttributes.Archive) != 0 ? 'A' : '-');
            result.Append((attributes & FileAttributes.Hidden) != 0 ? 'H' : '-');
            result.Append((attributes & FileAttributes.System) != 0 ? 'S' : '-');
            return result.ToString();
        }
        
        public static FileInfo GetEldestFile(DirectoryInfo directory)
        {
            var eldestFiles = directory.GetDirectories()
                .Select(x => GetEldestFile(x))
                .ToList();

            var eldestFileInCurrentDirectory = directory.GetFiles()
                .OrderBy(x => x.CreationTime)
                .FirstOrDefault();

            if (eldestFileInCurrentDirectory != null)
            {
                eldestFiles.Add(eldestFileInCurrentDirectory);
            }

            return eldestFiles.OrderBy(x => x.CreationTime).FirstOrDefault();
        }
    }
}