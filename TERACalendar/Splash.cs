using DevExpress.XtraSplashScreen;
using System;

namespace TERACalendar
{
    public partial class Splash : SplashScreen
    {
        public Splash()
        {
            InitializeComponent();
            InitData();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }

        private void InitData()
        {
            
        }
    }
}