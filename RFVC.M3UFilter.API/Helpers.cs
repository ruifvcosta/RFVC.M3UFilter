using System.Text;

namespace RFVC.M3UFilter.API
{
    public class Helpers
    {
        /// <summary>
        /// Generates a url for retriving a m3ulist with usual parameters
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pass"></param>
        /// <param name="type"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static string GenerateComposedUrl(string? url, string? pass, string? type, string? outp)
        {
            if (string.IsNullOrEmpty(url))
                return "";

            var result = new StringBuilder();
            result.Append(url);
            if (pass != null)
                result.Append($"&password={pass}");
            if (type != null)
                result.Append($"&type={type}");
            if (outp != null)
                result.Append($"&output={outp}");

            return result.ToString();
        }

        /// <summary>
        /// Generate a Stream from a string to save as a file
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
