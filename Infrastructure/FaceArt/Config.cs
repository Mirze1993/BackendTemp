using System.Drawing;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FaceArt;

public class Config
{
    public static DenseTensor<float> PreprocessImage(string imagePath)
    {
        Bitmap bitmap = new Bitmap(imagePath);
        Bitmap resized = new Bitmap(bitmap, new Size(112, 112));

        var input = new DenseTensor<float>(new[] { 1, 3, 112, 112 });

        for (int y = 0; y < 112; y++)
        {
            for (int x = 0; x < 112; x++)
            {
                Color pixel = resized.GetPixel(x, y);

                input[0, 0, y, x] = (pixel.R - 127.5f) / 128.0f; // Red
                input[0, 1, y, x] = (pixel.G - 127.5f) / 128.0f; // Green
                input[0, 2, y, x] = (pixel.B - 127.5f) / 128.0f; // Blue
            }
        }

        return input;
    }
    
    public static float[] GetFaceEmbedding(string imagePath, InferenceSession session)
    {
        var tensor = PreprocessImage(imagePath);
        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("data", tensor)
        };

        using var results = session.Run(inputs);
        return results.First().AsEnumerable<float>().ToArray(); // 512 float
    }
    
    public static float CosineSimilarity(float[] v1, float[] v2)
    {
        float dot = 0f, normA = 0f, normB = 0f;

        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            normA += v1[i] * v1[i];
            normB += v2[i] * v2[i];
        }

        return dot / (float)(Math.Sqrt(normA) * Math.Sqrt(normB));
    }
    
    public void GetSimilarity()
    {
        var session = new InferenceSession("C:\\Users\\m.c.abbasaliyev\\RiderProjects\\FaceMind_ArcFace_iResNet50_CASIA_FaceV5\\ArcFace_iResNet50_CASIA_FaceV5.onnx");

        float[] emb1 = GetFaceEmbedding("C:\\Users\\m.c.abbasaliyev\\Desktop\\44_Barack_Obama_3x4.jpg", session);
        float[] emb2 = GetFaceEmbedding("C:\\Users\\m.c.abbasaliyev\\Desktop\\Valeriy_Konovalyuk_3x4.jpg", session);

        float similarity = CosineSimilarity(emb1, emb2);
        Console.WriteLine($"Oxşarlıq dərəcəsi: {similarity}");

        if (similarity > 0.6f)
            Console.WriteLine("Eyni şəxs ola bilər.");
        else
            Console.WriteLine("Fərqli şəxslər.");   
    }
    

}