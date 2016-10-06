using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    public struct BasicOptionContainer<T>
    {
        internal static BasicOptionContainer<T> Create(Option<T> value)
        {
            return new BasicOptionContainer<T> { _value = value };
        }
        private static readonly ValueProvider<T, BasicOptionContainer<T>> Provider = (ref BasicOptionContainer<T> matcher, out T value) => value = matcher._value.value;
        private static readonly Evaluator<BasicOptionContainer<T>> Evaluator = (ref BasicOptionContainer<T> previous) => false;
        private Option<T> _value;
        public SomeMatcher<T, BasicOptionContainer<T>> Some()
        {
            return SomeMatcher<T, BasicOptionContainer<T>>.Create(ref this, Provider, Evaluator, _value.isSome);
        }

        public NoneMatcher<T, BasicOptionContainer<T>> None()
        {
            return NoneMatcher<T, BasicOptionContainer<T>>.Create(ref this, Provider, Evaluator, _value.isSome);
        }

        public BasicOptionContainerResult<T, TResult> To<TResult>()
        {
            return BasicOptionContainerResult<T, TResult>.Create(_value);
        } 
    }

    public struct OptionMatcher<T, TMatcher>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private bool _isSome;

        internal static OptionMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
            ValueProvider<T, TMatcher> valueProvider, Evaluator<TMatcher> evaluator, bool isSome)
        {
            return new OptionMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _isSome = isSome
            };
        }

        public SomeMatcher<T, TMatcher> Some()
        {
            return SomeMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public NoneMatcher<T, TMatcher> None()
        {
            return NoneMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
        }

        public OptionMatcher<T, NoneMatcher<T, TMatcher>> None(DelegateAction action)
        {
            var proxy = NoneMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, _isSome);
            return proxy.Do(action);
        }

        public OptionMatcher<T, NoneMatcher<T, TMatcher, TActionParam>> None<TActionParam>(
            DelegateAction<TActionParam> action, TActionParam param)
        {
            var proxy = _isSome
                ? NoneMatcher<T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator)
                : NoneMatcher<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, action, param);
            var vp = NoneMatcher<T, TMatcher, TActionParam>.NoneProvider;
            var e = NoneMatcher<T, TMatcher, TActionParam>.NoneEvaluator;
            return OptionMatcher<T, NoneMatcher<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, _isSome);
        }

        public OptionMatcherAfterElse<T, TMatcher> Else(DelegateAction action)
        {
            return OptionMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, action, _isSome);
        }

        public OptionMatcherAfterElse<T, TMatcher> Else(DelegateAction<Option<T>> action)
        {
            return OptionMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, action, _isSome);
        }

        public OptionMatcherAfterElse<T, TMatcher, TActionParam> Else<TActionParam>(DelegateAction<TActionParam> action,
            TActionParam param)
        {
            return OptionMatcherAfterElse<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                action, param, _isSome);
        }

        public OptionMatcherAfterElse<T, TMatcher, TActionParam> Else<TActionParam>(DelegateAction<Option<T>, TActionParam> action,
            TActionParam param)
        {
            return OptionMatcherAfterElse<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                action, param, _isSome);
        }

        public OptionMatcherAfterElse<T, TMatcher> IgnoreElse()
        {
            return OptionMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, () => { }, _isSome);
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

    public struct OptionMatcherAfterElse<T, TMatcher>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private Either<DelegateAction<Option<T>>, DelegateAction> _action; 
        private bool _isSome;


        internal static OptionMatcherAfterElse<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider, 
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction action, 
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElse<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>>, DelegateAction>.Right(action),
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElse<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<Option<T>> action,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElse<T, TMatcher>
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

    public struct OptionMatcherAfterElse<T, TMatcher, TActionParam>
    {
        private TMatcher _previous;
        private ValueProvider<T, TMatcher> _valueProvider;
        private Evaluator<TMatcher> _evaluator;
        private Either<DelegateAction<Option<T>, TActionParam>, DelegateAction<TActionParam>> _action;
        private TActionParam _param;
        private bool _isSome;


        internal static OptionMatcherAfterElse<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<TActionParam> action,
                                                                   TActionParam param,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElse<T, TMatcher, TActionParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>, TActionParam>, DelegateAction<TActionParam>>.Right(action),
                _param = param,
                _isSome = isSome
            };
        }

        internal static OptionMatcherAfterElse<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                   ValueProvider<T, TMatcher> valueProvider,
                                                                   Evaluator<TMatcher> evaluator,
                                                                   DelegateAction<Option<T>, TActionParam> action,
                                                                   TActionParam param,
                                                                   bool isSome)
        {
            return new OptionMatcherAfterElse<T, TMatcher, TActionParam>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _action = Either<DelegateAction<Option<T>, TActionParam>, DelegateAction<TActionParam>>.Left(action),
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