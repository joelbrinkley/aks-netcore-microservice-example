namespace FrontEnd.Services
{
    public class ServiceResponse<T>
    {
        public bool Successful { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}