using Smooth.Delegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.ComplexPredicates
{
    public struct ComplexPredicate<T, C>
    {
        private C _context;
        private PredicateDelegate<C, T> _previousResolver; 
        private Predicate<T> _predicate;

        internal ComplexPredicate(C context, PredicateDelegate<C, T> previousResolver, Predicate<T> predicate)
        {
            _predicate = predicate;
            _context = context;
            _previousResolver = previousResolver;
        }

        public static ComplexPredicate<T, bool> Create(Predicate<T> predicate)
        {
            return new ComplexPredicate<T, bool>
            {
                _predicate = predicate,
                _context = true,
                _previousResolver = _first
            };
        } 

        public bool Resolve(T value)
        {
            return _previousResolver(ref _context, value) && _predicate(value);
        }


        private static readonly PredicateDelegate<bool, T> _first = First;
        private static bool First(ref bool context, T value)
        {
            return context;
        }
    }

    public delegate bool PredicateDelegate<C, in T>(ref C context, T value);
}