using System.ComponentModel;

namespace BenBristow.Bootstrap.AspNetCoreMvc.Enums
{
    /// <summary>
    /// Bootstrap color variants.
    /// </summary>
    public enum Variant
    {
        /// <summary>
        /// The primary variant.
        /// </summary>
        [Description("primary")]
        Primary,

        /// <summary>
        /// The secondary variant.
        /// </summary>
        [Description("secondary")]
        Secondary,

        /// <summary>
        /// The success variant.
        /// </summary>
        [Description("success")]
        Success,

        /// <summary>
        /// The danger variant.
        /// </summary>
        [Description("danger")]
        Danger,

        /// <summary>
        /// The warning variant.
        /// </summary>
        [Description("warning")]
        Warning,

        /// <summary>
        /// The info variant.
        /// </summary>
        [Description("info")]
        Info,

        /// <summary>
        /// The light variant.
        /// </summary>
        [Description("light")]
        Light,

        /// <summary>
        /// The dark variant.
        /// </summary>
        [Description("dark")]
        Dark,
    }
}