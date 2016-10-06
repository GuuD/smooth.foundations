using System.ComponentModel;
using Smooth.Algebraics;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError;

namespace Smooth.Foundations.PatternMatching
{
    /// <summary>
    /// Defines extension methods for supplying Match() to specific types. Due to the way extension methods are resolved
    /// by the compiler, these are placed "closer" to the calling code (ie with a shorter namespace) than the general 
    /// type extension method to ensure these are chosen in preference to the general one.
    /// </summary>
    public static class SpecificTypeMatcherExtensions
    {
        public static BasicOptionContainer<T> Match<T>(this Option<T> option)
        {
            return BasicOptionContainer<T>.Create(option);
        }

        public static BasicOptionContainerResult<T, TResult> MatchTo<T, TResult>(this Option<T> option)
        {
            return BasicOptionContainerResult<T, TResult>.Create(option);
        }

        public static BasicVoEContainer<T> Match<T>(this ValueOrError<T> voe)
        {
            return BasicVoEContainer<T>.Create(voe);
        }

        public static BasicVoEContainerResult<T, TResult> MatchTo<T, TResult>(this ValueOrError<T> voe)
        {
            return BasicVoEContainerResult<T, TResult>.Create(voe);
        }
    }
}