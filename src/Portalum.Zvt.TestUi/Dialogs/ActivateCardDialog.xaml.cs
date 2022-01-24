using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Portalum.Zvt.TestUi.Dialogs
{
    /// <summary>
    /// Interaction logic for ActivateCardDialog.xaml
    /// </summary>
    public partial class ActivateCardDialog : Window
    {
        public decimal Amount { get; set; }
        public int Bonuspoints { get; set; }

        public ActivateCardDialog()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(TextBoxAmount.Text, out decimal amount))
            {
                Amount = amount;
            }
            else
            {
                return;
            }

            if (int.TryParse(TextBoxBonuspoints.Text, out int bonuspoints))
            {
                Bonuspoints = bonuspoints;
            }
            else
            {
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
