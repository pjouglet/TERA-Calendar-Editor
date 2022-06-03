using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using TERAResources;

namespace TERACalendar.Views
{
    public partial class AddItemView : XtraForm
    {
        public class AddItemEventArgs: EventArgs
        {
            public TeraItem Item { get; set; }
            public int Amount { get; set; }
            public DateTime Start { get; set; }
        }

        public static BindingList<TeraItem> TeraItemList = new BindingList<TeraItem>();
        public event EventHandler<AddItemEventArgs> OnAdditem;
        private DateTime _startTime;

        public AddItemView()
        {
            InitializeComponent();
            gridLookUpEdit1.Properties.DataSource = TeraItemList;
            simpleButton1.Click += SimpleButton1_Click;
            simpleButton2.Click += SimpleButton2_Click;
        }

        private void SimpleButton1_Click(object sender, EventArgs e)
        {
            if(gridLookUpEdit1.EditValue is null || textEdit1.EditValue is null)
            {
                Close();
                return;
            }

            OnAdditem?.Invoke(this, new AddItemEventArgs()
            {
                Item = TeraItem.ItemList.Find(item => item.Id == (int)gridLookUpEdit1.EditValue),
                Amount = int.Parse(textEdit1.EditValue.ToString()),
                Start = _startTime
            });
            Close();
        }

        public void InitData(TeraItem item, int amount, DateTime start)
        {
            _startTime = start;
            textEdit1.EditValue = amount.ToString();
            if (!(item is null))
            {
                gridLookUpEdit1.EditValue = item.Id;
                Text = "Edit Calendar Reward Item";
                simpleButton1.Text = "Edit";
            }
        }

        private void SimpleButton2_Click(object sender, System.EventArgs e)
        {
            Close();
        }

    }
}