using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct BasicOptionContainerResult<T, TResult>
    {
        internal static BasicOptionContainerResult<T, TResult> Create(Option<T> value)
        {
            return new BasicOptionContainerResult<T, TResult> { _value = value };
        }
        private static readonly ValueProvider<T, BasicOptionContainerResult<T, TResult>> Provider = 
            (ref BasicOptionContainerResult<T, TResult> matcher, out T value) => value = matcher._value.value;
        private static readonly Evaluator<BasicOptionContainerResult<T, TResult>, TResult> Evaluator = 
            (ref BasicOptionContainerResult<T, TResult> previous, out TResult result) =>
        {
            result = default(TResult);
            return false;
        };
        private Option<T> _value;

        public SomeMatcherResult<T, BasicOptionContainerResult<T, TResult>, TResult> Some()
        {
            return SomeMatcherResult<T, BasicOptionContainerResult<T, TResult>, TResult>.Create(ref this, Provider, Evaluator, _value.isSome);
        }

        public NoneMatcherResult<T, BasicOptionContainerResult<T, TResult>, TResult> None()
        {
            return NoneMatcherResult<T, BasicOptionContainerResult<T, TResult>, TResult>.Create(ref this, Provider, Evaluator, _value.isSome);
        }
    }

    public struct OptionMatcherResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher, TResult> _evaluator;
        private bool _isSome;

        internal static OptionMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
            ValueProvider<T, TMatcher> valueProvider, Evaluator<TMatcher, TResult> evaluator, bool isSome)
        {
            return new OptionMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _isSome = isSome
            };
        }

        public SomeMatcherResult<T, TMatcher, TResult> Some()
        {
            return SomeMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public NoneMatcherResult<T, TMatcher, TResult> None()
        {
            return NoneMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult> None(DelegateFunc<TResult> func)
        {
            var proxy = NoneMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
            return proxy.Return(func);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult> None(TResult value)
        {
            var proxy = NoneMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
            return proxy.Return(value);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> None<TFuncParam>(
            DelegateFunc<TFuncParam, TResult> action, TFuncParam param)
        {
            var proxy = _isSome
                ? NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateFunc<TResult> elseFunc)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseFunc, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateFunc<Option<T>, TResult> elseFunc)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseFunc, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TResult> Else(TResult elseResult)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseResult, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateFunc<TFuncParam, TResult> func,
            TFuncParam param)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                func, param, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateFunc<Option<T>, TFuncParam, TResult> func,
            TFuncParam param)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                func, param, _isSome);
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
                return result;
            Option<T> op;
            if (_isSome)
            {
                T value;
                _valueProvider(ref _previous, out value);
                op = value.ToSome();
            }
            else
            {
                op = Option<T>.None;
            }
            throw new NoMatchException("No match found for " + op);
        }
    }

    public struct OptionMatcherAfterElseResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher, TResult> _evaluator;
        private Union<DelegateFunc<Option<T>, TResult>, DelegateFunc<TResult>, TResult> _elseResult;
        private bool _isSome;


        internal static OptionMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   TResult result,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseResult = Union<DelegateFunc<Option<T>, TResult>, DelegateFunc<TResult>, TResult>.CreateThird(result),
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<TResult> func,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseResult = Union<DelegateFunc<Option<T>, TResult>, DelegateFunc<TResult>, TResult>.CreateSecond(func),
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<Option<T>, TResult> func,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseResult = Union<DelegateFunc<Option<T>, TResult>, DelegateFunc<TResult>, TResult>.CreateFirst(func),
                _isSome = isSome
            };
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
                return result;

            Option<T> op;
            if (_isSome)
            {
                T value;
                _valueProvider(ref _previous, out value);
                op = value.ToSome();
            }
            else
            {
                op = Option<T>.None;
            }
            return _elseResult.Cata((f, o) => f(o), op, func => func(), r => r);
        }
    }

    public struct OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher, TResult> _evaluator;
        private Either<DelegateFunc<Option<T>, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>> _elseFunc;
        private TFuncParam _param;
        private bool _isSome;


        internal static OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<TFuncParam, TResult> elseFunc,
                                                                   TFuncParam param,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFunc = Either<DelegateFunc<Option<T>, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>>.Right(elseFunc),
                _param = param,
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                   DelegateFunc<Option<T>, TFuncParam, TResult> elseFunc,
                                                                   TFuncParam param,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFunc = Either<DelegateFunc<Option<T>, TFuncParam, TResult>, DelegateFunc<TFuncParam, TResult>>.Left(elseFunc),
                _param = param,
                _isSome = isSome
            };
        }


        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
                return result;

            Option<T> op;
            if (_isSome)
            {
                T value;
                _valueProvider(ref _previous, out value);
                op = value.ToSome();
            }
            else
            {
                op = Option<T>.None;
            }
            return _elseFunc.Cata((f, p) => f.Call(p), Tuple.Create(op, _param), (func, p) => func(p), _param);
        }
    }
}