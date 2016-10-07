using System.Collections.Generic;
using Smooth.Delegates;
using Smooth.Foundations.Algebraics;
using Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.Option;
using Smooth.PatternMatching.MatcherDelegates;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs.ValueOrError
{
    #region Option Without Parameters
    public struct OfValueMatcher<T, TMatcher>
    {
        internal static OfValueMatcher<T, TMatcher> CreateSkip(ref TMatcher previousMatcher,
                                                                ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                Evaluator<TMatcher> evaluator)
        {
            return new OfValueMatcher<T, TMatcher>
            {
                _previous = previousMatcher,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _skip = true
            };
        }   

        internal static OfValueMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                        Evaluator<TMatcher> evaluator,
                                                        T value)
        {
            var matcher = new OfValueMatcher<T, TMatcher>
            {
                _values = ListPool<T>.Instance.Borrow(),
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
            matcher._values.Add(value);
            return matcher;
        }
        private static readonly Evaluator<OfValueMatcher<T, TMatcher>> OfEvaluator = Evaluate;
        private static readonly ValueProvider<ValueOrError<T>, OfValueMatcher<T, TMatcher>> OfValueProvider = GetValue;

        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private bool _skip;

        private List<T> _values;
        private DelegateAction<T> _action;

        public OfValueMatcher<T, TMatcher> Or(T value)
        {
            if (_skip)
            {
                return this;
            }
            _values.Add(value);
            return this;
        }

        public VoEMatcher<T, OfValueMatcher<T, TMatcher>> Do(DelegateAction<T> action)
        {
            if (!_skip)
            {
                _action = action;
            }
            return VoEMatcher<T, OfValueMatcher<T, TMatcher>>.Create(ref this, OfValueProvider, OfEvaluator, !_skip);
        }

        public VoEMatcher<T, OfValueMatcherParam<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
            TActionParam param)
        {
            var proxy = _skip 
                ? OfValueMatcherParam <T, TMatcher, TActionParam>.CreateSkip(ref _previous, _valueProvider, _evaluator) 
                : OfValueMatcherParam<T, TMatcher, TActionParam>.Create(ref _previous, _valueProvider, _evaluator, _values,
                action, param);
            var vp = OfValueMatcherParam<T, TMatcher, TActionParam>.OfValueProvider;
            var e = OfValueMatcherParam<T, TMatcher, TActionParam>.OfEvaluator;
            return VoEMatcher<T, OfValueMatcherParam<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e, !_skip);
        }

        private static bool Evaluate(ref OfValueMatcher<T, TMatcher> matcher)
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
            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                matcher._action(value);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfValueMatcher<T, TMatcher> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion

    #region Option With Action Parameter
    public struct OfValueMatcherParam<T, TMatcher, TActionParam>
    {

        internal static OfValueMatcherParam<T, TMatcher, TActionParam> CreateSkip(ref TMatcher previousMatcher,
            ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
            Evaluator<TMatcher> evaluator)
        {
            return new OfValueMatcherParam<T, TMatcher, TActionParam>
            {
                _previous = previousMatcher,
                _evaluator = evaluator,
                _valueProvider = valueProvider,
                _skip = true
            };
        }

        internal static OfValueMatcherParam<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                      ValueProvider<ValueOrError<T>, TMatcher> valueProvider,
                                                                      Evaluator<TMatcher> evaluator,
                                                                      List<T> values,
                                                                      DelegateAction<T, TActionParam> action,
                                                                      TActionParam param)
        {
            return new OfValueMatcherParam<T, TMatcher, TActionParam>
            {
                _values = values,
                _valueProvider = valueProvider,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _action = action,
                _param = param
            };
        }
        internal static readonly Evaluator<OfValueMatcherParam<T, TMatcher, TActionParam>> OfEvaluator = Evaluate;
        internal static readonly ValueProvider<ValueOrError<T>, OfValueMatcherParam<T, TMatcher, TActionParam>> OfValueProvider = GetValue;

        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<ValueOrError<T>, TMatcher> _valueProvider;
        private TMatcher _previous;
        private List<T> _values;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _param;
        private bool _skip;

        private static bool Evaluate(ref OfValueMatcherParam<T, TMatcher, TActionParam> matcher)
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

            ValueOrError<T> voe;
            matcher._valueProvider(ref m, out voe);
            var value = voe.Value;
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                matcher._action(value, matcher._param);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref OfValueMatcherParam<T, TMatcher, TActionParam> matcher, out ValueOrError<T> value)
        {
            matcher._valueProvider(ref matcher._previous, out value);
        }
    }
    #endregion
}