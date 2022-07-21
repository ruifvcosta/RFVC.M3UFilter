namespace RFVC.M3UFilter.API
{
    /// <summary>
    /// Parameters to use when calling the M3uFilter API
    /// </summary>
    public class M3uParameter
    {
        public string? Url { get; set; }

        public string? Password { get; set; }

        public string? Type { get; set; }

        public string? Output { get; set; }

        public string? Groups { get; set; }
    }
}
