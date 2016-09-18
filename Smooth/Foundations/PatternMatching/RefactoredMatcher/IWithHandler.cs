using Smooth.Delegates;

namespace Smooth.Foundations.Foundations.PatternMatching.RefactoredMatcher
{
    public interface IWithHandler<out TMatcher, T>
    {
        IWithHandler<TMatcher, T> Or(T value);
        TMatcher Do(DelegateAction<T> action);
    }

    public interface IWithHandler<out TMatcher, T1, T2>
    {
        IWithHandler<TMatcher, T1, T2> Or(T1 value1, T2 value2);
        TMatcher Do(DelegateAction<T1, T2> action);
    }

    public interface IWithHandler<out TMatcher, T1, T2, T3>
    {
        IWithHandler<TMatcher, T1, T2, T3> Or(T1 value1, T2 value2, T3 value3);
        TMatcher Do(DelegateAction<T1, T2, T3> action);
    }

    public interface IWithHandler<out TMatcher, T1, T2, T3, T4>
    {
        IWithHandler<TMatcher, T1, T2, T3, T4> Or(T1 value1, T2 value2, T3 value3, T4 value4);
        TMatcher Do(DelegateAction<T1, T2, T3, T4> action);
    }

    public interface IWithHandlerResult<out TMatcher, T, in TResult>
    {
        IWithHandlerResult<TMatcher, T, TResult> Or(T value);
        TMatcher Return(DelegateFunc<T, TResult> func);
    }

    public interface IWithHandlerResult<out TMatcher, T1, T2, in TResult>
    {
        IWithHandlerResult<TMatcher, T1, T2, TResult> Or(T1 value1, T2 value2);
        TMatcher Return(DelegateFunc<T1, T2, TResult> func);
    }

    public interface IWithHandlerResult<out TMatcher, T1, T2, T3, in TResult>
    {
        IWithHandlerResult<TMatcher, T1, T2, T3, TResult> Or(T1 value1, T2 value2, T3 value3);
        TMatcher Return(DelegateFunc<T1, T2, T3, TResult> func);
    }

    public interface IWithHandlerResult<out TMatcher, T1, T2, T3, T4, in TResult>
    {
        IWithHandlerResult<TMatcher, T1, T2, T3, T4, TResult> Or(T1 value1, T2 value2, T3 value3, T4 value4);
        TMatcher Return(DelegateFunc<T1, T2, T3, T4, TResult> func);
    }
}
