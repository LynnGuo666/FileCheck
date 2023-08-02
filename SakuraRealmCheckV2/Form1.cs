using System.Security.Cryptography;
using System.Text;

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
            string url = "https://pan.lynn6.cn/f/AWhp/files_list_md5.txt"; // 替换成实际的URL
            string filePath = "files.txt"; // 保存文件列表的文件路径

            try
            {
                await Task.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string content = await client.GetStringAsync(url);
                        File.WriteAllText(filePath, content);
                        MessageBox.Show("文件列表已成功获取并保存为 files.txt。");

                        // 在获取文件列表后进行文件存在性检测，并显示进度条
                        PerformFileExistenceCheck(filePath);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取文件列表时出现错误：" + ex.Message);
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

                // 设置进度条的最大值为文件数量
                progressBar1.Invoke((Action)(() => progressBar1.Maximum = fileEntries.Length));
                int progress = 0;

                // 获取是否需要比对MD5的选择状态
                bool shouldCompareMD5 = checkBox1.Checked;

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
                    string cloudMD5 = "N/A";

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

                        if (shouldCompareMD5 && !md5Matches)
                        {
                            message += "，但MD5校验不匹配";
                        }

                        message += $"，本地MD5: {localMD5}，云端MD5: {md5Info}";

                        lbResults.Invoke((Action)(() => lbResults.Items.Add(message)));
                    }

                    // 更新进度条的值
                    progressBar1.Invoke((Action)(() => progressBar1.Value = ++progress));
                }
            }
            else
            {
                MessageBox.Show("指定的安装目录不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool VerifyFileMD5(string filePath, string expectedMD5Info)
        {
            string actualMD5 = GetMD5HashFromFile(filePath);

            // 获取文件的实际MD5值
            int startIndex = expectedMD5Info.IndexOf("[MD5: ") + 6;
            int endIndex = expectedMD5Info.IndexOf("]");

            if (startIndex < 0 || endIndex < 0)
            {
                // 无效的MD5信息格式
                return false;
            }

            string expectedMD5 = expectedMD5Info.Substring(startIndex, endIndex - startIndex).ToLower();

            // 比较实际MD5值与预期MD5值是否匹配
            return actualMD5 == expectedMD5;
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
    }
}
