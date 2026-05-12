# Quickstart: RSS Feed Reader MVP — Execução Local

**Data**: 2026-05-11  
**Feature**: [spec.md](spec.md)

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) instalado (`dotnet --version` deve retornar `8.x.x`)
- Terminal (PowerShell, bash, zsh)
- Dois terminais abertos simultaneamente (um para backend, um para frontend)

---

## 1. Estrutura de portas

Antes de iniciar, confirme que os três arquivos de configuração estão sincronizados:

| Arquivo | Chave | Valor |
|---------|-------|-------|
| `backend/RSSFeedReader.Api/Properties/launchSettings.json` | `applicationUrl` | `http://localhost:5151` |
| `frontend/RSSFeedReader.UI/Properties/launchSettings.json` | `applicationUrl` | `http://localhost:5213` |
| `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` | `ApiBaseUrl` | `http://localhost:5151/api/` |

---

## 2. Iniciar o backend

**Terminal 1** — na raiz do repositório:

```bash
cd backend/RSSFeedReader.Api
dotnet run
```

Saída esperada:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5151
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Verificar** (opcional):

```bash
curl http://localhost:5151/api/subscriptions
# Esperado: []
```

---

## 3. Iniciar o frontend

**Terminal 2** — na raiz do repositório:

```bash
cd frontend/RSSFeedReader.UI
dotnet run
```

Saída esperada:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5213
```

Abra o navegador em: **http://localhost:5213**

---

## 4. Usar a aplicação

1. Na página principal, você verá o campo "URL do Feed" e o botão "Adicionar"
2. Digite qualquer string no campo (ex: `https://feeds.example.com/rss`)
3. Clique em "Adicionar" ou pressione Enter
4. A mensagem "Assinatura adicionada!" aparece brevemente e a URL entra na lista
5. O campo de entrada é limpo automaticamente após a adição
6. Tente submeter o campo vazio — a mensagem "Campo URL é obrigatório" é exibida

---

## 5. Verificar a conexão backend ↔ frontend

Se a lista não carregar ou as adições não funcionarem, verifique:

1. **Backend rodando?** — acesse `http://localhost:5151/api/subscriptions` no navegador
2. **CORS correto?** — abra o console do navegador (F12) e verifique erros de CORS
3. **URL base correta?** — confirme `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`:
   ```json
   {
     "ApiBaseUrl": "http://localhost:5151/api/"
   }
   ```
4. **Ports em uso?** — se outra aplicação estiver nas portas 5151 ou 5213, altere os
   `launchSettings.json` e atualize `appsettings.json` e a config de CORS no backend

---

## 6. Parar a aplicação

Em cada terminal, pressione `Ctrl+C`.

Os dados da sessão são perdidos ao parar o backend (armazenamento em memória — MVP).

---

## Nota sobre limpeza do template Blazor

Antes de criar os componentes de UI, execute a limpeza obrigatória do template Blazor:

- Remover `Pages/Home.razor`, `Pages/Counter.razor`, `Pages/Weather.razor`
- Limpar links desnecessários em `Layout/NavMenu.razor`
- Verificar que `Pages/Subscriptions.razor` usa `@page "/"`

Esta limpeza é pré-requisito definido no Development Workflow da constituição.
