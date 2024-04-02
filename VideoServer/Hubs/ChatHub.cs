using Microsoft.AspNetCore.SignalR;

namespace VideoServer.Hubs
{
    public class ChatHub : Hub
    {
        // Stores username-connectionId pairs for active users
        private static readonly Dictionary<string, string> _connections = [];

        public override async Task OnConnectedAsync()
        {
            // get username from the url
            string username = Context.GetHttpContext()?.Request.Query["username"];

            if (!string.IsNullOrEmpty(username))
            {
                // add newly connected user to _connections dictionary 
                _connections[Context.ConnectionId] = username;
                
                // broadcast a updated list of active users when a new user joins the chat
                var formattedConnections = _connections.Select(c => new { username = c.Value }).ToList();
                await Clients.All.SendAsync("receiveActiveUserList", formattedConnections);
            }
            else
            {
                // send back error message only to the user who initiated the connection
                await Clients.Caller.SendAsync("error", "Error retrieving username");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // if connectionId is in _connections dictionary, remove the connectionId from the _connections
            if (_connections.Remove(Context.ConnectionId, out string username))
            {
                // broadcast a updated list of active users when a user leaves the chat
                var formattedConnections = _connections.Select(c => new { username = c.Value }).ToList();
                await Clients.All.SendAsync("receiveActiveUserList", formattedConnections);
            } 
            else
            {
                // send back error message only to the user who initiated the connection
                await Clients.Caller.SendAsync("error", "Error retrieving username");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            // if sender's connectionId is in _connections dictionary, broadcast the new message 
            if (_connections.TryGetValue(Context.ConnectionId, out string username))
            {
                await Clients.All.SendAsync("receiveMessage", username, message);
            } 
            else
            {
                // send back error message only to the user who initiated the connection
                await Clients.Caller.SendAsync("error", "Error retrieving username");
            }
        }
    }
}
