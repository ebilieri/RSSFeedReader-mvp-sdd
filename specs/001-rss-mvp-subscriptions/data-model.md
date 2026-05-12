# Data Model: RSS Feed Reader MVP — Subscription Management

**Phase**: 1 — Design  
**Date**: 2026-05-11  
**Feature**: [spec.md](spec.md) | **Research**: [research.md](research.md)

---

## Entidades

### SubscriptionItem (Backend — `backend/RSSFeedReader.Api/Models/SubscriptionItem.cs`)

Representa uma assinatura de feed RSS/Atom adicionada pelo usuário.

| Campo | Tipo | Obrigatório | Validação | Notas |
|-------|------|-------------|-----------|-------|
| `Url` | `string` | Sim | Não nulo, não vazio após trim | Qualquer string não vazia aceita — sem validação de formato de URL (FR-004) |

```csharp
namespace RSSFeedReader.Api.Models;

public class SubscriptionItem
{
    public required string Url { get; set; }
}
```

**Notas**:
- Sem ID gerado pelo servidor (MVP — lista não precisa de identificação individual)
- Sem metadata (data de adição, título, etc.) — fora do escopo do MVP
- Representação na lista: URL bruta como texto, sem elementos adicionais (clarificação 2)

---

### AddSubscriptionRequest (Backend — DTO de entrada no endpoint POST)

DTO que representa o body da requisição de adição de assinatura.

| Campo | Tipo | Obrigatório | Validação |
|-------|------|-------------|-----------|
| `Url` | `string` | Sim | Não nulo, não vazio após trim |

```csharp
namespace RSSFeedReader.Api.Models;

public class AddSubscriptionRequest
{
    [Required]
    public required string Url { get; set; }
}
```

---

### SubscriptionItemDto (Frontend — `frontend/RSSFeedReader.UI/Models/SubscriptionItemDto.cs`)

DTO local no frontend para deserialização da resposta da API. Não compartilha assembly
com o backend (decisão de research: sem projeto shared no MVP).

| Campo | Tipo | Notas |
|-------|------|-------|
| `url` | `string` | Camelcase (serialização JSON padrão do ASP.NET Core) |

```csharp
namespace RSSFeedReader.UI.Models;

public class SubscriptionItemDto
{
    public string Url { get; set; } = string.Empty;
}
```

---

## Armazenamento em memória

O backend mantém a coleção de assinaturas em um serviço singleton:

```csharp
// ISubscriptionService.cs
namespace RSSFeedReader.Api.Services;

public interface ISubscriptionService
{
    IReadOnlyList<SubscriptionItem> GetAll();
    SubscriptionItem Add(string url);
}

// InMemorySubscriptionService.cs
namespace RSSFeedReader.Api.Services;

public class InMemorySubscriptionService : ISubscriptionService
{
    private readonly List<SubscriptionItem> _subscriptions = [];
    private readonly object _lock = new();

    public IReadOnlyList<SubscriptionItem> GetAll()
    {
        lock (_lock) return _subscriptions.AsReadOnly();
    }

    public SubscriptionItem Add(string url)
    {
        var item = new SubscriptionItem { Url = url.Trim() };
        lock (_lock) _subscriptions.Add(item);
        return item;
    }
}
```

**Características**:
- Ordem de inserção preservada (FR-003, SC-003 implícito)
- Thread-safe via `lock` (Decisão 3 do research.md)
- Dados perdidos ao reiniciar o backend (MVP — sem persistência)
- Sem limite de tamanho

---

## Regras de negócio (validação)

| Regra | Onde validada | Comportamento |
|-------|--------------|---------------|
| URL não pode ser vazia | Backend (endpoint POST) + Frontend (submit handler) | Backend: 400 Bad Request; Frontend: mensagem de erro exibida inline |
| URL duplicada aceita | — | Nenhuma verificação — duplicatas permitidas (FR-004, assumption) |
| Qualquer string não vazia aceita | — | Sem validação de formato de URL (FR-004) |

---

## Estado do componente Blazor (`Subscriptions.razor`)

O componente mantém estado local de UI (não persistido, não enviado para a API):

| Variável | Tipo | Propósito |
|----------|------|-----------|
| `newUrl` | `string` | Valor atual do campo de entrada (bind com `@bind`) |
| `subscriptions` | `List<SubscriptionItemDto>` | Cache local da lista retornada pela API |
| `successMessage` | `string` | Texto da mensagem de confirmação temporária (FR-007) |
| `errorMessage` | `string` | Texto da mensagem de erro para campo vazio (FR-008) |
| `isLoading` | `bool` | Flag de loading durante chamadas HTTP |

---

## Relacionamentos

```
[Usuário]
    │
    │ digita URL no campo
    ▼
[Subscriptions.razor] ──POST /api/subscriptions──► [SubscriptionsController]
    │                                                       │
    │◄────201 Created + SubscriptionItem────────────────────┤
    │                                                       │
    │ atualiza lista local                         [InMemorySubscriptionService]
    │                                                  List<SubscriptionItem>
    │
    │◄────GET /api/subscriptions (carregamento inicial)─────┘
```

*O frontend usa o item retornado pelo POST para atualizar a lista local sem
necessidade de re-fetch (otimização simples compatível com o MVP).*
