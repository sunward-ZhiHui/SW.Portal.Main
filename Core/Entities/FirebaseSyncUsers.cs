using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [FirestoreData]
    public class FirebaseSyncUsers
    {
        [FirestoreDocumentId]
        [Key]
        public string? Id { get; set; }
        [FirestoreProperty("userCode")]      
        public string? userCode { get; set; }       
        [FirestoreProperty("display_name")]
        public string? display_name { get; set; }
    }
}
