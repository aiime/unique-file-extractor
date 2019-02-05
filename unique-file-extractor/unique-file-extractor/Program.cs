using System;
using System.Collections.Generic;
using System.IO;

namespace UniqueFilesExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleSetUTF8();
            List<FileInfo> files = GetFiles(fromFolder: @"C:\From Folder", fileFormat: "mp3");
            ExtractUniqueFiles(files, outputFolder: @"C:\Unique Files");

            Console.ReadKey();
        }

        static void ConsoleSetUTF8()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        static List<FileInfo> GetFiles(string fromFolder, string fileFormat)
        {
            string[] fileNames = Directory.GetFiles(fromFolder, "*." + fileFormat + "*", SearchOption.AllDirectories);

            List<FileInfo> files = new List<FileInfo>(fileNames.Length);

            foreach (string fileName in fileNames)
            {
                files.Add(new FileInfo(fileName));
            }

            return files;
        }

        static void ExtractUniqueFiles(List<FileInfo> files, string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            List<FileInfo> filesChecked = new List<FileInfo>();

            int i = 1;
            foreach (FileInfo file in files)
            {
                bool identicalFound = false;

                foreach (FileInfo fileToCompare in files)
                {
                    if (file.Length == fileToCompare.Length &&
                        file.Name == fileToCompare.Name &&
                        file.FullName != fileToCompare.FullName &&
                        !filesChecked.Contains(fileToCompare))
                    {
                        if (identicalFound == false)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("IDENTICAL #" + i + ":");
                            Console.WriteLine(file.FullName);
                            Console.WriteLine(file.Length);
                            Console.WriteLine();
                            Console.ResetColor();
                            i++;
                            identicalFound = true;
                        }

                        Console.WriteLine("\t" + fileToCompare.FullName);
                        Console.WriteLine("\t" + fileToCompare.Length);
                        Console.WriteLine();

                        filesChecked.Add(fileToCompare);
                    }
                }

                File.Copy(file.FullName, outputFolder + @"\" + file.Name, overwrite: true);

                filesChecked.Add(file);
            }
        }
    }
}
