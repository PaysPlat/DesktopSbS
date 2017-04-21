using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace DesktopSbS
{
    public class ConfFile
    {
        private Dictionary<string, string> m_params;
        private List<string> m_content;
        private string
            m_confFileLoadPath = string.Empty,
            m_confFileSavePath = string.Empty;
        private string[]
            m_separator = new string[] { ";" },
            m_comment = new string[] { "#" };

        private static string exePath = Util.ExePath;



        public List<string> Content
        {
            get { return m_content; }
        }

        public Dictionary<string, string> Params
        {
            get { return m_params; }
        }

        public ConfFile(string loadPath, string savePath, string separator, string comment, bool isAbsolutePath = false)
        {
            m_comment[0] = comment;
            m_separator[0] = separator;
            m_confFileSavePath = (isAbsolutePath ? "" : exePath) + savePath;
			loadFromFile(loadPath, true);
            
        }

        public ConfFile(string loadPath, string savePath, string separator, bool isAbsolutePath = false)
        {
             m_separator[0] = separator;
             m_confFileSavePath = (isAbsolutePath ? "" : exePath) + savePath;
			 loadFromFile(loadPath, true);
        }

        public ConfFile(string loadPath, string savePath, bool isAbsolutePath = false)
        {
            m_confFileSavePath = (isAbsolutePath ? "" : exePath) + savePath;
			loadFromFile(loadPath, true);
        }

        public ConfFile(string path, bool isAbsolutePath = false)
        {
            m_confFileSavePath = (isAbsolutePath ? "" : exePath) + path;
            loadFromFile(m_confFileSavePath, true);
        }

        public ConfFile()
        {
        }


        private void initData()
        {
            initData(true);
        }

        private void initData(bool erase)
        {
            if (erase || m_params == null) m_params = new Dictionary<string, string>();
            if (erase || m_content == null) m_content = new List<string>();

        }


        public bool loadFromFile(string path, bool isAbsolutePath = false)
        {
            m_confFileLoadPath = (isAbsolutePath ? "" : exePath) + path;
            initData();
            StreamReader file=null;
			if (File.Exists(m_confFileLoadPath))
			{
				try
				{
					file = new StreamReader(m_confFileLoadPath);
				}
				catch
				{
					return false;
				}
			}
			else
			{
				return false;
			}
            string line = file.ReadLine();
            while (line != null)
            {
                line = line.Trim(new char[] { '\t', ' ' });
                m_content.Add(line);
                line = line.Split(m_comment, StringSplitOptions.None)[0];
                if (line != string.Empty)
                {
                    string[] tab_line = line.Split(m_separator, StringSplitOptions.None);
                    string key = tab_line[0];
                    string content = string.Empty;
                    if (tab_line.Length > 1)
                    {
                        content = line.Substring(key.Length + m_separator[0].Length).Trim(new char[] { '\t', ' ' });
                    }
                    if (!m_params.ContainsKey(key))
                    {
                        m_params.Add(key, content);
                    }
                    else
                    {
                        m_params[key] = content;
                    }
                }
                line = file.ReadLine();
            }
            file.Close();
            return true;
        }

        public bool saveToFile()
        {
            if (m_confFileSavePath != string.Empty)
            {
                return saveToFile(m_confFileSavePath);
            }
            else
            {
                return false;
            }
        }

        public bool saveToFile(string path)
        {
            m_confFileSavePath = Path.GetFullPath(path);
            initData(false);
            StreamWriter file;
            try
            {
                file = new StreamWriter(m_confFileSavePath, false);
            }
            catch
            {
                return false;
            }
            foreach (string str in m_content)
            {
                string[] tab_str = str.Split(m_comment, StringSplitOptions.None);
                string line = tab_str[0];
                if (line != string.Empty)
                {
                    string[] tab_line = line.Split(m_separator, StringSplitOptions.None);
                    string key = tab_line[0];
                    string content = m_params[key];
                    file.Write(key + m_separator[0] + content);
                    if (tab_str.Length > 1)
                    {
                        file.WriteLine("\t" + m_comment[0] + str.Substring(line.Length + 1));
                    }
                    else
                    {
                        file.WriteLine();
                    }
                }
                else
                {
                    file.WriteLine(str);
                }
            }
            file.Close();

            return true;
        }

        public void deleteFile()
        {
            if (File.Exists(m_confFileLoadPath))
            {
                File.Delete(m_confFileLoadPath);
            }
            if (File.Exists(m_confFileSavePath))
            {
                File.Delete(m_confFileSavePath);
            }
        }

        public void comment(string key)
        {
            initData(false);
            if (m_params.ContainsKey(key))
            {
                int id = -1;
                for (int i = 0; i < m_content.Count; i++)
                {
                    if (m_content[i].StartsWith(key))
                    {
                        id = i;
                        break;
                    }
                }
                if (id > -1)
                {
                    m_content[id] = m_comment[0] + m_content[id];
                }
                m_params.Remove(key);
            }

        }

        public void unComment(string key)
        {
            initData(false);
            int id = -1;
            for (int i = 0; i < m_content.Count; i++)
            {
                if (m_content[i].StartsWith(m_comment[0] + key))
                {
                    id = i;
                    break;
                }
            }
            if (id > -1)
            {
                string[] tab_line = m_content[id].Substring(m_comment[0].Length).Split(m_separator, StringSplitOptions.None);
                if (tab_line.Length > 1)
                {
                    m_content.RemoveAt(id);
                    Set(tab_line[0], tab_line[1]);
                }
            }

        }



        public string Get(string key)
        {
            initData(false);
            if (m_params.ContainsKey(key))
            {
                return m_params[key];
            }
            else
            {
                return string.Empty;
            }
        }

		public string GetString(string key, string defVal = "")
		{
			string val = Get(key);
			if (!string.IsNullOrEmpty(val) )
			{
				return val;
			}
			else
			{
				return defVal;
			}
		}

        public bool GetBool(string key,bool defVal =false)
        {
            string val = Get(key);
            bool res;
            if (!string.IsNullOrEmpty(val) && bool.TryParse(val,out res))
            {
                return res;
            }
            else
            {
                return defVal;
            }
        }

        public int GetInt(string key, int defVal = 0)
        {
            string val = Get(key);
            int res;
            if (!string.IsNullOrEmpty(val) && int.TryParse(val,NumberStyles.Any,Util.CultEn, out res))
            {
                return res;
            }
            else
            {
                return defVal;
            }
        }

        public double GetDouble(string key, double defVal = 0.0)
        {
            string val = Get(key);
            double res;
            if (!string.IsNullOrEmpty(val) && double.TryParse(val, NumberStyles.Any, Util.CultEn, out res))
            {
                return res;
            }
            else
            {
                return defVal;
            }
        }

        public List<string> GetListString(string key, List<string> defVal = null)
        {
            string val = Get(key);
            if (!string.IsNullOrEmpty(val))
            {
                return new List<string>(val.Split(m_separator, StringSplitOptions.None));
            }
            else
            {
                return defVal;
            }
        }



        public void Set(string key, string content, bool add = true)
        {
            initData(false);
            if (m_params.ContainsKey(key))
            {
                m_params[key] = content;
            }
            else
            {
                if (add)
                {
                    m_params.Add(key, content);
                    m_content.Add(key + m_separator[0] + content);
                }
            }
        }

        public void Set(string key,bool content,bool add =true)
        {
            Set(key, content.ToString(), add);
        }

        public void Set(string key, int content, bool add = true)
        {
            Set(key, content.ToString(Util.CultEn), add);
        }

        public void Set(string key, double content, bool add = true)
        {
            Set(key, content.ToString(Util.CultEn), add);
        }

        public void Set(string key, List<string> content, bool add = true)
        {
            Set(key, content.Aggregate((a,b)=>$"{a}{m_separator[0]}{b}" ), add);
        }

    }
}
