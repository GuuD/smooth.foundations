using Smooth.Delegates;

namespace Smooth.Foundations.Foundations.PatternMatching.RefactoredMatcher
{
    public interface IExecutableMatcher
    {
        void Exec();
    }

    public interface IExecutableMatcherResult<out TResult>
    {
        TResult Result();
    }

    public interface IMatcher<T1> : IExecutableMatcher
    {
        IWithHandler<IMatcher<T1>, T1> With(T1 value);
        IWhereHandler<IMatcher<T1>, T1> Where(Predicate<T1> predicate);
        IExecutableMatcher Else(DelegateAction<T1> elseAction);
        IExecutableMatcher IgnoreElse();
    }

    public interface IMatcher<T1, T2> : IExecutableMatcher
    {
        IWithHandler<IMatcher<T1, T2>, T1, T2> With(T1 value1, T2 value2);
        IWhereHandler<IMatcher<T1, T2>, T1, T2> Where(Predicate<T1, T2> predicate);
        IExecutableMatcher Else(DelegateAction<T1, T2> elseAction);
        IExecutableMatcher IgnoreElse();
    }

    public interface IMatcher<T1, T2, T3> : IExecutableMatcher
    {
        IWithHandler<IMatcher<T1, T2, T3>, T1, T2, T3> With(T1 value1, T2 value2, T3 value);
        IWhereHandler<IMatcher<T1, T2, T3>, T1, T2, T3> Where(Predicate<T1, T2, T3> predicate);
        IExecutableMatcher Else(DelegateAction<T1, T2, T3> elseAction);
        IExecutableMatcher IgnoreElse();
    }

    public interface IMatcher<T1, T2, T3, T4> : IExecutableMatcher
    {
        IWithHandler<IMatcher<T1, T2, T3, T4>, T1, T2, T3, T4> With(T1 value1, T2 value2, T3 value, T4 value4);
        IWhereHandler<IMatcher<T1, T2, T3, T4>, T1, T2, T3, T4> Where(Predicate<T1, T2, T3, T4> predicate);
        IExecutableMatcher Else(DelegateAction<T1, T2, T3, T4> elseAction);
        IExecutableMatcher IgnoreElse();
    }


    public interface IMatcherResult<T1, TResult> : IExecutableMatcherResult<TResult>
    {
        IWithHandlerResult<IMatcherResult<T1, TResult>, T1, TResult> With(T1 value);
        IWhereHandlerResult<IMatcherResult<T1, TResult>, T1, TResult> Where(Predicate<T1> predicate);
        IExecutableMatcherResult<TResult> Else(DelegateFunc<T1, TResult> elseResult);
        IExecutableMatcherResult<TResult> Else(TResult result);
    }

    public interface IMatcherResult<T1, T2, TResult> : IExecutableMatcherResult<TResult>
    {
        IWithHandlerResult<IMatcherResult<T1, T2, TResult>, T1, T2> With(T1 value1, T2 value2);
        IWhereHandlerResult<IMatcherResult<T1, T2, TResult>, T1, T2> Where(Predicate<T1, T2> predicate);
        IExecutableMatcherResult<TResult> Else(DelegateFunc<T1, T2, TResult> elseResult);
        IExecutableMatcherResult<TResult> Else(TResult result);
    }

    public interface IMatcherResult<T1, T2, T3, TResult> : IExecutableMatcherResult<TResult>
    {
        IWithHandlerResult<IMatcherResult<T1, T2, T3, TResult>, T1, T2, T3> With(T1 value1, T2 value2, T3 value);
        IWhereHandlerResult<IMatcherResult<T1, T2, T3, TResult>, T1, T2, T3> Where(Predicate<T1, T2, T3> predicate);
        IExecutableMatcherResult<TResult> Else(DelegateFunc<T1, T2, T3, TResult> elseResult);
        IExecutableMatcherResult<TResult> Else(TResult result);
    }

    public interface IMatcherResult<T1, T2, T3, T4, TResult> : IExecutableMatcherResult<TResult>
    {
        IWithHandlerResult<IMatcherResult<T1, T2, T3, T4, TResult>, T1, T2, T3, T4> With(T1 value1, T2 value2, T3 value, T4 value4);
        IWhereHandlerResult<IMatcherResult<T1, T2, T3, T4, TResult>, T1, T2, T3, T4> Where(Predicate<T1, T2, T3, T4> predicate);
        IExecutableMatcherResult<TResult> Else(DelegateFunc<T1, T2, T3, T4, TResult> elseResult);
        IExecutableMatcherResult<TResult> Else(TResult result);
    }
}
