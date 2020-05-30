using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RTorrentTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                string xmlResponse = await GetXmlResponse();

                xmlResponse = xmlResponse.Substring(xmlResponse.IndexOf("\r\n\r\n")).Trim();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlResponse);

                var activeTorrents = xmlDocument["methodResponse"]["params"]["param"]["value"]["array"]["data"]["value"]["array"]["data"]["value"]["array"]["data"].ChildNodes;

                Console.Clear();

                for (int i = 0; i < activeTorrents.Count; i++)
                {
                    var activeTorrent = activeTorrents[i];

                    activeTorrent = activeTorrent["array"]["data"];

                    string name = activeTorrent.ChildNodes[0].ChildNodes[0].ChildNodes[0].Value;
                    int currentBytes = int.Parse(activeTorrent.ChildNodes[1].ChildNodes[0].ChildNodes[0].Value);
                    decimal currentKib = (decimal)currentBytes / 1024;

                    Console.WriteLine($"{name} :: {string.Format("{0:0.#}", currentKib)}KiB");
                }

                await Task.Delay(2000);
            }
        }

        private static async Task<string> GetXmlResponse()
        {
            string xmlCommand = XmlRpcTorrentSerialize(new List<TorrentCommand>
            {
                //new TorrentCommand("view.list"),
                //new TorrentCommand("system.listMethods"),
                new TorrentCommand("d.multicall.filtered", "", "active", "or={d.up.rate=,d.down.rate=}", "d.name=", "d.up.rate=")
            });

            string response;

            try
            {
                using (var tcpClient = new TcpClient("192.168.178.51", 5000))
                {
                    var networkStream = tcpClient.GetStream();
                    var streamWriter = new StreamWriter(networkStream);
                    streamWriter.AutoFlush = true;
                    var streamReader = new StreamReader(networkStream);

                    var header = $"CONTENT_LENGTH{'\0'}{Encoding.ASCII.GetBytes(xmlCommand).Length}{'\0'}SCGI{'\0'}1{'\0'}UNTRUSTED_CONNECTION{'\0'}1";

                    await streamWriter.WriteAsync($"{header.Length}:{header},{xmlCommand}");

                    response = await streamReader.ReadToEndAsync();

                    streamWriter.Close();
                    tcpClient.Close();
                    //return response;
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }

            return response;
        }

        private static string XmlRpcTorrentSerialize(List<TorrentCommand> TorrentCommands)
        {
            var str = new StringBuilder();

            str.Append($"<?xml version='1.0' encoding='iso-8859-1'?><methodCall><methodName>");

            str.Append($"system.multicall</methodName><params><param><value><array><data>");
            foreach (var command in TorrentCommands)
            {
                str.Append($"<value><struct><member><name>methodName</name><value><string>{command.Command}</string></value></member><member><name>params</name><value><array><data>");

                foreach (var param in command.Parameters)
                {
                    str.Append($"<value><{GetParameterType(param)}>{param}</{GetParameterType(param)}></value>");
                }
                str.Append("</data></array></value></member></struct></value>");
            }
            str.Append($"</data></array></value></param>");
            str.Append("</params></methodCall>");
            return str.ToString();
        }

        protected static string GetParameterType(object parameter)
        {
            if ((parameter is long))
                return "i4";
            if (parameter is float)
                return "i8";
            return "string";
        }
    }

    public class TorrentCommand
    {
        public string Command { get; }
        public string[] Parameters { get; }

        public TorrentCommand(string command, params string[] parameters)
        {
            Command = command;
            Parameters = parameters;
        }
    }
}
