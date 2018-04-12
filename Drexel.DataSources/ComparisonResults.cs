using System;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents the result of a comparison between <see cref="IUniquelyIdentifiable"/> instances.
    /// </summary>
    [Flags]
    public enum ComparisonResults
    {
        /// <summary>
        /// Indicates that the compared instances were different.
        /// </summary>
        Different = 8,

#pragma warning disable CS0618 // Equivalent is marked as obsolete to dissuade external callers.
        /// <summary>
        /// Indicates that the compared instances were different, but considered equivalent. The meaning of
        /// "equivalent" will vary depending on the implementation of the
        /// <see cref="IUniquelyIdentifiable.Compare(IUniquelyIdentifiable)"/> method used to compute the comparison.
        /// </summary>
        /// <example>
        /// <code>
        /// ComparisonResults result = new ExampleIntWrapper(1).Compare(new ExampleStringWrapper("1");
        /// Console.WriteLine(result.HasFlag(ComparisonResult.Equivalent));
        /// > true
        /// </code>
        /// </example>
        DifferentButEquivalent = ComparisonResults.Different | ComparisonResults.Equivalent,
#pragma warning restore CS0618

        /// <summary>
        /// Indicates that the comparison resulted in an exact match.
        /// </summary>
        [Obsolete("An exact match is also equivalent, so ComparisonResults.Match should be used instead.")]
        ExactMatch = 1,

        /// <summary>
        /// Indicates that the compared instances were considered equivalent.
        /// </summary>
        [Obsolete("If something isn't an exact match, but is equivalent, then it is also Different. Use ComparisonResults.DifferentButEquivalent.")]
        Equivalent = 2,

#pragma warning disable CS0618 // Exact is marked as obsolete to dissuade external callers.
        /// <summary>
        /// Indicates that the comparison resulted in a match which was both exact and equivalent.
        /// </summary>
        Match = ComparisonResults.ExactMatch | ComparisonResults.Equivalent,
#pragma warning restore CS0618

        /// <summary>
        /// Indicates that the comparison should be considered to have invalidated any computed information based on
        /// the instances. The meaning of "invalidation" will vary depending on the implementation of the
        /// <see cref="IUniquelyIdentifiable.Compare(IUniquelyIdentifiable)"/> method used to compute the comparison.
        /// </summary>
        /// <example>
        /// <code>
        /// ExampleIntWrapper first = new ExampleStringWrapper("A");
        /// ExampleIntWrapper second = new ExampleStringWrapper("a");
        ///
        /// string value = first.GetValue(); // `value` now equals "A"
        /// first.ToLowerCase(); // modify the value contained by `first` to be "a"
        ///
        /// ComparisonResults result = first.Compare(second);
        /// Console.WriteLine(result.HasFlag(ComparisonResult.Invalidating));
        /// > true
        /// </code>
        /// Because the result is Invalidating, we know that we can't trust the contents of `value`.
        ///
        /// A real-world example of Invalidating is when a file has been moved. The file hasn't changed (ex. the MD5
        /// hash of the files will be equal), but if the path has been cached, it needs to be invalidated. Thus, a
        /// Comparison can return <code>(ComparisonResults.Match | ComparisonResults.Invalidating)</code>.
        /// </example>
        Invalidating = 4,
    }
}
