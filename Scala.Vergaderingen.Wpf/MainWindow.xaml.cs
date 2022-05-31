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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Scala.Vergaderingen.Core.Entities;
using Scala.Vergaderingen.Core.Services;

namespace Scala.Vergaderingen.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        VergaderService vergaderService = new VergaderService();
        bool isNew;
        private void BouwUren()
        {
            List<string> uren = new List<string>();
            for(int u = 0; u <= 23; u++)
            {
                for(int m = 0; m <= 45; m+=15)
                {
                    uren.Add(u.ToString("00") + ":" + m.ToString("00"));
                }
            }
            cmbVan.ItemsSource = uren;
            cmbTot.ItemsSource = uren;
        }
        private void BeeldStandaard()
        {
            grpVergaderingen.IsEnabled = true;
            grpDeelnemers.IsEnabled = true;
            grpDetails.IsEnabled = false;
            btnBewaren.Visibility = Visibility.Hidden;
            btnAnnuleren.Visibility = Visibility.Hidden;
        }
        private void BeeldBewerken()
        {
            grpVergaderingen.IsEnabled = false;
            grpDeelnemers.IsEnabled = false;
            grpDetails.IsEnabled = true;
            btnBewaren.Visibility = Visibility.Visible;
            btnAnnuleren.Visibility = Visibility.Visible;
        }
        private void ClearControls()
        {
            txtLocatie.Text = "";
            dtpDatum.SelectedDate = null;
            cmbVan.SelectedIndex = -1;
            cmbTot.SelectedIndex = -1;
            lstDeelnemers.ItemsSource = null;
            lstDeelnemers.Items.Refresh();
        }
        private void PopulateVergaderingen()
        {
            if (dtpFilter.SelectedDate == null)
                lstVergaderingen.ItemsSource = vergaderService.GetVergaderingen();
            else
                lstVergaderingen.ItemsSource = vergaderService.GetVergaderingen(dtpFilter.SelectedDate);
            lstVergaderingen.Items.Refresh();
        }
        private void PopulatePersonen()
        {
            cmbPersonen.ItemsSource = vergaderService.GetPersonen();
            cmbPersonen.Items.Refresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BouwUren();
            BeeldStandaard();
            PopulateVergaderingen();
            PopulatePersonen();
        }
        private void LstVergaderingen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearControls();
            if(lstVergaderingen.SelectedItem != null)
            {
                Vergadering vergadering = (Vergadering)lstVergaderingen.SelectedItem;
                dtpDatum.SelectedDate = vergadering.Datum;
                cmbVan.SelectedItem = vergadering.Van;
                cmbTot.SelectedItem = vergadering.Tot;
                txtLocatie.Text = vergadering.Locatie;
                lstDeelnemers.ItemsSource = vergaderService.GetPersonenInVergadering(vergadering);
                lstDeelnemers.Items.Refresh();
            }
        }
        private void DtpFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateVergaderingen();
        }
        private void BtnFilterWissen_Click(object sender, RoutedEventArgs e)
        {
            dtpFilter.SelectedDate = null;
            PopulateVergaderingen();
        }

        private void BtnNiew_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            ClearControls();
            BeeldBewerken();
            dtpDatum.Focus();
        }

        private void BtnWijzig_Click(object sender, RoutedEventArgs e)
        {
            if(lstVergaderingen.SelectedItem != null)
            {
                isNew = false;
                BeeldBewerken();
                dtpDatum.Focus();
            }
        }

        private void BtnVerwijder_Click(object sender, RoutedEventArgs e)
        {
            if (lstVergaderingen.SelectedItem != null)
            {
                if (MessageBox.Show("Ben je zeker dat deze vergadering mag verwijderd worden ?", "Vergadering verwijderen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Vergadering vergadering = (Vergadering)lstVergaderingen.SelectedItem;
                    if (!vergaderService.VergaderingVerwijderen(vergadering))
                    {
                        MessageBox.Show("Er is iets fout gelopen ... de vergadering werd niet verwijderd ...", "Fout", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    ClearControls();
                    PopulateVergaderingen();
                }
            }
        }

        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            BeeldStandaard();
            LstVergaderingen_SelectionChanged(null, null);
        }

        private void BtnBewaren_Click(object sender, RoutedEventArgs e)
        {
            if(dtpDatum.SelectedDate == null)
            {
                MessageBox.Show("Je dient een datum op te geven");
                dtpDatum.Focus();
                return;
            }
            if(cmbVan.SelectedItem == null)
            {
                MessageBox.Show("Je dient een startuur op te geven");
                cmbVan.Focus();
                return;
            }
            if (cmbTot.SelectedItem == null)
            {
                MessageBox.Show("Je dient een einduur op te geven");
                cmbTot.Focus();
                return;
            }
            if(cmbTot.SelectedIndex <= cmbVan.SelectedIndex)
            {
                MessageBox.Show("Het einduur dient na het startuur te komen");
                cmbVan.Focus();
                return;
            }
            DateTime datum =(DateTime) dtpDatum.SelectedDate;
            string van = cmbVan.SelectedItem.ToString();
            string tot = cmbTot.SelectedItem.ToString();
            string locatie = txtLocatie.Text.Trim();

            Vergadering vergadering;
            if(isNew)
            {
                vergadering = new Vergadering(datum, van, tot, locatie);
                vergaderService.VergaderingToevoegen(vergadering);
            }
            else
            {
                vergadering = (Vergadering)lstVergaderingen.SelectedItem;
                vergadering.Datum = datum;
                vergadering.Van = van;
                vergadering.Tot = tot;
                vergadering.Locatie = locatie;
                vergaderService.VergaderingWijzigen(vergadering);
            }
            dtpFilter.SelectedDate = null;
            PopulateVergaderingen();
            lstVergaderingen.SelectedValue = vergadering.Id;
            LstVergaderingen_SelectionChanged(null, null);
            BeeldStandaard();
        }

        private void BtnPersoonToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (lstVergaderingen.SelectedItem == null) return;
            if (cmbPersonen.SelectedItem == null) return;
            Vergadering vergadering = (Vergadering)lstVergaderingen.SelectedItem;
            Persoon persoon = (Persoon)cmbPersonen.SelectedItem;
            if (vergaderService.DeelnemerToevoegen(vergadering, persoon))
            {
                lstDeelnemers.ItemsSource = vergaderService.GetPersonenInVergadering(vergadering);
                lstDeelnemers.Items.Refresh();
            }
        }

        private void BtnPersoonVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (lstVergaderingen.SelectedItem == null) return;
            if (lstDeelnemers.SelectedItem == null) return;
            Vergadering vergadering = (Vergadering)lstVergaderingen.SelectedItem;
            Persoon persoon = (Persoon)lstDeelnemers.SelectedItem;
            if (vergaderService.DeelnemerVerwijderen(vergadering, persoon))
            {
                lstDeelnemers.ItemsSource = vergaderService.GetPersonenInVergadering(vergadering);
                lstDeelnemers.Items.Refresh();
            }
        }
    }
}
