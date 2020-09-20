using System;
using System.Linq;
using CommandLine;
using docs_finder.models;
using Microsoft.EntityFrameworkCore;


namespace docs_finder
{
    public class Options
    {
        [Option('i', "index", Required = false, HelpText = "Индексировать новые документы из диалогов")]
        public bool Index { get; set; }
        
        [Option('q', "query", Required = false, HelpText = "Поисковой запрос")]
        public string Query { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.Index)
                    {
                        Console.WriteLine("Индексируем изменения...");
                        var indexer = new DocIndexer(ApiBuilder.GetApi());
                        indexer.IndexNewDocs();
                    }

                    var query = string.Empty;
                    if (!string.IsNullOrEmpty(o.Query))
                    {
                        query = o.Query;
                    }
                    else
                    {
                        Console.WriteLine("Введите запрос:");
                        query = Console.ReadLine();
                    }

                    var searcher = new DocSearcher();
                    var results = searcher.Search(query.ToLower());
                    results.ForEach(Helpers.PrintDoc);
                });
        }
    }
}