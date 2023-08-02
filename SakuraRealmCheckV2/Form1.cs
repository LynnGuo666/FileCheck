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
            string url = "https://pan.lynn6.cn/f/qytO/files_list.txt"; // �滻��ʵ�ʵ�URL
            string filePath = "files.txt"; // �����ļ��б���ļ�·��

            try
            {
                await Task.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string content = await client.GetStringAsync(url);
                        File.WriteAllText(filePath, content);
                        MessageBox.Show("�ļ��б��ѳɹ���ȡ������Ϊ files.txt��");

                        // �ڻ�ȡ�ļ��б������ļ������Լ�⣬����������ڵ��ļ�
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

                    // ��������ض����ļ��к��ļ�
                    string folderName = Path.GetDirectoryName(clientPath);
                    if (skipSet.Contains(folderName) || skipSet.Contains(Path.GetFileName(clientPath)))
                    {
                        continue;
                    }

                    if (!File.Exists(clientPath))
                    {
                        lbResults.Items.Add($"�ļ� {fileName} �������� {installationDirectory}");
                    }
                }
            }
            else
            {
                MessageBox.Show("ָ���İ�װĿ¼�����ڣ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
