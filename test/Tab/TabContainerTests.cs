using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessibleBlazor.Tab;
using AccessibleBlazor.Tests.Tab;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components.Web;

namespace AccessibleBlazor.Tests;

public sealed class TabContainerTests : TestContext
{
	private readonly string AriaLabelledBy = "tab-list-label";
	private readonly string TabContainerClasses = "tab-container-classes";
	private readonly string TabListClasses = "tab-list-classes";
	private readonly string TabPanelContainerClasses = "tab-panel-container-classes";

	private readonly Dictionary<Tab, TabComponentMetadata> components = new()
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

	private readonly string ActiveTabClasses = "active-tab-classes";
	private readonly string InactiveTabClasses = "inactive-tab-classes";

	private string GetTabClasses(bool isActive) => isActive ? ActiveTabClasses : InactiveTabClasses;

	private IRenderedComponent<TabContainer<Tab>> RenderComponent(Tab activeTab)
	{
		return RenderComponent<TabContainer<Tab>>(parameters => parameters
			.Add(p => p.ActiveTab, activeTab)
			.Add(p => p.AriaLabelledBy, AriaLabelledBy)
			.Add(p => p.Components, components)
			.Add(p => p.TabContainerClasses, TabContainerClasses)
			.Add(p => p.TabListClasses, TabListClasses)
			.Add(p => p.TabPanelContainerClasses, TabPanelContainerClasses)
			.Add(p => p.GetTabClasses, GetTabClasses));
	}

	[Fact]
	public void RendersTabContainer()
	{
		var activeTab = Tab.Tab1;

		var cut = RenderComponent(activeTab);

		var tabList = cut.Find("[role='tablist']");
		Assert.Equal(AriaLabelledBy, tabList.GetAttribute("aria-labelledby"));

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
			Assert.Equal(isActive ? ActiveTabClasses : InactiveTabClasses, tabElement.GetAttribute("class"));
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

	[Theory]
	[InlineData(Tab.Tab1)]
	[InlineData(Tab.Tab2)]
	[InlineData(Tab.Tab3)]
	public async Task OnClick_MovesFocusToClickedTab(Tab clickedTab)
	{
		var cut = RenderComponent(Tab.Tab1);

		AssertTabElement(cut, Tab.Tab1, true);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, false);

		await GetTabElement(cut, clickedTab)
			.ClickAsync(new MouseEventArgs());

		AssertTabElement(cut, Tab.Tab1, clickedTab == Tab.Tab1);
		AssertTabElement(cut, Tab.Tab2, clickedTab == Tab.Tab2);
		AssertTabElement(cut, Tab.Tab3, clickedTab == Tab.Tab3);
	}

	[Fact]
	public async Task OnKeyPress_RightArrow_MovesFocusToNextTab()
	{
		var cut = RenderComponent(Tab.Tab1);

		AssertTabElement(cut, Tab.Tab1, true);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, false);

		await GetTabElement(cut, Tab.Tab1)
			.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

		AssertTabElement(cut, Tab.Tab1, false);
		AssertTabElement(cut, Tab.Tab2, true);
		AssertTabElement(cut, Tab.Tab3, false);

		await GetTabElement(cut, Tab.Tab2)
			.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

		AssertTabElement(cut, Tab.Tab1, false);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, true);

		await GetTabElement(cut, Tab.Tab3)
			.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

		AssertTabElement(cut, Tab.Tab1, true);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, false);
	}

	[Fact]
	public async Task OnKeyPress_LeftArrow_MovesFocusToNextTab()
	{
		var cut = RenderComponent(Tab.Tab1);

		AssertTabElement(cut, Tab.Tab1, true);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, false);

		await GetTabElement(cut, Tab.Tab1)
			.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

		AssertTabElement(cut, Tab.Tab1, false);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, true);

		await GetTabElement(cut, Tab.Tab3)
			.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

		AssertTabElement(cut, Tab.Tab1, false);
		AssertTabElement(cut, Tab.Tab2, true);
		AssertTabElement(cut, Tab.Tab3, false);

		await GetTabElement(cut, Tab.Tab2)
			.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowLeft" });

		AssertTabElement(cut, Tab.Tab1, true);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, false);
	}

	[Theory]
	[InlineData(Tab.Tab1)]
	[InlineData(Tab.Tab2)]
	[InlineData(Tab.Tab3)]
	public async Task OnKeyPress_Home_MovesFocusToFirstTab(Tab startActiveTab)
	{
		var cut = RenderComponent(startActiveTab);

		AssertTabElement(cut, Tab.Tab1, startActiveTab == Tab.Tab1);
		AssertTabElement(cut, Tab.Tab2, startActiveTab == Tab.Tab2);
		AssertTabElement(cut, Tab.Tab3, startActiveTab == Tab.Tab3);

		await GetTabElement(cut, startActiveTab)
			.KeyDownAsync(new KeyboardEventArgs { Key = "Home" });

		AssertTabElement(cut, Tab.Tab1, true);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, false);
	}

	[Theory]
	[InlineData(Tab.Tab1)]
	[InlineData(Tab.Tab2)]
	[InlineData(Tab.Tab3)]
	public async Task OnKeyPress_End_MovesFocusToFirstTab(Tab startActiveTab)
	{
		var cut = RenderComponent(startActiveTab);

		AssertTabElement(cut, Tab.Tab1, startActiveTab == Tab.Tab1);
		AssertTabElement(cut, Tab.Tab2, startActiveTab == Tab.Tab2);
		AssertTabElement(cut, Tab.Tab3, startActiveTab == Tab.Tab3);

		await GetTabElement(cut, startActiveTab)
			.KeyDownAsync(new KeyboardEventArgs { Key = "End" });

		AssertTabElement(cut, Tab.Tab1, false);
		AssertTabElement(cut, Tab.Tab2, false);
		AssertTabElement(cut, Tab.Tab3, true);
	}

	private static IElement GetTabElement(IRenderedComponent<TabContainer<Tab>> cut, Tab tab)
	{
		return cut.Find($"[role='tab'][id='tab-{tab}']");
	}

	private static void AssertTabElement(IRenderedComponent<TabContainer<Tab>> cut, Tab tab, bool isActive)
	{
		var tabElement = cut.Find($"[role='tab'][id='tab-{tab}']");
		Assert.Equal(isActive ? "true" : "false", tabElement.GetAttribute("aria-selected"));

		var tabPanel = cut.Find($"[role='tabpanel'][id='tabpanel-{tab}']");
		Assert.Equal($"display: {(isActive ? "block" : "none")}", tabPanel.GetAttribute("style"));
		Assert.Equal("0", tabPanel.GetAttribute("tabindex"));
	}

	public enum Tab
    {
        Tab1,
        Tab2,
        Tab3,
    }
}
