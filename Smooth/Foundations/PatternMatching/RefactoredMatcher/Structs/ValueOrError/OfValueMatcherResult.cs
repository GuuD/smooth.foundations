using System.Collections.Generic;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.PatternMatching.MatcherDelegates;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    #region Option Without Parameters
    public struct OfValueMatcherResult<T, TMatcher, TResult>
    {
        internal static OfValueMatcherResult<T, TMatcher, TResult> CreateSkip(ref TMatcher previousMatcher,
                                                                ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                Evaluator<TMatcher, TResult> evaluator)
        {
            return new OfValueMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        internal static OfValueMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                        Evaluator<TMatcher, TResult> evaluator,
                                                        T value)
        {
            var matcher = new OfValueMatcherResult<T, TMatcher, TResult>
            {
                _values = ListPool<T>.Instance.Borrow(),
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
            matcher._values.Add(value);
            return matcher;
        }
        private static readonly Evaluator<OfValueMatcherResult<T, TMatcher, TResult>, TResult> OfEvaluator = Evaluate;
        private static readonly ValueProvider<ValueOrError<T>, OfValueMatcherResult<T, TMatcher, TResult>> OfValueProvider = GetValue;

        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;

        private List<T> _values;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;

        public OfValueMatcherResult<T, TMatcher, TResult> Or(T value)
        {
            if (_skip)
            {
                return this;
            }
            _values.Add(value);
            return this;
        }

        public VoEMatcherResult<T, OfValueMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            }
            return VoEMatcherResult<T, OfValueMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public VoEMatcherResult<T, OfValueMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(result);
            }
            return VoEMatcherResult<T, OfValueMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public VoEMatcherResult<T, OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
            TFuncParam param)
        {
            var proxy = _skip
                ? OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, _values,
                func, param);
            var vp = OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.OfValueProvider;
            var e = OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>.OfEvaluator;
            return VoEMatcherResult<T, OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, !_skip);
        }

        private TResult GetResult(T value)
        {
            return !_funcOrResult.isLeft ? _funcOrResult.rightValue : _funcOrResult.leftValue(value);
        }

        private static bool Evaluate(ref OfValueMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
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
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                res = matcher.GetResult(value);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfValueMatcherResult<T, TMatcher, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region Option With Action Parameter
    public struct OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>
    {

        internal static OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> CreateSkip(ref TMatcher previousMatcher,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator)
        {
            return new OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _evaluator = evaluator,
                _valueProvider = valueProvider,
                _skip = true
            };
        }

        internal static OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                      ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                      Evaluator<TMatcher, TResult> evaluator,
                                                                      List<T> values,
                                                                      DelegateFunc<T, TFuncParam, TResult> action,
                                                                      TFuncParam param)
        {
            return new OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _values = values,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _func = action,
                _param = param
            };
        }
        internal static readonly Evaluator<OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> OfEvaluator = Evaluate;
        internal static readonly ValueProvider<ValueOrError<T>, OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult>> OfValueProvider = GetValue;

        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private List<T> _values;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _param;
        private bool _skip;

        private static bool Evaluate(ref OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out TResult res)
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
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                matcher._func(value, matcher._param);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfValueMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}