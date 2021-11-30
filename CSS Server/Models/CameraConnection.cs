using System.Net.WebSockets;
using System.Text;

namespace CSS_Server.Models
{
    public class CameraConnection
    {
        private WebSocket _webSocket;
        public CameraConnection(WebSocket webSocket)
        {
            this._webSocket = webSocket;
        }

        public async Task StartReading()
        {
            byte[] dataToSend = Encoding.UTF8.GetBytes("Hello this is data i will sent");

            await _webSocket.SendAsync(new ArraySegment<byte>(dataToSend, 0, dataToSend.Length), WebSocketMessageType.Text, true, CancellationToken.None);

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
                    Console.WriteLine("text message received");
                    break;
                case WebSocketMessageType.Binary:
                    Console.WriteLine("binary message received");
                    break;
                case WebSocketMessageType.Close:
                    Console.WriteLine("close message received maar ik verwacht dat hij hier nooit komt aangezien ik hem al eerder controleer");
                    break;
            }
        }

        public bool IsOnline()
        {
            if(_webSocket != null && _webSocket.State == WebSocketState.Open)
                return true;

            return false;
        }
    }
}
