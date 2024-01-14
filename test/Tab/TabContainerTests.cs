using System.Collections.Generic;
using System.Linq;
using AccessibleBlazor.Tab;
using AccessibleBlazor.Tests.Tab;

namespace AccessibleBlazor.Tests;

public sealed class TabContainerTests : TestContext
{
	[Fact]
	public void RendersTabContainer()
	{
		var ariaLabelledBy = "tab-list-label";
		var activeTab = Tab.Tab1;
		var components = new Dictionary<Tab, TabComponentMetadata>
		{
			[Tab.Tab1] = TabComponentMetadata.Create<SampleTab>(
				new Dictionary<string, object>
				{
					["Description"] = "Tab 1"
				}),
			[Tab.Tab2] =  TabComponentMetadata.Create<SampleTab>(
				new Dictionary<string, object>
				{
					["Description"] = "Tab 2"
				}),
        	[Tab.Tab3] =  TabComponentMetadata.Create<SampleTab>(
            new Dictionary<string, object>
				{
					["Description"] = "Tab 3"
				}),
		};
		var tabContainerClasses = "tab-container-classes";
		var tabListClasses = "tab-list-classes";
		var TabPanelContainerClasses = "tab-panel-container-classes";

		var activeTabClasses = "active-tab-classes";
		var inactiveTabClasses = "inactive-tab-classes";
        string getTabClasses(bool isActive) => isActive ? activeTabClasses : inactiveTabClasses;

        // Arrange
        var cut = RenderComponent<TabContainer<Tab>>(parameters => parameters
			.Add(p => p.ActiveTab, activeTab)
			.Add(p => p.AriaLabelledBy, ariaLabelledBy)
			.Add(p => p.Components, components)
			.Add(p => p.TabContainerClasses, tabContainerClasses)
			.Add(p => p.TabListClasses, tabListClasses)
			.Add(p => p.TabPanelContainerClasses, TabPanelContainerClasses)
			.Add(p => p.GetTabClasses, getTabClasses));

		var tabList = cut.Find("[role='tablist']");
		Assert.Equal(ariaLabelledBy, tabList.GetAttribute("aria-labelledby"));

		var tabs = components.Keys.ToArray();

		var tabElements = cut.FindAll("[role='tablist'] [role='tab']");

		Assert.Equal(tabs.Length, tabElements.Count);
		// Verify that there aren't any other unexpected tab elements rendered.
		Assert.Equal(tabElements.Count, cut.FindAll("[role='tab']").Count);

		for (var i = 0; i < tabs.Length; i++)
		{
			var tab = tabs[i];
			var tabElement = tabElements[i];

			var isActive = tab == activeTab;

			Assert.Equal($"tabpanel-{tab}", tabElement.GetAttribute("aria-controls"));
			Assert.Equal(isActive ? "true" : "false", tabElement.GetAttribute("aria-selected"));
			Assert.Equal(isActive ? activeTabClasses : inactiveTabClasses, tabElement.GetAttribute("class"));
			Assert.Equal($"tab-{tab}", tabElement.GetAttribute("id"));
			Assert.Equal(isActive ? "0" : "-1", tabElement.GetAttribute("tabindex"));
		}

		var tabPanels = cut.FindAll("[role='tabpanel']");
		Assert.Equal(tabs.Length, tabPanels.Count);

		// Verify that the tab panels are not descendents of the tab list.
		Assert.Empty(cut.FindAll("[role='tablist'] [role='tabpanel']"));

		for (var i = 0; i < tabs.Length; i++)
		{
			var tab = tabs[i];
			var tabPanel = tabPanels[i];

			var isActive = tab == activeTab;

			Assert.Equal($"tab-{tab}", tabPanel.GetAttribute("aria-labelledby"));
			Assert.Equal($"tabpanel-{tab}", tabPanel.GetAttribute("id"));
			Assert.Equal($"display: {(isActive ? "block" : "none")}", tabPanel.GetAttribute("style"));
			Assert.Equal("0", tabPanel.GetAttribute("tabindex"));
		}
	}

	private enum Tab
    {
        Tab1,
        Tab2,
        Tab3,
    }
}
