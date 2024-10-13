using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

#nullable disable
namespace MinaCardsMod.Patches
{
  public class IniFile
  {
    public static Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
    public static Dictionary<string, Dictionary<string, string>> dataOther = new Dictionary<string, Dictionary<string, string>>();

    public IniFile(string iniFile)
    {
      if (!File.Exists(iniFile))
        return;
      IniFile.Load(iniFile);
    }

    public static void Load(string iniFile)
    {
      string key = "";
      foreach (string readAllLine in File.ReadAllLines(iniFile))
      {
        string str = readAllLine.Trim();
        if (!string.IsNullOrWhiteSpace(str) && !str.StartsWith(";") && !str.StartsWith("#"))
        {
          if (str.StartsWith("[") && str.EndsWith("]"))
          {
            key = str.Substring(1, str.Length - 2);
            if (!IniFile.data.ContainsKey(key))
              IniFile.data[key] = new Dictionary<string, string>();
          }
          else
          {
            string[] strArray = str.Split(new char[1]{ '=' }, 2);
            if (strArray.Length == 2 && key != "")
              IniFile.data[key][strArray[0].Trim()] = strArray[1].Trim();
          }
        }
      }
    }

    public static void Save(string iniFile)
    {
      using (StreamWriter streamWriter = new StreamWriter(iniFile))
      {
        foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair1 in IniFile.data)
        {
          streamWriter.WriteLine("[" + keyValuePair1.Key + "]");
          foreach (KeyValuePair<string, string> keyValuePair2 in keyValuePair1.Value)
            streamWriter.WriteLine(keyValuePair2.Key + " = " + keyValuePair2.Value);
          streamWriter.WriteLine();
        }
      }
    }

    public static void Clear() => IniFile.data.Clear();

    public static string GetStringValue(string section, string key, string defaultValue = "")
    {
      return IniFile.data.ContainsKey(section) && IniFile.data[section].ContainsKey(key) ? IniFile.data[section][key] : defaultValue;
    }

    public static float GetFloatValue(string section, string key, float defaultValue = 0.0f)
    {
      float result;
      return float.TryParse(IniFile.GetStringValue(section, key), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : defaultValue;
    }

    public static int GetIntValue(string section, string key, int defaultValue = 0)
    {
      int result;
      return int.TryParse(IniFile.GetStringValue(section, key), out result) ? result : defaultValue;
    }

    public static bool GetBoolValue(string section, string key, bool defaultValue = false)
    {
      switch (IniFile.GetStringValue(section, key).ToLower())
      {
        case "true":
          return true;
        case "false":
          return false;
        default:
          return defaultValue;
      }
    }

    public static Vector2 GetVector2Value(string section, string key, Vector2 defaultValue = default (Vector2))
    {
      string stringValue = IniFile.GetStringValue(section, key);
      if (!string.IsNullOrEmpty(stringValue))
      {
        string[] strArray = stringValue.Replace("(", "").Replace(")", "").Split(',', StringSplitOptions.None);
        float result1;
        float result2;
        if (strArray.Length == 2 && float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) && float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          return new Vector2(result1, result2);
      }
      return defaultValue;
    }

    public static Color GetColorValue(string section, string key, Color defaultValue = default (Color))
    {
      string stringValue = IniFile.GetStringValue(section, key);
      if (!string.IsNullOrEmpty(stringValue))
      {
        string[] strArray = stringValue.Replace("RGBA(", "").Replace(")", "").Split(',', StringSplitOptions.None);
        float result1;
        float result2;
        float result3;
        float result4;
        if (strArray.Length == 4 && float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) && float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2) && float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3) && float.TryParse(strArray[3], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result4))
          return new Color(result1, result2, result3, result4);
      }
      return defaultValue;
    }

    public static void SetValue(string section, string key, string value)
    {
      if (!IniFile.data.ContainsKey(section))
        IniFile.data[section] = new Dictionary<string, string>();
      IniFile.data[section][key] = value;
    }

    public static void SetFloatValue(string section, string key, float value)
    {
      IniFile.SetValue(section, key, value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SetIntValue(string section, string key, int value)
    {
      IniFile.SetValue(section, key, value.ToString());
    }

    public static void SetBoolValue(string section, string key, bool value)
    {
      IniFile.SetValue(section, key, value.ToString().ToLower());
    }

    public static void SetVector2Value(string section, string key, Vector2 value)
    {
      string str = value.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.y.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      IniFile.SetValue(section, key, str);
    }

    public static void SetColorValue(string section, string key, Color value)
    {
      string str = value.r.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.g.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.b.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.a.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      IniFile.SetValue(section, key, str);
    }

    public static void LoadOther(string iniFile)
    {
      string key = "";
      foreach (string readAllLine in File.ReadAllLines(iniFile))
      {
        string str = readAllLine.Trim();
        if (!string.IsNullOrWhiteSpace(str) && !str.StartsWith(";") && !str.StartsWith("#"))
        {
          if (str.StartsWith("[") && str.EndsWith("]"))
          {
            key = str.Substring(1, str.Length - 2);
            if (!IniFile.dataOther.ContainsKey(key))
              IniFile.dataOther[key] = new Dictionary<string, string>();
          }
          else
          {
            string[] strArray = str.Split(new char[1]{ '=' }, 2);
            if (strArray.Length == 2 && key != "")
              IniFile.dataOther[key][strArray[0].Trim()] = strArray[1].Trim();
          }
        }
      }
    }

    public static void ClearOther() => IniFile.dataOther.Clear();

    public static string GetStringValueOther(string section, string key, string defaultValue = "")
    {
      return IniFile.dataOther.ContainsKey(section) && IniFile.dataOther[section].ContainsKey(key) ? IniFile.dataOther[section][key] : defaultValue;
    }

    public static float GetFloatValueOther(string section, string key, float defaultValue = 0.0f)
    {
      float result;
      return float.TryParse(IniFile.GetStringValueOther(section, key), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : defaultValue;
    }

    public static int GetIntValueOther(string section, string key, int defaultValue = 0)
    {
      int result;
      return int.TryParse(IniFile.GetStringValueOther(section, key), out result) ? result : defaultValue;
    }

    public static bool GetBoolValueOther(string section, string key, bool defaultValue = false)
    {
      switch (IniFile.GetStringValueOther(section, key).ToLower())
      {
        case "true":
          return true;
        case "false":
          return false;
        default:
          return defaultValue;
      }
    }

    public static Vector2 GetVector2ValueOther(string section, string key, Vector2 defaultValue = default (Vector2))
    {
      string stringValueOther = IniFile.GetStringValueOther(section, key);
      if (!string.IsNullOrEmpty(stringValueOther))
      {
        string[] strArray = stringValueOther.Replace("(", "").Replace(")", "").Split(',', StringSplitOptions.None);
        float result1;
        float result2;
        if (strArray.Length == 2 && float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) && float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          return new Vector2(result1, result2);
      }
      return defaultValue;
    }

    public static Color GetColorValueOther(string section, string key, Color defaultValue = default (Color))
    {
      string stringValueOther = IniFile.GetStringValueOther(section, key);
      if (!string.IsNullOrEmpty(stringValueOther))
      {
        string[] strArray = stringValueOther.Replace("RGBA(", "").Replace(")", "").Split(',', StringSplitOptions.None);
        float result1;
        float result2;
        float result3;
        float result4;
        if (strArray.Length == 4 && float.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) && float.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2) && float.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3) && float.TryParse(strArray[3], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result4))
          return new Color(result1, result2, result3, result4);
      }
      return defaultValue;
    }

    public static void SetValueOther(string section, string key, string value)
    {
      if (!IniFile.dataOther.ContainsKey(section))
        IniFile.dataOther[section] = new Dictionary<string, string>();
      IniFile.dataOther[section][key] = value;
    }

    public static void SetFloatValueOther(string section, string key, float value)
    {
      IniFile.SetValueOther(section, key, value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SetIntValueOther(string section, string key, int value)
    {
      IniFile.SetValueOther(section, key, value.ToString());
    }

    public static void SetBoolValueOther(string section, string key, bool value)
    {
      IniFile.SetValueOther(section, key, value.ToString().ToLower());
    }

    public static void SetVector2ValueOther(string section, string key, Vector2 value)
    {
      string str = value.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.y.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      IniFile.SetValueOther(section, key, str);
    }

    public static void SetColorValueOther(string section, string key, Color value)
    {
      string str = value.r.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.g.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.b.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + value.a.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      IniFile.SetValueOther(section, key, str);
    }

    public static Vector2 GetVector2ValueRedirect(
      bool redirect,
      string section,
      string key,
      Vector2 defaultValue = default (Vector2))
    {
      return redirect ? IniFile.GetVector2Value(section, key) : IniFile.GetVector2ValueOther(section, key);
    }

    public static Color GetColorValueRedirect(
      bool redirect,
      string section,
      string key,
      Color defaultValue = default (Color))
    {
      return redirect ? IniFile.GetColorValue(section, key) : IniFile.GetColorValueOther(section, key);
    }

    public static bool GetBoolValueRedirect(
      bool redirect,
      string section,
      string key,
      bool defaultValue = false)
    {
      return redirect ? IniFile.GetBoolValue(section, key) : IniFile.GetBoolValueOther(section, key);
    }

    public static int GetIntValueRedirect(
      bool redirect,
      string section,
      string key,
      int defaultValue = 0)
    {
      return redirect ? IniFile.GetIntValue(section, key) : IniFile.GetIntValueOther(section, key);
    }

    public static float GetFloatValueRedirect(
      bool redirect,
      string section,
      string key,
      float defaultValue = 0.0f)
    {
      return redirect ? IniFile.GetFloatValue(section, key) : IniFile.GetFloatValueOther(section, key);
    }

    public static string GetStringValueRedirect(
      bool redirect,
      string section,
      string key,
      string defaultValue = "")
    {
      return redirect ? IniFile.GetStringValue(section, key) : IniFile.GetStringValueOther(section, key);
    }
  }
}