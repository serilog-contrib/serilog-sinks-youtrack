namespace Serilog.Sinks.YouTrack.Services
{
    internal sealed class YouTrackContracts
    {
        public static readonly YouTrackContracts Issue = new YouTrackContracts("/rest/issue");
        public static readonly YouTrackContracts Auth = new YouTrackContracts("/rest/user/login");
        public static readonly YouTrackContracts AuthCookie = new YouTrackContracts("jetbrains.charisma.main.security.PRINCIPAL");

        public readonly string Value;
        private bool Equals(YouTrackContracts other)
        {
            return string.Equals((string) Value, (string) other.Value);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj)) return false;
            if (object.ReferenceEquals(this, obj)) return true;
            return obj is YouTrackContracts && Equals((YouTrackContracts)obj);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(YouTrackContracts item)
        {
            return item.Value;
        }

        private YouTrackContracts(string value)
        {
            Value = value;
        }
    }
}