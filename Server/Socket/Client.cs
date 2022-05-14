using System.Net;
using System.Net.Sockets;
using Server.Services;
using SharedLibrary;
using SharedLibrary.Networking;

namespace Server.Socket;

public sealed class Client
{
    public static int DataBufferSize { get; set; } = 4096;
    public int Id { get; set; }
    public TCP Tcp { get; set; }
    public UDP Udp { get; set; }

    public Client(int id)
    {
        Id = id;
        Tcp = new TCP(id);
        Udp = new UDP(id);
    }

    public class TCP
    {
        private readonly int _id;
        private NetworkStream _stream;
        private byte[] _receiveBuffer;
        private Packet _receivedData;

        public TCP(int id)
        {
            _id = id;
        }

        public TcpClient Socket { get; set; }

        public void Connect(TcpClient socket)
        {
            Socket = socket;
            socket.ReceiveBufferSize = DataBufferSize;
            socket.SendBufferSize = DataBufferSize;

            _stream = socket.GetStream();

            _receivedData = new Packet();
            _receiveBuffer = new byte[DataBufferSize];
            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(_id, "Holla");
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (Socket != null)
                {
                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending data to player {_id} via Tcp : {e}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var byteLength = _stream.EndRead(result);
                if (byteLength <= 0)
                {
                    //todo: disconnect
                    return;
                }

                var data = new byte[byteLength];
                Array.Copy(_receiveBuffer, data, byteLength);

                _receivedData.Reset(HandleData(data));

                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //todo: disconnect
            }
        }

        private bool HandleData(byte[] data)
        {
            var packetLength = 0;
            _receivedData.SetBytes(data);
            if (_receivedData.UnreadLength() >= 4)
            {
                packetLength = _receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
            {
                var packetBytes = _receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using var packet = new Packet(packetBytes);
                    var packetId = packet.ReadInt();
                    SocketServerService.PacketHandlers[packetId].Invoke(_id, packet);
                });

                packetLength = 0;

                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }
    
    public class UDP
    {
        private int _id;
        public IPEndPoint EndPoint;

        public UDP(int id)
        {
            _id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            ServerSend.UDPTest(_id);
        }

        public void SendData(Packet packet)
        {
            SocketServerService.SendUDPData(EndPoint, packet);
        }

        public void HandleData(Packet packet)
        {
            var length = packet.ReadInt();
            var packetBytes = packet.ReadBytes(length);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using var packetLocal = new Packet(packetBytes);
                var packetId = packetLocal.ReadInt();
                SocketServerService.PacketHandlers[packetId].Invoke(_id, packetLocal);
            });
        }
    }
}