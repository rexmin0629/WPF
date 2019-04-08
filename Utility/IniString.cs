using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public class IniString
    {
        private string[] splitstr = { "\r\n", "\r", "\n" };
        public Dictionary<string, Dictionary<string, string>> Dic = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, string> Empty = new Dictionary<string, string>();


        public IniString(string msg)
        {
            string[] msgArr = msg.Split(splitstr, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < msgArr.Length; i++)
            {
                msgArr[i] = msgArr[i].Trim();
            }

            string nowSection = string.Empty;

            for (int i = 0; i < msgArr.Length; i++)
            {
                if (msgArr[i].Contains('[') && msgArr[i].Contains(']'))
                {//section
                    nowSection = msgArr[i].Replace("[", string.Empty).Replace("]", string.Empty);
                    if(!Dic.ContainsKey(nowSection)){
                        Dic.Add(nowSection, new Dictionary<string, string>());
                    }
                }
                else if (Dic.ContainsKey(nowSection))
                {
                    string[] kvp = msgArr[i].Split('=');
                    if(kvp.Length >= 2){                        
                             Dic[nowSection][kvp[0]] = kvp[1];                         
                    }
                }
            }
        }

        public string[] getSections()
        {
            return Dic.Keys.ToArray();
        }

        public Dictionary<string, string> getSectionValue(string section)
        {
            if (Dic.ContainsKey(section))
            {
                return Dic[section];
            }
            else
            {
                return Empty;
            }
        }

    }
}
