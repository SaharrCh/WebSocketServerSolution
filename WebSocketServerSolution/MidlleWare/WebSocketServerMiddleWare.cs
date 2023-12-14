using System.Net.WebSockets;
using System.Text;

namespace WebSocketServerSolution.MidlleWare
{
    public class WebSocketServerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketServerConnectionManager _webSocketServerConnectionManager;
        public WebSocketServerMiddleWare(RequestDelegate next, WebSocketServerConnectionManager webSocketServerConnectionManager)
        {
            _webSocketServerConnectionManager = webSocketServerConnectionManager;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)

        {
            if (context.WebSockets.IsWebSocketRequest)
            {
               
                WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();
                string connID = _webSocketServerConnectionManager.AddSocket(websocket);
                await SendMessageAsync(websocket, connID);

                Console.WriteLine("Socket connected ...");
                await RecieveMessage(websocket, async (result, message) =>
                 {
                     if (result.MessageType == WebSocketMessageType.Text)
                     {
                         Console.WriteLine("Message recieved..");
                         Console.WriteLine($"Message {Encoding.UTF8.GetString(message, 0, result.Count)}");
                         return;
                     }
                     else
                     {
                         Console.WriteLine("Recieve Close..");
                         return;
                     }
                 });
            }
            else
                await _next(context);
        }
        public async Task RecieveMessage(WebSocket ws, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        private async Task SendMessageAsync(WebSocket ws, string msg)
        {
            var buffer = Encoding.UTF8.GetBytes(msg);
            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

       
    }





}

