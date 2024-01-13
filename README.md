# Accessible Blazor Samples

## Samples

### Tabs Pattern

> [!IMPORTANT]  
> Requires an interactive render mode to set in the component where the `TabContainer`component is being used. Ideally
> @rendermode should be called inside `TabContainer` itself, but since this component uses type parameters this is not possible
> currently due to an outstanding but with the Razor compiler involving the usage of @rendermode and @typeparam in the same component.
>
> Issue: https://github.com/dotnet/razor/issues/9683

![blazor-tab-container](https://github.com/david-acker/AccessibleBlazorSamples/assets/26313567/23e89fe3-19c7-4a7b-902c-62ff0dfaf745)

More Information: [ARIA Authoring Practices Guide (APG) - Tabs Pattern](https://www.w3.org/WAI/ARIA/apg/patterns/tabs/)
