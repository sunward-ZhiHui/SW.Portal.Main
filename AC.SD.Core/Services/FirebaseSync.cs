using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using System;
using System.Linq;
using AC.SD.Core.Pages.Email;

namespace AC.SD.Core.Services
{
    public interface IFirebaseSync
    {
        Task<List<FirebaseSyncUsers>> GetUsersAsync();
        Task<FirebaseSyncUsers> CheckAddAsync(string userCode, string displayName);
        Task<FirebaseSyncUsers> AddAsync(string userCode, string displayName);
    }

    public class FirebaseSync : IFirebaseSync
    {
        private readonly FirestoreDb _db;
        public FirebaseSync(FirestoreDb db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<List<FirebaseSyncUsers>> GetUsersAsync()
        {
            var collection = _db.Collection("Users");
            var query = collection.OrderBy("name");
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => doc.ConvertTo<FirebaseSyncUsers>()).ToList();
        }
        public async Task<FirebaseSyncUsers> CheckAddAsync(string userCode, string displayName)
        {
            // Check if the entry already exists
            var existingUserQuery = _db.Collection("Users")
                                         .WhereEqualTo("userCode", userCode);
            var existingUserQuerySnapshot = await existingUserQuery.GetSnapshotAsync();

            if (existingUserQuerySnapshot.Count > 0)
            {
                // Entry with the same user code already exists, handle accordingly
                // For example, you may log a message or skip the insertion
                Console.WriteLine($"User with userCode '{userCode}' already exists. Skipping insertion.");
                return null; // or return the existing user or handle it in a different way
            }

            // Entry does not exist, proceed with insertion
            var firebaseSyncUsers = new FirebaseSyncUsers
            {
                userCode = userCode,
                display_name = displayName
            };

            var collection = _db.Collection("Users");
            var document = await collection.AddAsync(firebaseSyncUsers);
            firebaseSyncUsers.Id = document.Id;

            return firebaseSyncUsers;
        }
        public async Task<FirebaseSyncUsers> AddAsync(string userCode, string displayName)
        {
            var firebaseSyncUsers = new FirebaseSyncUsers
            {
                userCode = userCode,
                display_name = displayName
            };

            var collection = _db.Collection("FirebaseSyncUsers");
            var document = await collection.AddAsync(firebaseSyncUsers);
            firebaseSyncUsers.Id = document.Id;

            return firebaseSyncUsers;
        }
    }
}
