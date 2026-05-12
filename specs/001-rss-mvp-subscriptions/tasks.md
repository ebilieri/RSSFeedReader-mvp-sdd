# Tasks: RSS Feed Reader MVP — Subscription Management

**Input**: Design documents from `/specs/001-rss-mvp-subscriptions/`  
**Prerequisites**: [plan.md](plan.md) · [spec.md](spec.md) · [research.md](research.md) · [data-model.md](data-model.md) · [contracts/api.md](contracts/api.md) · [quickstart.md](quickstart.md)

**Tests**: Não solicitados na especificação — nenhuma tarefa de teste automatizado incluída.

**Organization**: Tarefas agrupadas por user story para permitir implementação e teste independentes.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem dependências incompletas)
- **[Story]**: A qual user story a tarefa pertence (US1, US2)
- Caminhos exatos de arquivo incluídos em todas as descrições

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Inicialização dos dois projetos .NET e estrutura de diretórios

- [x] T001 Criar projeto backend `backend/RSSFeedReader.Api` via `dotnet new webapi` em `backend/`
- [x] T002 Criar projeto frontend `frontend/RSSFeedReader.UI` via `dotnet new blazorwasm` em `frontend/`

**Checkpoint**: Dois projetos criados e executáveis com `dotnet run` em seus respectivos diretórios

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Infraestrutura de configuração e integração que PRECISA estar completa antes de qualquer user story

**⚠️ CRÍTICO**: Nenhuma tarefa de user story pode começar até que esta fase esteja completa

- [x] T003 Configurar porta 5151 em `backend/RSSFeedReader.Api/Properties/launchSettings.json` (substituir porta padrão, perfil `http` apenas)
- [x] T004 [P] Configurar porta 5213 em `frontend/RSSFeedReader.UI/Properties/launchSettings.json` (substituir porta padrão, perfil `http` apenas)
- [x] T005 Criar `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` com `{ "ApiBaseUrl": "http://localhost:5151/api/" }`
- [x] T006 Configurar CORS em `backend/RSSFeedReader.Api/Program.cs`: policy `AllowBlazorFrontend` com `.WithOrigins("http://localhost:5213")` — sem wildcards
- [x] T007 Configurar `HttpClient` DI em `frontend/RSSFeedReader.UI/Program.cs`: ler `ApiBaseUrl` de `builder.Configuration["ApiBaseUrl"]` e registrar `HttpClient` com `BaseAddress`
- [x] T008 Limpar páginas de demonstração do template Blazor: remover `frontend/RSSFeedReader.UI/Pages/Home.razor`, `Counter.razor`, `Weather.razor` e limpar links de demonstração de `frontend/RSSFeedReader.UI/Layout/NavMenu.razor`

**Checkpoint**: Backend roda em `http://localhost:5151`, frontend em `http://localhost:5213`; frontend consegue fazer requests ao backend sem erros de CORS

---

## Phase 3: User Story 1 - Add Feed Subscription (Priority: P1) 🎯 MVP

**Goal**: Permitir que o usuário insira uma URL no campo de entrada, confirme e veja a assinatura adicionada imediatamente na lista, com campo limpo e mensagem de confirmação temporária.

**Independent Test**: Abrir o app em `http://localhost:5213`, digitar qualquer string não vazia no campo, clicar "Adicionar" — a URL deve aparecer na lista, o campo deve limpar e a mensagem "Assinatura adicionada!" deve aparecer por alguns segundos. Tentar submeter campo vazio deve exibir "Insira uma URL" sem adicionar item.

### Implementation for User Story 1

- [ ] T009 [P] [US1] Criar `backend/RSSFeedReader.Api/Models/SubscriptionItem.cs` com propriedade `public required string Url { get; set; }`
- [ ] T010 [P] [US1] Criar `backend/RSSFeedReader.Api/Models/AddSubscriptionRequest.cs` com `[Required] public required string Url { get; set; }`
- [ ] T011 [US1] Criar `backend/RSSFeedReader.Api/Services/ISubscriptionService.cs` com métodos `IReadOnlyList<SubscriptionItem> GetAll()` e `SubscriptionItem Add(string url)`
- [ ] T012 [US1] Criar `backend/RSSFeedReader.Api/Services/InMemorySubscriptionService.cs` implementando `ISubscriptionService` com `List<SubscriptionItem>` + lock de thread (conforme data-model.md)
- [ ] T013 [US1] Registrar `InMemorySubscriptionService` como singleton em `backend/RSSFeedReader.Api/Program.cs`: `builder.Services.AddSingleton<ISubscriptionService, InMemorySubscriptionService>()`
- [ ] T014 [US1] Criar `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs` com atributos `[ApiController]` e `[Route("api/[controller]")]` na classe, injetar `ISubscriptionService` via construtor — implementar `POST /api/subscriptions` (retorna `CreatedAtAction` 201 + item criado; 400 se URL vazia após trim via `[Required]`) e `GET /api/subscriptions` (retorna `Ok(service.GetAll())` 200 + array)
- [ ] T015 [P] [US1] Criar `frontend/RSSFeedReader.UI/Models/SubscriptionItemDto.cs` com `public string Url { get; set; } = string.Empty`
- [ ] T016 [US1] Criar `frontend/RSSFeedReader.UI/Services/SubscriptionApiService.cs` com `AddAsync(string url)` (POST) e `GetAllAsync()` (GET) usando `HttpClient` injetado
- [ ] T017 [US1] Registrar `SubscriptionApiService` em `frontend/RSSFeedReader.UI/Program.cs`: `builder.Services.AddScoped<SubscriptionApiService>()`
- [ ] T018 [US1] Criar `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` com diretiva `@page "/"`, heading `<h1>Minhas Assinaturas</h1>`, injetar `SubscriptionApiService`, declarar variáveis de estado: `newUrl` (string), `subscriptions` (List<SubscriptionItemDto>), `successMessage` (string), `errorMessage` (string), `isLoading` (bool — `true` durante chamadas HTTP; botão "Adicionar" fica `disabled` enquanto `isLoading == true`)
- [ ] T019 [US1] Implementar formulário em `Subscriptions.razor`: label "URL do Feed", campo `<input>` com `@bind="newUrl"`, botão `<button disabled="@isLoading">Adicionar</button>` com handler `OnAddSubmit` — handler: (1) valida campo vazio/espaços → exibe `errorMessage = "Insira uma URL"` e retorna (FR-008); (2) define `isLoading = true`, limpa `errorMessage`; (3) chama `AddAsync`, adiciona item retornado à lista local, limpa `newUrl` (FR-006); (4) define `isLoading = false`, exibe `successMessage = "Assinatura adicionada!"` por 3s via `Task.Delay(3000)` + `StateHasChanged` (FR-007); `errorMessage` é limpa no início de cada nova submissão (passo 2)
- [ ] T020 [US1] Adicionar `OnInitializedAsync` em `Subscriptions.razor` para carregar lista inicial via `GetAllAsync()` ao abrir a página — em caso de exceção (backend inacessível), capturar `HttpRequestException`, exibir `errorMessage = "Não foi possível conectar ao servidor. Verifique se o backend está em execução."` e inicializar `subscriptions` como lista vazia (sem crash da página)

**Checkpoint**: US1 completamente funcional — adicionar URL, ver na lista, campo limpo, mensagem de confirmação, erro para campo vazio. Testável de forma independente.

---

## Phase 4: User Story 2 - View Subscription List (Priority: P2)

**Goal**: Exibir a lista de assinaturas da sessão de forma clara, com estado vazio explícito e preservação da ordem de inserção.

**Independent Test**: Abrir o app sem adicionar nada — deve exibir estado vazio sem erros. Adicionar 3 URLs diferentes — todas devem aparecer na ordem exata de inserção, exibidas como URL bruta sem elementos adicionais. Reiniciar o backend e reabrir o app — lista deve começar vazia.

### Implementation for User Story 2

- [ ] T021 [US2] Adicionar estado vazio em `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`: renderizar mensagem de estado vazio (ex.: "Nenhuma assinatura adicionada ainda.") quando `subscriptions` estiver vazia — sem erros visuais (SC-005)
- [ ] T022 [US2] Verificar e ajustar renderização da lista em `Subscriptions.razor`: cada item exibe apenas `item.Url` como texto puro, sem número sequencial, ícone ou elementos adicionais (clarification 3 do spec.md)

**Checkpoint**: US1 + US2 funcionam de forma independente. Estado vazio, lista populada e order de inserção todos verificáveis.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Melhorias que afetam toda a aplicação

- [ ] T023 [P] Atualizar `frontend/RSSFeedReader.UI/Layout/NavMenu.razor`: garantir que apenas o link para a página de Assinaturas (`/`) está presente; remover quaisquer links de template remanescentes
- [ ] T024 Executar validação do quickstart.md: iniciar backend (`cd backend/RSSFeedReader.Api && dotnet run`), iniciar frontend (`cd frontend/RSSFeedReader.UI && dotnet run`), abrir `http://localhost:5213`, verificar todos os cenários de aceitação do spec.md (US1 + US2)

**Checkpoint final**: Aplicativo completo e funcional conforme [quickstart.md](quickstart.md)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sem dependências — pode iniciar imediatamente
- **Foundational (Phase 2)**: Depende da conclusão do Setup — **BLOQUEIA** todas as user stories
- **US1 (Phase 3)**: Depende da conclusão do Foundational — pode iniciar após Phase 2
- **US2 (Phase 4)**: Depende da conclusão de US1 (usa mesma página e lista em memória)
- **Polish (Phase 5)**: Depende de US1 + US2 completos

### User Story Dependencies

- **User Story 1 (P1)**: Inicia após Phase 2 — sem dependência de outras user stories
- **User Story 2 (P2)**: Inicia após US1 — adiciona empty state e verificação de ordenação sobre a infraestrutura criada em US1

### Within User Story 1

- T009 e T010 (modelos) podem ser feitos em paralelo entre si e com T015 (DTO frontend)
- T011 (interface) antes de T012 (implementação)
- T012 antes de T013 (registro no DI)
- T013 antes de T014 (controller usa service via DI)
- T016 (SubscriptionApiService) antes de T017 (registro) e T018 (razor page)
- T018 antes de T019 e T020 (setup do componente antes dos handlers)
- T020 pode ser feito junto com T019

### Parallel Opportunities

- T003 e T004 (launchSettings.json de cada projeto) podem rodar em paralelo
- T009, T010 e T015 (modelos backend + DTO frontend) podem rodar em paralelo
- T023 (NavMenu) pode rodar em paralelo com T024 (validação)

---

## Parallel Example: User Story 1

```bash
# Terminal 1 — modelos backend
cd backend/RSSFeedReader.Api/Models
# Criar SubscriptionItem.cs (T009)

# Terminal 2 — DTO frontend (paralelo com T009)
cd frontend/RSSFeedReader.UI/Models
# Criar SubscriptionItemDto.cs (T015)

# Terminal 1 — após T009
# Criar AddSubscriptionRequest.cs (T010)

# Sequencial: T011 → T012 → T013 → T014
# Sequencial: T016 → T017 → T018 → T019 → T020
```

---

## Implementation Strategy

**MVP Scope (recomendado)**: Phases 1 + 2 + 3 = entrega de US1 funcional  
**Full Scope**: Phases 1–5 = ambas as user stories + polish

**Entrega incremental sugerida**:
1. Phase 1 + 2: infraestrutura funcional (backend e frontend rodando e conectados)
2. Phase 3: US1 completo e testável (valor central do MVP entregue)
3. Phase 4: US2 (empty state + verificação de ordem)
4. Phase 5: polish e validação final
