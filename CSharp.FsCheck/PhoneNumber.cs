using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharp.FsCheck
{
    public class PhoneNumber
    {
        public int CountryCode { get; private set; }
        public int IdentificationCode { get; private set; }
        public int SubscriberNumber { get; private set; }

        private PhoneNumber() { }

        private PhoneNumber(int countryCode, int identificationCode, int subscriberNumber)
        {
            CountryCode = countryCode;
            IdentificationCode = identificationCode;
            SubscriberNumber = subscriberNumber;
        }

        public static bool TryParse(string number, out PhoneNumber ph)
        {
            var reg = new Regex(@"\+(?<cc>\d{1,3}) (?<ic>\d+)\s*(?<sn>\d+)");
            if (!reg.IsMatch(number))
            {
                ph = null;
                return false;
            }
            var match = reg.Match(number);
            var countryCode = int.Parse(match.Groups["cc"].Value);
            var identificationCode = int.Parse(match.Groups["ic"].Value);
            var subscriberNumber = int.Parse(match.Groups["sn"].Value);
            ph = new PhoneNumber(countryCode, identificationCode, subscriberNumber);
            return true;
        }
    }
}
