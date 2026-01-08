#nullable enable

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

    private readonly SplitsSettings settings;
    private int visibleMaxSeparation = 0;
    private int currentIndex = 0;
    private int[] visibleIndices = [];
    private DateTime lastSwitchedTime = DateTime.Now;
    private readonly Dictionary<string, string[]> separatedNamesCacheDict = new ();
    private readonly Dictionary<string, string[]> visibleSeparatedNamesCacheDict = new ();
    private readonly Dictionary<string, InfoCache[]> infoCacheDict = new ();

    private record InfoCache(
        string Text,
        int NextIndex,
        bool IsDifferentPre,
        bool IsDifferentNext,
        float DisplayTime,
        Color? TextColor,
        Font? TextFont
    );

    public int MaxSeparation { get; private set; } = 0;
    
    /// <summary>
    /// Converts full-width characters to half-width characters.
    /// Note: Converts a~z, A~Z, 0~9, symbols, and space from full-width to half-width. Full-width katakana characters are not converted.
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <returns>The converted text with half-width characters</returns>
    public static string ConvertFullToHalf(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

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

    public MultiNameDisplayController(SplitsSettings splitsSettings)
    {
        settings = splitsSettings;
    }

    public void ResetNames()
    {
        MaxSeparation = 0;
        currentIndex = 0;
        separatedNamesCacheDict.Clear();
        lastSwitchedTime = DateTime.Now;

        void calcMax(int arg)
        {
            MaxSeparation = Math.Max(MaxSeparation, arg);
        }
        
        //Set the maximum number of split name separations
         foreach (ISegment split in settings.CurrentState.Run)
        {
            if (settings.EnableSubsplits)
            {
                Match match = SplitComponent.SubsplitRegex.Match(split.Name);
                if (match.Success)
                { 
                    calcMax(GetSeparatedNames(match.Groups[1].Value).Length);
                    calcMax(GetSeparatedNames(match.Groups[2].Value).Length);
                    continue;
                }
                else if (split.Name.StartsWith("-"))
                {
                    calcMax(GetSeparatedNames(split.Name[1..]).Length);
                    continue;
                }
            }

            calcMax(GetSeparatedNames(split.Name).Length);
        }
    }

    public void ResetVisibleNames()
    {
        visibleSeparatedNamesCacheDict.Clear();

        visibleIndices = Enumerable.Range(0, Math.Min(MaxSeparation, settings.MultiNameDetailsList.Count))
            .Where(index => settings.MultiNameDetailsList[index].IsVisible)
            .ToArray();

        if (! settings.AlreadyLoadedMultiNameDetails)
        {
            return;
        } 
        else if (visibleIndices.Length == 0)
        {
            settings.MultiNameDetailsList[0].GetChkIsVisible().Checked = true;
            visibleIndices = [0];
        }

        foreach (var kvp in separatedNamesCacheDict)
        {
            string splitName = kvp.Key;
            string[] separatedNames = kvp.Value;

            var visibleNames = visibleIndices
                .Where(index => index >= 0 && index < separatedNames.Length)
                .Select(index => separatedNames[index])
                .ToArray();
            
            visibleSeparatedNamesCacheDict[splitName] = visibleNames.Length == 0 ? new string[] { "" } : visibleNames;
        }

        visibleMaxSeparation = visibleIndices.Length;

        ResetInfo();
    }

    public void ResetInfo()
    {
        infoCacheDict.Clear();
    }

    private string[] GetSeparatedNames(string originalSplitName)
    {
        string splitName = ConvertFullToHalf(originalSplitName).Trim();

        if (separatedNamesCacheDict.TryGetValue(splitName, out string[] cachedSeparatedNames))
        {
            return cachedSeparatedNames;
        }

        string[] separatedNames = splitName.Contains(settings.MultiNameSeparator)
            ? splitName.Split(new string[] { settings.MultiNameSeparator }, StringSplitOptions.None).Select(s => s.Trim()).ToArray()
            : new string[] { splitName.Trim() };

        separatedNamesCacheDict[splitName] = separatedNames;
        
        return separatedNames;
    }

    //Returns label information with MultiName functionality applied
    public SimpleLabel ModLabel(LiveSplitState state, SimpleLabel nameLabel)
    {
        double alphaRate = 1;
        
        InfoCache? cache = null;

        if (MaxSeparation > 1)
        {

            DateTime currentTime = DateTime.Now;

            // Get elapsed time (in seconds) since last switch
            double elapsedTime = (currentTime - lastSwitchedTime).TotalSeconds;

            if (infoCacheDict.TryGetValue(nameLabel.Text, out InfoCache[] cachedVisibleSeparatedNames))
            {
                if (currentIndex >= 0 && currentIndex < cachedVisibleSeparatedNames.Length)
                {
                    cache = cachedVisibleSeparatedNames[currentIndex];
                }
                else
                {
                    #if DEBUG
                    Trace.WriteLine($"[MultiNameSplits] Invalid currentIndex: {currentIndex}, Array length: {cachedVisibleSeparatedNames.Length}");
                    #endif
                    currentIndex = 0;
                    cache = cachedVisibleSeparatedNames[0];
                }
            }
            else
            {
                string[] separatedNames = visibleSeparatedNamesCacheDict[ConvertFullToHalf(nameLabel.Text).Trim()];

                infoCacheDict[nameLabel.Text] = new InfoCache[visibleMaxSeparation];

                for (int index = 0; index < visibleMaxSeparation; index++)
                {
                    int preIndex = index - 1 < 0 ? visibleMaxSeparation - 1 : index - 1;
                    int nextIndex = index >= visibleMaxSeparation - 1 ? 0 : index + 1;
                    string currentText = separatedNames.Length > index ? separatedNames[index] : separatedNames.Last();
                    bool isDifferentPre = currentText != (separatedNames.Length > preIndex ? separatedNames[preIndex] : separatedNames.Last());
                    bool isDifferentNext = currentText != (separatedNames.Length > nextIndex ? separatedNames[nextIndex] : separatedNames.Last());

                    int detailsListIndex = visibleIndices[index];
                    MultiNameDetailsSettings multiNameDetailsSettings = settings.MultiNameDetailsList[detailsListIndex];
                    float displayTime = multiNameDetailsSettings.OverrideDisplayTime ? multiNameDetailsSettings.DisplayTime : settings.MultiNameDisplayTime;
                    Color? textColor = multiNameDetailsSettings.OverrideTextColor ? multiNameDetailsSettings.TextColor : null;
                    Font? textFont = multiNameDetailsSettings.OverrideTextFont ? multiNameDetailsSettings.TextFont : null;
                    
                    infoCacheDict[nameLabel.Text][index] = new InfoCache(currentText, nextIndex, isDifferentPre, isDifferentNext, displayTime, textColor, textFont);
                }

                if (currentIndex >= 0 && currentIndex < infoCacheDict[nameLabel.Text].Length)
                {
                    cache = infoCacheDict[nameLabel.Text][currentIndex];
                }
                else
                {
                    #if DEBUG
                    Trace.WriteLine($"[MultiNameSplits] Invalid currentIndex after creation: {currentIndex}, Array length: {infoCacheDict[nameLabel.Text].Length}");
                    #endif
                    currentIndex = 0;
                    cache = infoCacheDict[nameLabel.Text][0];
                }
            }
            
            // When switch time has passed
            if (elapsedTime >= cache.DisplayTime)
            {
                // Only perform switch if next text is different from current text
                if (cache.IsDifferentNext)
                {
                    alphaRate = 0;
                }

                currentIndex = cache.NextIndex;
                lastSwitchedTime = currentTime;
            }
            else
            {
                // During fade-in time
                if (cache.IsDifferentPre && elapsedTime < settings.MultiNameTransitionTime)
                {
                    alphaRate = elapsedTime / settings.MultiNameTransitionTime;
                }
                // During fade-out time
                else if (cache.IsDifferentNext && elapsedTime > cache.DisplayTime - settings.MultiNameTransitionTime)
                {
                    alphaRate = (cache.DisplayTime - elapsedTime) / settings.MultiNameTransitionTime;
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

        nameLabel.ForeColor = modColor((Color)(cache != null && cache.TextColor != null ? cache.TextColor : nameLabel.ForeColor));
        nameLabel.ShadowColor = modColor(state.LayoutSettings.ShadowsColor);
        nameLabel.OutlineColor = modColor(state.LayoutSettings.TextOutlineColor);
        nameLabel.Font = cache != null && cache.TextFont != null ? cache.TextFont : state.LayoutSettings.TextFont;
        
        return nameLabel;
    }
}
