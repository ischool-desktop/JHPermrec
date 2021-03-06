﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Framework;


namespace JHPermrec.UpdateRecord.Transfer
{
    public partial class AddTransStudBase : FISCA.Presentation.Controls.BaseForm,IRewriteAPI_JH.ITransStudBase
    {
        public enum AddTransStudStatus { Added, Modify }

        //private StudentRecord student;

        private JHSchool.Data.JHStudentRecord  _student;
        private AddTransStudStatus _status;
        private JHSchool.Data.JHPhoneRecord _StudentPhone;


        public void setStudent_Status(JHSchool.Data.JHStudentRecord student, AddTransStudStatus status)
        {
            _student = student;
            _status = status;
            Errors = new EnhancedErrorProvider();
            Errors.Icon = Properties.Resources.warning;

            cboNewNationality.Items.AddRange(DALTransfer1.GetNationalities().ToArray());
            cboClass.Items.Add(new KeyValuePair<string, string>("", "<空白>"));

            foreach (KeyValuePair<string, string> classItem in DALTransfer1.GetClassNameList())
            {
                cboClass.Items.Add(classItem);
            }


            cboClass.DisplayMember = "Value";
            cboClass.ValueMember = "Key";

            cboClass.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboClass.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboNewGender.Items.AddRange(new string[] { "男", "女" });

            if (_student != null)
            {

                //把資料填入各項控制項當中
                txtName.Text = txtNewName.Text = _student.Name;
                txtSSN.Text = txtNewSSN.Text = _student.IDNumber;
                if (_student.Birthday.HasValue)
                    dtBirthDate.Text = dtNewBirthday.Text = _student.Birthday.Value.ToString();
                cboGender.Text = cboNewGender.Text = _student.Gender;
                cboNationality.Text = cboNewNationality.Text = _student.Nationality;
                txtBirthPlace.Text = txtNewBirthPlace.Text = _student.BirthPlace;
                _StudentPhone = JHSchool.Data.JHPhone.SelectByStudentID(_student.ID);

                txtTel.Text = txtNewTel.Text = _StudentPhone.Contact;
                //txtEngName.Text = txtNewEngName.Text = _student.na
                if (_student.Class != null)
                    lblClassName.Text = cboClass.Text = _student.Class.Name;
                if (_student.SeatNo.HasValue)
                    lblSeatNo.Text = cboSeatNo.Text = _student.SeatNo.Value.ToString();
                lblStudentNum.Text = cbotStudentNumber.Text = _student.StudentNumber;
            }

            //依照status不同調整畫面大小
            if (_status == AddTransStudStatus.Added)
            {
                gpOld.Visible = false;
                this.Size = new Size(422, 378);
                //txtNewName.Text = "";
                txtNewSSN.Text = AddTransBackgroundManager.StudentIDNumber;

            }
            else
            {
                gpOld.Visible = true;
                this.Size = new Size(815, 378);
            }
            setClassNo();
            reLoadStudNumItems();

            AddTransBackgroundManager.AddTransStudBaseObj = this;
        }


        private EnhancedErrorProvider Errors { get; set; }

        public AddTransStudBase()
        {
            InitializeComponent();           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNewName.Text.Trim() == "")
                {
                    MsgBox.Show("姓名必填");
                    return;
                }

                if (string.IsNullOrEmpty(cbotStudentNumber.Text))
                {
                    Errors.SetError(cbotStudentNumber, "學號空白!");
                }

                string sid = string.Empty;
                if (_status == AddTransStudStatus.Added)
                {
                    JHSchool.Data.JHStudentRecord NewStudRec = new JHSchool.Data.JHStudentRecord();
                    NewStudRec.Name = txtNewName.Text;
                    NewStudRec.Gender = cboNewGender.Text;
                    NewStudRec.IDNumber = txtNewSSN.Text;
                    sid = JHSchool.Data.JHStudent.Insert(NewStudRec);
                    _StudentPhone = JHSchool.Data.JHPhone.SelectByStudentID(sid);
                    _StudentPhone.Contact = txtNewTel.Text;
                }

                if (StudCheckTool.CheckStudIDNumberSame(txtNewSSN.Text, sid))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("身分證號重複請檢查");
                    return;
                }

                Dictionary<string, int> chkSum = new Dictionary<string, int>();
                foreach (JHSchool.Data.JHStudentRecord studRec in JHSchool.Data.JHStudent.SelectAll())
                {
                    if(studRec.Status == K12.Data.StudentRecord.StudentStatus.一般 )
                    if (!string.IsNullOrEmpty(studRec.StudentNumber))
                    {
                        if (chkSum.ContainsKey(studRec.StudentNumber))
                            chkSum[studRec.StudentNumber]++;
                        else
                            chkSum.Add(studRec.StudentNumber, 1);
                    }
                }

                if (chkSum.ContainsKey(cbotStudentNumber.Text))
                {
                    if (chkSum[cbotStudentNumber.Text] > 1)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("學號重複請檢查");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(sid))
                    _student = JHSchool.Data.JHStudent.SelectByID(sid);

                //_student.Name = txtNewName.Text;

                _student.IDNumber = txtNewSSN.Text;
                DateTime dt;
                if (DateTime.TryParse(dtNewBirthday.Text, out dt))
                    _student.Birthday = dt;

                //_student.Gender = cboNewGender.Text;
                _student.Nationality = cboNewNationality.Text;
                _student.BirthPlace = txtNewBirthPlace.Text;
                //_StudentPhone.Contact = txtNewTel.Text;
                //_student.EnglishName = txtNewEngName.Text;

                foreach (JHSchool.Data.JHClassRecord cr in JHSchool.Data.JHClass.SelectAll())
                {
                    if (cboClass.Text == cr.Name)
                    {
                        _student.RefClassID = cr.ID;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(cboSeatNo.Text))
                    _student.SeatNo = null;
                else
                {
                    int no;
                    int.TryParse(cboSeatNo.Text, out no);
                    _student.SeatNo = no;
                }
                _student.StudentNumber = cbotStudentNumber.Text;


                if (_status == AddTransStudStatus.Added)
                {

                }

                JHSchool.Data.JHStudent.Update(_student);
                JHSchool.Data.JHPhone.Update(_StudentPhone);


                //log
                JHSchool.PermRecLogProcess prlp = new JHSchool.PermRecLogProcess();
                if (_status == AddTransStudStatus.Added)
                    prlp.SaveLog("學生.轉入異動", "新增班級資料", "修改轉入與班級資料.");
                else
                    prlp.SaveLog("學生.轉入異動", "修改班級資料", "修改轉入與班級資料.");

                AddTransBackgroundManager.SetStudent(_student);

                AddTransManagerForm atmf = new AddTransManagerForm();
                this.Visible = false;
                atmf.StartPosition = FormStartPosition.CenterParent;
                atmf.ShowDialog(FISCA.Presentation.MotherForm.Form);
                this.Close();
                JHSchool.Student.Instance.SyncAllBackground();
                JHSchool.Data.JHStudent.RemoveAll();
                JHSchool.Data.JHStudent.SelectAll();
            }
            catch(Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                return;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboClassName_TextChanged(object sender, EventArgs e)
        {

            cboSeatNo.Text = "";
            setClassNo();
            reLoadStudNumItems();
        }

        private void reLoadStudNumItems()
        {
            cbotStudentNumber.Items.Clear();
            cbotStudentNumber.Items.Add(lblStudentNum.Text);
            cbotStudentNumber.Items.Add(DAL.DALTransfer2.GetGradeYearLastStudentNumber(cboClass.Text));
            if (lblStudentNum.Text != "")
                cbotStudentNumber.Items.Add("");
        }

        // 產生座號
        private void setClassNo()
        {
            cboSeatNo.Items.Clear();
            cboSeatNo.Items.AddRange(DAL.DALTransfer2.GetClassNullNoList(cboClass.Text).ToArray());
        }



        private void cboSeatNo_TextChanged(object sender, EventArgs e)
        {
            //Errors.Clear();
        }

        private void cbotStudentNumber_TextChanged(object sender, EventArgs e)
        {
            
            Errors.Clear();
        }

        private void btnBefore_Click(object sender, EventArgs e)
        {
            AddTransBackgroundManager.AddTransStudObj.Visible = true;
            this.Visible = false;
        }

        private void txtNewName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbotStudentNumber.Text))
                Errors.SetError(cbotStudentNumber, "學號空白!");
            else
                Errors.SetError(cbotStudentNumber, "");
        }

        private void cboClass_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        public void Display()
        {            
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowDialog(FISCA.Presentation.MotherForm.Form);            
        }

        public void SetData(object x, object y)
        {
            setStudent_Status((JHSchool.Data.JHStudentRecord)x, (AddTransStudStatus)y);
        }
    }
}
