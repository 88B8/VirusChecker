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
                    string result = await manager.UploadAndAnalyze(filePath, status =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action<string>(statusMessage => resultLabel.Text = statusMessage), status);
                        }
                        else
                        {
                            resultLabel.Text = status;
                        }
                    });
                    if (InvokeRequired)
                    {
                        Invoke(new Action<string>(statusMessage => resultLabel.Text = statusMessage), result);
                    }
                    else
                    {
                        resultLabel.Text = result;
                    }
                }
                catch (Exception ex)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action<string>(statusMessage => resultLabel.Text = statusMessage), "Произошла ошибка: " + ex.Message);
                    }
                    else
                    {
                        resultLabel.Text = "Произошла ошибка: " + ex.Message;
                    }
                }
            } else
            {
                resultLabel.Text = "Файл не выбран";
            }
        }
    }
}
