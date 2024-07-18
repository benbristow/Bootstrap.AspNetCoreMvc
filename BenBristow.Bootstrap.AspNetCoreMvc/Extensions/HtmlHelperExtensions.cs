using System.Linq.Expressions;
using System.Text.Encodings.Web;
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
    /// Generates a Bootstrap-styled input element for the specified model expression.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The <see cref="IHtmlHelper{TModel}"/> instance this method extends.</param>
    /// <param name="expression">An expression that identifies the property to display.</param>
    /// <param name="inputType">The type of the input element (e.g., "text", "email"). Defaults to "text".</param>
    /// <param name="placeholder">The placeholder text for the input element. Defaults to an empty string.</param>
    /// <param name="attributes">An object that contains the HTML attributes to set for the element. Optional.</param>
    /// <returns>An <see cref="IHtmlContent"/> that represents the rendered input element.</returns>
    /// <remarks>
    /// This method generates an input element with Bootstrap styling applied. It supports custom HTML attributes,
    /// which can be used to override the default attributes or add new ones.
    /// </remarks>
    public static IHtmlContent BootstrapInputFor<TModel, TValue>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TValue>> expression,
        string inputType = "text",
        string placeholder = "",
        object? attributes = null)
    {
        var isRequired = htmlHelper.ViewData.ModelExplorer.GetExplorerForProperty(htmlHelper.NameFor(expression)).Metadata.IsRequired;

        var defaultHtmlAttributes = new
        {
            placeholder,
            @class = "form-control" + (isRequired ? " required" : "")
        };

        var defaultAttributesDict = HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributes);
        var customAttributesDict = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);

        var finalAttributes = new RouteValueDictionary(defaultAttributesDict);
        foreach (var attr in customAttributesDict)
            finalAttributes[attr.Key] = attr.Value;

        var input = inputType.Equals("textarea", StringComparison.OrdinalIgnoreCase)
            ? htmlHelper.TextAreaFor(expression, finalAttributes)
            : htmlHelper.TextBoxFor(expression, finalAttributes);

        var writer = new StringWriter();
        htmlHelper.LabelFor(expression, new { @class = "form-label" }).WriteTo(writer, HtmlEncoder.Default);
        input.WriteTo(writer, HtmlEncoder.Default);
        htmlHelper.ValidationMessageFor(expression, string.Empty, new { @class = "text-danger" }).WriteTo(writer, HtmlEncoder.Default);

        return new HtmlString(writer.ToString());
    }
}