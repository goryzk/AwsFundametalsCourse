using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3Playground;

public class S3Helper
{
    public async Task UploadFile()
    {
        await using var inputStream = new FileStream("./Microservices-General.png", FileMode.Open, FileAccess.Read);
        var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = "gorawscourse",
            Key = "images/Microservices-General.png",
            ContentType = "image/png",
            InputStream = inputStream
        };

        await s3Client.PutObjectAsync(putObjectRequest);
    }

    public async Task DownloadFile()
    {
        var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = "gorawscourse",
            Key = "images/Microservices-General.png",
        };

        var response = await s3Client.GetObjectAsync(getObjectRequest);

        using var memStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memStream);
        var text = Encoding.Default.GetString(memStream.ToArray());
        Console.WriteLine(text);
    }
}