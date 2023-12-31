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
using Newtonsoft.Json;


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
            string jsonUrl = "https://setup-online.lynnguo666.workers.dev/?id=sakurarealm"; // 获取 JSON 数据的云端链接
            string filePath = "files.txt"; // 保存 files.txt 内容的本地文件路径
            string skipPath = "skip.txt"; // 保存 skip.txt 内容的本地文件路径
            string deletePath = "delete.txt";

            btnGetFiles.Enabled = false;
            btnGetFiles.Text = "获取中";
            lbResults.Items.Clear();

            try
            {
                await Task.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // 从云端获取 JSON 数据
                        string jsonContent = await client.GetStringAsync(jsonUrl);

                        // 解析 JSON 数据
                        dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
                        string filesUrl = jsonData.list;
                        string skipUrl = jsonData.skip;
                        string deleteUrl = jsonData.delete;


                        // 从云端获取 files.txt
                        string filesContent = await client.GetStringAsync(filesUrl);
                        File.WriteAllText(filePath, filesContent);

                        // 从云端获取 skip.txt
                        string skipContent = await client.GetStringAsync(skipUrl);
                        File.WriteAllText(skipPath, skipContent);
                        // 获取Delete.txt
                        string deleteContent = await client.GetStringAsync(deleteUrl);
                        File.WriteAllText(deletePath, deleteContent);

                        btnGetFiles.Text = "检测中";

                        // 获取完两个文件后，进行文件存在性检测
                        PerformFileExistenceCheck(filePath, skipPath);

                        // 检测结束后，开始删除文件

                        DeleteFilesFromDeleteList(deleteUrl);


                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取文件列表时出现错误：" + ex.Message);
            }
        }

        private void PerformFileExistenceCheck(string filePath, string skipPath)
        {
            string installationDirectory = Path.Combine(Application.StartupPath, "Game");

            if (Directory.Exists(installationDirectory))
            {
                string[] fileEntries = File.ReadAllLines(filePath);
                string[] skipItems = File.ReadAllLines(skipPath);

                HashSet<string> skipSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string skipItem in skipItems)
                {
                    skipSet.Add(skipItem?.Trim());
                }

                // 设置进度条的最大值为文件数量
                progressBar1.Invoke((Action)(() => progressBar1.Maximum = fileEntries.Length));
                int progress = 1;

                // 获取是否需要比对MD5的选择状态
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

                    // 跳过检查特定的文件夹和文件
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
                        string message = $"文件 {fileName} ";
                        if (!fileExists)
                        {
                            message += "不存在于 ";
                        }
                        else
                        {
                            message += "存在于 ";
                        }
                        message += $"{installationDirectory}";

                        if (shouldCompareMD5 && md5Matches == false)
                        {
                            message += "，但MD5校验不匹配";
                        }

                        message += $"，本地MD5: {localMD5}，云端MD5: {md5Info}";

                        lbResults.Invoke((Action)(() => lbResults.Items.Add(message)));

                        if (checkBox2.Checked && (!fileExists || (shouldCompareMD5 && !md5Matches)))
                        {
                            missingFiles.Add(fileName);
                        }
                    }

                    // 更新进度条的值
                    progressBar1.Invoke((Action)(() => progressBar1.Value = ++progress));
                }

                if (checkBox2.Checked && missingFiles.Count > 0)
                {
                    DownloadMissingFiles(missingFiles);
                    btnGetFiles.Enabled = false;
                    btnGetFiles.Text = "补全中";
                }
                else
                {
                    btnGetFiles.Enabled = true;
                    btnGetFiles.Text = "开始检测";
                    MessageBox.Show("检测完成！");
                }
            }
            else
            {
                MessageBox.Show("指定的安装目录不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 新增方法：下载缺失的文件
        private async void DownloadMissingFiles(List<string> missingFiles)
        {
            string serverBaseUrl = "https://download-us.34036330.xyz/d/global/Game/";
            string installationDirectory = Path.Combine(Application.StartupPath, "Game");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    progressBar1.Invoke((Action)(() => progressBar1.Value = 0)); // 重置进度条
                    progressBar1.Invoke((Action)(() => progressBar1.Maximum = missingFiles.Count));

                    int progress = 0;

                    foreach (string missingFile in missingFiles)
                    {
                        string clientPath = Path.Combine(installationDirectory, missingFile);
                        string serverUrl = serverBaseUrl + missingFile;
                        await DownloadFileAsync(client, serverUrl, clientPath);

                        // 更新进度条的值
                        progressBar1.Invoke((Action)(() => progressBar1.Value = ++progress));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("下载文件时出现错误：" + ex.Message);
            }
            btnGetFiles.Enabled = true;
            btnGetFiles.Text = "开始检测";
            MessageBox.Show("补全完成！");
        }

        private bool VerifyFileMD5(string filePath, string expectedMD5Info)
        {
            string actualMD5 = GetMD5HashFromFile(filePath);

            // 获取文件的实际MD5值
            int startIndex = expectedMD5Info.IndexOf("[MD5: ") + 6;
            int endIndex = expectedMD5Info.IndexOf("]");

            //            if (startIndex < 0 || endIndex < 0)
            //            {
            // 无效的MD5信息格式
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
            // 比较实际MD5值与预期MD5值是否匹配
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

        // 辅助方法，用于异步下载文件
        private async Task DownloadFileAsync(HttpClient client, string url, string filePath)
        {
            try
            {
                byte[] fileData = await client.GetByteArrayAsync(url);
                File.WriteAllBytes(filePath, fileData);
                lbResults.Invoke((Action)(() => lbResults.Items.Add($"已下载文件：{filePath}")));
            }
            catch (Exception ex)
            {
                lbResults.Invoke((Action)(() => lbResults.Items.Add($"下载文件 {url} 时出现错误：{ex.Message}")));
            }
        }
        private async Task DeleteFilesFromDeleteList(string deletePath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 获取 delete.txt 内容
                    string deleteContent = await client.GetStringAsync(deletePath);
                    string installationDirectory = Path.Combine(Application.StartupPath, "Game");

                    // 删除文件列表中的每个文件
                    string[] filesToDelete = deleteContent.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string fileToDelete in filesToDelete)
                    {
                        string fileName = fileToDelete.Trim();
                        string filePath = Path.Combine(installationDirectory, fileName);

                        // 确保文件存在后再进行删除
                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                                lbResults.Invoke((Action)(() => lbResults.Items.Add($"已删除文件：{filePath}")));
                            }
                            catch (Exception ex)
                            {
                                lbResults.Invoke((Action)(() => lbResults.Items.Add($"删除文件 {filePath} 时出现错误：{ex.Message}")));
                            }
                        }
                        else
                        {
                            lbResults.Invoke((Action)(() => lbResults.Items.Add($"文件 {filePath} 不存在，无需删除。")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除文件时出现错误：" + ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // 点击按钮后执行文件获取
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


        private void label1_Click(object sender, EventArgs e)
        {
            string url = "https://afdian.net/a/lynnguo666";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
