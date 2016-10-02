using System.Runtime.InteropServices;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{
    internal struct BasicContainer<T>
    {
        internal static BasicContainer<T> Create(T value)
        {
            return new BasicContainer<T> {_value = value};
        }
        internal static ValueProvider<T, BasicContainer<T>> Provider = (ref BasicContainer<T> matcher, out T value) => value = matcher._value;
        internal static Evaluator<BasicContainer<T>> Evaluator = (ref BasicContainer<T> previous) => false; 
        private T _value;
    }

    public struct GeneralMatcher<T, TMatcher>
    {
        private static readonly DelegateAction<T> EmptyAction = _ => { }; 
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;

        internal static GeneralMatcher<T, TMatcher> Create(ref TMatcher previous, ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator)
        {
            return new GeneralMatcher<T, TMatcher>
            {
                _previous = previous,
                _evaluator = evaluator,
                _valueProvider = valueProvider
            };
        }

        public WhereMatcher<T, TMatcher> Where(Predicate<T> predicate)
        {
            return WhereMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, predicate);
        }

        public WhereMatcher<T, TMatcher, P> Where<P>(Predicate<T, P> predicate, P param)
        {
            return WhereMatcher<T, TMatcher, P>.Create(ref _previous, _valueProvider, _evaluator, predicate, param);
        }

        public WithMatcher<T, TMatcher> With(T value)
        {
            return WithMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, value);
        }

        public GeneralMatcherAfterElse<T, TMatcher> Else(DelegateAction<T> elseAction)
        {
            return GeneralMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, elseAction);
        }

        public GeneralMatcherAfterElse<T, TMatcher, TActionParam> Else<TActionParam>(
            DelegateAction<T, TActionParam> elseAction, TActionParam param)
        {
            return GeneralMatcherAfterElse<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator,
                elseAction, param);
        } 

        public GeneralMatcherAfterElse<T, TMatcher> IgnoreElse()
        {
            return GeneralMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, EmptyAction);
        }

        public void Exec()
        {
            T value;
            if (_evaluator(ref _previous)) return;
            _valueProvider(ref _previous, out value);
            throw new NoMatchException("No match found for " + value);
        }
    }

    public struct GeneralMatcherAfterElse<T, TMatcher>
    {
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private DelegateAction<T> _elseAction; 

        internal static GeneralMatcherAfterElse<T, TMatcher> Create(ref TMatcher previous, ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator, DelegateAction<T> elseAction)
        {
            return new GeneralMatcherAfterElse<T, TMatcher>
            {
                _previous = previous,
                _evaluator = evaluator,
                _valueProvider = valueProvider,
                _elseAction = elseAction
            };
        }

        public void Exec()
        {
            T value;
            if (_evaluator(ref _previous)) return;
            _valueProvider(ref _previous, out value);
            _elseAction(value);
        }
    }

    public struct GeneralMatcherAfterElse<T, TMatcher, TActionParam>
    {
        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private DelegateAction<T, TActionParam> _elseAction;
        private TActionParam _param;

        internal static GeneralMatcherAfterElse<T, TMatcher, TActionParam> Create(ref TMatcher previous, 
                                                                                     ValueProvider<T, TMatcher> valueProvider,
                                                                                     Evaluator<TMatcher> evaluator, 
                                                                                     DelegateAction<T, TActionParam> elseAction,
                                                                                     TActionParam param)
        {
            return new GeneralMatcherAfterElse<T, TMatcher, TActionParam>
            {
                _previous = previous,
                _evaluator = evaluator,
                _valueProvider = valueProvider,
                _elseAction = elseAction,
                _param = param
            };
        }

        public void Exec()
        {
            T value;
            if (_evaluator(ref _previous)) return;
            _valueProvider(ref _previous, out value);
            _elseAction(value, _param);
        }
    }
}