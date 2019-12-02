namespace PandaDoc.Standard
{
    using System.Collections.Generic;

    public class PandaDocBearerToken : Dictionary<string, string>
    {
        public string AccessToken
        {
            get { return this["access_token"]; }
            set { this["access_token"] = value; }
        }

        public string RefreshToken
        {
            get { return this["refresh_token"]; }
            set { this["refresh_token"] = value; }
        }

        public string ApiKey
        {
            get { return this["api_key"]; }
            set { this["api_key"] = value; }
        }
    }
}
