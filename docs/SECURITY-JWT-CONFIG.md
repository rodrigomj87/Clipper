# Configuração de Segurança - JWT Settings

## ⚠️ IMPORTANTE - CONFIGURAÇÃO DE PRODUÇÃO

### Variáveis de Ambiente Obrigatórias

Para produção, configure as seguintes variáveis de ambiente:

```bash
# JWT Secret Key (CRÍTICO - Mínimo 256 bits)
JWTSETTINGS__SECRETKEY=sua_chave_super_secreta_de_pelo_menos_32_caracteres_aqui

# JWT Configuration
JWTSETTINGS__ISSUER=ClipperAPI-Production
JWTSETTINGS__AUDIENCE=ClipperClient-Production
JWTSETTINGS__EXPIRATIONINMINUTES=60
JWTSETTINGS__REFRESHTOKENEXPIRATIONINDAYS=7
```

### Como Gerar uma Chave Segura

```powershell
# PowerShell - Gerar chave aleatória de 64 caracteres
-join ((1..64) | ForEach {Get-Random -input ([char[]]([char]'a'..[char]'z') + [char[]]([char]'A'..[char]'Z') + [char[]]([char]'0'..[char]'9'))})

# Ou use um gerador online seguro como: https://randomkeygen.com/
```

### Docker Compose (Exemplo)

```yaml
version: '3.8'
services:
  clipper-api:
    image: clipper-api:latest
    environment:
      - JWTSETTINGS__SECRETKEY=${JWT_SECRET_KEY}
      - JWTSETTINGS__ISSUER=ClipperAPI-Production
      - JWTSETTINGS__AUDIENCE=ClipperClient-Production
      - JWTSETTINGS__EXPIRATIONINMINUTES=60
      - JWTSETTINGS__REFRESHTOKENEXPIRATIONINDAYS=7
    ports:
      - "80:8080"
```

### Azure App Service

Configure nas Application Settings:
- Nome: `JwtSettings__SecretKey`
- Valor: Sua chave secreta

### AWS / Kubernetes

Use Secrets Manager ou Kubernetes Secrets para armazenar a chave.

## 🔐 Desenvolvimento Local

Para desenvolvimento, a chave já está configurada via User Secrets:

```bash
# Verificar secrets configurados
dotnet user-secrets list --project src/Clipper.API

# Adicionar nova secret
dotnet user-secrets set "JwtSettings:SecretKey" "sua_nova_chave" --project src/Clipper.API
```

## ✅ Validação de Segurança

O sistema validará automaticamente:
- ✅ Presença da chave secreta
- ✅ Tamanho mínimo de 256 bits (32 caracteres)
- ✅ Configuração completa de Issuer e Audience

Se alguma validação falhar, a aplicação **não iniciará**.
