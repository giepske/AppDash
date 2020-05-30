using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using static System.Int16;

namespace MinecraftPlugin
{
    public class MinecraftServerHelper
    {
        public StatusResponse PingServer(string hostname, int port)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient(hostname, port))
                using (var stream = tcpClient.GetStream())
                {
                    SendHandshake(stream);

                    SendStatusRequest(stream);

                    return ReadStatusResponse(stream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private StatusResponse ReadStatusResponse(Stream stream)
        {
            var buffer = new byte[MaxValue];
            int offset = 0;
            stream.Read(buffer, 0, buffer.Length);

            var length = ReadVarInt(ref offset, buffer);
            var packet = ReadVarInt(ref offset, buffer);
            var jsonLength = ReadVarInt(ref offset, buffer);

            var json = ReadString(ref offset, buffer, jsonLength);

            return JsonConvert.DeserializeObject<StatusResponse>(json);
        }

        private void SendStatusRequest(Stream stream)
        {
            List<byte> buffer = new List<byte>();
            Flush(stream, buffer, 0);
        }

        private void SendHandshake(Stream stream)
        {
            List<byte> buffer = new List<byte>();
            WriteVarInt(buffer, 47);
            WriteString(buffer, "localhost");
            WriteShort(buffer, 25565);
            WriteVarInt(buffer, 1);
            Flush(stream, buffer, 0);
        }

        //methods below are taken from here: https://gist.github.com/csh/2480d14fbbb33b4bbae3#file-serverping-cs-L165
        //the global stream has been changed to a local stream (taken from parameter)

        internal static byte ReadByte(ref int offset, byte[] buffer)
        {
            var b = buffer[offset];
            offset += 1;
            return b;
        }

        internal static byte[] Read(ref int offset, byte[] buffer, int length)
        {
            var data = new byte[length];
            Array.Copy(buffer, offset, data, 0, length);
            offset += length;
            return data;
        }

        internal static int ReadVarInt(ref int offset, byte[] buffer)
        {
            var value = 0;
            var size = 0;
            int b;
            while (((b = ReadByte(ref offset, buffer)) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5)
                {
                    throw new IOException("This VarInt is an imposter!");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        internal static string ReadString(ref int offset, byte[] buffer, int length)
        {
            var data = Read(ref offset, buffer, length);
            return Encoding.UTF8.GetString(data);
        }

        internal static void WriteVarInt(List<byte> buffer, int value)
        {
            while ((value & 128) != 0)
            {
                buffer.Add((byte)(value & 127 | 128));
                value = (int)((uint)value) >> 7;
            }
            buffer.Add((byte)value);
        }

        internal static void WriteShort(List<byte> buffer, short value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        internal static void WriteString(List<byte> buffer, string data)
        {
            var buffer2 = Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer, buffer2.Length);
            buffer.AddRange(buffer2);
        }

        internal static void Write(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }

        internal static void Flush(Stream stream, List<byte> buffer, int id = -1)
        {
            var buffer2 = buffer.ToArray();
            buffer.Clear();

            var add = 0;
            var packetData = new[] { (byte)0x00 };
            if (id >= 0)
            {
                WriteVarInt(buffer, id);
                packetData = buffer.ToArray();
                add = packetData.Length;
                buffer.Clear();
            }

            WriteVarInt(buffer, buffer2.Length + add);
            var bufferLength = buffer.ToArray();
            buffer.Clear();

            stream.Write(bufferLength, 0, bufferLength.Length);
            stream.Write(packetData, 0, packetData.Length);
            stream.Write(buffer2.ToArray(), 0, buffer2.Length);
        }

        //JSON objects of the status response

        public class StatusResponse
        {
            public Description Description { get; set; }
            public Players Players { get; set; }
            public Version Version { get; set; }
            public string Favicon { get; set; }
        }

        public class Description
        {
            public string Text { get; set; }
        }

        public class Players
        {
            public int Max { get; set; }
            public int Online { get; set; }
        }

        public class Version
        {
            public string Name { get; set; }
            public int Protocol { get; set; }
        }
    }
}
