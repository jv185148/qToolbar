using q;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace qMain
{
    public class cCompatDialog
    {
        qCommon.Interfaces.iCompatDialog child;
        q.Common.CompatFlags compatFlag;

        public cCompatDialog(qCommon.Interfaces.iCompatDialog child)
        {
            this.child = child;

            child.AcceptClicked += Child_AcceptClicked;
            child.CancelClicked += Child_CancelClicked;

            loadList();

            SetCurrent();

            child.comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            child.comboBox.SelectionChanged -= ComboBox_SelectionChanged;
            child.UseCompat = true;
        }

        private void loadList()
        {
            string[] arr = q.Common.Win_Compat_List();

            foreach (string str in arr)
            {
                child.comboBox.Items.Add(str);
            }
            child.comboBox.Text = child.comboBox.Items[0].ToString();
        }

        private void SetCurrent()
        {
            string sFlag = Common.Get_Long_Compat_String(child.CurrentFlag);

            child.comboBox.Text = sFlag;

            if (sFlag.ToLower() != "none")
                child.UseCompat = true;

        }

        private void Child_CancelClicked(object sender)
        {
            child.DialogResult = false;
            ((Window)child).Close();
        }

        private void Child_AcceptClicked(object sender)
        {
            compatFlag = q.Common.Parse(child.comboBox.SelectedItem.ToString());

            if (!child.UseCompat)
                compatFlag = Common.CompatFlags.none;

            q.Common.SetCompatFlag(child.targetFile, compatFlag);
            child.flag = compatFlag;

            child.DialogResult = true;
            ((Window)child).Close();
        }
    }
}
