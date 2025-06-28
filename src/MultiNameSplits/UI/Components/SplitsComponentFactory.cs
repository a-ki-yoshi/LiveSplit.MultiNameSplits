using System;

using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(SplitsComponentFactory))]

namespace LiveSplit.UI.Components;

public class SplitsComponentFactory : IComponentFactory
{
    public string ComponentName => "Multi Name Splits";

    public string Description => "Displays a list of split times and deltas, and can automatically switch between multiple split names at set intervals. Optionally shows subsplits when enabled.";

    public ComponentCategory Category => ComponentCategory.List;

    public IComponent Create(LiveSplitState state)
    {
        return new SplitsComponent(state);
    }

    public string UpdateName => ComponentName;

    public string UpdateURL => "https://raw.githubusercontent.com/a-ki-yoshi/LiveSplit.MultiNameSplits/master/docs/";
    public string XMLURL => UpdateURL + "Update.xml";

    public Version Version => Version.Parse("0.0.1");
}
