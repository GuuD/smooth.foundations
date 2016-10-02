using System.Collections.Generic;
using System.Linq;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.PatternMatching.MatcherDelegates;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{
    #region General Without Parameters
    public struct WithMatcher<T, TMatcher>
    {
        internal static WithMatcher<T, TMatcher> Create(ref TMatcher previousMatcher,
                                                        ValueProvider<T, TMatcher> extractor,
                                                        Evaluator<TMatcher> evaluator, 
                                                        T value)
        {
            var matcher = new WithMatcher<T, TMatcher>
            {
                _values = ListPool<T>.Instance.BorrowConditional(l => l.Capacity >= 4, () => new List<T>(6)),
                _extractor = extractor,
                _evaluator = evaluator,
                _previous = previousMatcher
            };
            matcher._values.Add(value);
            return matcher;
        }
        private static readonly Evaluator<WithMatcher<T, TMatcher>> WithEvaluator = Evaluate;
        private static readonly ValueProvider<T, WithMatcher<T, TMatcher>> WithValueProvider = GetValue;

        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _extractor;
        private TMatcher _previous;

        private List<T> _values;
        private DelegateAction<T> _action;

        public WithMatcher<T, TMatcher> Or(T value)
        {
            _values.Add(value);
            return this;
        }

        public GeneralMatcher<T, WithMatcher<T, TMatcher>> Do(DelegateAction<T> action)
        {
            _action = action;
            return GeneralMatcher<T, WithMatcher<T, TMatcher>>.Create(ref this, WithValueProvider, WithEvaluator);
        }

        public GeneralMatcher<T, WithMatcherParam<T, TMatcher, TActionParam>> Do<TActionParam>(DelegateAction<T, TActionParam> action,
            TActionParam param)
        {
            var proxy = WithMatcherParam<T, TMatcher, TActionParam>.Create(ref _previous, _extractor, _evaluator, _values,
                action, param);
            var vp = WithMatcherParam<T, TMatcher, TActionParam>.WithValueProvider;
            var e = WithMatcherParam<T, TMatcher, TActionParam>.WithEvaluator;
            return GeneralMatcher<T, WithMatcherParam<T, TMatcher, TActionParam>>.Create(ref proxy, vp, e);
        }

        private static bool Evaluate(ref WithMatcher<T, TMatcher> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._extractor(ref m, out value);         
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                matcher._action(value);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref WithMatcher<T, TMatcher> matcher, out T value)
        {
            matcher._extractor(ref matcher._previous, out value);
        }
    }
    #endregion

    #region General With Action Parameter
    public struct WithMatcherParam<T, TMatcher, TActionParam>
    {
        internal static WithMatcherParam<T, TMatcher, TActionParam> Create(ref TMatcher previousMatcher,
                                                                      ValueProvider<T, TMatcher> extractor,
                                                                      Evaluator<TMatcher> evaluator,
                                                                      List<T> values,
                                                                      DelegateAction<T, TActionParam> action,
                                                                      TActionParam param)
        {
            var matcher = new WithMatcherParam<T, TMatcher, TActionParam>
            {
                _values = values,
                _extractor = extractor,
                _evaluator = evaluator,
                _previous = previousMatcher,
                _action = action,
                _param = param
            };
            return matcher;
        }
        internal static readonly Evaluator<WithMatcherParam<T, TMatcher, TActionParam>> WithEvaluator = Evaluate;
        internal static readonly ValueProvider<T, WithMatcherParam<T, TMatcher, TActionParam>> WithValueProvider = GetValue;

        private Evaluator<TMatcher> _evaluator;
        private ValueProvider<T, TMatcher> _extractor;
        private TMatcher _previous;
        private List<T> _values;
        private DelegateAction<T, TActionParam> _action;
        private TActionParam _param;

        private static bool Evaluate(ref WithMatcherParam<T, TMatcher, TActionParam> matcher)
        {
            var m = matcher._previous;
            var intermediateResult = matcher._evaluator(ref m);
            if (intermediateResult)
            {
                return true;
            }

            T value;
            matcher._extractor(ref m, out value);
            var result = matcher._values.Slinq().Contains(value);
            if (result)
            {
                matcher._action(value, matcher._param);
            }
            ListPool<T>.Instance.Release(matcher._values);
            return result;
        }
        private static void GetValue(ref WithMatcherParam<T, TMatcher, TActionParam> matcher, out T value)
        {
            matcher._extractor(ref matcher._previous, out value);
        }
    }
    #endregion
}
