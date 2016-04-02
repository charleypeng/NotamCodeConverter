using Newtonsoft.Json;

namespace NotamCodeConverter
{
    /// <summary>
    /// CodeModel
    /// </summary>
    public class CodeItem
    {
        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty("id")]
        public int ID { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        [JsonProperty("bianma")]
        public string Code { get; set; }
        /// <summary>
        /// Chinese Character
        /// </summary>
        [JsonProperty("hanzi")]
        public string Character { get; set; }
    }
}
