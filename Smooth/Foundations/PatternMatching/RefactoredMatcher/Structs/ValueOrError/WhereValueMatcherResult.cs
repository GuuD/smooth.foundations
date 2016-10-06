using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct WhereValueMatcherResult<T, TMatcher, TResult>
    {

        internal static WhereValueMatcherResult<T, TMatcher, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereValueMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereValueMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator, Predicate<T> predicate)
        {
            return new WhereValueMatcherResult<T, TMatcher, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
        }

        private static readonly ValueProvider<ValueOrError<T>, WhereValueMatcherResult<T, TMatcher, TResult>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereValueMatcherResult<T, TMatcher, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;
        private bool _skip;

        public VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            }
            return VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(result);
            }
            return VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public VoEMatcherResult<T, WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
                                                                                                   TFuncParam param)
        {
            var proxy = _skip
                ? WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, func, param);
            var vp = WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WhereValueProvider;
            var e = WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WhereEvaluator;
            return VoEMatcherResult<T, WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, !_skip);
        }

        private TResult GetResult(T value)
        {
            return !_funcOrResult.isLeft ? _funcOrResult.rightValue : _funcOrResult.leftValue(value);
        }

        private static bool Evaluate(ref WhereValueMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var success = matcher._predicate(value);
            if (success)
            {
                res = matcher.GetResult(value);
            }
            return success;
        }
        private static void GetValue(ref WhereValueMatcherResult<T, TMatcher, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    #region General With func Parameter
    public struct WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>
    {

        internal static WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _skip = true
            };
        }


        internal static WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator, Predicate<T> predicate,
                                                           DelegateFunc<T, TFuncParam, TResult> func,
                                                           TFuncParam param)
        {
            return new WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _func = func,
                _param = param
            };
        }

        internal static readonly ValueProvider<ValueOrError<T>, WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _param;
        private bool _skip;


        private static bool Evaluate(ref WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out TResult res)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var success = matcher._predicate(value);
            if (success)
            {
                res = matcher._func(value, matcher._param);
            }
            return success;
        }
        private static void GetValue(ref WhereValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Predicate Parameter
    public struct WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>
    {

        internal static WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator, Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return new WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _param = param
            };
        }

        private static readonly ValueProvider<ValueOrError<T>, WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _param;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;
        private bool _skip;


        public VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            }
            return VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(result);
            }
            return VoEMatcherResult<T, WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult>.Create(ref this, WhereValueProvider, WhereEvaluator, !_skip);
        }

        public VoEMatcherResult<T, WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
                                                                                                                    TFuncParam param)
        {
            var proxy = _skip
            ? WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
            : WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                                                                                          _predicate, _param, func, param);
            var vp = WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.WhereValueProvider;
            var e = WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.WhereEvaluator;
            return VoEMatcherResult<T, WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult>.Create(
                ref proxy, vp, e, !_skip);
        }

        private TResult GetResult(T value)
        {
            return !_funcOrResult.isLeft ? _funcOrResult.rightValue : _funcOrResult.leftValue(value);
        }

        private static bool Evaluate(ref WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult> matcher, out TResult res)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var success = matcher._predicate(value, matcher._param);
            if (success)
            {
                res = matcher.GetResult(value);
            }
            return success;
        }
        private static void GetValue(ref WhereValueMatcherResult<T, TMatcher, TPredicateParam, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    public struct WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
    {

        internal static WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator)
        {
            return new WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }


        internal static WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator,
                                                           Predicate<T, TPredicateParam> predicate,
                                                           TPredicateParam predicateParam,
                                                           DelegateFunc<T, TFuncParam, TResult> func,
                                                           TFuncParam funcParam)
        {
            return new WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
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

        internal static readonly ValueProvider<ValueOrError<T>, WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _funcParam;
        private bool _skip;


        private static bool Evaluate(ref WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> matcher, out TResult res)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var success = matcher._predicate(value, matcher._predicateParam);
            if (success)
            {
                res = matcher._func(value, matcher._funcParam);
            }
            return success;
        }
        private static void GetValue(ref WhereValueMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}
