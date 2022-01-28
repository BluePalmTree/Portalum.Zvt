using Portalum.Zvt.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Portalum.Zvt.ControlPanel.Dialogs
{
    /// <summary>
    /// Interaction logic for AuthorizationDialog.xaml
    /// </summary>
    public partial class AuthorizationDialog : Window
    {
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool PrinterReady { get; set; }
        public string Track1 { get; set; }
        public string Track2 { get; set; }
        public string Track3 { get; set; }
        public string CardNo { get; set; }
        public DateTime? ExpiryDate { get; set; } = null;

        public AuthorizationDialog()
        {
            InitializeComponent();

            ComboBoxPaymentType.ItemsSource = Enum.GetValues(typeof(PaymentType));
            ComboBoxPaymentType.SelectedItem = PaymentType.PTDecission;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(this.TextBoxAmount.Text, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount))
            {
                MessageBox.Show("Cannot parse amount");
                return;
            }
            else
            {
                Amount = amount;
            }

            PaymentType = (PaymentType)ComboBoxPaymentType.SelectedItem;
            PrinterReady = CheckBoxPrinterReady.IsChecked.GetValueOrDefault();

            Track1 = TextBoxTrack1.Text.Trim();
            Track2 = TextBoxTrack2.Text.Trim();
            Track3 = TextBoxTrack3.Text.Trim();
            CardNo = TextBoxCardNumber.Text.Trim();            
            ExpiryDate = DatePickerExpiryDate.SelectedDate;

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

