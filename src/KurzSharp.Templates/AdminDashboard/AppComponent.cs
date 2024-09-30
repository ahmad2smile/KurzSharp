// ReSharper disable RedundantUsingDirective
#nullable enable
#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.StaticInput;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using static Microsoft.AspNetCore.Components.Web.RenderMode;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen.Blazor;
#endif

namespace KurzSharp.Templates.AdminDashboard;

#if NET8_0_OR_GREATER
public class AppComponent : ComponentBase
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private IComponentRenderMode? RenderModeForPage => HttpContext.Request.Path.StartsWithSegments("/Account")
        ? null
        : InteractiveServer;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // <!DOCTYPE html>
        builder.AddMarkupContent(0, "<!DOCTYPE html>");

        // <html lang="en">
        builder.OpenElement(1, "html");
        builder.AddAttribute(2, "lang", "en");

        // <head>
        builder.OpenElement(3, "head");

        // <meta charset="utf-8"/>
        builder.OpenElement(4, "meta");
        builder.AddAttribute(5, "charset", "utf-8");
        builder.CloseElement(); // </meta>

        // <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
        builder.OpenElement(6, "meta");
        builder.AddAttribute(7, "name", "viewport");
        builder.AddAttribute(8, "content", "width=device-width, initial-scale=1.0");
        builder.CloseElement(); // </meta>

        // <base href="/"/>
        builder.OpenElement(9, "base");
        builder.AddAttribute(10, "href", "/");
        builder.CloseElement(); // </base>

        // <link href="..." rel="stylesheet"/>
        builder.OpenElement(11, "link");
        builder.AddAttribute(12, "href", "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap");
        builder.AddAttribute(13, "rel", "stylesheet");
        builder.CloseElement(); // </link>

        // <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet"/>
        builder.OpenElement(14, "link");
        builder.AddAttribute(15, "href", "_content/MudBlazor/MudBlazor.min.css");
        builder.AddAttribute(16, "rel", "stylesheet");
        builder.CloseElement(); // </link>

        // <link rel="icon" type="image/ico" href="favicon.ico"/>
        builder.OpenElement(17, "link");
        builder.AddAttribute(18, "rel", "icon");
        builder.AddAttribute(19, "type", "image/ico");
        builder.AddAttribute(20, "href", "favicon.ico");
        builder.CloseElement(); // </link>

        // <HeadOutlet @rendermode="RenderModeForPage" />
        builder.OpenComponent<HeadOutlet>(21);
        builder.AddComponentRenderMode(RenderModeForPage);
        builder.CloseComponent();

        // <RadzenTheme Theme="material" @rendermode="RenderModeForPage" />
        builder.OpenComponent<RadzenTheme>(23);
        builder.AddAttribute(24, "Theme", "material");
        builder.AddComponentRenderMode(RenderModeForPage);
        builder.CloseComponent();

        builder.CloseElement(); // </head>

        // <body>
        builder.OpenElement(26, "body");

        // <Routes @rendermode="RenderModeForPage" />
        // ReSharper disable once RedundantNameQualifier
        builder.OpenComponent<RoutesComponent>(27);
        builder.AddComponentRenderMode(RenderModeForPage);
        builder.CloseComponent();

        // <script src="_framework/blazor.web.js"></script>
        builder.OpenElement(29, "script");
        builder.AddAttribute(30, "src", "_framework/blazor.web.js");
        builder.CloseElement(); // </script>

        // <script src="_content/MudBlazor/MudBlazor.min.js"></script>
        builder.OpenElement(31, "script");
        builder.AddAttribute(32, "src", "_content/MudBlazor/MudBlazor.min.js");
        builder.CloseElement(); // </script>

        // <script src="_content/Extensions.MudBlazor.StaticInput/NavigationObserver.js"></script>
        builder.OpenElement(33, "script");
        builder.AddAttribute(34, "src", "_content/Extensions.MudBlazor.StaticInput/NavigationObserver.js");
        builder.CloseElement(); // </script>

        // <script src="_content/Radzen.Blazor/Radzen.Blazor.js?v=@(typeof(Radzen.Colors).Assembly.GetName().Version)"></script>
        builder.OpenElement(35, "script");
        var radzenVersion = typeof(Radzen.Colors).Assembly.GetName().Version;
        builder.AddAttribute(36, "src", $"_content/Radzen.Blazor/Radzen.Blazor.js?v={radzenVersion}");
        builder.CloseElement(); // </script>

        builder.CloseElement(); // </body>
        builder.CloseElement(); // </html>
    }
}

#else
public class AppComponent {}
#endif
