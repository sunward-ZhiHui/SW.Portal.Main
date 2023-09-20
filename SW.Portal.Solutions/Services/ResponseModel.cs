namespace SW.Portal.Solutions.Services
{
    public enum ResponseCode
    {
        Success = 1,
        Failure = 2
    }
    public class ResponseModel<T>
    {
        public ResponseModel()
        {
            ErrorMessages = new List<string>();
        }

        public ResponseCode ResponseCode { get; set; }
        public T Result { get; set; }
        public IList<T> Results { get; set; }        
        public IList<string> ErrorMessages { get; set; }
    }
}
