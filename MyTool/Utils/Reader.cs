#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyTool.Utils
{
    public class Reader
    {
        private static readonly string DataPath = Directory.GetCurrentDirectory().Replace("bin/Debug/net5.0", "Data");
        private static readonly string SystemSeparator = Path.DirectorySeparatorChar.ToString();
        private static readonly string AccountFolder = "Account";
        private static readonly string ReviewLinkFolder = "ReviewLink";
        private static readonly string SubmitLinkFolder = "SubmitLink";
        
        public static async Task<List<string>> ReadLinksFromTxt(string subjectFolderName, string fileName)
        {
            var filePath = GetPath(ReviewLinkFolder, subjectFolderName, fileName);
            var linkArray = await File
                .ReadAllLinesAsync(filePath);
            return linkArray.ToList();
        }
        
        public static async Task<List<string>> ReadLinksFromTxt(string filePath)
        {
            var linkArray = await File
                .ReadAllLinesAsync(filePath);
            return linkArray.ToList();
        }
        
        public static async Task<List<AccountInfo>> ReadAccountsFromTxt(string fileName)
        {
            var accountList = new List<AccountInfo>();
            var dataPath = GetPath(AccountFolder, fileName);
            var accountArray = await File.ReadAllLinesAsync(dataPath);
            foreach (var line in accountArray)
            {
                var infoArray = line.Split(",");
                var account = new AccountInfo(infoArray[0],infoArray[1]+"@fpt.edu.vn", infoArray[2]);
                accountList.Add(account);
            }

            return accountList;
        }

        private static string GetPath(string folderLv1Name, string folderLv2Name = "", string fileName = "")
        {
            return 
                $"{DataPath}{SystemSeparator}{folderLv1Name}" +
                $"{(folderLv2Name != "" ? $"{SystemSeparator}{folderLv2Name}" : "")}" +
                $"{(fileName != "" ? $"{SystemSeparator}{fileName}" : "")}";
        }

        public static void Print<T>(List<T> list)
        {
            list.ForEach(item =>
            {
                Console.WriteLine(item);
            });
        }

        public static List<string> GetAllFilePaths(string linkFolderName, string subjectFolderName)
        {
            var basePath = GetPath(linkFolderName, subjectFolderName);
            var filesArray = Directory.GetFiles(basePath);
            return filesArray.ToList();
        }

        public static string GetFilePath(string linkFolderName, string subjectFolderName, string fileOwnerName)
        {
            var basePath = GetPath(linkFolderName, subjectFolderName);
            var filesList = Directory.GetFiles(basePath).ToList();
            return filesList.FirstOrDefault(file => file.Contains(fileOwnerName));
        }

        public static async Task WriteSubmitLinksToReviewLinks(List<string> list, string fileName, string subjectFolderName)
        {
            var path = GetPath(ReviewLinkFolder, subjectFolderName, fileName + ".txt");
            await File.WriteAllLinesAsync(path, list);
        }
    }
}