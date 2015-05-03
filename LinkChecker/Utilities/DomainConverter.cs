namespace LinkChecker.Utilities
{
    public class DomainConverter
    {
        /// <summary>
        ///   Converts the domain name to IDN (punicode encoded)
        /// </summary>
        /// <param name = "aDomain">the domain name</param>
        /// <returns>domain name punicode encoded</returns>
        public static string ToIdn(string aDomain)
        {

          /*  var parts = aDomain.Split('.');
            aDomain = "";
            foreach (var partuni in parts)
            {
                string partpuny = Punycode.Encode(partuni);
                if (help.Right(partpuny, 1) != "-")
                {
                    partpuny = "xn--" + partpuny;
                }
                else
                {
                    partpuny = partuni;
                }
                aDomain = aDomain + partpuny + ".";
            }
            aDomain = help.Left(aDomain, aDomain.Length - 1);*/
            return aDomain;
        }
    }
}
