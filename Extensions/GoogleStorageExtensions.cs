using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class GoogleStorageExtensions
{
  public static async Task MakeObjectPublicAsync(this StorageClient client, string bucketName, string objectName)
  {
    var obj = await client.GetObjectAsync(bucketName, objectName);
    obj.Acl = new List<ObjectAccessControl>
        {
            new ObjectAccessControl
            {
                Entity = "allUsers",
                Role = "READER"
            }
        };
    await client.UpdateObjectAsync(obj);
  }

  public static string GenerateSignedUrl(string jsonServiceAccountPath, string bucketName, string objectName, TimeSpan validFor)
  {
    var signer = UrlSigner.FromServiceAccountPath(jsonServiceAccountPath);
    return signer.Sign(bucketName, objectName, validFor, HttpMethod.Get);
  }
}