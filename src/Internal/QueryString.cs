using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Pispower.Video.RestfulApi.Internal
{
    /**
     * QueryString 类，该类封装了对于QueryString的一些常用操作的方法。 
     */
    public class QueryString
    {

        private readonly Dictionary<string, List<string>> _dic;

        public QueryString()
        {
            _dic = new Dictionary<string, List<string>>();
        }

        public QueryString(Dictionary<string, List<string>> dic)
        {
            // 创建一个新的 Dictionary，不影响传入的实体。
            _dic = dic.ToDictionary(item => item.Key, item => item.Value.ToList());
        }

        /**
         * 获得一个参数的值，如果有多个则返回第一个。
         */
        public string GetParameter(string key)
        {
            return _dic.ContainsKey(key) ? _dic[key][0] : null;
        }

        /**
         * 获得参数的所有值。
         */
        public string[] GetParameterValues(string key)
        {
            return _dic.ContainsKey(key) ? _dic[key].ToArray() : null;
        }

        /**
         * 往该实体中增加一个新的值。
         */
        public void Add(string key, string val)
        {
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, new List<string>());
            }
            _dic[key].Add(val);
            
        }

        /**
         * 获得当前QueryString的所有参数的名字。
         */
        public List<string> GetParameterNames()
        {
            return (from n in _dic select n.Key).ToList();
        }

        /**
         * 获取当前QueryString的字典。
         */
        public Dictionary<string, List<string>> GetDictionary()
        {
            return _dic.ToDictionary(item => item.Key, item => item.Value.ToList());
        }

        /**
         * 获得当前QueryString中经过排序的字典。
         */
        public Dictionary<string, List<string>> GetSortedDictionary()
        {
            return _dic.OrderBy(di => di.Key).ToDictionary(item => item.Key, item => item.Value.OrderBy(v => v).ToList());
        }

        /**
         * 获得该QueryString的字符串值。
         */
        public string GetString()
        {
            return ToString(_dic, false);
        }

        /**
         * 获得经过排序的当前QueryString的字符串值
         */
        public string GetSortedString()
        {
            return ToString(GetSortedDictionary(), false);
        }

        /**
         * 获得经过UTF8编码的当前QueryString的字符串的值。
         */
        public string GetEncodedString()
        {
            return ToString(_dic, true);
        }

        public static string ToString(Dictionary<string, List<string>> dic, Boolean encode)
        {
            var str = "";
            foreach (var key in dic.Keys)
            {
                foreach (var val in dic[key])
                {
                    if (str.Length != 0)
                        str += "&";
                    str += key + "=" + (encode ? HttpUtility.UrlEncode(val, Encoding.UTF8) : val);
                }
            }
            return str;
        }

        /**
         * 复制当前的QueryString
         */
        public QueryString Clone()
        {
            return new QueryString(_dic);
        }

        public override string ToString()
        {
            return GetString();
        }
    }
}
