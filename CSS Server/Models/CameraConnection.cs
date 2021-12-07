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
        private WebSocket _webSocket;
        private ILogger<CameraController> _logger;

        public CameraConnection(WebSocket webSocket, ILogger<CameraController> logger)
        {
            this._logger = logger;
            this._webSocket = webSocket;
        }

        public async Task StartReading()
        {
            
            while (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.CloseStatus.HasValue || result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Received close frame, so closing this conenction", CancellationToken.None);
                    _webSocket = null;
                } else
                {
                    HandleReceivedMessage(result, buffer);
                }
            }
        }

        private void HandleReceivedMessage(WebSocketReceiveResult result, byte[] buffer)
        {
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    HandleTextWebsocketMessage(buffer);
                    break;
                case WebSocketMessageType.Binary:
                    Console.WriteLine("binary message received");
                    break;
            }
        }

        private void HandleTextWebsocketMessage(byte[] buffer)
        {
            //Convert sent data to jObject
            JObject jData = JObject.Parse(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

            if (!jData.TryGetValue("type", out JToken type))
                return;

            MessageType sentMessageType = Enum.Parse<MessageType>(type.ToString());

            switch (sentMessageType)
            {
                case MessageType.LOGIN:
                    _logger.LogInformation("WTF, already accepted camera sent login message again??");
                    break;

                case MessageType.FOOTAGE_RESPONSE_ALL:
                    //Get footage from response.
                    JArray footage_all = new JArray();

                    if(jData.TryGetValue("footage", out JToken temp))
                        footage_all = (JArray)temp;

                    //call footage all received event.
                    FootageAllReceived?.Invoke(this, new FootageAllReceivedEventArgs()
                    {
                        Footage = footage_all
                    });
                    break;

                case MessageType.DOWNLOAD_RESPONSE:
                    // footage came in
                    // do not know what to do with it..
                    break;
                default:
                    _logger.LogInformation("WTF, unknow message type '?'", type.ToString());
                    break;
            }
            

        }

        public bool IsOnline()
        {
            return (_webSocket != null && _webSocket.State == WebSocketState.Open);
        }

        public async Task Send(JObject data)
        {
            byte[] dataToSend = Encoding.UTF8.GetBytes("Hello this is data i will sent");

            await _webSocket.SendAsync(new ArraySegment<byte>(dataToSend, 0, dataToSend.Length), WebSocketMessageType.Text, true, CancellationToken.None);

        }

        #region events
        public event EventHandler<FootageAllReceivedEventArgs> FootageAllReceived;


        #endregion
    }
}
