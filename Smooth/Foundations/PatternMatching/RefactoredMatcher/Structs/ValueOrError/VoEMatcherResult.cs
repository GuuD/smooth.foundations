using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct BasicVoEContainerResult<T, TResult>
    {
        internal static BasicVoEContainerResult<T, TResult> Create(ValueOrError<T> value)
        {
            return new BasicVoEContainerResult<T, TResult> { _value = value };
        }

        private static readonly ValueProvider<ValueOrError<T>, BasicVoEContainerResult<T, TResult>> Provider = 
            (ref BasicVoEContainerResult<T, TResult> matcher, out ValueOrError<T> value) => value = matcher._value;


        private static readonly Evaluator<BasicVoEContainerResult<T, TResult>, TResult> Evaluator = (ref BasicVoEContainerResult<T, TResult> previous, out TResult result) =>
        {
            result = default(TResult);
            return false;
        };
        private ValueOrError<T> _value;

        public ValueMatcherResult<T, BasicVoEContainerResult<T, TResult>, TResult> Value()
        {
            return ValueMatcherResult<T, BasicVoEContainerResult<T, TResult>, TResult>.Create(ref this, Provider, Evaluator, !_value.IsError);
        }

        public ErrorMatcherResult<T, BasicVoEContainerResult<T, TResult>, TResult> Error()
        {
            return ErrorMatcherResult<T, BasicVoEContainerResult<T, TResult>, TResult>.Create(ref this, Provider, Evaluator, !_value.IsError);
        }
    }

    public struct VoEMatcherResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private Evaluator<TMatcher, TResult> _evaluator;
        private bool _isValue;

        internal static VoEMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider, Evaluator<TMatcher, TResult> evaluator, bool isSome)
        {
            return new VoEMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _isValue = isSome
            };
        }

        public ValueMatcherResult<T, TMatcher, TResult> Value()
        {
            return ValueMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
        }

        public ErrorMatcherResult<T, TMatcher, TResult> Error()
        {
            return ErrorMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult> Error(DelegateFunc<string, TResult> func)
        {
            var proxy = ErrorMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
            return proxy.Return(func);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult> Error(DelegateFunc<TResult> func)
        {
            var proxy = ErrorMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
            return proxy.Return(func);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TResult>, TResult> Error(TResult value)
        {
            var proxy = ErrorMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
            return proxy.Return(value);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> Error<TFuncParam>(
            DelegateFunc<TFuncParam, TResult> action, TFuncParam param)
        {
            var proxy = _isValue
                ? ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _isValue);
        }

        public VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> Error<TFuncParam>(
            DelegateFunc<string, TFuncParam, TResult> action, TFuncParam param)
        {
            var proxy = _isValue
                ? ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return VoEMatcherResult<T, ErrorMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _isValue);
        }

        public VoEMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateFunc<TResult> elseFunc)
        {
            return VoEMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseFunc);
        }

        public VoEMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateFunc<ValueOrError<T>, TResult> elseFunc)
        {
            return VoEMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseFunc);
        }

        public VoEMatcherAfterElseResult<T, TMatcher, TResult> Else(TResult elseResult)
        {
            return VoEMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseResult);
        }

        public VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateFunc<TFuncParam, TResult> func,
            TFuncParam param)
        {
            return VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                func, param);
        }

        public VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateFunc<ValueOrError<T>, TFuncParam, TResult> func,
            TFuncParam param)
        {
            return VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                func, param);
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
                return result;
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            throw new NoMatchException("No match found for " + voe);
        }
    }

    public struct VoEMatcherAfterElseResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private Evaluator<TMatcher, TResult> _evaluator;
        private Union<DelegateFunc<ValueOrError<T>, TResult>, DelegateFunc<TResult>, TResult> _elseResult;

        internal static VoEMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   TResult result)
        {
            return new VoEMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseResult = Union<DelegateFunc<ValueOrError<T>, TResult>, DelegateFunc<TResult>, TResult>.CreateThird(result),
            };
        }

        internal static VoEMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<TResult> func)
        {
            return new VoEMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseResult = Union<DelegateFunc<ValueOrError<T>, TResult>, DelegateFunc<TResult>, TResult>.CreateSecond(func),
            };
        }

        internal static VoEMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<ValueOrError<T>, TResult> func)
        {
            return new VoEMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseResult = Union<DelegateFunc<ValueOrError<T>, TResult>, DelegateFunc<TResult>, TResult>.CreateFirst(func),
            };
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
                return result;
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            return _elseResult.Cata((f, v) => f(v), voe, func => func(), r => r);
        }
    }

    public struct VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private Evaluator<TMatcher, TResult> _evaluator;
        private Either<DelegateFunc<ValueOrError<T>, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>> _elseFunc;
        private TFuncParam _param;

        internal static VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<TFuncParam, TResult> elseFunc,
                                                                   TFuncParam param)
        {
            return new VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFunc = Either<DelegateFunc<ValueOrError<T>, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>>.Right(elseFunc),
                _param = param,
            };
        }

        internal static VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<ValueOrError<T>, TFuncParam, TResult> elseFunc,
                                                                   TFuncParam param)
        {
            return new VoEMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFunc = Either<DelegateFunc<ValueOrError<T>, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>>.Left(elseFunc),
                _param = param,
            };
        }


        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
                return result;
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            return _elseFunc.Cata((f, p) => f.Call(p), Tuple.Create(voe, _param), (func, p) => func(p), _param);
        }
    }
}