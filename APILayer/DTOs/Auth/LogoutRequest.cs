namespace InventoryAPI.DTOs.Auth
{
    public class LogoutRequest
    {
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
    }
}
