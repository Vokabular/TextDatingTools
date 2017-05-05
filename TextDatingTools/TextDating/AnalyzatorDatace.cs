using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
    // TODO: z této třídy by měl dědit specializovaný český a latinský analyzátor
    public class AnalyzatorDatace
    {
        // TODO: konstanty by měly ležet mimo kód, aby se dala analyzovat (nejdřív) latina
        private const int cintZacatekKonec = 15;
        private const int cintStoLet = 100;
        private const int cintPrelom = 10;
        private const int cintOkoloRoku = 3;
        private const int cintJenPolovina = 12;


        public const string csOtaznik = "(?)";
        public const string csOkoloRoku = "okolo roku";
        public const string csPoRoce = "po roce";
        public const string csPrelom = "přelom";
        public const string csJenPolovina = "polovina";
        public const string csPolovina = ". polovina";
        //		public const string csPolovina = "polovina";
        public const string cs1polovina = "1. polovina";
        public const string cs2polovina = "2. polovina";
        public const string csLeta = ". léta";
        public const string csStoleti = ". století";
        public const string csA = " a ";
        public const string csNebo = " nebo ";
        public const string csZacatek = "začátek";
        public const string csKonec = "konec";
        public const string csTretina = ". třetina";
        public const string csCtvrtina = ". čtvrtina";
        public const string csPost = "post";
        public const string csAnte = "ante";


        private static Dictionary<string, string> mgdcTexty;
        /*
                const int cintPocetTextu = 14;
        */

        static AnalyzatorDatace()
        {
            mgdcTexty = new Dictionary<string, string>();
            //mgdcTexty.Add(cs1polovina, cs1polovina);
            //mgdcTexty.Add(cs2polovina, cs2polovina);
            mgdcTexty.Add(csPolovina, csPolovina);
            mgdcTexty.Add(csJenPolovina, csJenPolovina);

            mgdcTexty.Add(csA, csA);
            mgdcTexty.Add(csKonec, csKonec);
            mgdcTexty.Add(csLeta, csLeta);
            mgdcTexty.Add(csNebo, csNebo);
            mgdcTexty.Add(csOkoloRoku, csOkoloRoku);
            mgdcTexty.Add(csOtaznik, csOtaznik);
            //mgdcTexty.Add(csPolovina, csPolovina);
            mgdcTexty.Add(csPoRoce, csPoRoce);
            mgdcTexty.Add(csPrelom, csPrelom);
            mgdcTexty.Add(csStoleti, csStoleti);
            mgdcTexty.Add(csZacatek, csZacatek);
            mgdcTexty.Add(csTretina, csTretina);
            mgdcTexty.Add(csCtvrtina, csCtvrtina);
            mgdcTexty.Add(csAnte, csAnte);
            mgdcTexty.Add(csPost, csPost);
        }

        protected static DateTimeFormatInfo DefaultLocale = CultureInfo.GetCultureInfo("cs-CZ").DateTimeFormat;
        public DateTimeFormatInfo Locale { get; }

        public AnalyzatorDatace(DateTimeFormatInfo locale = null)
        {
            // nabízí mi to syntaktickou zkratku "Lokalizace = lokalizace ?? VychoziLokalizace", ale ta pro mě není čitelná
            Locale = locale;
            if (Locale == null)
                Locale = DefaultLocale;
        }

        public string UrcitObdobiVzniku(IDatace mdtcDatace)
        {
            /*
             * Zvolené řešení: (1. století začíná 1. 1. 1 (není 1. 1. 0)
             * 13. století: 1201-1300
             * 1. polovina 13. století: 1201-1250
             * 2. polovina 13. století: 1251-1300
             */
            /*
            Rozmezí datace; odlišné přístupy:
            XML TEI (http://www.tei-c.org/release/doc/tei-p5-doc/en/html/examples-date.html):
            <date notBefore="1250" notAfter="1300">the second half of the thirteenth century</date>

            Deep encoding of etymological information in TEI - arXiv (https://arxiv.org/pdf/1611.10122)
            <date notBefore="0350" notAfter=“0399”> IVe2 </date> (= second half of the 4th century (CE))

            TEI P5 Customization and Encoding Guidelines for Greek Manuscripts in Sweden (https://www.manuscripta.se/guidelines)
            13th century = 1201–1300
            13th century (1st half) = 1201–1250
            13th century (2nd half) = 1251-1300

            PreiodO (http://perio.do/; https://test.perio.do/#/p/)
            17th Century::1st half (1600 - 1649)
            17th Century::2nd half (1650 - 1699)

            Eagle/Europeana (https://www.eagle-network.eu/resources/vocabularies/dates/; https://www.eagle-network.eu/voc/dates/lod/291.html)
            
            first half of II CE; 0101 ; 0150
            second half of II CE; 0151 ; 0200
            third quarter of II AD: 0151 ; 0175
            beginning of II AD: 0101 ; 0110 (https://www.eagle-network.eu/voc/dates/lod/278.html)
            earlier II AD: 0101 ; 0115 (https://www.eagle-network.eu/voc/dates/lod/288.html)
            second quarter of II AD: 0126 ; 0150 (https://www.eagle-network.eu/voc/dates/lod/290.html)
            early II AD: 0101 ; 0125 (https://www.eagle-network.eu/voc/dates/lod/289.html)
            mid II AD: 0126 ; 0175 (https://www.eagle-network.eu/voc/dates/lod/292.html)
            late II AD: 0176 ; 0200 (https://www.eagle-network.eu/voc/dates/lod/295.html)

            Úseky 2. století (https://www.eagle-network.eu/voc/dates/lod/103.html)


            TEI Mediaval Manuscripts (http://medieval.bodleian.ox.ac.uk/source/added/MS.%20Canon.%20Gr.%2065)
            <date calendar="Gregorian" notAfter="1300" notBefore="1266">13th century, last third</date>
            http://medieval.bodleian.ox.ac.uk/source/added/MS.%20Auct.%20T.%204.%202
            <date calendar="Gregorian" notAfter="1460" notBefore="1440">15th century, middle</date>
            http://medieval.bodleian.ox.ac.uk/source/added/MS.%20Auct.%20T.%204.%202
            <date calendar="Gregorian" notBefore="1290" notAfter="1333">13th century, end - 14th century, first third</date>


            Syriaca (http://syriaca.org/documentation/dates.html)
            first half of 5th century: 0400; 0450
            second half of 6th century: 0550; 0600

            HOW TO USE DATES IN HISTORY (http://courses.wcupa.edu/jones/his101/misc/dates.htm)
            The first century CE includes the first one hundred years after the first minute of YEAR ONE. In other words, if YEAR ONE began at midnight of January 1, YEAR ONE, then the last day of the first century was December 31, 100.

            Český národní korpus (http://wiki.korpus.cz/doku.php/seznamy:index#zdrojove_texty_diachronnich_korpusu; http://wiki.korpus.cz/lib/exe/fetch.php/seznamy:diakorp.xlsx)
            1350-1400 (Pasionál muzejní (Muz III D 44) (R))
            1400-1450 (Životy svatých otců (UK XVII D 36) (R))

            Internetová jazyková příručka (http://prirucka.ujc.cas.cz/?id=795)
            20. a 30. léta, 20. a 21. století

            Dvacátá léta jsou ta, která mají na místě desítek dvojku, tedy např. 1820–1829, 1920–1929, 2020–2029. Třicátá léta jsou 1830–1839, 1930–1939, 2030–2039 atd. Pro roky 1900–1909, 1910–1919, 2000–2009, 2009–2019 atp. čeština žádné specifické pojmenování nemá („nultá“ a „desátá“ léta se užívají zřídka). Tato období většinou označujeme jako první léta století, začátek století, první desetiletí, druhé desetiletí apod.

            Do prvního století počítáme roky 1, 2, …, 100, do druhého století roky 101, 102, …, 200. Dvacáté století začalo 1. ledna 1901 a skončilo o půlnoci z 31. prosince 2000 na 1. ledna 2001. Devadesátá léta dvacátého století však skončila o rok dříve, 31. prosince 1999. Rok 2000 byl posledním rokem desátého desetiletí dvacátého století a zároveň posledním rokem dvacátého století. Dvacáté první století začalo dnem 1. ledna roku 2001 a skončí o půlnoci z 31. prosince 2100 na 1. ledna 2101, přičemž devadesátá léta dvacátého prvního století budou léta 2090–2099.

            Datum 1. ledna 2001 analogicky považujeme i za počátek třetího tisíciletí, jež skončí o půlnoci z 31. prosince 3000 na 1. ledna 3001.

            Způsoby sdružování časových úseků do větších celků, další členění a jejich pojmenovávání mají své historické příčiny a překrývají se v nich dva principy: aritmetický a praktický.

            */

            /*
             * 
             * https://developers.google.com/chart/interactive/docs/gallery/timeline
             * google.charts.load("current", {packages:["timeline"]});
  google.charts.setOnLoadCallback(drawChart);
  function drawChart() {

    var container = document.getElementById('timeline');
    var chart = new google.visualization.Timeline(container);
    var dataTable = new google.visualization.DataTable();
    dataTable.addColumn({ type: 'string', id: 'Position' });
    dataTable.addColumn({ type: 'string', id: 'Name' });
    dataTable.addColumn({ type: 'date', id: 'Start' });
    dataTable.addColumn({ type: 'date', id: 'End' });
    dataTable.addRows([
      			[ 'Století', '14. století', new Date(1301, 0, 0) , new Date(1400, 11, 30)],
			[ 'Století', '15. století', new Date(1401, 0, 0) , new Date(1500, 11, 30)]
			[ 'Polovina', '1. polovina', new Date(1301, 0, 0) , new Date(1350, 11, 30)],
			[ 'Polovina', '2. polovina', new Date(1351, 0, 0) , new Date(1400, 11, 30)]
			[ 'Třetina', '1. třetina', new Date(1301, 0, 0) , new Date(1333, 11, 30)],
			[ 'Třetina', '2. třetina', new Date(1334, 0, 0) , new Date(1366, 11, 30)],
			[ 'Třetina', '3. třetina', new Date(1367, 0, 0) , new Date(1400, 11, 30)]
			[ 'Čtvrtina', '1. čtvrtina', new Date(1301, 0, 0) , new Date(1325, 11, 30)],
			[ 'Čtvrtina', '2. čtvrtina', new Date(1326, 0, 0) , new Date(1350, 11, 30)],
			[ 'Čtvrtina', '3. čtvrtina', new Date(1351, 0, 0) , new Date(1375, 11, 30)],
			[ 'Čtvrtina', '4. čtvrtina', new Date(1376, 0, 0) , new Date(1400, 11, 30)]
			[ 'Desetiletí', '10. léta', new Date(1310, 0, 0) , new Date(1319, 11, 30)],
			[ 'Desetiletí', '20. léta', new Date(1320, 0, 0) , new Date(1329, 11, 30)],
			[ 'Desetiletí', '30. léta', new Date(1330, 0, 0) , new Date(1339, 11, 30)],
			[ 'Desetiletí', '40. léta', new Date(1340, 0, 0) , new Date(1349, 11, 30)],
			[ 'Desetiletí', '50. léta', new Date(1350, 0, 0) , new Date(1359, 11, 30)],
			[ 'Desetiletí', '60. léta', new Date(1360, 0, 0) , new Date(1369, 11, 30)],
			[ 'Desetiletí', '70. léta', new Date(1370, 0, 0) , new Date(1379, 11, 30)],
			[ 'Desetiletí', '80. léta', new Date(1380, 0, 0) , new Date(1389, 11, 30)],
			[ 'Desetiletí', '90. léta', new Date(1390, 0, 0) , new Date(1399, 11, 30)]
			[ 'Předěly', 'začátek 14. století', new Date(1301, 0, 0) , new Date(1315, 11, 30)],
			[ 'Předěly', 'konec 14. století', new Date(1385, 0, 0) , new Date(1400, 11, 30)],
			[ 'Předěly', 'okolo roku 1420', new Date(1417, 0, 0) , new Date(1423, 11, 30)],
			[ 'Předěly', 'po roce 1423', new Date(1423, 0, 0) , new Date(1426, 11, 30)],
			[ 'Předěly', 'přelom 13. a 14. století', new Date(1290, 0, 0) , new Date(1310, 11, 30)]
    ]);

    chart.draw(dataTable);
      }
             */

            string sDatace = null;
            if (mdtcDatace.Stoleti != 0)
            {
                if (mdtcDatace.PolovinaStoleti == 2 || (mdtcDatace.PolovinaStoleti == 0 && (mdtcDatace.NePoRoce - mdtcDatace.Stoleti) > 50))
                {
                    sDatace = String.Format("{0}51–{1}", (mdtcDatace.Stoleti / 100), mdtcDatace.Stoleti + 100);
                }
                else
                    sDatace = String.Format("{0}–{1}50", mdtcDatace.Stoleti + 1, (mdtcDatace.Stoleti / 100));
            }
            return sDatace;
        }


        public Datace AnalyzovatDataci(string sSlovniPopis)
        {
            //return AnalyzujDataci(sSlovniPopis);
            return AnalyzujDataci2(sSlovniPopis);
        }

        private Datace AnalyzujDataci2(string sSlovniPopis)
        {
            char[] chOddelovace = new char[] { '/', '–', ',' };
            Datace dt = new Datace();
            List<string> glsCoObsahuje = new List<string>();
            List<string> glsNeznamaSlova = new List<string>();
            ZjistiObsahANeznamaSlova(sSlovniPopis, ref glsCoObsahuje, ref glsNeznamaSlova);

            List<TextovyPrvek> gltpTextovePrvky = new TextovePrvky(sSlovniPopis);
            List<string> glsNeznameVyrazy = new List<string>();
            foreach (TextovyPrvek tp in gltpTextovePrvky)
            {
                if (tp.Typ == TypTextovehoPrvku.Neurceno)
                    glsNeznameVyrazy.Add(tp.Text);
            }

            if (glsNeznameVyrazy.Count > 0)
            {
                dt.Upresneni = "neznámé výrazy (" + String.Join("; ", glsNeznameVyrazy.ToArray()) + ")";
                return dt;
            }

            //if (glsNeznamaSlova.Count > 0) {
            //  dt.Upresneni = "neznámé výrazy (" + String.Join("; ", glsNeznamaSlova.ToArray()) + ")";
            //  return dt;
            //}

            string mstrSlovniPopis = sSlovniPopis;
            string sPopis = mstrSlovniPopis;

            if (glsCoObsahuje.Contains(csOtaznik))
            {
                dt.Upresneni = csOtaznik;
                glsCoObsahuje.Remove(csOtaznik);
                sPopis = sPopis.Replace(csOtaznik, "").Trim();
            }

            string[] asRozhrani = null;
            if (sSlovniPopis.IndexOfAny(chOddelovace) > 0)
            {
                asRozhrani = sPopis.Split(chOddelovace);
            }

            if (glsCoObsahuje.Contains(csNebo))
            {
                asRozhrani = sPopis.Split(new string[] { csNebo }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (asRozhrani != null)
            {
                string zacatek = asRozhrani[0].Trim();
                if (asRozhrani[1].Contains(csLeta) && !asRozhrani[0].Contains(csLeta))
                    zacatek = zacatek + asRozhrani[1].Substring(asRozhrani[1].IndexOf(csLeta, StringComparison.Ordinal) + 1);
                Datace dtZacatek = new Datace(zacatek.Trim());
                Datace dtKonec = new Datace(asRozhrani[1].Trim());
                dt = dtKonec;
                dt.NePredRokem = dtZacatek.NePredRokem;
                if (dtZacatek.Upresneni != null)
                {
                    if (dtKonec.Upresneni != null)
                        dt.Upresneni = String.Format("{0}; {1}", dtZacatek.Upresneni, dtKonec.Upresneni);
                    else
                        dt.Upresneni = dtZacatek.Upresneni;
                }

                return dt;
            }

            if (glsCoObsahuje.Contains(csPrelom))
            {
                //vypreparovat první a druhé století
                sPopis = sPopis.Remove(sPopis.IndexOf(csPrelom), csPrelom.Length + 1);
                asRozhrani = sPopis.Split(new string[] { " a " }, StringSplitOptions.RemoveEmptyEntries);
                dt.RelativniChronologie = 0;
                Datace dtKonec = new Datace(asRozhrani[1]);
                Datace dtZacatek = new Datace();
                dtZacatek.Stoleti = dtKonec.Stoleti - cintStoLet;
                dtZacatek.RelativniChronologie = 9;
                dtZacatek.NePredRokem = dtZacatek.Stoleti + (cintStoLet - cintPrelom);
                dt.Stoleti = dtKonec.Stoleti;
                dt.PolovinaStoleti = dtKonec.PolovinaStoleti;
                dt.NePredRokem = dtZacatek.NePredRokem;
                dt.NePoRoce = dtZacatek.Stoleti + (cintStoLet + cintPrelom);
                dt.Upresneni = csPrelom;

            }

            if (glsCoObsahuje.Count == 2 && glsCoObsahuje.Contains(csA) && glsCoObsahuje.Contains(csStoleti))
            {
                int iStoleti = ZjistiStoleti(sPopis);
                sPopis = sPopis.Replace(((iStoleti / cintStoLet) + 1).ToString() + csStoleti, "").Replace(csA, "").Trim();

                int iStoletiStart = ZjistiStoleti(sPopis);
                dt.Stoleti = iStoleti;
                dt.RelativniChronologie = 0;
                dt.PolovinaStoleti = 0;
                dt.Desetileti = 0;
                dt.NePredRokem = iStoletiStart + 1;
                dt.NePoRoce = iStoleti + cintStoLet;
                //dt.SlovniPopis = sSlovniPopis;
                return dt;
            }

            if (glsCoObsahuje.Contains(csStoleti))
            {
                //vypreparovat století, analayzovat zbytek
                int iStoleti = ZjistiStoleti(sPopis);
                dt.Stoleti = iStoleti;

                sPopis = sPopis.Replace(((iStoleti / cintStoLet) + 1).ToString() + csStoleti, "").Trim();
                if (glsCoObsahuje.Contains(csLeta) ||
                    glsCoObsahuje.Contains(csPolovina) ||
                    glsCoObsahuje.Contains(csTretina) ||
                    glsCoObsahuje.Contains(csCtvrtina) ||
                    glsCoObsahuje.Contains(csJenPolovina))
                {

                    CasovyZlomek zl = ZjistiZlomek(sPopis);
                    sPopis = sPopis.Replace(zl.CitatelPopis + zl.JmenovatelPopis, "").Trim();

                    if (zl.Jmenovatel == 10)
                    {
                        dt.Desetileti = zl.Citatel;
                        dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(dt.Desetileti * zl.Jmenovatel);
                        dt.NePredRokem = dt.Stoleti + zl.Jmenovatel * (zl.Citatel == 1 ? 0 : zl.Citatel) + 1;
                        dt.NePoRoce = dt.NePredRokem + zl.Jmenovatel;
                    }
                    else if (zl.Jmenovatel == 2 && zl.Citatel == -1)
                    {

                        dt.PolovinaStoleti = 0;
                        dt.RelativniChronologie = 5;
                        dt.Upresneni = csJenPolovina;

                        dt.NePredRokem = dt.Stoleti + (50) - cintJenPolovina + 1;
                        dt.NePoRoce = dt.NePredRokem + 50;
                        //int desetileti = dt.NePoRoce - dt.Stoleti;
                        //dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(desetileti);
                    }
                    else
                    {
                        dt.Stoleti = iStoleti;
                        dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(((cintStoLet / zl.Jmenovatel) * (zl.Citatel)));
                        dt.Desetileti = ((cintStoLet / zl.Jmenovatel) * (zl.Citatel)) / 10;

                        dt.NePredRokem = dt.Stoleti + ((cintStoLet / zl.Jmenovatel) * (zl.Citatel - 1)) + 1;
                        dt.NePoRoce = dt.Stoleti + ((cintStoLet / zl.Jmenovatel) * (zl.Citatel));

                        RelativniChronologieNaZakladeRoku(dt.NePoRoce);
                    }

                }
                else if (glsCoObsahuje.Count == 1)
                {
                    dt.RelativniChronologie = 0;
                    dt.NePredRokem = dt.Stoleti + 1;
                    dt.NePoRoce = dt.Stoleti + cintStoLet;
                }
            }
            if (sPopis == csZacatek)
            {
                dt.RelativniChronologie = 1;
                dt.NePredRokem = dt.Stoleti;
                dt.NePoRoce = dt.Stoleti + cintZacatekKonec;
                dt.Upresneni = csZacatek;
                dt.Desetileti = 2;
                dt.PolovinaStoleti = 1;
                sPopis = sPopis.Replace(csZacatek, "").Trim();
            }
            if (sPopis == csKonec)
            {
                dt.NePoRoce = dt.Stoleti + cintStoLet;
                dt.NePredRokem = dt.Stoleti + cintStoLet - cintZacatekKonec;
                dt.RelativniChronologie = 9;
                dt.PolovinaStoleti = 2;
                dt.Desetileti = 9;
                dt.Upresneni = csKonec;
                sPopis = sPopis.Replace(csKonec, "").Trim();

            }

            if (glsCoObsahuje.Contains(csPoRoce))
            {
                sPopis = sPopis.Remove(sPopis.IndexOf(csPoRoce), csPoRoce.Length);
                dt.Upresneni = csPoRoce;
                int iRok;
                if (Int32.TryParse(sPopis, out iRok))
                {
                    UrciDataciNaZakladeRoku(dt, iRok);
                    dt.NePoRoce = dt.Rok + cintOkoloRoku; //TODO Tady by mělo být koncové datum, např. úmrtí autora
                    dt.NePredRokem = dt.Rok;
                }
            }
            if (glsCoObsahuje.Contains(csOkoloRoku))
            {
                sPopis = sPopis.Remove(sPopis.IndexOf(csOkoloRoku), csOkoloRoku.Length);
                dt.Upresneni = csOkoloRoku;
                int iRok;
                if (Int32.TryParse(sPopis, out iRok))
                {
                    UrciDataciNaZakladeRoku(dt, iRok);
                    dt.NePoRoce = dt.Rok + cintOkoloRoku;
                    dt.NePredRokem = dt.Rok - cintOkoloRoku;
                    int desetileti = dt.NePoRoce - dt.Stoleti;
                    dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(desetileti);
                }
            }

            if (glsCoObsahuje.Contains(csPost))
            {
                sPopis = sPopis.Remove(sPopis.IndexOf(csPost), csPost.Length + 1);
                dt.Upresneni = csPost;
                int iRok;
                if (Int32.TryParse(sPopis.Substring(0, 4), out iRok))
                {
                    UrciDataciNaZakladeRoku(dt, iRok);
                    dt.NePoRoce = iRok;
                }
            }

            if (glsCoObsahuje.Contains(csAnte))
            {
                sPopis = sPopis.Remove(sPopis.IndexOf(csAnte), csAnte.Length + 1);
                if (!String.IsNullOrEmpty(dt.Upresneni))
                {
                    dt.Upresneni += "; " + csAnte;
                }
                else
                {
                    dt.Upresneni = csAnte;
                }
                int iRok;
                if (Int32.TryParse(sPopis.Substring(0, 4), out iRok))
                {
                    UrciDataciNaZakladeRoku(dt, iRok);
                    dt.NePoRoce = iRok;
                }
            }

            if (glsCoObsahuje.Count == 1 && glsCoObsahuje.Contains(csA))
            {
                asRozhrani = sPopis.Split(new string[] { csA }, StringSplitOptions.RemoveEmptyEntries);

                if (asRozhrani != null)
                {
                    string zacatek = asRozhrani[0].Trim();
                    if (asRozhrani[1].Contains(csLeta) && !asRozhrani[0].Contains(csLeta))
                        zacatek = zacatek + asRozhrani[1].Substring(asRozhrani[1].IndexOf(csLeta, StringComparison.Ordinal) + 1);
                    Datace dtZacatek = new Datace(zacatek.Trim());
                    Datace dtKonec = new Datace(asRozhrani[1].Trim());
                    dt = dtKonec;
                    dt.NePredRokem = dtZacatek.NePredRokem;
                    if (dtZacatek.Upresneni != null)
                    {
                        if (dtKonec.Upresneni != null)
                            dt.Upresneni = String.Format("{0}; {1}", dtZacatek.Upresneni, dtKonec.Upresneni);
                        else
                            dt.Upresneni = dtZacatek.Upresneni;
                    }

                    return dt;
                }
            }

            //jde pouze o rok
            if (glsCoObsahuje.Count == 0)
            {
                int iRok;
                DateTime date;
                if (DateTime.TryParse(sPopis, Locale, DateTimeStyles.None, out date))
                {
                    iRok = date.Year;
                    UrciDataciNaZakladeRoku(dt, iRok);
                }
                else if (Int32.TryParse(sPopis, out iRok))
                {
                    UrciDataciNaZakladeRoku(dt, iRok);
                }
                // TODO: upozorňovat na neplatné údaje pomocí výjimek, které se na vhodné úrovni zachytí a vytvoří se popis, co je v dataci špatné nebo neznámé
            }
            return dt;

        }


        private static CasovyZlomek ZjistiZlomek(string sPopis)
        {
            CasovyZlomek zl = new CasovyZlomek();
            string sJmenovatel = csPolovina;

            int iPozice = sPopis.IndexOf(sJmenovatel);
            if (iPozice == -1)
            {
                sJmenovatel = csTretina;
                iPozice = sPopis.IndexOf(sJmenovatel);
            }
            if (iPozice == -1)
            {
                sJmenovatel = csCtvrtina;
                iPozice = sPopis.IndexOf(sJmenovatel);
            }
            if (iPozice == -1)
            {
                sJmenovatel = csLeta;
                iPozice = sPopis.IndexOf(sJmenovatel);
            }
            if (iPozice == -1)
            {
                sJmenovatel = csJenPolovina;
                iPozice = sPopis.IndexOf(sJmenovatel);
            }
            if (iPozice == -1)
                return zl;
            zl.JmenovatelPopis = sJmenovatel;
            zl.Jmenovatel = CasovyZlomek.JmenovatelPopisNaCislo(sJmenovatel);

            string sCitatel = null;
            if (sJmenovatel == csLeta)
            {
                sCitatel = sPopis.Substring(sPopis.IndexOf(sJmenovatel) - 2, 2);
            }
            else if (sJmenovatel == csJenPolovina)
            {
                sCitatel = "";
            }
            else
            {
                sCitatel = sPopis.Substring(sPopis.IndexOf(sJmenovatel) - 1, 2);
            }
            if (sCitatel.EndsWith("."))
                sCitatel = sCitatel.Substring(0, sCitatel.Length - 1);
            zl.CitatelPopis = sCitatel;
            if (sCitatel == "")
                zl.Citatel = 0;
            zl.Citatel = CasovyZlomek.CitatelPopisNaCislo(sCitatel);
            return zl;

        }

        private static int ZjistiStoleti(string sPopis)
        {
            int iStoleti = -1;
            int iPozice = sPopis.IndexOf(csStoleti);
            if (iPozice > 0)
            {
                //předpokládá se dvoumístný údaj o století - rozšířit i na 9. století a níže
                string sText = sPopis.Substring(iPozice - 2, 2).Trim();
                iStoleti = ZjistiStoleti(sText, iStoleti);
            }
            else
            {
                Regex regex = new Regex("(\\d+)\\.");
                if(regex.IsMatch(sPopis))
                {
                    string sText = regex.Match(sPopis).Groups[1].Value;
                    iStoleti = ZjistiStoleti(sText, iStoleti);
                }
            }
            return iStoleti;
        }

        private static int ZjistiStoleti(string sText, int iStoleti)
        {
            if (Int32.TryParse(sText, out iStoleti))
            {
                iStoleti = (iStoleti - 1) * 100;
            }
            return iStoleti;
        }


        private static void UrciDataciNaZakladeRoku(IDatace dt, int iRok)
        {
            dt.Rok = iRok;
            dt.Stoleti = (iRok / 100) * 100;
            int iDesetileti = dt.Rok - dt.Stoleti;
            dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(iDesetileti);
            dt.Desetileti = iDesetileti / 10;
            dt.RelativniChronologie = RelativniChronologieNaZakladeRoku(iRok);
            if (dt.NePredRokem == 0)
                dt.NePredRokem = dt.Rok;
            dt.NePoRoce = dt.Rok;
        }

        private static void ZjistiObsahANeznamaSlova(string sSlovniPopis, ref List<string> glsCoObsahuje, ref List<string> glNeznamaSlova)
        {
            StringBuilder sbSlovniPopis = new StringBuilder(sSlovniPopis);

            foreach (string sval in mgdcTexty.Values)
            {
                while (sbSlovniPopis.ToString().Contains(sval))
                {
                    int i = sbSlovniPopis.ToString().IndexOf(sval);
                    sbSlovniPopis.Remove(i, sval.Length);
                    glsCoObsahuje.Add(sval);
                }
            }

            const string csPismena = "0123456789/–.";
            string[] asSlova = sbSlovniPopis.ToString().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string sSlovo in asSlova)
            {
                for (int i = 0; i < sSlovo.Length; i++)
                {
                    if (!(csPismena.Contains(sSlovo[i].ToString())))
                    {
                        glNeznamaSlova.Add(sSlovo);
                        break;
                    }
                }
            }
        }

        private static int PolovinaStoletiNaZakladeDesetileti(int iDesetileti)
        {
            return iDesetileti < 51 ? 1 : 2;
        }

        private static int RelativniChronologieNaZakladeRoku(int iRok)
        {
            int iStoleti = (iRok / 100) * 100;
            int iDesetileti = iRok - iStoleti;
            int iTemp = (int)(Math.Truncate(iDesetileti / 12.5)) + 1; //snad to bude fungovat; ve VB Fix
            return iTemp;
        }

        // TODO: VS píše, že se tato funkce nepoužívá -- už nebo ještě?
        private static void RozebratUpresneniVPopisu(ref string sPopis, string sUpresneni, ref Datace dtDatace)
        {
            if (sPopis.Contains(sUpresneni))
            {
                dtDatace.Upresneni = String.IsNullOrEmpty(dtDatace.Upresneni) ? sUpresneni : dtDatace.Upresneni + " " + sUpresneni;
                sPopis = sPopis.Replace(sUpresneni, "").Trim();
            }
        }
    }
}
