using System.Collections.Generic;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option
{
    #region Option Without Parameters
    public struct OfOptionMatcher<T, TMatcher>
    {
        internal static OfOptionMatcher<T, TMatcher> CreateSkip(ref TMatcher previousMatcher,
                                                                ValueProvider<T, TMatcher> valueProvider,
                                                                Evaluator<TMatcher> evaluator)
        {
            return new OfOptionMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }   

        internal static OfOptionMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<T, TMatcher> valueProvider,
                                                        Evaluator<TMatcher> evaluator,
                                                        T value)
        {
            var matcher = new OfOptionMatcher<T, TMatcher>
            {
                _values = ListPool<T>.Instance.Borrow(),
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
            matcher._values.Add(value);
            return matcher;
        }
        private static readonly Evaluator<OfOptionMatcher<T, TMatcher>> OfEvaluator = Evaluate;
        private static readonly ValueProvider<T, OfOptionMatcher<T, TMatcher>> OfValueProvider = GetValue;

        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;

        private List<T> _values;
        private DelegateAction<T> _action;

        public OfOptionMatcher<T, TMatcher> Or(T value)
        {
            if (_skip)
            {
                return this;
            }
            _values.Add(value);
            return this;
        }

        public OptionMatcher<T, OfOptionMatcher<T, TMatcher>> Do(DelegateAction<T> action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return OptionMatcher<T, OfOptionMatcher<T, TMatcher>>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public OptionMatcher<T, OfOptionMatcherParam<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
            TActionParam param)
        {
            var proxy = _skip 
                ? OfOptionMatcherParam <T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator) 
                : OfOptionMatcherParam<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, _values,
                action, param);
            var vp = OfOptionMatcherParam<T, TMatcher, TActionParam>.OfValueProvider;
            var e = OfOptionMatcherParam<T, TMatcher, TActionParam>.OfEvaluator;
            return OptionMatcher<T, OfOptionMatcherParam<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, !_skip);
        }

        private static bool Evaluate(ref OfOptionMatcher<T, TMatcher> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
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
                matcher._action(value);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfOptionMatcher<T, TMatcher> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region Option With Action Parameter
    public struct OfOptionMatcherParam<T, TMatcher, TActionParam>
    {

        internal static OfOptionMatcherParam<T, TMatcher, TActionParam> CreateSkip(ref TMatcher previousMatcher,
            ValueProvider<T, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator)
        {
            return new OfOptionMatcherParam<T, TMatcher, TActionParam>
            {
                _previous = previousMatcher,
                _evaluator = evaluator,
                _valueProvider = valueProvider,
                _skip = true
            };
        }

        internal static OfOptionMatcherParam<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                      ValueProvider<T, TMatcher> valueProvider,
                                                                      Evaluator<TMatcher> evaluator,
                                                                      List<T> values,
                                                                      DelegateAction<T, TActionParam> action,
                                                                      TActionParam param)
        {
            return new OfOptionMatcherParam<T, TMatcher, TActionParam>
            {
                _values = values,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _action = action,
                _param = param
            };
        }
        internal static readonly Evaluator<OfOptionMatcherParam<T, TMatcher, TActionParam>> OfEvaluator = Evaluate;
        internal static readonly ValueProvider<T, OfOptionMatcherParam<T, TMatcher, TActionParam>> OfValueProvider = GetValue;

        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;
        private List<T> _values;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _param;
        private bool _skip;

        private static bool Evaluate(ref OfOptionMatcherParam<T, TMatcher, TActionParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
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
                matcher._action(value, matcher._param);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfOptionMatcherParam<T, TMatcher, TActionParam> matcher, out T value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}