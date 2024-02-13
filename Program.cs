using System.Threading;
using System.IO;
using System.Drawing;

namespace redimensionador;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Thread thread = new (Redimensionar);
        thread.Start();
        Console.WriteLine("Acabou");
    }


    static void Redimensionar()
    {
        #region "Diretorios"
        string diretorio_entrada = "Arquivos_Entrada";
        string diretorio_redimensionados = "Arquivos_Redimensionados";
        string diretorio_finalizados = "Arquivos_Finalizados";

        if (!Directory.Exists(diretorio_entrada))
            Directory.CreateDirectory(diretorio_entrada);
        if (!Directory.Exists(diretorio_finalizados))
            Directory.CreateDirectory(diretorio_finalizados);
        if (!Directory.Exists(diretorio_redimensionados))
            Directory.CreateDirectory(diretorio_redimensionados);
        #endregion

        FileStream fileStream; // Utilizar a mesma variável e ir instanciado em cima dela
        FileInfo fileInfo;
        while (true)
        {
            var arquivoEntrada = Directory.EnumerateFiles(diretorio_entrada);

            int novaAltura = 200;

            foreach (var arquivo in arquivoEntrada)
            {
                // Abrir arquivo
                fileStream = new FileStream(arquivo, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                fileInfo = new FileInfo(arquivo);

                // Redimensionar + Copiar os arquivos redimensionados para a pasta de redimensionados
                string caminho = Environment.CurrentDirectory + @"\" + diretorio_redimensionados + @"\" + DateTime.Now.Millisecond.ToString() + "_" + fileInfo.Name ;
                Redimensionador(Image.FromStream(fileStream), novaAltura, caminho);

                // Fechar o arquivo
                fileStream.Close();
                
                // Mover arquivos de entrada para a pasta de finalizados
                string caminhoFinalizado = Environment.CurrentDirectory + @"\" + diretorio_finalizados + @"\" + fileInfo.Name;
                fileInfo.MoveTo(caminhoFinalizado);
            }

            Thread.Sleep(new TimeSpan(0, 0, 3));
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="imagem">Imagem a ser redimensionada</param>
    /// <param name="altura">Altura que desejamos redimensionar</param>
    /// <param name="caminho">Caminho onde os serão salvo os arquivos redimensionados </param>
    /// <returns></returns>
    static void Redimensionador(Image imagem, int altura, string caminho)
    {
        double ratio = (double)altura / imagem.Height;
        int novaLargura = (int)(imagem.Width * ratio);
        int novaAltura = (int)(imagem.Height * ratio);

        Bitmap novaImage = new Bitmap(novaLargura, novaAltura);

        using(Graphics g = Graphics.FromImage(novaImage))
        {
            g.DrawImage(imagem, 0, 0, novaLargura, novaAltura);
        }

        novaImage.Save(caminho);
        imagem.Dispose();
    }
}
