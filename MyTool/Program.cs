using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyTool.Utils;
using OpenQA.Selenium.Chrome;

namespace MyTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await RunGetLink("PMG", "DE150182");
            await RunReview("PMG","Base", "HaKhue", "HuuLoi");
        }
        
        static async Task<bool> RunReview(string course, string accountFileName, params string[] exceptedNames)
        {
            Console.WriteLine($"****************************** {course} ********************************");
            Console.WriteLine();
            Console.WriteLine();
            var accounts = await Reader.ReadAccountsFromTxt($"{accountFileName}.txt");
            var files = Reader.GetAllFilePaths("ReviewLink", course);
            var listTask = new List<Task>();
            foreach (var account in accounts.Where(a => !exceptedNames.ToList().Contains(a.Name)))
            {
                listTask.Add(Task.Run(async () =>
                {
                    var linksToReview = new List<string>();
                    foreach (var file in files)
                    {
                        var fileOwner = Path.GetFileName(file).Replace(".txt", "");
                        if (account.Name != fileOwner)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Đang thêm link của {fileOwner}...");
                            Console.WriteLine();
                            var linksInFile = await Reader.ReadLinksFromTxt(file);
                            linksToReview.AddRange(linksInFile);
                            Console.WriteLine($"Đã thêm");
                            Console.WriteLine();
                        }
                    }

                    if (linksToReview.Any())
                    {
                        await new Driver(new ChromeDriver(), account, linksToReview, Mode.REVIEW, course).Start();
                    }
                    Console.WriteLine($"{account.Name} đã hoàn thành");
                }));
            }

            await Task.WhenAll(listTask);
            return true;
        }

        static async Task<bool> RunGetLink(string course, params string[] names)
        {
            Console.WriteLine($"****************************** {course} ********************************");
            Console.WriteLine();
            Console.WriteLine();
            var nameList = names.ToList();
            var accounts = await Reader.ReadAccountsFromTxt("Clients.txt");
            var listTask = new List<Task>();
            foreach (var name in nameList)
            {
                listTask.Add(Task.Run(async () =>
                {
                    var account = accounts.FirstOrDefault(acc => acc.Name == name);
                    var fileToRead = Reader.GetFilePath("SubmitLink", course, name);
                    var linksInFile = await Reader.ReadLinksFromTxt(fileToRead);
                    await new Driver(new ChromeDriver(), account, linksInFile, Mode.GET, course).Start();
                }));
            }

            await Task.WhenAll(listTask);
            return true;
        }
    }
}