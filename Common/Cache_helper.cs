﻿using System;
using System.Web;

namespace kyj_project.Common
{
    public class Cache_helper
    {
        /// <summary>  
        /// 获取数据缓存  
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        public static object GetCache(string cacheKey)
        {
            var objCache = HttpRuntime.Cache.Get(cacheKey);
            return objCache;
        }
        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        public static void SetCache(string cacheKey, object objObject)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject);
        }
        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        public static void SetCache(string cacheKey, object objObject, int timeout = 30)
        {
            try
            {
                if (objObject == null) return;
                var objCache = HttpRuntime.Cache;
                //相对过期  
                //objCache.Insert(cacheKey, objObject, null, DateTime.MaxValue,  new TimeSpan(0, 0, timeout), CacheItemPriority.NotRemovable, null);  
                //绝对过期时间  
                objCache.Insert(cacheKey, objObject, null, DateTime.Now.AddMinutes(timeout), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            }
            catch (Exception)
            {
                //throw;  
            }
        }
        /// <summary>  
        /// 移除指定数据缓存  
        /// </summary>  
        public static void RemoveAllCache(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            cache.Remove(cacheKey);
        }
        /// <summary>  
        /// 移除全部缓存  
        /// </summary>  
        public static void RemoveAllCache()
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
