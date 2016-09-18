using Smooth.Delegates;

namespace Smooth.Foundations.Foundations.PatternMatching.RefactoredMatcher
{
    public interface IWhereHandler<out TMatcher, out T>
    {
        TMatcher Do(DelegateAction<T> action);
    }

    public interface IWhereHandler<out TMatcher, out T1, out T2>
    {
        TMatcher Do(DelegateAction<T1, T2> action);
    }

    public interface IWhereHandler<out TMatcher, out T1, out T2, out T3>
    {
        TMatcher Do(DelegateAction<T1, T2, T3> action);
    }

    public interface IWhereHandler<out TMatcher, out T1, out T2, out T3, out T4>
    {
        TMatcher Do(DelegateAction<T1, T2, T3, T4> action);
    }

    public interface IWhereHandlerResult<out TMatcher, out T, in TResult>
    {
        TMatcher Return(DelegateFunc<T, TResult> func);
    }

    public interface IWhereHandlerResult<out TMatcher, out T1, out T2, in TResult>
    {
        TMatcher Return(DelegateFunc<T1, T2, TResult> func);
    }

    public interface IWhereHandlerResult<out TMatcher, out T1, out T2, out T3, in TResult>
    {
        TMatcher Return(DelegateFunc<T1, T2, T3, TResult> func);
    }

    public interface IWhereHandlerResult<out TMatcher, out T1, out T2, out T3, out T4, in TResult>
    {
        TMatcher Return(DelegateFunc<T1, T2, T3, T4, TResult> func);
    }
}