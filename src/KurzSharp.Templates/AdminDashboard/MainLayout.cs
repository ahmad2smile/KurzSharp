#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
#endif

namespace KurzSharp.Templates.AdminDashboard;

#if NET8_0_OR_GREATER
public class MainLayout : LayoutComponentBase
{
    private bool _drawerOpen = true;
    private MudTheme _theme = new();

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        var customTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Teal.Accent4,
                Secondary = Colors.Green.Accent4,
                Background = Colors.Gray.Lighten5,
                AppbarText = Colors.Shades.Black,
                AppbarBackground = "#f3f6f9",
                DrawerBackground = "#f3f6f9"
            },
            Typography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" }
                }
            }
        };

        _theme = customTheme;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<MudThemeProvider>(0);
        builder.AddAttribute(1, "Theme", _theme);
        builder.CloseComponent();

        builder.OpenComponent<MudPopoverProvider>(2);
        builder.CloseComponent();

        builder.OpenComponent<MudDialogProvider>(3);
        builder.CloseComponent();

        builder.OpenComponent<MudSnackbarProvider>(4);
        builder.CloseComponent();

        builder.OpenComponent<MudLayout>(5);
        builder.AddAttribute(6, "ChildContent", (RenderFragment)((builder2) =>
        {
            // MudAppBar
            builder2.OpenComponent<MudAppBar>(7);
            builder2.AddAttribute(8, "Color", Color.Surface);
            builder2.AddAttribute(9, "Elevation", 0);
            builder2.AddAttribute(10, "ChildContent", (RenderFragment)((builder3) =>
            {
                // MudIconButton (Menu)
                builder3.OpenComponent<MudIconButton>(11);
                builder3.AddAttribute(12, "Icon", Icons.Material.Filled.Menu);
                builder3.AddAttribute(13, "Color", Color.Default);
                builder3.AddAttribute(14, "Edge", Edge.Start);
                builder3.AddAttribute(15, "OnClick", EventCallback.Factory.Create<MouseEventArgs>(this, DrawerToggle));
                builder3.CloseComponent();

                // MudText
                builder3.OpenComponent<MudText>(16);
                builder3.AddAttribute(17, "Typo", Typo.h6);
                builder3.AddAttribute(18, "Color", Color.Dark);
                builder3.AddAttribute(19, "ChildContent",
                    (RenderFragment)((builder4) => { builder4.AddContent(20, "KurzSharp Dashboard"); }));
                builder3.CloseComponent();

                // MudSpacer
                builder3.OpenComponent<MudSpacer>(21);
                builder3.CloseComponent();
            }));
            builder2.CloseComponent(); // MudAppBar

            // MudDrawer
            builder2.OpenComponent<MudDrawer>(25);
            builder2.AddAttribute(26, "Open", _drawerOpen);
            builder2.AddAttribute(27, "OpenChanged",
                EventCallback.Factory.Create<bool>(this, value => _drawerOpen = value));
            builder2.AddAttribute(28, "Variant", DrawerVariant.Responsive);
            builder2.AddAttribute(29, "ClipMode", DrawerClipMode.Always);
            builder2.AddAttribute(30, "Breakpoint", Breakpoint.Md);
            builder2.AddAttribute(31, "Elevation", 0);
            builder2.AddAttribute(32, "ChildContent", (RenderFragment)((builder3) =>
            {
                // NavMenu component
                builder3.OpenComponent<NavMenuComponent>(33);
                builder3.CloseComponent();
            }));
            builder2.CloseComponent(); // MudDrawer

            // MudMainContent
            builder2.OpenComponent<MudMainContent>(34);
            builder2.AddAttribute(35, "Style", "background: var(--mud-palette-drawer-background); min-height: 100vh;");
            builder2.AddAttribute(36, "ChildContent", (RenderFragment)((builder3) =>
            {
                // MudContainer
                builder3.OpenComponent<MudContainer>(37);
                builder3.AddAttribute(38, "MaxWidth", MaxWidth.ExtraExtraLarge);
                builder3.AddAttribute(39, "ChildContent", (RenderFragment)((builder4) =>
                {
                    // MudPaper
                    builder4.OpenComponent<MudPaper>(40);
                    builder4.AddAttribute(41, "Elevation", 1);
                    builder4.AddAttribute(42, "Class", "pl-10 pt-10 rounded-tl-lg");
                    builder4.AddAttribute(42, "Style", "min-height: calc(100vh - 20px - var(--mud-appbar-height));");
                    builder4.AddAttribute(43, "ChildContent", (RenderFragment)((builder5) =>
                    {
                        // @Body
                        builder5.AddContent(44, Body);
                    }));
                    builder4.CloseComponent(); // MudPaper
                }));
                builder3.CloseComponent(); // MudContainer
            }));
            builder2.CloseComponent(); // MudMainContent
        }));
        builder.CloseComponent(); // MudLayout

        // Error UI
        builder.OpenElement(45, "div");
        builder.AddAttribute(46, "id", "blazor-error-ui");
        builder.AddContent(47, "An unhandled error has occurred.");
        builder.OpenElement(48, "a");
        builder.AddAttribute(49, "href", "");
        builder.AddAttribute(50, "class", "reload");
        builder.AddContent(51, "Reload");
        builder.CloseElement();
        builder.OpenElement(52, "a");
        builder.AddAttribute(53, "class", "dismiss");
        builder.AddContent(54, "🗙");
        builder.CloseElement();
        builder.CloseElement();
    }
}
#else
public class MainLayout{}
#endif
