using UnityEngine;

namespace Mirror.Examples.Chat
{
    [AddComponentMenu("")]
    public class ChatNetworkManager : NetworkManager
    {
        public string PlayerName { get; set; }

        public ChatWindow chatWindow;

        public Player playerPrefab;

        void Awake()
        {
            client.Authenticated.AddListener(OnAuthenticated);
        }

        public class CreatePlayerMessage : MessageBase
        {
            public string name;
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            conn.RegisterHandler<NetworkConnectionToClient, CreatePlayerMessage>(OnCreatePlayer);
        }

        public void OnAuthenticated(NetworkConnection conn)
        {
            // tell the server to create a player with this name
            conn.Send(new CreatePlayerMessage { name = PlayerName });
        }

        private void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
        {
            // create a gameobject using the name supplied by client
            GameObject playergo = Instantiate(playerPrefab).gameObject;
            playergo.GetComponent<Player>().playerName = createPlayerMessage.name;

            // set it as the player
            server.AddPlayerForConnection(connection, playergo);

            chatWindow.gameObject.SetActive(true);
        }
    }
}
