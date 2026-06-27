using System.ComponentModel.DataAnnotations;

namespace IdeaSparringPartner.Api.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
