using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber)
        {
            LoadRedirectionRules("redirectionRules.txt");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any , portNumber);
            serverSocket.Bind(hostEndPoint);
        }

        public void StartServer()
        {
            serverSocket.Listen(100);
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);

            }
        }



        public void HandleConnection(object obj)
        {

            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            byte[] requestRecieve = new byte[1024 * 1024];
            while (true)
            {
                try
                {
                    int receivedLen =  clientSocket.Receive(requestRecieve);
                    string reqString = Encoding.ASCII.GetString(requestRecieve);
                    //Console.WriteLine(reqString);
                    if (receivedLen == 0)
                        break;
                    Request requestObj = new Request(reqString);
                    Response response = HandleRequest(requestObj);
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }

            clientSocket.Close();
        }

       Response HandleRequest(Request request)
        {


            string contentType = "text/html" ;

            try
            { 
                Response resp;
                 if (!(request.ParseRequest()))
                 {
                     resp = new Response(StatusCode.BadRequest, contentType, File.ReadAllText(Configuration.RootPath + Configuration.BadRequestDefaultPageName) , null);
                     return resp;
                 }


                request.relativeURI = request.headerLines.ElementAt(0).Value;
                if (!(request.relativeURI == "/"))
                 {
                    request.relativeURI =  request.relativeURI.Split('/')[1];
                    if (GetRedirectionPagePathIFExist(request.relativeURI) != string.Empty)
                    {
                        resp = new Response(StatusCode.Redirect, contentType, File.ReadAllText(Configuration.RootPath + Configuration.RedirectionDefaultPageName), GetRedirectionPagePathIFExist(request.relativeURI));
                        return resp;
                    }

                    else
                    {
                        string fileFound = null;
                        foreach (string file in Directory.EnumerateFiles("yourpath/inetpub/wwwroot/fcis1/", request.relativeURI, SearchOption.AllDirectories))
                        {
                            fileFound = file;
                        }

                        if (fileFound == null)
                        {
                            resp = new Response(StatusCode.NotFound, contentType, File.ReadAllText(Configuration.RootPath + Configuration.NotFoundDefaultPageName), null);
                            return resp;
                        }
                        else
                        {
                            resp = new Response(StatusCode.OK, contentType, File.ReadAllText(fileFound), null);
                            return resp;
                        }
                    }
                }


                resp = new Response(StatusCode.OK, contentType, File.ReadAllText(Configuration.RootPath + Configuration.DefaultPage), null);
                return resp;

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Response resp = new Response(StatusCode.InternalServerError, contentType, LoadDefaultPage(Configuration.RedirectionDefaultPageName), null);
                return resp;

            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, Configuration.DefaultPage); // absolute path
            try {
                if (!(File.Exists(filePath)))
                {
                    throw new Exception();
                }
                else
                {
                    string fileContent = File.ReadAllText(filePath);
                    return fileContent;
                }

            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
            // else read file and return its content

            
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file
                string fileContent = File.ReadAllText(filePath);
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                Configuration.RedirectionRules[fileContent.Split(',')[0]] = fileContent.Split(',')[1];
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
