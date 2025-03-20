using System.Collections.Generic;
using System;

namespace AC.SD.Core.Services
{
    public class AttachmentService
    {
        private readonly Dictionary<string, (byte[], string)> _attachmentStore = new();

        public string StoreAttachment(byte[] data, string contentType)
        {
            string token = Guid.NewGuid().ToString();
            _attachmentStore[token] = (data, contentType);
            return token;
        }

        public (byte[] Data, string ContentType)? GetAttachment(string token)
        {
            return _attachmentStore.TryGetValue(token, out var fileData) ? fileData : null;
        }

        public void RemoveAttachment(string token)
        {
            _attachmentStore.Remove(token);
        }
    }


}
