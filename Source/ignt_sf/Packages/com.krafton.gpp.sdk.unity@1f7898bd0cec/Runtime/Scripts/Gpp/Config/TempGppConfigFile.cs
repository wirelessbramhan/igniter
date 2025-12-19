using System.Collections.Generic;
using Newtonsoft.Json;

public class TempGppConfigFile
{
    [JsonProperty("active_stage")]
    public string ActiveStage;

    [JsonProperty("stages")]
    public List<GppConfig> Stages;
}