using System.Linq;

namespace Raele.Util
{
    public static class GrammarUtils
    {
        public delegate string StringTransformation(string param);

        public static class FP
        {
            public static StringTransformation Compose(params StringTransformation[] sequence)
                => sequence.Length <= 0
                    ? (StringTransformation) (word => word)
                    : (StringTransformation) (word => sequence.Aggregate(word, (final, curr) => curr(final)));
            
            // This class should contain first order functions that uses closures to convert GrammarUtils functions and
            // string methods to StringTransformation functions with predefined arguments
            
            public static StringTransformation Concat(string other)
                => word
                => string.Concat(word, other);
        }

        public static string AttachArticle(string word)
            => GrammarUtils.IsVowel(word[0])
                ? $"an {word}"
                : $"a {word}";

        public static bool IsVowel(char c)
            => c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';

        public static string ToLowerCase(string word)
            => word.ToLower();
    }
}
