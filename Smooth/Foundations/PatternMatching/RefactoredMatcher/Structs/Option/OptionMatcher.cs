using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct OptionMatcher<T, TMatcher>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator; 
        private bool _isSome;

        internal static OptionMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
            ValueProvider<T, TMatcher> valueProvider, Evaluator<TMatcher> evaluator, bool isSome)
        {
            return new OptionMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _isSome = isSome
            };
        }

        public SomeMatcher<T, TMatcher> Some()
        {
            return SomeMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public NoneMatcher<T, TMatcher> None()
        {
            return NoneMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public OptionMatcher<T, NoneMatcher<T, TMatcher>> None(DelegateAction action)
        {
            var proxy = NoneMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
            var vp = NoneMatcher<T, TMatcher>.NoneProvider;
            var e = NoneMatcher<T, TMatcher>.NoneEvaluator;
            return OptionMatcher<T, NoneMatcher<T, TMatcher>>.Create(ref proxy, vp, e, _isSome);
        }

        public OptionMatcher<T, NoneMatcher<T, TMatcher, TActionParam>> None<TActionParam>(
            DelegateAction<TActionParam> action, TActionParam param)
        {
            var proxy = _isSome
                ? NoneMatcher<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : NoneMatcher<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = NoneMatcher<T, TMatcher, TActionParam>.NoneProvider;
            var e = NoneMatcher<T, TMatcher, TActionParam>.NoneEvaluator;
            return OptionMatcher<T, NoneMatcher<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, _isSome);
        } 
         
    }
}