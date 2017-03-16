﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;

namespace NovatecEnergyWeb.Models
{
    public class ExportadorDadosTeste
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private string sFileName = @"demo.xlsx";
        string sWebRootFolder;

        public string SFileName
        {
            get { return sFileName; }
        }

        public ExportadorDadosTeste(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            sWebRootFolder = _hostingEnvironment.WebRootPath + "\\excel";
        }

        public byte[] ExportarParaExcel()
        {
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                //First add the headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Names";
                worksheet.Cells[1, 3].Value = "Gender";
                worksheet.Cells[1, 4].Value = "Salary (in $)";

                //Add values
                worksheet.Cells["A2"].Value = 1000;
                worksheet.Cells["B2"].Value = "Jon";
                worksheet.Cells["C2"].Value = "M";
                worksheet.Cells["D2"].Value = 5000;

                worksheet.Cells["A3"].Value = 1001;
                worksheet.Cells["B3"].Value = "Graham";
                worksheet.Cells["C3"].Value = "M";
                worksheet.Cells["D3"].Value = 10000;

                worksheet.Cells["A4"].Value = 1002;
                worksheet.Cells["B4"].Value = "Jenny";
                worksheet.Cells["C4"].Value = "F";
                worksheet.Cells["D4"].Value = 5000;

                package.Save(); //Save the workbook.
            }

            //Parte do Download
            byte[] fileBytes = File.ReadAllBytes(Path.Combine(sWebRootFolder, sFileName));
            
            return fileBytes;
        }
    }
}