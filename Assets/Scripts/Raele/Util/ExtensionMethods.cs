using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raele.Util
{
    public static class ExtensionMethods
    {
        private const string DEFAULT_ASSERT_MESSAGE = "Assertion failed. (Invalid argument)";
		private const string DEFAULT_CAST_ERROR_MESSAGE = "Failed to cast {0} to {1}";
        public const string LOG_MESSAGE_WITH_CONTEXT = "{4}: '{2}' (of type '{3}')\n\nLogged by GameObject named '{0}' (component {1})\n";

		// -------------------------------------------------------------------------------------------------------------
		// CALLBACK INVOKE
		// -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static void SafeInvoke(this Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				ExtensionMethods.LogSafeInvokeErrorMessage(e);
			}
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static R? SafeInvokeNullable<R>(this Func<R> action) where R : struct
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				ExtensionMethods.LogSafeInvokeErrorMessage(e);
                return null;
			}
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static R SafeInvoke<R>(this Func<R> action) where R : class
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				ExtensionMethods.LogSafeInvokeErrorMessage(e);
                return null;
			}
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		private static void LogSafeInvokeErrorMessage(Exception e)
		{
			Debug.LogException(e);
			// REMOVE Debug.LogErrorFormat("Unhandled exception raised during callback: {0}\n{1}".FormatWith(e.Message, e.StackTrace));
		}

        // -------------------------------------------------------------------------------------------------------------
		// ENUMERABLE METHODS
		// -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static IEnumerable<T> Apply<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T t in source)
            {
                action(t);
                yield return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void ForEach<T>(this IEnumerable<T> source, Action action)
        {
            foreach (T t in source)
            {
                action();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T t in source)
            {
                action(t);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            IEnumerator<T> e = source.GetEnumerator();
            for (int i = 0; e.MoveNext(); i++)
            {
                action(e.Current, i);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void AddTo<T>(this IEnumerable<T> source, ICollection<T> collection)
        {
            foreach (T t in source)
            {
                collection.Add(t);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
            => source ?? new T[0];

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
            => source.Count == 0;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> TakeProportion<T>(this IEnumerable<T> source, float proportion, bool roundUp = true)
        {
            float n = source.Count() * Mathf.Clamp01(proportion);
            int takeCount = roundUp ? Mathf.CeilToInt(n) : Mathf.FloorToInt(n);
            return source.Take(takeCount);
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetGreater<T>(this IEnumerable<T> source, T initialValue, Func<T, int> comparer)
        {
            T best = initialValue;
            int bestValue = comparer(initialValue);
            foreach (T t in source)
            {
                int tValue = comparer(t);
                if (tValue > bestValue)
                {
                    best = t;
                    bestValue = tValue;
                }
            }
            return best;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetGreater<T>(this IEnumerable<T> source, Func<T, int> comparer)
        {
            T best = default(T);
            int bestValue = 0;
            bool firstEval = true;
            foreach (T t in source)
            {
                int tValue = comparer(t);
                if (firstEval || tValue > bestValue)
                {
                    best = t;
                    bestValue = tValue;
                    firstEval = false;
                }
            }
            return best;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetLower<T>(this IEnumerable<T> source, Func<T, int> comparer)
            => source.GetGreater(t => comparer(t) * -1);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T exception)
            => source.Except(exception.ToSingletonCollection());

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T exception, IEqualityComparer<T> comparer)
            => source.Except(exception.ToSingletonCollection(), comparer);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> ToSingletonCollection<T>(this T t)
        {
            yield return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Plus<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            foreach (T t in source)
            {
                yield return t;
            }

            foreach (T t in other)
            {
                yield return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Plus<T>(this IEnumerable<T> source, T addition)
        {
            foreach (T t in source)
            {
                yield return t;
            }

            yield return addition;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Without<T>(this IEnumerable<T> source, params T[] elements)
            => source.Where(element => !elements.Contains(element));

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Compact<T>(this IEnumerable<T> source)
            => source.Where(element => element != null && !element.Equals(default));

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
            => source.SelectMany(many => many);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> All<T>(this IEnumerator<T> enumerator)
        {
            enumerator.Reset();
            
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void Times(this int amount, Action action)
        {
            for (int i = 0; i < amount; i++)
            {
                action();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void Times(this int amount, Action<int> action)
        {
            for (int i = 0; i < amount; i++)
            {
                action(i);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Times<T>(this int amount, Func<T> action)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return action();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Times<T>(this int amount, Func<int, T> action)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return action(i);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> Times<T>(this int amount, T repeatingItem)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return repeatingItem;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Times<T>(this int amount, T initial, Func<T, T> action)
        {
            for (int i = 0; i < amount; i++)
            {
                initial = action(initial);
            }

            return initial;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Times<T>(this int amount, T initial, Func<T, int, T> action)
        {
            for (int i = 0; i < amount; i++)
            {
                initial = action(initial, i);
            }

            return initial;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> source, Func<T> provider)
            => source.Any() ? source : provider().ToSingletonCollection();

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> source)
            => source.ToDictionary(pair => pair.Key, pair => pair.Value);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Exception e)
        {
            try {
                return source.Single();
            } catch {
                throw e;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, string message)
        {
            try {
                return source.Single();
            } catch (Exception f) {
                throw new Exception(message, f);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<Exception> provider)
        {
            try {
                return source.Single();
            } catch {
                throw provider();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<string> provider)
        {
            try {
                return source.Single();
            } catch (Exception f) {
                throw new Exception(provider(), f);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<IEnumerable<T>, Exception> provider)
        {
            try {
                return source.Single();
            } catch {
                throw provider(source);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<IEnumerable<T>, string> provider)
        {
            try {
                return source.Single();
            } catch (Exception f) {
                throw new Exception(provider(source), f);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, Exception e)
        {
            try {
                return source.Single(predicate);
            } catch {
                throw e;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, string message)
        {
            try {
                return source.Single(predicate);
            } catch (Exception f) {
                throw new Exception(message, f);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<Exception> provider)
        {
            try {
                return source.Single(predicate);
            } catch {
                throw provider();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<string> provider)
        {
            try {
                return source.Single(predicate);
            } catch (Exception f) {
                throw new Exception(provider(), f);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<IEnumerable<T>, Exception> provider)
        {
            try {
                return source.Single(predicate);
            } catch {
                throw provider(source);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<IEnumerable<T>, string> provider)
        {
            try {
                return source.Single(predicate);
            } catch (Exception f) {
                throw new Exception(provider(source), f);
            }
        }

        // -------------------------------------------------------------------------------------------------------------
        // String Building
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static string Join(this IEnumerable source, string separator = " ")
            => string.Join(separator, source);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static string Join(this IEnumerable source, string separator, string prefix, string sufix)
        {
            StringBuilder builder = new StringBuilder();
            prefix.IfNotNullOrEmpty(() => builder.Append(prefix));

            var iterator = source.GetEnumerator();
            if (iterator.MoveNext())
            {
                builder.Append(iterator.Current);
            }
            while (iterator.MoveNext())
            {
                separator.IfNotNullOrEmpty(() => builder.Append(separator));
                builder.Append(iterator.Current);
            }

            sufix.IfNotNullOrEmpty(() => builder.Append(sufix));
            return builder.ToString();
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
		}

        // -------------------------------------------------------------------------------------------------------------
		// ARRAY HELPERS
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] Add<T>(this T[] source, T addition)
        {
            var list = source.ToList();
            list.Add(addition);
            return list.ToArray();
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T[] Add<T>(this T[] source, T[] collection)
        {
            var list = source.ToList();
            list.AddRange(collection);
            return list.ToArray();
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IsEmpty<T>(this T[] source)
        {
            return source.Length == 0;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T[] Shuffle<T>(this T[] source, System.Random random = null)
        {
            random = random ?? new System.Random();
            for (int i = source.Length - 1; i >= 0; i--)
            {
                int swapIndex = random.Next(i);
                T iValue = source[swapIndex];
                source[swapIndex] = source[i];
                source[i] = iValue;
            }
            return source;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetRandomElement<T>(this T[] source)
        {
            int randomIndex = UnityEngine.Random.Range(1, source.Length) - 1;
            
            return source[randomIndex];
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetRandomElement<T>(this T[] source, System.Random rng)
        {
            int randomIndex = rng.Next(source.Length);
            
            return source[randomIndex];
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetOrDefault<T>(this T[] source, int index)
        {
            if (source.Length > index)
            {
                return source[index];
            }
            else
            {
                return default;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetOrDefault<T>(this T[] source, int index, T defaultValue)
        {
            if (source.Length > index)
            {
                return source[index];
            }
            else
            {
                return defaultValue;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T GetOrDefault<T>(this T[] source, int index, Func<T> provider)
        {
            if (source.Length > index)
            {
                return source[index];
            }
            else
            {
                return provider();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool TryGet<T>(this T[] source, int index, out T output)
        {
            if (source.Length > index)
            {
                output = source[index];
                return true;
            }
            else
            {
                output = default;
                return false;
            }
        }

        // -------------------------------------------------------------------------------------------------------------
        // LIST OPERATIONS
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static List<T> Compact<T>(this List<T> source)
        {
            source.RemoveAll(default);
            return source;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Pop<T>(this LinkedList<T> source)
        {
            T result = source.Last.Value;
            source.RemoveLast();
            return result;
        }
        
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T PopOrDefault<T>(this LinkedList<T> source, T defaultValue = default)
        {
            return source.IsEmpty() ? defaultValue : source.Pop();
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static LinkedList<T> Push<T>(this LinkedList<T> source, T element)
        {
            source.AddLast(element);
            return source;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Shift<T>(this LinkedList<T> source)
        {
            T result = source.First.Value;
            source.RemoveFirst();
            return result;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ShiftOrDefault<T>(this LinkedList<T> source, T defaultValue = default)
        {
            return source.IsEmpty() ? defaultValue : source.Shift();
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static LinkedList<T> Unshift<T>(this LinkedList<T> source, T element)
        {
            source.AddFirst(element);
            return source;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static int RemoveIf<T>(this LinkedList<T> source, Func<T, bool> predicate)
        {
            int removalCount = 0;

            for (LinkedListNode<T> current = source.First; current != null; current = current.Next)
            {
                if (predicate(current.Value))
                {
                    current.List.Remove(current);
                    removalCount++;
                }
            }

            return removalCount;
        }
        
        /**
         * Get the first N elements of the list; or all elements of the list (possibly 0) if the list has fewer than or
         * exactly N elements.
         * The elements will be arranged in the same order they are available in the list. That means the first element
         * of the resulting range will be the first element of the source list; and the last element of the resulting
         * range will be the (N - 1)th element of the source range.
         */
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> GetStartingRange<T>(this LinkedList<T> source, int amount)
        {
            return source.Count <= amount
                ? source
                : (amount - 1).Times(
                    source.First.ToSingletonCollection().ToLinkedList(),
                    nodeList => nodeList.Push(nodeList.Last.Value.Next)
                )
                .Select(node => node.Value);
        }
        
        /**
         * Get the last N elements of the list; or all elements of the list (possibly 0) if the list has fewer than or
         * exactly N elements.
         * The elements will be arranged in the same order they are available in the list. That means the first element
         * of the resulting range will be the (N - 1)th element from the back to the front of the source list; and the
         * last element of the resulting range will be the last element of the source list.
         */
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> GetClosingRange<T>(this LinkedList<T> source, int amount)
        {
            return source.Count <= amount
                ? source
                : (amount - 1).Times(
                    source.Last.ToSingletonCollection().ToLinkedList(),
                    nodeList => nodeList.Unshift(nodeList.First.Value.Previous)
                )
                .Select(node => node.Value);
        }

        // -------------------------------------------------------------------------------------------------------------
        // DICTIONARY
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static TValue GetOrDefault<TValue, TKey>(this IDictionary<TKey, TValue> source, TKey key, TValue value = default)
            => source.TryGetValue(key, out TValue result) ? result : value;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static TValue GetOrInitialize<TValue, TKey>(this IDictionary<TKey, TValue> source, TKey key, TValue value = default)
            => source.TryGetValue(key, out TValue result) ? result : source[key] = value;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static TValue GetOrInitialize<TValue, TKey>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> provider)
            => source.TryGetValue(key, out TValue result) ? result : source[key] = provider();
        
        public delegate void ToPair<in T, K, V>(T item, out K key, out V value);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T> source, ToPair<T, K, V> provider)
        {
            Dictionary<K, V> result = new Dictionary<K, V>();
            
            source.ForEach(t =>
            {
                provider(t, out K k, out V v);
                result[k] = v;
            });

            return result;
        }

        // -------------------------------------------------------------------------------------------------------------
        // BASIC TYPES
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IsBetween(this int i, int lowerEnd, int higherEnd)
        {
            return lowerEnd <= i && i <= higherEnd;
        }

        // -------------------------------------------------------------------------------------------------------------
        // NULLABLE
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool TryGetValue<T>(this T? source, out T output) where T : struct
        {
            output = source.HasValue ? source.Value : default;
            return source.HasValue;
        }

		// -------------------------------------------------------------------------------------------------------------
		// SORTING
		// -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] Sort<T>(this IEnumerable<T> source)
		{
			return source.ToArray().Sort();
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] SortBy<T>(this IEnumerable<T> source, Func<T, int> comparer, bool ascending = true)
		{
			return source.ToArray().SortBy(comparer, ascending);
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] Sort<T>(this IEnumerable<T> source, Comparison<T> comparison)
		{
			return source.ToArray().Sort(comparison);
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] Sort<T>(this T[] source)
		{
			Array.Sort(source);
			return source;
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] SortBy<T>(this T[] source, Func<T, int> comparer, bool ascending = true)
		{
			int ordering = ascending.AsInt() * 2 - 1;
			Array.Sort(source, (a, b) => (comparer(a) - comparer(b)) * ordering);
			return source;
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T[] Sort<T>(this T[] source, Comparison<T> comparison)
		{
			Array.Sort(source, comparison);
			return source;
		}

        // -------------------------------------------------------------------------------------------------------------
        // CALLBACK METHODS
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void InvokeIf(this Action action, bool condition)
        {
            if (condition)
            {
                action();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void InvokeIf(this Action action, Func<bool> condition)
        {
            if (condition())
            {
                action();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void InvokeIf(this Action action, object o)
        {
            if (o.NotDefault())
            {
                action();
            }
        }

        // -------------------------------------------------------------------------------------------------------------
        // FUNCTIONAL PROGRAMMING HELPERS
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Then<T>(this T t, Action action) // TODO Shouldn't this be ThenDo instead? I think the implementations are the same
        {
            if (t != null && !t.Equals(default(T)))
            {
                action();
                return t;
            }
            else
            {
                return default(T);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Then<T>(this T t, Action<T> action)
        {
            if (t != null && !t.Equals(default(T)))
            {
                action(t);
                return t;
            }
            else
            {
                return default(T);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R Then<T, R>(this T t, Func<R> provider) // TODO Maybe should return R? instead so that there's no problem with default structs/primitives
        {
            if (t != null && !t.Equals(default(T)))
            {
                return provider();
            }
            else
            {
                return default(R);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R Then<T, R>(this T t, Func<T, R> provider)
        {
            if (t != null && !t.Equals(default(T)))
            {
                return provider(t);
            }
            else
            {
                return default(R);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R Then<T, R>(this T t, R result)
        {
            if (t != null && !t.Equals(default(T)))
            {
                return result;
            }
            else
            {
                return default(R);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, U condition, Action action)
        {
            if (t != null && !t.Equals(default(T))
                && condition != null && !condition.Equals(default(U)))
            {
                action();
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, Func<U> predicate, Action action)
        {
            U target = predicate();

            if (t != null && !t.Equals(default(T))
                && target != null && !target.Equals(default(U)))
            {
                action();
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, Func<T, U> predicate, Action action)
        {
            U target = predicate(t);

            if (t != null && !t.Equals(default(T))
                && target != null && !target.Equals(default(U)))
            {
                action();
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, U condition, Action<T> action)
        {
            if (t != null && !t.Equals(default(T))
                && condition != null && !condition.Equals(default(U)))
            {
                action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, Func<U> predicate, Action<T> action)
        {
            U target = predicate();

            if (t != null && !t.Equals(default(T))
                && target != null && !target.Equals(default(U)))
            {
                action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, Func<T, U> predicate, Action<T> action)
        {
            U target = predicate(t);

            if (t != null && !t.Equals(default(T))
                && target != null && !target.Equals(default(U)))
            {
                action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, U condition, Func<T, T> action)
        {
            if (t != null && !t.Equals(default(T))
                && condition != null && !condition.Equals(default(U)))
            {
                return action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, Func<U> predicate, Func<T, T> action)
        {
            U target = predicate();

            if (t != null && !t.Equals(default(T))
                && target != null && !target.Equals(default(U)))
            {
                return action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenIf<T, U>(this T t, Func<T, U> predicate, Func<T, T> action)
        {
            U target = predicate(t);

            if (t != null && !t.Equals(default(T))
                && target != null && !target.Equals(default(U)))
            {
                return action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenDo<T>(this T t, Action action)
        {
            if (t != null && !t.Equals(default(T)))
            {
                action();
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenDo<T>(this T t, Action<T> action)
        {
            if (t != null && !t.Equals(default(T)))
            {
                action(t);
            }
            
            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenThrow<T>(this T t, string message)
        {
            if (t != null && !t.Equals(default(T)))
            {
                throw new Exception(message);
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenThrow<T>(this T t, Func<string> messageProvider)
        {
            if (t != null && !t.Equals(default(T)))
            {
                string msg = messageProvider();
                throw new Exception(msg);
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenThrow<T>(this T t, Func<T, string> messageProvider)
        {
            if (t != null && !t.Equals(default(T)))
            {
                string msg = messageProvider(t);
                throw new Exception(msg);
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenThrow<T>(this T t, Exception exception)
        {
            if (t != null && !t.Equals(default(T)))
            {
                throw exception;
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenThrow<T, E>(this T t, Func<E> thrower) where E : Exception
        {
            if (t != null && !t.Equals(default(T)))
            {
                throw thrower();
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T ThenThrow<T, E>(this T t, Func<T, E> thrower) where E : Exception
        {
            if (t != null && !t.Equals(default(T)))
            {
                throw thrower(t);
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static void Otherwise<T>(this T t, Action action)
        {
            if (t == null || t.Equals(default(T)))
            {
                action();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Otherwise<T>(this T t, Func<T> provider)
        {
            if (t == null || t.Equals(default(T)))
            {
                return provider();
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Otherwise<T>(this T t, T result)
        {
            if (t == null || t.Equals(default(T)))
            {
                return result;
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T OtherwiseDo<T>(this T t, Action action)
        {
            if (t == null || t.Equals(default(T)))
            {
                action();
            }

            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T OtherwiseDo<T>(this T t, Action<T> action)
        {
            if (t == null || t.Equals(default(T)))
            {
                action(t);
            }

            return t;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T OtherwiseThrow<T, E>(this T t, Func<E> thrower) where E : Exception
        {
            if (t == null || t.Equals(default(T)))
            {
                throw thrower();
            }
            else
            {
                return t;
            }
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T OtherwiseThrow<T>(this T t, string message)
		{
			if (t == null || t.Equals(default(T)))
			{
				throw new UnityException(message);
			}
			else
			{
				return t;
			}
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T OtherwiseThrow<T>(this T t, Func<string> msgProvider)
        {
            if (t == null || t.Equals(default(T)))
            {
                throw new UnityException(msgProvider());
            }
            else
            {
                return t;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T AssignTo<T>(this T t, out T var)
            => var = t;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T As<T>(this T t, out T var) // Alias to AssignTo
            => var = t;

        // -------------------------------------------------------------------------------------------------------------
        // CAST
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Cast<T>(this object obj, string msg = null, params object[] format)
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw new Exception(msg?.FormatWith(format) ?? DEFAULT_CAST_ERROR_MESSAGE.FormatWith(obj.GetType(), typeof(T)));
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrDefault<T>(this object obj) where T : class
            => obj as T;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj) where T : class
            => (T) obj;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj, string message) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw new Exception(message);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj, Exception e) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw e;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj, Func<string> provider) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw new Exception(provider());
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj, Func<Exception> provider) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw provider();
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj, Func<object, string> provider) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw new Exception(provider(obj));
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T CastOrThrow<T>(this object obj, Func<object, Exception> provider) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch
            {
                throw provider(obj);
            }
        }

        // -------------------------------------------------------------------------------------------------------------
        // AND
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R And<T, R>(this T lp, R rp)
                => lp != null
                    && rp != null
                    && !lp.Equals(default(T))
                    && !rp.Equals(default(T))
                ? rp
                : default(R);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R And<T, R>(this T lp, Func<R> rpPromise)
            => lp != null && !lp.Equals(default(T))
                    && rpPromise()
                        .As(out R rp)
                        != null
                    && !rp.Equals(default(T))
                ? rp
                : default(R);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R And<T, R>(this T lp, Func<T, R> rpPromise)
            => lp != null && !lp.Equals(default(T))
                    && rpPromise(lp)
                        .As(out R rp)
                        != null
                    && !rp.Equals(default(T))
                ? rp
                : default(R);

        // -------------------------------------------------------------------------------------------------------------
        // VALIDATIONS
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IfNotNull<T>(this T t, Action action) where T : class
        {
            if (t != null)
            {
                action();
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IfNotNull<T>(this T t, Action<T> action) where T : class
        {
            if (t != null)
            {
                action(t);
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R IfNotNull<T, R>(this T t, Func<R> action) where T : class
                => t != null ? action() : default;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R IfNotNull<T, R>(this T t, Func<T, R> action) where T : class
                => t != null ? action(t) : default;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool NotNullOrEmpty(this string str)
                => !string.IsNullOrEmpty(str);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IfNotNullOrEmpty(this string str, Action action)
        {
            if (!string.IsNullOrEmpty(str))
            {
                action();
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IfNotNullOrEmpty(this string str, Action<string> action)
        {
            if (!string.IsNullOrEmpty(str))
            {
                action(str);
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R IfNotNullOrEmpty<R>(this string str, Func<R> action)
                => !string.IsNullOrEmpty(str) ? action() : default;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R IfNotNullOrEmpty<R>(this string str, Func<string, R> action)
                => !string.IsNullOrEmpty(str) ? action(str) : default;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IfNotNullOrEmpty<T>(this IReadOnlyCollection<T> source, Action action)
        {
            if (source != null && !source.IsEmpty())
            {
                action();
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IfNotNullOrEmpty<T>(this IReadOnlyCollection<T> source, Action<IReadOnlyCollection<T>> action)
        {
            if (source != null && !source.IsEmpty())
            {
                action(source);
                return true;
            }
            else
            {
                return false;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R IfNotNullOrEmpty<T, R>(this IReadOnlyCollection<T> source, Func<R> action)
        {
            if (source != null && !source.IsEmpty())
            {
                return action();
            }
            else
            {
                return default;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static R IfNotNullOrEmpty<T, R>(this IReadOnlyCollection<T> source, Func<IReadOnlyCollection<T>, R> action)
        {
            if (source != null && !source.IsEmpty())
            {
                return action(source);
            }
            else
            {
                return default;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool Differs(this object obj, object other)
				=> !obj?.Equals(other) ?? other != null;

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static bool IsDefault<T>(this T t)
				=> t == null || t.Equals(default(T));

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static bool NotDefault<T>(this T t)
				=> t != null && !t.Equals(default(T));

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool Not<T>(this T t)
            => !t.NotDefault();

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T Case<T>(this T t, T value, Action action)
		{
			action.InvokeIf(t?.Equals(value) ?? value == null);
			return t;
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T Case<T>(this T t, T value, Action<T> action)
				=> t.Case(value, () => action(t));

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
            => value.CompareTo(min) > 0 && value.CompareTo(max) < 0;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static bool IsEqualOrBetween<T>(this T value, T min, T max) where T : IComparable<T>
            => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

        // -------------------------------------------------------------------------------------------------------------
		// ASSERTIONS
        // -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static T Assert<T>(this T t, Func<T, bool> predicate, string msg = DEFAULT_ASSERT_MESSAGE)
		{
			if (predicate(t))
			{
				return t;
			}
			else
			{
				throw new UnityException(msg);
			}
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T Assert<T>(this T t, Func<T, bool> predicate, Func<string> msgProvider)
        {
            if (predicate(t))
            {
                return t;
            }
            else
            {
                throw new UnityException(msgProvider());
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T AssertNotDefault<T>(this T t, string msg = DEFAULT_ASSERT_MESSAGE)
        {
            if (t != null && !t.Equals(default(T)))
            {
                return t; 
            }
            else
            {
                throw new UnityException(msg);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T AssertNotDefault<T>(this T t, Func<string> msgProvider)
        {
            if (t != null && !t.Equals(default(T)))
            {
                return t; 
            }
            else
            {
                msgProvider
                    ?.Invoke()
                    ?.ThenThrow(R.Identity);
                throw new UnityException(DEFAULT_ASSERT_MESSAGE);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T AssertNotDefault<T>(this T t, Component context, string msg = DEFAULT_ASSERT_MESSAGE)
        {
            if (t != null && !t.Equals(default(T)))
            {
                return t; 
            }
            else
            {
                string message = LOG_MESSAGE_WITH_CONTEXT.FormatWith(
                    context.gameObject.name,
                    context.GetType().Name,
                    t,
                    typeof(T),
                    msg
                );
                throw new UnityException(message);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T AssertNotDefault<T>(this T t, Component context, Func<string> msgProvider)
        {
            if (t != null && !t.Equals(default(T)))
            {
                return t; 
            }
            else
            {
                string message = LOG_MESSAGE_WITH_CONTEXT.FormatWith(
                    context.gameObject.name,
                    context.GetType().Name,
                    t,
                    typeof(T),
                    msgProvider?.Invoke() ?? DEFAULT_ASSERT_MESSAGE
                );
                throw new UnityException(message);
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static IEnumerable<T> AssertNotNullOrEmpty<T>(this IEnumerable<T> source, string msg = DEFAULT_ASSERT_MESSAGE, params object[] format)
        {
            if (source == null || !source.Any())
            {
                throw new UnityException(string.Format(msg, format));
            }
            else
            {
                return source;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T[] AssertNotNullOrEmpty<T>(this T[] source, string msg = DEFAULT_ASSERT_MESSAGE)
        {
            if (source == null || source.IsEmpty())
            {
                throw new UnityException(msg);
            }
            else
            {
                return source;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T[] AssertNotNullOrEmpty<T>(this T[] source, Func<string> msgProvider)
        {
            if (source == null || source.IsEmpty())
            {
                throw new UnityException(msgProvider());
            }
            else
            {
                return source;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static string AssertNotNullOrEmpty(this string str, string msg = DEFAULT_ASSERT_MESSAGE)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new UnityException(msg);
            }
            else
            {
                return str;
            }
		}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static string AssertNotNullOrEmpty(this string str, Func<string> msgProvider)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new UnityException(msgProvider());
            }
            else
            {
                return str;
            }
        }

		// -------------------------------------------------------------------------------------------------------------
		// CONVERTIONS
		// -------------------------------------------------------------------------------------------------------------

        [System.Diagnostics.DebuggerStepThroughAttribute]
		public static int AsInt(this bool b) => b.GetHashCode();
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static float AsFloat(this int i) => i;
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static double AsDouble(this long l) => l;

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
            => new LinkedList<T>(source);

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public static T[,] To2DArray<T>(this T[][] source)
        {
            if (source.Length == 0)
            {
                return new T[0, 0];
            }
            
            int height = source.Select(ary => ary.Length).Max();

            T[,] result = new T[source.Length, height];

            for (int i = 0; i < source.Length; i++)
            {
                for (int j = 0; j < source[i].Length; j++)
                {
                    result[i, j] = source[i][j];
                }
            }

            return result;
        }

#if false
		// -------------------------------------------------------------------------------------------------------------
		// TEST SUITE
		// -------------------------------------------------------------------------------------------------------------

        static ExtensionMethods()
        {
            object nullObj = null;
            object obj = new object();
            string emptyStr = string.Empty;
            string nullStr = null;
            string str = "JUJUBA STRING";
            bool True = true;
            bool False = false;

            UnityEngine.Debug.Log("Starting ExtensionMethods module test suite...");

            // VALIDATIONS
            obj.IfNotNull(() => expect(true));
            nullObj.IfNotNull(() => expect(false));

            obj.IfNotNull(o => expect(o == obj));
            nullObj.IfNotNull(o => expect(false));

            expect(obj.AssertNotNull() == obj);
            try
            {
                nullObj.AssertNotNull("true is true");
                expect(false);
            }
            catch (UnityException e)
            {
                expect("true is true".Equals(e.Message));
            }

            expect(True.AssertTrue());
            try
            {
                False.AssertTrue("false is not true");
                expect(false);
            }
            catch (UnityException e)
            {
                expect("false is not true".Equals(e.Message));
            }

            UnityEngine.Debug.Log("Finished execution of ExtensionMethods module test suite.");
        }

        private static void expect(bool b, string msg = "JUJUBA ERROR!!", params object[] args)
        {
            if (!b)
            {
                UnityEngine.Debug.LogErrorFormat(msg, args);
            }
        }
#endif
	}
}
