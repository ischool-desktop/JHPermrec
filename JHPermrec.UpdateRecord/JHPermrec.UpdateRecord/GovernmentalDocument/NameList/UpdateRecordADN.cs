using System;
using System.Windows.Forms;
using FISCA.DSAUtil;
using Framework;
using JHSchool.Permrec.Feature.Legacy;
using System.Xml;

namespace JHPermrec.UpdateRecord.GovernmentalDocument.NameList
{
    public partial class UpdateRecordADN : FISCA.Presentation.Controls.BaseForm
    {
        //private string _id;
        public event EventHandler DataSaved;

        public UpdateRecordADN()
        {
            InitializeComponent();
        }

        private ISummaryProvider _provider;
        internal void Initialize(ISummaryProvider provider)
        {
            _provider = provider;
            txtNumber.Text = provider.ADNumber;
            dtpDate.Text = provider.ADDate;
            CheckSaveable();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNumber.Text))
            {
                MsgBox.Show("�п�J�ַǤ帹�C");
                return;
            }

            DateTime date;
            if (!DateTime.TryParse(dtpDate.DateString, out date))
            {
                MsgBox.Show("����榡�����T�C");
                return;
            }

            // �ק�W�U����������P�帹
            DSXmlHelper helper = new DSXmlHelper("AuthorizeBatchRequest");
            helper.AddElement("AuthorizeBatch");
            helper.AddElement("AuthorizeBatch", "Field");
            helper.AddElement("AuthorizeBatch/Field", "ADNumber", txtNumber.Text);
            helper.AddElement("AuthorizeBatch/Field", "ADDate", dtpDate.DateString);
            helper.AddElement("AuthorizeBatch", "Condition");
            helper.AddElement("AuthorizeBatch/Condition", "ID", _provider.ID);

            try
            {
                EditStudent.ModifyUpdateRecordBatch(new DSRequest(helper));
                
            }
            catch (Exception ex)
            {
                MsgBox.Show("�s��֭�帹���ѡG" + ex);
            }

            // �ק��]�t�����ʬ����帹
            helper = new DSXmlHelper("UpdateRequest");
            helper.AddElement("UpdateRecord");
            helper.AddElement("UpdateRecord", "Field");
            helper.AddElement("UpdateRecord/Field", "ADNumber", txtNumber.Text);
            helper.AddElement("UpdateRecord/Field", "ADDate", dtpDate.DateString);
            helper.AddElement("UpdateRecord", "Condition");

            if (_provider.GetEntities().Length <= 0) //�W�U���S������ǥ͡A�N����s�ǥͪ��ַǤ帹�F�C
                return;

            foreach (IEntryFormat entity in _provider.GetEntities())
                helper.AddElement("UpdateRecord/Condition", "ID", entity.ID);

            try
            {
                EditStudent.ModifyUpdateRecord(new DSRequest(helper));
                if (DataSaved != null)
                    DataSaved(this, null);
            }
            catch (Exception ex)
            {
                MsgBox.Show("�s��֭�帹���ѡG" + ex);
            }


            string batchName = "";
            string schoolYear = "";
            string semester = "";

            if (_provider.ID != "")
            {
                DSResponse dsrsp = QueryStudent.GetUpdateRecordBatch(_provider.ID);

                DSXmlHelper helper_ = dsrsp.GetContent();

                //��W�W�U�� �Ǧ~�B�Ǵ��B�W��
                foreach (XmlNode node in helper_.GetElements("UpdateRecordBatch"))
                {
                    schoolYear = node.SelectSingleNode("SchoolYear").InnerText;
                    semester = node.SelectSingleNode("Semester").InnerText;
                    batchName = node.SelectSingleNode("Name").InnerText;
                }
                // log�A2018/3/1 �o�~�s�W�A�]������ [10-03][01] ��ӾǦ~�׮֭�L���帹�����ʦW�U���������F ����
                // �����ʦW�U ��u���s�W�|���t�ά����A�{�b�վ�R���B�n���帹���|������
                JHSchool.PermRecLogProcess prlp = new JHSchool.PermRecLogProcess();
                string desc = "�n���帹" + schoolYear + "�Ǧ~��,��" + semester + "�Ǵ�," + batchName + "�W�U,���:" + dtpDate.DateString + ",�帹:"+ txtNumber.Text;
                prlp.SaveLog("�а�.�W�U", "�n���帹", desc);
               
            }

       


            this.Close();
            
        }

        private void UpdateRecordADN_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void dtpDate_TextChanged(object sender, EventArgs e)
        {
            CheckSaveable();
        }

        private void CheckSaveable()
        {
            btnOK.Enabled = IsValid(); ;
        }

        private bool IsValid()
        {
            if (!dtpDate.IsValid)
                return false;
            return true;
        }
    }
}