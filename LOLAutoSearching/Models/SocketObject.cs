using LOLAutoSearching.Objects;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CapstoneDesign
{
    public static class SocketObject
    {
        private static Socket client;
        private static void SocketReady()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("52.141.1.54");

            //IPAddress ipAddress = IPAddress.Parse("192.168.0.9");
            IPEndPoint ipep = new IPEndPoint(ipAddress, 9001);
            try
            {
                client.Connect(ipep);
            }
            catch
            {
                try
                {
                    Debug.Assert(client.Connected);
                    ipAddress = IPAddress.Parse("112.145.135.126");
                    ipep = new IPEndPoint(ipAddress, 9001);
                    client.Connect(ipep);
                }
                catch
                {
                    Debug.Assert(client.Connected);
                    ipAddress = IPAddress.Parse("221.150.243.58");
                    ipep = new IPEndPoint(ipAddress, 9001);
                    client.Connect(ipep);
                }
            }
        }

        public enum ImageType { gameType, userName };
        public static string GetImageSearching(User user, byte[] content)
        {
            SocketReady();
            client.Send(Encoding.UTF8.GetBytes($"{user.ID}/{user.PW}/{user.Session}/{content.Length}"));
            Thread.Sleep(100);

            int arraySize = content.Length;
            int loopTime = (arraySize / 1024) + 1;

            for (int index = 0; index < loopTime; index++)
            {
                byte[] buffer = new byte[1024];
                for (int bufferIndex = 0; bufferIndex < 1024; bufferIndex++)
                {
                    if (index * 1024 + bufferIndex >= arraySize)
                    {
                        break;
                    }
                    buffer[bufferIndex] = content[index * 1024 + bufferIndex];
                }
                Thread.Sleep(100);
                int reslut = client.Send(buffer);
            }


            string dataString = "";
            try
            {
                byte[] data = new byte[128 * 50];
                client.Receive(data);
                foreach (char r in Encoding.UTF8.GetString(data))
                {
                    if (r != 0)
                    {
                        dataString += r;
                    }
                    else if (r == 0)
                    {
                        break;
                    }
                }
            }
            catch
            {
                dataString = null;
            }
            client.Close();
            return dataString;
        }
    }
}
