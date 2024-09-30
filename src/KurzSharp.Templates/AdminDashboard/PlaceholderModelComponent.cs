#if NET8_0_OR_GREATER
using System.Linq.Expressions;
using KurzSharp.Templates.Database;
using KurzSharp.Templates.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;
using MudBlazor;
using Radzen;
#endif

namespace KurzSharp.Templates.AdminDashboard;

#if NET8_0_OR_GREATER
[Route("/KurzSharp/placeholderModels")]
public class PlaceholderModelComponent : ComponentBase
{
    [Inject]
    public KurzSharpDbContext DbContext { get; set; } = null!;

    private IQueryable<PlaceholderModel> _placeholderModels = null!;

    private IList<PlaceholderModel> _selectedPlaceholderModels = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _placeholderModels = DbContext.PlaceholderModels;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // <PageTitle>Home</PageTitle>
        builder.OpenComponent<PageTitle>(1);
        builder.AddAttribute(3, "ChildContent",
            (RenderFragment)((builder2) => { builder2.AddContent(4, "PlaceholderModels"); }));
        builder.CloseComponent();

        // <MudText Class="mb-8">PlaceholderModel Hosts near you</MudText>
        builder.OpenComponent<MudText>(5);
        builder.AddAttribute(6, "Class", "mb-8");
        builder.AddAttribute(7, "ChildContent",
            (RenderFragment)(builder2 => { builder2.AddContent(8, "PlaceholderModels"); }));
        builder.CloseComponent();

        // <RadzenDataGrid ...>
        builder.OpenComponent<RadzenDataGrid<PlaceholderModel>>(9);
        builder.AddAttribute(9, "AllowFiltering", true);
        builder.AddAttribute(10, "AllowColumnResize", true);
        builder.AddAttribute(11, "AllowAlternatingRows", false);
        builder.AddAttribute(12, "FilterMode", FilterMode.Advanced);
        builder.AddAttribute(13, "AllowSorting", true);
        builder.AddAttribute(14, "PageSize", 5);
        builder.AddAttribute(15, "AllowPaging", true);
        builder.AddAttribute(16, "PagerHorizontalAlign", HorizontalAlign.Left);
        builder.AddAttribute(17, "ShowPagingSummary", true);
        builder.AddAttribute(18, "Data", _placeholderModels);
        builder.AddAttribute(19, "ColumnWidth", "300px");
        builder.AddAttribute(20, "LogicalFilterOperator", LogicalFilterOperator.Or);
        builder.AddAttribute(21, "SelectionMode", DataGridSelectionMode.Single);
        builder.AddAttribute(22, "Value", _selectedPlaceholderModels);
        builder.AddAttribute(23, "ValueChanged",
            EventCallback.Factory.Create<IList<PlaceholderModel>>(this, value => _selectedPlaceholderModels = value));
        builder.AddAttribute(24, "ValueExpression",
            (Expression<Func<IList<PlaceholderModel>>>)(() => _selectedPlaceholderModels));

        builder.AddAttribute(25, "Columns", (RenderFragment)(columnsBuilder =>
        {
            var seq2 = 0;

            AddColumn(nameof(PlaceholderModel.Id), "Id", columnsBuilder, ref seq2);
        }));

        builder.CloseComponent();
    }

    private static void AddColumn(string property, string title, RenderTreeBuilder columnsBuilder,
        ref int seq)
    {
#pragma warning disable ASP0006
        columnsBuilder.OpenComponent<RadzenDataGridColumn<PlaceholderModel>>(seq++);
        columnsBuilder.AddAttribute(seq++, "Property", property);
        columnsBuilder.AddAttribute(seq++, "Title", title);
        columnsBuilder.CloseComponent();
#pragma warning restore ASP0006
    }
}
#else
public class PlaceholderModelComponent{}
#endif
