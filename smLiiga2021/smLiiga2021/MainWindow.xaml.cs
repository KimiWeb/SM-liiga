using System;
using System.Windows;
using System.Xml;

namespace smLiiga2021
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        struct pelaajaTiedot
        {
            public string nro;
            public string nimi;
            public int maalienLkm;
        }
        pelaajaTiedot[] kotiPelaaja = new pelaajaTiedot[50];
        pelaajaTiedot[] vierasPelaaja = new pelaajaTiedot[50];

        int kotiMaalit = 0;
        int vierasMaalit = 0;
        string kotiJoukkue = "kotijoukkue";
        string vierasJoukkue = "vierasjoukkue";
        string maalintekija = "";

        public MainWindow()
        {
            InitializeComponent();
            KirjaaOtteluTiedot();
            XmlReader lukija = XmlReader.Create("SMliiga.xml");
            string joukkue = "";
            lukija.MoveToContent();

            while (lukija.Read())
            {
                // luetaan joukkuetiedot 
                if (lukija.NodeType == XmlNodeType.Element &&
                   lukija.Name == "Joukkue")
                {
                    if (lukija.HasAttributes)
                    {
                        joukkue = lukija.GetAttribute("nimi");
                        lstKotijoukkue.Items.Add(joukkue);
                        lstVierasjoukkue.Items.Add(joukkue);
                    }
                }
            }
        }
        void tyhjennäKotiPelaajaArray()
        {
            for (int i = 0; i < kotiPelaaja.Length; i++)
            {
                kotiPelaaja[i].nimi = "";
                kotiPelaaja[i].nro = "";
                kotiPelaaja[i].maalienLkm = 0;
            }
        }
        void tyhjennäVierasPelaajaArray()
        {
            for (int i = 0; i < kotiPelaaja.Length; i++)
            {
                vierasPelaaja[i].nimi = "";
                vierasPelaaja[i].nro = "";
                vierasPelaaja[i].maalienLkm = 0;
            }
        }

        private void lstKotijoukkue_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            tyhjennäKotiPelaajaArray();
            lstKotipelaajat.Items.Clear();
            lstKotiMaalit.Items.Clear();

            string haettavaJoukkue = lstKotijoukkue.SelectedItem.ToString();
            kotiJoukkue = haettavaJoukkue;
            tarkastaTiimivalinnat();
            KirjaaOtteluTiedot();
            lblKotijoukkue.Content = haettavaJoukkue;
            lblKotiMaalit.Content = kotiMaalit;
            

            XmlReader lukija = XmlReader.Create("SMliiga.xml");
            string joukkue = "";
            lukija.MoveToContent();
            // tyhjätään listalaatikko 
            // lue niin kauan kuin XML-tiedostossa riittää tietoa 
            int ind = 0;

            while (lukija.Read())
            {
                // luetaan joukkuetiedot 
                if (lukija.NodeType == XmlNodeType.Element &&
                   lukija.Name == "Joukkue")
                {
                    if (lukija.HasAttributes)
                    {
                        joukkue = lukija.GetAttribute("nimi");
                        if (joukkue == haettavaJoukkue)
                        {
                            // luetaan yhden joukkueen pelaajat array-tauluun 
                            while (lukija.Read())
                            {
                                // lopetetaan tämä silmukka, kun joukkue vaihtuu (huom. EndElement) 
                                // break-käsky lopettaa silmukan 
                                if (lukija.NodeType == XmlNodeType.EndElement &&
                                    lukija.Name == "Joukkue")
                                {
                                    break;
                                }
                                if (lukija.NodeType == XmlNodeType.Element &&
                                lukija.Name == "Nimi")
                                {
                                    lukija.Read();
                                    kotiPelaaja[ind].nimi = lukija.Value;
                                    lstKotipelaajat.Items.Add(lukija.Value);
                                }

                                if (lukija.NodeType == XmlNodeType.Element &&
                                     lukija.Name == "Pelaajanro")
                                {
                                    lukija.Read();
                                    kotiPelaaja[ind].nro = lukija.Value;
                                    ind++;
                                }
                            }
                        }
                    }            
                }
            }
        }

        private void btnKirjaaKotiMaali_Click(object sender, RoutedEventArgs e)
        {
            if (lstKotipelaajat.SelectedIndex < 0)
            {
                MessageBox.Show("Valitse maalin tehnyt pelaaja");
                return;
            }
            DateTime tämäHetki = DateTime.Now;
            string kellonAika = tämäHetki.ToShortTimeString();

            lstKotiMaalit.Items.Add(kellonAika + " " + lstKotipelaajat.SelectedItem.ToString());
            kotiMaalit++;
            KirjaaOtteluTiedot();
            lblKotiMaalit.Content = kotiMaalit;

            int maalinTekijänInd = lstKotipelaajat.SelectedIndex;
            kotiPelaaja[maalinTekijänInd].maalienLkm++;

            if (kotiPelaaja[maalinTekijänInd].maalienLkm == 3)
            {
                maalintekija = kotiPelaaja[maalinTekijänInd].nimi;
                onnittelePelaajaa();
            }
        }

        private void btnKirjaaVierasMaali_Click(object sender, RoutedEventArgs e)
        {
            if (lstVieraspelaajat.SelectedIndex < 0)
            {
                MessageBox.Show("Valitse maalin tehnyt pelaaja");
                return;
            }
            DateTime tämäHetki = DateTime.Now;
            string kellonAika = tämäHetki.ToShortTimeString();

            lstVierasMaalit.Items.Add(kellonAika + " " + lstVieraspelaajat.SelectedItem.ToString());
            vierasMaalit++;
            KirjaaOtteluTiedot();
            lblVierasMaalit.Content = vierasMaalit;

            int maalinTekijänInd = lstVieraspelaajat.SelectedIndex;
            vierasPelaaja[maalinTekijänInd].maalienLkm++;

            if (vierasPelaaja[maalinTekijänInd].maalienLkm == 3)
            {
                maalintekija = vierasPelaaja[maalinTekijänInd].nimi;
                onnittelePelaajaa();
            }
        }

        private void lstVierasjoukkue_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            tyhjennäVierasPelaajaArray();
            lstVieraspelaajat.Items.Clear();
            lstVierasMaalit.Items.Clear();

            string haettavaJoukkue = lstVierasjoukkue.SelectedItem.ToString();
            vierasJoukkue = haettavaJoukkue;
            tarkastaTiimivalinnat();
            KirjaaOtteluTiedot();
            lblVierasjoukkue.Content = haettavaJoukkue;
            lblVierasMaalit.Content = vierasMaalit; 

            XmlReader lukija = XmlReader.Create("SMliiga.xml");
            string joukkue = "";
            lukija.MoveToContent();
            // tyhjätään listalaatikko 
            // lue niin kauan kuin XML-tiedostossa riittää tietoa 
            int ind = 0;

            while (lukija.Read())
            {
                // luetaan joukkuetiedot 
                if (lukija.NodeType == XmlNodeType.Element &&
                   lukija.Name == "Joukkue")
                {
                    if (lukija.HasAttributes)
                    {
                        joukkue = lukija.GetAttribute("nimi");
                        if (joukkue == haettavaJoukkue)
                        {
                            // luetaan yhden joukkueen pelaajat array-tauluun 
                            while (lukija.Read())
                            {
                                // lopetetaan tämä silmukka, kun joukkue vaihtuu (huom. EndElement) 
                                // break-käsky lopettaa silmukan 
                                if (lukija.NodeType == XmlNodeType.EndElement &&
                                    lukija.Name == "Joukkue")
                                {
                                    break;
                                }
                                if (lukija.NodeType == XmlNodeType.Element &&
                                lukija.Name == "Nimi")
                                {
                                    lukija.Read();
                                    vierasPelaaja[ind].nimi = lukija.Value;
                                    lstVieraspelaajat.Items.Add(lukija.Value);
                                }

                                if (lukija.NodeType == XmlNodeType.Element &&
                                     lukija.Name == "Pelaajanro")
                                {
                                    lukija.Read();
                                    vierasPelaaja[ind].nro = lukija.Value;
                                    ind++;
                                }
                            }
                        }
                    }
                }
            }
        }
        //ei saa lähtötietoja ja ei palauta niitä
      /*  void tarkastaTiimivalinnatVieras()
        {
            if(vierasJoukkue == kotiJoukkue)
            {
                MessageBox.Show("Virhe valitse eri tiimi kuin vastustajalla");
                return;
            }
        }*/
        void tarkastaTiimivalinnat()
        {
            if (vierasJoukkue == kotiJoukkue)
            {
                MessageBox.Show("Valitse eri tiimi kuin vastustajalla");
            } 
        }
        void onnittelePelaajaa()
        {
            MessageBox.Show("Onnittelut "
                                  + maalintekija
                                  + " hän teki juuri kypärätempun!");
        }
        void KirjaaOtteluTiedot()
        {
            lblPelintiedot.Content = pvm.Text
                                    + " "
                                    + kotiJoukkue
                                    + " - "
                                    + vierasJoukkue;
            lblTilanne.Content = kotiMaalit
                                 + " - "
                                 + vierasMaalit;
        }

        private void btnNextTab_Click(object sender, RoutedEventArgs e)
        {
            int newIndex = smLiigaTAB.SelectedIndex + 1;
            if (newIndex >= smLiigaTAB.Items.Count)
                newIndex = 0;
            smLiigaTAB.SelectedIndex = newIndex;
        }

        private void pvm_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
