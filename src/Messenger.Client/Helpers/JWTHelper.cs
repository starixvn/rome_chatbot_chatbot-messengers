namespace Messenger.Client.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Jose;

    public interface IJwtHelper
    {
        string Encode(string secretKey, IDictionary<string, object> data);

        string Decode(string secretKey, string token);
    }

    public class JwtHelper : IJwtHelper
    {
        public string Decode(string secretKey, string token)
        {
            var signingKey = Encoding.UTF8.GetBytes(secretKey);

            var encodedPayload = JWT.Decode(
                token,
                signingKey,
                JwsAlgorithm.HS512);

            return encodedPayload;
        }

        public string Encode(string secretKey, IDictionary<string, object> data)
        {
            var signingKey = Encoding.UTF8.GetBytes(secretKey);

            var encodedPayload = JWT.Encode(
                data,
                signingKey,
                JwsAlgorithm.HS512);

            return encodedPayload;
        }
    }
}