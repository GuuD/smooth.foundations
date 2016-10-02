using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct NoneMatcher<T, TMatcher>
    {
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private DelegateAction _action;

        public static NoneMatcher<T, TMatcher> Create(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            bool isSome)
        {
            return new NoneMatcher<T, TMatcher>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = isSome
            };
        } 

        internal static readonly ValueProvider<T, NoneMatcher<T, TMatcher>> NoneProvider = GetValue;
        internal static readonly Evaluator<NoneMatcher<T, TMatcher>> NoneEvaluator = Evaluate;

        public OptionMatcher<T, NoneMatcher<T, TMatcher>> Do(DelegateAction action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return OptionMatcher<T, NoneMatcher<T, TMatcher>>.Create(ref this, NoneProvider, NoneEvaluator, _skip);
        }

        public OptionMatcher<T, NoneMatcher<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<TActionParam> action, TActionParam param)
        {
            var proxy = _skip
                ? NoneMatcher<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : NoneMatcher<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = NoneMatcher<T, TMatcher, TActionParam>.NoneProvider;
            var e = NoneMatcher<T, TMatcher, TActionParam>.NoneEvaluator;
            return OptionMatcher<T, NoneMatcher<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, _skip);
        }

        private static bool Evaluate(ref NoneMatcher<T, TMatcher> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }
            matcher._action();
            return true;
        }

        private static void GetValue(ref NoneMatcher<T, TMatcher> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    public struct NoneMatcher<T, TMatcher, TActionParam>
    {
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private DelegateAction<TActionParam> _action;
        private TActionParam _param;

        public static NoneMatcher<T, TMatcher, TActionParam> CreateSkip(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator)
        {
            return new NoneMatcher<T, TMatcher, TActionParam>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        public static NoneMatcher<T, TMatcher, TActionParam> Create(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            DelegateAction<TActionParam> action,
            TActionParam param)
        {
            return new NoneMatcher<T, TMatcher, TActionParam>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = action,
                _param = param
            };
        }

        internal static readonly ValueProvider<T, NoneMatcher<T, TMatcher, TActionParam>> NoneProvider = GetValue;
        internal static readonly Evaluator<NoneMatcher<T, TMatcher, TActionParam>> NoneEvaluator = Evaluate;

        private static bool Evaluate(ref NoneMatcher<T, TMatcher, TActionParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }
            matcher._action(matcher._param);
            return true;
        }

        private static void GetValue(ref NoneMatcher<T, TMatcher, TActionParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}