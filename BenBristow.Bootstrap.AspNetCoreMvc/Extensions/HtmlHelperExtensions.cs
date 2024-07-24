using System.Linq.Expressions;
using System.Text.Encodings.Web;
using BenBristow.Bootstrap.AspNetCoreMvc.Enums;
using BenBristow.Extensions;
using Microsoft.AspNetCore.Html;
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
    /// <param name="attributes">An object that contains the HTML attributes to set for the element. Optional.</param>
    /// <returns>An <see cref="IHtmlContent"/> that represents the rendered button element.</returns>
    public static IHtmlContent BootstrapButton(
        this IHtmlHelper htmlHelper,
        string text,
        Variant variant = Variant.Primary,
        string type = "button",
        string? href = null,
        object? attributes = null)
    {
        var tag = string.IsNullOrEmpty(href) ? "button" : "a";
        var baseClass = $"btn btn-{variant.GetDescription()}";

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
    /// Generates a Bootstrap-styled input element for the specified model expression.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper{TModel}"/> instance this method extends.</param>
    /// <param name="expression">An expression that identifies the model property.</param>
    /// <param name="inputType">The type of the input element. Defaults to "text".</param>
    /// <param name="placeholder">The placeholder text for the input element. Optional.</param>
    /// <param name="attributes">An object that contains the HTML attributes to set for the element. Optional.</param>
    /// <returns>An <see cref="IHtmlContent"/> that represents the rendered input element.</returns>
    public static IHtmlContent BootstrapInputFor<TModel, TValue>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TValue>> expression,
        string inputType = "text",
        string placeholder = "",
        object? attributes = null)
    {
        var modelExplorer = htmlHelper.ViewData.ModelExplorer.GetExplorerForProperty(htmlHelper.NameFor(expression));
        var isRequired = modelExplorer.Metadata.IsRequired;

        // Prepare attributes
        var defaultAttributes = new RouteValueDictionary(
            new
            {
                placeholder,
                @class = "form-control" + (isRequired ? " required" : "")
            });


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
        var validationMessage = htmlHelper.ValidationMessageFor(expression, string.Empty, new { @class = "text-danger" });

        // Combine elements
        var writer = new StringWriter();
        label.WriteTo(writer, HtmlEncoder.Default);
        input.WriteTo(writer, HtmlEncoder.Default);
        validationMessage.WriteTo(writer, HtmlEncoder.Default);

        return new HtmlString(writer.ToString());
    }
}