﻿using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Xml;
using System.IO;
using Framework.Legacy;
using JHPermrec.UpdateRecord.DAL;
using K12.Data;

namespace JHPermrec.UpdateRecord.GovernmentalDocument.NameList
{
    public class DeadStudentList : ReportBuilder
    {
        // 死亡名冊
        protected override void Build(System.Xml.XmlElement source, string location)
        {
            if (JHSchool.Permrec.Program.ModuleType == JHSchool.Permrec.Program.ModuleFlag.KaoHsiung)
                ProcessKaoHsiung(source, location);        
        }


        // 高雄用
        private void ProcessKaoHsiung(System.Xml.XmlElement source, string location)
        {
            // 資料轉換
            Dictionary<string, JHPermrec.UpdateRecord.DAL.StudBatchUpdateRecContentEntity> data = StudBatchUpdateRecEntity.ConvertGetContentData(source);

            int peoTotalCount = 0;  // 總人數
            int peoBoyCount = 0;    // 男生人數
            int peoGirlCount = 0;   // 女生人數

            int tmpY, tmpM;
            tmpY = DateTime.Now.Year;
            tmpM = DateTime.Now.Month;
            string tmpRptY, tmpRptM;
            tmpRptY = (tmpY - 1911).ToString();
            if (tmpM < 10)
                tmpRptM = "0" + tmpM.ToString();
            else
                tmpRptM = tmpM.ToString();


            #region 建立 Excel

            //從 Resources 將死亡異動名冊template讀出來
            Workbook template = new Workbook();
            template.Open(new MemoryStream(GDResources.JDeadStudentListTemplate), FileFormatType.Excel2003);

            //產生 excel
            Workbook wb = new Aspose.Cells.Workbook();
            wb.Open(new MemoryStream(GDResources.JDeadStudentListTemplate), FileFormatType.Excel2003);
            
            #endregion
            #region 複製樣式-預設樣式、欄寬

            //設定預設樣式
            wb.DefaultStyle = template.DefaultStyle;

            #endregion

            int rowi = 0, rowj = 1, numcount = 1, j = 0;
            int recCount = 0;
            int totalRec = data.Count;

            rowj = 4;                        
            wb.Worksheets[0].Cells[rowi, 4].PutValue(StudBatchUpdateRecEntity.GetContentSchoolName() + "  " + StudBatchUpdateRecEntity.GetContentSchoolYear() + "學年第 " + StudBatchUpdateRecEntity.GetContentSemester() + "學期");
            wb.Worksheets[0].Cells[rowi, 8].PutValue("列印日期：" + UpdateRecordUtil.ChangeDate1911(DateTime.Now.ToString()));
            wb.Worksheets[0].Cells[rowi + 1, 8].PutValue("列印時間：" + DateTime.Now.ToLongTimeString());


            //將xml資料填入至excel
            foreach (StudBatchUpdateRecContentEntity sburce in data.Values)
            {
                recCount++;

                #region 填入學生資料
                // 班級
                wb.Worksheets[0].Cells[rowj, 0].PutValue(sburce.GetClassName());

                // 座號
                wb.Worksheets[0].Cells[rowj, 1].PutValue(sburce.GetSeatNo());

                // 學號
                wb.Worksheets[0].Cells[rowj, 2].PutValue(sburce.GetStudentNumber());

                // 姓名
                wb.Worksheets[0].Cells[rowj, 3].PutValue(sburce.GetName());

                // 身分證
                wb.Worksheets[0].Cells[rowj, 4].PutValue(sburce.GetIDNumber());

                // 出生年月日
                if (!string.IsNullOrEmpty(sburce.GetBirthday()))
                    wb.Worksheets[0].Cells[rowj, 5].PutValue(UpdateRecordUtil.ChangeDate1911(sburce.GetBirthday()));

                // 性別
                wb.Worksheets[0].Cells[rowj, 6].PutValue(sburce.GetGender());

                // 異動年級
                wb.Worksheets[0].Cells[rowj, 7].PutValue(sburce.GetGradeYear());

                // 異動日期
                if (!string.IsNullOrEmpty(sburce.GetUpdateDate()))
                    wb.Worksheets[0].Cells[rowj, 8].PutValue(UpdateRecordUtil.ChangeDate1911(sburce.GetUpdateDate()));

                // 學籍最後核准文號
                wb.Worksheets[0].Cells[rowj, 9].PutValue(sburce.GetLastADNumber());

                if (sburce.GetGender() == "男")
                    peoBoyCount++;
                if (sburce.GetGender() == "女")
                    peoGirlCount++;

                peoTotalCount++;

                #endregion

                rowj++;

                //回報進度
                ReportProgress((int)(((double)recCount * 100.0) / ((double)totalRec)));
            }

            Style st2 = wb.Styles[wb.Styles.Add()];
            StyleFlag sf2 = new StyleFlag();
            sf2.Borders = true;

            st2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            st2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            st2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            st2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            int tmpMaxRow = 0, tmpMaxCol = 0;
            for (int wbIdx1 = 0; wbIdx1 < wb.Worksheets.Count; wbIdx1++)
            {
                tmpMaxRow = wb.Worksheets[wbIdx1].Cells.MaxDataRow - 3;
                tmpMaxCol = wb.Worksheets[wbIdx1].Cells.MaxDataColumn + 1;
                wb.Worksheets[wbIdx1].Cells.CreateRange(4, 0, tmpMaxRow, tmpMaxCol).ApplyStyle(st2, sf2);
            }


            // 統計人數
            rowj++;
            wb.Worksheets[0].Cells.CreateRange(rowj, 2, 1, 2).Merge();
            wb.Worksheets[0].Cells[rowj, 2].PutValue("男：" + peoBoyCount.ToString());
            wb.Worksheets[0].Cells[rowj, 4].PutValue("女：" + peoGirlCount.ToString());
            wb.Worksheets[0].Cells[rowj, 8].PutValue("總計：" + peoTotalCount.ToString());
            wb.Worksheets[0].Cells.CreateRange(rowj + 1, 0, 1, 10).Merge();
            //            wb.Worksheets[0].Cells[rowj + 1, 0].PutValue("校長                                                          教務主任                                                          註冊組長                                                          核對員");
            wb.Worksheets[0].Cells[rowj + 1, 0].PutValue("核對員                                                          註冊組長                                                          教務主任                                                          校長");


            // 顯示頁
            PageSetup pg = wb.Worksheets[0].PageSetup;
            string tmp = "&12 " + tmpRptY + "年" + tmpRptM + "月 填報" + "共&N頁";
            pg.SetHeader(2, tmp);

            //儲存 Excel
            wb.Save(location, FileFormatType.Excel2003);
        }

        public override string Copyright
        {
            get { return "IntelliSchool"; }
        }

        public override string Description
        {
            get { return "中部辦公室95年11月編印管理手冊規範格式"; }
        }

        public override string ReportName
        {
            get { return "死亡學生名冊"; }
        }

        public override string Version
        {
            get { return "1.0.0.0"; }
        }
    }
}
