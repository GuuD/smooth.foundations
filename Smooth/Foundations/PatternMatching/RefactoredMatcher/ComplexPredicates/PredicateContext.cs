using Smooth.Delegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.ComplexPredicates
{
    public struct PredicateContext<C, T>
    {
        private static readonly PredicateDelegate<PredicateContext<C, T>, T> _resolve = Resolve;
        private ComplexPredicate<T, C> _previous;

        public static ComplexPredicate<T, PredicateContext<C, T>> Combine(ComplexPredicate<T, C> previous,
            Predicate<T> next)
        {
            var context = new PredicateContext<C, T> {_previous = previous};
            return new ComplexPredicate<T, PredicateContext<C, T>>(context, _resolve, next);
        } 

        private static bool Resolve(ref PredicateContext<C, T> cp, T value)
        {
            return cp._previous.Resolve(value);
        }
    }
}