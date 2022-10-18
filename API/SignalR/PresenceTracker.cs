
using System.Collections.Concurrent;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers =
            new ConcurrentDictionary<string, List<string>>();
        public Task<bool> UserConnected(string username, string connectionId)
        {
            var isOnline = false;

            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.TryAdd(username, new List<string> { connectionId });
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            var isOffline = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.TryRemove(username, out _);
                    isOffline = true;
                }
            }
            return Task.FromResult(isOffline);
        }
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.Where(kvp => kvp.Value.Any()).Select(kvp => kvp.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetOrAdd(username, _ => new List<string>());
            }
            return Task.FromResult(connectionIds);

        }
    }
}