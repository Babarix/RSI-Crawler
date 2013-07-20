using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;


namespace RSI_Crawler
{
    public partial class Form1 : Form
    {
        List<String> findings;
        BWCrawler bwc;

        public Form1()
        {
            InitializeComponent();
            listBox1.DisplayMember = "title";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start();
        }

        void start()
        {
            button1.Enabled = false;
            textBox2.Enabled = false;

            bwc = new BWCrawler();

            bwc.c.tick += new Crawler.tickEventHandler(ct_tick);

            label3.Text = "Status: Starting";

            findings = bwc.run(textBox2.Text);

            label3.Text = "Status: Done and " + findings.Count + " found. Babarix wishes you a good day.";
            label4.Text = "";

            foreach (String s in findings)
            {
                FormattedLink fl = new FormattedLink(s);
                listBox1.Items.Add(fl);
            }

            int max = 0;
            foreach (FormattedLink fl in listBox1.Items)
            {
                int l = textBox2.CreateGraphics().MeasureString(fl.title, Form1.DefaultFont).ToSize().Width;
                if (l > max)
                    max = l;
            }

            listBox1.ColumnWidth = max;
            listBox1.Enabled = true;
            propertyGrid1.Enabled = true;
            button1.Enabled = true;
            textBox2.Enabled = true;
        }

        void ct_tick(int found, int searched, int page)
        {
            this.Invoke((MethodInvoker)delegate
            {
                label4.Text = "Found: " + found + " Searched: " + searched + "/1000 Page: " + page;
            });
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = listBox1.SelectedItem;

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            object propGrid = typeof(PropertyGrid).InvokeMember("gridView", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, propertyGrid1, null);
            propGrid.GetType().InvokeMember("MoveSplitterTo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                                                            null, propGrid, new object[] { 0 });

        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.OldSelection != null)
            {
                PropertyGrid pg = (PropertyGrid)sender;
                System.Windows.Forms.Clipboard.SetText(pg.SelectedGridItem.Value.ToString());
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Process.Start("http://google.com");
        }

    }
}
