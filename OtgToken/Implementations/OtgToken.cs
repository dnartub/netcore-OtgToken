using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OtgToken
{
    /// <summary>
    /// One-time generated Token
    /// 1-2 Minutes liftime
    /// </summary>
    public class OtgToken : IOtgToken
    {
        private static char[] _digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static int _tokenLength = 32;

        /// <summary>
        /// Generate token string string of numbers
        /// * publicKey and DateTime.Now using for generation
        /// </summary>
        public string Generate(string publicKey)
        {
            var date = DateTime.Now;
            var template = GetTemplate(date, publicKey);

            var result = GetDigits(template, _tokenLength);
            return result;
        }

        /// <summary>
        /// Validate token
        ///  * publicKey and DateTime.Now using for validation
        /// </summary>
        public bool Validate(string token, string publicKey)
        {
            var date = DateTime.Now;
            if (Validate(date, token, publicKey))
            {
                return true;
            }
            else if (Validate(date.AddMinutes(-1), token, publicKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generate Http-headers for http-request
        /// * key using for generate publicKey
        /// </summary>
        public Dictionary<string, string> GetOtgTokenHeaders(string key)
        {
            var requestId = Guid.NewGuid().ToString();

            return new Dictionary<string, string>()
            {
                {ValidateOtgTokenAttribute.OtgKeyHeaderName, key},
                {ValidateOtgTokenAttribute.OtgTokenHeaderName, Generate(key + requestId)},
                {ValidateOtgTokenAttribute.OtgRequestIdHeaderName, requestId}
            };
        }

        private bool Validate(DateTime date, string token, string publicKey)
        {
            var template = GetTemplate(date, publicKey);
            var newToken = GetDigits(template, _tokenLength);

            if (newToken == token)
            {
                return true;
            }

            return false;
        }

        // TODO: Modify this template for unique hashing algorithm
        private string GetTemplate(DateTime date, string publicKey)
        {
            return $"jkkhkjfh&*&*{date.ToString("MM")}LKJ*UY*k{date.ToString("mm")}H&*h878h{publicKey}df{date.ToString("dd")}&*YH{date.ToString("HH")}(*JHHB{date.ToString("yyyy")}J_xcfcd23";
        }

        // TODO: Modify this function for unique hashing algorithm
        private string GetDigits(string template, int count)
        {
            var hash = GetHash(template);
            var digitsCharArray = hash.Where(@char => _digits.Contains(@char)).ToArray();
            var digits = new string(digitsCharArray);
            digits = Truncate(digits, count);

            if (digits.Length == count)
            {
                return digits;
            }
            else
            {
                return digits + GetDigits(template + hash, count - digits.Length);
            }
        }

        // TODO: Modify this function for unique hashing algorithm
        public string GetHash(string template)
        {
            var data = Encoding.UTF8.GetBytes(template);
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            var result = string.Concat(hash.Select(x => x.ToString("X2")));
            return result;
        }

        public string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

}
