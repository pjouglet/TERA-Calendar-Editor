using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraSplashScreen;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using TERACalendar.Views;
using TERAResources;

namespace TERACalendar
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            SplashScreenManager.ShowForm(typeof(Splash));
            Configuration.LoadConfiguration();
            TeraItem.LoadXMLItems(Configuration.LoadIcons);
            TeraItem.ItemList.ForEach(i => AddItemView.TeraItemList.Add(i));
            SplashScreenManager.CloseForm();

            InitializeComponent();
            schedulerControl1.PopupMenuShowing += SchedulerControl1_PopupMenuShowing;
            schedulerControl1.EditAppointmentFormShowing += SchedulerControl1_EditAppointmentFormShowing;
            schedulerControl1.InitAppointmentImages += SchedulerControl1_InitAppointmentImages;
            schedulerControl1.Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
            GetItemsFromDB();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SQLManager.Close();
        }

        private void GetItemsFromDB()
        {
            SqlDataReader reader = SQLManager.GetitemsFromDB();
            while (reader.Read())
            {
                TeraItem item = TeraItem.ItemList.Find(i => i.Id == reader.GetInt32(1));
                int amount = reader.GetInt32(2);
                DateTime time = reader.GetDateTime(0);

                Appointment app = schedulerControl1.DataStorage.CreateAppointment(AppointmentType.Normal);
                app.Start = time;
                app.End = new DateTime(time.Year, time.Month, time.AddDays(1).Day, 00, 00, 00);
                app.Subject = $"{item.Name} (x{amount})";
                app.AllDay = true;
                app.CustomFields["item"] = item;
                app.CustomFields["amount"] = amount;
                schedulerDataStorage1.Appointments.Add(app);
            }
            reader.Close();
        }

        private void SchedulerControl1_InitAppointmentImages(object sender, AppointmentImagesEventArgs e)
        {
            AppointmentImageInfo info = new AppointmentImageInfo();
            info.Image = ((TeraItem)e.Appointment.CustomFields["item"]).ImageIcon;
            e.ImageInfoList.Add(info);
        }

        private void SchedulerControl1_EditAppointmentFormShowing(object sender, AppointmentFormEventArgs e)
        {
            var item = schedulerDataStorage1.Appointments.Items.Find(a => a.Start == e.Appointment.Start);
            e.Handled = true;
            var view = new AddItemView();
            view.OnAdditem += View_OnAdditem;
            view.InitData((item is null) ? null : (TeraItem)item.CustomFields["item"], (item is null) ? 1 : (int)item.CustomFields["amount"], e.Appointment.Start);
            view.Show(this);
        }

        private void View_OnAdditem(object sender, AddItemView.AddItemEventArgs e)
        {
            var item = schedulerDataStorage1.Appointments.Items.Find(a => a.Start == e.Start);
            if(!(item is null))
            {
                int index = schedulerDataStorage1.Appointments.Items.IndexOf(item);
                schedulerDataStorage1.Appointments.Items[index].Subject = $"{e.Item.Name} (x{e.Amount})";
                schedulerDataStorage1.Appointments.Items[index].CustomFields["item"] = e.Item;
                schedulerDataStorage1.Appointments.Items[index].CustomFields["amount"] = e.Amount;
                SQLManager.UpdateItem(e.Item.Id, e.Amount, e.Start);
                return;
            }

            Appointment app = schedulerControl1.DataStorage.CreateAppointment(AppointmentType.Normal);
            app.Start = e.Start;
            app.End = new DateTime(e.Start.Year, e.Start.Month, e.Start.AddDays(1).Day, 00, 00, 00);
            app.Subject = $"{e.Item.Name} (x{e.Amount})";
            app.AllDay = true;
            app.CustomFields["item"] = e.Item;
            app.CustomFields["amount"] = e.Amount;
            schedulerDataStorage1.Appointments.Add(app);
            SQLManager.AddItem(e.Item.Id, e.Amount, e.Start);
        }

        private void SchedulerControl1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Clear();
        }
    }
}
