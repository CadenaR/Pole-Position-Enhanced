using System;

namespace Mirror.Examples.Chat
{
    public class Player : NetworkBehaviour
    {
        [SyncVar]
        public string playerName;

        //public static event Action<Player, string> OnMessage;

        public delegate void OnMessageDelegate(Player player, string message);

        public static event OnMessageDelegate OnMessage;

        [Command]
        public void CmdSend(string message)
        {
            if (message.Trim() != "")
                RpcReceive(message.Trim());
        }

        [ClientRpc]
        public void RpcReceive(string message)
        {
            OnMessage?.Invoke(this, message);
        }
    }
}
