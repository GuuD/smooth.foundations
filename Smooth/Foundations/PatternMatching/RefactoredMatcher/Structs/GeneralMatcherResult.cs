using System.CodeDom;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{

    internal struct BasicContainerResult<T, TResult>
    {
        internal static BasicContainerResult<T, TResult> Create(T value)
        {
            return new BasicContainerResult<T, TResult> {_value = value};
        } 
        internal static ValueProvider<T, BasicContainerResult<T, TResult>> Provider = (ref BasicContainerResult<T, TResult> matcher, out T value) => value = matcher._value;
        internal static Evaluator<BasicContainerResult<T, TResult>, TResult> Evaluator = (ref BasicContainerResult<T, TResult> previous, out TResult result) =>
        {
            result = default(TResult);
            return false;
        };
        private T _value;
    }

    public struct GeneralMatcherResult<T, TMatcher, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;

        internal static GeneralMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previous, 
                                                                ValueProvider<T, TMatcher> valueProvider,
                                                                Evaluator<TMatcher, TResult> evaluator)
        {
            return new GeneralMatcherResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _evaluator = evaluator,
                _valueProvider = valueProvider
            };
        }

        public WhereMatcherResult<T, TMatcher, TResult> Where(Predicate<T> predicate)
        {
            return WhereMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, predicate);
        }

        public WhereMatcherResult<T, TMatcher, TPredicateParam, TResult> Where<TPredicateParam>(Predicate<T, TPredicateParam> predicate, TPredicateParam param)
        {
            return WhereMatcherResult<T, TMatcher, TPredicateParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, predicate, param);
        }

        public WithMatcherResult<T, TMatcher, TResult> With(T value)
        {
            return WithMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, value);
        }

        public GeneralMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateFunc<T, TResult> elseFunc)
        {
            return GeneralMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseFunc);
        }

        public GeneralMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> elseFunc, TFuncParam elseParam)
        {
            return GeneralMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseFunc, elseParam);
        }

        public GeneralMatcherAfterElseResult<T, TMatcher, TResult> Else(TResult elseResult)
        {
            return GeneralMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, elseResult);
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
            {
                return result;
            }
            T value;
            _valueProvider(ref _previous, out value);
            throw new NoMatchException("No match found for value " + value);
        }
    }

    public struct GeneralMatcherAfterElseResult<T, TMatcher, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private Either<DelegateFunc<T, TResult>, TResult> _elseFuncOrElseResult;

        internal static GeneralMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previous,
                                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                                   DelegateFunc<T, TResult> elseFunc)
        {
            return new GeneralMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFuncOrElseResult = Either<DelegateFunc<T, TResult>, TResult>.Left(elseFunc)
            };
        }

        internal static GeneralMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previous,
                                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                                   TResult elseResult)
        {
            return new GeneralMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFuncOrElseResult = Either<DelegateFunc<T, TResult>, TResult>.Right(elseResult)
            };
        }

        private TResult GetResult(T value)
        {
            return _elseFuncOrElseResult.isLeft ? _elseFuncOrElseResult.leftValue(value) : _elseFuncOrElseResult.rightValue;
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
            {
                return result;
            }
            T value;
            _valueProvider(ref _previous, out value);
            return GetResult(value);
        }
    }

    public struct GeneralMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private DelegateFunc<T, TFuncParam, TResult> _elseFunc;
        private TFuncParam _elseParam;

        internal static GeneralMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previous,
                                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                                   Evaluator<TMatcher, TResult> evaluator,
                                                                                   DelegateFunc<T, TFuncParam, TResult> elseFunc,
                                                                                   TFuncParam elseParam)
        {
            return new GeneralMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _elseFunc = elseFunc,
                _elseParam = elseParam
            };
        }

        public TResult Result()
        {
            TResult result;
            if (_evaluator(ref _previous, out result))
            {
                return result;
            }
            T value;
            _valueProvider(ref _previous, out value);
            return _elseFunc(value, _elseParam);
        }
    }
}
