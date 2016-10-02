using Smooth.Algebraics;
using Smooth.Delegates;

namespace Smooth.PatternMatching.MatcherDelegates
{
    public delegate bool Evaluator<TMatcher>(ref TMatcher previous);

    public delegate bool Evaluator<TMatcher, TResult>(ref TMatcher previous, out TResult result);

    public delegate void ValueProvider<T, TMatcher>(ref TMatcher matcher, out T value);
}
