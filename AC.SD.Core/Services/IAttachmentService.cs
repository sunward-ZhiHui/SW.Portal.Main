namespace AC.SD.Core.Services
{
    public interface IAttachmentService
    {
        string StoreAttachment(byte[] data, string contentType);
        (byte[] Data, string ContentType)? GetAttachment(string token);
        void RemoveAttachment(string token);
    }
}
