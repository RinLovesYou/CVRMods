using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace ProtonMediaControlFix
{
    public enum SocketCommand : byte
    {
        PlayPause = 1,
        Skip = 2,
        Prev = 3,
    }
    public static class Playerctl
    {
        private static TcpClient Client = null;
        public static void Connect()
        {
            Client = new TcpClient("localhost", MediaControlFixMod.PlayerCtlPort.Value);
        }
        private static void ExecuteCmd(SocketCommand cmd)
        {
            if (Client == null || !Client.Connected)
                return;
            
            Client.GetStream().WriteByte((byte)cmd);
        }

        public static void PlayPause() => ExecuteCmd(SocketCommand.PlayPause);
        public static void Skip() => ExecuteCmd(SocketCommand.Skip);
        public static void Previous() => ExecuteCmd(SocketCommand.Prev);
    }
}