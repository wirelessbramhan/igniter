using System;
using System.Collections.Generic;
using System.Linq;
using Gpp;
using Gpp.Utils;
using UnityEngine;

[Serializable]
public class GppConfigSO : ScriptableObject
{
    public string activeStage;

    public List<GppConfig> stages;

    private const string FILE_PATH_CONFIG_SO = "GppSDK/GppConfig";
    private static GppConfigSO _configSo;

    public GppConfig ActiveConfig => stages.First(config => config.Stage.Equals(activeStage, StringComparison.OrdinalIgnoreCase));
    
    public GppConfig GetConfigByStage(string stage) => stages.First(config => config.Stage.Equals(stage, StringComparison.OrdinalIgnoreCase));

    public static GppConfigSO LoadFromFile()
    {
        try
        {
            return _configSo ?? ResUtil.LoadScriptableObject<GppConfigSO>(FILE_PATH_CONFIG_SO);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"GppConfigSO LoadFromFile Failed : {ex.Message}");
            return null;
        }
    }

    public static void LoadFromFileAsync(Action<GppConfigSO> callback)
    {
        if (_configSo is null)
        {
            GppSDK.GetCoroutineRunner().Run(ResUtil.LoadScriptableObject<GppConfigSO>(FILE_PATH_CONFIG_SO, (configSo) =>
            {
                _configSo = configSo;
                callback?.Invoke(_configSo);
            }));
        }
        else
        {
            callback?.Invoke(_configSo);
        }
    }

    public static GppConfig GetConfigFromFileByStage(string stage)
    {
        return LoadFromFile().stages.First(config => config.Stage.Equals(stage, StringComparison.OrdinalIgnoreCase));
    }

    public static GppConfig GetActiveConfigFromFile()
    {
        return LoadFromFile()?.ActiveConfig;
    }
}