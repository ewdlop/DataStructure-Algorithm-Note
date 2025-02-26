namespace Algorithm.Claude
{
    /// <summary>
    /// A heartbeat is a periodic signal sent between nodes in a distributed system to indicate that they're operational. If a node fails to send heartbeats, other nodes can detect the failure and take appropriate action. This is fundamental for failure detection in distributed systems.
    /// </summary>
    public class HeartbeatMonitor
    {
        private readonly Dictionary<string, NodeStatus> _nodeStatuses = new Dictionary<string, NodeStatus>();
        private readonly TimeSpan _timeout;
        private readonly Timer _checkTimer;

        public event EventHandler<NodeFailedEventArgs> NodeFailed;

        public HeartbeatMonitor(TimeSpan checkInterval, TimeSpan timeout)
        {
            _timeout = timeout;
            _checkTimer = new Timer(_ => CheckNodes(), null, checkInterval, checkInterval);
        }

        public void RecordHeartbeat(string nodeId)
        {
            lock (_nodeStatuses)
            {
                _nodeStatuses[nodeId] = new NodeStatus
                {
                    LastHeartbeat = DateTime.UtcNow,
                    IsHealthy = true
                };
            }
        }

        private void CheckNodes()
        {
            DateTime now = DateTime.UtcNow;

            lock (_nodeStatuses)
            {
                foreach (var entry in _nodeStatuses)
                {
                    if (entry.Value.IsHealthy && now - entry.Value.LastHeartbeat > _timeout)
                    {
                        entry.Value.IsHealthy = false;
                        NodeFailed?.Invoke(this, new NodeFailedEventArgs { NodeId = entry.Key });
                    }
                }
            }
        }

        public class NodeStatus
        {
            public DateTime LastHeartbeat { get; set; }
            public bool IsHealthy { get; set; }
        }

        public class NodeFailedEventArgs : EventArgs
        {
            public string NodeId { get; set; }
        }
    }
}