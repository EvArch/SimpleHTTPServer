using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
       public static StreamWriter sr = new StreamWriter("yourpath/log.txt" );

        public static void LogException(Exception ex)
        {
                DateTime now = DateTime.Now;
                sr.WriteAsync("Datetime: " + now);
                sr.WriteAsync("Message: " + ex.Message);
                sr.Flush(); //flush write buffer content to into file and clear buffer for next use
        }
    }
}
