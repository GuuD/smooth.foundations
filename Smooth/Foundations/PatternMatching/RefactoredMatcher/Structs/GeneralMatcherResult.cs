using Smooth.PatternMatching.MatcherDelegates;

namespace Smooth.Foundations.PatternMatching.RefactoredMatcher.Structs
{
    public struct GeneralMatcherResult<T, TMatcher, TResult>
    {
        private Evaluator<TMatcher, TResult> _evaluator;
        private ValueProvider<T, TMatcher> _valueProvider;
        private TMatcher _previous;

        internal static GeneralMatcherResult<T, TMatcher, TResult> Create(ref TMatcher previous, 
                                                                ValueProvider<T, TMatcher> valueProvider,
                                                                Evaluator<TMatcher, TResult> evaluator)
        {
            return new GeneralMatcherResult<T, TMatcher, TResult>
            {
                _previous = previous,
                _evaluator = evaluator,
                _valueProvider = valueProvider
            };
        }

        //public WhereMatcher<T, TMatcher> Where(Predicate<T> predicate)
        //{
        //    return WhereMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, predicate);
        //}

        //public WhereMatcher<T, TMatcher, P> Where<P>(Predicate<T, P> predicate, P param)
        //{
        //    return WhereMatcher<T, TMatcher, P>.Create(ref _previous, _valueProvider, _evaluator, predicate, param);
        //}

        //public WithMatcher<T, TMatcher> With(T value)
        //{
        //    return WithMatcher<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, value);
        //}

        //public ExecutableMatcherAfterElse<T, TMatcher> Else(DelegateAction<T> elseAction)
        //{
        //    return ExecutableMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, elseAction);
        //}

        //public ExecutableMatcherAfterElse<T, TMatcher> IgnoreElse()
        //{
        //    return ExecutableMatcherAfterElse<T, TMatcher>.Create(ref _previous, _valueProvider, _evaluator, EmptyAction);
        //}

        //public void Exec()
        //{
        //    Option<bool> result;
        //    T value;
        //    do
        //    {
        //        result = _evaluator(ref _previous);
        //        if (result.isSome && result.value)
        //        {
        //            return;
        //        }
        //    } while (result.isSome);
        //    // We didn't find the match
        //    _valueProvider(ref _previous, out value);
        //    throw new NoMatchException("No match found for " + value);
        //}
    }

    //public struct ExecutableMatcherAfterElse<T, TMatcher>
    //{
    //    private Evaluator<TMatcher> _evaluator;
    //    private ValueProvider<T, TMatcher> _valueProvider;
    //    private TMatcher _previous;
    //    private DelegateAction<T> _elseAction;

    //    internal static ExecutableMatcherAfterElse<T, TMatcher> Create(ref TMatcher previous, ValueProvider<T, TMatcher> valueProvider,
    //        Evaluator<TMatcher> evaluator, DelegateAction<T> elseAction)
    //    {
    //        return new ExecutableMatcherAfterElse<T, TMatcher>
    //        {
    //            _previous = previous,
    //            _evaluator = evaluator,
    //            _valueProvider = valueProvider,
    //            _elseAction = elseAction
    //        };
    //    }

    //    public void Exec()
    //    {
    //        Option<bool> result;
    //        T value;
    //        _valueProvider(ref _previous, out value);
    //        do
    //        {
    //            result = _evaluator(ref _previous);
    //            if (result.isSome && result.value)
    //            {
    //                return;
    //            }
    //        } while (result.isSome);
    //        // We didn't find the match
    //        _elseAction(value);
    //    }
    //}

    //public struct ExecutableMatcherAfterElse<T, TMatcher, TActionParam>
    //{
    //    private Evaluator<TMatcher> _evaluator;
    //    private ValueProvider<T, TMatcher> _valueProvider;
    //    private TMatcher _previous;
    //    private DelegateAction<T, TActionParam> _elseAction;
    //    private TActionParam _param;

    //    internal static ExecutableMatcherAfterElse<T, TMatcher, TActionParam> Create(ref TMatcher previous,
    //                                                                                 ValueProvider<T, TMatcher> valueProvider,
    //                                                                                 Evaluator<TMatcher> evaluator,
    //                                                                                 DelegateAction<T, TActionParam> elseAction,
    //                                                                                 TActionParam param)
    //    {
    //        return new ExecutableMatcherAfterElse<T, TMatcher, TActionParam>
    //        {
    //            _previous = previous,
    //            _evaluator = evaluator,
    //            _valueProvider = valueProvider,
    //            _elseAction = elseAction,
    //            _param = param
    //        };
    //    }

    //    public void Exec()
    //    {
    //        Option<bool> result;
    //        T value;
    //        _valueProvider(ref _previous, out value);
    //        do
    //        {
    //            result = _evaluator(ref _previous);
    //            if (result.isSome && result.value)
    //            {
    //                return;
    //            }
    //        } while (result.isSome);
    //        // We didn't find the match
    //        _elseAction(value, _param);
    //    }
    //}
}
