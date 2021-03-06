﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHSchool.Permrec.StudentExtendControls.Reports
{
    public partial class StudentAtSchoolCertificateForm : FISCA.Presentation.Controls.BaseForm 
    {
        bool isDefaultTemplate = true;
        StudentAtSchoolCertificateManager sascm;
        public StudentAtSchoolCertificateForm()
        {
            InitializeComponent();
            sascm = new StudentAtSchoolCertificateManager();

            if (sascm.GetisDefaultTemplate())
                cbxDefault.Checked = true;
            else
                cbxUserDefine.Checked = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkDefault_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sascm.SaveDefaulTemplate();
        }

        private void lnkUserDefine_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sascm.SaveUserDefineTemplate();
        }

        private void lnkUpload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sascm.SetUserDefineTemplateToSystem();
        }

        private void cmdPrint_Click(object sender, EventArgs e)
        {
            // 檢查選項是否有設定
            if (cbxDefault.Checked == false && cbxUserDefine.Checked == false && Student.Instance.SelectedKeys.Count < 1)
                return;

            if (txtCertDoc.Text.Trim() == "" || txtCertNo.Text.Trim() == "")            
                if (MessageBox.Show("校內文號輸入不完整，請問是否繼續列印 ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;

            sascm.SetCertDoc(txtCertDoc.Text);
            sascm.SetCertNo(txtCertNo.Text);

            sascm.SetSemester(School.DefaultSemester,false );

            if (cbxDefault.Checked)
                isDefaultTemplate = true;
            else
                isDefaultTemplate = false;

            cmdPrint.Enabled = false;
            sascm.PrintData(Student.Instance.SelectedKeys, isDefaultTemplate);            
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("學生.報表", "列印", "列印" + Student.Instance.SelectedKeys.Count + "筆在學證明書資料。");

            cmdPrint.Enabled = true;

        }
    }
}
