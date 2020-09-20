using System;
using docs_finder.models;

namespace docs_finder
{
    public static class Helpers
    {
        public static void ColoredWriteLine(string str, ConsoleColor color)
        {
            var fore = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = fore;
        }
        
        public static void ColoredWrite(string str, ConsoleColor color)
        {
            var fore = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ForegroundColor = fore;
        }
        
        public static void PrintDoc(Doc doc)
        {
            ColoredWrite(doc.Title, ConsoleColor.DarkYellow);
            Console.WriteLine(" " + doc.Uri);
        }
    }
}