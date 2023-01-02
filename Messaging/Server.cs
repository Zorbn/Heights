using System.Net;
using System.Net.Sockets;

namespace Messaging;

public static class Server
{
    public delegate void OnClientConnect(int id);

    public delegate void OnTick();

    private static TcpListener TcpListener;
    private static Dictionary<int, MessageStream> Clients;
    private static Dictionary<Message.MessageType, MessageStream.MessageHandler> MessageHandlers;

    private static int TickRate;
    private static int LastClientId;

    private static MessageStream.OnDisconnect OnDisconnectCallback;
    private static OnClientConnect OnClientConnectCallback;
    private static OnTick OnTickCallback;

    public static void StartServer(Dictionary<Message.MessageType, MessageStream.MessageHandler> messageHandlers, int tickRate, OnTick onTick,
        MessageStream.OnDisconnect onDisconnect, OnClientConnect onClientConnect)
    {
        MessageHandlers = messageHandlers;
        Clients = new Dictionary<int, MessageStream>();
        TickRate = tickRate;

        OnDisconnectCallback = onDisconnect;
        OnTickCallback = onTick;
        OnClientConnectCallback = onClientConnect;
        
        TcpListener = new TcpListener(IPAddress.Any, IpUtils.DefaultPort);
        TcpListener.Start();
        TcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

        Tick();

        SpinWait.SpinUntil(() => false);
    }

    public static void StopServer()
    {
        foreach (KeyValuePair<int, MessageStream> messageStream in Clients) messageStream.Value.StopReading();

        TcpListener?.Stop();
    }

    private static void Tick()
    {
        Task.Delay(1000 / TickRate).ContinueWith(_ => Tick());

        OnTickCallback();
    }

    private static void TcpConnectCallback(IAsyncResult result)
    {
        TcpClient client = TcpListener.EndAcceptTcpClient(result); // Finish accepting client
        TcpListener.BeginAcceptTcpClient(TcpConnectCallback, null); // Begin accepting new clients
        Console.WriteLine($"Connection from: {client.Client.RemoteEndPoint}...");

        int newClientId = LastClientId++;
        Clients.Add(newClientId, new MessageStream(client, newClientId, MessageHandlers, OnDisconnect));
        Clients[newClientId].StartReading();

        InitializeData initData = new()
        {
            Id = newClientId
        };

        Clients[newClientId].SendMessage(Message.MessageType.Initialize, initData);
        OnClientConnectCallback.Invoke(newClientId);
    }

    public static void Disconnect(int id)
    {
        if (!Clients.TryGetValue(id, out MessageStream clientStream)) return;
        clientStream.StopReading();
    }

    private static void OnDisconnect(int id)
    {
        Clients.Remove(id);
        OnDisconnectCallback(id);
    }

    public static void SendMessage(int id, Message.MessageType type, IData data)
    {
        if (!Clients.ContainsKey(id)) return;
        Clients[id].SendMessage(type, data);
    }

    public static void SendMessageToAll(Message.MessageType type, IData data)
    {
        foreach (KeyValuePair<int, MessageStream> client in Clients) SendMessage(client.Key, type, data);
    }

    public static void SendMessageToAllExcluding(int excludedId, Message.MessageType type, IData data)
    {
        foreach (KeyValuePair<int, MessageStream> client in Clients)
        {
            if (client.Key == excludedId) continue;
            SendMessage(client.Key, type, data);
        }
    }
}