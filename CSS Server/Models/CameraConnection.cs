using CSS_Server.Controllers;
using CSS_Server.Models.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSS_Server.Models
{
    public class CameraConnection
    {
        private readonly WebSocket _webSocket;
        private readonly ILogger<CameraController> _logger;

        public CameraConnection(WebSocket webSocket, ILogger<CameraController> logger)
        {
            this._logger = logger;
            this._webSocket = webSocket;
        }

        public async Task<bool> Close()
        {
            if (_webSocket != null && _webSocket.State != WebSocketState.Aborted)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                    return true;
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogDebug(ex.Message);
                }
                catch (WebSocketException ex)
                {
                    _logger.LogDebug(ex.Message);
                }
            }
            return false;
        }

        public async Task StartReading()
        {
            while (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                Message message = await ReceiveMessageAsync(_webSocket);

                if (message != null)
                    HandleReceivedMessage(message);
            }

            _logger.LogInformation("Stopping reading websocket with state: " + _webSocket?.State);
        }

        private void HandleReceivedMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.LOGIN:
                    _logger.LogDebug("WTF, already accepted camera sent login message again?? JSON Message: {0}", Message.ToJson(message));
                    break;

                case MessageType.FOOTAGE_RESPONSE_ALL:
                    // Call FootageAllReceived event.
                    FootageAllReceived?.Invoke(this, new FootageAllReceivedEventArgs()
                    {
                        Footage = message.Footage // Get footage from response.
                    });
                    break;

                case MessageType.DOWNLOAD_RESPONSE:
                    // footage came in
                    // do not know what to do with it..
                    break;

                default:
                    _logger.LogDebug("WTF, unknow message type '?'", message.Type.ToString());
                    break;
            }
        }

        public bool IsOnline()
        {
            return (_webSocket != null && _webSocket.State == WebSocketState.Open);
        }

        public async Task SendAsync(Message message)
        {
            byte[] dataToSend = Encoding.UTF8.GetBytes(Message.ToJson(message));
            await _webSocket.SendAsync(dataToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        // TODO: returns null, both in case of messaging gone wrong and incorrect message.
        // In case of former, should stop reading. In case of latter, dont handle message
        /// <summary>
        /// Tries to receive a single message from the given websocket.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns>Message if receiving was succesfull, null otherwise.</returns>
        public static async Task<Message> ReceiveMessageAsync(WebSocket webSocket)
        {
            try
            {
                // Create a buffer to hold the size of the message to receive.
                byte[] sizeBuffer = new byte[sizeof(Int32)];
                var result = await webSocket.ReceiveAsync(sizeBuffer, CancellationToken.None);

                // Check if the connection is still valid, and the expected amount of bytes have been received.
                if (result.Count != sizeof(Int32) || result.CloseStatus.HasValue ||
                    result.MessageType == WebSocketMessageType.Close)
                {
                    //TODO: log
                    return null;
                }

                // Create a buffer based on the previously received size and receive the message.
                int size = BitConverter.ToInt32(sizeBuffer);
                byte[] dataBuffer = new byte[size];
                result = await webSocket.ReceiveAsync(dataBuffer, CancellationToken.None);

                // Check if the connection is still valid, and the expected amount of bytes have been received.
                if (result.Count != size || result.CloseStatus.HasValue ||
                    result.MessageType == WebSocketMessageType.Close)
                {
                    //TODO: log
                    return null;
                }

                // Create a message based on the received string with JSON.
                string json = Encoding.UTF8.GetString(dataBuffer);
                return Message.FromJson(json);
            }
            catch (Exception)
            {
                // TODO: log
                return null;
            }
        }

        #region events
        public event EventHandler<FootageAllReceivedEventArgs> FootageAllReceived;


        #endregion
    }
}
