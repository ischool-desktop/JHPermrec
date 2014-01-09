﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.Permrec.StudentExtendControls.Reports
{
    public partial class StudGraduateCertficateForm : FISCA.Presentation.Controls.BaseForm
    {
        StudGraduateCertficateManager sgcm;
        bool isDefalutTemplate = true;

        public StudGraduateCertficateForm()
        {
            InitializeComponent();
            sgcm = new StudGraduateCertficateManager();
            if (sgcm.GetisDefaultTemplate())
                cbxDefault.Checked = true;
            else
                cbxUserDefine.Checked = true;
        }

        private void cmdPrint_Click(object sender, EventArgs e)
        {
            if (cbxDefault.Checked == false && cbxUserDefine.Checked == false && Student.Instance.SelectedKeys.Count < 1)
                return;

            //if (txtCertDoc.Text.Trim() == "" || txtCertNo.Text.Trim() == "")
            //    if (MessageBox.Show("校內文號輸入不完整，請問是否繼續列印 ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            //        return;

            if (cbxDefault.Checked)
                isDefalutTemplate = true;
            else
                isDefalutTemplate = false;

            cmdPrint.Enabled = false;
            sgcm.SetCertDoc(txtCertDoc.Text);
            sgcm.SetCertNo(txtCertNo.Text);
            sgcm.SetSemester(School.DefaultSemester, true);

            sgcm.PrintData(Student.Instance.SelectedKeys, isDefalutTemplate);
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("學生.報表", "列印", "列印" + Student.Instance.SelectedKeys.Count + "筆轉學證明書。");
            cmdPrint.Enabled = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkDefault_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sgcm.SaveDefaulTemplate();
        }

        private void lnkUserDefine_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sgcm.SaveUserDefineTemplate();
        }

        private void lnkUpload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sgcm.SetUserDefineTemplateToSystem();
        }
    }
}
