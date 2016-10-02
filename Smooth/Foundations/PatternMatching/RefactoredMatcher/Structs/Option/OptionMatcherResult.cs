using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct OptionMatcherResult<T, TMatcher, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private bool _isSome;

        internal static OptionMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
            ValueProvider<T, TMatcher> valueProvider, Evaluator<TMatcher> evaluator, bool isSome)
        {
            return new OptionMatcherResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _isSome = isSome
            };
        }

        public SomeMatcherResult<T, TMatcher, TMatcherResult> Some()
        {
            return SomeMatcherResult<T, TMatcher, TMatcherResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public NoneMatcherResult<T, TMatcher, TResult> None()
        {
            return NoneMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult> None(DelegateAction action)
        {
            var proxy = NoneMatcherResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
            var vp = NoneMatcherResult<T, TMatcher, TResult>.NoneProvider;
            var e = NoneMatcherResult<T, TMatcher, TResult>.NoneEvaluator;
            return OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TResult>, TResult>.Create(ref proxy, vp, e, _isSome);
        }

        public OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult> None<TFuncParam>(
            DelegateAction<TFuncParam> action, TFuncParam param)
        {
            var proxy = _isSome
                ? NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneProvider;
            var e = NoneMatcherResult<T, TMatcher, TFuncParam, TResult>.NoneEvaluator;
            return OptionMatcherResult<T, NoneMatcherResult<T, TMatcher, TFuncParam, TResult>, TResult>.Create(ref proxy, vp, e, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateAction action)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, action, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TResult> Else(DelegateAction<Option<T>> action)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, action, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateAction<TFuncParam> action,
            TFuncParam param)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                action, param, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Else<TFuncParam>(DelegateAction<Option<T>, TFuncParam> action,
            TFuncParam param)
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>.Create(ref _previous, _valueProvider, _evaluator,
                action, param, _isSome);
        }

        public OptionMatcherAfterElseResult<T, TMatcher, TResult> IgnoreElse()
        {
            return OptionMatcherAfterElseResult<T, TMatcher, TResult>.Create(ref _previous, _valueProvider, _evaluator, () => { }, _isSome);
        }

        public void Exec()
        {
            if (_evaluator(ref _previous)) return;
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
        private Evaluator<TMatcher> _evaluator;
        private Either<DelegateAction<Option<T>>, DelegateAction> _action;
        private bool _isSome;


        internal static OptionMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction action,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>>, DelegateAction>.Right(action),
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElseResult<T, TMatcher, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<Option<T>> action,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>>, DelegateAction>.Left(action),
                _isSome = isSome
            };
        }


        public void Exec()
        {
            if (_evaluator(ref _previous))
                return;
            if (_action.isRight)
            {
                _action.rightValue();
                return;
            }
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
            _action.leftValue(op);
        }
    }

    public struct OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private Either<DelegateAction<Option<T>, TFuncParam>, DelegateAction<TFuncParam>> _action;
        private TFuncParam _param;
        private bool _isSome;


        internal static OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<TFuncParam> action,
                                                                   TFuncParam param,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>, TFuncParam>, DelegateAction<TFuncParam>>.Right(action),
                _param = param,
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<Option<T>, TFuncParam> action,
                                                                   TFuncParam param,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElseResult<T, TMatcher, TFuncParam, TResult>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>, TFuncParam>, DelegateAction<TFuncParam>>.Left(action),
                _param = param,
                _isSome = isSome
            };
        }


        public void Exec()
        {
            if (_evaluator(ref _previous))
                return;
            if (_action.isRight)
            {
                _action.rightValue(_param);
                return;
            }
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
            _action.leftValue(op, _param);
        }
    }
}