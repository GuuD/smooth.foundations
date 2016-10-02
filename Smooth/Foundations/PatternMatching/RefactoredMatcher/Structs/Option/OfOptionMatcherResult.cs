using System.Collections.Generic;
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
                _values = ListPool<T>.Instance.BorrowConditional(l => l.Capacity >= 4, () => new List<T>(6)),
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
        private DelegateFunc<T, TResult> _func;

        public OfOptionMatcherResult<T, TMatcher, TResult> Or(T value)
        {
            if (_skip)
            {
                return this;
            }
            _values.Add(value);
            return this;
        }

        public OptionMatcher<T, OfOptionMatcherResult<T, TMatcher, TResult>> Return(DelegateFunc<T, TResult> func)
        {
            if (!_skip)
            {
                _func = func;
            }
            return OptionMatcher<T, OfOptionMatcherResult<T, TMatcher, TResult>>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public OptionMatcher<T, OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func,
            TFuncParam param)
        {
            var proxy = _skip
                ? OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, _values,
                func, param);
            var vp = OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.OfValueProvider;
            var e = OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>.OfEvaluator;
            return OptionMatcher<T, OfOptionMatcherResultParam<T, TMatcher, TFuncParam, TResult>>.Create(ref proxy, vp, e, !_skip);
        }

        private static bool Evaluate(ref OfOptionMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
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
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                res = matcher._func(value);
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
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                res = matcher._func(value, matcher._param);
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