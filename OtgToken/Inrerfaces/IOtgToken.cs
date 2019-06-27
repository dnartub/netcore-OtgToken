using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OtgToken
{
    /// <summary>
    /// One-time generated Token
    /// 1-2 Minutes liftime
    /// </summary>
    public interface IOtgToken
    {
        /// <summary>
        /// Generate token string string of numbers
        /// * publicKey and DateTime.Now using for generation
        /// </summary>
        string Generate(string publicKey);
        /// <summary>
        /// Validate token
        ///  * publicKey and DateTime.Now using for validation
        /// </summary>
        bool Validate(string token, string publicKey);

        /// <summary>
        /// Generate Http-headers for http-request
        /// * key using for generate publicKey
        /// </summary>
        Dictionary<string, string> GetOtgTokenHeaders(string publicKey);
    }
}
