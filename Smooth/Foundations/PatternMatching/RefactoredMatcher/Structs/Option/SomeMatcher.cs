using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct SomeMatcher<T, TMatcher>
    {
        private TMatcher _previous;
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private bool _skip;

        internal static SomeMatcher<T, TMatcher> Create(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            bool isSome)
        {
            return new SomeMatcher<T, TMatcher>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = !isSome
            };
        } 

        public OfOptionMatcher<T, TMatcher> Of(T value)
        {
            return _skip
                ? OfOptionMatcher<T, TMatcher>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfOptionMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, value);
        }

        public WhereOptionMatcher<T, TMatcher> Where(Predicate<T> predicate)
        {
            return _skip
                ? WhereOptionMatcher<T, TMatcher>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereOptionMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, predicate);
        }

        public WhereOptionMatcher<T, TMatcher, TPredicateParam> Where<TPredicateParam>(Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return _skip
                ? WhereOptionMatcher<T, TMatcher, TPredicateParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereOptionMatcher<T, TMatcher, TPredicateParam>.Create(ref _previous, _valueProvider, _evaluator, predicate, param);
        }

    }
}