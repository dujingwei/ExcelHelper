﻿using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Web;
using System.Text;

namespace ExcelHelper.Operating
{
    public static class WorkbookExtensions
    {
        public static void Download(this IWorkbook workbook, string fileName)
        {
            string contentType = string.Empty;

            if (workbook.GetType() == typeof (HSSFWorkbook))
            {
                //Office2003
                contentType = "application/vnd.ms-excel";
            }
            else if ((workbook.GetType() == typeof (XSSFWorkbook)))
            {
                //Office2007
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }

            var request = HttpContext.Current.Request;


            if (request.UserAgent.ToLower().Contains("msie") || request.UserAgent.ToLower().Contains("trident"))//前面是IE10，后面是IE11
            {
                fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
            }
            else if (request.UserAgent.ToLower().Contains("firefox") )
            {
                fileName = "\"" + fileName + "\"";
            }

            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = contentType;
            response.Charset = "uft-8";
            response.ContentEncoding = System.Text.Encoding.UTF8;
            response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.WriteTo(response.OutputStream);
            }

            response.Flush();
            response.End();

        }

        public static void Save(this IWorkbook workbook, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }
    }
}
