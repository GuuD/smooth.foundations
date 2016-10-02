using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct WhereOptionMatcher<T, TMatcher>
    {

        internal static WhereOptionMatcher<T, TMatcher> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereOptionMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereOptionMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, Predicate<T> predicate)
        {
            return new WhereOptionMatcher<T, TMatcher>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
        }

        private static readonly ValueProvider<T, WhereOptionMatcher<T, TMatcher>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereOptionMatcher<T, TMatcher>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateAction<T> _action;
        private bool _skip;

        public OptionMatcher<T, WhereOptionMatcher<T, TMatcher>> Do(DelegateAction<T> action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return OptionMatcher<T, WhereOptionMatcher<T, TMatcher>>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public GeneralMatcher<T, WhereOptionMatcherParam<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
                                                                                                   TActionParam param)
        {
            var proxy = _skip 
                ? WhereOptionMatcherParam<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereOptionMatcherParam<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, action, param);
            var vp = WhereOptionMatcherParam<T, TMatcher, TActionParam>.WhereValueProvider;
            var e = WhereOptionMatcherParam<T, TMatcher, TActionParam>.WhereEvaluator;
            return GeneralMatcher<T, WhereOptionMatcherParam<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e);
        }

        private static bool Evaluate(ref WhereOptionMatcher<T, TMatcher> matcher)
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
            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value);
            if (result)
            {
                matcher._action(value);
            }
            return result;
        }
        private static void GetValue(ref WhereOptionMatcher<T, TMatcher> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    #region General With Action Parameter
    public struct WhereOptionMatcherParam<T, TMatcher, TActionParam>
    {

        internal static WhereOptionMatcherParam<T, TMatcher, TActionParam> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereOptionMatcherParam<T, TMatcher, TActionParam>
            {
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _skip = true
            };
        }


        internal static WhereOptionMatcherParam<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator,
                                                           Predicate<T> predicate,
                                                           DelegateAction<T, TActionParam> action,
                                                           TActionParam param)
        {
            return new WhereOptionMatcherParam<T, TMatcher, TActionParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _action = action,
                _param = param
            };
        }

        internal static readonly ValueProvider<T, WhereOptionMatcherParam<T, TMatcher, TActionParam>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereOptionMatcherParam<T, TMatcher, TActionParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _param;
        private bool _skip;


        private static bool Evaluate(ref WhereOptionMatcherParam<T, TMatcher, TActionParam> matcher)
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
            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value);
            if (result)
            {
                matcher._action(value, matcher._param);
            }
            return result;
        }
        private static void GetValue(ref WhereOptionMatcherParam<T, TMatcher, TActionParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Predicate Parameter
    public struct WhereOptionMatcher<T, TMatcher, TPredicateParam>
    {


        internal static WhereOptionMatcher<T, TMatcher, TPredicateParam> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereOptionMatcher<T, TMatcher, TPredicateParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereOptionMatcher<T, TMatcher, TPredicateParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return new WhereOptionMatcher<T, TMatcher, TPredicateParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _param = param
            };
        }

        private static readonly ValueProvider<T, WhereOptionMatcher<T, TMatcher, TPredicateParam>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereOptionMatcher<T, TMatcher, TPredicateParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _param;
        private DelegateAction<T> _action;
        private bool _skip;


        public OptionMatcher<T, WhereOptionMatcher<T, TMatcher, TPredicateParam>> Do(DelegateAction<T> action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return OptionMatcher<T, WhereOptionMatcher<T, TMatcher, TPredicateParam>>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public OptionMatcher<T, WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
                                                                                                                    TActionParam param)
        {
            var proxy = _skip 
            ? WhereOptionMatcherParam < T, TMatcher, TPredicateParam, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
            : WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                                                                                          _predicate, _param, action, param);
            var vp = WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.WhereValueProvider;
            var e = WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.WhereEvaluator;
            return OptionMatcher<T, WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>>.Create(
                ref proxy, vp, e, !_skip);
        }

        private static bool Evaluate(ref WhereOptionMatcher<T, TMatcher, TPredicateParam> matcher)
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
            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value, matcher._param);
            if (result)
            {
                matcher._action(value);
            }
            return result;
        }
        private static void GetValue(ref WhereOptionMatcher<T, TMatcher, TPredicateParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    public struct WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
    {

        internal static WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }


        internal static WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator,
                                                           Predicate<T, TPredicateParam> predicate,
                                                           TPredicateParam predicateParam,
                                                           DelegateAction<T, TActionParam> action,
                                                           TActionParam actionParam)
        {
            return new WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
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

        internal static readonly ValueProvider<T, WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _actionParam;
        private bool _skip;


        private static bool Evaluate(ref WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam> matcher)
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
            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value, matcher._predicateParam);
            if (result)
            {
                matcher._action(value, matcher._actionParam);
            }
            return result;
        }
        private static void GetValue(ref WhereOptionMatcherParam<T, TMatcher, TPredicateParam, TActionParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}