using Amazon.S3.Model;
using Amazon.S3;
using MvcCoreProyectoSejo.Models;

namespace MvcCoreProyectoSejo.Services
{
    public class ServiceStorageAWS
    {
        private IAmazonS3 client;
        private string BucketName;

        public ServiceStorageAWS(KeysModel keysModel, IAmazonS3 client)
        {
            this.BucketName = keysModel.BucketName;
            this.client = client;
        }

        // Método para subir las imágenes donde necesitamos
        // el nombre de la carpeta, el nombre de la imagen, el stream y el bucket name
        public async Task<bool> UploadFileAsync(string folderName, string fileName, Stream stream)
        {
            // Agregar la carpeta al nombre del archivo
            string key = folderName + "/" + fileName;

            PutObjectRequest request = new PutObjectRequest
            {
                InputStream = stream,
                Key = key,
                BucketName = this.BucketName
            };
            PutObjectResponse response = await this.client.PutObjectAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
