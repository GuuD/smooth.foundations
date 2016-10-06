using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct ValueMatcherResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private bool _skip;

        internal static ValueMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            bool isValue)
        {
            return new ValueMatcherResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = !isValue
            };
        }

        public OfValueMatcherResult<T, TMatcher, TResult> Of(T value)
        {
            return _skip
                ? OfValueMatcherResult<T, TMatcher, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfValueMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, value);
        }

        public WhereValueMatcherResult<T, TMatcher, TResult> Where(Predicate<T> predicate)
        {
            return _skip
                ? WhereValueMatcherResult<T, TMatcher, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereValueMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                    predicate);
        }

        public WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult> Where<TPredicateParam>(
            Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return _skip
                ? WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>.CreateSkip(ref _previous,
                    _valueProvider, _evaluator)
                : WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>.Create(ref _previous, _valueProvider,
                    _evaluator, predicate, param);
        }
    }
}