
using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses
{
    public abstract class HalResponse
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        // means that the property will be ignored when the value is null
        public List<Link> Links { get; set; } = [];
    }


    public class Link
    {
        public required string Href { get; set; }
        public required string Rel { get; set; }
        public required string Type { get; set; }
    }
}
