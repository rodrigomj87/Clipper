/**
 * @file: CorsSettings.cs
 * @responsibility: modelo de configuração para política de CORS
 * @exports: CorsSettings
 * @imports: nenhum
 * @layer: Common/Settings
 */

namespace Clipper.Application.Common.Settings
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = System.Array.Empty<string>();
        public string[] AllowedMethods { get; set; } = System.Array.Empty<string>();
        public string[] AllowedHeaders { get; set; } = System.Array.Empty<string>();
        public bool AllowCredentials { get; set; }
        public int PreflightMaxAgeMinutes { get; set; } = 10;
    }
}
