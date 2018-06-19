using Newtonsoft.Json;

public class Command
{
    [JsonProperty("setType")]
    public string setType { get; set; }

    [JsonProperty("command")]
    public string command { get; set; }

}
