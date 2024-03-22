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
        [FirestoreProperty("userCode")]
        public string? UserCode { get; set; }
        [FirestoreProperty("Filepath")]
        public string? Filepath { get; set; }
    }
}
