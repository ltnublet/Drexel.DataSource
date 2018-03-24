namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a binding between a <see cref="IConfigurationRequirement"/> and an <see cref="object"/> which
    /// satisfies its requirements.
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        /// The binding <see cref="IConfigurationRequirement"/>.
        /// </summary>
        IConfigurationRequirement Requirement { get; }

        /// <summary>
        /// The bound <see cref="object"/>.
        /// </summary>
        object Bound { get; }
    }
}
