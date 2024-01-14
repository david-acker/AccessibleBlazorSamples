namespace AccessibleBlazor.Tab;

public sealed partial class TabContainer<TTab> where TTab : Enum
{
    [Parameter, EditorRequired]
    public required string AriaLabelledBy { get; set;}

    [Parameter, EditorRequired]
    public required Dictionary<TTab, TabComponentMetadata> Components { get; set; }
    
    [Parameter, EditorRequired]
    public required string TabContainerClasses { get; set; }
    
    [Parameter, EditorRequired] 
    public required string TabListClasses { get; set; }

    [Parameter]
    public string TabPanelContainerClasses { get; set; } = string.Empty;
    
    [Parameter, EditorRequired]
    public required Func<bool, string> GetTabClasses { get; set; }
    
    [Parameter]
    public TTab? ActiveTab { get; set; } 

    private TTab[] Tabs => Components.Keys.ToArray();
    
    ElementReference TabButton { set => _tabButtons.Add(value); }
    private readonly List<ElementReference> _tabButtons = [];

    protected override void OnInitialized()
    {
        ActiveTab ??= Tabs.FirstOrDefault();
    }

    private bool IsActiveTab(TTab tab)
    { 
        return EqualityComparer<TTab>.Default.Equals(tab, ActiveTab);
    }

    private void SetActiveTab(TTab tab) => ActiveTab = tab;

    private Type? GetTabPanelComponentType(TTab tab)
    {
        if (!Components.TryGetValue(tab, out TabComponentMetadata? value))
        {
            return null;
        }

        return Type.GetType(value.AssemblyQualifiedName);
    }

    private IDictionary<string, object>? GetTabPanelComponentParameters(TTab tab)
    {
        if (!Components.TryGetValue(tab, out TabComponentMetadata? value))
        {
            return null;
        }
        
        return value.Parameters;
    }

    private bool _onKeyDownPreventDefault = false;
    
    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        _onKeyDownPreventDefault = true;

        var activeTabIndex  = Array.IndexOf(Tabs, ActiveTab);
        var tabCount = Tabs.Length;
        
        int? newActiveTabIndex = e.Key switch
        {
            "ArrowRight" => (activeTabIndex + 1) % tabCount,
            "ArrowLeft" => (activeTabIndex - 1 + tabCount) % tabCount,
            "Home" => 0,
            "End" => tabCount - 1,
            _ => null
        };

        if (!newActiveTabIndex.HasValue)
        {
            _onKeyDownPreventDefault = false;
            return;
        }

        SetActiveTab(Tabs[newActiveTabIndex.Value]);

        var activeTabPanel = _tabButtons[newActiveTabIndex.Value];
        await activeTabPanel.FocusAsync();
    }
}