using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            foreach (FileInfo file in files)
            {
                if (filesChecked.Contains(file)) continue;

                bool identicalFound = false;
                byte[] fileBytes = File.ReadAllBytes(file.FullName);

                foreach (FileInfo fileToCompare in files)
                {
                    byte[] fileToCompareBytes = File.ReadAllBytes(fileToCompare.FullName);

                    if (fileBytes.SequenceEqual(fileToCompareBytes) && !filesChecked.Contains(fileToCompare))
                    {
                        if (identicalFound == false)
                        {
                            //Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("[original]");
                            Console.WriteLine("Name: " + file.Name);
                            Console.WriteLine("Path: " + file.FullName);
                            Console.WriteLine("Size: " + file.Length);
                            Console.WriteLine();
                            //Console.ResetColor();
                            identicalFound = true;
                        }
                        else
                        {
                            Console.WriteLine("\t" + "[duplicate]");
                            Console.WriteLine("\t" + "Name: " + fileToCompare.Name);
                            Console.WriteLine("\t" + "Path: " + fileToCompare.FullName);
                            Console.WriteLine("\t" + "Size: " + fileToCompare.Length);
                            Console.WriteLine();
                        }

                        filesChecked.Add(fileToCompare);
                    }
                }

                File.Copy(file.FullName, outputFolder + @"\" + file.Name, overwrite: true);
                filesChecked.Add(file);
            }
        }
    }
}
