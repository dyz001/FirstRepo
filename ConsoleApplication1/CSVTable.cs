using System.Collections.Generic;
public class CSVTable
{
    [System.Serializable]
    public class RowData
    {
        public string[] dataArray;
        public CSVTable table;

        public RowData(CSVTable inTable)
        {
            table = inTable;
            dataArray = new string[table.fields.Count];
        }

        public string this[string field]
        {
            get
            {
                try
                {
                    field = field.ToLower();
                    int index = table.fieldDict[field];
                    return dataArray[index];
                }
                catch (KeyNotFoundException)
                {
                    throw new System.ArgumentException("CSV field is error: field = " + field + ", table = " + table.name);
                }
            }
        }
    }
    public List<RowData> records = new List<RowData>();
    public List<RowData> Records
    {
        get
        {
            if (keyDict == null)
                Initialize();
            return records;
        }
    }

    public List<string> fields = new List<string>();
    public string name;

    private Dictionary<int, RowData> keyDict;
    private Dictionary<string, int> fieldDict;

    //
    // create internal dictionary for speeding up lookup operation.
    //
    public void Initialize()
    {
        keyDict = new Dictionary<int, RowData>();
        fieldDict = new Dictionary<string, int>();
        for (int index = 0; index < records.Count; index++)
        {
            records[index].table = this;
            int key = CastUtil.ParseInt(records[index].dataArray[0]);
            keyDict.Add(key, records[index]);
        }
        for (int index = 0; index < fields.Count; index++)
        {
            fieldDict.Add(fields[index].ToLower(), index);
        }
    }

    public bool IsExistKey(int ID)
    {
        if (keyDict == null)
            Initialize();
        return keyDict.ContainsKey(ID);
    }

    //
    // indexer.eg. table[1020]["SomeField"]
    //
    public RowData this[int ID]
    {
        get
        {
            if (keyDict == null)
                Initialize();
            try
            {
                return keyDict[ID];
            }
            catch (KeyNotFoundException)
            {
                throw new System.ArgumentException("CSV key is error: ID = " + ID + ", table = " + name);
            }
        }
    }
}