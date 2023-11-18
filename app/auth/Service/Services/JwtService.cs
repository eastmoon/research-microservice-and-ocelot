using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    /// <summary>
    /// 提供 JWT 產生解析服務
    /// 參考文獻 https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration Configuration;
        public JwtService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public string GenerateToken(string userName, int expireMinutes = 30)
        {
            // 從設定檔中取得設定參數，在此範例中使用指定字串
            var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>();

            // JSON Web Token (JWT) 是一個開放標準 ( RFC 7519 )，是通過一種緊湊、自洽的方式定義了在多點間以安全協議傳輸 JSON 物件的方法。
            // 這種 JSON 物件是經過數字簽名的，因此是可被驗證的、可信的。JWT 可以使用對稱加密演算法（演算法 HMAC）加密，或者更常見地使用 RSA 演算法以 Public/Private 祕鑰對來加密。
            // 一個標準 JWT 由三個部分組成：
            // Header：記錄加密演算法
            // Payload：記錄了各種 RFC 7519 規定的或使用者自定義的資訊域，稱為 "Claim"
            // Signature：記錄整個 Token 的數位簽章
            // 而在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claim，文獻參考 https://tools.ietf.org/html/rfc7519
            // 詳細可用聲明可參考文獻 https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtregisteredclaimnames
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName)); // User.Identity.Name
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

            // NameId 在文獻中並未說明用途，雖範例有使用但可忽略此設定
            //claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userName));

            // 自定義 JWT 內容，套用 "roles" 加入登入者該有的角色
            claims.Add(new Claim("roles", "Root"));

            // 自定義 JWT 內容，套用在 "UserType" 自定義的原則授權處理
            claims.Add(new Claim("UserType", "Admin"));
            claims.Add(new Claim("UserType", "User"));

            // 自定義 JWT 內容，套用在 "accesslevel" 自定義的原則授權處理
            claims.Add(new Claim("accesslevel", "1"));

            // 產生 Cliams 辨識物件
            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            // SecurityTokenDescriptor 是定義 JWT 生成的相關設定，其中將 JWT 的 Claim 會放置在 Subject 屬性
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // 註冊 Token 發送者 ( Issuer )
                Issuer = issuer,
                // 註冊 Token 使用者，由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                Audience = issuer,
                //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
                //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 建立 JWT
            // JwtSecurityTokenHandler  用於產出依據 SecurityTokenDescriptor 定義的 JWT 物件，並取得序列化後的結果 ( 字串格式 )
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
