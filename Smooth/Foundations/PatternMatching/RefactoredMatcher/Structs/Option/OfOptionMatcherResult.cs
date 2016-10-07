using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    #region Option Without Parameters
    public struct OfOptionMatcherResult<T, TMatcher, TResult>
    {
        internal static OfOptionMatcherResult<T, TMatcher, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                                ValueProvider<T, TMatcher> valueProvider,
                                                                Evaluator<TMatcher, TResult> evaluator)
        {
            return new OfOptionMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static OfOptionMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<T, TMatcher> valueProvider,
                                                        Evaluator<TMatcher, TResult> evaluator,
                                                        T value)
        {
            var matcher = new OfOptionMatcherResult<T, TMatcher, TResult>
            {
                _values = ListPool<T>.Instance.Borrow(),
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
            matcher._values.Add(value);
            return matcher;
        }
        private static readonly Evaluator<OfOptionMatcherResult<T, TMatcher, TResult>, TResult> OfEvaluator = Evaluate;
        private static readonly ValueProvider<T, OfOptionMatcherResult<T, TMatcher, TResult>> OfValueProvider = GetValue;

        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;

        private List<T> _values;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;

        public OfOptionMatcherResult<T, TMatcher, TResult> Or(T value)
        {
            if (_skip)
            {
                return this;
            }
            _values.Add(value);
            return this;
        }

        public OptionMatcherResult<T, OfOptionMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            }
            return OptionMatcherResult<T, OfOptionMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public OptionMatcherResult<T, OfOptionMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(result);
            }
            return OptionMatcherResult<T, OfOptionMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public OptionMatcherResult<T, OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
            TFuncParam param)
        {
            var proxy = _skip
                ? OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, _values,
                func, param);
            var vp = OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.OfValueProvider;
            var e = OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.OfEvaluator;
            return OptionMatcherResult<T, OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, !_skip);
        }

        private TResult GetResult(T value)
        {
            return !_funcOrResult.isLeft ? _funcOrResult.rightValue : _funcOrResult.leftValue(value);
        }

        private static bool Evaluate(ref OfOptionMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                ListPool<T>.Instance.Release(matcher._values);
                return true;
            }
            if (matcher._skip)
            {
                ListPool<T>.Instance.Release(matcher._values);
                return false;
            }
            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                res = matcher.GetResult(value);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfOptionMatcherResult<T, TMatcher, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region Option With Action Parameter
    public struct OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>
    {

        internal static OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> CreateSkip(ref TMatcher previousMatcher,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator)
        {
            return new OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _evaluator = evaluator,
                _valueProvider = valueProvider,
                _skip = true
            };
        }

        internal static OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                      ValueProvider<T, TMatcher> valueProvider,
                                                                      Evaluator<TMatcher, TResult> evaluator,
                                                                      List<T> values,
                                                                      DelegateFunc<T, TFuncParam, TResult> action,
                                                                      TFuncParam param)
        {
            return new OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _values = values,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _func = action,
                _param = param
            };
        }
        internal static readonly Evaluator<OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> OfEvaluator = Evaluate;
        internal static readonly ValueProvider<T, OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>> OfValueProvider = GetValue;

        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private List<T> _values;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _param;
        private bool _skip;

        private static bool Evaluate(ref OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                ListPool<T>.Instance.Release(matcher._values);
                return true;
            }
            if (matcher._skip)
            {
                ListPool<T>.Instance.Release(matcher._values);
                return false;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                matcher._func(value, matcher._param);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}