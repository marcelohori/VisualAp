using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;


namespace VisualApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const string subscriptionKey = "122f8037b2684280a871bf9e6e476eba";
        const string uriBase = "https://visionfiap.cognitiveservices.azure.com/";
        string imageFilePath = "";
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            // Filtro para aceitar somente imagens nos formatos JPEG and PNG  
            ofd.Filter = "JPEG *.jpg|*.jpg|PNG *.png|*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Armazena arquivo selecionado em uma variavel generica   
                imageFilePath = ofd.FileName;
                // Preview da imagem selecionada no controle pictureBox  
                pictureBox1.Image = Image.FromFile(imageFilePath);
            }
        }

        

        // Metodo que faz a requisicao restful   
        async Task<string> MakeAnalaysisRequest(string imageFilePath)
        {
            // Instancia um novo objeto do tipo HttpClient para realizar o post no Azure  
            HttpClient client = new HttpClient();

            // Parametros de requisicao 
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Especifica as categorias pesquisadas   
            string requestParamters = "visualFeatures=Categories,Description,Color&language=pt";

            // Endpoint com os parametros   
            string uri = uriBase + "vision/v2.1/analyze" + "?" + requestParamters;

            // Cria um objeto para o retorno do Request    
            HttpResponseMessage responseMessage;

            // Tamanho da imagem selecionada   
            byte[] dataByte = GetImageAsBytesArray(imageFilePath);

            
            using (ByteArrayContent content = new ByteArrayContent(dataByte))
            {
                // Define o conteudo 
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                // Envia o conteudo do Request
                responseMessage = await client.PostAsync(uri, content);

                // Recebe a resposta do request enviado   
                string contentString = await responseMessage.Content.ReadAsStringAsync();

                // Retorna resultado
                return contentString;
            }

        }

        // Metodo que verifica tamanho da imagem
        private byte[] GetImageAsBytesArray(string imageFilePath)
        {
            // Cria um file stream para acessar os bytes do arquivo selecionado   
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            // Cria um leitor binario   
            BinaryReader reader = new BinaryReader(fileStream);
            // Retorna os bytes do arquivo   
            return reader.ReadBytes((int)fileStream.Length);
        }

        private async void btnMostraResultado_Click(object sender, EventArgs e)
        {
            // Pega o resultado 
            string result = await MakeAnalaysisRequest(imageFilePath);

            // Mostra resultado no textbox   
            txtResult.Text = result;
        }
    }
}
