using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11 ,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI ;
        public Dictionary<string, string> headerLines = new Dictionary<string, string>();
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
       // string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
          
        }

        public bool ParseRequest()
        {
           

            if(!(ParseRequestLine()))
                return false;

            if (!(requestLines.Length == 3 || requestLines.Length > 3))
                return false;

            if (!(ParseRequestLine()))
                return false;

            
            if (!(ValidateBlankLine()))
                return false;


            if (!(LoadHeaderLines()))
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            if (requestString != null)
            {

                string[] delimeter = { "\r\n" };
                requestLines = requestString.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);  
                return true;
            }
            

                return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {

            for (int i = 0; i < requestLines.Length - 2; i++)
            {
                headerLines.Add(requestLines[i].Split(' ')[0], requestLines[i].Split(' ')[1]);
            }

            string m = headerLines.ElementAt(0).Value;

            if (m == "GET")
                method = RequestMethod.GET;
            else if (m == "POST")
                method = RequestMethod.POST;
            else
                method = RequestMethod.HEAD;
               

            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestString.EndsWith("\n"))
                return true;
            
            return false;
        }


    }
}
