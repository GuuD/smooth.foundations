using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct ErrorMatcher<T, TMatcher>
    {
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private Either<DelegateAction<string>, DelegateAction> _action;

        public static ErrorMatcher<T, TMatcher> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            bool isValue)
        {
            return new ErrorMatcher<T, TMatcher>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = isValue
            };
        } 

        internal static readonly ValueProvider<ValueOrError<T>, ErrorMatcher<T, TMatcher>> NoneProvider = GetValue;
        internal static readonly Evaluator<ErrorMatcher<T, TMatcher>> NoneEvaluator = Evaluate;

        public VoEMatcher<T, ErrorMatcher<T, TMatcher>> Do(DelegateAction<string> action)
        {
            if (!_skip)
            {
                _action = Either<DelegateAction<string>, DelegateAction>.Left(action);
            }
            return VoEMatcher<T, ErrorMatcher<T, TMatcher>>.Create(ref this, NoneProvider, NoneEvaluator, _skip);
        }

        public VoEMatcher<T, ErrorMatcher<T, TMatcher>> Do(DelegateAction action)
        {
            if (!_skip)
            {
                _action = Either<DelegateAction<string>, DelegateAction>.Right(action);
            }
            return VoEMatcher<T, ErrorMatcher<T, TMatcher>>.Create(ref this, NoneProvider, NoneEvaluator, _skip);
        }

        public VoEMatcher<T, ErrorMatcher<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<TActionParam> action, TActionParam param)
        {
            var proxy = _skip
                ? ErrorMatcher<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcher<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = ErrorMatcher<T, TMatcher, TActionParam>.NoneProvider;
            var e = ErrorMatcher<T, TMatcher, TActionParam>.NoneEvaluator;
            return VoEMatcher<T, ErrorMatcher<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, _skip);
        }

        private static bool Evaluate(ref ErrorMatcher<T, TMatcher> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }
            if (matcher._action.isLeft)
            {
                ValueOrError<T> voe;
                matcher._valueProvider(ref m, out voe);
                matcher._action.leftValue(voe.Error);
            }
            else
            {
                matcher._action.rightValue();
            }
            return true;
        }

        private static void GetValue(ref ErrorMatcher<T, TMatcher> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }

    public struct ErrorMatcher<T, TMatcher, TActionParam>
    {
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;
        private Either<DelegateAction<string, TActionParam>, DelegateAction<TActionParam>> _action;
        private TActionParam _param;

        public static ErrorMatcher<T, TMatcher, TActionParam> CreateSkip(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator)
        {
            return new ErrorMatcher<T, TMatcher, TActionParam>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }

        public static ErrorMatcher<T, TMatcher, TActionParam> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            DelegateAction<TActionParam> action,
            TActionParam param)
        {
            return new ErrorMatcher<T, TMatcher, TActionParam>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<string, TActionParam>, DelegateAction<TActionParam>>.Right(action),
                _param = param
            };
        }

        public static ErrorMatcher<T, TMatcher, TActionParam> Create(ref TMatcher previous,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator,
            DelegateAction<string, TActionParam> action,
            TActionParam param)
        {
            return new ErrorMatcher<T, TMatcher, TActionParam>
            {
                _previous = previous,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<string, TActionParam>, DelegateAction<TActionParam>>.Left(action),
                _param = param
            };
        }

        internal static readonly ValueProvider<ValueOrError<T>, ErrorMatcher<T, TMatcher, TActionParam>> NoneProvider = GetValue;
        internal static readonly Evaluator<ErrorMatcher<T, TMatcher, TActionParam>> NoneEvaluator = Evaluate;

        private static bool Evaluate(ref ErrorMatcher<T, TMatcher, TActionParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }
            if (matcher._skip)
            {
                return false;
            }
            if (matcher._action.isLeft)
            {
                ValueOrError<T> voe;
                matcher._valueProvider(ref m, out voe);
                matcher._action.leftValue(voe.Error, matcher._param);
            }
            else
            {
                matcher._action.rightValue(matcher._param);
            }
            return true;
        }

        private static void GetValue(ref ErrorMatcher<T, TMatcher, TActionParam> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
}