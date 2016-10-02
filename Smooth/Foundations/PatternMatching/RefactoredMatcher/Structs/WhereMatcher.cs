using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{
    #region General Without Any Parameters
    public struct WhereMatcher<T, TMatcher>
    {
        internal static WhereMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, Predicate<T> predicate)
        {
            return new WhereMatcher<T, TMatcher>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
        }

        private static readonly ValueProvider<T, WhereMatcher<T, TMatcher>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereMatcher<T, TMatcher>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateAction<T> _action;

        public GeneralMatcher<T, WhereMatcher<T, TMatcher>> Do(DelegateAction<T> action)
        {
            _action = action;
            return GeneralMatcher<T, WhereMatcher<T, TMatcher>>.Create(ref this, WhereValueProvider, WhereEvaluator);
        }

        public GeneralMatcher<T, WhereMatcherParam<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
                                                                                                   TActionParam param)
        {
            var proxy = WhereMatcherParam<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, action, param);
            var vp = WhereMatcherParam<T, TMatcher, TActionParam>.WhereValueProvider;
            var e = WhereMatcherParam<T, TMatcher, TActionParam>.WhereEvaluator;
            return GeneralMatcher<T, WhereMatcherParam<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e);
        }

        private static bool Evaluate(ref WhereMatcher<T, TMatcher> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value);
            if (result)
            {
                matcher._action(value);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcher<T, TMatcher> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Action Parameter
    public struct WhereMatcherParam<T, TMatcher, TActionParam>
    {
        internal static WhereMatcherParam<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, 
                                                           Predicate<T> predicate,
                                                           DelegateAction<T, TActionParam> action,
                                                           TActionParam param)
        {
            return new WhereMatcherParam<T, TMatcher, TActionParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _action = action,
                _param = param
            };
        }

        internal static readonly ValueProvider<T, WhereMatcherParam<T, TMatcher, TActionParam>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereMatcherParam<T, TMatcher, TActionParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _param;

        private static bool Evaluate(ref WhereMatcherParam<T, TMatcher, TActionParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value);
            if (result)
            {
                matcher._action(value, matcher._param);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcherParam<T, TMatcher, TActionParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Predicate Parameter
    public struct WhereMatcher<T, TMatcher, TPredicateParam>
    {
        internal static WhereMatcher<T, TMatcher, TPredicateParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return new WhereMatcher<T, TMatcher, TPredicateParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _param = param
            };
        }

        private static readonly ValueProvider<T, WhereMatcher<T, TMatcher, TPredicateParam>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereMatcher<T, TMatcher, TPredicateParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _param;
        private DelegateAction<T> _action;

        public GeneralMatcher<T, WhereMatcher<T, TMatcher, TPredicateParam>> Do(DelegateAction<T> action)
        {
            _action = action;
            return GeneralMatcher<T, WhereMatcher<T, TMatcher, TPredicateParam>>.Create(ref this, WhereValueProvider, WhereEvaluator);
        }

        public GeneralMatcher<T, WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
                                                                                                                    TActionParam param)
        {
            var proxy = WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.Create(ref _previous,
                _valueProvider, _evaluator,
                _predicate, _param, action, param);
            var vp = WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.WhereValueProvider;
            var e = WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.WhereEvaluator;
            return GeneralMatcher<T, WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>>.Create(
                ref proxy, vp, e);
        }

        private static bool Evaluate(ref WhereMatcher<T, TMatcher, TPredicateParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value, matcher._param);
            if (result)
            {
                matcher._action(value);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcher<T, TMatcher, TPredicateParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Predicate and Action Parameters
    public struct WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
    {
        internal static WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, 
                                                           Predicate<T, TPredicateParam> predicate, 
                                                           TPredicateParam predicateParam,
                                                           DelegateAction<T, TActionParam> action,
                                                           TActionParam actionParam)
        {
            return new WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _predicateParam = predicateParam,
                _action = action,
                _actionParam = actionParam
            };
        }

        internal static readonly ValueProvider<T, WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _actionParam;

        private static bool Evaluate(ref WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }
            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value, matcher._predicateParam);
            if (result)
            {
                matcher._action(value, matcher._actionParam);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcherParam<T, TMatcher, TPredicateParam, TActionParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}
