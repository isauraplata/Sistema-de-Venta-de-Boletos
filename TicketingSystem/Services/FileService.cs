using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;

namespace TicketingSystem.Services;
public class FirebaseStorageService : IFileService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;
    private const string UPLOADS_FOLDER = "uploads";

    public FirebaseStorageService(IConfiguration configuration)
    {
        var credential = GoogleCredential.FromFile("credentials.json");
        _storageClient = StorageClient.Create(credential);
        _bucketName = configuration["Firebase:StorageBucket"]; 
    }

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file was provided");

        // Validar el tipo de archivo
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Invalid file type");

        // Generar nombre único para el archivo
        var uniqueFileName = $"{UPLOADS_FOLDER}/{Guid.NewGuid()}{fileExtension}";

        // Subir archivo a Firebase Storage
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var uploadOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            };

            await _storageClient.UploadObjectAsync(
                _bucketName,
                uniqueFileName,
                file.ContentType,
                memoryStream,
                options: uploadOptions);
        }

        // Retornar la URL pública del archivo
        return $"https://storage.googleapis.com/{_bucketName}/{uniqueFileName}";
    }

    public async void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;

        try
        {
            
            var uri = new Uri(imageUrl);
            var objectName = uri.LocalPath.TrimStart('/');
            objectName = objectName.Substring(objectName.IndexOf('/') + 1);

            // Eliminar el objeto de Firebase Storage
            await _storageClient.DeleteObjectAsync(_bucketName, objectName);
        }
        catch (Exception ex)
        {
 
            throw new Exception($"Error deleting file from Firebase Storage: {ex.Message}");
        }
    }
}