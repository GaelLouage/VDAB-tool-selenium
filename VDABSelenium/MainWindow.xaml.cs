using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VDABSelenium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Gebruiker gebruiker = null;
        private IWebDriver driver = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnVerzendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGebruikernaam.Text) && !string.IsNullOrEmpty(txtWachtwoord.Password)
                && !string.IsNullOrEmpty(txtGemeente.Text) && !string.IsNullOrEmpty(txtZoekeenJob.Text))
            {
                gebruiker = new Gebruiker(
                    txtGebruikernaam.Text,
                    txtWachtwoord.Password,
                    txtZoekeenJob.Text,
                    txtGemeente.Text
                    );
                // object driver  (firefox)
                await Task.Run(() =>
                {
                    driver = new FirefoxDriver();
                    driver.Url = "https://www-login.vdab.be/LRR_IKL/Handler?TAM_OP=login&ERROR_CODE=0x00000000&URL=/&HOSTNAME=www.vdab.be&context=default";
                    // username text field
                    try
                    {
                        driver.FindElement(By.Id("username")).SendKeys(gebruiker.Gebruikersnaam);
                        // pasword text field
                        driver.FindElement(By.Id("password")).SendKeys(gebruiker.Wachtwoord);
                        // login button
                        driver.FindElement(By.Id("loginbutton")).Click();
                        // cookies aanvaarden
                        driver.FindElement(By.ClassName("c-btn")).Click();
                        // find een job click
                        driver.FindElement(By.Id("vej-zoek-een-job")).Click();
                        // trefwoord
                        // een cpu pauze zodat pagina kan laden
                        Thread.Sleep(2000);
                        driver.FindElement(By.Id("search-what")).SendKeys(gebruiker.Trefwoord);
                        // gemeente, provincie
                        driver.FindElement(By.Id("vej-search-where")).SendKeys(gebruiker.Gemeente);
                    }
                    catch
                    {
                        MessageBox.Show("Gebruikersnaam of Wachtwoord is niet correct.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
                try
                {
                    // zoek de job
                    driver.FindElement(By.Id("VEJ-zoek")).Click();
                    Thread.Sleep(2000);
                    if ((bool)rdbVoltijds.IsChecked)
                    {
                        ClickDeeltijdsVoltijds("vej_arbeidsduur_0");
                    }
                    else if ((bool)rdbDeeltijds.IsChecked)
                    {
                        ClickDeeltijdsVoltijds("vej_arbeidsduur_1");
                    }
                    if (cbAantalJobs.SelectedIndex != -1)
                    {
                        try
                        {
                            for (int i = 0; i < cbAantalJobs.SelectedIndex + 1; i++)
                            {
                                driver.FindElement(By.ClassName("unsaved-star")).Click();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Fout bij het opslaan van een job.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    txtZoekeenJob.Text = string.Empty;
                    txtGemeente.Text = string.Empty;
                }
                catch
                {
                    MessageBox.Show("Er ging iets mis bij het zoeken naar een job.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Alle Tekstvelden zijn niet ingevuld.", "Invullen!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClickDeeltijdsVoltijds(string VolDeeltijds)
        {
            try
            {
                // vej-voltijds-deeltijds
                Thread.Sleep(2000);
                driver.FindElement(By.Id("vej-voltijds-deeltijds")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.Id(VolDeeltijds)).Click();
            }
            catch 
            {
                MessageBox.Show("Probleem bij het zoeken naar tijdsregeling.","Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
        }
    }
    class Gebruiker
    {
        private string _gebruikersnaam;
        private string _wachtwoord;
        private string _trefwoord;
        private string _gemeente;
        public Gebruiker() { }

        public Gebruiker(string gebruikersnaam, string wachtwoord, string trefwoord, string gemeente)
        {
            _gebruikersnaam = gebruikersnaam;
            _wachtwoord = wachtwoord;
            _trefwoord = trefwoord;
            _gemeente = gemeente;
        }
        public string Gebruikersnaam { get => _gebruikersnaam; set => _gebruikersnaam = value; }
        public string Wachtwoord { get => _wachtwoord; set => _wachtwoord = value; }
        public string Trefwoord { get => _trefwoord; set => _trefwoord = value; }
        public string Gemeente { get => _gemeente; set => _gemeente = value; }
    }
}
