# Research: RSS Feed Reader MVP — Subscription Management

**Phase**: 0 — Pre-Design Research  
**Date**: 2026-05-11  
**Feature**: [spec.md](spec.md) | **Plan**: [plan.md](plan.md)

## Objetivo

Resolver todos os pontos marcados como NEEDS CLARIFICATION no Technical Context
e documentar as decisões técnicas que guiarão o design da Phase 1.

---

## Decisão 1: Versão do .NET

**Decision**: .NET 8 (LTS)

**Rationale**: Versão LTS ativa com suporte estendido até novembro de 2026. Compatível
com ASP.NET Core Web API e Blazor WebAssembly standalone. Templates disponíveis via
`dotnet new webapi` e `dotnet new blazorwasm`. Sem dependências de versão específica
nos documentos de stakeholders.

**Alternatives considered**:
- .NET 9 (current): descartado — versão non-LTS, menor estabilidade de longo prazo
- .NET 6: descartado — LTS expirado em novembro de 2024

---

## Decisão 2: Estrutura de projetos

**Decision**: Dois projetos separados — `backend/RSSFeedReader.Api` (Web API) e
`frontend/RSSFeedReader.UI` (Blazor WASM). Sem projeto compartilhado de modelos
para o MVP.

**Rationale**: Separação obrigatória pelo Princípio III da constituição. Para o MVP,
o único modelo (`SubscriptionItem`) é tão simples (uma propriedade: URL) que um
projeto shared seria overhead desnecessário (viola Princípio V — YAGNI). O frontend
usará um DTO local simples para deserialização da resposta da API.

**Alternatives considered**:
- Projeto shared `RSSFeedReader.Shared`: descartado para MVP — adiciona complexidade
  sem benefício real quando há apenas um modelo com uma propriedade
- Blazor Server (hosted): descartado — a arquitetura especificada é WASM standalone
  com API separada (TechStack.md)

---

## Decisão 3: Padrão de armazenamento em memória

**Decision**: Serviço singleton `InMemorySubscriptionService` registrado no container
de DI do ASP.NET Core, contendo `List<SubscriptionItem>` com lock de thread
(objeto `lock` ou `List` sincronizada).

**Rationale**: Singleton garante que todos os requests compartilhem a mesma lista
durante a sessão. Lock necessário porque o ASP.NET Core processa requests
concorrentemente por padrão — sem lock, múltiplos `POST` simultâneos poderiam
corromper a lista. Para MVP com um usuário local, o overhead é desprezível.

**Alternatives considered**:
- `ConcurrentBag<T>`: descartado — não preserva ordem de inserção, violando SC (lista
  ordenada por inserção)
- `IMemoryCache`: descartado — overkill para este caso; adiciona dependência sem
  benefício tangível no MVP

---

## Decisão 4: Design do endpoint REST

**Decision**: Dois endpoints mínimos:
- `POST /api/subscriptions` — corpo: `{ "url": "string" }`, retorna `201 Created`
  com o item criado
- `GET /api/subscriptions` — retorna `200 OK` com array `[{ "url": "string" }]`

Validação no backend: URL não pode ser nula ou vazia (trim). Retorna `400 Bad Request`
com mensagem de erro se a validação falhar.

**Rationale**: REST mínimo suficiente para US1 (adicionar) e US2 (listar). FR-001
e FR-002 atendidos. Retorno `201` com o item criado permite ao frontend confirmar
a adição sem re-fetch imediato. FR-008 (validação de campo vazio) é tratado tanto
no frontend (UX) quanto no backend (segurança — Princípio II).

**Alternatives considered**:
- Endpoint único com verbo customizado: descartado — viola convenções REST e
  dificulta manutenção
- `PUT /api/subscriptions` para adicionar: descartado — semântica incorreta
  (PUT = idempotente, replace); POST é correto para criar

---

## Decisão 5: Comunicação frontend ↔ backend

**Decision**: `HttpClient` injetado via DI no Blazor WASM, com `BaseAddress` lida
de `wwwroot/appsettings.json["ApiBaseUrl"]`. Classe de serviço dedicada
`SubscriptionApiService` encapsula as chamadas HTTP.

**Rationale**: Princípio IV obriga configuração via arquivo externo. Encapsular
em `SubscriptionApiService` mantém o componente `Subscriptions.razor` livre de
lógica HTTP (Princípio III — frontend limitado a UI).

**Pattern**:
```csharp
// Program.cs (frontend)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5151/api/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<SubscriptionApiService>();
```

**Alternatives considered**:
- URL hardcoded no `Program.cs`: descartado — viola Princípio IV e II
- Refit ou RestSharp: descartado — bibliotecas externas sem justificativa para
  dois endpoints simples (viola Princípio V)

---

## Decisão 6: Feedback visual de sucesso e erro (FR-007, FR-008)

**Decision**: Variáveis de estado em `Subscriptions.razor` controlam visibilidade
de mensagens inline (sem biblioteca de toasts/notifications):
- `successMessage` (string): exibida após adição bem-sucedida, limpa após 3 segundos
  via `Task.Delay` + `StateHasChanged`
- `errorMessage` (string): exibida quando campo vazio é submetido, limpa quando
  usuário começa a digitar

**Rationale**: Princípio V — implementar apenas o necessário. Mensagens inline em
Blazor não requerem bibliotecas externas. `Task.Delay` + `StateHasChanged` é o
padrão idiomático Blazor para feedback temporário.

**Alternatives considered**:
- MudBlazor ou Blazorise (componentes de Snackbar): descartado — overhead
  significativo de dependência para dois tipos de mensagem simples
- JavaScript interop para toasts: descartado — adiciona complexidade
  desnecessária ao MVP

---

## Decisão 7: Configuração de portas

**Decision**:
- Backend: `http://localhost:5151` (definido em `backend/RSSFeedReader.Api/Properties/launchSettings.json`)
- Frontend: `http://localhost:5213` (definido em `frontend/RSSFeedReader.UI/Properties/launchSettings.json`)
- `ApiBaseUrl` no `wwwroot/appsettings.json`: `"http://localhost:5151/api/"`
- CORS no backend: `.WithOrigins("http://localhost:5213")`

**Rationale**: Portas não-padrão evitam conflitos com outros serviços locais comuns
(3000, 8080, 5000). Valores escolhidos conforme exemplos da documentação de stakeholders
(TechStack.md). Princípio IV: todas as portas em arquivos de configuração, nunca no código.

**Alternatives considered**:
- Porta 5000/5001 (padrão .NET): descartado — conflito frequente com outros projetos
  em desenvolvimento local

---

## Unknowns resolvidos

| Unknown original | Resolução |
|---|---|
| Versão .NET | .NET 8 LTS |
| Estrutura de projetos | 2 projetos: backend/RSSFeedReader.Api + frontend/RSSFeedReader.UI |
| Padrão de armazenamento | Singleton com List<T> + lock |
| Design de endpoints | POST + GET /api/subscriptions com validação |
| Comunicação frontend↔backend | HttpClient + SubscriptionApiService + appsettings.json |
| Feedback visual | Variáveis de estado inline, sem bibliotecas externas |
| Portas | Backend: 5151, Frontend: 5213 |

**Nenhum NEEDS CLARIFICATION permanece. Phase 1 pode prosseguir.**
