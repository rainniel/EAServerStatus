namespace EAServerStatus.Models
{
    public class ServerStatus
    {
        public int Online { get; private set; }
        public int ElyosPercentage { get; private set; }
        public int AsmoPercentage { get; private set; }
        public Status Status { get; private set; }

        public ServerStatus(int online, int elyos, int asmodian, Status status)
        {
            Online = online;
            ElyosPercentage = elyos;
            AsmoPercentage = asmodian;
            Status = status;
        }

        public ServerStatus(Status status)
        {
            Status = status;
        }
    }

    public enum Status
    {
        Null,
        Online,
        Offline,
        ZeroPlayer,
        RequestError
    }
}
