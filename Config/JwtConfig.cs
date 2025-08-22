namespace UserCRUD.Config
{
    public class JwtConfig
    {
        public string Secret { get; set; } = "chave_super_secreta_Jabil_Ambiente_Dev";
        public string Issuer { get; set; } = "UsuariosCtrlAPI";
        public string Audience { get; set; } = "UsuariosCtrlAPI_usuarios";
        public int ExpirationMinutes { get; set; } = 60;
    }
}
