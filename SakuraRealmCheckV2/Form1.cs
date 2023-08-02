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
            string url = "https://pan.lynn6.cn/f/qytO/files_list.txt"; // 替换成实际的URL
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

                        // 在获取文件列表后进行文件存在性检测，并输出不存在的文件
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
                string[] fileNames = File.ReadAllLines(filePath);
                string[] skipItems = File.ReadAllLines("skip.txt");

                HashSet<string> skipSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string skipItem in skipItems)
                {
                    skipSet.Add(skipItem?.Trim());
                }

                lbResults.Items.Clear();

                foreach (string fileName in fileNames)
                {
                    string clientPath = Path.Combine(installationDirectory, fileName);

                    // 跳过检查特定的文件夹和文件
                    string folderName = Path.GetDirectoryName(clientPath);
                    if (skipSet.Contains(folderName) || skipSet.Contains(Path.GetFileName(clientPath)))
                    {
                        continue;
                    }

                    if (!File.Exists(clientPath))
                    {
                        lbResults.Items.Add($"文件 {fileName} 不存在于 {installationDirectory}");
                    }
                }
            }
            else
            {
                MessageBox.Show("指定的安装目录不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
