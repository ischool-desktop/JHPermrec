using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHPermrec.UpdateRecord.GovernmentalDocument.NameList
{
    /// <summary>
    /// �s�ͦW�U��Ʈ榡��@
    /// </summary>
    public class EnrollEntry : AbstractEntryFormat
    {      
        #region IEntityFormat ����
        
        public override void Initialize(XmlElement element)
        {
            base.Initialize(element);
            
            // �B�z�J�Ǹ��N��
            //ColumnInfo column = new ColumnInfo(element.GetAttribute("�J�Ǹ��N��"), 100);
            ColumnInfo column = new ColumnInfo(element.GetAttribute("�J�Ǹ��N��"), 0);
            _attributes.Add("�J�Ǹ��N��", column);           
        }

        public override string Group
        {
            get { return Department; }
        }

        #endregion
    }
}
