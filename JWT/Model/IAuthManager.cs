namespace JWT.Server.Model
{
   public interface IAuthManager
   {
      string Authenticate(string username, string password);
   }
}
