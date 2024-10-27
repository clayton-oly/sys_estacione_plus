using EmguCV = Emgu.CV; // Alias para Emgu CV
using OpenCvSharp; // Usando OpenCvSharp
using System;
using System.Windows.Forms;
using Tesseract;

namespace EstacionePlus
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
        }

        private void btnSelectImagem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\"; // Diretório inicial
                openFileDialog.Filter = "Imagens (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"; // Filtros para tipos de arquivos
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName; // Obtém o caminho da imagem selecionada
                    ProcessarImagem(imagePath); // Chama o método para processar a imagem
                }
            }
        }

        private void ProcessarImagem(string imagePath)
        {
            string tessDataPath = @"C:\Users\Emil\source\repos\EstacionePlus\tessdata";

            try
            {
                // Carregar a imagem com OpenCV
                Mat image = Cv2.ImRead(imagePath, ImreadModes.Grayscale);

                // Aumentar o contraste e o brilho
                image.ConvertTo(image, MatType.CV_8U, alpha: 1.5, beta: 20); // Ajuste os valores conforme necessário

                // Aplicar um filtro de desfoque leve para reduzir ruído
                Cv2.GaussianBlur(image, image, new OpenCvSharp.Size(3, 3), 0); // Usando OpenCvSharp.Size

                // Salvar a imagem processada para revisão (opcional)
                Cv2.ImWrite(@"C:\Users\Emil\source\repos\EstacionePlus\placas\exemplo_preprocessado.jpg", image);

                // Processar a imagem com o Tesseract
                var ocrEngine = new TesseractEngine(tessDataPath, "por", EngineMode.Default);
                ocrEngine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

                var page = ocrEngine.Process(Pix.LoadFromFile(@"C:\Users\Emil\source\repos\EstacionePlus\placas\exemplo_preprocessado.jpg"));
                string textoExtraido = page.GetText();

                // Exibir o texto extraído no TextBox
                txtResult.Text = textoExtraido; // Adiciona o texto extraído ao TextBox

                Console.WriteLine("Texto Extraído da Imagem:");
                Console.WriteLine(textoExtraido);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro durante o OCR: " + ex.Message);
            }
        }
    }
}
