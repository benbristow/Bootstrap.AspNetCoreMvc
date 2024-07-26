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
/// Provides a set of methods for generating HTML elements with Bootstrap styling.
/// </summary>
public static class HtmlHelperExtensions
{
    /// <summary>
    /// Generates a Bootstrap-styled alert element with the specified message, variant, and dismissible option.
    /// </summary>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper"/> instance this method extends.</param>
    /// <param name="message">The message to display in the alert.</param>
    /// <param name="variant">The Bootstrap variant of the alert. Defaults to Variant.Primary.</param>
    /// <param name="dismissible">Specifies whether the alert is dismissible. Defaults to false.</param>
    /// <returns>An <see cref="IHtmlContent"/> that represents the rendered alert element.</returns>
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
    /// Generates a Bootstrap-styled button element with the specified text, variant, and attributes.
    /// </summary>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper"/> instance this method extends.</param>
    /// <param name="text">The text to display on the button.</param>
    /// <param name="variant">The Bootstrap variant of the button. Defaults to Variant.Primary.</param>
    /// <param name="type">The type of the button. Defaults to "button".</param>
    /// <param name="href">The URL the button links to. Optional.</param>
    /// <param name="size">The Bootstrap size of the button. Defaults to Size.Default.</param>
    /// <param name="attributes">An object that contains the HTML attributes to set for the element. Optional.</param>
    /// <returns>An <see cref="IHtmlContent"/> that represents the rendered button element.</returns>
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
            ["class"] = baseClass
        };

        if (string.IsNullOrEmpty(href))
        {
            mergedAttributes["type"] = type;
        }
        else
        {
            mergedAttributes["href"] = href;
        }

        tagBuilder.MergeAttributes(mergedAttributes);
        tagBuilder.InnerHtml.Append(text);

        return tagBuilder;
    }

    /// <summary>
    /// Creates a Bootstrap styled form input with label and validation message.
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
        var modelExplorer = htmlHelper.ViewData.ModelExplorer.GetExplorerForProperty(htmlHelper.NameFor(expression));
        var isRequired = modelExplorer.Metadata.IsRequired;

        // Prepare attributes
        var inputClass = "form-control";
        if (size != Size.Default)
            inputClass += $" form-control-{size.GetDescription()}";

        // Add validation class if necessary
        var fieldName = htmlHelper.NameFor(expression);
        if (htmlHelper.ViewData.ModelState.TryGetValue(fieldName, out var modelState))
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
                @class = inputClass
            });

        if (isRequired)
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
            : htmlHelper.TextBoxFor(expression, defaultAttributes);

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

    private static string GetValidationClass(ModelStateEntry modelState)
    {
        if (modelState.Errors.Any())
        {
            return "is-invalid";
        }

        return modelState.AttemptedValue != null ? "is-valid" : string.Empty;
    }
}