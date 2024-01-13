using System.Diagnostics.CodeAnalysis;

namespace AccessibleBlazor.Tab;

public sealed class TabComponentMetadata
{
    public required string AssemblyQualifiedName { get; init; }

    public required IDictionary<string, object>? Parameters { get; init; }
    
    public static TabComponentMetadata Create<TComponent>(IDictionary<string, object>? parameters = null)
        where TComponent : IComponent
    {
        return new TabComponentMetadata(typeof(TComponent), parameters);
    }
    
    [SetsRequiredMembers]
    private TabComponentMetadata(Type componentType, IDictionary<string, object>? parameters = null)
    {
        AssemblyQualifiedName = componentType.AssemblyQualifiedName
            ?? throw new ArgumentException($"The component type {componentType.Name} does not have a fully qualified assembly name.");
        
        Parameters = parameters;
    }
}