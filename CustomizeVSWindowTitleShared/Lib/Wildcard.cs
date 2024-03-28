using System;
using System.Text.RegularExpressions;

namespace ErwinMayerLabs.Lib {
    /// <summary>
    /// Represents a wildcard running on the
    /// <see cref="System.Text.RegularExpressions"/> engine.
    /// http://www.codeproject.com/KB/recipes/wildcardtoregex.aspx
    /// </summary>
    /// <example>
    /// // Get a list of files in the My Documents folder
    /// string[] files = System.IO.Directory.GetFiles(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal));
    /// // Create a new wildcard to search for all .txt files, regardless of case
    /// Wildcard wildcard = new Wildcard("*.txt", RegexOptions.IgnoreCase);
    /// // Print all matching files
    /// foreach(string file in files)
    ///     if(wildcard.IsMatch(file))
    ///         Console.WriteLine(file);
    /// </example>
    public class Wildcard : Regex {
        /// <summary>
        /// Initializes a wildcard with the given search pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        public Wildcard(string pattern)
            : base(WildcardToRegex(pattern)) {}

        /// <summary>
        /// Initializes a wildcard with the given search pattern and options.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">A combination of one or more
        /// <see cref="RegexOptions"/>.</param>
        public Wildcard(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options) {}

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static string WildcardToRegex(string pattern) {
            return "^" + Escape(pattern).
                Replace("\\*", ".*").
                Replace("\\?", ".") + "$";
        }

        public static string GetMatch(string pattern, string haystack) {
            pattern = pattern.Replace("(*)", "7FE1610B02784E98_S_B1EC35E7B0874757").Replace("(?)", "7FE1610B02784E98_Q_B1EC35E7B0874757");
            var m = new Regex(WildcardToRegex(pattern).Replace("7FE1610B02784E98_S_B1EC35E7B0874757", "(.*)").Replace("7FE1610B02784E98_Q_B1EC35E7B0874757", "(.)"), RegexOptions.IgnoreCase).Match(haystack);
            if (!m.Success) throw new InvalidOperationException("No match found.");
            if (m.Groups.Count != 2) throw new InvalidOperationException("Multiple matches found.");
            return m.Groups[1].Value;
        }
    }
}