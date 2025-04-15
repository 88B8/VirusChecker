namespace VirusChecker
{
    public partial class Form1 : Form
    {
        private VirusCheckerManager manager = new VirusCheckerManager();

        public Form1()
        {
            InitializeComponent();
        }

        private async void chooseFileButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Исполняемые файлы (*.exe;*.dll)|*.exe;*.dll|Документы (*.pdf)|*.pdf|Архивы (*.zip)|*.zip";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    var scanResult = await manager.UploadAndAnalyze(filePath, status =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action<string>(msg => resultLabel.Text = msg), status);
                        }
                        else
                        {
                            resultLabel.Text = status;
                        }
                    });

                    if (scanResult.MaliciousCount > 0 || scanResult.TotalEngines > 0)
                    {
                        string finalMessage = scanResult.IsDangerous
                            ? $"⚠️ Обнаружено угроз: {scanResult.MaliciousCount} из {scanResult.TotalEngines} антивирусов сработали"
                            : $"✅ Безопасно: {scanResult.MaliciousCount} из {scanResult.TotalEngines} антивирусов сработали";

                        if (InvokeRequired)
                        {
                            Invoke(new Action<string>(msg => resultLabel.Text = msg), finalMessage);
                        }
                        else
                        {
                            resultLabel.Text = finalMessage;
                        }
                    }
                    else
                    {
                        resultLabel.Text = "Файл еще не был проанализирован на VirusTotal.";
                    }
                }
                catch (Exception ex)
                {
                    string error = "Произошла ошибка: " + ex.Message;
                    if (InvokeRequired)
                    {
                        Invoke(new Action<string>(msg => resultLabel.Text = msg), error);
                    }
                    else
                    {
                        resultLabel.Text = error;
                    }
                }
            }
            else
            {
                resultLabel.Text = "Файл не выбран";
            }
        }
    }
}
