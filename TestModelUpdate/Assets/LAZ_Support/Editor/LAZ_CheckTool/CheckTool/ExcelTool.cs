using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Excel;
using System.Data;

public class ExcelTool
{
    public static DataSet GetData(string path)
    {
        path = path.Replace(@"\", "/");
        path = path.Replace("//", "/");//以防重复添加斜杠
        if (!File.Exists(path))
        {
            Debug.LogError("不存在" + path + ", 请检查。");
            return null;
        }
        FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
        DataSet result = excelDataReader.AsDataSet();
        return result;
    }
}
