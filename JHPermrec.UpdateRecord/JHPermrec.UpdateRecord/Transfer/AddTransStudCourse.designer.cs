﻿namespace JHPermrec.UpdateRecord.Transfer
{
    partial class AddTransStudCourse
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.lstView = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.chkSaveYes = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.chkSaveNo = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.lblmsg = new DevComponents.DotNetBar.LabelX();
            this.lblStudMsg = new DevComponents.DotNetBar.LabelX();
            this.btnSave1 = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cboSelectClass = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(358, 241);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(121, 23);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "繼續輸入評量成績";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstView
            // 
            // 
            // 
            // 
            this.lstView.Border.Class = "ListViewBorder";
            this.lstView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lstView.Location = new System.Drawing.Point(3, 85);
            this.lstView.Name = "lstView";
            this.lstView.Size = new System.Drawing.Size(476, 119);
            this.lstView.TabIndex = 1;
            this.lstView.UseCompatibleStateImageBehavior = false;
            this.lstView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "CourseID";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "課程";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "科目";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "學分";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "修課狀態";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 160;
            // 
            // chkSaveYes
            // 
            this.chkSaveYes.BackColor = System.Drawing.Color.Transparent;
            this.chkSaveYes.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.chkSaveYes.Location = new System.Drawing.Point(3, 241);
            this.chkSaveYes.Name = "chkSaveYes";
            this.chkSaveYes.Size = new System.Drawing.Size(136, 23);
            this.chkSaveYes.TabIndex = 2;
            this.chkSaveYes.Text = "自動加入該班課程";
            // 
            // chkSaveNo
            // 
            this.chkSaveNo.BackColor = System.Drawing.Color.Transparent;
            this.chkSaveNo.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.chkSaveNo.Location = new System.Drawing.Point(127, 241);
            this.chkSaveNo.Name = "chkSaveNo";
            this.chkSaveNo.Size = new System.Drawing.Size(158, 23);
            this.chkSaveNo.TabIndex = 3;
            this.chkSaveNo.Text = "不變更學生修課記錄";
            // 
            // lblmsg
            // 
            this.lblmsg.BackColor = System.Drawing.Color.Transparent;
            this.lblmsg.Location = new System.Drawing.Point(3, 212);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(476, 23);
            this.lblmsg.TabIndex = 4;
            // 
            // lblStudMsg
            // 
            this.lblStudMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblStudMsg.Location = new System.Drawing.Point(3, 12);
            this.lblStudMsg.Name = "lblStudMsg";
            this.lblStudMsg.Size = new System.Drawing.Size(476, 23);
            this.lblStudMsg.TabIndex = 5;
            this.lblStudMsg.Text = "labelX1";
            // 
            // btnSave1
            // 
            this.btnSave1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave1.BackColor = System.Drawing.Color.Transparent;
            this.btnSave1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave1.Location = new System.Drawing.Point(277, 241);
            this.btnSave1.Name = "btnSave1";
            this.btnSave1.Size = new System.Drawing.Size(75, 23);
            this.btnSave1.TabIndex = 11;
            this.btnSave1.Text = "儲存";
            this.btnSave1.Click += new System.EventHandler(this.btnSave1_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(3, 41);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(125, 23);
            this.labelX1.TabIndex = 12;
            this.labelX1.Text = "請選擇加入班級課程";
            // 
            // cboSelectClass
            // 
            this.cboSelectClass.DisplayMember = "Text";
            this.cboSelectClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSelectClass.FormattingEnabled = true;
            this.cboSelectClass.ItemHeight = 17;
            this.cboSelectClass.Location = new System.Drawing.Point(127, 42);
            this.cboSelectClass.Name = "cboSelectClass";
            this.cboSelectClass.Size = new System.Drawing.Size(121, 23);
            this.cboSelectClass.TabIndex = 13;
            this.cboSelectClass.SelectedIndexChanged += new System.EventHandler(this.cboSelectClass_SelectedIndexChanged);
            // 
            // AddTransStudCourse
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(484, 273);
            this.Controls.Add(this.cboSelectClass);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnSave1);
            this.Controls.Add(this.lblStudMsg);
            this.Controls.Add(this.lblmsg);
            this.Controls.Add(this.chkSaveNo);
            this.Controls.Add(this.chkSaveYes);
            this.Controls.Add(this.lstView);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MaximumSize = new System.Drawing.Size(492, 307);
            this.MinimumSize = new System.Drawing.Size(492, 307);
            this.Name = "AddTransStudCourse";
            this.Text = "學生修課";
            this.Load += new System.EventHandler(this.AddTransStudCourse_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.Controls.ListViewEx lstView;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkSaveYes;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkSaveNo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private DevComponents.DotNetBar.LabelX lblmsg;
        private DevComponents.DotNetBar.LabelX lblStudMsg;
        private DevComponents.DotNetBar.ButtonX btnSave1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSelectClass;
    }
}