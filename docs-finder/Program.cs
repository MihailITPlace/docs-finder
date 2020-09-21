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

                    if (!string.IsNullOrEmpty(o.Query))
                    {
                        var searcher = new DocSearcher();
                        var results = searcher.Search(o.Query.ToLower());
                        results.ForEach(Helpers.PrintDoc);
                    }
                });
        }
    }
}