using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

// <summary>
// Class that manages the MultiName functionality
// </summary>
public class MultiNameDisplayController
{
    private static readonly Dictionary<string, string> FullToHalfMap = new() {
        { "　", " " }, { "！", "!" }, { "＂", "\"" }, { "＃", "#" }, { "＄", "$" },
        { "％", "%" }, { "＆", "&" }, { "＇", "'" }, { "（", "(" }, { "）", ")" },
        { "＊", "*" }, { "＋", "+" }, { "，", "," }, { "－", "-" }, { "．", "." },
        { "／", "/" }, { "：", ":" }, { "；", ";" }, { "＜", "<" }, { "＝", "=" },
        { "＞", ">" }, { "？", "?" }, { "＠", "@" }, { "［", "[" }, { "＼", "\\" },
        { "］", "]" }, { "＾", "^" }, { "＿", "_" }, { "｀", "`" }, { "｛", "{" },
        { "｜", "|" }, { "｝", "}" }, { "～", "~" }
    };

    private SplitsSettings Settings;
    private int MaxSeparation = 0;
    private int SplitNameIndex = 0;
    private DateTime LastSwitchedTime = DateTime.Now;
    private Dictionary<string, string[]> ArrayCacheDict;
    private Dictionary<string, InfoCache[]> InfoCacheDict;

    private record InfoCache(
        string Text,
        int NextIndex,
        bool IsDifferentPre,
        bool IsDifferentNext
    );
    
    /// <summary>
    /// Converts full-width characters to half-width characters.
    /// Note: Converts a~z, A~Z, 0~9, symbols, and space from full-width to half-width. Full-width katakana characters are not converted.
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <returns>The converted text with half-width characters</returns>
    public static string ConvertFullToHalf(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var result = text;
        
        foreach (var pair in FullToHalfMap)
        {
            result = result.Replace(pair.Key, pair.Value);
        }

        for (int i = 0; i < result.Length; i++)
        {
            char c = result[i];
            if (c >= 0xFF01 && c <= 0xFF5E)
            {
                result = result.Replace(c, (char)(c - 0xFEE0));
            }
        }

        return result;
    }

    public MultiNameDisplayController(SplitsSettings SplitsSettings)
    {
        Settings = SplitsSettings;
        ArrayCacheDict = new Dictionary<string, string[]>();
        InfoCacheDict = new Dictionary<string, InfoCache[]>();
    }

    public void Reset()
    {
        ArrayCacheDict.Clear();
        InfoCacheDict.Clear();
        SplitNameIndex = 0;
        LastSwitchedTime = DateTime.Now;
        
        //Set the maximum number of split name separations
        MaxSeparation = 0;

        foreach (ISegment split in Settings.CurrentState.Run)
        {
            if (Settings.EnableSubsplits)
            {
                Match match = SplitComponent.SubsplitRegex.Match(split.Name);
                if (match.Success)
                {
                    MaxSeparation = Math.Max(MaxSeparation, GetSplitNameArray(match.Groups[1].Value).Length);
                    MaxSeparation = Math.Max(MaxSeparation, GetSplitNameArray(match.Groups[2].Value).Length);
                    continue;
                }
                else if (split.Name.StartsWith("- "))
                {
                    MaxSeparation = Math.Max(MaxSeparation, GetSplitNameArray(split.Name.Substring(2)).Length);
                    continue;
                }
            }
            MaxSeparation = Math.Max(MaxSeparation, GetSplitNameArray(split.Name).Length);
        }
    }

    private string[] GetSplitNameArray(string originalSplitName)
    {
        //string splitName = ConvertFullToHalf(originalSplitName).Trim();
        string splitName = ConvertFullToHalf(originalSplitName).Trim();

        if (ArrayCacheDict.TryGetValue(splitName, out string[] cachedResult))
        {
            return cachedResult;
        }

        string[] result = splitName.Contains(Settings.MultiNameSeparator)
            ? splitName.Split(new string[] { Settings.MultiNameSeparator }, StringSplitOptions.None).Select(s => s.Trim()).ToArray()
            : new string[] { splitName.Trim() };

        ArrayCacheDict[splitName] = result;
        return result;
    }

    //Returns label information with MultiName functionality applied
    public SimpleLabel ModLabel(LiveSplitState state, SimpleLabel nameLabel)
    {
        double alphaRate = 1;

        if (MaxSeparation > 1)
        {
            DateTime currentTime = DateTime.Now;

            // Get elapsed time (in seconds) since last switch
            double elapsedTime = (currentTime - LastSwitchedTime).TotalSeconds;

            InfoCache cache = null;

            if (InfoCacheDict.TryGetValue(nameLabel.Text, out InfoCache[] cachedResult))
            {
                if (SplitNameIndex >= 0 && SplitNameIndex < cachedResult.Length)
                {
                    cache = cachedResult[SplitNameIndex];
                }
                else
                {
                    Trace.WriteLine($"[MultiNameSplits] Invalid SplitNameIndex: {SplitNameIndex}, Array length: {cachedResult.Length}");
                    SplitNameIndex = 0;
                    cache = cachedResult[0];
                }
            }
            else
            {
                string[] splitNames = GetSplitNameArray(nameLabel.Text);

                InfoCacheDict[nameLabel.Text] = new InfoCache[MaxSeparation];

                for (int index = 0; index < MaxSeparation; index++)
                {
                    int preIndex = index - 1 < 0 ? MaxSeparation - 1 : index - 1;
                    int nextIndex = index >= MaxSeparation - 1 ? 0 : index + 1;
                    string currentText = splitNames.Length > index ? splitNames[index] : splitNames.Last();
                    bool isDifferentPre = currentText != (splitNames.Length > preIndex ? splitNames[preIndex] : splitNames.Last());
                    bool isDifferentNext = currentText != (splitNames.Length > nextIndex ? splitNames[nextIndex] : splitNames.Last());

                    InfoCacheDict[nameLabel.Text][index] = new InfoCache(currentText, nextIndex, isDifferentPre, isDifferentNext);
                }

                if (SplitNameIndex >= 0 && SplitNameIndex < InfoCacheDict[nameLabel.Text].Length)
                {
                    cache = InfoCacheDict[nameLabel.Text][SplitNameIndex];
                }
                else
                {
                    Trace.WriteLine($"[MultiNameSplits] Invalid SplitNameIndex after creation: {SplitNameIndex}, Array length: {InfoCacheDict[nameLabel.Text].Length}");
                    SplitNameIndex = 0;
                    cache = InfoCacheDict[nameLabel.Text][0];
                }
            }
            
            // When switch time has passed
            if (elapsedTime >= Settings.MultiNameDisplayTime)
            {
                // Only perform switch if next text is different from current text
                if (cache.IsDifferentNext)
                {
                    alphaRate = 0;
                }
                SplitNameIndex = cache.NextIndex;
                LastSwitchedTime = currentTime;
            }
            else
            {
                // During fade-in time
                if (cache.IsDifferentPre && elapsedTime < Settings.MultiNameTransitionTime)
                {
                    alphaRate = elapsedTime / Settings.MultiNameTransitionTime;
                }
                // During fade-out time
                else if (cache.IsDifferentNext && elapsedTime > Settings.MultiNameDisplayTime - Settings.MultiNameTransitionTime)
                {
                    alphaRate = (Settings.MultiNameDisplayTime - elapsedTime) / Settings.MultiNameTransitionTime;
                }
                else
                {
                    alphaRate = 1;
                }
            }

            nameLabel.Text = cache.Text;
        }

        Color modColor (Color baseColor)
        {
            return Color.FromArgb((int)(alphaRate * baseColor.A), baseColor);
        }

        nameLabel.ForeColor = modColor(nameLabel.ForeColor);
        nameLabel.ShadowColor = modColor(state.LayoutSettings.ShadowsColor);
        nameLabel.OutlineColor = modColor(state.LayoutSettings.TextOutlineColor);

        return nameLabel;
    }
}
