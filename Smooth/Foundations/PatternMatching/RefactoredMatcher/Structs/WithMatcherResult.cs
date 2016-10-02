using System.Collections.Generic;
using System.Linq;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{
    #region General With Result Without Parameters
    public struct WithMatcherResult<T, TMatcher, TResult>
    {
        internal static WithMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<T, TMatcher> extractor,
                                                        Evaluator<TMatcher, TResult> evaluator,
                                                        T value)
        {
            var matcher = new WithMatcherResult<T, TMatcher, TResult>
            {
                _values = ListPool<T>.Instance.BorrowConditional(l => l.Capacity >= 4, () => new List<T>(6)),
                _valueProvider = extractor,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
            matcher._values.Add(value);
            return matcher;
        }
        private static readonly Evaluator<WithMatcherResult<T, TMatcher, TResult>, TResult> WithEvaluator = Evaluate;
        private static readonly ValueProvider<T, WithMatcherResult<T, TMatcher, TResult>> WithValueProvider = GetValue;

        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;

        private List<T> _values;
        private Either<DelegateFunc<T, TResult>, TResult> _funcOrResult;

        public WithMatcherResult<T, TMatcher, TResult> Or(T value)
        {
            _values.Add(value);
            return this;
        }

        public GeneralMatcherResult<T, WithMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<T, TResult> func)
        {
            _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Left(func);
            return GeneralMatcherResult<T, WithMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this,
                WithValueProvider, WithEvaluator);
        }

        public GeneralMatcherResult<T, WithMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult res)
        {
            _funcOrResult = Either<DelegateFunc<T, TResult>, TResult>.Right(res);
            return GeneralMatcherResult<T, WithMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this,
                WithValueProvider, WithEvaluator);
        }

        public GeneralMatcherResult<T, WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<T, TFuncParam, TResult> func, TFuncParam param)
        {
            var proxy = WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider,
                _evaluator, _values, func, param);
            var vp = WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WithValueProvider;
            var e = WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>.WithEvaluator;
            return
                GeneralMatcherResult<T, WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult>.Create(
                    ref proxy, vp, e);
        }

        private TResult GetResult(T value)
        {
            return _funcOrResult.isLeft ? _funcOrResult.leftValue(value) : _funcOrResult.rightValue;
        }

        private static bool Evaluate(ref WithMatcherResult<T, TMatcher, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var match = matcher._values.Slinq().Contains(value);
            if (match)
            {
                res = matcher.GetResult(value);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return match;
        }

        private static void GetValue(ref WithMatcherResult<T, TMatcher, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Result With Func Parameter
    public struct WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>
    {
        internal static WithMatcherResultParam<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<T, TMatcher> extractor,
                                                        Evaluator<TMatcher, TResult> evaluator,
                                                        List<T> values,
                                                        DelegateFunc<T, TFuncParam, TResult> func,
                                                        TFuncParam funcParam)
        {
            var matcher = new WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>
            {
                _values = values,
                _valueProvider = extractor,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _func = func,
                _funcParam = funcParam
            };
            return matcher;
        }
        internal static readonly Evaluator<WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>, TResult> WithEvaluator = Evaluate;
        internal static readonly ValueProvider<T, WithMatcherResultParam<T, TMatcher, TFuncParam, TResult>> WithValueProvider = GetValue;

        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;

        private List<T> _values;
        private DelegateFunc<T, TFuncParam, TResult> _func;
        private TFuncParam _funcParam;


        private static bool Evaluate(ref WithMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out TResult res)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m, out res);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._valueProvider(ref m, out value);
            var match = matcher._values.Slinq().Contains(value);
            if (match)
            {
                res = matcher._func(value, matcher._funcParam);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return match;
        }

        private static void GetValue(ref WithMatcherResultParam<T, TMatcher, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}
