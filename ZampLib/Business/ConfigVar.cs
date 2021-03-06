using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace ZampLib.Business
{
    public class ConfigVar
    {
        #region const
        public string procMariaDB = "mysqld";
        public string procApache = "httpd";
        #endregion

        #region prop
        public string _env { get; set; }
        public string pathBase { get; set; }
        public Dictionary<string, string> pathMariaDB { get; set; } = new Dictionary<string, string>();
        public string MariaDB_scelta { get; set; }
        public string pathApache { get; set; }
        //public string pathPHP { get; set; }
        public Dictionary<string, string> pathPHP { get; set; } = new Dictionary<string, string>();
        public string pathGit { get; set; }
        public string pathSass { get; set; }
        public string pathNode { get; set; }
        public string wp_cli { get; set; }

        public bool checkBackUpAllDBOnExit { get; set; }

        public string PHP_scelta { get; set; }
        public string default_editor_path { get; set; }
        public string apache_http_port { get; set; }
        public string apache_https_port { get; set; }
        public string mariadb_port { get; set; }
        public string pid_currentproc_apache { get; set; }
        public string pid_currentproc_mariadb { get; set; }


        public string apache_vers { get; set; }
        public string php_vers { get; set; }
        public string mariadb_vers { get; set; }
        public string composer_vers { get; set; }
        public string git_vers { get; set; }
        public string sass_vers { get; set; }
        public string node_vers { get; set; }
        public string wp_cli_vers { get; set; }


        public List<string> ListPathConsole { get; set; } //percorsi aggiuntivi che devo aggiungere alla %PATH% QUANDO apro la console

        public string MariaDB_bin
        {
            get
            {
                return System.IO.Path.Combine(MariaDB_path_scelto, "bin", procMariaDB + ".exe");
            }
        }
        public string Apache_bin
        {
            get
            {
                return System.IO.Path.Combine(pathApache, "bin", procApache + ".exe");
            }
        }
        public string PHP_bin
        {
            get
            {
                return System.IO.Path.Combine(PHP_path_scelto, "php.exe");
            }
        }
        public string Composer_bin
        {
            get
            {
                return System.IO.Path.Combine(pathBase, "Apps", "composer", "composer.bat");
            }
        }
        public string Apache_httpd_conf
        {
            get
            {
                return System.IO.Path.Combine(pathApache, "conf", "httpd.conf");
            }
        }
        public string Apache_httpd_vhosts
        {
            get
            {
                return System.IO.Path.Combine(pathApache, "conf", "extra", "httpd-vhosts.conf");
            }
        }
        //public string Apache_zampgui_conf
        //{
        //    get
        //    {
        //        return System.IO.Path.Combine(pathApache, "conf", "extra", "zampgui.conf");
        //    }
        //}
        public string phpmyadmin_config 
        { 
            get 
            {
                string htdocs = Path.Combine(pathApache, "htdocs");
                string[] subdir = (from x in Directory.GetDirectories(htdocs) select new DirectoryInfo(x).Name).ToArray();
                string pma_config = "";
                foreach(string s in subdir)
                {
                    if(s.StartsWith("pma"))
                    {
                        string pma_base = Path.Combine(htdocs, s);
                        pma_config = Path.Combine(pma_base, "config.inc.php");
                        if (!System.IO.File.Exists(pma_config))
                        {
                            pma_config = "";
                        }
                        break;
                    }
                }
                return pma_config;
            } 
        }
        public string PHP_ini
        {
            get
            {
                return System.IO.Path.Combine(PHP_path_scelto, "php.ini");
            }
        }
        public string MariaDB_ini
        {
            get
            {
                return System.IO.Path.Combine(MariaDB_path_scelto, "data", "my.ini");
            }
        }
        public string getPID_apache
        {
            get
            {
                if (!string.IsNullOrEmpty(pid_currentproc_apache))
                {
                    return pid_currentproc_apache;
                }
                else
                    return "";
            }
        }
        public string getPID_mariadb
        {
            get
            {
                if (!string.IsNullOrEmpty(pid_currentproc_mariadb))
                {
                    return pid_currentproc_mariadb;
                }
                else
                    return "";
            }
        }
        public string MariaDB_path_scelto { 
            get { return this.pathMariaDB[this.MariaDB_scelta]; } 
        }
        public string PHP_path_scelto
        {
            get
            {
                return this.pathPHP[this.PHP_scelta];
            }
        }

        public string url_phpmyadmin
        {
            get
            {
                return "http://127.0.0.1:" + apache_http_port + "/phpmyadmin";

                string htdocs = Path.Combine(pathApache, "htdocs");
                string[] subdir = (from x in Directory.GetDirectories(htdocs) select new DirectoryInfo(x).Name).ToArray();
                string pma_url = "";
                foreach (string s in subdir)
                {
                    if (s.StartsWith("pma"))
                    {
                        pma_url = "http://127.0.0.1:" + apache_http_port + "/" + s + "/index.php";
                        break;
                    }
                }
                return pma_url;
            }
        }
        public string url_adminer
        {
            get
            {
                return "http://127.0.0.1:" + apache_http_port + "/adminer";
                string htdocs = Path.Combine(pathApache, "htdocs");
                string[] name_files = (from x in Directory.GetFiles(htdocs) select new DirectoryInfo(x).Name).ToArray();
                string adminer_url = "";
                foreach (string s in name_files)
                {
                    if (s.StartsWith("adminer"))
                    {
                        adminer_url = "http://127.0.0.1:" + apache_http_port + "/" + s;
                        break;
                    }
                }
                return adminer_url;
            }
        }
        public string url_phpinfo
        {
            get
            {
                string _info_path = Path.Combine(pathApache, "htdocs", "info.php");
                string _url = "";
                if(System.IO.File.Exists(_info_path))
                {
                    _url = "http://127.0.0.1:" + apache_http_port + "/info.php";
                }
                return _url;
            }
        }
        #endregion

        public ConfigVar()
        {
            _env = ZampGUILib.getval_from_appsetting("env");
            JObject jobj = ZampGUILib.getJson_Env();
            this.pathBase = (string)jobj[_env]["pathBase"];
            this.pathApache = System.IO.Path.Combine(this.pathBase, "Apps", (string)jobj[_env]["pathApache"]);


            foreach (Newtonsoft.Json.Linq.JProperty kv in jobj[_env]["pathMariaDB"])
            {
                this.pathMariaDB.Add(kv.Name, System.IO.Path.Combine(this.pathBase, "Apps", kv.Value.ToString()));
            }
            this.MariaDB_scelta = (string)jobj[_env]["MariaDB_scelta"];

            foreach (Newtonsoft.Json.Linq.JProperty kv in jobj[_env]["pathPHP"])
            {
                this.pathPHP.Add(kv.Name, System.IO.Path.Combine(this.pathBase, "Apps", kv.Value.ToString()));
            }
            this.PHP_scelta = (string)jobj[_env]["PHP_scelta"];


            //se nel file config c'è solo il nome allora vuol dire che la directory è definita dentro zampgui/apps
            if(!string.IsNullOrEmpty((string)jobj[_env]["pathGit"]))
            {
                string temp2 = System.IO.Path.Combine(this.pathBase, "Apps", (string)jobj[_env]["pathGit"]);
                if (System.IO.Directory.Exists(temp2))
                {
                    this.pathGit = temp2;
                }
                else
                {
                    this.pathGit = (string)jobj[_env]["pathGit"];
                }
            }
            else { this.pathGit = ""; }


            //se nel file config c'è solo il nome allora vuol dire che la directory è definita dentro zampgui/apps
            if (!string.IsNullOrEmpty((string)jobj[_env]["pathSass"]))
            {
                string temp2 = System.IO.Path.Combine(this.pathBase, "Apps", (string)jobj[_env]["pathSass"]);
                if (System.IO.Directory.Exists(temp2))
                {
                    this.pathSass = temp2;
                }
                else
                {
                    this.pathSass = (string)jobj[_env]["pathSass"];
                }
            }
            else { this.pathSass = ""; }


            //se nel file config c'è solo il nome allora vuol dire che la directory è definita dentro zampgui/apps
            if (!string.IsNullOrEmpty((string)jobj[_env]["pathNode"]))
            {
                string temp2 = System.IO.Path.Combine(this.pathBase, "Apps", (string)jobj[_env]["pathNode"]);
                if (System.IO.Directory.Exists(temp2))
                {
                    this.pathNode = temp2;
                }
                else
                {
                    this.pathNode = (string)jobj[_env]["pathNode"];
                }
            }
            else { this.pathNode = ""; }


            //se nel file config c'è solo il nome allora vuol dire che la directory è definita dentro zampgui/apps
            if (!string.IsNullOrEmpty((string)jobj[_env]["wpcli"]))
            {
                string temp2 = System.IO.Path.Combine(this.pathBase, "Apps", (string)jobj[_env]["wpcli"]);
                if (System.IO.Directory.Exists(temp2))
                {
                    this.wp_cli = temp2;
                }
                else
                {
                    this.wp_cli = (string)jobj[_env]["wpcli"];
                }
            }
            else { this.wp_cli = ""; }


            if (jobj[_env]["checkBackUpAllDBOnExit"] == null || (string)jobj[_env]["checkBackUpAllDBOnExit"] == "")
                this.checkBackUpAllDBOnExit = false;
            else
                this.checkBackUpAllDBOnExit = (bool)jobj[_env]["checkBackUpAllDBOnExit"];

            this.default_editor_path = (string)jobj[_env]["default_editor_path"];
            this.apache_http_port = (string)jobj[_env]["apache_http_port"];
            this.apache_https_port = (string)jobj[_env]["apache_https_port"];
            this.mariadb_port = (string)jobj[_env]["mariadb_port"];
            this.pid_currentproc_apache = (string)jobj[_env]["pid_currentproc_apache"];
            this.pid_currentproc_mariadb = (string)jobj[_env]["pid_currentproc_mariadb"];


            string temp = (string)jobj[_env]["ListPathConsole"];
            this.ListPathConsole = new List<string>();
            if(!string.IsNullOrEmpty(temp))
            {
                this.ListPathConsole = temp.Split('|').ToList();
            }
            
        }

        public string validateSetting()
        {
            string sout = "";
            if (string.IsNullOrEmpty(MariaDB_path_scelto) || !System.IO.Directory.Exists(MariaDB_path_scelto)) 
            { 
                sout += "pathMariaDB incorrect in config.json" + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(pathApache) || !System.IO.Directory.Exists(pathApache)) 
            { 
                sout += "pathApache incorrect in config.json" + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(PHP_path_scelto) || !System.IO.Directory.Exists(PHP_path_scelto))
            { 
                sout += "pathPHP incorrect in config.json" + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(pathBase) || !System.IO.Directory.Exists(pathBase))
            { 
                sout += "pathBase incorrect in config.json" + Environment.NewLine; 
            }
            if (string.IsNullOrEmpty(default_editor_path)) 
            { 
                sout += "default_editor_path incorrect in config.json"; 
            }

            if(!int.TryParse(apache_http_port, out int na))
            {
                sout += "apache_http_port is not a number in config.json";
            }
            if (!int.TryParse(apache_https_port, out int nb))
            {
                sout += "apache_https_port is not a number in config.json";
            }
            if (!int.TryParse(mariadb_port, out int nc))
            {
                sout += "mariadb_port is not a number in config.json";
            }

            if(!string.IsNullOrEmpty(pid_currentproc_apache))
            {
                if(!int.TryParse(pid_currentproc_apache, out int nd) || !ZampGUILib.checkRunningProc(pid_currentproc_apache) || ZampGUILib.getNameProc_fromPID(pid_currentproc_apache) != this.procApache)
                {
                    updatePID(typeProg.apache, typeStartorKill.kill, Convert.ToInt32(pid_currentproc_apache));
                }
            }
            if (!string.IsNullOrEmpty(pid_currentproc_mariadb))
            {
                if (!int.TryParse(pid_currentproc_mariadb, out int ne) || !ZampGUILib.checkRunningProc(pid_currentproc_mariadb) || ZampGUILib.getNameProc_fromPID(pid_currentproc_mariadb) != this.procMariaDB)
                {
                    updatePID(typeProg.mariadb, typeStartorKill.kill, Convert.ToInt32(pid_currentproc_mariadb));
                }
            }


            
            return sout;
        }

        public void otherUpdate()
        {
            JObject jobj = ZampGUILib.getJson_Env();
            jobj[_env]["checkBackUpAllDBOnExit"] = checkBackUpAllDBOnExit;
            ZampGUILib.setJson_Env(jobj);
        }
        public void updateDefaultEditor(string default_editor_path)
        {
            JObject jobj = ZampGUILib.getJson_Env();
            jobj[_env]["default_editor_path"] = default_editor_path;
            ZampGUILib.setJson_Env(jobj);

            /*
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;

            this.default_editor_path = default_editor_path;

            app.Settings.Remove("default_editor_path");
            app.Settings.Add("default_editor_path", default_editor_path);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            */
        }

        public void updateAdditionalPath()
        {
            JObject jobj = ZampGUILib.getJson_Env();
            jobj[_env]["ListPathConsole"] = String.Join("|", this.ListPathConsole.ToArray());

            jobj[_env]["pathGit"] = pathGit.StartsWith(System.IO.Path.Combine(pathBase, "Apps"))? pathGit.Remove(0, System.IO.Path.Combine(pathBase, "Apps").Length).Trim('\\'): pathGit;
            jobj[_env]["pathSass"] = pathSass.StartsWith(System.IO.Path.Combine(pathBase, "Apps")) ? pathSass.Remove(0, System.IO.Path.Combine(pathBase, "Apps").Length).Trim('\\') : pathSass;
            jobj[_env]["pathNode"] = pathNode.StartsWith(System.IO.Path.Combine(pathBase, "Apps")) ? pathNode.Remove(0, System.IO.Path.Combine(pathBase, "Apps").Length).Trim('\\') : pathNode;

            wp_cli_vers = refresh_wpcli_vers();
            sass_vers = refresh_sass_vers();
            git_vers = refresh_git_vers();
            node_vers = refresh_node_vers();

            ZampGUILib.setJson_Env(jobj);
        }

        public void updatePort()
        {
            JObject jobj = ZampGUILib.getJson_Env();
            jobj[_env]["apache_http_port"] = apache_http_port;
            jobj[_env]["apache_https_port"] = apache_https_port;
            jobj[_env]["mariadb_port"] = mariadb_port;
            ZampGUILib.setJson_Env(jobj);
            change_port();
        }

        public void updatePID(typeProg type_program, typeStartorKill type_op, int? pid)
        {
            JObject jobj = ZampGUILib.getJson_Env();
            switch (type_program)
            {
                case typeProg.apache:
                    if(type_op == typeStartorKill.kill)
                    {
                        pid_currentproc_apache = "";
                    }
                    else
                    {
                        pid_currentproc_apache = pid.ToString();
                    }
                    jobj[_env]["pid_currentproc_apache"] = getPID_apache;
                    break;
                case typeProg.mariadb:
                    if (type_op == typeStartorKill.kill)
                    {
                        pid_currentproc_mariadb = "";
                    }
                    else
                    {
                        pid_currentproc_mariadb = pid.ToString();
                    }
                    jobj[_env]["pid_currentproc_mariadb"] = getPID_mariadb;
                    break;
            }
            ZampGUILib.setJson_Env(jobj);
        }

        public void updatePath(string abs_main_path)
        {
            JObject jobj = ZampGUILib.getJson_Env();

            //pathMariaDB = ManLib.ManZampLib.get_first_dir(abs_main_path, "mariadb");
            //pathPHP = ManLib.ManZampLib.get_first_dir(abs_main_path, "php");
            //string _pathMariaDB = System.IO.Path.Combine(abs_main_path, "Apps", this.MariaDB_scelta);
            //string _pathPHP = System.IO.Path.Combine(abs_main_path, "Apps", this.PHP_scelta);
            pathApache = ZampLib.ZampGUILib.get_first_dir(abs_main_path, "Apache");
            

            jobj[_env]["pathBase"] = abs_main_path;
            jobj[_env]["pathApache"] = pathApache;
            jobj[_env]["MariaDB_scelta"] = this.MariaDB_scelta;
            jobj[_env]["PHP_scelta"] = this.PHP_scelta;

            foreach(var kv in pathMariaDB)
            {
                jobj[_env]["pathMariaDB"][kv.Key] = System.IO.Path.Combine(abs_main_path, "Apps", kv.Key);
            }

            foreach (var kv in pathMariaDB.Keys.ToList())
            {
                pathMariaDB[kv] = System.IO.Path.Combine(abs_main_path, "Apps", kv);
            }

            foreach (var kv in pathPHP)
            {
                jobj[_env]["pathPHP"][kv.Key] = System.IO.Path.Combine(abs_main_path, "Apps", kv.Key);
            }
            foreach (var kv in pathPHP.Keys.ToList())
            {
                pathPHP[kv] = System.IO.Path.Combine(abs_main_path, "Apps" ,kv);
            }


            ZampGUILib.setJson_Env(jobj);

            change_path(abs_main_path);

            pathBase = abs_main_path;


            /*
            JObject jobj = ManZampLib.getJson_Env();
            
            pathMariaDB = pathMariaDB.ToLower().Replace(pathBase, abs_main_path);
            pathApache = pathApache.ToLower().Replace(pathBase, abs_main_path);
            pathPHP = pathPHP.ToLower().Replace(pathBase, abs_main_path);

            jobj[_env]["pathBase"] = abs_main_path;
            jobj[_env]["pathMariaDB"] = pathMariaDB;
            jobj[_env]["pathApache"] = pathApache;
            jobj[_env]["pathPHP"] = pathPHP;
            
            ManZampLib.setJson_Env(jobj);

            change_path(pathBase, abs_main_path);

            pathBase = abs_main_path;
            */
        }
        public void get_software_version()
        {
            Regex regex;
            Match match;


            apache_vers = ZampGUILib.startProc_and_wait_output(Apache_bin, "-v", true);
            regex = new Regex(@"Apache.\d+\.\d+\.\d+");
            match = regex.Match(apache_vers);
            if (match.Success)
            {
                apache_vers = match.Value.Replace("Apache/","");
            }


            php_vers = ZampGUILib.startProc_and_wait_output(PHP_bin, "-v", true);
            regex = new Regex(@"PHP \d+\.\d+.\d+");
            match = regex.Match(php_vers);
            if (match.Success)
            {
                php_vers = match.Value.Replace("PHP ","");
            }

            mariadb_vers = ZampGUILib.startProc_and_wait_output(MariaDB_bin, "--version", true);
            regex = new Regex(@"Ver \d+\.\d+\.\d+");
            match = regex.Match(mariadb_vers);
            if (match.Success)
            {
                mariadb_vers = match.Value.Replace("Ver","").Trim();
            }


            composer_vers = ZampGUILib.startProc_and_wait_output(Composer_bin, "--version", true, PHP_path_scelto);
            regex = new Regex(@"Composer version \d+\.\d+\.\d+");
            match = regex.Match(composer_vers);
            if (match.Success)
            {
                composer_vers = match.Value.Replace("Composer version", "").Trim();
            }

            
            git_vers = refresh_git_vers();
            node_vers = refresh_node_vers();
            sass_vers = refresh_sass_vers();
            wp_cli_vers = refresh_wpcli_vers();
            

        }

        #region private
        private void change_path(string newpath)
        {
            //string[] arrfiles = { Apache_httpd_conf, PHP_ini, MariaDB_ini, Path.Combine(newpath, "scripts", "start_all.vbs") };
            List<string> arrfiles = new List<string>();
            arrfiles.Add(Apache_httpd_conf);
            //arrfiles.Add(Apache_zampgui_conf);
            arrfiles.Add(Path.Combine(newpath, "scripts", "start_all.vbs"));
            foreach (var kv in pathMariaDB)
            {
                arrfiles.Add(System.IO.Path.Combine(kv.Value, "data", "my.ini"));
            }
            foreach (var kv in pathPHP)
            {
                arrfiles.Add(System.IO.Path.Combine(kv.Value, "php.ini"));
            }



            foreach (var f in arrfiles)
            {
                if(!System.IO.File.Exists(f))
                {
                    ZampGUILib.printMsg_and_exit("file " + f + " not found" , true);
                    continue;
                }
                string text = System.IO.File.ReadAllText(f);
                string file_name = Path.GetFileName(f);
                string dirName = "";

                switch (file_name)
                {
                    case "httpd.conf":
                        // Use Regex.Replace to replace the pattern in the input.
                        // ... The pattern N.t indicates three letters.
                        // ... N, any character, and t.
                        text = Regex.Replace(text, @"^Define ZAMPROOT.*", "Define ZAMPROOT \"" + newpath.Replace("\\", "/") + "\"", RegexOptions.Multiline);
                        break;
                    case "php.ini":
                        //string name_xdebug_file = ManLib.ManZampLib.get_first_file(Path.Combine(PHP_path_scelto, "ext"), "xdebug");
                        //text = Regex.Replace(text, @"^extension_dir.*", "extension_dir = \"" + Path.Combine(PHP_path_scelto, "ext") + "\"", RegexOptions.Multiline);
                        //if(!string.IsNullOrEmpty(name_xdebug_file))
                        //{
                        //    text = Regex.Replace(text, @"^zend_extension.*", "zend_extension = \"" + name_xdebug_file + "\"", RegexOptions.Multiline);
                        //}
                        dirName = new DirectoryInfo(f).Parent.FullName;
                        string name_xdebug_file = ZampLib.ZampGUILib.get_first_file(Path.Combine(dirName, "ext"), "xdebug");
                        text = Regex.Replace(text, @"^extension_dir.*", "extension_dir = \"" + Path.Combine(dirName, "ext") + "\"", RegexOptions.Multiline);
                        if(!string.IsNullOrEmpty(name_xdebug_file))
                        {
                            text = Regex.Replace(text, @"^zend_extension.*", "zend_extension = \"" + name_xdebug_file + "\"", RegexOptions.Multiline);
                        }
                        break;
                    case "my.ini":
                        dirName = new DirectoryInfo(f).Parent.Parent.FullName;
                        //text = Regex.Replace(text, @"^datadir.*", "datadir=" + Path.Combine(MariaDB_path_scelto, "data").Replace("\\", "/"), RegexOptions.Multiline);
                        //text = Regex.Replace(text, @"^plugin-dir=.*", "plugin-dir=" + Path.Combine(MariaDB_path_scelto, "lib", "plugin").Replace("\\", "/"), RegexOptions.Multiline);
                        text = Regex.Replace(text, @"^datadir.*", "datadir=" + Path.Combine(dirName, "data").Replace("\\", "/"), RegexOptions.Multiline);
                        text = Regex.Replace(text, @"^plugin-dir=.*", "plugin-dir=" + Path.Combine(dirName, "lib", "plugin").Replace("\\", "/"), RegexOptions.Multiline);
                        break;
                    case "start_all.vbs":
                        //
                        string rep1 = "WinScriptHost.Run Chr(34) & \"" + Apache_bin + "\" & Chr(34), 0" + Environment.NewLine;
                        string rep2 = "WinScriptHost.Run Chr(34) & \"" + MariaDB_bin + "\" & Chr(34), 0" + Environment.NewLine;
                        //text = Regex.Replace(text, @"^WinScriptHost.Run.*", rep, RegexOptions.Multiline);

                        text = Regex.Replace(text, @"^WinScriptHost.Run.*httpd.exe.*", rep1, RegexOptions.Multiline);
                        text = Regex.Replace(text, @"^WinScriptHost.Run.*mysqld.exe.*", rep2, RegexOptions.Multiline);
                        break;
                }

                /*
                if (f.EndsWith("conf") || f.EndsWith("my.ini"))
                {
                    text = ManZampLib.replace_ignorecase(text, oldpath.Replace("\\", "/"), newpath.Replace("\\", "/"));
                }
                else
                {
                    text = ManZampLib.replace_ignorecase(text, oldpath.Replace("\\", "\\\\"), newpath);
                }
                */
                System.IO.File.WriteAllText(f, text);
            }
        }
        private void change_port()
        {
            string[] arrfiles = { Apache_httpd_conf, MariaDB_ini };

            foreach (var f in arrfiles)
            {
                if (!System.IO.File.Exists(f))
                {
                    ZampGUILib.printMsg_and_exit("file " + f + " not found", true);
                    continue;
                }

                string text = System.IO.File.ReadAllText(f);
                if (f.Equals(Apache_httpd_conf))
                {
                    var regex = new Regex(@"^\s*Define HTTP_PORT.*$", RegexOptions.Multiline);
                    text = regex.Replace(text, "Define HTTP_PORT  \"" + apache_http_port + "\"");

                    regex = new Regex(@"^\s*Define HTTPS_PORT.*$", RegexOptions.Multiline);
                    text = regex.Replace(text, "Define HTTPS_PORT  \"" + apache_https_port + "\"");
                }
                if (f.Equals(MariaDB_ini))
                {
                    var regex = new Regex(@"^\s*port.*$", RegexOptions.Multiline);
                    text = regex.Replace(text, "port=" + mariadb_port);
                }

                System.IO.File.WriteAllText(f, text);
            }

            //trying to change phpmyadmin config
            string phpmyadmin_config = this.phpmyadmin_config;
            if (File.Exists(phpmyadmin_config))
            {
                string text = System.IO.File.ReadAllText(phpmyadmin_config);
                var regex = new Regex(@"^\s*\$cfg\['Servers'\]\[\$i\]\['port'\].*$", RegexOptions.Multiline);
                text = regex.Replace(text, "$cfg['Servers'][$i]['port'] = '" + mariadb_port + "';");

                regex = new Regex(@"^\s*\$cfg\['Servers'\]\[\$i\]\['controlport'\].*$", RegexOptions.Multiline);
                text = regex.Replace(text, "$cfg['Servers'][$i]['controlport'] = '" + mariadb_port + "';");
                System.IO.File.WriteAllText(phpmyadmin_config, text);
            }

        }

        public string get_friendly_name(typeProg tpg)
        {
            string friendly_name = "";
            switch (tpg)
            {
                case typeProg.mariadb:
                    friendly_name = "mariadb";
                    break;
                case typeProg.apache:
                    friendly_name = "apache";
                    break;
                case typeProg.editor:
                    friendly_name = "default editor";
                    break;
            }
            return friendly_name;
        }
        public string get_correct_pid(typeProg tpg)
        {
            string pid = "";
            switch (tpg)
            {
                case typeProg.mariadb:
                    pid = getPID_mariadb;
                    break;
                case typeProg.apache:
                    pid = getPID_apache;
                    break;
            }
            return pid;
        }
        public string get_correct_path_prog(typeProg tpg)
        {
            string path = "";
            switch (tpg)
            {
                case typeProg.mariadb:
                    path = MariaDB_bin;
                    break;
                case typeProg.apache:
                    path = Apache_bin;
                    break;
                case typeProg.editor:
                    path = default_editor_path;
                    break;
            }
            return path;
        }

        private string refresh_wpcli_vers()
        {
            string str = "";
            Regex regex;
            Match match;

            if (System.IO.Directory.Exists(wp_cli))
            {
                str = ZampGUILib.startProc_and_wait_output(System.IO.Path.Combine(wp_cli, "wp.bat"), " cli info", true, PHP_path_scelto);
                regex = new Regex(@"WP-CLI version:.\d+\.\d+\.\d+");
                match = regex.Match(str);
                if (match.Success)
                {
                    str = match.Value.Replace("WP-CLI version", "").Trim().Trim(':');
                }
            }
            return str;
        }
        private string refresh_sass_vers()
        {
            string str = "";
            Regex regex;
            Match match;

            if (System.IO.Directory.Exists(pathSass))
            {
                str = ZampGUILib.startProc_and_wait_output(System.IO.Path.Combine(pathSass, "sass.bat"), "--version", true, pathSass);
                //regex = new Regex(@"Composer version \d+\.\d+\.\d+");
                //match = regex.Match(composer_vers);
                //if (match.Success)
                //{
                //    composer_vers = match.Value;
                //}
            }
            return str;
        }

        private string refresh_node_vers()
        {
            string str = "";
            Regex regex;
            Match match;
            if (System.IO.Directory.Exists(pathNode))
            {
                str = ZampGUILib.startProc_and_wait_output(System.IO.Path.Combine(pathNode, "node.exe"), "--version", true, pathNode);
                //regex = new Regex(@"Composer version \d+\.\d+\.\d+");
                //match = regex.Match(composer_vers);
                //if (match.Success)
                //{
                //    composer_vers = match.Value;
                //}
            }
            return str;
        }
        private string refresh_git_vers()
        {
            string str = "";
            Regex regex;
            Match match;

            if (System.IO.Directory.Exists(pathGit))
            {
                str = ZampGUILib.startProc_and_wait_output(System.IO.Path.Combine(pathGit, "git.exe"), "--version", true, pathGit);
                str = str.Replace("git version ", "").Trim();
                //regex = new Regex(@"Composer version \d+\.\d+\.\d+");
                //match = regex.Match(composer_vers);
                //if (match.Success)
                //{
                //    composer_vers = match.Value;
                //}
            }

            return str;
        }
        #endregion
    }
}

