using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ZampLib.Business;
using ZampLib;
using System.Net;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ZampGUI
{
    public partial class FormOneClick_WP : Form
    {
        #region vars
        public ConfigVar cv;
        public string temp_folder;
        public string wp_latest_zip;

        BackgroundWorker backgroundWorker2;
        #endregion

        #region constructor
        public FormOneClick_WP(ConfigVar cv)
        {
            InitializeComponent();
            this.cv = cv;

            temp_folder = Path.Combine(cv.pathBase, "temp");

            //using (WebClient wc = new WebClient())
            //{
            //    var str = wc.DownloadString("https://api.wordpress.org/core/version-check/1.7/");
            //    JObject json = JObject.Parse(str);

            //    string download_link = json["offers"][0]["download"].ToString();
            //    Uri uri = new Uri(download_link);
            //    string filename = System.IO.Path.GetFileName(uri.LocalPath);
            //    wp_latest_zip = Path.Combine(temp_folder, filename);
            //}

            backgroundWorker2 = new BackgroundWorker();
        }
        #endregion

        #region eventi form
        private void btnInstall_Click(object sender, EventArgs e)
        {
            string nome_sito = txt_url.Text.Trim();
            string dbname = txt_dbname.Text.Trim();
            string dbuser = txt_dbuser.Text.Trim();
            string dbpass = txt_dbpassword.Text.Trim();

            string sitetitle = txt_sitetitle.Text.Trim();
            string siteuser = txt_siteuser.Text.Trim();
            string sitepassword = txt_sitepassword.Text.Trim();
            string siteemail = txt_siteemail.Text.Trim();


            string wpfolder = Path.Combine(cv.pathBase, "sites", "wp", nome_sito);

            if (string.IsNullOrEmpty(nome_sito)
               || string.IsNullOrEmpty(dbname)
               || string.IsNullOrEmpty(dbuser)
               || string.IsNullOrEmpty(dbpass)
               || string.IsNullOrEmpty(sitetitle)
               || string.IsNullOrEmpty(siteuser)
               || string.IsNullOrEmpty(sitepassword)
               || string.IsNullOrEmpty(siteemail))
            {
                MessageBox.Show("Please complete every field in the form");
                return;
            }



            if (Directory.Exists(wpfolder))
            {
                //MessageBox.Show("error: sites/wp folder contains \"" + nome_sito + "\" directory");
                //return;
            }
            else
            {
                Directory.CreateDirectory(wpfolder);
            }
            if (dbname.Length < 2)
            {
                MessageBox.Show("db name too short");
                return;
            }

            List<string> all_db = ZampGUILib.getAllDB(cv.mariadb_port);
            foreach (string s in all_db)
            {
                if (s.Equals(dbname))
                {
                    //MessageBox.Show("database \"" + s + "\" already exists");
                    //return;
                }
            }


            btnInstall.Enabled = false;
            //txt_dbname.Enabled = false;
            //txt_url.Enabled = false;
            //comboBox_protocol.Enabled = false;

            ProcessStartInfo pro = new ProcessStartInfo();
            pro.FileName = System.IO.Path.Combine(cv.pathBase, "scripts", "wpcli_create.bat");
            pro.UseShellExecute = false;
            pro.WorkingDirectory = wpfolder;

            string args = "\"" + dbname + "\" \"" + dbuser + "\" \"" + dbpass + "\" \"" + nome_sito + "\" \"" + sitetitle + "\" \"" + siteuser + "\" \"" + sitepassword + "\" \"" + siteemail + "\"";
            pro.Arguments =  args;
            Process proStart = new Process();
            proStart.StartInfo = pro;

            string addToPath = "\"" + cv.Apache_bin + "\";\"" + cv.PHP_path_scelto + "\";\"" + cv.MariaDB_bin + "\";\"" + cv.wp_cli + "\";\"" + System.IO.Path.Combine(cv.pathBase, "scripts") + "\"";
            string PATH = Environment.GetEnvironmentVariable("PATH");
            pro.EnvironmentVariables["PATH"] = addToPath + ";" + PATH;
            proStart.Start();



            //ZampGUILib.ExecuteBatchFile_dont_wait(System.IO.Path.Combine(cv.pathBase, "scripts", "open_console.bat"),
            //        new string[] { apache_dir_bin, cv.PHP_path_scelto, mariadb_dir_bin, composer_path, drive_letter, cv.pathBase, ListPathConsole }
            //);

        }


        #endregion


        #region metodi privati
        //private void bEnable_btn_install()
        //{
        //    bool b_enable_btn = true;
        //    label7.Text = "";
        //    if (string.IsNullOrEmpty(txt_url.Text))
        //    {
        //        label7.Text += "please insert web url" + Environment.NewLine;
        //        b_enable_btn = false;
        //    }
        //    if (string.IsNullOrEmpty(txt_dbname.Text))
        //    {
        //        label7.Text += "please insert database name" + Environment.NewLine;
        //        b_enable_btn = false;
        //    }
        //    //if (comboBox_protocol.SelectedItem == null || string.IsNullOrEmpty(comboBox_protocol.SelectedItem.ToString()))
        //    //{
        //    //    label7.Text += "Please select http or https" + Environment.NewLine;
        //    //    b_enable_btn = false;
        //    //}


        //    btnInstall.Enabled = b_enable_btn;
        //}
        //private void reset_progress_step2()
        //{
        //    progressBar2.Minimum = 0;
        //    progressBar2.Maximum = 100;
        //    progressBar2.Value = 0;
        //    label8.Text = "";
        //}
        //private void reset_progress_step1()
        //{
        //    progressBar1.Minimum = 0;
        //    progressBar1.Value = 0;
        //    label8.Text = "";
        //}
        //private void step2()
        //{
        //    progressBar1.PerformStep();
        //    reset_progress_step2();

        //    //*************************************************************************************
        //    //step 2 - unzip wordpress and create site folder
        //    addOutput("Unzipping wordpress");
        //    label8.Text = "Unzipping wordpress";



        //    backgroundWorker2.DoWork += backgroundWorker2_DoWork;
        //    backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
        //    backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;  //Tell the user how the process went
        //    backgroundWorker2.WorkerReportsProgress = true;
        //    backgroundWorker2.WorkerSupportsCancellation = false; //Allow for the process to be cancelled
        //    backgroundWorker2.RunWorkerAsync();

        //}
        //private void step3()
        //{
        //    progressBar1.PerformStep();
        //    reset_progress_step2();



        //    //*************************************************************************************
        //    //step 3 - create site folder e change setting 
        //    addOutput("Moving folder - and change config site");
        //    label8.Text = "Moving folder - and change config site";
        //    string dir_wp = Path.Combine(cv.pathBase, "temp", "wordpress");
        //    string new_site = Path.Combine(cv.pathBase, "sites", txt_url.Text);
        //    string wp_conf_sample = Path.Combine(new_site, "wp-config-sample.php");
        //    string wp_conf = Path.Combine(new_site, "wp-config.php");

        //    Directory.Move(dir_wp, new_site);
        //    File.Copy(wp_conf_sample, wp_conf);


        //    string all_text = System.IO.File.ReadAllText(wp_conf);
        //    all_text = Regex.Replace(all_text, @"database_name_here", txt_dbname.Text);
        //    all_text = Regex.Replace(all_text, @"username_here", "root");
        //    all_text = Regex.Replace(all_text, @"password_here", "root");
        //    all_text = Regex.Replace(all_text, @"localhost", "127.0.0.1:" + cv.mariadb_port);


        //    // prendo i dati casuali da web
        //    List<string> list_rand = new List<string>();
        //    using (var client = new WebClient())
        //    {
        //        string result = client.DownloadString("https://api.wordpress.org/secret-key/1.1/salt/");
        //        //addOutput(Environment.NewLine + result + Environment.NewLine);
        //        list_rand = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

        //    }


        //    string[] arr_key = new string[] { "AUTH_KEY", "SECURE_AUTH_KEY", "LOGGED_IN_KEY", "NONCE_KEY", "AUTH_SALT", "SECURE_AUTH_SALT", "LOGGED_IN_SALT", "NONCE_SALT" };
        //    foreach(string key in arr_key)
        //    {
        //        all_text = Regex.Replace(all_text, @"define\(\s'" + key + @"',\s*'put your unique phrase here'\s\);", get_code_fromlist(list_rand, key), RegexOptions.Multiline);
        //    }

        //    System.IO.File.WriteAllText(wp_conf, all_text);
        //    step4();
        //}

        //private void step4()
        //{
        //    progressBar1.PerformStep();
        //    reset_progress_step2();

        //    //*************************************************************************************
        //    //step 4 - db
        //    addOutput("Creating database ..");
        //    label8.Text = "Creating database";

        //    string bin_mysql = Path.Combine(cv.MariaDB_path_scelto, "bin", "mysql.exe"); 

        //    string sout = ZampGUILib.startProc_and_wait_output(bin_mysql, "-u root --password=root -h localhost --port=" + cv.mariadb_port + " -e \""
        //        + "CREATE DATABASE " + txt_dbname.Text + " CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
        //        + "\"");

        //    step5();
        //}

        //private void step5()
        //{
        //    progressBar1.PerformStep();
        //    reset_progress_step2();

        //    //*************************************************************************************
        //    //step 5 - unzip wordpress and create site folder
        //    addOutput("changing virtualhostm");
        //    label8.Text = "Final Step";
        //    string path_vhost = Path.Combine(cv.pathApache, "conf", "extra", "httpd-vhosts.conf");


        //    //gestione virtualhost
        //    string scontent_virtualhost = File.ReadAllText(path_vhost);
        //    if(scontent_virtualhost.Contains(txt_url.Text))
        //    {
        //        MessageBox.Show("httpd-vhosts.conf contains a definitio for \"" + txt_url.Text + "\", adjust the file with proper setting");
        //    }
        //    else
        //    {
        //        string s_host_template = "";
        //        //if (comboBox_protocol.SelectedItem.ToString().ToLower().Equals("http"))
        //        //{
        //        //    s_host_template = Path.Combine(cv.pathBase, "scripts", "template_http.txt");
        //        //}
        //        //else
        //        //{
        //        //    s_host_template = Path.Combine(cv.pathBase, "scripts", "template_https.txt");
        //        //}


        //        string all_text = File.ReadAllText(s_host_template);
        //        all_text = Regex.Replace(all_text, @"_email_admin", "info@admin.com");
        //        all_text = Regex.Replace(all_text, @"_dir_url_", txt_url.Text, RegexOptions.Multiline);

        //        File.AppendAllText(path_vhost, Environment.NewLine + Environment.NewLine + all_text);
        //    }


        //    //gestione host
        //    //string path_host_file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
        //    //string contents = System.IO.File.ReadAllText(path_host_file);




        //    //riavvio processi
        //    addOutput(ZampGUILib.killproc(cv, typeProg.apache));
        //    addOutput(ZampGUILib.killproc(cv, typeProg.mariadb));

        //    System.Threading.Thread.Sleep(1000);

        //    addOutput(ZampGUILib.startProc(cv, typeProg.apache, new string[] { }));
        //    addOutput(ZampGUILib.startProc(cv, typeProg.mariadb, new string[] { }));

        //    //messaggio con link
        //    progressBar1.PerformStep();
        //    btnInstall.Enabled = true;
        //    txt_dbname.Enabled = true;
        //    txt_url.Enabled = true;
        //    //comboBox_protocol.Enabled = true;


        //    if (MessageBox.Show("Process completed - add \"" + txt_url.Text + "\" to host file and then reload your browser. Click yes to open your site", "Visit", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
        //    {
        //        //string _url = comboBox_protocol.SelectedItem.ToString().ToLower() + "://" + txt_url.Text;
        //        //System.Diagnostics.Process.Start(_url);
        //    }

        //    reset_progress_step2();
        //    reset_progress_step1();

        //    this.Close();

        //}


        //private string get_code_fromlist(List<string> list_rand, string key)
        //{
        //    foreach(string s in list_rand)
        //    {
        //        if (s.Contains(key))
        //            return s.Replace("$", ",");
        //    }
        //    return "";
        //}
        //private void addOutput(string testo)
        //{
        //    string[] lines = Regex.Split(testo, Environment.NewLine);
        //    foreach (string s in lines)
        //    {
        //        if (!string.IsNullOrEmpty(s))
        //        {
        //            string datamia = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            txtOut.AppendText(datamia + " - " + s + Environment.NewLine);
        //        }

        //    }
        //}
        #endregion



        #region evventi download zip
        //private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    //MessageBox.Show("completato");
        //    step2();
        //}
        //void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    progressBar2.Value = e.ProgressPercentage;
        //}

        //#endregion

        //#region eventi unzip
        //private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        //{
        //    string extractPath = Path.Combine(cv.pathBase, "temp");

        //    if (Directory.Exists(Path.Combine(cv.pathBase, "temp", "wordpress")))
        //    {
        //        Directory.Delete(Path.Combine(cv.pathBase, "temp", "wordpress"), true);
        //    }


        //    using (ZipArchive archive = ZipFile.OpenRead(wp_latest_zip))
        //    {
        //        int num_files = archive.Entries.Count;
        //        int step = num_files / 100;
        //        int count = 0;
        //        int progress_count = 0;

        //        foreach (ZipArchiveEntry entry in archive.Entries)
        //        {
        //            string fileDestinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

        //            if (Path.GetFileName(fileDestinationPath).Length == 0)
        //            {
        //                // If it is a directory:
        //                if (entry.Length != 0)
        //                    throw new IOException("Directory entry with data.");

        //                Directory.CreateDirectory(fileDestinationPath);
        //            }
        //            else
        //            {
        //                // If it is a file:
        //                // Create containing directory:
        //                Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath));
        //                entry.ExtractToFile(fileDestinationPath);
        //            }



        //            if (count > step)
        //            {
        //                count = 0;
        //                progress_count++;
        //                backgroundWorker2.ReportProgress(progress_count);
        //            }
        //            else
        //            {
        //                count++;
        //            }
        //        }
        //    }
        //    backgroundWorker2.ReportProgress(100);
        //}

        //private void backgroundWorker2_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        //{
        //    progressBar2.Value = e.ProgressPercentage;
        //}

        //private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //        //lblStatus.Text = "Process was cancelled";
        //    }
        //    else if (e.Error != null)
        //    {
        //        //lblStatus.Text = "There was an error running the process. The thread aborted";
        //    }
        //    else
        //    {
        //        //lblStatus.Text = "Process was completed";
        //        step3();
        //    }
        //}
        #endregion

    }
}
