using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{

    public class XMLNode
    {
        public List<XMLNode> ChildNodes;
        public string Name;
        public List<XMLAttribute> Attributes;
        public string InnerText;
        public XMLNode FirstChild
        {
            get
            {
                if (ChildNodes.Count > 0)
                {
                    return ChildNodes[0];
                }
                return null;
            }
        }
        public void AddChild(XMLNode param)
        {
            if (ChildNodes == null)
            {
                ChildNodes = new List<XMLNode>();
            }
            ChildNodes.Add(param);
        }

        public void AddAttribute(XMLAttribute param)
        {
            if (Attributes == null)
            {
                Attributes = new List<XMLAttribute>();
            }
            Attributes.Add(param);
        }
    }
    public class XMLAttribute
    {
        public string Value;
        public string Name;
    }

    public class XMLAttributeList
    {
        private List<XMLAttribute> m_nodeList;
        public XMLAttributeList()
        {
            m_nodeList = new List<XMLAttribute>();
        }
        public XMLAttribute this[string name]
        {
            get
            {
                XMLAttribute ret = m_nodeList.Find(new Predicate<XMLAttribute>(p => p.Name == name));
                return ret;
            }
        }
        public XMLAttribute this[int i]
        {
            get
            {
                if (i > 0 && i < m_nodeList.Count)
                {
                    return m_nodeList[i];
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class ParseException : Exception
    {
        private string msg = "";
        public ParseException(string msg)
        {
            this.msg = msg;
        }
        public string ToString()
        {
            return msg;
        }
    }

    class MyXMLParse : XMLNode
    {
        private const char lt = '<';
        private const char gt = '>';
        private const char equal = '=';
        private const char slash = '/';
        private const char space = ' ';
        private const char quote = '\"';
        private const char singlequote = '\'';


        public enum EXmlNodeTag
        {
            EBegin,
            ENodeName,
            EAttibName,
            EAttibValue,
            EEnd,
        }

        static double TakeAquareRoot(int x)
        {
            return Math.Sqrt(x);
        }

        //static void Main(string[] args)
        //{
        //    List<int> integers = new List<int>();
        //    integers.Add(1);
        //    integers.Add(2);
        //    integers.Add(3);
        //    integers.Add(4);
        //    Converter<int, double> convert = TakeAquareRoot;
        //    List<double> doubles;
        //    doubles = integers.ConvertAll<double>(convert);
        //    foreach (double d in doubles)
        //    {
        //        Console.WriteLine(d);
        //    }
        //    Console.ReadKey();
        //}

        public void Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new IOException("File Not Found:" + fileName);
            }
            byte[] buf = File.ReadAllBytes(fileName);
            string content = Encoding.UTF8.GetString(buf);
            parseNode2(this, content, 1);
        }

        private void parseNode(XMLNode parent, string content, int start)
        {
            //first non_empty char must be <
            if (start > content.Length) return;
            while (start < content.Length)
            {
                if (content[start] == lt)
                    break;
                parent.InnerText += content[start];
                start++;
            }
            parent.InnerText += content[start];
            XMLNode ret = new XMLNode();            
            //parse name
            StringBuilder nodeName = new StringBuilder(20);
            StringBuilder attribName = new StringBuilder(20);
            StringBuilder attribValue = new StringBuilder(20);
            StringBuilder innerText = new StringBuilder(300);
            bool nameEnd = false, attribEnd = true, innerTextEnd = false, nodeEnd = false, attribNameEnd = true, attribValueEnd = true;
            for (int i = start + 1; i < content.Length ; ++i)
            {
                if (content[i] == gt)
                {
                    //parse innerText or child element  
                    if (nodeEnd) return;
                    if (!nameEnd)
                    {
                        nameEnd = true;                        
                        if (nodeName.ToString() == parent.Name) return;
                        ret.Name = nodeName.ToString();
                    }
                    if (!attribValueEnd)
                    {
                        ret.AddAttribute(new XMLAttribute() { Name = attribName.ToString(), Value = attribValue.ToString() });
                        attribName.Clear();
                        attribValue.Clear();
                        attribValueEnd = true;
                    }                    
                    if (attribValueEnd)
                    {                        
                        parseNode(ret, content, i + 1);
                        if(!string.IsNullOrEmpty(ret.Name))
                            parent.AddChild(ret);
                        else
                        {
                            return;
                        }
                        //return;
                    }
                }

                if (content[i] == lt)
                {
                    if (nameEnd)
                    {
                        innerTextEnd = true;
                    }                    
                }

                if (content[i] == space)
                {
                    //parse attibute
                    if (!nameEnd)
                    {
                        nameEnd = true;
                        ret.Name = nodeName.ToString();
                    }
                    if (!attribValueEnd)
                    {
                        ret.AddAttribute(new XMLAttribute() { Name = attribName.ToString(), Value = attribValue.ToString() });
                        attribName.Clear();
                        attribValue.Clear();
                    }
                    if (attribNameEnd)
                    {
                        attribNameEnd = false;
                        attribValueEnd = true;
                        attribName.Clear();
                    }
                }
                if(content[i] == slash && content[i - 1] == lt)
                {
                    //continue, name or attibute end, parse next sibling
                    nodeEnd = true;
                }
                if (content[i] == equal)
                {
                    attribValue.Clear();
                    attribNameEnd = true;
                    attribValueEnd = false;
                }
                if (!innerTextEnd)
                {
                    parent.InnerText += content[i];
                }
                if (IsEscapeChar(content[i])) continue;
                if (!nameEnd)
                {
                    nodeName.Append(content[i]);
                }
                if (!attribNameEnd)
                {
                    attribName.Append(content[i]);
                }
                if (!attribValueEnd)
                {
                    attribValue.Append(content[i]);
                }                
            }

        }

        private void parseNode2(XMLNode parent, string content, int start)
        {
            XMLNode ret = new XMLNode();
            int nameStart = content.IndexOf(lt, start);
            int nameEnd = content.IndexOf(gt, nameStart);
            string nameStr = content.Substring(nameStart + 1, nameEnd - nameStart - 1);
            int flag1 = nameStr.IndexOf(space);
            if (flag1 == -1)
            {
                if (nameStr[0] == slash) return;
                ret.Name = nameStr;
            }
            else
            {
                //strip attibute
                string attrString = content.Substring(flag1, nameEnd - flag1);
                string[] attrList = attrString.Split(space);
                int len = attrList.Length;
                StringBuilder attriValue = new StringBuilder(20);
                for (int i = 0; i < len; ++i)
                {
                    string tmp = attrList[i];
                    string[] tmp2 = tmp.Split(equal);
                    for (int j = 0; j < tmp2[1].Length; ++j)
                    {
                        if(tmp2[1][j] != singlequote && tmp2[1][j] != quote)
                            attriValue.Append(tmp2[j]);
                    }
                    ret.AddAttribute(new XMLAttribute() { Name = tmp2[0], Value = attriValue.ToString() });
                }
                int innerTextEnd = content.IndexOf(ret.Name);
                ret.InnerText = content.Substring(nameEnd, innerTextEnd - nameEnd);                
                //parse childnode
                //parseNode2(ret,)
            }
            parent.AddChild(ret);
            parseNode2(ret, content, nameEnd + 1);
        }
        private bool IsEscapeChar(char c)
        {
            if (c == slash || c == quote || c == singlequote || c== equal) return true;
            return false;
        }
    }
}
