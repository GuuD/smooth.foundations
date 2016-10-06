using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct NoneMatcherResult<T, TMatcher, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private Either<DelegateFunc<TResult>, TResult> _funcOrResult;

        public static NoneMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            bool isSome)
        {
            return new NoneMatcherResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = isSome
            };
        }

        internal static readonly ValueProvider<T, NoneMatcherResult<T, TMatcher, TResult>> NoneProvider = GetValue;
        internal static readonly Evaluator<NoneMatcherResult<T, TMatcher, TResult>, TResult> NoneEvaluator = Evaluate;

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult> Return(DelegateFunc<TResult> func)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<TResult>, TResult>.Left(func);
            }
            return OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, NoneProvider, NoneEvaluator, _skip);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult> Return(TResult result)
        {
            if (!_skip)
            {
                _funcOrResult = Either<DelegateFunc<TResult>, TResult>.Right(result);
            }
            return OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref this, NoneProvider, NoneEvaluator, _skip);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> Return<TFuncParam>(DelegateFunc<TFuncParam, TResult>  func, TFuncParam param)
        {
            var proxy = _skip
                ? NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, func, param);
            var vp = NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _skip);
        }

        private static bool Evaluate(ref NoneMatcherResult<T, TMatcher, TResult> matcher, out TResult result)
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
            result = matcher._funcOrResult.Cata(f => f(), r => r);
            return true;
        }

        private static void GetValue(ref NoneMatcherResult<T, TMatcher, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    public struct NoneMatcherResult<T, TMatcher, TFuncParam, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private DelegateFunc<TFuncParam, TResult>  _func;
        private TFuncParam _param;

        public static NoneMatcherResult<T, TMatcher, TFuncParam, TResult> CreateSkip(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator)
        {
            return new NoneMatcherResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        public static NoneMatcherResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previous,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher, TResult> evaluator,
            DelegateFunc<TFuncParam, TResult>  func,
            TFuncParam param)
        {
            return new NoneMatcherResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _func = func,
                _param = param
            };
        }

        internal static readonly ValueProvider<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>> NoneProvider = GetValue;
        internal static readonly Evaluator<NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> NoneEvaluator = Evaluate;

        private static bool Evaluate(ref NoneMatcherResult<T, TMatcher, TFuncParam, TResult> matcher, out TResult result)
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
            result = matcher._func(matcher._param);
            return true;
        }

        private static void GetValue(ref NoneMatcherResult<T, TMatcher, TFuncParam, TResult> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}