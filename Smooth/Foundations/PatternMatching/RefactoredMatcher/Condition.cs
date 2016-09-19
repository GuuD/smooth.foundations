using System;
using System.Collections.Generic;
using Smooth.Algebraics;
using Smooth.Pools;
using Smooth.Slinq;
using Smooth.Delegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher
{
    internal struct Condition<T>
    {
        [Flags]
        private enum ConditionType
        {
            Value = 1 << 0,
            Predicate = 1 << 1
        }

        private readonly Delegates.Predicate<T> _predicate;
        private Option<Either<T, List<T>>> _value; 
        private readonly ConditionType _conditionType;

        internal Condition(Delegates.Predicate<T> predicate) : this()
        {
            _predicate = predicate;
            _conditionType = ConditionType.Predicate;;
        }

        internal Condition(T value) : this()
        {
            _value = Either<T, List<T>>.Left(value).ToSome();
            _conditionType = ConditionType.Value;
        }

        internal Condition(List<T> values) : this()
        {
            _value = Either<T, List<T>>.Right(values).ToSome();
            _conditionType = ConditionType.Value;
        }

        internal Condition(Delegates.Predicate<T> predicate, T value) : this()
        {
            _predicate = predicate;
            _value = Either<T, List<T>>.Left(value).ToSome();
            _conditionType = ConditionType.Predicate | ConditionType.Value;
        }

        internal Condition(Delegates.Predicate<T> predicate, List<T> values) : this()
        {
            _predicate = predicate;
            _value = Either<T, List<T>>.Right(values).ToSome();
            _conditionType = ConditionType.Predicate | ConditionType.Value;
        }

        internal bool Resolve(T valueToResolve)
        {
            if ((_conditionType & ConditionType.Predicate & ConditionType.Value) == _conditionType)
            {
                return ResolveValue(valueToResolve) &&
                       _predicate(valueToResolve);
            }
            var resolution = (_conditionType & ConditionType.Value) == _conditionType
                ? ResolveValue(valueToResolve)
                : _predicate(valueToResolve);
            Dispose();
            return resolution;
        }

        private bool ResolveValue(T valueToResolve)
        {
            return _value.Select((stored, resolving) => stored.isLeft
                ? Collections.EqualityComparer<T>.Default.Equals(stored.leftValue, resolving)
                : stored.rightValue.Slinq()
                    .Any((v, r) => Collections.EqualityComparer<T>.Default.Equals(v, r), resolving), valueToResolve)
                .ValueOr(true);
        }

        private void Dispose()
        {
            _value.ForEach(v => v.ForEach(_ => { }, l => ListPool<T>.Instance.Release(l)));
        }
    }
}
