using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebCrawlerLib
{
    public class Crawler
    {
        #region Static And Constant Variables
        private HashSet<string> urlHashList = new HashSet<string>();//Stored urls for checking unique
        private readonly string baseUrl;//This is default url
        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();//Stored unique urls processing by the threads 
        private readonly object _lock = new object();//Lock Object for using lock operations
        private const string regexPaternMatchHtml = @"href=\""(.*?)\""";//Matched links with in href
        private const string regexPaternMatchLinks = @"(<a.*?>.*?</a>)";//Match links between a tags
        #endregion

        /// <summary>
        /// Create Crawler set url and Run First Thread
        /// </summary>
        /// <param name="inputUrl"></param>
        public Crawler(string inputUrl)
        {
            if (!(inputUrl.StartsWith("http://") || inputUrl.StartsWith("https://")))
            {
                inputUrl = "https://" + inputUrl;
            }

            baseUrl = inputUrl;
            if (!CheckUrl(inputUrl))
                return;

            urlHashList.Add(baseUrl);
            CrawlUrl(baseUrl);

        }

        public int Crawl()
        {
            Semaphore semaphore = new Semaphore(3, 30);//Initial Start & Total Capacity of Thread

            using (var finished = new CountdownEvent(1))
            {
                //Run ProcessWithTreadPoolMethod Until Queue is Empty
                while (queue.Count > 0)
                {
                    finished.AddCount(); // Indicate that there is another work item.

                    semaphore.WaitOne();
                    ThreadPool.QueueUserWorkItem(arg =>
                    {
                        try
                        {
                            ProcessWithThreadPoolMethod();
                        }
                        finally
                        {
                            semaphore.Release();
                            finished.Signal();
                        }
                    });
                }

                finished.Signal(); // Signal that queueing is complete.
                finished.Wait(); // Wait for all work items to complete.
            }
            return urlHashList.Count;
        }


        /// <summary>
        /// Process Threads with thread pool and run ThreadJob
        /// </summary>
        /// <param name="o"></param>
        public void ProcessWithThreadPoolMethod()
        {
            string url = String.Empty;
            queue.TryDequeue(out url);

            if (!String.IsNullOrEmpty(url))
                CrawlUrl(url);
        }

        /// <summary>
        /// Download Html String With WebClient
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private string DownloadHtmlString(string uri)
        {
            try
            {
                using (var wc = new System.Net.WebClient() { Encoding = UTF8Encoding.UTF8 })
                {
                    //Console.WriteLine($"{uri}");
                    return wc.DownloadString(uri);
                }
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// DownloadHtmlString And Write All Corrected Urls
        /// </summary>
        /// <param name="url"></param>
        public void CrawlUrl(string url)
        {
            ExtractUrls(DownloadHtmlString(url));
        }

        /// <summary>
        /// Extract Urls From The HTML String
        /// </summary>
        /// <param name="html"></param>
        private void ExtractUrls(string html)
        {
            if (String.IsNullOrEmpty(html)) return;

            string url = String.Empty;
            MatchCollection matches = Regex.Matches(html, regexPaternMatchLinks, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                url = match.Groups[1].Value;

                Match urlMatch = Regex.Match(url, regexPaternMatchHtml, RegexOptions.Singleline);

                if (urlMatch.Success)
                {
                    url = UrlUtils.BuildUrl(baseUrl, urlMatch.Groups[1].Value);

                    if (CheckUrl(url) && !urlHashList.Contains(url))
                    {
                        lock (_lock)
                        {
                            urlHashList.Add(url);
                            queue.Enqueue(url);
                            Console.WriteLine($"{url}");
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Check is a File (doc|docx|pdf|rar|zip|jpg)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsFile(string url)
        {
            if (url.EndsWith(".zip") || url.EndsWith(".rar") || url.EndsWith(".doc") || url.EndsWith(".docx")
                || url.EndsWith(".jpg") || url.EndsWith(".pdf") || url.EndsWith(".xls") || url.EndsWith(".xlsx"))
                return true;

            return false;
        }

        /// <summary>
        /// Check url is a with in same subdomain
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool CheckUrl(string url)
        {
            if (url.StartsWith("mailto:"))
            {
                return false;
            }
            if (IsFile(url))
            {
                return false;
            }

            if (!UrlUtils.IsSameHost(baseUrl, url))
            {
                return false;
            }
            return true;
        }
    }
}
