# API Contract: RSS Feed Reader MVP — Subscription Management

**Version**: 1.0.0  
**Date**: 2026-05-11  
**Base URL**: `http://localhost:5151/api`  
**Feature**: [spec.md](../spec.md) | **Data Model**: [data-model.md](../data-model.md)

---

## Overview

A API do backend expõe dois endpoints REST para gerenciamento de assinaturas em memória.
Não há autenticação, paginação ou versionamento de API no MVP.

**Content-Type**: `application/json`  
**Encoding**: UTF-8  
**CORS**: Configurado para aceitar requisições de `http://localhost:5213` apenas

---

## Endpoints

### POST /api/subscriptions

Adiciona uma nova assinatura de feed à lista em memória.

**Request**

```http
POST /api/subscriptions
Content-Type: application/json

{
  "url": "https://example.com/feed.rss"
}
```

| Campo | Tipo | Obrigatório | Validação |
|-------|------|-------------|-----------|
| `url` | string | Sim | Não nulo; não vazio após trim |

**Response — 201 Created**

Retornado quando a assinatura é adicionada com sucesso.

```http
HTTP/1.1 201 Created
Content-Type: application/json

{
  "url": "https://example.com/feed.rss"
}
```

**Response — 400 Bad Request**

Retornado quando `url` é nula, vazia, ou contém apenas espaços em branco.

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "errors": {
    "Url": ["The Url field is required."]
  }
}
```

**Behavior notes**:
- A URL recebe `Trim()` antes de ser armazenada
- URLs duplicadas são aceitas sem erro (FR-004)
- Qualquer string não vazia é aceita — sem validação de formato URL (FR-004)
- Retorna o item criado (após trim) para o frontend atualizar a lista local sem re-fetch

---

### GET /api/subscriptions

Retorna a lista completa de assinaturas em ordem de inserção.

**Request**

```http
GET /api/subscriptions
```

Sem parâmetros, sem body, sem headers adicionais.

**Response — 200 OK**

```http
HTTP/1.1 200 OK
Content-Type: application/json

[
  { "url": "https://feeds.example.com/rss" },
  { "url": "https://blog.example.com/atom.xml" }
]
```

Retorna array vazio `[]` quando não há assinaturas (estado inicial / após restart).

**Behavior notes**:
- Sempre retorna 200 OK, mesmo para lista vazia
- Ordem de inserção preservada
- Dados da sessão atual apenas — sem histórico entre restarts

---

## Tratamento de erros

| Cenário | HTTP Status | Descrição |
|---------|-------------|-----------|
| URL vazia ou apenas espaços | 400 Bad Request | Validação de model binding do ASP.NET Core |
| Rota não encontrada | 404 Not Found | Padrão do framework |
| Erro interno | 500 Internal Server Error | Padrão do framework; não expõe detalhes internos |

*Nota: Para o MVP não há tratamento customizado de erros além do padrão do ASP.NET Core.*

---

## Configuração CORS

```csharp
// Program.cs — backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5213")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar após routing, antes de endpoints:
app.UseCors("AllowBlazorFrontend");
```

**Princípio de segurança**: Sem wildcards (`*`). Origem explícita `http://localhost:5213`
conforme Princípio II da constituição.

---

## Exemplos de uso

**Adicionar assinatura (curl)**:
```bash
curl -X POST http://localhost:5151/api/subscriptions \
  -H "Content-Type: application/json" \
  -d '{"url": "https://feeds.example.com/rss"}'
```

**Listar assinaturas (curl)**:
```bash
curl http://localhost:5151/api/subscriptions
```

**Adicionar assinatura (HttpClient C# — Blazor WASM)**:
```csharp
var response = await _httpClient.PostAsJsonAsync("subscriptions", new { url = inputUrl });
if (response.IsSuccessStatusCode)
{
    var item = await response.Content.ReadFromJsonAsync<SubscriptionItemDto>();
    // atualizar lista local com item retornado
}
```
