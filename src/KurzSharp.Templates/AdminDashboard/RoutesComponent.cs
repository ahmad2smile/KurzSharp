#if NET8_0_OR_GREATER
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
#endif

namespace KurzSharp.Templates.AdminDashboard;

#if NET8_0_OR_GREATER
public class RoutesComponent : ComponentBase
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // <Router AppAssembly="typeof(Program).Assembly">
        builder.OpenComponent<Router>(0);
        builder.AddComponentParameter(1, "AppAssembly", Assembly.GetExecutingAssembly());

        // Found parameter with ChildContent
        // NOTE: Need this name-qualifier as in generated code it causes ambiguous using in ASP.NET Core proj
        // ReSharper disable once RedundantNameQualifier
        builder.AddAttribute(2, "Found", (RenderFragment<Microsoft.AspNetCore.Components.RouteData>)(routeData =>
            builder2 =>
            {
                // <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)">
                builder2.OpenComponent<RouteView>(3);
                builder2.AddAttribute(4, "RouteData", routeData);
                builder2.AddAttribute(5, "DefaultLayout", typeof(MainLayout));

                builder2.CloseComponent(); // </AuthorizeRouteView>

                // <FocusOnNavigate RouteData="routeData" Selector="h1"/>
                builder2.OpenComponent<FocusOnNavigate>(8);
                builder2.AddAttribute(9, "RouteData", routeData);
                builder2.AddAttribute(10, "Selector", "h1");
                builder2.CloseComponent(); // </FocusOnNavigate>
            }));

        builder.CloseComponent(); // </Router>
    }
}

#else
public class RoutesComponent {}
#endif
