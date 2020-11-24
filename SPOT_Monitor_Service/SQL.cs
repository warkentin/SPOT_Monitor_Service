using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SPOT_Monitor_Service
{
    class SQL
    {
        public SQL()
        {
            //SQL-Verbindung aufbauen
            string servername = System.Net.Dns.GetHostName();
            _myConnection = new SqlConnection("user id=sa; password=;server=" + servername + "\\SQLEXPRESS;" +
                                              "Trusted_Connection=yes; database=SFC-Polipol; connection timeout=30");

            _myCon_central = new SqlConnection("user id=sa; password=;server=dpn-svr-membrain\\SQLEXPRESS;" +
                                              "Trusted_Connection=yes; database=SPOT; connection timeout=30");
        }

        public SqlConnection _myConnection;
        public SqlConnection _myCon_central;

        public SqlConnection myConnection
        {
            get { return _myConnection; }
            set { _myConnection = value; }
        }

        public SqlConnection myCon_central
        {
            get { return _myCon_central; }
            set { _myCon_central = value; }
        }

        public string loggingToDB(string server, string offene_Messages, string Aktive_Personen, string Messages_Gesamt)
        {
            DateTime loc_date = DateTime.Now.ToUniversalTime();
            string local_date =loc_date.Month + "/" + loc_date.Day + "/" + loc_date.Year + " " + loc_date.Hour + ":" + loc_date.Minute + ":" + loc_date.Second;
            string abfrage = "Insert into tbl_log values ('" + server + "', '" + local_date + "', '" + offene_Messages + "', '" + Aktive_Personen + "', '" + Messages_Gesamt + "');";

            try
            {
                _myCon_central.Open();

                SqlCommand cmd = new SqlCommand(abfrage, _myCon_central);
                cmd.ExecuteNonQuery();
                _myCon_central.Close();
                return abfrage;
            }
            catch (Exception err)
            {
                return abfrage + err.Message;
            }
        }

        public string get_Anzahl_Personen_aktiv()
        {
            // SQL-Datenvariable erstellen
            SqlDataReader rdr = null;
            string anzahl = "0";
            try
            {
                _myConnection.Open();

                // SQL-Abfrage starten
                string abfrage = "select count(*) from employee where isloggedin = 1";

                SqlCommand cmd = new SqlCommand(abfrage, myConnection);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                anzahl = Convert.ToString((int)rdr[0]);
                rdr.Close();
                _myConnection.Close();
            }
            catch (Exception err)
            {                
                anzahl = "--";
            }
            return anzahl;
        }       

        public string get_Backuppfad(string alias)
        {
            // SQL-Datenvariable erstellen
            SqlDataReader rdr = null;
            string pfad = "";
            try
            {
                _myConnection.Open();

                // SQL-Abfrage starten
                string abfrage = "select Backuppfad from Werke where Alias = '" + alias.ToUpper() + "'";

                SqlCommand cmd = new SqlCommand(abfrage, myConnection);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                pfad = (string)rdr[0];
                rdr.Close();
                _myConnection.Close();
            }
            catch (Exception err)
            {                
            }
            return pfad;
        }        

        public string get_anzahl_messages_today()
        {
            // SQL-Datenvariable erstellen
            SqlDataReader rdr = null;
            string anzahl = "0";
            try
            {
                _myConnection.Open();

                // SQL-Abfrage starten
                string local_date = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                string local_date_start = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + " 00:00:00";
                string abfrage = "select COUNT(*) from Message where TimeManual between '" + local_date_start + "' and '" + local_date + "';";
                SqlCommand cmd = new SqlCommand(abfrage, myConnection);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                anzahl = Convert.ToString((int)rdr[0]);
                rdr.Close();
                _myConnection.Close();
            }
            catch (Exception err)
            {
                //MessageBox.Show(err.ToString());
                anzahl = "--";
            }
            return anzahl;
        }

        public string get_anzahl_messages_offen()
        {
            // SQL-Datenvariable erstellen
            SqlDataReader rdr = null;
            string anzahl = "0";
            try
            {
                _myConnection.Open();

                // SQL-Abfrage starten
                string abfrage = "select count(*) from Message where isprocessed = 0;";
                SqlCommand cmd = new SqlCommand(abfrage, myConnection);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                anzahl = Convert.ToString((int)rdr[0]);
                rdr.Close();
                _myConnection.Close();
            }
            catch (Exception err)
            {                
                anzahl = "--";
            }
            return anzahl;
        }

        public int get_anzahl_durchschnitt(int werk)
        {
            // SQL-Datenvariable erstellen
            SqlDataReader rdr = null;
            int anzahl = 0;
            try
            {
                _myConnection.Open();

                // SQL-Abfrage starten
                string abfrage = "select Messages from Werke where ID = " + werk + ";";
                SqlCommand cmd = new SqlCommand(abfrage, myConnection);
                rdr = cmd.ExecuteReader();
                rdr.Read();
                anzahl = (int)rdr[0];
                rdr.Close();
                _myConnection.Close();
            }
            catch (Exception err)
            {
                //MessageBox.Show(err.ToString());
                anzahl = 0;
            }
            return anzahl;
        }
    }
}
