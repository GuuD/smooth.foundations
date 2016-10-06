using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    public struct BasicVoEContainer<T>
    {
        internal static BasicVoEContainer<T> Create(ValueOrError<T> value)
        {
            return new BasicVoEContainer<T> { _value = value };
        }
        private static readonly ValueProvider<ValueOrError<T>, BasicVoEContainer<T>> Provider = (ref BasicVoEContainer<T> matcher, out ValueOrError<T> value) => value = matcher._value;
        private static readonly Evaluator<BasicVoEContainer<T>> Evaluator = (ref BasicVoEContainer<T> previous) => false;
        private ValueOrError<T> _value;

        public ValueMatcher<T, BasicVoEContainer<T>> Value()
        {
            return ValueMatcher<T, BasicVoEContainer<T>>.Create(ref this, Provider, Evaluator, !_value.IsError);
        }

        public ErrorMatcher<T, BasicVoEContainer<T>> Error()
        {
            return ErrorMatcher<T, BasicVoEContainer<T>>.Create(ref this, Provider, Evaluator, !_value.IsError);
        }

        public BasicVoEContainerResult<T, TResult> To<TResult>()
        {
            return BasicVoEContainerResult<T, TResult>.Create(_value);
        } 
    }

    public struct VoEMatcher<T, TMatcher>
    {
        private TMatcher _previous;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private bool _isValue;

        internal static VoEMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider, Evaluator<TMatcher> evaluator, bool isValue)
        {
            return new VoEMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _isValue = isValue
            };
        }

        public ValueMatcher<T, TMatcher> Value()
        {
            return ValueMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
        }

        public ErrorMatcher<T, TMatcher> Error()
        {
            return ErrorMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
        }

        public VoEMatcher<T, ErrorMatcher<T, TMatcher>> Error(DelegateAction<string> action)
        {
            var proxy = ErrorMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
            return proxy.Do(action);
        }

        public VoEMatcher<T, ErrorMatcher<T, TMatcher>> Error(DelegateAction action)
        {
            var proxy = ErrorMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isValue);
            return proxy.Do(action);
        }

        public VoEMatcher<T, ErrorMatcher<T, TMatcher, TActionParam>> Error<TActionParam>(
            DelegateAction<string, TActionParam> action, TActionParam param)
        {
            var proxy = _isValue
                ? ErrorMatcher<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcher<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = ErrorMatcher<T, TMatcher, TActionParam>.NoneProvider;
            var e = ErrorMatcher<T, TMatcher, TActionParam>.NoneEvaluator;
            return VoEMatcher<T, ErrorMatcher<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, _isValue);
        }

        public VoEMatcher<T, ErrorMatcher<T, TMatcher, TActionParam>> Error<TActionParam>(
            DelegateAction<TActionParam> action, TActionParam param)
        {
            var proxy = _isValue
                ? ErrorMatcher<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : ErrorMatcher<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = ErrorMatcher<T, TMatcher, TActionParam>.NoneProvider;
            var e = ErrorMatcher<T, TMatcher, TActionParam>.NoneEvaluator;
            return VoEMatcher<T, ErrorMatcher<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, _isValue);
        }

        public VoEMatcherAfterElse<T, TMatcher> Else(DelegateAction action)
        {
            return VoEMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, action);
        }

        public VoEMatcherAfterElse<T, TMatcher> Else(DelegateAction<ValueOrError<T>> action)
        {
            return VoEMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, action);
        }

        public VoEMatcherAfterElse<T, TMatcher, TActionParam> Else<TActionParam>(DelegateAction<TActionParam> action,
            TActionParam param)
        {
            return VoEMatcherAfterElse<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                action, param);
        }

        public VoEMatcherAfterElse<T, TMatcher, TActionParam> Else<TActionParam>(DelegateAction<ValueOrError<T>, TActionParam> action,
            TActionParam param)
        {
            return VoEMatcherAfterElse<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                action, param);
        }

        public VoEMatcherAfterElse<T, TMatcher> IgnoreElse()
        {
            return VoEMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, () => { });
        }

        public void Exec()
        {
            if (_evaluator(ref _previous))
                return;
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            throw new NoMatchException("No match found for " + voe);
        }
    }

    public struct VoEMatcherAfterElse<T, TMatcher>
    {
        private TMatcher _previous;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private Either<DelegateAction<ValueOrError<T>>, DelegateAction> _action; 


        internal static VoEMatcherAfterElse<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider, 
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction action)
        {
            return new VoEMatcherAfterElse<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<ValueOrError<T>>, DelegateAction>.Right(action),
            };
        }

        internal static VoEMatcherAfterElse<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<ValueOrError<T>> action)
        {
            return new VoEMatcherAfterElse<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<ValueOrError<T>>, DelegateAction>.Left(action),
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
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            _action.leftValue(voe);
        }
    }

    public struct VoEMatcherAfterElse<T, TMatcher, TActionParam>
    {
        private TMatcher _previous;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private Either<DelegateAction<ValueOrError<T>, TActionParam>, DelegateAction<TActionParam>> _action;
        private TActionParam _param;


        internal static VoEMatcherAfterElse<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<TActionParam> action,
                                                                   TActionParam param)
        {
            return new VoEMatcherAfterElse<T, TMatcher, TActionParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<ValueOrError<T>, TActionParam>, DelegateAction<TActionParam>>.Right(action),
                _param = param,
            };
        }

        internal static VoEMatcherAfterElse<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<ValueOrError<T>, TActionParam> action,
                                                                   TActionParam param)
        {
            return new VoEMatcherAfterElse<T, TMatcher, TActionParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<ValueOrError<T>, TActionParam>, DelegateAction<TActionParam>>.Left(action),
                _param = param,
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
            ValueOrError<T> voe;
            _valueProvider(ref _previous, out voe);
            _action.leftValue(voe, _param);
        }
    }
}