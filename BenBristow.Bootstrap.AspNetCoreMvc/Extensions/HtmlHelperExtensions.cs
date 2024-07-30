using System.Linq.Expressions;
using BenBristow.Bootstrap.AspNetCoreMvc.Enums;
using BenBristow.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace BenBristow.Bootstrap.AspNetCoreMvc.Extensions;

/// <summary>
///     Provides a set of methods for generating HTML elements with Bootstrap styling.
/// </summary>
public static class HtmlHelperExtensions
{
    /// <summary>
    ///     Generates a Bootstrap-styled alert element with the specified message, variant, and dismissible option.
    /// </summary>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper" /> instance this method extends.</param>
    /// <param name="message">The message to display in the alert.</param>
    /// <param name="variant">The Bootstrap variant of the alert. Defaults to Variant.Primary.</param>
    /// <param name="dismissible">Specifies whether the alert is dismissible. Defaults to false.</param>
    /// <returns>An <see cref="IHtmlContent" /> that represents the rendered alert element.</returns>
    public static IHtmlContent BootstrapAlert(
        this IHtmlHelper htmlHelper,
        string message,
        Variant variant = Variant.Primary,
        bool dismissible = false)
    {
        var tagBuilder = new TagBuilder("div");
        tagBuilder.AddCssClass($"alert alert-{variant.GetDescription()}");
        tagBuilder.Attributes["role"] = "alert";

        if (dismissible)
        {
            tagBuilder.AddCssClass("alert-dismissible");

            var dismissButton = new TagBuilder("button");
            dismissButton.AddCssClass("btn-close");
            dismissButton.Attributes["type"] = "button";
            dismissButton.Attributes["data-bs-dismiss"] = "alert";
            dismissButton.Attributes["aria-label"] = "Close";

            tagBuilder.InnerHtml.AppendHtml(dismissButton);
        }

        tagBuilder.InnerHtml.Append(message);

        return tagBuilder;
    }

    /// <summary>
    ///     Generates a Bootstrap styled pagination component.
    /// </summary>
    /// <param name="htmlHelper">The IHtmlHelper instance this method extends.</param>
    /// <param name="currentPage">The current active page number.</param>
    /// <param name="totalPages">The total number of pages.</param>
    /// <param name="pageUrl">A function that generates the URL for each page number.</param>
    /// <param name="maxVisiblePages">The maximum number of page numbers to display. Defaults to 5.</param>
    /// <param name="showFirstLast">Determines whether to show "First" and "Last" buttons. Defaults to true.</param>
    /// <param name="firstText">The text for the "First" button. Defaults to "«".</param>
    /// <param name="lastText">The text for the "Last" button. Defaults to "»".</param>
    /// <param name="previousText">The text for the "Previous" button. Defaults to "‹".</param>
    /// <param name="nextText">The text for the "Next" button. Defaults to "›".</param>
    /// <param name="size">The size of the pagination component. Defaults to Size.Default.</param>
    /// <returns>An IHtmlContent representing the rendered pagination component.</returns>
    /// <remarks>
    ///     This method creates a responsive pagination component that adjusts based on the current page and total pages.
    ///     It includes "Previous" and "Next" buttons, as well as optional "First" and "Last" buttons.
    ///     The pagination adapts to show a sensible range of page numbers around the current page.
    /// </remarks>
    /// <example>
    ///     Usage in a Razor view:
    ///     <code>
    /// @Html.BootstrapPagination(
    ///     currentPage: Model.CurrentPage,
    ///     totalPages: Model.TotalPages,
    ///     pageUrl: pageNumber => Url.Action("Index", new { page = pageNumber }),
    ///     maxVisiblePages: 7,
    ///     size: Size.Small
    /// )
    /// </code>
    /// </example>
    public static IHtmlContent BootstrapPagination(
        this IHtmlHelper htmlHelper,
        int currentPage,
        int totalPages,
        Func<int, string> pageUrl,
        int maxVisiblePages = 5,
        bool showFirstLast = true,
        string firstText = "«",
        string lastText = "»",
        string previousText = "‹",
        string nextText = "›",
        Size size = Size.Default)
    {
        if (totalPages <= 1)
            return HtmlString.Empty;

        var ulTag = new TagBuilder("ul");
        ulTag.AddCssClass("pagination");

        if (size != Size.Default)
            ulTag.AddCssClass($"pagination-{size.GetDescription()}");

        // Add First and Previous buttons
        if (showFirstLast)
            ulTag.InnerHtml.AppendHtml(CreatePaginationItem(1, firstText, currentPage > 1, pageUrl));
        ulTag.InnerHtml.AppendHtml(CreatePaginationItem(currentPage - 1, previousText, currentPage > 1, pageUrl));

        // Calculate visible page range
        var startPage = Math.Max(1, currentPage - maxVisiblePages / 2);
        var endPage = Math.Min(totalPages, startPage + maxVisiblePages - 1);

        if (endPage - startPage + 1 < maxVisiblePages)
            startPage = Math.Max(1, endPage - maxVisiblePages + 1);

        // Add numbered page buttons
        for (var i = startPage; i <= endPage; i++)
            ulTag.InnerHtml.AppendHtml(CreatePaginationItem(i, i.ToString(), true, pageUrl, i == currentPage));

        // Add Next and Last buttons
        ulTag.InnerHtml.AppendHtml(CreatePaginationItem(currentPage + 1, nextText, currentPage < totalPages, pageUrl));
        if (showFirstLast)
            ulTag.InnerHtml.AppendHtml(CreatePaginationItem(totalPages, lastText, currentPage < totalPages, pageUrl));

        var navTag = new TagBuilder("nav");
        navTag.InnerHtml.AppendHtml(ulTag);

        return navTag;
    }

    /// <summary>
    ///     Generates a Bootstrap-styled button element with the specified text, variant, and attributes.
    /// </summary>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper" /> instance this method extends.</param>
    /// <param name="text">The text to display on the button.</param>
    /// <param name="variant">The Bootstrap variant of the button. Defaults to Variant.Primary.</param>
    /// <param name="type">The type of the button. Defaults to "button".</param>
    /// <param name="href">The URL the button links to. Optional.</param>
    /// <param name="size">The Bootstrap size of the button. Defaults to Size.Default.</param>
    /// <param name="attributes">An object that contains the HTML attributes to set for the element. Optional.</param>
    /// <returns>An <see cref="IHtmlContent" /> that represents the rendered button element.</returns>
    public static IHtmlContent BootstrapButton(
        this IHtmlHelper htmlHelper,
        string text,
        Variant variant = Variant.Primary,
        string type = "button",
        string? href = null,
        Size size = Size.Default,
        object? attributes = null)
    {
        var tag = string.IsNullOrEmpty(href) ? "button" : "a";

        var baseClass = $"btn btn-{variant.GetDescription()}";
        if (size != Size.Default)
            baseClass += $" btn-{size.GetDescription()}";

        var tagBuilder = new TagBuilder(tag);
        var mergedAttributes = new RouteValueDictionary(attributes)
        {
            ["class"] = baseClass,
        };

        if (string.IsNullOrEmpty(href))
            mergedAttributes["type"] = type;
        else
            mergedAttributes["href"] = href;

        tagBuilder.MergeAttributes(mergedAttributes);
        tagBuilder.InnerHtml.Append(text);

        return tagBuilder;
    }

    /// <summary>
    ///     Creates a Bootstrap styled form input with label and validation message.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper instance.</param>
    /// <param name="expression">An expression that identifies the object that contains the displayed or posted value.</param>
    /// <param name="inputType">The type of the input element. Defaults to "text".</param>
    /// <param name="placeholder">The placeholder text for the input.</param>
    /// <param name="size">The size of the input. Defaults to Size.Default.</param>
    /// <param name="marginBottom">The bottom margin for the input group. Nullable.</param>
    /// <param name="attributes">Additional HTML attributes to apply to the input element.</param>
    /// <returns>An IHtmlContent containing the input group.</returns>
    public static IHtmlContent BootstrapInputFor<TModel, TValue>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TValue>> expression,
        string inputType = "text",
        string placeholder = "",
        Size size = Size.Default,
        Margin? marginBottom = null,
        object? attributes = null)
    {
        var modelExpressionProvider = new ModelExpressionProvider(htmlHelper.MetadataProvider);
        var modelExpression = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
        var metadata = modelExpression.Metadata;

        // Prepare attributes
        var inputClass = "form-control";
        if (size != Size.Default)
            inputClass += $" form-control-{size.GetDescription()}";

        // Add validation class if necessary
        var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(modelExpression.Name);
        if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out var modelState))
        {
            var validationClass = GetValidationClass(modelState);
            if (!string.IsNullOrEmpty(validationClass))
                inputClass += " " + validationClass;
        }

        // Build attributes
        var defaultAttributes = new RouteValueDictionary(
            new
            {
                placeholder,
                @class = inputClass,
            });

        if (metadata.IsRequired)
            defaultAttributes["required"] = "required";

        if (attributes != null)
        {
            var customAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
            foreach (var attr in customAttributes)
                defaultAttributes[attr.Key] = attr.Value;
        }

        // Create input
        var input = inputType.Equals("textarea", StringComparison.OrdinalIgnoreCase)
            ? htmlHelper.TextAreaFor(expression, defaultAttributes)
            : htmlHelper.TextBoxFor(expression, inputType, defaultAttributes);

        // Create label and validation message
        var label = htmlHelper.LabelFor(expression, new { @class = "form-label" });
        var validationMessage = htmlHelper.ValidationMessageFor(expression, string.Empty, new { @class = "invalid-feedback" });

        // Create wrapper div using TagBuilder
        var wrapper = new TagBuilder("div");
        if (marginBottom != null)
            wrapper.AddCssClass($"mb-{marginBottom.GetDescription()}");

        // Combine elements
        wrapper.InnerHtml.AppendHtml(label);
        wrapper.InnerHtml.AppendHtml(input);
        wrapper.InnerHtml.AppendHtml(validationMessage);

        return wrapper;
    }

    private static TagBuilder CreatePaginationItem(int pageNumber, string text, bool enabled, Func<int, string> pageUrl, bool active = false)
    {
        var liTag = new TagBuilder("li");
        liTag.AddCssClass("page-item");
        if (!enabled)
            liTag.AddCssClass("disabled");
        if (active)
            liTag.AddCssClass("active");

        var aTag = new TagBuilder("a");
        aTag.AddCssClass("page-link");
        aTag.Attributes["href"] = enabled ? pageUrl(pageNumber) : "#";
        aTag.InnerHtml.Append(text);

        liTag.InnerHtml.AppendHtml(aTag);
        return liTag;
    }

    private static string GetValidationClass(ModelStateEntry modelState)
    {
        if (modelState.Errors.Any())
            return "is-invalid";

        return modelState.AttemptedValue != null ? "is-valid" : string.Empty;
    }
}