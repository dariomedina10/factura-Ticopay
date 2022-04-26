using System.Collections.Generic;

namespace TicoPay.Socket
{
    public static class CacheHelper
    {
        static readonly Dictionary<string, int> Dictionary =
        new Dictionary<string, int>();


        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <param name="index">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Add(int index, string key)
        {
            lock (Dictionary)
            {
                Dictionary.Add(key, index);
            }
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public static void Clear(string key)
        {
            lock (Dictionary)
            {
                Dictionary.Remove(key);
            }
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            lock (Dictionary)
            {
                return Dictionary.ContainsKey(key);
            }
        }

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public static int Get(string key) 
        {
            try
            {
                lock (Dictionary)
                {
                    return Dictionary[key];
                }
            }
            catch
            {
                return -1;
            }
        }

        public static void Update(int index, string key)
        {
            try
            {
                lock (Dictionary)
                {
                    Dictionary[key] = index;
                }
                
            }
            catch
            {
              
            }
        }
    }
}
