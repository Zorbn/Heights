using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging;

public static class Client
{
    public delegate void OnConnect();
    public delegate void OnConnectFailed();
    public delegate void OnTick();

    private static TcpClient Socket;
    private static MessageStream MessageStream;
    private static int TickRate;
    
    private static bool IsInitialized;

    private static Dictionary<Message.MessageType, MessageStream.MessageHandler> MessageHandlers;
    private static readonly Dictionary<Message.MessageType, MessageStream.MessageHandler> DefaultMessageHandlers = new()
    {
        { Message.MessageType.Initialize, HandleInitialize }
    };

    private static MessageStream.OnDisconnect OnDisconnectCallback;
    private static OnConnect OnConnectCallback;
    private static OnConnectFailed OnConnectFailedCallback;
    private static OnTick OnTickCallback;
    
    public static void StartClient(string ip, Dictionary<Message.MessageType, 
        MessageStream.MessageHandler> messageHandlers, int tickRate, OnTick onTick, 
        MessageStream.OnDisconnect onDisconnect, OnConnect onConnect, OnConnectFailed onConnectFailed)
    {
        TickRate = tickRate;
        
        OnDisconnectCallback = onDisconnect;
        OnConnectCallback = onConnect;
        OnConnectFailedCallback = onConnectFailed;
        OnTickCallback = onTick;
        
        MessageHandlers = messageHandlers;

        foreach (KeyValuePair<Message.MessageType, MessageStream.MessageHandler> defaultMessageHandler in DefaultMessageHandlers)
        {
            if (!MessageHandlers.ContainsKey(defaultMessageHandler.Key))
            {
                MessageHandlers.Add(defaultMessageHandler.Key, defaultMessageHandler.Value);
            }
        }
        
        Socket = new TcpClient()
        {
            ReceiveBufferSize = MessageStream.DataBufferSize,
            SendBufferSize = MessageStream.DataBufferSize
        };
        Socket.BeginConnect(IPAddress.Parse(ip), 8052, ConnectCallback, Socket);

        Tick();
        
        SpinWait.SpinUntil(() => false);
    }

    private static void Tick()
    {
        Task.Delay(1000 / TickRate).ContinueWith(_ => Tick());

        if (!IsInitialized) return;
        OnTickCallback();
    }
    
    private static void ConnectCallback(IAsyncResult result)
    {
        try
        {
            Socket.EndConnect(result);
        }
        catch
        {
            OnConnectFailedCallback();
        }

        OnConnectCallback();

        if (!Socket.Connected) return;
            
        MessageStream = new MessageStream(Socket, 0, MessageHandlers, OnDisconnectCallback);
        MessageStream.StartReading();
    }

    public static void SendMessage(Message.MessageType type, Data data)
    {
        if (!IsInitialized) return;
        
        MessageStream.SendMessage(type, data);
    }
    
    public static void HandleInitialize(Data data)
    {
        if (data is not InitializeData initData) return;

        IsInitialized = true;
        MessageStream.Id = initData.Id;
            
        Console.WriteLine($"Initialized with id of: {initData.Id}.");
    }
}