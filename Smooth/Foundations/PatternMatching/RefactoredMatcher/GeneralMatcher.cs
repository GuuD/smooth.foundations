using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Smooth.Delegates;
using Smooth.Foundations.Foundations.PatternMatching.RefactoredMatcher;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher
{
    public class GeneralMatcher<T> : IMatcher<T>, IWhereHandler<IMatcher<T>, T>, IWithHandler<IMatcher<T>, T>
    {

        private HashSet<T> _withValues = new HashSet<T>();
        private Predicate<T> _predicate;
        private DelegateAction<T> _elseAction = DefaultElseAction;

        private static readonly DelegateAction<T> DefaultElseAction =
            val => { throw new NoMatchException($"No match found for value {val}"); };

        public IWithHandler<IMatcher<T>, T> With(T value)
        {
            _withValues.Add(value);
            return this;
        }

        public IWhereHandler<IMatcher<T>, T> Where(Predicate<T> predicate)
        {
            throw new System.NotImplementedException();
        }

        public IExecutableMatcher Else(DelegateAction<T> elseAction)
        {
            throw new System.NotImplementedException();
        }

        public IExecutableMatcher IgnoreElse()
        {
            throw new System.NotImplementedException();
        }

        public void Exec()
        {
            throw new System.NotImplementedException();
        }

        public IWithHandler<IMatcher<T>, T> Or(T value)
        {
            throw new System.NotImplementedException();
        }

        IMatcher<T> IWhereHandler<IMatcher<T>, T>.Do(DelegateAction<T> action)
        {
            throw new System.NotImplementedException();
        }

        IMatcher<T> IWithHandler<IMatcher<T>, T>.Do(DelegateAction<T> action)
        {
            throw new System.NotImplementedException();
        }
    }
}