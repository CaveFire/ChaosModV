using Serilog;
using System;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;

namespace TwitchChatVotingProxy.VotingReceiver
{
    /// <summary>
    /// Twitch voting receiver implementation
    /// </summary>
    class TwitchVotingReceiver : IVotingReceiver
    {
        public static readonly int RECONNECT_INTERVAL = 1000;

        public event EventHandler<OnMessageArgs> OnMessage;

        private TwitchVotingReceiverConfig config;
        private ILogger logger = Log.Logger.ForContext<TwitchVotingReceiver>();

        public TwitchVotingReceiver(TwitchVotingReceiverConfig config)
        {
            this.config = config;
        }

        public void SendMessage(string message)
        {
            try
            {
                TwitchChatVotingProxy.overlayServer.Broadcast(message);
                logger.Information($"send message ${message}");
            } catch (Exception e)
            {
                logger.Error(e, $"failed to send message to channel \"{config.ChannelName}\"");
            }
        }
        /// <summary>
        /// Called when the twitch client receives a message
        /// </summary>
        public void OnMessageReceived(String message, String sender)
        {
            var evnt = new OnMessageArgs();
            evnt.Message = message;
            evnt.ClientId = sender;
            evnt.Username = sender; // lower case the username to allow case-insensitive comparisons
            OnMessage.Invoke(this, evnt);
        }
    }
}
