using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct SomeMatcherResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private bool _skip;

        internal static SomeMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            bool isSome)
        {
            return new SomeMatcherResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = !isSome
            };
        }

        public OfOptionMatcherResult<T, TMatcher, TResult> Of(T value)
        {
            return _skip
                ? OfOptionMatcherResult<T, TMatcher, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfOptionMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, value);
        }

        public WhereOptionMatcherResult<T, TMatcher, TResult> Where(Predicate<T> predicate)
        {
            return _skip
                ? WhereOptionMatcherResult<T, TMatcher, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereOptionMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                    predicate);
        }

        public WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult> Where<TPredicateParam>(
            Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return _skip
                ? WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>.CreateSkip(ref _previous,
                    _valueProvider, _evaluator)
                : WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>.Create(ref _previous, _valueProvider,
                    _evaluator, predicate, param);
        }
    }
}