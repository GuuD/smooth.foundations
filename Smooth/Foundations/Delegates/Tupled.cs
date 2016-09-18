using System;
using Smooth.Algebraics;

namespace Smooth.Delegates {

	/// <summary>
	/// Provides methods for invoking a delegate that takes individual parameters using a tuple.
	/// 
	/// Many Smooth methods allow the use of an optional parameter that is passed to a delegate in order to capture state without allocating a closure.  One use case of this class is to convert a tupled state parameter into individual parameters.
	/// 
	/// For instance:
	/// 
	/// option.Where((player, p) => player.team == p.Item1 && player.score > p.Item2, Tuple.Create(myTeam, myScore))
	/// 
	/// Could be written as:
	/// 
	/// option.Where((v, p) => Tupled.Call((player, team, score) => player.team == team && player.score > score, v, p), Tuple.Create(myTeam, myScore))
	/// 
	/// While this form is slightly longer and requires an extra method call, it can be useful as a "quick and dirty" way to use meaningful parameter names in complicated delegates.
	/// </summary>
	public static class Tupled {

		#region Action, Tuple

		public static void Call<T1>(this DelegateAction<T1> action, Tuple<T1> t) {
			action(t.Item1);
		}

		public static void Call<T1, T2>(this DelegateAction<T1, T2> action, Tuple<T1, T2> t) {
			action(t.Item1, t.Item2);
		}
		
		public static void Call<T1, T2, T3>(this DelegateAction<T1, T2, T3> action, Tuple<T1, T2, T3> t) {
			action(t.Item1, t.Item2, t.Item3);
		}
		
		public static void Call<T1, T2, T3, T4>(this DelegateAction<T1, T2, T3, T4> action, Tuple<T1, T2, T3, T4> t) {
			action(t.Item1, t.Item2, t.Item3, t.Item4);
		}
		
		public static void Call<T1, T2, T3, T4, T5>(this DelegateAction<T1, T2, T3, T4, T5> action, Tuple<T1, T2, T3, T4, T5> t) {
			action(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
		}
		
		public static void Call<T1, T2, T3, T4, T5, T6>(this DelegateAction<T1, T2, T3, T4, T5, T6> action, Tuple<T1, T2, T3, T4, T5, T6> t) {
			action(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
		}
		
		public static void Call<T1, T2, T3, T4, T5, T6, T7>(this DelegateAction<T1, T2, T3, T4, T5, T6, T7> action, Tuple<T1, T2, T3, T4, T5, T6, T7> t) {
			action(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);
		}
		
		public static void Call<T1, T2, T3, T4, T5, T6, T7, T8>(this DelegateAction<T1, T2, T3, T4, T5, T6, T7, T8> action, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> t) {
			action(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8);
		}
		
		public static void Call<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this DelegateAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> t) {
			action(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9);
		}

		#endregion

		#region Action, First, Tuple

		public static void Call<F, T1>(this DelegateAction<F, T1> action, F first, Tuple<T1> t) {
			action(first, t.Item1);
		}
		
		public static void Call<F, T1, T2>(this DelegateAction<F, T1, T2> action, F first, Tuple<T1, T2> t) {
			action(first, t.Item1, t.Item2);
		}
		
		public static void Call<F, T1, T2, T3>(this DelegateAction<F, T1, T2, T3> action, F first, Tuple<T1, T2, T3> t) {
			action(first, t.Item1, t.Item2, t.Item3);
		}
		
		public static void Call<F, T1, T2, T3, T4>(this DelegateAction<F, T1, T2, T3, T4> action, F first, Tuple<T1, T2, T3, T4> t) {
			action(first, t.Item1, t.Item2, t.Item3, t.Item4);
		}
		
		public static void Call<F, T1, T2, T3, T4, T5>(this DelegateAction<F, T1, T2, T3, T4, T5> action, F first, Tuple<T1, T2, T3, T4, T5> t) {
			action(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
		}
		
		public static void Call<F, T1, T2, T3, T4, T5, T6>(this DelegateAction<F, T1, T2, T3, T4, T5, T6> action, F first, Tuple<T1, T2, T3, T4, T5, T6> t) {
			action(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
		}
		
		public static void Call<F, T1, T2, T3, T4, T5, T6, T7>(this DelegateAction<F, T1, T2, T3, T4, T5, T6, T7> action, F first, Tuple<T1, T2, T3, T4, T5, T6, T7> t) {
			action(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);
		}
		
		public static void Call<F, T1, T2, T3, T4, T5, T6, T7, T8>(this DelegateAction<F, T1, T2, T3, T4, T5, T6, T7, T8> action, F first, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> t) {
			action(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8);
		}

		#endregion

		#region Func, Tuple

		public static R Call<T1, R>(this DelegateFunc<T1, R> func, Tuple<T1> t) {
			return func(t.Item1);
		}
		
		public static R Call<T1, T2, R>(this DelegateFunc<T1, T2, R> func, Tuple<T1, T2> t) {
			return func(t.Item1, t.Item2);
		}
		
		public static R Call<T1, T2, T3, R>(this DelegateFunc<T1, T2, T3, R> func, Tuple<T1, T2, T3> t) {
			return func(t.Item1, t.Item2, t.Item3);
		}
		
		public static R Call<T1, T2, T3, T4, R>(this DelegateFunc<T1, T2, T3, T4, R> func, Tuple<T1, T2, T3, T4> t) {
			return func(t.Item1, t.Item2, t.Item3, t.Item4);
		}
		
		public static R Call<T1, T2, T3, T4, T5, R>(this DelegateFunc<T1, T2, T3, T4, T5, R> func, Tuple<T1, T2, T3, T4, T5> t) {
			return func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
		}
		
		public static R Call<T1, T2, T3, T4, T5, T6, R>(this DelegateFunc<T1, T2, T3, T4, T5, T6, R> func, Tuple<T1, T2, T3, T4, T5, T6> t) {
			return func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
		}
		
		public static R Call<T1, T2, T3, T4, T5, T6, T7, R>(this DelegateFunc<T1, T2, T3, T4, T5, T6, T7, R> func, Tuple<T1, T2, T3, T4, T5, T6, T7> t) {
			return func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);
		}
		
		public static R Call<T1, T2, T3, T4, T5, T6, T7, T8, R>(this DelegateFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> func, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> t) {
			return func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8);
		}
		
		public static R Call<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this DelegateFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> t) {
			return func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8, t.Item9);
		}

		#endregion

		#region Func, First, Tuple

		public static R Call<F, T1, R>(this DelegateFunc<F, T1, R> func, F first, Tuple<T1> t) {
			return func(first, t.Item1);
		}
		
		public static R Call<F, T1, T2, R>(this DelegateFunc<F, T1, T2, R> func, F first, Tuple<T1, T2> t) {
			return func(first, t.Item1, t.Item2);
		}
		
		public static R Call<F, T1, T2, T3, R>(this DelegateFunc<F, T1, T2, T3, R> func, F first, Tuple<T1, T2, T3> t) {
			return func(first, t.Item1, t.Item2, t.Item3);
		}
		
		public static R Call<F, T1, T2, T3, T4, R>(this DelegateFunc<F, T1, T2, T3, T4, R> func, F first, Tuple<T1, T2, T3, T4> t) {
			return func(first, t.Item1, t.Item2, t.Item3, t.Item4);
		}
		
		public static R Call<F, T1, T2, T3, T4, T5, R>(this DelegateFunc<F, T1, T2, T3, T4, T5, R> func, F first, Tuple<T1, T2, T3, T4, T5> t) {
			return func(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);
		}
		
		public static R Call<F, T1, T2, T3, T4, T5, T6, R>(this DelegateFunc<F, T1, T2, T3, T4, T5, T6, R> func, F first, Tuple<T1, T2, T3, T4, T5, T6> t) {
			return func(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6);
		}
		
		public static R Call<F, T1, T2, T3, T4, T5, T6, T7, R>(this DelegateFunc<F, T1, T2, T3, T4, T5, T6, T7, R> func, F first, Tuple<T1, T2, T3, T4, T5, T6, T7> t) {
			return func(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7);
		}
		
		public static R Call<F, T1, T2, T3, T4, T5, T6, T7, T8, R>(this DelegateFunc<F, T1, T2, T3, T4, T5, T6, T7, T8, R> func, F first, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> t) {
			return func(first, t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8);
		}

        #endregion

        #region Predicates

	    public static bool Call<T1>(this Predicate<T1> predicate, Tuple<T1> tuple)
	    {
	        return predicate(tuple.Item1);
	    }
        public static bool Call<T1, T2>(this Predicate<T1, T2> predicate, Tuple<T1, T2> tuple)
        {
            return predicate(tuple.Item1, tuple.Item2);
        }
        public static bool Call<T1, T2, T3>(this Predicate<T1, T2, T3> predicate, Tuple<T1, T2, T3> tuple)
        {
            return predicate(tuple.Item1, tuple.Item2, tuple.Item3);
        }
        public static bool Call<T1, T2, T3, T4>(this Predicate<T1, T2, T3, T4> predicate, Tuple<T1, T2, T3, T4> tuple)
        {
            return predicate(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        #endregion

    }
}
