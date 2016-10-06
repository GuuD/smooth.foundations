using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct WhereOptionMatcherResult<T, TMatcher, TResult>
    {

        internal static WhereOptionMatcherResult<T, TMatcher, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereOptionMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereOptionMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator, Predicate<T> predicate)
        {
            return new WhereOptionMatcherResult<T, TMatcher, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
        }

        private static readonly ValueProvider<T, WhereOptionMatcherResult<T, TMatcher, TResult>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereOptionMatcherResult<T, TMatcher, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;
        private bool _skip;

        public OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            }
            return OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(result);
            }
            return OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public OptionMatcherResult<T, WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
                                                                                                   TFuncParam param)
        {
            var proxy = _skip
                ? WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, func, param);
            var vp = WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WhereValueProvider;
            var e = WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WhereEvaluator;
            return OptionMatcherResult<T, WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, !_skip);
        }

        private TResult GetResult(T value)
        {
            return !_funcOrResult.isLeft ? _funcOrResult.rightValue : _funcOrResult.leftValue(value);
        }

        private static bool Evaluate(ref WhereOptionMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
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
            var success = matcher._predicate(value);
            if (success)
            {
                res = matcher.GetResult(value);
            }
            return success;
        }
        private static void GetValue(ref WhereOptionMatcherResult<T, TMatcher, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    #region General With func Parameter
    public struct WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>
    {

        internal static WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _skip = true
            };
        }


        internal static WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator, Predicate<T> predicate,
                                                           DelegateFunc<T, TFuncParam, TResult> func,
                                                           TFuncParam param)
        {
            return new WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _func = func,
                _param = param
            };
        }

        internal static readonly ValueProvider<T, WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _param;
        private bool _skip;


        private static bool Evaluate(ref WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
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
            var success = matcher._predicate(value);
            if (success)
            {
                res = matcher._func(value, matcher._param);
            }
            return success;
        }
        private static void GetValue(ref WhereOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Predicate Parameter
    public struct WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>
    {

        internal static WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator, Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return new WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _param = param
            };
        }

        private static readonly ValueProvider<T, WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _param;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;
        private bool _skip;


        public OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            }
            return OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(result);
            }
            return OptionMatcherResult<T, WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public OptionMatcherResult<T, WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
                                                                                                                    TFuncParam param)
        {
            var proxy = _skip
            ? WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
            : WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                                                                                          _predicate, _param, func, param);
            var vp = WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.WhereValueProvider;
            var e = WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.WhereEvaluator;
            return OptionMatcherResult<T, WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult>.Create(
                ref proxy, vp, e, !_skip);
        }

        private TResult GetResult(T value)
        {
            return !_funcOrResult.isLeft ? _funcOrResult.rightValue : _funcOrResult.leftValue(value);
        }

        private static bool Evaluate(ref WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
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
            var success = matcher._predicate(value, matcher._param);
            if (success)
            {
                res = matcher.GetResult(value);
            }
            return success;
        }
        private static void GetValue(ref WhereOptionMatcherResult<T, TMatcher, TPredicateParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    public struct WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
    {

        internal static WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }


        internal static WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator,
                                                           Predicate<T, TPredicateParam> predicate,
                                                           TPredicateParam predicateParam,
                                                           DelegateFunc<T, TFuncParam, TResult> func,
                                                           TFuncParam funcParam)
        {
            return new WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _predicateParam = predicateParam,
                _func = func,
                _funcParam = funcParam
            };
        }

        internal static readonly ValueProvider<T, WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _funcParam;
        private bool _skip;


        private static bool Evaluate(ref WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
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
            var success = matcher._predicate(value, matcher._predicateParam);
            if (success)
            {
                res = matcher._func(value, matcher._funcParam);
            }
            return success;
        }
        private static void GetValue(ref WhereOptionMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}
