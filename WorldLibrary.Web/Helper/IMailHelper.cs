namespace WorldLibrary.Web.Helper
{
    public interface IMailHelper
    {
        Response SendEmail(string to, string subject, string body);
    }
}
