namespace Project_Web___Veterinary_Clínic.Helpers
{
    public interface IEmailHelper
    {
        Response SendEmail(string to, string subject, string body);
    }
}
