using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TorrentPlugin.RTorrent
{
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

    public static class RTorrentHelper
    {
        public static bool TestConnection(string host, int port)
        {
            try
            {
                string xmlCommand = XmlRpcTorrentSerialize(new List<TorrentCommand>
                {
                    new TorrentCommand("system.client_version")
                });

                string xml = GetXmlResponse(host, port, xmlCommand);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                JObject jObject = JObject.Parse(JsonConvert.SerializeXmlNode(doc));

                string version = jObject["methodResponse"]["params"]["param"]["value"]["array"]["data"]["value"]["array"]["data"]["value"]["string"].Value<string>();

                return Version.TryParse(version, out Version _);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Tuple<int, int> GetCurrentStats(string host, int port)
        {
            try
            {
                string xmlCommand = XmlRpcTorrentSerialize(new List<TorrentCommand>
                {
                    //new TorrentCommand("view.list"),
                    //new TorrentCommand("system.listMethods"),
                    new TorrentCommand("d.multicall.filtered", "", "active", "or={d.up.rate=,d.down.rate=}", "d.name=", "d.up.rate=", "d.down.rate=")
                });

                string xml = GetXmlResponse(host, port, xmlCommand);

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(xml);

                XmlSerializer serializer = new XmlSerializer(typeof(MethodResponse));
                MethodResponse resultingMessage = (MethodResponse)serializer.Deserialize(new StringReader(xml));

                var torrents = resultingMessage.Params.Param.Value.Array.Data.Value[0].Array.Data.Value[0].Array.Data.Value;

                int download = 0;
                int upload = 0;

                foreach (var torrent in torrents)
                {
                    download += int.Parse(torrent.Array.Data.Value[2].I8);
                    upload += int.Parse(torrent.Array.Data.Value[1].I8);
                }

                return new Tuple<int, int>(download, upload);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetXmlResponse(string host, int port, string xmlCommand)
        {
            try
            {
                using (var tcpClient = new TcpClient(host, port))
                {
                    var networkStream = tcpClient.GetStream();
                    var streamWriter = new StreamWriter(networkStream) {AutoFlush = true};
                    var streamReader = new StreamReader(networkStream);

                    var header = $"CONTENT_LENGTH{'\0'}{Encoding.ASCII.GetBytes(xmlCommand).Length}{'\0'}SCGI{'\0'}1{'\0'}UNTRUSTED_CONNECTION{'\0'}1";

                    streamWriter.Write($"{header.Length}:{header},{xmlCommand}");

                    var response = streamReader.ReadToEnd();

                    streamWriter.Close();
                    tcpClient.Close();

                    return response.Substring(response.IndexOf("\r\n\r\n") + 4);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
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

        private static string GetParameterType(object parameter)
        {
            if ((parameter is long))
                return "i4";
            if (parameter is float)
                return "i8";
            return "string";
        }
    }

    [XmlRoot(ElementName = "value")]
    public class Value
    {
        [XmlElement(ElementName = "string")]
        public string String { get; set; }
        [XmlElement(ElementName = "i8")]
        public string I8 { get; set; }
        [XmlElement(ElementName = "array")]
        public Array Array { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        [XmlElement(ElementName = "value")]
        public List<Value> Value { get; set; }
    }

    [XmlRoot(ElementName = "array")]
    public class Array
    {
        [XmlElement(ElementName = "data")]
        public Data Data { get; set; }
    }

    [XmlRoot(ElementName = "param")]
    public class Param
    {
        [XmlElement(ElementName = "value")]
        public Value Value { get; set; }
    }

    [XmlRoot(ElementName = "params")]
    public class Params
    {
        [XmlElement(ElementName = "param")]
        public Param Param { get; set; }
    }

    [XmlRoot(ElementName = "methodResponse")]
    public class MethodResponse
    {
        [XmlElement(ElementName = "params")]
        public Params Params { get; set; }
    }
}
