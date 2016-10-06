using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct WhereValueMatcher<T, TMatcher>
    {

        internal static WhereValueMatcher<T, TMatcher> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereValueMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereValueMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, Predicate<T> predicate)
        {
            return new WhereValueMatcher<T, TMatcher>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
        }

        private static readonly ValueProvider<ValueOrError<T>, WhereValueMatcher<T, TMatcher>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereValueMatcher<T, TMatcher>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateAction<T> _action;
        private bool _skip;

        public VoEMatcher<T, WhereValueMatcher<T, TMatcher>> Do(DelegateAction<T> action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return VoEMatcher<T, WhereValueMatcher<T, TMatcher>>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public VoEMatcher<T, WhereValueMatcherParam<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
                                                                                                   TActionParam param)
        {
            var proxy = _skip 
                ? WhereValueMatcherParam<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereValueMatcherParam<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, action, param);
            var vp = WhereValueMatcherParam<T, TMatcher, TActionParam>.WhereValueProvider;
            var e = WhereValueMatcherParam<T, TMatcher, TActionParam>.WhereEvaluator;
            return VoEMatcher<T, WhereValueMatcherParam<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, !_skip);
        }

        private static bool Evaluate(ref WhereValueMatcher<T, TMatcher> matcher)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var result = matcher._predicate(value);
            if (result)
            {
                matcher._action(value);
            }
            return result;
        }
        private static void GetValue(ref WhereValueMatcher<T, TMatcher> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    #region General With Action Parameter
    public struct WhereValueMatcherParam<T, TMatcher, TActionParam>
    {

        internal static WhereValueMatcherParam<T, TMatcher, TActionParam> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereValueMatcherParam<T, TMatcher, TActionParam>
            {
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _skip = true
            };
        }


        internal static WhereValueMatcherParam<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator,
                                                           Predicate<T> predicate,
                                                           DelegateAction<T, TActionParam> action,
                                                           TActionParam param)
        {
            return new WhereValueMatcherParam<T, TMatcher, TActionParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _action = action,
                _param = param
            };
        }

        internal static readonly ValueProvider<ValueOrError<T>, WhereValueMatcherParam<T, TMatcher, TActionParam>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereValueMatcherParam<T, TMatcher, TActionParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _param;
        private bool _skip;


        private static bool Evaluate(ref WhereValueMatcherParam<T, TMatcher, TActionParam> matcher)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var result = matcher._predicate(value);
            if (result)
            {
                matcher._action(value, matcher._param);
            }
            return result;
        }
        private static void GetValue(ref WhereValueMatcherParam<T, TMatcher, TActionParam> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Predicate Parameter
    public struct WhereValueMatcher<T, TMatcher, TPredicateParam>
    {


        internal static WhereValueMatcher<T, TMatcher, TPredicateParam> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereValueMatcher<T, TMatcher, TPredicateParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereValueMatcher<T, TMatcher, TPredicateParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator, Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return new WhereValueMatcher<T, TMatcher, TPredicateParam>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _param = param
            };
        }

        private static readonly ValueProvider<ValueOrError<T>, WhereValueMatcher<T, TMatcher, TPredicateParam>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereValueMatcher<T, TMatcher, TPredicateParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _param;
        private DelegateAction<T> _action;
        private bool _skip;


        public VoEMatcher<T, WhereValueMatcher<T, TMatcher, TPredicateParam>> Do(DelegateAction<T> action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return VoEMatcher<T, WhereValueMatcher<T, TMatcher, TPredicateParam>>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public VoEMatcher<T, WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
                                                                                                                    TActionParam param)
        {
            var proxy = _skip 
            ? WhereValueMatcherParam < T, TMatcher, TPredicateParam, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
            : WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                                                                                          _predicate, _param, action, param);
            var vp = WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.WhereValueProvider;
            var e = WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>.WhereEvaluator;
            return VoEMatcher<T, WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>>.Create(
                ref proxy, vp, e, !_skip);
        }

        private static bool Evaluate(ref WhereValueMatcher<T, TMatcher, TPredicateParam> matcher)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var result = matcher._predicate(value, matcher._param);
            if (result)
            {
                matcher._action(value);
            }
            return result;
        }
        private static void GetValue(ref WhereValueMatcher<T, TMatcher, TPredicateParam> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    public struct WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
    {

        internal static WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator)
        {
            return new WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }


        internal static WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher> evaluator,
                                                           Predicate<T, TPredicateParam> predicate,
                                                           TPredicateParam predicateParam,
                                                           DelegateAction<T, TActionParam> action,
                                                           TActionParam actionParam)
        {
            return new WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>
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

        internal static readonly ValueProvider<ValueOrError<T>, WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam>> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _actionParam;
        private bool _skip;


        private static bool Evaluate(ref WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam> matcher)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var result = matcher._predicate(value, matcher._predicateParam);
            if (result)
            {
                matcher._action(value, matcher._actionParam);
            }
            return result;
        }
        private static void GetValue(ref WhereValueMatcherParam<T, TMatcher, TPredicateParam, TActionParam> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}