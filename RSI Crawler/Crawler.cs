using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abot.Crawler;
using Abot.Poco;
using System.Net;
using System.Text.RegularExpressions;

namespace RSI_Crawler
{
    // Code Cleanup from Test Class
    class Crawler
    {
        PoliteWebCrawler crawler; // The Crawler
        String serchterm; // The searchterm
        List<String> ret; // List of found url's to return
        bool reached_end; // Check if all pages of search result are searched
        int page; // Current searched page
        public delegate void tickEventHandler(int found, int searched, int page); // Simplyfied Handle for the tick event
        public event tickEventHandler tick; // The tick event ti update the GUI
        int found; // Statistic for GUI about founded pages
        int searched; // Statistic for GUI about searched pages

        //Konstructor
        public Crawler()
        {
            setup_abot();
            reached_end = false;
            page = -1;
        }

        // Setting up bot config
        public void setup_abot()
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();

            crawlConfig.CrawlTimeoutSeconds = 150;
            crawlConfig.MaxConcurrentThreads = 25;
            crawlConfig.IsExternalPageCrawlingEnabled = false;
            crawlConfig.MaxCrawlDepth = 1;
            crawlConfig.MaxPagesToCrawl = 1000;
            crawlConfig.UserAgentString = "abot v1.0 http://code.google.com/p/abot";

            crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);

            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;

          
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                Regex rx = new Regex(@"\d{5}");

                if (!rx.IsMatch(pageToCrawl.Uri.ToString()) && !pageToCrawl.Uri.ToString().Contains("text="))
                    return new CrawlDecision { Allow = false, Reason = "Want only comlinks" };

                return new CrawlDecision { Allow = true, Reason = "OK Link" }; ;
            });

        }

        // run methode that starts the crawler
        public List<String> run(String ilink, String iserchterm)
        {
            serchterm = iserchterm;
            ret = new List<string>();

            while (!reached_end)
            { 
                page++;
                CrawlResult result = crawler.Crawl(new Uri(ilink.Replace(' ','+') + "&page=" + page));
                if (result.ErrorOccurred)
                {
                    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
                    reached_end = true;
                }
                else
                {
                    Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
                    setup_abot();
                }
              
            }

            return ret;

        }

        // Filter the found pages to find only wanted ones
        private bool pass_filter(CrawledPage cp)
        {
            bool retb = false;
            searched++;

            if (cp.RawContent.ToLower().Contains(serchterm.ToLower()))
            {
                retb = true;
            }
            if (cp.RawContent.Contains("There is nothing. You drift endlessly through space."))
            {
                reached_end = true;
            }

            this.tick(found, searched, page);
            return retb;
        }

        // Add found link to return list
        private void add_ret(String s)
        {
            if (!ret.Contains(s) && !s.Contains("text="))
            {
                ret.Add(s);
                found++;
            }
        }

        // Debug for every pages started to crawl
        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        // Debug + filter for parsed pages
        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
            {
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);
                if (pass_filter(crawledPage))
                {
                    add_ret(crawledPage.Uri.ToString());
                }
            }

            if (string.IsNullOrEmpty(crawledPage.RawContent))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
        }
    }
}
