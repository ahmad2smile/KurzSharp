#nullable enable
#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using Microsoft.AspNetCore.Components.Routing;
#endif

namespace KurzSharp.Templates.AdminDashboard;

#if NET8_0_OR_GREATER
public class NavMenuComponent : ComponentBase, IDisposable
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private string? currentUrl;

    protected override void OnInitialized()
    {
        currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<MudNavMenu>(0);
        builder.AddAttribute(1, "ChildContent",
            (RenderFragment)(builder2 =>
            {
                // NOTE: Comment to avoid auto format wrap
                CreateNavLink(builder2, "", "Dashboard");
                CreateNavLink(builder2, "placeholderModel", "PlaceHolderModel");
            }));
        builder.CloseComponent();
    }

    private static void CreateNavLink(RenderTreeBuilder builder2, string href, string navName)
    {
        builder2.OpenComponent<MudNavLink>(2);
        builder2.AddAttribute(3, "Href", $"KurzSharp/{href}");
        builder2.AddAttribute(4, "Match", NavLinkMatch.Prefix);
        builder2.AddAttribute(5, "Icon", Icons.Material.Filled.Dashboard);
        builder2.AddAttribute(6, "ActiveClass", "");
        builder2.AddAttribute(7, "ChildContent", (RenderFragment)(builder3 => { builder3.AddContent(8, navName); }));
        builder2.CloseComponent();
    }
}
#else
public class NavMenuComponent {}
#endif
