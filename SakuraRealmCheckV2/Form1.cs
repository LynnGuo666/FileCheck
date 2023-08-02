using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SakuraRealmCheckV2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGetFiles_Click(object sender, EventArgs e)
        {
            string url = "https://download-us.34036330.xyz/d/global/files_list_md5.txt"; // �滻��ʵ�ʵ�URL
            string filePath = "files.txt"; // �����ļ��б���ļ�·��
            btnGetFiles.Enabled = false;
            btnGetFiles.Text = "��ȡ��";
            lbResults.Items.Clear();
            try
            {
                await Task.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string content = await client.GetStringAsync(url);
                        File.WriteAllText(filePath, content);
                        // MessageBox.Show("�ļ��б��ѳɹ���ȡ������Ϊ files.txt��");
                        btnGetFiles.Text = "�����";
                        // �ڻ�ȡ�ļ��б������ļ������Լ�⣬����ʾ������
                        PerformFileExistenceCheck(filePath);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("��ȡ�ļ��б�ʱ���ִ���" + ex.Message);
            }
        }

        private void PerformFileExistenceCheck(string filePath)
        {
            string installationDirectory = Path.Combine(Application.StartupPath, "Game");

            if (Directory.Exists(installationDirectory))
            {
                string[] fileEntries = File.ReadAllLines(filePath);
                string[] skipItems = File.ReadAllLines("skip.txt");

                HashSet<string> skipSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string skipItem in skipItems)
                {
                    skipSet.Add(skipItem?.Trim());
                }

                // ���ý����������ֵΪ�ļ�����
                progressBar1.Invoke((Action)(() => progressBar1.Maximum = fileEntries.Length));
                int progress = 1;

                // ��ȡ�Ƿ���Ҫ�ȶ�MD5��ѡ��״̬
                bool shouldCompareMD5 = checkBox1.Checked;

                List<string> missingFiles = new List<string>();

                foreach (string fileEntry in fileEntries)
                {
                    string[] parts = fileEntry.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    string fileName = parts[0].Trim();
                    string md5Info = parts[1].Trim();

                    string clientPath = Path.Combine(installationDirectory, fileName);

                    // ��������ض����ļ��к��ļ�
                    string folderName = Path.GetDirectoryName(clientPath);
                    if (skipSet.Contains(folderName) || skipSet.Contains(Path.GetFileName(clientPath)))
                    {
                        continue;
                    }

                    bool fileExists = File.Exists(clientPath);
                    bool md5Matches = false;

                    string localMD5 = "N/A";
                    md5Info = md5Info.Replace("MD5: ", "");

                    if (shouldCompareMD5 && fileExists)
                    {
                        md5Matches = VerifyFileMD5(clientPath, md5Info);
                        localMD5 = GetMD5HashFromFile(clientPath);
                    }

                    if (!fileExists || (shouldCompareMD5 && !md5Matches))
                    {
                        string message = $"�ļ� {fileName} ";
                        if (!fileExists)
                        {
                            message += "�������� ";
                        }
                        else
                        {
                            message += "������ ";
                        }
                        message += $"{installationDirectory}";

                        if (shouldCompareMD5 && md5Matches == false)
                        {
                            message += "����MD5У�鲻ƥ��";
                        }

                        message += $"������MD5: {localMD5}���ƶ�MD5: {md5Info}";

                        lbResults.Invoke((Action)(() => lbResults.Items.Add(message)));

                        if (checkBox2.Checked && (!fileExists || (shouldCompareMD5 && !md5Matches)))
                        {
                            missingFiles.Add(fileName);
                        }
                    }

                    // ���½�������ֵ
                    progressBar1.Invoke((Action)(() => progressBar1.Value = ++progress));
                }

                if (checkBox2.Checked && missingFiles.Count > 0)
                {
                    DownloadMissingFiles(missingFiles);
                    btnGetFiles.Enabled = false;
                    btnGetFiles.Text = "��ȫ��";
                }
                else
                {
                    btnGetFiles.Enabled = true;
                    btnGetFiles.Text = "��ʼ���";
                    MessageBox.Show("�����ɣ�");
                }
            }
            else
            {
                MessageBox.Show("ָ���İ�װĿ¼�����ڣ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ��������������ȱʧ���ļ�
        private async void DownloadMissingFiles(List<string> missingFiles)
        {
            string serverBaseUrl = "https://download-us.34036330.xyz/d/global/Game/";
            string installationDirectory = Path.Combine(Application.StartupPath, "Game");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    progressBar1.Invoke((Action)(() => progressBar1.Value = 0)); // ���ý�����
                    progressBar1.Invoke((Action)(() => progressBar1.Maximum = missingFiles.Count));

                    int progress = 0;

                    foreach (string missingFile in missingFiles)
                    {
                        string clientPath = Path.Combine(installationDirectory, missingFile);
                        string serverUrl = serverBaseUrl + missingFile;
                        await DownloadFileAsync(client, serverUrl, clientPath);

                        // ���½�������ֵ
                        progressBar1.Invoke((Action)(() => progressBar1.Value = ++progress));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("�����ļ�ʱ���ִ���" + ex.Message);
            }
            btnGetFiles.Enabled = true;
            btnGetFiles.Text = "��ʼ���";
            MessageBox.Show("��ȫ��ɣ�");
        }

        private bool VerifyFileMD5(string filePath, string expectedMD5Info)
        {
            string actualMD5 = GetMD5HashFromFile(filePath);

            // ��ȡ�ļ���ʵ��MD5ֵ
            int startIndex = expectedMD5Info.IndexOf("[MD5: ") + 6;
            int endIndex = expectedMD5Info.IndexOf("]");

            //            if (startIndex < 0 || endIndex < 0)
            //            {
            // ��Ч��MD5��Ϣ��ʽ
            //                return false;
            //            }
            string expectedMD5 = expectedMD5Info;

            if (actualMD5 == expectedMD5Info)
            {
                return true;
            }
            else
            {
                return false;
            }
            // �Ƚ�ʵ��MD5ֵ��Ԥ��MD5ֵ�Ƿ�ƥ��
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            lbResults.Items.Clear();
        }

        // ���������������첽�����ļ�
        private async Task DownloadFileAsync(HttpClient client, string url, string filePath)
        {
            try
            {
                byte[] fileData = await client.GetByteArrayAsync(url);
                File.WriteAllBytes(filePath, fileData);
                lbResults.Invoke((Action)(() => lbResults.Items.Add($"�������ļ���{filePath}")));
            }
            catch (Exception ex)
            {
                lbResults.Invoke((Action)(() => lbResults.Items.Add($"�����ļ� {url} ʱ���ִ���{ex.Message}")));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // �����ť��ִ���ļ���ȡ
            btnGetFiles_Click(sender, e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://help.yumoe.top";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://afdian.net/a/lynnguo666";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://help.yumoe.top/?p=SakuraRealmLauncherV3-Preload";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Enabled == false)
            {
                MessageBox.Show("����δӵ�иù��ܵ�Ȩ�ޣ�");
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {
            string url = "https://afdian.net/a/lynnguo666";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
