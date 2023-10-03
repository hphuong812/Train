using MessagePack;
using Newtonsoft.Json;

[MessagePackObject()]
public class LevelItemData
{
    [Key(0)]
    [JsonProperty("id")]
    public int Id { get; set; }

    [Key(1)]
    [JsonProperty("levelKey")]
    public string LevelKey { get; set; }

    [Key(2)]
    [JsonProperty("name")]
    public string Name { get; set; }

    [Key(3)]
    [JsonProperty("maxRail")]
    public int MaxRail { get; set; }
}