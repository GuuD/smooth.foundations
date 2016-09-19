using System.Collections.Generic;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Foundations.PatternMatching.RefactoredMatcher;
using Smooth.Pools;
using Smooth.Slinq;

namespace Smooth.Foundations.Foundations.PatternMatching.RefactoredMatcher
{
    internal class Resolver<T>
    {

        private readonly List<Tuple<Condition<T>, DelegateAction<T>>> _conditions = new List<Tuple<Condition<T>, DelegateAction<T>>>(7);

        internal void AddPredicateAndAction(Predicate<T> predicate, DelegateAction<T> action)
        {
            _conditions.Add(Tuple.Create(new Condition<T>(predicate), action));
        }

        internal void AddComplexPredicateAndAction(Predicate<T> predicate, T value, DelegateAction<T> action)
        {
            _conditions.Add(Tuple.Create(new Condition<T>(predicate, value), action));
        }

        internal void AddComplexPredicateAndAction(Predicate<T> predicate, List<T> values, DelegateAction<T> action)
        {
            _conditions.Add(Tuple.Create(new Condition<T>(predicate, values), action));
        }

        internal void AddValuePredicateAndAction(T value, DelegateAction<T> action)
        {
            _conditions.Add(Tuple.Create(new Condition<T>(value), action));
        }

        internal void AddValuesPredicateAndAction(List<T> values, DelegateAction<T> action)
        {
            _conditions.Add(Tuple.Create(new Condition<T>(values), action));
        }

        internal Option<DelegateAction<T>> Resolve(T value)
        {
            var result = _conditions.Slinq()
                .FirstOrNone((c, v) => c.Item1.Resolve(v), value)
                .Select(t => t.Item2);
            _conditions.Clear();
            return result;
        }

       
    }
}
