using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace ConsoleApplication1
{
    class EventToAction
    {
        string eventId;
        string[] actionIds;
    }
    
    class Program
    {
        static string PVEOutlandTemplate = "PVEOutlandTemplate";
        static string PVEOutland = "PVEOutland";
        static string PVEOutlandlevel = "PVEOutlandlevel";
        static string PVEEvent = "PVEEvent";
        static string PVEAction = "PVEAction";

        Dictionary<int, EventToAction> EventToActionDic = new Dictionary<int, EventToAction>();
        static XmlDocument m_xmlDoc;


        static List<CSVTable> tables = new List<CSVTable>();
        public class MutiKill
        {
            public int id;
            public int cnt;
        }
        static void Main(string[] args)
        {
            Random rand = new Random();
            MutiKill k = new MutiKill();
            int bulletId = 0;
            while (true)
            {
                int input = Console.Read();
                switch (input)
                {
                    case 'B':
                        bulletId = rand.Next(65535);
                        Console.WriteLine("bulletId:" + bulletId);
                        break;
                    case 'K':                                                
                        if (k.id == 0)
                        {
                            k.id = bulletId;
                            Console.WriteLine("asign bulletId:" + k.id);
                        }
                        k.cnt++;
                        if (k.cnt > 1 && k.id != bulletId)
                        {
                            Console.WriteLine("bulletId:" + k.id + ", killCnt:" + (k.cnt - 1));
                            k.id = bulletId;
                            k.cnt = 1;
                        }
                        break;
                }
                if (input == 'C')
                {
                    break;
                }
            }
            Console.WriteLine("End");
        }

        public static List<string> GetDiffHashFile(byte[] content)
        {
            string fileContent = System.Text.Encoding.Default.GetString(content);
            byte[] tmp = File.ReadAllBytes("test.ini");
            if (tmp != null)
            {
                return null;
            }

            string oldFileContent = System.Text.Encoding.Default.GetString(tmp);
            List<string> ret = new List<string>();
            string[] remoteList = fileContent.Split('\n');
            string[] localList = oldFileContent.Split('\n');
            Array.Sort(remoteList);
            Array.Sort(localList);//升序

            int len1 = remoteList.Length;
            int len2 = localList.Length;
            int i = 0, k = 0;
            for (; i < len1;)
            {
                string[] tmp_remote = remoteList[i].Split('=');
                string[] tmp_local = localList[k].Split('=');
                string pre_remote = tmp_remote[0];
                string pre_local = tmp_local[0];
                if (pre_remote.CompareTo(pre_local) < 0)
                {
                    ret.Add(pre_remote);
                     ++i;
                }
                else if(pre_remote.CompareTo(pre_local) > 0)
                {
                    ++k;
                }
                else
                {
                    if(tmp_remote[1].CompareTo(tmp_local[1]) != 0)
                    {
                        ret.Add(pre_remote);
                        ++k;
                        ++i;
                    }
                }
            }

            return ret;
        }

        static XmlNode readXml(string path)
        {
            if (m_xmlDoc == null)
            {
                m_xmlDoc = new XmlDocument();
                try
                {
                    m_xmlDoc.Load(path);
                    XmlNode selectNode = m_xmlDoc.FirstChild;
                    return selectNode;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return null;
                }
            }
            return m_xmlDoc.FirstChild;
        }
        static void readCSVFile(string fileName)
        {
            FileInfo finfo = new FileInfo(fileName);
            string path = finfo.ToString();
            if (path.EndsWith(".csv", System.StringComparison.CurrentCultureIgnoreCase))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                if (reader == null)
                {
                    return;
                }

                string line = reader.ReadLine();
                // 第一行有汉字视为注释
                if (Check(line))
                {
                    line = reader.ReadLine();
                }
                if (line == null)
                {
                    reader.Close();
                    return;
                }

                CSVTable table = new CSVTable();
                table.name = System.IO.Path.GetFileNameWithoutExtension(path);
                tables.Add(table);

                string[] fields = line.Split(',');
                foreach (string s in fields)
                {
                    string trim = s.Trim();
                    trim = trim.Trim('"');
                    table.fields.Add(trim);
                }
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == "")
                    {
                        continue;
                    }

                    CSVTable.RowData row = new CSVTable.RowData(table);
                    table.records.Add(row);

                    string[] record = line.Split(',');
                    int index = 0;
                    foreach (string s in record)
                    {
                        string trim = s.Trim();
                        trim = trim.Trim('"');
                        int Key;
                        if (index == 0 && !int.TryParse(trim, out Key))
                        {
                            reader.Close();
                            throw new System.ArgumentException("Invalid key in CSVTable:" + path + ",content:" + trim);
                        }
                        try
                        {
                            row.dataArray[index++] = trim;
                        }
                        catch (System.Exception e)
                        {
                            Console.Error.WriteLine("Error:" + e.ToString());
                        }
                    }
                }
                reader.Close();

            }
        }
        private static bool Check(string pendingString)
        {
            Regex reg = new Regex(@"[\u4e00-\u9fa5]");//正则表达式
            if (reg.IsMatch(pendingString))
            {
                return true; //有汉字
            }
            else
            {
                return false;//没有汉字
            }
        }

        private static CSVTable getCSVTableByName(string fn)
        {
            foreach (CSVTable t in tables)
            {
                if (t.name == fn)
                {
                    return t;
                }
            }
            return null;
        }

        private static List<string> getLevelListByMapDiff(int id)
        {
            List<string> levelList = new List<string>();
            CSVTable table = getCSVTableByName(PVEOutland);
            string[] temp = null;
            if(table.IsExistKey(id))
            {
                CSVTable.RowData row = table[id];
                temp = row["levelidlist"].Split(";".ToCharArray());
            }
            if(temp != null)
            {
                foreach(string t in temp)
                {
                    levelList.Add(t);
                }
            }
            return levelList;            
        }

        private static string getEventList(int levelId)
        {
            string result = "";
            CSVTable table = getCSVTableByName(PVEOutlandlevel);
            if (table.IsExistKey(levelId))
            {
                CSVTable.RowData row = table[levelId];
                result = row["eventseq"];
            }
            return result;
        }

        private static string getDisplayEventList(string eventlist)
        {
            StringBuilder result = new StringBuilder();
            CSVTable eventTable = getCSVTableByName(PVEEvent);
            CSVTable actionTable = getCSVTableByName(PVEAction);
            string[] events = eventlist.Split(";".ToCharArray());
            foreach (string singleEvent in events)
            {
                string buf = singleEvent.Trim("()".ToCharArray());
                string[] temp = buf.Split("@".ToCharArray());
                string eId = temp[0];
                string[] actionList = temp[1].Split("|".ToCharArray());



                foreach (CSVTable.RowData row in eventTable.Records)
                {
                    if (row["eventid"] == eId)
                    {
                        result.Append(row["ext"]).Append("@");
                        break;
                    }
                }

                foreach (string action in actionList)
                {
                    foreach (CSVTable.RowData row in actionTable.Records)
                    {
                        if (row["actionid"] == action)
                        {
                            result.Append(row["Ext"]).Append("|");
                            break;
                        }
                    }
                }                
            }
            return result.ToString(0, result.Length - 1);
        }

        private static string getActionList(int id)
        {
            string result = "";
            CSVTable table = getCSVTableByName(PVEOutlandlevel);
            if (table.IsExistKey(id))
            {
                result = table[id]["actionlist"];
            }
            return result;
        }

        private static string getDisplayAcitonList(string actionList)
        {
            StringBuilder sb = new StringBuilder();
            string[] temp = actionList.Split(";".ToCharArray());
            CSVTable actionTable = getCSVTableByName(PVEAction);
            foreach (string t in temp)
            {
                foreach (CSVTable.RowData row in actionTable.Records)
                {
                    if (row["actionid"] == t)
                    {
                        sb.Append(row["ext"]).Append(";");
                        break;
                    }
                }
            }
            return sb.ToString(0, sb.Length - 1);
        }
    }
}
