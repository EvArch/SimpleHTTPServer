using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {

             headerLines.Add(GetStatusLine(code));
            headerLines.Add("Content-Type: "+contentType);
            headerLines.Add("Date: "+DateTime.Now);
            if (redirectoinPath != null)
                headerLines.Add("Location: "+redirectoinPath);
            headerLines.Add(content);
            foreach (string s in headerLines)
            {
                responseString += s;
                responseString += '\n';
            }
        }

        private string GetStatusLine(StatusCode code)
        {
            string statusLine = "";
            int codeNum = code.GetHashCode();
            if (codeNum == 200)
                statusLine = Configuration.ServerHTTPVersion+' '+codeNum+' '+"OK";
            else if(codeNum == 500)
                statusLine = Configuration.ServerHTTPVersion + ' ' + codeNum + ' ' + "Internal Server Error";
            else if(codeNum == 404)
                statusLine = Configuration.ServerHTTPVersion + ' ' + codeNum + ' ' + "Not Found";
            else if(codeNum == 400)
                statusLine = Configuration.ServerHTTPVersion + ' ' + codeNum + ' ' + "Bad Request";
            else if(codeNum == 301)
                statusLine = Configuration.ServerHTTPVersion + ' ' + codeNum + ' ' + "Redirect";

            return statusLine;
        }
    }
}
