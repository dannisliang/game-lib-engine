using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class ObjectExtensions {
    
    public static string ToJson(this object inst) {
        if (inst == null) {
            return null;
        }

        try {
            return Engine.Data.Json.JsonMapper.ToJson(inst).FilterJson();
        }
        catch (Exception e) {
            LogUtil.LogError("ToJson:FAILED:" + e);
            return null;
        }
    }

    public static string ToJson<T>(this T inst) {
        if (inst == null) {
            return null;
        }
        
        try {
            return Engine.Data.Json.JsonMapper.ToJson(inst).FilterJson();
        }
        catch (Exception e) {
            LogUtil.LogError("ToJson:FAILED:" + e);
            return null;
        }
    }
    
    public static T FromJson<T>(this string inst) {
        return Engine.Data.Json.JsonMapper.ToObject<T>(inst.FilterJson());
    }
    
    public static object FromJson(this string inst) {
        return Engine.Data.Json.JsonMapper.ToObject<object>(inst.FilterJson());
    }

    //

    public static T ToDataObject<T>(this object val) {
        if (val == null) {
            return default(T);
        }
        return val.ToJson().FromJson<T>();
    }

    public static string FilterJson(this string val) {
        if (string.IsNullOrEmpty(val))
            return val;
        return val
                .Replace("\\\"", "\"")
                .TrimStart('"').TrimEnd('"');
    }
    
    public static object ConvertJson(this string val) {
        if (val.StartsWith("{") || val.StartsWith("[")) {
            try {
                
                if (val.TrimStart().StartsWith("{")) {
                    return val.FilterJson().FromJson<Dictionary<string, object>>();
                }
                else if (val.TrimStart().StartsWith("[")) {
                    return val.FilterJson().FromJson<List<object>>();
                }
                
            }
            catch (Exception e) {
                UnityEngine.Debug.Log("ERROR parsing attribute:" + e);
            }
        }
        
        return val;
    }

}