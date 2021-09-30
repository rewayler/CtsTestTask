using System;
using CtsTestTask;

namespace CtsTestTaskConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting CTS Test Task Console.");
            Console.WriteLine("-g [count]   generate xml file");
            Console.WriteLine("-p           process xml file");

            if (args.Length == 0) return;
            else if (args[0] == "-g")
                GenerateXML((Convert.ToInt32(args[1])));
            else if (args[0] == "-p")
                ProcessXML();
        }
        private static void GenerateXML(int count)
        {
            Console.WriteLine($"Generating TradesList.xml, {count} items...");
            (new XMLGenerator()).GenerateTestXml(AppDomain.CurrentDomain.BaseDirectory, count);
            Console.WriteLine("XML Generated.");
        }
        private static void ProcessXML()
        {
            Console.WriteLine("Reading TradesList.xml...");
            (new XMLProcessor(Console.WriteLine)).Process($"{AppDomain.CurrentDomain.BaseDirectory}TradesList.xml");
            Console.WriteLine("XML Proccesed.");
        }
    }
}