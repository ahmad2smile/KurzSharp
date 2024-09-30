#if NET8_0_OR_GREATER
using Variant = MudBlazor.Variant;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
#endif

namespace KurzSharp.Templates.AdminDashboard;

#if NET8_0_OR_GREATER
[Route("/KurzSharp")]
public class HomeComponent : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // <PageTitle>Home</PageTitle>
        builder.OpenComponent<PageTitle>(1);
        builder.AddAttribute(3, "ChildContent",
            (RenderFragment)(builder2 => { builder2.AddContent(4, "KurzSharp"); }));
        builder.CloseComponent();

        // <MudText Class="mb-8">PlaceholderModel Hosts near you</MudText>
        builder.OpenComponent<MudText>(5);
        builder.AddAttribute(6, "Class", "mb-8");
        builder.AddAttribute(7, "ChildContent",
            (RenderFragment)(builder2 => { builder2.AddContent(8, "Dashboard"); }));
        builder.CloseComponent();

        builder.OpenComponent<MudCard>(0);
        builder.AddAttribute(1, "ChildContent", (RenderFragment)(builder2 =>
        {
            // Open MudCardContent component
            builder2.OpenComponent<MudCardContent>(13);
            builder2.AddAttribute(14, "ChildContent", (RenderFragment)(builder3 =>
            {
                // Open first MudText component for content
                builder3.OpenComponent<MudText>(15);
                builder3.AddAttribute(16, "ChildContent",
                    (RenderFragment)(builder4 =>
                    {
                        builder4.AddContent(17,
                            "<- You can find all your models with [AdminDashboard] attributes in the Nav Menu.");
                    }));
                builder3.CloseComponent(); // Close first MudText

                // Open second MudText component with Typo.body2
                builder3.OpenComponent<MudText>(18);
                builder3.AddAttribute(19, "Typo", Typo.body2);
                builder3.AddAttribute(20, "ChildContent",
                    (RenderFragment)(builder4 =>
                    {
                        builder4.AddContent(21, "For more information check KurzSharp Github:");
                    }));
                builder3.CloseComponent(); // Close second MudText
            }));
            builder2.CloseComponent(); // Close MudCardContent

            // Open MudCardActions component
            builder2.OpenComponent<MudCardActions>(22);
            builder2.AddAttribute(23, "ChildContent", (RenderFragment)(builder3 =>
            {
                // Open MudButton component
                builder3.OpenComponent<MudButton>(24);
                builder3.AddAttribute(25, "Variant", Variant.Text);
                builder3.AddAttribute(26, "Color", Color.Primary);
                builder3.AddAttribute(27, "ChildContent",
                    (RenderFragment)(builder4 => { builder4.AddContent(28, "GitHub"); }));
                builder3.AddAttribute(29, "Href", "https://github.com/ahmad2smile/KurzSharp");
                builder3.AddAttribute(29, "Target", "_blank");
                builder3.AddAttribute(29, "Rel", "noopener");
                builder3.CloseComponent(); // Close MudButton
            }));
            builder2.CloseComponent(); // Close MudCardActions
        }));
        builder.CloseComponent(); // Close MudCard
    }
}
#else
public class HomeComponent{}
#endif
