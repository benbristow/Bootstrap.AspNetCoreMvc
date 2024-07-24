using System.ComponentModel;

namespace BenBristow.Bootstrap.AspNetCoreMvc.Enums;

/// <summary>
/// Represents the size options for Bootstrap components.
/// </summary>
public enum Size
{
    /// <summary>
    /// The default size.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The small size
    /// </summary>
    [Description("sm")]
    Small = 1,

    /// <summary>
    /// The large size
    /// </summary>
    [Description("lg")]
    Large = 2
}