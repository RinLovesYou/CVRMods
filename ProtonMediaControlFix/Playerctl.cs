using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ABI_RC.Core;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using static ProtonMediaControlFix.MediaControlFixMod;

namespace ProtonMediaControlFix
{
    public enum SocketCommand : byte
    {
        PlayPause = 1,
        Skip = 2,
        Prev = 3,
        Playing = 4,
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
            if (Client is not { Connected: true })
                return;

            Client.GetStream().WriteByte((byte)cmd);
        }

        public static void PlayPause() => ExecuteCmd(SocketCommand.PlayPause);
        public static void Skip() => ExecuteCmd(SocketCommand.Skip);
        public static void Previous() => ExecuteCmd(SocketCommand.Prev);
        
        //scuffed ass TCP, i hate you, this code is more reliable in go :troll:
        public static Task Playing()
        {
            var stream = Client.GetStream();
            List<byte> allData = new List<byte>();
            
            while (Client is { Connected: true })
            {
                //continue until there's actually something to read
                if (!stream.CanRead)
                {
                    continue;
                }
                
                //decent buffer, allows for 512 wchars, if your song title is longer than this, what the fuck?
                var data = new byte[1024];
                
                //read the data
                if (stream.Read(data, 0, data.Length) == data.Length)
                {
                    allData.AddRange(data);
                }
                else if (stream.Read(data, 0, data.Length) > 0)
                {
                    allData.AddRange(data.Take(stream.Read(data, 0, data.Length)));
                }
                
                //nothing here but us chickens
                if (allData.Count <= 0) continue;

                switch (allData[0])
                {
                    case (byte)SocketCommand.Playing:
                    {
                        //remove the command byte
                        allData.Remove(4);
                        
                        //turn to byte array and get string
                        var bytes = allData.ToArray();
                        var metadata = Encoding.Unicode.GetString(bytes);

                        Log.Msg($"Now Playing: {metadata}");
                        
                        //run on main thread to be safe
                        Dispatcher.Enqueue(() =>
                        {
                            CohtmlHud.Instance.ViewDropText("Now Playing", metadata);
                        });
                        
                        //tell the socket we have received the title. Somehow this method drops packets a whole lot,
                        //so we have the socket resend until ack
                        ExecuteCmd(SocketCommand.Playing);
                        
                        allData.Clear();
                        break;
                    }
                }
            }

            return null;
        }
    }
}