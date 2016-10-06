using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct ErrorMatcherResult<T, TMatcher, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private Union<DelegateFunc<string, TResult>, DelegateFunc<TResult>, TResult> _funcOrResult;

        public static ErrorMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            bool isValue)
        {
            return new ErrorMatcherResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = isValue
            };
        }

        internal static readonly ValueProvider<ValueOrError<T>, ErrorMatcherResult<T, TMatcher, TResult>> ErrorValueProvider = GetValue;
        internal static readonly Evaluator<ErrorMatcherResult<T, TMatcher, TResult>, TResult> ErrorEvaluator = Evaluate;

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<string, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Union<DelegateFunc<string, TResult>, DelegateFunc<TResult>, TResult>.CreateFirst(func);
            }
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, ErrorValueProvider, ErrorEvaluator, _skip);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Union<DelegateFunc<string, TResult>, DelegateFunc<TResult>, TResult>.CreateSecond(func);
            }
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, ErrorValueProvider, ErrorEvaluator, _skip);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Union<DelegateFunc<string, TResult>, DelegateFunc<TResult>, TResult>.CreateThird(result);
            }
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, ErrorValueProvider, ErrorEvaluator, _skip);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<TFuncParam, TResult>  func, TFuncParam param)
        {
            var proxy = _skip
                ? ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, func, param);
            var vp = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _skip);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<string, TFuncParam, TResult> func, TFuncParam param)
        {
            var proxy = _skip
                ? ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, func, param);
            var vp = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _skip);
        }

        private TResult GetResult()
        {
            if (_funcOrResult.Case != Variant.First)
                return _funcOrResult.Case == Variant.Second
                    ? _funcOrResult.Case2()
                    : _funcOrResult.Case3;
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            return _funcOrResult.Case1(voe.Error);
        }

        private static bool Evaluate(ref ErrorMatcherResult<T, TMatcher, TResult> matcher, out TResult result)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out result);
            if (intermediateResult)
            {
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }
            result = matcher.GetResult();
            return true;
        }

        private static void GetValue(ref ErrorMatcherResult<T, TMatcher, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    public struct ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private Either<DelegateFunc<string, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>> _func;
        private TFuncParam _param;

        public static ErrorMatcherResult<T, TMatcher, TFuncParam, TResult> CreateSkip(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator)
        {
            return new ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        public static ErrorMatcherResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            DelegateFunc<TFuncParam, TResult>  func,
            TFuncParam param)
        {
            return new ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _func = Either<DelegateFunc<string, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>>.Right(func),
                _param = param
            };
        }

        public static ErrorMatcherResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            DelegateFunc<string, TFuncParam, TResult> func,
            TFuncParam param)
        {
            return new ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _func = Either<DelegateFunc<string, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>>.Left(func),
                _param = param
            };
        }

        internal static readonly ValueProvider<ValueOrError<T>, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>> NoneProvider = GetValue;
        internal static readonly Evaluator<ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> NoneEvaluator = Evaluate;

        private TResult GetResult()
        {
            if (_func.isLeft)
            {
                ValueOrError<T> voe;
                _valueProvider(ref _previous, out voe);
                return _func.leftValue(voe.Error, _param);
            }
            return _func.rightValue(_param);
        }

        private static bool Evaluate(ref ErrorMatcherResult<T, TMatcher, TFuncParam, TResult> matcher, out TResult result)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out result);
            if (intermediateResult)
            {
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }
            result = matcher.GetResult();
            return true;
        }

        private static void GetValue(ref ErrorMatcherResult<T, TMatcher, TFuncParam, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}