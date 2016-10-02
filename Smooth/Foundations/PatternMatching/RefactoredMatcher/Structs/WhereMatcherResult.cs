using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{
    #region General With Result Without Any Parameters
    public struct WhereMatcherResult<T, TMatcher, TResult>
    {
        internal static WhereMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator,
                                                           Predicate<T> predicate)
        {
            return new WhereMatcherResult<T, TMatcher, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
        }

        private static readonly ValueProvider<T, WhereMatcherResult<T, TMatcher, TResult>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereMatcherResult<T, TMatcher, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;


        public GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            return GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this,
               WhereValueProvider, WhereEvaluator);
        }

        public GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult res)
        {
            _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(res);
            return GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this,
               WhereValueProvider, WhereEvaluator);
        }

        public GeneralMatcherResult<T, WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> Do<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func, TFuncParam param)
        {
            var proxy = WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, func, param);
            var vp = WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WhereValueProvider;
            var e = WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WhereEvaluator;
            return GeneralMatcherResult<T, WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e);
        }

        private TResult GetResult(T value)
        {
            return _funcOrResult.isLeft ? _funcOrResult.leftValue(value) : _funcOrResult.rightValue;
        }

        private static bool Evaluate(ref WhereMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value);
            if (result)
            {
                res = matcher.GetResult(value);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcherResult<T, TMatcher, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
    
    #region General With Result With Func Parameter
    public struct WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>
    {
        internal static WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator,
                                                           Predicate<T> predicate,
                                                           DelegateFunc<T, TFuncParam, TResult> func,
                                                           TFuncParam param)
        {
            return new WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _predicate = predicate,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _func = func,
                _param = param
            };
        }

        internal static readonly ValueProvider<T, WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T> _predicate;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _param;

        private static bool Evaluate(ref WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value);
            if (result)
            {
                res = matcher._func(value, matcher._param);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Result With Predicate Parameter
    public struct WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>
    {
        internal static WhereMatcherResult<T, TMatcher, TPredicateParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator,
                                                           Predicate<T, TPredicateParam> predicate,
                                                           TPredicateParam predicateParam)
        {
            return new WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _predicate = predicate,
                _predicateParam = predicateParam
            };
        }

        private static readonly ValueProvider<T, WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>> WhereValueProvider = GetValue;
        private static readonly Evaluator<WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;


        public GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            return GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult>.Create(ref this,
               WhereValueProvider, WhereEvaluator);
        }

        public GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult> Return(TResult res)
        {
            _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(res);
            return GeneralMatcherResult<T, WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>, TResult>.Create(ref this,
               WhereValueProvider, WhereEvaluator);
        }

        public GeneralMatcherResult<T, WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult> Do<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func, TFuncParam param)
        {
            var proxy = WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                _predicate, _predicateParam, func, param);
            var vp = WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.WhereValueProvider;
            var e = WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>.WhereEvaluator;
            return GeneralMatcherResult<T, WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e);
        }

        private TResult GetResult(T value)
        {
            return _funcOrResult.isLeft ? _funcOrResult.leftValue(value) : _funcOrResult.rightValue;
        }

        private static bool Evaluate(ref WhereMatcherResult<T, TMatcher, TPredicateParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value, matcher._predicateParam);
            if (result)
            {
                res = matcher.GetResult(value);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcherResult<T, TMatcher, TPredicateParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
    #region General With Result With Predicate And Func Parameters
    public struct WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
    {
        internal static WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                           ValueProvider<T, TMatcher> valueProvider,
                                                           Evaluator<TMatcher, TResult> evaluator,
                                                           Predicate<T, TPredicateParam> predicate,
                                                           TPredicateParam predicateParam,
                                                           DelegateFunc<T, TFuncParam, TResult> func,
                                                           TFuncParam param)
        {
            return new WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _predicate = predicate,
                _predicateParam = predicateParam,
                _func = func,
                _funcParam = param
            };
        }

        internal static readonly ValueProvider<T, WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>> WhereValueProvider = GetValue;
        internal static readonly Evaluator<WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult>, TResult> WhereEvaluator = Evaluate;


        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Predicate<T, TPredicateParam> _predicate;
        private TPredicateParam _predicateParam;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _funcParam;

        private static bool Evaluate(ref WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._predicate(value, matcher._predicateParam);
            if (result)
            {
                res = matcher._func(value, matcher._funcParam);
            }
            return result;
        }
        private static void GetValue(ref WhereMatcherResultParam<T, TMatcher, TPredicateParam, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}