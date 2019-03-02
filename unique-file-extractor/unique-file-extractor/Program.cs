using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace UniqueFilesExtractor
{
    class Program
    {
        const string INPUT_FOLDER = "input_folder";
        const string OUTPUT_FOLDER = "output_folder";
        const string COMPARISON_TYPE = "comparison_type";
        const string FILE_FORMAT_SPECIFIED = "file_format_specified";
        const string FILE_FORMAT = "file_format";

        static void Main(string[] args)
        {
            ConsoleSetUTF8();

            Config config = new Config();

            List<FileInfo> files = new List<FileInfo>();
            if (config.FindConfigValue(FILE_FORMAT_SPECIFIED) == "yes")
            {
                files = GetFiles(fromFolder: config.FindConfigValue(INPUT_FOLDER),
                                 fileFormat: config.FindConfigValue(FILE_FORMAT));
            }
            else
            {
                files = GetFiles(fromFolder: config.FindConfigValue(INPUT_FOLDER));
            }

            if (config.FindConfigValue(COMPARISON_TYPE) == "bytes")
            {
                ExtractUniqueFiles(files, outputFolder: config.FindConfigValue(OUTPUT_FOLDER));
            }
            else
            {  
                ExtractUniqueFiles(CalculateHashCodes(files), outputFolder: config.FindConfigValue(OUTPUT_FOLDER));
            }

            Console.WriteLine("End");
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

            string[] fileNames = Directory.GetFiles(fromFolder, "*." + fileFormat, SearchOption.AllDirectories);
            List<FileInfo> files = new List<FileInfo>(fileNames.Length);
            foreach (string fileName in fileNames)
            {
                files.Add(new FileInfo(fileName));
            }
            return files;
        }

        static List<FileInfo> GetFiles(string fromFolder)
        {
            return GetFiles(fromFolder, "*");
        }

        static Dictionary<byte[], FileInfo> CalculateHashCodes(List<FileInfo> files)
        {
            Dictionary<byte[], FileInfo> fileByHash = new Dictionary<byte[], FileInfo>(files.Count);

            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine("File: " + files[i].FullName);

                using (FileStream stream = File.OpenRead(files[i].FullName))
                {
                    byte[] hash = MD5.Create().ComputeHash(stream);
                    fileByHash.Add(hash, files[i]);

                    Console.Write("Hash: ");
                    foreach (byte hashByte in hash) Console.Write(hashByte);
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
            return fileByHash;
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

                    if (fileBytes.Length != fileToCompareBytes.Length)
                    {
                        continue;
                    }

                    if (!filesChecked.Contains(fileToCompare) && fileBytes.SequenceEqual(fileToCompareBytes))
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

        static void ExtractUniqueFiles(Dictionary<byte[], FileInfo> fileByHash, string outputFolder)
        {
            if (fileByHash.Count == 0)
            {
                Console.WriteLine("There are no files.");
                return;
            }

            Directory.CreateDirectory(outputFolder);

            List<byte[]> checkedHashes = new List<byte[]>();

            foreach (KeyValuePair<byte[], FileInfo> fileByHashEntry in fileByHash)
            {
                if (checkedHashes.Contains(fileByHashEntry.Key)) continue;
                
                bool identicalFound = false;

                foreach (KeyValuePair<byte[], FileInfo> fileByHashEntryToCompare in fileByHash)
                {
                    if (fileByHashEntry.Key.Length != fileByHashEntryToCompare.Key.Length)
                    {
                        continue;
                    }

                    if (!checkedHashes.Contains(fileByHashEntryToCompare.Key) && fileByHashEntry.Key.SequenceEqual(fileByHashEntryToCompare.Key))
                    {
                        if (identicalFound == false)
                        {
                            Console.WriteLine("[original]");
                            Console.WriteLine("Name: " + fileByHashEntry.Value.Name);
                            Console.WriteLine("Path: " + fileByHashEntry.Value.FullName);
                            foreach (byte hashByte in fileByHashEntry.Key) Console.Write(hashByte); Console.WriteLine();
                            Console.WriteLine("Size: " + fileByHashEntry.Value.Length);
                            Console.WriteLine();
                            identicalFound = true;
                        }
                        else
                        {
                            Console.WriteLine("\t" + "[duplicate]");
                            Console.WriteLine("\t" + "Name: " + fileByHashEntryToCompare.Value.Name);
                            Console.WriteLine("\t" + "Path: " + fileByHashEntryToCompare.Value.FullName);
                            foreach (byte hashByte in fileByHashEntryToCompare.Key) Console.Write(hashByte); Console.WriteLine();
                            Console.WriteLine("\t" + "Size: " + fileByHashEntryToCompare.Value.Length);
                            Console.WriteLine();
                        }

                        checkedHashes.Add(fileByHashEntryToCompare.Key);
                    }
                }

                if (File.Exists(outputFolder + @"\" + fileByHashEntry.Value.Name))
                {
                    int copyIndex = 1;
                    string newPathWithCopyIndex = String.Format(@"{0}\{1}({2}){3}", 
                                                                outputFolder, 
                                                                Path.GetFileNameWithoutExtension(fileByHashEntry.Value.FullName), 
                                                                copyIndex,
                                                                fileByHashEntry.Value.Extension);

                    while (File.Exists(newPathWithCopyIndex) == true)
                    {
                        copyIndex++;
                        newPathWithCopyIndex = String.Format(@"{0}\{1}({2}){3}", 
                                                             outputFolder, 
                                                             Path.GetFileNameWithoutExtension(fileByHashEntry.Value.FullName), 
                                                             copyIndex,
                                                             fileByHashEntry.Value.Extension);
                    }
                    File.Copy(fileByHashEntry.Value.FullName, newPathWithCopyIndex);
                }
                else
                {
                    File.Copy(fileByHashEntry.Value.FullName, outputFolder + @"\" + fileByHashEntry.Value.Name);
                }
                checkedHashes.Add(fileByHashEntry.Key);
            }
        }
    }
}
