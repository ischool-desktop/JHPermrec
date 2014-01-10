using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Framework;
using Framework.Security;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHPermrec.UpdateRecord
{
    //[FeatureCode("Content0140", "���ʸ��")]
    [FCode("JHSchool.Student.Detail0081", "���ʸ��")]
    internal partial class UpdateRecordItem : FISCA.Presentation.DetailContent
    {
        private Dictionary<string, string> _headers;
        private bool _isInitialized = false;
        public static string FeatureCode = "";
        private FeatureAce _permission;
        private string _StudentID=string.Empty;
        private List<JHSchool.Data.JHUpdateRecordRecord> _StudUpateRecList;
        private bool isBGBusy = false;
        private BackgroundWorker BGWorker;

        public UpdateRecordItem()
        {
            InitializeComponent();
            Group = "���ʸ��";
            
            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += new DoWorkEventHandler(BGWorker_DoWork);
            BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWorker_RunWorkerCompleted);
            
            JHSchool.Data.JHUpdateRecord.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHUpdateRecord_AfterUpdate);
            JHSchool.Data.JHUpdateRecord.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHUpdateRecord_AfterInsert);
            JHSchool.Data.JHUpdateRecord.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHUpdateRecord_AfterDelete);
           
            if (!string.IsNullOrEmpty(_StudentID))
                BGWorker.RunWorkerAsync();

            #region �v���P�_�{���X�C
            //���o�� Class �wĳ�� FeatureCode�C
            FeatureCode = FeatureCodeAttribute.GetCode(this.GetType());
            _permission = User.Acl[FeatureCode];

            btnAdd.Visible = _permission.Editable;
            btnRemove.Visible = _permission.Editable;
            bthUpdate.Visible = _permission.Editable;

            btnView.Visible = !_permission.Editable;
            #endregion

            Disposed += new EventHandler(UpdateRecordItem_Disposed);
        }

        void UpdateRecordItem_Disposed(object sender, EventArgs e)
        {
            JHSchool.Data.JHUpdateRecord.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(JHUpdateRecord_AfterUpdate);
            JHSchool.Data.JHUpdateRecord.AfterInsert -= new EventHandler<K12.Data.DataChangedEventArgs>(JHUpdateRecord_AfterInsert);
            JHSchool.Data.JHUpdateRecord.AfterDelete -= new EventHandler<K12.Data.DataChangedEventArgs>(JHUpdateRecord_AfterDelete);

        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            ReloadData();
        }

        void JHUpdateRecord_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            ReloadData();
        }

        

        void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isBGBusy)
            {
                isBGBusy = false;
                BGWorker.RunWorkerAsync();
                return;
            }
            else
            {

                BindData();
                this.Loading = false;
            }

        }

        
        void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudUpateRecList = JHSchool.Data.JHUpdateRecord.SelectByStudentID(PrimaryKey);
        }

        void JHUpdateRecord_AfterInsert(object sender, K12.Data.DataChangedEventArgs e)
        {
            ReloadData();
        }

        void JHUpdateRecord_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            ReloadData();
        }

        

        private void Initialize()
        {
            if (_isInitialized) return;
            _headers = new Dictionary<string, string>();
            _headers.Add("UpdateDate", "���ʤ��");
            _headers.Add("UpdateDescription", "���ʭ�]");
            _headers.Add("ADDate", "���ʮ֭���");
            _headers.Add("ADNumber", "���ʮ֭�帹");
            _isInitialized = true;
            
        }


        private void BindData()
        {
            Initialize();
            
            foreach (string key in _headers.Keys)
            {
                // Set Columns Width
                ColumnHeader ch = new ColumnHeader();
                ch.Text = _headers[key];
                ch.Tag = key;
                if (key == "UpdateDate")
                    ch.Width = 100;
                if (key == "ADDate")
                    ch.Width = 100;
                if(key=="UpdateDescription")
                    ch.Width =180;
                if (key == "ADNumber")
                    ch.Width = 120;


                lstRecord.Columns.Add(ch);
            }
            // �Ƨ�
            _StudUpateRecList.Sort(new Comparison<JHSchool.Data.JHUpdateRecordRecord>(StudUpateRecListSorter1));


            foreach (JHSchool.Data.JHUpdateRecordRecord node in _StudUpateRecList )
            {                
                ListViewItem item = new ListViewItem();
                item.Tag = node;
                item.Text = node.UpdateDate;
                string updateDesc = "";

                if (node.UpdateCode == "1")
                    updateDesc = "�s��";

                if (node.UpdateCode == "2")
                    updateDesc = "���~";

                if (node.UpdateCode == "3")
                    updateDesc = "��J";

                if (node.UpdateCode == "4")
                    updateDesc = "��X";

                if (node.UpdateCode == "5")
                    updateDesc = "���";

                if (node.UpdateCode == "6")
                    updateDesc = "�_��";

                if (node.UpdateCode == "7")
                    updateDesc = "����";

                if (node.UpdateCode == "8")
                    updateDesc = "��Ū";

                if (node.UpdateCode == "9")
                    updateDesc = "�󥿾��y";

                if (node.UpdateCode == "10")
                    updateDesc = "�����׷~�~��";

                if (node.UpdateCode == "11")
                    updateDesc = "���`";

                if (node.UpdateDescription.Length > 0)
                    updateDesc += ":" + node.UpdateDescription;
                // Add Data
                item.SubItems.Add((updateDesc));
                item.SubItems.Add(node.ADDate);
                item.SubItems.Add(node.ADNumber);
 

                lstRecord.Items.Add(item);
            }
        }

        // �Ƨǥ�
        private int StudUpateRecListSorter1(JHSchool.Data.JHUpdateRecordRecord x, JHSchool.Data.JHUpdateRecordRecord y)
        {
            DateTime dtx, dty;

            DateTime.TryParse(x.UpdateDate, out dtx);
            DateTime.TryParse(y.UpdateDate, out dty);
            return dtx.CompareTo(dty);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DAL.StudUpdateRecordEntity sure = DAL.DALTransfer.AddStudUpdateRecordEntity(PrimaryKey,JHPermrec.UpdateRecord.DAL.DALTransfer.UpdateType.Empty,DateTime.Now.ToShortDateString ());
            if (sure == null)
                return;
            UpdateRecordItemForm form = new UpdateRecordItemForm(UpdateRecordItemForm.actMode.�s�W , sure, PrimaryKey);
            form.ShowDialog();
        }

        // �������
        private void ReloadData()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ReloadData));
            }
            else
            {
                if (BGWorker.IsBusy)
                    isBGBusy = true;
                else
                {
                    lstRecord.Clear();
                    this.Loading = true;
                    this.CancelButtonVisible = false;
                    this.SaveButtonVisible = false;

                    BGWorker.RunWorkerAsync();
                }
            }
        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            if (lstRecord.SelectedItems.Count < 1)
                MsgBox.Show("�z��������ܤ@�����");
            if (lstRecord.SelectedItems.Count == 1)
                EditStudentUpdateRecord();
        }

        // �קﲧ�ʬ���
        private void EditStudentUpdateRecord()
        {
            JHSchool.Data.JHUpdateRecordRecord objUpdate = lstRecord.SelectedItems[0].Tag as JHSchool.Data.JHUpdateRecordRecord;
            DAL.StudUpdateRecordEntity sure = DAL.DALTransfer.SetStudUpdateRecordEntityTrans(objUpdate);
            UpdateRecordItemForm form = new UpdateRecordItemForm(UpdateRecordItemForm.actMode.�ק�, sure, PrimaryKey);
            form.ShowDialog();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            bthUpdate_Click(sender, e);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstRecord.SelectedItems.Count < 1)
                MsgBox.Show("�z��������ܤ@�����");
            if (lstRecord.SelectedItems.Count == 1)
            {
                JHSchool.Data.JHUpdateRecordRecord record = lstRecord.SelectedItems[0].Tag as JHSchool.Data.JHUpdateRecordRecord;
                if (MsgBox.Show("�z�T�w�N�������ʸ�ƥä[�R��?", "�T�{", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        // �R�����ʰO��
                        JHSchool.Data.JHUpdateRecord.Delete(record);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("���ʸ�ƧR�����ѡG" + ex.Message);
                    }
                }
            }
        }


        public UpdateRecordItem Clone()
        {
            return new UpdateRecordItem();
        }



        private void lstRecord_DoubleClick(object sender, EventArgs e)
        {
            if (lstRecord.SelectedItems.Count < 1)
                return;

            if (lstRecord.SelectedItems.Count == 1)
                EditStudentUpdateRecord();

        }
    }
}
