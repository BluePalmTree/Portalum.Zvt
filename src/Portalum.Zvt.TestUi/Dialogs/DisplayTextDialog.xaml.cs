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

            this.DialogResult = true;
            this.Close();
        }
    }
}
