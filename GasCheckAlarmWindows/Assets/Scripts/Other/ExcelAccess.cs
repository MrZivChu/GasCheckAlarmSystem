using UnityEngine;
using Excel;
using System.Data;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelAccess
{
    /// <summary>
    /// 读取 Excel ; 需要添加 Excel.dll; System.Data.dll;
    /// </summary>
    /// <param name="excelPath">excel文件名</param>
    /// <param name="sheetName">sheet名称</param>
    /// <returns>DataRow的集合</returns>
    static DataRowCollection ReadExcel(string excelPath, string sheetName)
    {
        if (!string.IsNullOrEmpty(excelPath) && File.Exists(excelPath))
        {
            FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();
            //int columns = result.Tables[0].Columns.Count;
            //int rows = result.Tables[0].Rows.Count;

            //tables可以按照sheet名获取，也可以按照sheet索引获取
            //return result.Tables[0].Rows;
            return result.Tables[sheetName].Rows;
        }
        return null;
    }

    /// <summary>
    /// 写入 Excel ; 需要添加 OfficeOpenXml.dll;
    /// </summary>
    /// <param name="excelPath">excel文件名</param>
    /// <param name="sheetName">sheet名称</param>
    public static void WriteExcel(string excelPath, string sheetName, List<HistoryDataModel> datalist)
    {
        if (datalist != null && datalist.Count > 0)
        {
            FileInfo newFile = new FileInfo(excelPath);
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(excelPath);
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells[1, 1].Value = "探头名称";
                worksheet.Cells[1, 2].Value = "所属主机";
                worksheet.Cells[1, 3].Value = "所属厂区";
                worksheet.Cells[1, 4].Value = "气体种类";
                worksheet.Cells[1, 5].Value = "当前数值";
                worksheet.Cells[1, 6].Value = "一级报警值";
                worksheet.Cells[1, 7].Value = "二级报警值";
                worksheet.Cells[1, 8].Value = "检测时间";
                for (int i = 0; i < datalist.Count; i++)
                {
                    HistoryDataModel model = datalist[i];
                    worksheet.Cells["A" + (i + 2)].Value = model.ProbeName;
                    worksheet.Cells["B" + (i + 2)].Value = model.MachineName;
                    worksheet.Cells["C" + (i + 2)].Value = model.FactoryName;
                    worksheet.Cells["D" + (i + 2)].Value = model.GasKind;
                    worksheet.Cells["E" + (i + 2)].Value = model.GasValue;
                    worksheet.Cells["F" + (i + 2)].Value = model.FirstAlarmValue;
                    worksheet.Cells["G" + (i + 2)].Value = model.SecondAlarmValue;
                    worksheet.Cells["H" + (i + 2)].Value = model.CheckTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                package.Save();
            }
        }
    }
}

