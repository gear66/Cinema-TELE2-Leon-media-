using Newtonsoft.Json;

public class Message
{
    [JsonProperty("payload")]
    public Payload payload { get; set; }

    [JsonProperty("command")]
    public Command command { get; set; }

    [JsonProperty("user")]
    public User user { get; set; }

}
