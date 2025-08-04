
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
        using var original = Image.Load(stream);     
        using var image = original.CloneAs<Rgb24>(); 
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
        foreach (var input in session.InputMetadata)
        {
            Console.WriteLine($"Input name: {input.Key}, Type: {input.Value.ElementType}, Dimensions: {string.Join(",", input.Value.Dimensions)}");
        }
        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input.1", tensor)
        };

        using var results = session.Run(inputs);
        var embedding = results.First().AsEnumerable<float>().ToArray();

        return embedding;
    }
     
    private static float[] AverageEmbeddings(List<float[]> embeddings)
    {
        int length = embeddings[0].Length;
        float[] avg = new float[length];

        foreach (var emb in embeddings)
        {
            for (int i = 0; i < length; i++)
                avg[i] += emb[i];
        }

        for (int i = 0; i < length; i++)
            avg[i] /= embeddings.Count;

        return avg;
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
        var session = GetInferenceSession();
        float[] emb1 = GetFaceEmbedding(file1, session);
        float[] emb2 = GetFaceEmbedding(file2, session);
        return CosineSimilarity(emb1, emb2);
       
    }
    
    public float CompareToGallery(Stream queryImage, List<Stream> knownImages)
    {
        var session = GetInferenceSession();
        var galleryEmbeddings = knownImages
            .Select(mm=>GetFaceEmbedding(mm,session))
            .ToList();

        var avgEmbedding = AverageEmbeddings(galleryEmbeddings);
        var queryEmbedding = GetFaceEmbedding(queryImage,session);

        return CosineSimilarity(avgEmbedding, queryEmbedding);
    }

    private static InferenceSession GetInferenceSession()
    {
        var path = "C:\\Users\\m.c.abbasaliyev\\Downloads\\model.onnx";
        var session = new InferenceSession(path);
        return session;
    }
}