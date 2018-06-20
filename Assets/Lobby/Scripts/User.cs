using Newtonsoft.Json;

public class User
{
    [JsonProperty("userType")]
    public string userType { get; set; }

    [JsonProperty("userName")]
    public string userName { get; set; }

    [JsonProperty("toggleState")]
    public bool toggleState { get; set; }
}
