using Messaging;
using Shared;

namespace Server;

public class Server
{
    private readonly Dictionary<int, Player> players = new();
    private readonly MapData mapData;

    public Server()
    {
        mapData = MapData.LoadFromFile("Content/map.json");
        mapData.FindSpawnPos();
        
        Dictionary<Message.MessageType, MessageStream.MessageHandler> messageHandlers = new()
        {
            { Message.MessageType.ExampleNotification, ExampleNotification.HandleNotification },
            { Message.MessageType.MovePlayer, HandleMovePlayer }
        };
        
        Messaging.Server.StartServer("127.0.0.1", messageHandlers, 60, OnTick, OnDisconnect, OnConnect);
    }
    
    private void OnTick()
    {
        KeyValuePair<int, Player>[] allPlayerPairs = players.ToArray();
        foreach (KeyValuePair<int,Player> pair in allPlayerPairs)
        {
            Messaging.Server.SendMessageToAllExcluding(pair.Key, Message.MessageType.MovePlayer, new MovePlayerData
            {
                Id = pair.Key,
                X = pair.Value.Position.X,
                Y = pair.Value.Position.Y,
                Direction = (byte)pair.Value.Direction
            });
        }
    }
    
    private void OnConnect(int id)
    {
        var newPlayer = new Player(mapData.SpawnPos);
        players.Add(id, newPlayer);
        
        // Tell old players about the new player.
        Messaging.Server.SendMessageToAllExcluding(id, Message.MessageType.SpawnPlayer, new SpawnPlayerData
        {
            Id = id,
            X = newPlayer.Position.X,
            Y = newPlayer.Position.Y
        });

        // Tell new players about all players (old players and themself).
        KeyValuePair<int, Player>[] allPlayerPairs = players.ToArray();
        foreach (KeyValuePair<int, Player> pair in allPlayerPairs)
        {
            Messaging.Server.SendMessage(id, Message.MessageType.SpawnPlayer, new SpawnPlayerData
            {
                Id = pair.Key,
                X = pair.Value.Position.X,
                Y = pair.Value.Position.Y
            });
        }
    }

    private void OnDisconnect(int id)
    {
        players.Remove(id);
        Messaging.Server.SendMessageToAll(Message.MessageType.DestroyPlayer, new DestroyPlayerData
        {
            Id = id
        });
    }

    private void HandleMovePlayer(int fromId, Data data)
    {
        if (data is not MovePlayerData moveData) return;

        Player player = players[fromId];
        player.Position.X = moveData.X;
        player.Position.Y = moveData.Y;
        player.Direction = (Direction)moveData.Direction;
    }
}