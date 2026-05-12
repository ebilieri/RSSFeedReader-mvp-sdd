# Implementation Plan: RSS Feed Reader MVP — Subscription Management

**Branch**: `001-rss-mvp-subscriptions` | **Date**: 2026-05-11 | **Spec**: [spec.md](spec.md)  
**Input**: Feature specification from `/specs/001-rss-mvp-subscriptions/spec.md`

## Summary

Implementar o MVP do RSS Feed Reader: um aplicativo web local que permite ao usuário
adicionar URLs de feeds RSS/Atom a uma lista de assinaturas em memória e visualizar
essa lista em tempo real. A solução usa ASP.NET Core Web API como backend (armazenamento
em memória, endpoint REST) e Blazor WebAssembly como frontend (formulário de entrada,
lista de assinaturas, mensagens de feedback). Nenhuma operação de rede ou busca de feed
ocorre nesta fase.

## Technical Context

**Language/Version**: C# / .NET 8 (LTS)  
**Primary Dependencies**: ASP.NET Core Web API (backend), Blazor WebAssembly (frontend) — sem bibliotecas externas adicionais para o MVP  
**Storage**: In-memory (`List<SubscriptionItem>` gerenciado por serviço singleton no backend); dados perdidos ao reiniciar  
**Testing**: Manual (sem testes automatizados no MVP); xUnit adicionado em fase futura  
**Target Platform**: localhost — Windows, macOS, Linux (desenvolvimento local, usuário único)  
**Project Type**: Web application (backend API + frontend SPA)  
**Performance Goals**: Resposta visual < 1 segundo após confirmação de adição (SC-002); carregamento inicial sem requisitos específicos de tempo além de usabilidade local  
**Constraints**: Sem persistência entre sessões; sem operações de rede; CORS explícito obrigatório; portas coordenadas via arquivos de configuração  
**Scale/Scope**: 1 usuário, 1 sessão, armazenamento em memória — escala mínima intencional

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Verificar conformidade com os princípios da constituição antes de prosseguir:

- [x] **I. MVP-First Development** — Feature restrita a gerenciamento de assinaturas (adicionar + listar). Nenhuma funcionalidade de Extended-MVP (busca de feeds, HttpClient, Syndication) ou Post-MVP (persistência, remoção) está no escopo. ✅
- [x] **II. Security by Design** — Entradas validadas no endpoint da API (string não vazia, trim). CORS configurado com origens explícitas (`http://localhost:5213`). Nenhum valor hardcoded — URL base lida de `wwwroot/appsettings.json`. ✅
- [x] **III. Clean Architecture Separation** — Backend: lógica de negócio e armazenamento em memória. Frontend: UI e exibição apenas. Comunicação via REST (POST /api/subscriptions, GET /api/subscriptions). ✅
- [x] **IV. Configuration Over Convention** — Portas definidas em `launchSettings.json` (backend: 5151, frontend: 5213). URL base da API em `wwwroot/appsettings.json`. CORS referencia porta do frontend. ✅
- [x] **V. Simplicity and Maintainability** — Nenhuma biblioteca externa além do framework base. Armazenamento `List<T>` em serviço singleton. Dois componentes de UI (formulário + lista). Zero dependências desnecessárias. ✅

**Resultado**: APROVADO — todos os gates passam. Nenhuma violação identificada.

## Project Structure

### Documentation (this feature)

```text
specs/001-rss-mvp-subscriptions/
├── plan.md              # Este arquivo
├── research.md          # Phase 0 — decisões técnicas e padrões
├── data-model.md        # Phase 1 — modelo de dados
├── quickstart.md        # Phase 1 — guia de execução local
├── contracts/
│   └── api.md           # Phase 1 — contratos REST
└── tasks.md             # Phase 2 — gerado por /speckit.tasks
```

### Source Code (repository root)

```text
backend/
├── RSSFeedReader.Api/
│   ├── Controllers/
│   │   └── SubscriptionsController.cs
│   ├── Models/
│   │   └── SubscriptionItem.cs
│   ├── Services/
│   │   ├── ISubscriptionService.cs
│   │   └── InMemorySubscriptionService.cs
│   ├── Properties/
│   │   └── launchSettings.json      # porta: http://localhost:5151
│   └── Program.cs                   # CORS, DI, routing
│
frontend/
├── RSSFeedReader.UI/
│   ├── Pages/
│   │   └── Subscriptions.razor      # @page "/" — página principal do MVP
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor            # links limpos (sem demo pages)
│   ├── Services/
│   │   └── SubscriptionApiService.cs
│   ├── wwwroot/
│   │   └── appsettings.json         # { "ApiBaseUrl": "http://localhost:5151/api/" }
│   ├── Properties/
│   │   └── launchSettings.json      # porta: http://localhost:5213
│   └── Program.cs                   # HttpClient configurado via appsettings
```

**Structure Decision**: Web application (Option 2) — backend API separado do frontend Blazor WASM.
Limpeza obrigatória das páginas de demonstração do template (`Home.razor`, `Counter.razor`,
`Weather.razor`) antes de qualquer implementação de UI (Princípio I do Development Workflow
da constituição).

## Complexity Tracking

> Nenhuma violação identificada — tabela não aplicável para este plano.

---

## Constitution Check — Post-Design Re-evaluation

*Re-check após Phase 1 (data-model.md + contracts/api.md gerados).*

- [x] **I. MVP-First Development** — `SubscriptionItem` tem apenas `Url`. Sem campos de metadata (data, título, icon). Nenhuma dependência adicionada além do framework base. ✅
- [x] **II. Security by Design** — `AddSubscriptionRequest.Url` anotado com `[Required]`. CORS com `WithOrigins("http://localhost:5213")` — sem wildcard. `ApiBaseUrl` em `wwwroot/appsettings.json` — sem hardcode. ✅
- [x] **III. Clean Architecture Separation** — `SubscriptionApiService` no frontend encapsula todo HTTP. `Subscriptions.razor` contém apenas lógica de UI. `InMemorySubscriptionService` no backend contém toda lógica de negócio. ✅
- [x] **IV. Configuration Over Convention** — 3 arquivos de configuração documentados em [quickstart.md](quickstart.md). Portas em `launchSettings.json`, URL base em `appsettings.json`. ✅
- [x] **V. Simplicity and Maintainability** — 3 modelos (SubscriptionItem, AddSubscriptionRequest, SubscriptionItemDto), 1 interface, 1 serviço, 1 controller, 1 page Blazor. Mínimo para o MVP. ✅

**Resultado pós-design**: APROVADO — todos os gates passam.

---

## Artefatos Gerados

| Fase | Artefato | Status |
|------|----------|--------|
| Phase 0 | [research.md](research.md) | ✅ Completo |
| Phase 1 | [data-model.md](data-model.md) | ✅ Completo |
| Phase 1 | [contracts/api.md](contracts/api.md) | ✅ Completo |
| Phase 1 | [quickstart.md](quickstart.md) | ✅ Completo |
| Phase 2 | tasks.md | ⏳ Pendente — gerar com `/speckit.tasks` |

