using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using FISCA.DSAUtil;
using Framework;
using JHSchool.Data;
using FCode = Framework.Security.FeatureCodeAttribute;
using FISCA.Presentation;

namespace JHSchool.Permrec.StudentExtendControls
{
    [FCode("JHSchool.Student.Detail0070", "�a�}���")]
    internal partial class AddressPalmerwormItem : FISCA.Presentation.DetailContent
    {
        JHAddressRecord _StudAddressRec;
        private ChangeListener _DataListener_Permanent;
        private ChangeListener _DataListener_Mailing;
        private ChangeListener _DataListener_Other;
        private AddressType _address_type;
        private PermRecLogProcess prlp;

        private enum AddressType
        {
            Permanent,
            Mailing,
            Other
        }

        private EnhancedErrorProvider _errors;
        private EnhancedErrorProvider _warnings;

        private BackgroundWorker _getCountyBackgroundWorker;
        private bool isBGBusy = false;
        private BackgroundWorker BGWorker;

        //Town -> ZipCode
        private Dictionary<string, string> _zip_code_mapping = new Dictionary<string, string>();

        private bool _isInitialized = false;
        public AddressPalmerwormItem()
        {
            InitializeComponent();
            Group = "�a�}���";

            _errors = new EnhancedErrorProvider();
            _warnings = new EnhancedErrorProvider();
            prlp = new PermRecLogProcess();

            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += new DoWorkEventHandler(BGWorker_DoWork);
            BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWorker_RunWorkerCompleted);


            _DataListener_Permanent = new ChangeListener();
            _DataListener_Mailing = new ChangeListener();
            _DataListener_Other = new ChangeListener();

            _DataListener_Permanent.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Permanent_StatusChanged);
            _DataListener_Mailing.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Mailing_StatusChanged);
            _DataListener_Other.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Other_StatusChanged);

            // �[�J���y Listener Data
            _DataListener_Permanent.Add(new TextBoxSource(txtZipcode));
            _DataListener_Permanent.Add(new ComboBoxSource(cboCounty, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Permanent.Add(new ComboBoxSource(cboTown, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Permanent.Add(new TextBoxSource(txtDistrict));
            _DataListener_Permanent.Add(new TextBoxSource(txtArea));
            _DataListener_Permanent.Add(new TextBoxSource(txtDetail));
            _DataListener_Permanent.Add(new TextBoxSource(txtLongtitude));
            _DataListener_Permanent.Add(new TextBoxSource(txtLatitude));


            // �[�J�p�� Listener Data
            _DataListener_Mailing.Add(new TextBoxSource(txtZipcode));
            _DataListener_Mailing.Add(new ComboBoxSource(cboCounty, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Mailing.Add(new ComboBoxSource(cboTown, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Mailing.Add(new TextBoxSource(txtDistrict));
            _DataListener_Mailing.Add(new TextBoxSource(txtArea));
            _DataListener_Mailing.Add(new TextBoxSource(txtDetail));
            _DataListener_Mailing.Add(new TextBoxSource(txtLongtitude));
            _DataListener_Mailing.Add(new TextBoxSource(txtLatitude));

            // �[�J�䥦 Listener Data
            _DataListener_Other.Add(new TextBoxSource(txtZipcode));
            _DataListener_Other.Add(new ComboBoxSource(cboCounty, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Other.Add(new ComboBoxSource(cboTown, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Other.Add(new TextBoxSource(txtDistrict));
            _DataListener_Other.Add(new TextBoxSource(txtArea));
            _DataListener_Other.Add(new TextBoxSource(txtDetail));
            _DataListener_Other.Add(new TextBoxSource(txtLongtitude));
            _DataListener_Other.Add(new TextBoxSource(txtLatitude));

            JHAddress.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHAddress_AfterUpdate);

            _address_type = AddressType.Permanent;

            _getCountyBackgroundWorker = new BackgroundWorker();
            _getCountyBackgroundWorker.DoWork += new DoWorkEventHandler(_getCountyBackgroundWorker_DoWork);
            _getCountyBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_getCountyBackgroundWorker_RunWorkerCompleted);
            _getCountyBackgroundWorker.RunWorkerAsync();

            if (User.Acl["Content0050"].Editable)
                return;

            if (User.Acl["Content0050"].Viewable)
                this.Enabled = false;

            Disposed += new EventHandler(AddressPalmerwormItem_Disposed);
        }

        void AddressPalmerwormItem_Disposed(object sender, EventArgs e)
        {
            JHAddress.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(JHAddress_AfterUpdate);
        }

        void JHAddress_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHAddress_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!BGWorker.IsBusy)
                        BGWorker.RunWorkerAsync();
                }
            }
        }

        void _DataListener_Other_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (Framework.User.Acl[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _DataListener_Mailing_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (Framework.User.Acl[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _DataListener_Permanent_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (Framework.User.Acl[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {

            //// ���J�S�� �F�۰ʸ�
            //if (!string.IsNullOrEmpty(txtArea.Text))
            //{
            //    if (txtArea.Text.IndexOf("�F") == -1)
            //        txtArea.Text += "�F";
            //}

            // �ˬd�e���x�s�۹���
            if (_address_type == AddressType.Permanent)
            {
                _StudAddressRec.Permanent.ZipCode = txtZipcode.Text;
                _StudAddressRec.Permanent.County = cboCounty.Text;
                _StudAddressRec.Permanent.Town = cboTown.Text;
                _StudAddressRec.Permanent.District = txtDistrict.Text;
                _StudAddressRec.Permanent.Area = txtArea.Text;
                _StudAddressRec.Permanent.Detail = txtDetail.Text;
                _StudAddressRec.Permanent.Longitude = txtLongtitude.Text;
                _StudAddressRec.Permanent.Latitude = txtLatitude.Text;

            }

            if (_address_type == AddressType.Mailing)
            {
                _StudAddressRec.Mailing.ZipCode = txtZipcode.Text;
                _StudAddressRec.Mailing.County = cboCounty.Text;
                _StudAddressRec.Mailing.Town = cboTown.Text;
                _StudAddressRec.Mailing.District = txtDistrict.Text;
                _StudAddressRec.Mailing.Area = txtArea.Text;
                _StudAddressRec.Mailing.Detail = txtDetail.Text;
                _StudAddressRec.Mailing.Longitude = txtLongtitude.Text;
                _StudAddressRec.Mailing.Latitude = txtLatitude.Text;

            }

            if (_address_type == AddressType.Other)
            {
                _StudAddressRec.Address1.ZipCode = txtZipcode.Text;
                _StudAddressRec.Address1.County = cboCounty.Text;
                _StudAddressRec.Address1.Town = cboTown.Text;
                _StudAddressRec.Address1.District = txtDistrict.Text;
                _StudAddressRec.Address1.Area = txtArea.Text;
                _StudAddressRec.Address1.Detail = txtDetail.Text;
                _StudAddressRec.Address1.Longitude = txtLongtitude.Text;
                _StudAddressRec.Address1.Latitude = txtLatitude.Text;
            }

            prlp.SetAfterSaveText("���y�l���ϸ�", _StudAddressRec.Permanent.ZipCode);
            prlp.SetAfterSaveText("���y����", _StudAddressRec.Permanent.County);
            prlp.SetAfterSaveText("���y�m����", _StudAddressRec.Permanent.Town);
            prlp.SetAfterSaveText("���y����", _StudAddressRec.Permanent.District);
            prlp.SetAfterSaveText("���y�F", _StudAddressRec.Permanent.Area);
            prlp.SetAfterSaveText("���y�䥦�a�}", _StudAddressRec.Permanent.Detail);
            prlp.SetAfterSaveText("���y�g��", _StudAddressRec.Permanent.Longitude);
            prlp.SetAfterSaveText("���y�n��", _StudAddressRec.Permanent.Latitude);
            prlp.SetAfterSaveText("�p���l���ϸ�", _StudAddressRec.Mailing.ZipCode);
            prlp.SetAfterSaveText("�p������", _StudAddressRec.Mailing.County);
            prlp.SetAfterSaveText("�p���m����", _StudAddressRec.Mailing.Town);
            prlp.SetAfterSaveText("�p������", _StudAddressRec.Mailing.District);
            prlp.SetAfterSaveText("�p���F", _StudAddressRec.Mailing.Area);
            prlp.SetAfterSaveText("�p���䥦�a�}", _StudAddressRec.Mailing.Detail);
            prlp.SetAfterSaveText("�p���g��", _StudAddressRec.Mailing.Longitude);
            prlp.SetAfterSaveText("�p���n��", _StudAddressRec.Mailing.Latitude);
            prlp.SetAfterSaveText("�䥦�l���ϸ�", _StudAddressRec.Address1.ZipCode);
            prlp.SetAfterSaveText("�䥦����", _StudAddressRec.Address1.County);
            prlp.SetAfterSaveText("�䥦�m����", _StudAddressRec.Address1.Town);
            prlp.SetAfterSaveText("�䥦����", _StudAddressRec.Address1.District);
            prlp.SetAfterSaveText("�䥦�F", _StudAddressRec.Address1.Area);
            prlp.SetAfterSaveText("�䥦�䥦�a�}", _StudAddressRec.Address1.Detail);
            prlp.SetAfterSaveText("�䥦�g��", _StudAddressRec.Address1.Longitude);
            prlp.SetAfterSaveText("�䥦�n��", _StudAddressRec.Address1.Latitude);

            _errors.Clear();
            JHAddress.Update(_StudAddressRec);
            prlp.SetActionBy("���y", "�ǥͦa�}��T");
            prlp.SetAction("�ק�ǥͦa�}��T");
            JHStudentRecord studRec = JHStudent.SelectByID(PrimaryKey);
            prlp.SetDescTitle("�ǥͩm�W:" + studRec.Name + ",�Ǹ�:" + studRec.StudentNumber + ",");
            prlp.SaveLog("", "", "student", PrimaryKey);
            BindDataToForm();
        }

        // ��Ū���ǥ͸�T������
        void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isBGBusy)
            {
                isBGBusy = false;
                BGWorker.RunWorkerAsync();
                return;
            }
            Initialize();
            BindDataToForm();

        }

        // �z�LDALŪ���ǥͦa�}��T
        void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudAddressRec = JHAddress.SelectByStudentID(PrimaryKey);
        }

        // ���o�a�}��������W��
        private string GetAddressTypeTitle()
        {
            if (_address_type == AddressType.Permanent)
                return "���y�a�}";
            else if (_address_type == AddressType.Mailing)
                return "�p���a�}";
            else if (_address_type == AddressType.Other)
                return "�䥦�a�}";
            else
                return string.Empty;
        }

        // ��J�a�}��T��e��
        public void BindDataToForm()
        {
            DataListenerPause();

            prlp.SetBeforeSaveText("���y�l���ϸ�", _StudAddressRec.Permanent.ZipCode);
            prlp.SetBeforeSaveText("���y����", _StudAddressRec.Permanent.County);
            prlp.SetBeforeSaveText("���y�m����", _StudAddressRec.Permanent.Town);
            prlp.SetBeforeSaveText("���y����", _StudAddressRec.Permanent.District);
            prlp.SetBeforeSaveText("���y�F", _StudAddressRec.Permanent.Area);
            prlp.SetBeforeSaveText("���y�䥦�a�}", _StudAddressRec.Permanent.Detail);
            prlp.SetBeforeSaveText("���y�g��", _StudAddressRec.Permanent.Longitude);
            prlp.SetBeforeSaveText("���y�n��", _StudAddressRec.Permanent.Latitude);
            prlp.SetBeforeSaveText("�p���l���ϸ�", _StudAddressRec.Mailing.ZipCode);
            prlp.SetBeforeSaveText("�p������", _StudAddressRec.Mailing.County);
            prlp.SetBeforeSaveText("�p���m����", _StudAddressRec.Mailing.Town);
            prlp.SetBeforeSaveText("�p������", _StudAddressRec.Mailing.District);
            prlp.SetBeforeSaveText("�p���F", _StudAddressRec.Mailing.Area);
            prlp.SetBeforeSaveText("�p���䥦�a�}", _StudAddressRec.Mailing.Detail);
            prlp.SetBeforeSaveText("�p���g��", _StudAddressRec.Mailing.Longitude);
            prlp.SetBeforeSaveText("�p���n��", _StudAddressRec.Mailing.Latitude);
            prlp.SetBeforeSaveText("�䥦�l���ϸ�", _StudAddressRec.Address1.ZipCode);
            prlp.SetBeforeSaveText("�䥦����", _StudAddressRec.Address1.County);
            prlp.SetBeforeSaveText("�䥦�m����", _StudAddressRec.Address1.Town);
            prlp.SetBeforeSaveText("�䥦����", _StudAddressRec.Address1.District);
            prlp.SetBeforeSaveText("�䥦�F", _StudAddressRec.Address1.Area);
            prlp.SetBeforeSaveText("�䥦�䥦�a�}", _StudAddressRec.Address1.Detail);
            prlp.SetBeforeSaveText("�䥦�g��", _StudAddressRec.Address1.Longitude);
            prlp.SetBeforeSaveText("�䥦�n��", _StudAddressRec.Address1.Latitude);

            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;

            // �P�_���P Bind ��۹���
            if (_address_type == AddressType.Permanent)
            {
                txtZipcode.Text = _StudAddressRec.Permanent.ZipCode;
                cboCounty.Text = _StudAddressRec.Permanent.County;
                cboTown.Text = _StudAddressRec.Permanent.Town;
                txtDistrict.Text = _StudAddressRec.Permanent.District;
                txtArea.Text = _StudAddressRec.Permanent.Area;
                txtDetail.Text = _StudAddressRec.Permanent.Detail;
                txtLongtitude.Text = _StudAddressRec.Permanent.Longitude;
                txtLatitude.Text = _StudAddressRec.Permanent.Latitude;
                _DataListener_Permanent.Reset();
                _DataListener_Permanent.ResumeListen();
            }

            if (_address_type == AddressType.Mailing)
            {
                txtZipcode.Text = _StudAddressRec.Mailing.ZipCode;
                cboCounty.Text = _StudAddressRec.Mailing.County;
                cboTown.Text = _StudAddressRec.Mailing.Town;
                txtDistrict.Text = _StudAddressRec.Mailing.District;
                txtArea.Text = _StudAddressRec.Mailing.Area;
                txtDetail.Text = _StudAddressRec.Mailing.Detail;
                txtLongtitude.Text = _StudAddressRec.Mailing.Longitude;
                txtLatitude.Text = _StudAddressRec.Mailing.Latitude;
                _DataListener_Mailing.Reset();
                _DataListener_Mailing.ResumeListen();
            }

            if (_address_type == AddressType.Other)
            {
                txtZipcode.Text = _StudAddressRec.Address1.ZipCode;
                cboCounty.Text = _StudAddressRec.Address1.County;
                cboTown.Text = _StudAddressRec.Address1.Town;
                txtDistrict.Text = _StudAddressRec.Address1.District;
                txtArea.Text = _StudAddressRec.Address1.Area;
                txtDetail.Text = _StudAddressRec.Address1.Detail;
                txtLongtitude.Text = _StudAddressRec.Address1.Longitude;
                txtLatitude.Text = _StudAddressRec.Address1.Latitude;
                _DataListener_Other.Reset();
                _DataListener_Other.ResumeListen();
            }


        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (BGWorker.IsBusy)
                isBGBusy = true;
            else
                BGWorker.RunWorkerAsync();
        }

        public DetailContent GetContent()
        {
            return new AddressPalmerwormItem();
        }


        void _getCountyBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<string> countyList = e.Result as List<string>;
            foreach (string county in countyList)
            {
                cboCounty.AddItem(county);
            }
        }

        void _getCountyBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Framework.Feature.Config.GetCountyList();
        }

        private void Initialize()
        {
            _errors.Clear();
            _warnings.Clear();

            if (_isInitialized) return;

            _isInitialized = true;
        }

        private XmlElement _current_response;


        private void cboCounty_TextChanged(object sender, EventArgs e)
        {
            cboTown.SelectedItem = null;
            cboTown.Items.Clear();
            if (cboCounty.GetText() != "")
            {
                XmlElement[] townList = Framework.Feature.Config.GetTownList(cboCounty.Text);
                _zip_code_mapping = new Dictionary<string, string>();
                foreach (XmlElement each in townList)
                {
                    string name = each.GetAttribute("Name");

                    if (!_zip_code_mapping.ContainsKey(name))
                        _zip_code_mapping.Add(name, each.GetAttribute("Code"));

                    cboTown.AddItem(name);
                }
            }

            ShowFullAddress();
        }

        private void cboTown_TextChanged(object sender, EventArgs e)
        {
            if (_date_updating) return;
            CheckTownChange();
        }

        private void CheckTownChange()
        {
            string value = cboTown.GetText();
            if (!string.IsNullOrEmpty(value))
            {
                if (_zip_code_mapping.ContainsKey(value))
                {
                    if (txtZipcode.Text.Length > 3)
                    {
                        txtZipcode.Text = _zip_code_mapping[value] + txtZipcode.Text.Substring(3, txtZipcode.Text.Length - 3);
                    }
                    else
                    {
                        txtZipcode.Text = _zip_code_mapping[value];
                    }
                }
            }
            ShowFullAddress();
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void ShowFullAddress()
        {
            string fullAddress = "";
            if (txtZipcode.Text != "")
                fullAddress += "[" + txtZipcode.Text + "]";
            fullAddress += cboCounty.GetText();
            fullAddress += cboTown.GetText();
            fullAddress += txtDistrict.Text;
            fullAddress += txtArea.Text;
            fullAddress += txtDetail.Text;
            this.lblFullAddress.Text = fullAddress;
        }

        private void txtLongtitude_TextChanged(object sender, EventArgs e)
        {
            decimal d;
            if (!string.IsNullOrEmpty(txtLongtitude.Text) && !decimal.TryParse(txtLongtitude.Text, out d))
            {
                _errors.SetError(txtLongtitude, "�g�ץ������Ʀr�κA�C");
                return;
            }
            else
                _errors.SetError(txtLongtitude, string.Empty);
        }

        private void txtLatitude_TextChanged(object sender, EventArgs e)
        {
            decimal d;
            if (!string.IsNullOrEmpty(txtLatitude.Text) && !decimal.TryParse(txtLatitude.Text, out d))
            {
                _errors.SetError(txtLatitude, "�n�ץ������Ʀr�κA�C");
                return;
            }
            else
                _errors.SetError(txtLatitude, string.Empty);
        }

        private void btnPAddress_Click(object sender, EventArgs e)
        {
            if (_errors.HasError)
            {
                MsgBox.Show("��ƿ��~�A�Эץ����");
                return;
            }

            _address_type = AddressType.Permanent;
            DataListenerPause();
            DisplayAddress(GetCurrentAddress());
            _DataListener_Permanent.Reset();
            _DataListener_Permanent.ResumeListen();
        }

        private void DataListenerPause()
        {
            _DataListener_Other.SuspendListen();
            _DataListener_Mailing.SuspendListen();
            _DataListener_Permanent.SuspendListen();
        }

        private void btnFAddress_Click(object sender, EventArgs e)
        {
            if (_errors.HasError)
            {
                MsgBox.Show("��ƿ��~�A�Эץ����");
                return;
            }

            _address_type = AddressType.Mailing;
            DataListenerPause();
            DisplayAddress(GetCurrentAddress());
            _DataListener_Mailing.Reset();
            _DataListener_Mailing.ResumeListen();
        }

        private void btnOAddress_Click(object sender, EventArgs e)
        {
            if (_errors.HasError)
            {
                MsgBox.Show("��ƿ��~�A�Эץ����");
                return;
            }

            _address_type = AddressType.Other;
            DataListenerPause();
            DisplayAddress(GetCurrentAddress());
            _DataListener_Other.Reset();
            _DataListener_Other.ResumeListen();
        }

        private void txtZipcode_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void txtZipcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (_date_updating) return;

            if (e.KeyCode == Keys.Enter)
                CheckZipCode();
        }

        private void txtZipcode_Validated(object sender, EventArgs e)
        {
            if (_date_updating) return;

            if (!_errors.ContainError(txtZipcode))
                CheckZipCode();
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            MapForm.ShowMap(txtLatitude.Text, txtLongtitude.Text, txtDetail.Text);
        }

        private void CheckZipCode()
        {
            string zipcode = "";
            if (txtZipcode.Text.Length > 3)
                zipcode = txtZipcode.Text.Remove(3);
            else
                zipcode = txtZipcode.Text;

            KeyValuePair<string, string> ctPair = Framework.Feature.Config.FindTownByZipCode(zipcode);
            if (ctPair.Key == null)
                _warnings.SetError(txtZipcode, "�d�L���l���ϸ����������m���ơC");
            else
            {
                _warnings.SetError(txtZipcode, string.Empty);

                string county = ctPair.Key;
                string town = ctPair.Value;

                cboCounty.SetComboBoxText(county);
                cboTown.SetComboBoxText(town);
            }
        }

        private bool IsValid()
        {
            return !_errors.HasError;
        }

        private K12.Data.AddressItem GetCurrentAddress()
        {
            if (_address_type == AddressType.Permanent)
                return _StudAddressRec.Permanent;

            else if (_address_type == AddressType.Mailing)
                return _StudAddressRec.Mailing;

            else if (_address_type == AddressType.Other)
                return _StudAddressRec.Address1;
            else
                throw new ArgumentException("�S������ Address Type�C");
        }

        private bool _date_updating = false;
        private void DisplayAddress(K12.Data.AddressItem addr)
        {
            _date_updating = true;
            btnAddressType.Text = GetAddressTypeTitle();
            if (btnAddressType.Text == "���y�a�}")
            {
                lnklblAddress1.Text = "�ƻs�p���a�}";
                lnklblAddress2.Text = "�ƻs�䥦�a�}";
            }

            if (btnAddressType.Text == "�p���a�}")
            {
                lnklblAddress1.Text = "�ƻs���y�a�}";
                lnklblAddress2.Text = "�ƻs�䥦�a�}";
            }

            if (btnAddressType.Text == "�䥦�a�}")
            {
                lnklblAddress1.Text = "�ƻs�p���a�}";
                lnklblAddress2.Text = "�ƻs���y�a�}";
            }
            cboCounty.SetComboBoxText(addr.County);
            cboTown.SetComboBoxText(addr.Town);
            txtDistrict.Text = addr.District;
            txtArea.Text = addr.Area;
            txtDetail.Text = addr.Detail;
            txtLongtitude.Text = addr.Longitude;
            txtLatitude.Text = addr.Latitude;
            txtZipcode.Text = addr.ZipCode;

            _date_updating = false;
        }



        private void btnQueryPoint_Click(object sender, EventArgs e)
        {
            try
            {
                DSXmlHelper h = new DSXmlHelper("Request");
                string address = cboCounty.GetText() + cboTown.GetText() + txtDistrict.Text + txtArea.Text + txtDetail.Text;
                h.AddText(".", address);

                DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Common.QueryCoordinates", new DSRequest(h));
                h = rsp.GetContent();
                if (h.GetElement("Error") != null)
                    MsgBox.Show("�L�k�d�ߦ��a�}�y�Ь�����T");
                else
                {
                    string latitude = h.GetText("Latitude");
                    string longitude = h.GetText("Longitude");

                    if (!string.IsNullOrEmpty(txtLatitude.Text) || !string.IsNullOrEmpty(txtLongtitude.Text))
                    {
                        string msg = "�w�d�ߥX���a�}�y�Ь��G\n\n(" + longitude + "," + latitude + ")\n\n�n���N�ثe�y�г]�w�ܡH";
                        if (MsgBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            txtLatitude.Text = latitude;
                            txtLongtitude.Text = longitude;
                        }
                    }
                    else
                    {
                        txtLatitude.Text = latitude;
                        txtLongtitude.Text = longitude;
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("�d�߮y�и�T���~�C");
                Diagnostic.FeedbackError(ex, "�a�}��ƶ��ت��d�߮y�и�T�C");
            }
        }

        private void txtDistrict_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void lnklblAddress1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (btnAddressType.Text == "���y�a�}")
            {
                //�ƻs�p���a�}
                txtZipcode.Text = _StudAddressRec.Mailing.ZipCode;
                cboCounty.Text = _StudAddressRec.Mailing.County;
                cboTown.Text = _StudAddressRec.Mailing.Town;
                txtDistrict.Text = _StudAddressRec.Mailing.District;
                txtArea.Text = _StudAddressRec.Mailing.Area;
                txtDetail.Text = _StudAddressRec.Mailing.Detail;
                txtLongtitude.Text = _StudAddressRec.Mailing.Longitude;
                txtLatitude.Text = _StudAddressRec.Mailing.Latitude;
            }

            if (btnAddressType.Text == "�p���a�}")
            {

                // �ƻs���y�a�}
                txtZipcode.Text = _StudAddressRec.Permanent.ZipCode;
                cboCounty.Text = _StudAddressRec.Permanent.County;
                cboTown.Text = _StudAddressRec.Permanent.Town;
                txtDistrict.Text = _StudAddressRec.Permanent.District;
                txtArea.Text = _StudAddressRec.Permanent.Area;
                txtDetail.Text = _StudAddressRec.Permanent.Detail;
                txtLongtitude.Text = _StudAddressRec.Permanent.Longitude;
                txtLatitude.Text = _StudAddressRec.Permanent.Latitude;

            }

            if (btnAddressType.Text == "�䥦�a�}")
            {
                // �ƻs�p���a�}
                txtZipcode.Text = _StudAddressRec.Mailing.ZipCode;
                cboCounty.Text = _StudAddressRec.Mailing.County;
                cboTown.Text = _StudAddressRec.Mailing.Town;
                txtDistrict.Text = _StudAddressRec.Mailing.District;
                txtArea.Text = _StudAddressRec.Mailing.Area;
                txtDetail.Text = _StudAddressRec.Mailing.Detail;
                txtLongtitude.Text = _StudAddressRec.Mailing.Longitude;
                txtLatitude.Text = _StudAddressRec.Mailing.Latitude;
            }

        }

        private void lnklblAddress2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (btnAddressType.Text == "���y�a�}")
            {
                // �ƻs�䥦�a�}
                txtZipcode.Text = _StudAddressRec.Address1.ZipCode;
                cboCounty.Text = _StudAddressRec.Address1.County;
                cboTown.Text = _StudAddressRec.Address1.Town;
                txtDistrict.Text = _StudAddressRec.Address1.District;
                txtArea.Text = _StudAddressRec.Address1.Area;
                txtDetail.Text = _StudAddressRec.Address1.Detail;
                txtLongtitude.Text = _StudAddressRec.Address1.Longitude;
                txtLatitude.Text = _StudAddressRec.Address1.Latitude;

            }

            if (btnAddressType.Text == "�p���a�}")
            {
                // �ƻs�䥦�a�}
                txtZipcode.Text = _StudAddressRec.Address1.ZipCode;
                cboCounty.Text = _StudAddressRec.Address1.County;
                cboTown.Text = _StudAddressRec.Address1.Town;
                txtDistrict.Text = _StudAddressRec.Address1.District;
                txtArea.Text = _StudAddressRec.Address1.Area;
                txtDetail.Text = _StudAddressRec.Address1.Detail;
                txtLongtitude.Text = _StudAddressRec.Address1.Longitude;
                txtLatitude.Text = _StudAddressRec.Address1.Latitude;

            }

            if (btnAddressType.Text == "�䥦�a�}")
            {
                // �ƻs���y�a�}
                txtZipcode.Text = _StudAddressRec.Permanent.ZipCode;
                cboCounty.Text = _StudAddressRec.Permanent.County;
                cboTown.Text = _StudAddressRec.Permanent.Town;
                txtDistrict.Text = _StudAddressRec.Permanent.District;
                txtArea.Text = _StudAddressRec.Permanent.Area;
                txtDetail.Text = _StudAddressRec.Permanent.Detail;
                txtLongtitude.Text = _StudAddressRec.Permanent.Longitude;
                txtLatitude.Text = _StudAddressRec.Permanent.Latitude;
            }
        }
    }
}
