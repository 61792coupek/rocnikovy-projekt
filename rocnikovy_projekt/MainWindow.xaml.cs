using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace rocnikovy_projekt
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _dialogText;
        public string DialogText
        {
            get => _dialogText;
            set
            {
                _dialogText = value;
                OnPropertyChanged();
            }
        }

        private string _inventarText;
        public string InventarText
        {
            get => _inventarText;
            set
            {
                _inventarText = value;
                OnPropertyChanged();
            }
        }

        // Změněno z VypsanoVDeniku na Vypsano, aby sedělo s XAML Bindingem
        private string _vypsano;
        public string Vypsano
        {
            get => _vypsano;
            set
            {
                _vypsano = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            DialogText = "CÍL:\nDOSTAŇ SE DO DOMU A VYKRADNI TREZOR";
            InventarText = "Inventář:\n -";
            Vypsano = "Poznámky:\n";
        }

        // Menu
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            StartHry();
        }

        private void btn_ukoncit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartHry()
        {
            Menu.Visibility = Visibility.Collapsed;
            Scena1.Visibility = Visibility.Visible;
        }

        // SCÉNA 1 a globální proměnné

        bool maSprej = false;
        bool maKamen = false;
        bool maKlic = false;
        bool maRukavice = false;
        bool kameraNefunguje = false;

        // SCÉNA 2 - nové proměnné
        bool maPero = false;
        bool maDenik = false;
        int trezorPokusy = 3;

        // --- AKTUALIZOVANÁ METODA PRO INVENTÁŘ ---
        private void AktualizujInventar()
        {
            string text = "Inventář:\n";
            bool maNeco = false;

            if (maSprej) { text += "- Sprej\n"; maNeco = true; }
            if (maKamen) { text += "- Kámen\n"; maNeco = true; }
            if (maRukavice) { text += "- Rukavice\n"; maNeco = true; }
            if (maKlic) { text += "- Klíč\n"; maNeco = true; }
            if (maPero) { text += "- Pero\n"; maNeco = true; }
            if (maDenik) { text += "- Deník\n"; maNeco = true; }

            // Pokud hráč nic nemá, vypíše se pomlčka
            if (!maNeco)
            {
                text += " -";
            }

            InventarText = text;
        }

        // --- AKTUALIZOVANÁ METODA PROHRY ---
        private void ProhralJsi()
        {
            Menu.Visibility = Visibility.Visible;
            Scena1.Visibility = Visibility.Collapsed;
            Scena2.Visibility = Visibility.Collapsed;

            // Pojistka: Pokud prvek Trezor ještě v XAML nemáš, zabrání to spadnutí hry
            if (Trezor != null)
            {
                Trezor.Visibility = Visibility.Collapsed;
            }

            DialogText = "CÍL:\nDOSTAŇ SE DO DOMU A VYKRADNI TREZOR";

            // Reset všech proměnných
            maSprej = false;
            maKamen = false;
            maKlic = false;
            maRukavice = false;
            kameraNefunguje = false;

            maPero = false;
            maDenik = false;
            trezorPokusy = 3;
            Vypsano = ""; // Vymaže poznámky z bloku

            AktualizujInventar(); // Automaticky vyprázdní inventář
        }

        private void MapImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void btn_kvetinac_Click(object sender, RoutedEventArgs e)
        {
            if (maKlic == false)
            {
                maKlic = true;
                AktualizujInventar();

                if (kameraNefunguje == false)
                    DialogText = "Za květináčem jsem našel klíč. Ale ta kamera mě pořád vidí...";
                else
                    DialogText = "Našel jsem klíč! Teď už můžu dovnitř.";
            }
            else
            {
                DialogText = "Klíč už mám, v květináči jsou už jen žížaly.";
            }
        }

        private void btn_kamen_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "To tady nikdo neuklízí, že tady leží kámen uprostřed cesty?\nMožná bych s ním ale mohl něco rozbít...";
            maKamen = true;
            maSprej = false;
            AktualizujInventar();
        }

        private void btn_spray_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Co tady dělá sprej? Ale třeba ho nějak můžu využít...";
            maSprej = true;
            maKamen = false;
            AktualizujInventar();
        }

        private async void btn_dvere_Click(object sender, RoutedEventArgs e)
        {
            if (maRukavice && kameraNefunguje && maKlic)
            {
                DialogText = "Super! Dostal jsem se dovnitř bez toho, aby mě někdo viděl. Teď už jen stačí zjistit 4místný PIN kód od trezoru a mám to!";
                PrejdiDoKancelare();
            }
            // Ostatní podmínky (prohry)
            else if (!maRukavice)
            {
                DialogText = "Nechal jsem na dveřích otisky prstů, teď budu dopaden.\nNAŠLI MĚ";
                await Task.Delay(4000);
                ProhralJsi();
            }
            else if (!kameraNefunguje)
            {
                DialogText = "Viděli mě na kameře.\nNAŠLI MĚ";
                await Task.Delay(4000);
                ProhralJsi();
            }
            else
            {
                DialogText = "Ještě dovnitř nemůžu, nemám klíč.";
            }
        }

        private async void btn_kamera_Click(object sender, RoutedEventArgs e)
        {
            if (maSprej == false && maKamen == false)
            {
                DialogText = "Potřebuju abych na té kameře nebyl vidět. Něco tady musí být co mi pomůže...";
            }
            else if (maKamen == true)
            {
                DialogText = "Rozbil sem kameru kamenem a spustil se alarm.\nNAŠLI MĚ";
                await Task.Delay(4000);
                ProhralJsi();
            }
            else if (maSprej == true && maKlic == false)
            {
                DialogText = "Kamera je zasprejovaná, nespustilo to alarm a nebudu tam vidět. Teď už jen najít klíč a můžu dovnitř.";
                kameraNefunguje = true;
            }
            else //má klíč a kamera je zasprejovaná
            {
                DialogText = "Kamera je zasprejovaná, nespustilo to alarm a nebudu tam vidět. Můžu dovnitř!";
                kameraNefunguje = true;
            }
        }

        private void btn_rukavice_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Kožené rukavice? No jasně! Přece na těch luxusních dveřích nenechám své otisky prstů. Beru je.";
            maRukavice = true;
            AktualizujInventar();
        }

        private void btn_obejitZezadu_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Zezadu to nepůjde...";
        }

        private void btn_ker_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "V keři nic není...";
        }

        private void PrejdiDoKancelare()
        {
            Scena1.Visibility = Visibility.Collapsed;
            Scena2.Visibility = Visibility.Visible;
        }

        // ======================= SCÉNA 2 =======================

        private void btn_policka_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Na poličce jsou jen nezajímavé knihy.";
        }

        private void btn_obraz_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Zajímavý obraz, ale trezor za ním schovaný není.";
        }

        private void btn_vesak_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Na věšáku visí prázdný kabát, nic v něm není.";
        }

        private async void btn_socha_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Sakra! Shodil jsem sochu a udělala obrovský rámus.\nNAŠLI MĚ";
            await Task.Delay(4000);
            ProhralJsi();
        }

        private async void btn_telefon_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Telefon zrovna začal vyzvánět! Zvedl jsem to a prozradil se.\nNAŠLI MĚ";
            await Task.Delay(4000);
            ProhralJsi();
        }

        private void btn_pero_Click(object sender, RoutedEventArgs e)
        {
            if (!maPero)
            {
                DialogText = "Našel jsem pero. Beru si ho, možná si budu potřebovat něco zapsat.";
                maPero = true;
                AktualizujInventar();
            }
            else
            {
                DialogText = "Pero už mám v inventáři.";
            }
        }

        private void btn_denik_Click(object sender, RoutedEventArgs e)
        {
            if (!maDenik)
            {
                DialogText = "Prázdný deník. Vezmu si ho na poznámky.";
                maDenik = true;
                AktualizujInventar();
            }
            else
            {
                DialogText = "Deník už mám u sebe.";
            }
        }

        // --- METODA PRO ZÁPIS ČÍSEL ---
        private void ZapisCislo(string cislo)
        {
            // Zjišťuje, jestli už má hráč pero i deník
            if (maPero && maDenik)
            {
                if (string.IsNullOrEmpty(Vypsano))
                {
                    Vypsano = "Poznámky:\n";
                }

                // Přidá číslo, jen pokud už není zapsané
                if (!Vypsano.Contains(cislo))
                {
                    Vypsano += $"- {cislo}\n";
                }
            }
            else if (maDenik && !maPero)
            {
                DialogText += "\n(Mám sice deník, ale nemám čím psát!)";
            }
            else if (!maDenik && maPero)
            {
                DialogText += "\n(Mám pero, ale nemám si to kam zapsat!)";
            }
        }

        private void btn_dres_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Na zdi visí podepsaný sportovní dres s číslem 11.";
            ZapisCislo("11");
        }

        private void btn_taska_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "V tašce se válí papírek s napsaným číslem 20.";
            ZapisCislo("20");
        }

        private void btn_slozka_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "V téhle složce je dokument majitele domu. Narodil se v roce 1967.";
            ZapisCislo("67");
        }

        private void btn_mapa_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Na mapě je červeně zvýrazněný dům s číslem popisným 53.";
            ZapisCislo("53");
        }

        private void btn_suplik_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "Šuplík je skoro prázdný, je tu jen 13 kancelářských sponek.";
            ZapisCislo("13");
        }

        private void btn_dopis_Click(object sender, RoutedEventArgs e)
        {
            DialogText = "V dopise je napsáno 'Heslo je DRESROK'. To je zvláštní, co by to mohlo asi znamenat?";
            ZapisCislo("DRESROK");
        }

        private void btn_trezor_Click(object sender, RoutedEventArgs e)
        {
            DialogText = $"Trezor! Musím zadat 4místný PIN. Mám na to ještě {trezorPokusy} pokusy.";
            trezorZadaniPinu();
        }

        private void trezorZadaniPinu()
        {
            Scena2.Visibility = Visibility.Collapsed;
            Trezor.Visibility = Visibility.Visible;
        }

        // TREZOR

        private async void Button_potvrdit_Click(object sender, RoutedEventArgs e)
        {
            string zadanyPin = txt_pin.Text; // Načte text z TextBoxu

            if (zadanyPin == "1167")
            {
                // VÝHRA
                DialogText = "Trezor se otevřel.VYHRÁL JSEM!";
                txt_pin.IsEnabled = false; // Zakáže další psaní
                await Task.Delay(4000);
                ProhralJsi();
            }
            else
            {
                // ŠPATNÝ PIN
                trezorPokusy--; // Odečte pokus
                txt_pin.Text = ""; // Vymaže text pro další pokus

                if (trezorPokusy > 0)
                {
                    DialogText = $"Špatný PIN! Zbývající pokusy: {trezorPokusy}";
                }
                else
                {
                    // PROHRA
                    DialogText = "Sakra! Spustil se alarm a zámek se zablokoval..\nNAŠLI MĚ!";
                    await Task.Delay(4000);
                    ProhralJsi();
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_zpet_Click(object sender, RoutedEventArgs e)
        {
            Scena2.Visibility = Visibility.Visible;
            Trezor.Visibility = Visibility.Collapsed;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}