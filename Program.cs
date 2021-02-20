using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text;
using System.Threading;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
           
            CreateRedirectionRulesFile();
            Server serverObj = new Server(8080);
            serverObj.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            StreamWriter sr = new StreamWriter("yourpath/redirectionRules.txt");
            string rule = "aboutus.html,aboutus2.html";
            sr.WriteLine(rule);
            sr.Close();
        }
         
    }
}
