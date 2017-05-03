using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daliboris.Texty.Evidence.Objekty.UnitTests
{
    [TestClass()]
    public class AnalyzatorDataceTests
    {
        private TestContext _context;

        public TestContext TestContext
        {
            get { return _context; }
            set { _context = value; }
        }

        [TestMethod()]
        [DeploymentItem("TestData\\AnalyzatorDataceTestData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\TestData\\AnalyzatorDataceTestData.xml",
            "Datace",
            DataAccessMethod.Sequential)]
        public void AnalyzovatDataci()
        {
            int stoleti = Int32.Parse((string) TestContext.DataRow["Stoleti"]);
            int polovinaStoleti = Int32.Parse((string) TestContext.DataRow["PolovinaStoleti"]);
            int desetileti = Int32.Parse((string) TestContext.DataRow["Desetileti"]);
            int rok = Int32.Parse((string) TestContext.DataRow["Rok"]);
            int relativniChronologie = Int32.Parse((string) TestContext.DataRow["RelativniChronologie"]);
            int nePredRokem = Int32.Parse((string) TestContext.DataRow["NePredRokem"]);
            int nePoRoce = Int32.Parse((string) TestContext.DataRow["NePoRoce"]);
            string upresneni = null;
            if (TestContext.DataRow["Upresneni"] != DBNull.Value)
                upresneni = (string) TestContext.DataRow["Upresneni"];
            string nazev = (string) TestContext.DataRow["Nazev"];

            string obdobiVzniku = (string) TestContext.DataRow["ObdobiVzniku"];


            string slovniPopis = (string) TestContext.DataRow["SlovniPopis"];

            AnalyzovatDataciJednotlive(slovniPopis, stoleti, polovinaStoleti, desetileti, rok, relativniChronologie,
                nePredRokem, nePoRoce, upresneni, nazev);
        }

        private void AnalyzovatDataciJednotlive(string slovniPopis,
            int stoleti,
            int polovinaStoleti,
            int desetileti,
            int rok,
            int relativniChronologie,
            int nePredRokem,
            int nePoRoce,
            string upresneni,
            string nazev)
        {
            string message = String.Format("{0}: {1}", nazev, slovniPopis);


            Datace datace = new Datace(slovniPopis);
            Assert.AreEqual(nePredRokem, datace.NePredRokem, message);
            Assert.AreEqual(nePoRoce, datace.NePoRoce, message);
            Assert.AreEqual(rok, datace.Rok, message);
            Assert.AreEqual(stoleti, datace.Stoleti, message);
            Assert.AreEqual(desetileti, datace.Desetileti, message);
            Assert.AreEqual(polovinaStoleti, datace.PolovinaStoleti, message);
            Assert.AreEqual(relativniChronologie, datace.RelativniChronologie, message);
            Assert.AreEqual(upresneni, datace.Upresneni, message);


        }

        [TestMethod()]
        [DeploymentItem("TestData\\AnalyzatorDataceTestData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\TestData\\AnalyzatorDataceTestData.xml",
            "Datace",
            DataAccessMethod.Sequential)]
        public void AnalyzovatObdobiVzniku()
        {
            string obdobiVzniku = (string) TestContext.DataRow["ObdobiVzniku"];
            string slovniPopis = (string) TestContext.DataRow["SlovniPopis"];
            string nazev = (string) TestContext.DataRow["Nazev"];

            AnalyzovatDataciJednotlive(slovniPopis, obdobiVzniku, nazev);
        }

        private void AnalyzovatDataciJednotlive(string slovniPopis, string obdobiVzniku, string nazev)
        {
            string message = String.Format("{0}: {1}", nazev, slovniPopis);

            Datace datace = new Datace(slovniPopis);
            string obdobi = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual(obdobiVzniku, obdobi, message);

        }

        [TestMethod()]
        public void AnalyzovatDataci1polovinaStoleti()
        {
            string popis = "1. polovina 14. století";
            Datace datace = new Datace(popis);

            Assert.AreEqual(1301, datace.NePredRokem);
            Assert.AreEqual(1350, datace.NePoRoce);
            Assert.AreEqual(1300, datace.Stoleti);
            Assert.AreEqual(5, datace.Desetileti);
            Assert.AreEqual(0, datace.Rok);
            Assert.AreEqual(1, datace.PolovinaStoleti);
            Assert.AreEqual(0, datace.RelativniChronologie);

            string obdobi = AnalyzatorDatace.UrcitObdobiVzniku(datace);
            Assert.AreEqual("1301–1350", obdobi);


        }

        [TestMethod()]
        public void AnalyzovatDataciPostAnteTest()
        {
            string popis = "post 1460, ante 1470";
            Datace datace = new Datace(popis);


            Assert.AreEqual(1460, datace.NePredRokem);
            Assert.AreEqual(1470, datace.NePoRoce);
            Assert.AreEqual("post; ante", datace.Upresneni);
            Assert.AreEqual(1400, datace.Stoleti);
            Assert.AreEqual(7, datace.Desetileti);
            Assert.AreEqual(2, datace.PolovinaStoleti);
            Assert.AreEqual(6, datace.RelativniChronologie);

            popis = "post 1416, ante 1450";
            datace = new Datace(popis);


            Assert.AreEqual(1416, datace.NePredRokem);
            Assert.AreEqual(1450, datace.NePoRoce);
            Assert.AreEqual("post; ante", datace.Upresneni);
            Assert.AreEqual(1400, datace.Stoleti);
            Assert.AreEqual(5, datace.Desetileti);
            Assert.AreEqual(1, datace.PolovinaStoleti);
            Assert.AreEqual(5, datace.RelativniChronologie);


        }

        [TestMethod()]
        public void AnalyzovatDataciPoRoceTest()
        {
            string popis = "po roce 1623";
            Datace datace = new Datace(popis);

            Assert.AreEqual(1623, datace.NePredRokem);
            Assert.AreEqual(1600, datace.Stoleti);
            Assert.AreEqual(1, datace.PolovinaStoleti);
            Assert.AreEqual(1623, datace.Rok);
        }

        [TestMethod]
        public void AnalyzovatDataciPolovinaNeboStoleti()
        {
            string popis = "2. polovina 17. století nebo začátek 18. století";
            Datace datace = new Datace(popis);
            Assert.AreEqual(1651, datace.NePredRokem);
            Assert.AreEqual(1715, datace.NePoRoce);
        }

        [TestMethod]
        public void AnalyzovatObdobiVznikuPolovinaStoleti()
        {
            string popis = "polovina 14. století";
            Datace datace = new Datace(popis);
            string obdobiVznku = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual("1351–1400", obdobiVznku);
        }

        [TestMethod]
        public void AnalyzovatObdobiVznikuKonec13Stoleti()
        {
            string popis = "konec 13. století";
            Datace datace = new Datace(popis);
            string obdobiVznku = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual("1251–1300", obdobiVznku);
        }

        [TestMethod]
        public void AnalyzovatObdobiVznikuOkoloRoku1450()
        {
            string popis = "okolo roku 1450";
            Datace datace = new Datace(popis);
            string obdobiVznku = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual("1451–1500", obdobiVznku);
        }

        [TestMethod]
        public void AnalyzovatObdobiVzniku1622a1624()
        {
            string popis = "1622 a 1624";
            Datace datace = new Datace(popis);
            string obdobiVznku = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual("1601–1650", obdobiVznku);
        }

        [TestMethod]
        public void AnalyzovatObdobi14a15Stoleti()
        {
            string popis = "14. a 15. století";
            Datace datace = new Datace(popis);
            string obdobiVznku = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual("1451–1500", obdobiVznku);
        }

        [TestMethod]
        public void AnalyzovatDataci06041821a16041821()
        {
            string popis = "9. 4. 1821 a 16. 4. 1821";
            Datace datace = new Datace(popis);
            string obdobiVznku = AnalyzatorDatace.UrcitObdobiVzniku(datace);

            Assert.AreEqual("1801–1850", obdobiVznku);
        }
    }
}
