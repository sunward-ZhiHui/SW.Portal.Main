using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
namespace SW.Portal.Solutions.Services
{
    [FirestoreData]
    public class FirebaseEmailDoc
    {
        [FirestoreDocumentId]
        [Key]
        public string? Id { get; set; }       
        [FirestoreProperty("Filepath")]
        public string? Filepath { get; set; }
        [FirestoreProperty("Imagepath")]
        public string? Imagepath { get; set; }
    }
}
