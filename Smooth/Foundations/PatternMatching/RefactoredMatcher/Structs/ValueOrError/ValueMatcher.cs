using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct ValueMatcher<T, TMatcher>
    {
        private TMatcher _previous;
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private bool _skip;

        internal static ValueMatcher<T, TMatcher> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            bool isValue)
        {
            return new ValueMatcher<T, TMatcher>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = !isValue
            };
        } 

        public OfValueMatcher<T, TMatcher> Of(T value)
        {
            return _skip
                ? OfValueMatcher<T, TMatcher>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfValueMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, value);
        }

        public WhereValueMatcher<T, TMatcher> Where(Predicate<T> predicate)
        {
            return _skip
                ? WhereValueMatcher<T, TMatcher>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereValueMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, predicate);
        }


        public WhereValueMatcher<T, TMatcher, TPredicateParam> Where<TPredicateParam>(Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return _skip
                ? WhereValueMatcher<T, TMatcher, TPredicateParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereValueMatcher<T, TMatcher, TPredicateParam>.Create(ref _previous, _valueProvider, _evaluator, predicate, param);
        }

    }
}