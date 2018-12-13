using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Edward;

namespace RenameFileForSX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 
        /// </summary>
        public class DirectoryAllFiles
        {
            static List<FileInformation> FileList = new List<FileInformation>();
            public static List<FileInformation> GetAllFiles(DirectoryInfo dir)
            {
                FileInfo[] allFile = dir.GetFiles();
                foreach (FileInfo fi in allFile)
                {
                    FileList.Add(new FileInformation { FileName = fi.Name, FilePath = fi.FullName, FileDirectory = fi.DirectoryName, FileExtension = fi.Extension });
                }
                DirectoryInfo[] allDir = dir.GetDirectories();
                foreach (DirectoryInfo d in allDir)
                {
                    GetAllFiles(d);
                }
                return FileList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FileInformation
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string FileDirectory { get; set; }
            public string FileExtension { get; set; }
        }






        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "批量修改文件名For山西改造,Ver:" + Application.ProductVersion;

            txtFolder.SetWatermark("双击此处打开文件夹");
        }

        private void txtFolder_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                txtFolder.Text = fbd.SelectedPath;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFolder.Text.Trim()))
            {
                updateMessage(lstInfo, "文件夹地址为空,请重新选择.");
                txtFolder.Focus();
                return;
            }
            if (!Directory.Exists(txtFolder.Text.Trim()))
            {
                updateMessage(lstInfo, "文件夹地址不存在,请重新输入.");
                txtFolder.SelectAll();
                txtFolder.Focus();
                return;
            }
            lstInfo.Items.Clear();
            long filecount = 0;
            List<FileInformation> list = new List<FileInformation>();
            list = DirectoryAllFiles.GetAllFiles(new System.IO.DirectoryInfo(@txtFolder.Text.Trim()));
 
            foreach (var item in list)
            {

                //FIlE0001类
                string newname = string.Empty;
                if (item.FileName.ToLower().Trim().Contains("file"))
                {
                    try
                    {
                        string[] temp = item.FilePath.Split('\\');
                        newname = temp[temp.Length - 3] + "@" + temp[temp.Length - 2] + "_" + item.FileName.ToLower() .Replace("file", "");
                    }
                    catch (Exception ex)
                    {

                        WriteLog(item.FilePath + " 文件名异常");
                    }



                  //  File.Move(item.FilePath, item.FileDirectory + @"\" + newname);
                }
                //
                if (item.FileName.Contains("_")   && !item.FileName.Contains ("@"))
                {
                    string[] tempfile = item.FileName.Replace(item.FileExtension, "").Split('_');
                    if (tempfile.Length == 5)    //H_20170817013502_CT00000_123456_0001
                    {
                        try
                        {
                            string[] temp = item.FilePath.Split('\\');
                            newname = temp[temp.Length - 3] + "@" + tempfile[1] + "_" + tempfile[tempfile.Length - 1] + item.FileExtension;
                        }
                        catch (Exception ex)
                        {
                            
                           WriteLog(item.FilePath + " 文件名异常");
                        }
               
                        // File.Move(item.FilePath, item.FileDirectory + @"\" + newname);

                    }

                    if (tempfile.Length == 3)
                    {

                        if (tempfile[tempfile.Length - 1].Length == 5) //8176162_20180513043852_00011
                        {

                

                                try
                                {
                                    string[] temp = item.FilePath.Split('\\');
                                    newname = temp[temp.Length - 3] + "@" + tempfile[1] + "_" + tempfile[tempfile.Length - 1].Substring(1, 4) + item.FileExtension;
                                }
                                catch (Exception)
                                {

                                    WriteLog(item.FilePath + " 文件名异常");
                                }

  




                        }

                        if (tempfile[tempfile.Length - 1].Length == 4)    //J800000_00000020170730100947_0016
                        {

                            if (tempfile[1].ToLower().StartsWith("hda") | tempfile[1].ToLower().StartsWith ("a")) //20181210173000_HDA00N_0011  20181210173000_A0001N_0011
                            {

                                try
                                {
                                    string[] temp = item.FilePath.Split('\\');
                                    newname = temp[temp.Length - 3] + "@" + tempfile[0] + "_" + tempfile[tempfile.Length - 1] + item.FileExtension;

                                   // MessageBox.Show(newname);
                                }
                                catch (Exception)
                                {

                                    WriteLog(item.FilePath + " 文件名异常");
                                }
                   

                            }
                            else
                            {


                                try
                                {
                                    string[] temp = item.FilePath.Split('\\');
                                    newname = temp[temp.Length - 3] + "@" + tempfile[1].Substring(6, 14) + "_" + tempfile[tempfile.Length - 1] + item.FileExtension;
                                }
                                catch (Exception)
                                {

                                    WriteLog(item.FilePath + " 文件名异常");
                                }
                            }
           

                        }

                    }

                }

                if (!string.IsNullOrEmpty(newname) && !File.Exists(newname))
                {
                    try
                    {
                        filecount++;
                        updateMessage(lstInfo, "正在处理:" + item.FileName);
                        Application.DoEvents();
                        File.Move(item.FilePath, item.FileDirectory + @"\" + newname);
                    }
                    catch (Exception ex)
                    {

                        updateMessage(lstInfo, ex.Message);
                    }

                }
            }


            MessageBox.Show("处理完毕,共计处理:" + filecount + "个文件");

        }





        #region 更新信息
        /// <summary>
        /// 更新信息到listbox中
        /// </summary>
        /// <param name="listbox">listbox name</param>
        /// <param name="message">message</param>
        public static void updateMessage(ListBox listbox, string message)
        {
            if (listbox.Items.Count > 1000)
                listbox.Items.RemoveAt(0);

            string item = string.Empty;
            //listbox.Items.Add("");
            item = DateTime.Now.ToString("HH:mm:ss") + " " + @message;
            if (listbox.InvokeRequired)
            {
                listbox.BeginInvoke(new Action<string>((msg) =>
                {
                    listbox.Items.Add(msg);
                }), item);

            }
            else
            {
                listbox.Items.Add(item);
            }
            if (listbox.Items.Count > 1)
            {
                listbox.TopIndex = listbox.Items.Count - 1;
                listbox.SetSelected(listbox.Items.Count - 1, true);
            }

        }
        #endregion

        public static void WriteLog(string log)
        {
            string logfile = Application.StartupPath  + @"\HA_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                StreamWriter sw = new StreamWriter(logfile, true);
                log = DateTime.Now.ToString("HH:mm:ss") + "->" + log;
                sw.WriteLine(log);
                sw.Close();
            }
            catch (Exception)
            {

                //throw;
            }


        }





    }
}
