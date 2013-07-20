using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RSI_Crawler
{
    // Moveing BW from form to own class
    class BWCrawler
    {
        String searchterm;
        List<String> findings;
        public Crawler c;
        BackgroundWorker bw;

        public BWCrawler()
        {
            c = new Crawler();

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        public List<String> run(String isearchterm)
        {
            searchterm = isearchterm;

            bw.RunWorkerAsync();

            while (bw.IsBusy)
                System.Windows.Forms.Application.DoEvents();

            return findings;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            findings = c.run(@"https://robertsspaceindustries.com/comm-link?text=" + searchterm, searchterm);
        }


        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
