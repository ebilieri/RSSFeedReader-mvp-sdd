# Tech stack for RSS Feed Reader

Nosso leitor de RSS usará um backend ASP.NET Core Web API e um frontend Blazor WebAssembly. Essa combinação permite o desenvolvimento rápido do MVP enquanto suporta aprimoramentos futuros prontos para produção.

## Why ASP.NET Core Web API + Blazor WebAssembly?

Construir um leitor de RSS com um backend **ASP.NET Core Web API** e um frontend **Blazor WebAssembly** oferece várias vantagens:

1. **Quick Development**: Ambas as tecnologias funcionam bem juntas com configuração mínima, permitindo o desenvolvimento rápido da demonstração.

2. **Separation of Concerns**: O backend lida com o gerenciamento de dados e (no Extended-MVP) operações de feed, enquanto o frontend se concentra na interação do usuário.

3. **Cross-Platform**: Tanto ASP.NET Core quanto Blazor são multiplataforma, permitindo que o aplicativo seja executado no Windows, macOS e Linux.

4. **Incremental Complexity**: Comece com o gerenciamento simples de assinaturas (MVP), depois adicione busca de feeds (Extended-MVP), depois persistência e recursos avançados.

5. **Future-Ready Architecture**: Enquanto o MVP é mínimo (apenas gerenciamento de lista de assinaturas), esta arquitetura suporta a adição de:

   - Busca e análise de feeds (`System.ServiceModel.Syndication`)
   - Persistência em banco de dados (EF Core + SQLite)
   - Processamento em segundo plano (`BackgroundService` para polling)
   - Recursos avançados (lido/não lido, pastas, etc.)

6. **Shared Code**: O Blazor WebAssembly usa C#, permitindo compartilhamento de código entre frontend e backend se necessário.

## Responsibilities

Para o MVP (somente gerenciamento de assinaturas):

**Backend** é responsável por:

- Expor uma API para adicionar assinaturas
- Armazenar assinaturas em memória
- Retornar a lista de assinaturas

**Frontend** é responsável por:

- Interface de gerenciamento de assinaturas (campo de entrada + botão adicionar)
- Exibir a lista de assinaturas

Para o Extended-MVP (adicionar busca de feeds):

**Backend** adiciona:

- Busca e análise de feeds RSS/Atom quando solicitado
- Retornar itens de feed para a interface

**Frontend** adiciona:

- Botão de atualização manual
- Exibir itens (título e link no mínimo)
- Mensagens básicas de erro

## MVP-first implementation approach

Para entregar o MVP rapidamente:

**MVP (subscription management only):**

- **Storage**: Usar armazenamento em memória (List<string> ou modelo simples). As assinaturas são perdidas quando o aplicativo para.
- **No feed operations**: Sem cliente HTTP, sem biblioteca de análise, sem busca de feeds
- **Focus**: Interface básica e comunicação com a API (adicionar assinatura, obter lista de assinaturas)

**Extended-MVP (add feed fetching):**

- **Parsing**: Adicionar `System.ServiceModel.Syndication` para análise básica de RSS/Atom
- **HTTP client**: Adicionar HttpClient para busca de feeds
- **Refresh**: Somente manual - sem polling ou agendamento em segundo plano
- **Error handling**: Mensagens simples de "failed to load", sem diagnósticos detalhados
- **Content display**: Apenas texto simples (título + link), sem renderização HTML necessária

Essa abordagem incremental torna o desenvolvimento extremamente rápido enquanto mantém a arquitetura limpa para aprimoramentos futuros.

## Local development

### Blazor project initialization

Ao criar um novo projeto Blazor WebAssembly a partir do template, o projeto inclui páginas de demonstração que devem ser removidas para evitar conflitos com os recursos do MVP.

**⚠️ CRÍTICO: Esta limpeza deve ser concluída na Fase 2 (Foundational) e VERIFICADA antes do início de qualquer implementação de recursos de interface. Erros em tempo de execução por limpeza incompleta desperdiçarão tempo de desenvolvimento.**

**Required cleanup steps:**

1. **Remove template demo pages** de `frontend/[ProjectName].UI/Pages/`:
   - Excluir `Home.razor` (conflita com a rota raiz)
   - Excluir `Counter.razor` (página de demonstração)
   - Excluir `Weather.razor` (página de demonstração)

2. **Update navigation menu** em `frontend/[ProjectName].UI/Layout/NavMenu.razor`:
   - Remover links de navegação para as páginas de demonstração excluídas
   - Atualizar itens de menu para refletir apenas os recursos do MVP
   - Alterar o texto do link de navegação raiz para corresponder à sua página principal (ex.: "Subscriptions")

3. **Verify routing**:
   - Garantir que apenas UMA página use a diretiva `@page "/"` (sua página principal do MVP)
   - Todas as outras páginas devem usar rotas únicas (ex.: `@page "/settings"`)

4. **Verify cleanup completion** antes de prosseguir com a implementação:

   ```powershell
   # List all Razor pages - should show ONLY your MVP pages (e.g., NotFound.razor, Subscriptions.razor)
   Get-ChildItem frontend/[ProjectName].UI/Pages/ -Filter *.razor | Select-Object Name
   ```

   **PARAR: Não prosseguir com a implementação de recursos até:**
   - ✗ Home.razor foi REMOVIDO
   - ✗ Counter.razor foi REMOVIDO  
   - ✗ Weather.razor foi REMOVIDO
   - ✓ Apenas suas páginas do MVP permanecem

5. **Test for routing conflicts immediately** após a limpeza:

   ```powershell
   # Clean build to remove cached assemblies
   dotnet clean frontend/[ProjectName].UI/[ProjectName].UI.csproj
   dotnet build frontend/[ProjectName].UI/[ProjectName].UI.csproj
   
   # Start frontend to verify no routing errors
   dotnet run --project frontend/[ProjectName].UI
   ```

   Navegue para a URL do frontend no seu navegador. Se você vir um erro de "ambiguous route" no console do navegador (F12 Developer Tools), a limpeza está incompleta. **Corrija o problema antes de implementar qualquer recurso.**

**Por que isso importa:**

Os templates do Blazor incluem páginas de demonstração com rotas pré-configuradas. Se você criar novas páginas com as mesmas rotas (especialmente a rota raiz `/`), você encontrará **ambiguous route exceptions** em tempo de execução. A mensagem de erro será semelhante a:

```
System.InvalidOperationException: The following routes are ambiguous:
'' in '[ProjectName].UI.Pages.Home'
'' in '[ProjectName].UI.Pages.YourFeature'
```

Esses erros só aparecem em tempo de execução após você já ter implementado recursos, tornando-os custosos para depurar. As etapas de verificação acima detectam esse problema imediatamente durante a limpeza da Fase 2, antes de qualquer trabalho de recurso começar.

Limpar páginas de template antes de implementar recursos do MVP previne esses conflitos e garante uma estrutura de projeto limpa focada em requisitos de negócio.

### Port configuration

O backend da API e o frontend executam em portas localhost separadas. **Port consistency is critical** - as portas devem ser coordenadas entre três locais:

1. **Backend port** (definida em `backend/RSSFeedReader.Api/Properties/launchSettings.json`):

   - Padrão: `http://localhost:5151`
   - Este é onde a API escuta as requisições

2. **Frontend port** (definida em `frontend/RSSFeedReader.UI/Properties/launchSettings.json`):

   - Padrão: `http://localhost:5213`
   - Este é onde o aplicativo Blazor executa

3. **API base URL** (configurada em `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`):

   - Deve corresponder à porta do backend do passo 1
   - Exemplo: `{"ApiBaseUrl": "http://localhost:5151/api/"}`

4. **CORS policy** (configurada em `backend/RSSFeedReader.Api/Program.cs`):

   - Deve permitir a porta do frontend do passo 2
   - Exemplo: `.WithOrigins("http://localhost:5213", "https://localhost:7025")`

### Configuration best practices

- **Frontend Program.cs**: Ler a URL da API a partir da configuração, não codificar diretamente:

  ```csharp
  var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5151/api/";
  builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
  ```

- **Backend CORS**: Permitir as portas reais do frontend definidas no launchSettings.json

- **Testing setup**: Antes de testar, verificar:

  1. O backend está em execução e acessível na porta configurada
  2. O appsettings.json do frontend aponta para a porta correta do backend
  3. O CORS permite a origem do frontend

**For MVP:** Testar adicionando URLs de assinaturas e verificando se aparecem na lista.

**For Extended-MVP:** Testar com um feed conhecido como <https://devblogs.microsoft.com/dotnet/feed/>

## Future enhancements (post-MVP)

Quando estiver pronto para estender além da demonstração básica, esta arquitetura suporta:

- **Database persistence**: Adicionar EF Core + SQLite para armazenar assinaturas e itens entre sessões
- **Background polling**: Implementar `BackgroundService` para atualizar feeds automaticamente em um agendamento
- **HTML sanitization**: Adicionar biblioteca `HtmlSanitizer` para exibir com segurança conteúdo rico de feeds
- **Website-to-feed discovery**: Usar `HtmlAgilityPack` para encontrar URLs de feeds a partir de links de sites
- **Better error handling**: Implementar lógica de retry, timeouts e mensagens de erro detalhadas
- **Testing**: Adicionar testes unitários e de integração usando xUnit
- **Optimization**: Implementar cache HTTP (ETag/Last-Modified), deduplicação e melhorias de desempenho

## Summary

ASP.NET Core Web API com Blazor WebAssembly fornece um caminho direto para construir o leitor de RSS de forma incremental:

- **MVP**: Somente gerenciamento de assinaturas (adicionar + listar) - extremamente simples, sem operações de feed
- **Extended-MVP**: Adicionar busca de feeds e exibição de itens - ainda simples com armazenamento em memória e atualização manual
- **Future**: Adicionar persistência, processamento em segundo plano e recursos avançados

A arquitetura é intencionalmente mínima para permitir desenvolvimento rápido, enquanto as escolhas tecnológicas suportam a adição de recursos prontos para produção posteriormente sem exigir uma reescrita completa.
