using System;
using System.Text;
using WebCrawlerLib;

namespace MelontechOrhanBekir
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.Write("Write url to crawl:");
            string inputUrl = Console.ReadLine();

            Crawler crawler = new Crawler(inputUrl);

            int urlCount = crawler.Crawl();          

            Console.WriteLine($"--- Finished with {urlCount} unique urls---");
            Console.ReadLine();
        }
    }

    
}
