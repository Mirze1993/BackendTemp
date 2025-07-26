
using System.Reflection;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace FaceArt;

public class ImgSimilarityService
{
     static DenseTensor<float> PreprocessImage(Stream stream)
    {  if (stream == null || stream.Length == 0)
            throw new ArgumentException("Şəkil stream boşdur.");

        stream.Position = 0;
        using var image = Image.Load<Rgb24>(stream);
        image.Mutate(x => x.Resize(112, 112));

        var input = new DenseTensor<float>([1, 3, 112, 112]);

        for (int y = 0; y < 112; y++)
        {
            for (int x = 0; x < 112; x++)
            {
                Rgb24 pixel = image[x, y];

                input[0, 0, y, x] = (pixel.R - 127.5f) / 128.0f; // Red
                input[0, 1, y, x] = (pixel.G - 127.5f) / 128.0f; // Green
                input[0, 2, y, x] = (pixel.B - 127.5f) / 128.0f; // Blue
            }
        }

        return input;
    }
    
     static float[] GetFaceEmbedding(Stream stream, InferenceSession session)
    {
        var tensor = PreprocessImage(stream);
        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("data", tensor)
        };

        using var results = session.Run(inputs);
        return results.First().AsEnumerable<float>().ToArray(); // 512 float
    }
    
     static float CosineSimilarity(float[] v1, float[] v2)
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
    
    public float GetSimilarity(Stream file1, Stream file2)
    { 
        var currentDirectory = System.IO.Directory.GetCurrentDirectory(); 
        var currentDirectory2 = Path.GetDirectoryName(typeof(ImgSimilarityService).Assembly.Location);;
        Console.WriteLine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
        Console.WriteLine(System.Environment.CurrentDirectory);
        Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
        Console.WriteLine(Environment.CurrentDirectory);

        var path = "C:\\Users\\ROG\\RiderProjects\\BackendTemp\\Infrastructure\\FaceArt\\model\\FaceMind_ArcFace_iResNet50_CASIA_FaceV5\\ArcFace_iResNet50_CASIA_FaceV5.onnx";


        var session = new InferenceSession(path);

        float[] emb1 = GetFaceEmbedding(file1, session);
        float[] emb2 = GetFaceEmbedding(file2, session);

        return CosineSimilarity(emb1, emb2);
       
    }
    

}