namespace CounterPartyAISAApi
{
    public class JwtSettings
    {
        public string Name { get; set; }
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public List<string> Audiences { get; set; }
        public string ValidIssuer { get; set; }
       
        public string ValidAudience { get; set; }
        public int ClockSkewSeconds { get; set; }
    }
}
