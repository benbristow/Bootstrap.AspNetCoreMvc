using System.ComponentModel;

namespace BenBristow.Bootstrap.AspNetCoreMvc.Enums;

/// <summary>
/// Represents margin options for Bootstrap classes.
/// </summary>
public enum Margin
{
    /// <summary>
    /// Represents a margin of 0.
    /// </summary>
    [Description("0")]
    Margin0 = 0,

    /// <summary>
    /// Represents a margin of 1.
    /// </summary>
    [Description("1")]
    Margin1,

    /// <summary>
    /// Represents a margin of 2.
    /// </summary>
    [Description("2")]
    Margin2,

    /// <summary>
    /// Represents a margin of 3.
    /// </summary>
    [Description("3")]
    Margin3,

    /// <summary>
    /// Represents a margin of 4.
    /// </summary>
    [Description("4")]
    Margin4,

    /// <summary>
    /// Represents a margin of 5.
    /// </summary>
    [Description("5")]
    Margin5,

    /// <summary>
    /// Represents an auto margin.
    /// </summary>
    [Description("auto")]
    Auto,
}