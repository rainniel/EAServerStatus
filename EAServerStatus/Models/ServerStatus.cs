namespace EAServerStatus.Models
{
    public class ServerStatus
    {
        public int Online { get; private set; }
        public int ElyosPercentage { get; private set; }
        public int AsmoPercentage { get; private set; }
        public Status Status { get; private set; }
        public bool IsError { get; private set; }

        public ServerStatus(int online, int elyos, int asmodian, Status status)
        {
            Online = online;
            ElyosPercentage = elyos;
            AsmoPercentage = asmodian;
            Status = status;
            IsError = status == Status.DataError || status == Status.ServerError || status == Status.RequestError;
        }

        public ServerStatus(Status status) : this(0, 0, 0, status) { }
    }

    public enum Status
    {
        Null,
        Online,
        ZeroPlayer,
        Maintenance,
        DataError,
        ServerError,
        RequestError
    }
}
