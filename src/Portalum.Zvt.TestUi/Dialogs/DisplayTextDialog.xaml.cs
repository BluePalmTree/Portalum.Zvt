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
    /// Interaction logic for DisplayTextDialog.xaml
    /// </summary>
    public partial class DisplayTextDialog : Window
    {
        public List<string> DisplayTexts { get; set; }
        public int DisplayDuration { get; set; }
        public int CountBeeps { get; set; }

        public DisplayTextDialog()
        {
            InitializeComponent();

            DisplayTexts = new List<string>();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            int maxLines = TextBoxDisplayText.LineCount > 8 ? 8 : TextBoxDisplayText.LineCount;

            for (int i = 0; i < maxLines; i++)
            {
                DisplayTexts.Add(TextBoxDisplayText.GetLineText(i));
            }

            if (int.TryParse(TextBoxDisplayDuration.Text, out int displayDur))
            {
                DisplayDuration = displayDur;
            }
            else
            {
                DisplayDuration = 5;
            }

            if (int.TryParse(TextBoxCountBeeps.Text, out int countBee))
            {
                CountBeeps = countBee;
            }
            else
            {
                CountBeeps = 0;
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
