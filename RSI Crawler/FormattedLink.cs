using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace RSI_Crawler
{
    [DefaultPropertyAttribute("Formatted Links")]
    class FormattedLink
    {
        [Category("Result")]
        [Description("link to RSI Page")]
        public String link { get; set; }

        [Category("Result")]
        [Description("ref tag for wiki")]
        public String lref { get; set; }

        [Category("Result")]
        [Description("short ref tag")]
        public String sref { get; set; }

        [Category("Result")]
        [Description("link in RSI format")]
        public String rsi { get; set; }

        [Category("Result")]
        [Description("id of the post")]
        public String id { get; set; }

        [Category("Result")]
        [Description("title of the post")]
        public String title { get; set; }


        public FormattedLink(String ilink)
        {
            Regex rx = new Regex(@"\d{5}");
            int pos = 0;
            int pos6 = 0;
            int poscom = 0;
            link = ilink;

            id = rx.Match(link).Value;

            pos = link.IndexOf(id);
            pos6 = pos + 6;
            title = link.Substring(pos6, link.Length - pos6).Replace('-', ' ');

            poscom = link.IndexOf(".com/") + 5;
            rsi = link.Substring(poscom, link.Length - poscom) + "/";
            rsi = "{{RSI|url=" + rsi + "|text=" + title + "}}";

            sref = "<ref name=\"ID" + id + "\" />";
            lref = "<ref name=\"ID" + id + "\">" + rsi + "</ref>";



        }

    }
}
