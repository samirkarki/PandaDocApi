using System;
using Newtonsoft.Json;
using PandaDoc.Models.GetDocument;

namespace PandaDoc.Models.SendDocument
{
    public class ShareDocumentRequest
    {
        [JsonProperty("recipient")]
        public string Recipient { get; set; }
        [JsonProperty("lifetime")]
        public int LifeTime { get; set; }
    }

    public class ShareDocumentResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
        public string Recipient { get; set; }


    }
}