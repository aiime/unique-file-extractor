using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniqueFilesExtractor
{
    class Program
    {
        const string INPUT_FOLDER = "input_folder";
        const string OUTPUT_FOLDER = "output_folder";
        const string FILE_FORMAT = "file_format";

        static void Main(string[] args)
        {
            ConsoleSetUTF8();

            Config config = new Config();
            List<FileInfo> files = GetFiles(fromFolder: config.FindConfigValue(INPUT_FOLDER), 
                                            fileFormat: config.FindConfigValue(FILE_FORMAT));
            ExtractUniqueFiles(files, outputFolder: config.FindConfigValue(OUTPUT_FOLDER));

            Console.ReadKey();
        }

        static void ConsoleSetUTF8()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        static List<FileInfo> GetFiles(string fromFolder, string fileFormat)
        {
            if(!Directory.Exists(fromFolder))
            {
                Console.WriteLine("Input folder doesn't exist.");
                return new List<FileInfo>();
            }

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
            if (files.Capacity == 0)
            {
                Console.WriteLine("There are no files.");
                return;
            }

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
                            Console.WriteLine("[original]");
                            Console.WriteLine("Name: " + file.Name);
                            Console.WriteLine("Path: " + file.FullName);
                            Console.WriteLine("Size: " + file.Length);
                            Console.WriteLine();
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

                if (File.Exists(outputFolder + @"\" + file.Name))
                {
                    int copyIndex = 1;
                    string newPathWithCopyIndex = 
                        String.Format(@"{0}\{1}({2}){3}", outputFolder, Path.GetFileNameWithoutExtension(file.FullName), copyIndex, file.Extension);
                    while (File.Exists(newPathWithCopyIndex) == true)
                    {
                        copyIndex++;
                        newPathWithCopyIndex = 
                            String.Format(@"{0}\{1}({2}){3}", outputFolder, Path.GetFileNameWithoutExtension(file.FullName), copyIndex, file.Extension);
                    }
                    File.Copy(file.FullName, newPathWithCopyIndex);
                }
                else
                {
                    File.Copy(file.FullName, outputFolder + @"\" + file.Name);
                }            
                filesChecked.Add(file);
            }
        }
    }
}
