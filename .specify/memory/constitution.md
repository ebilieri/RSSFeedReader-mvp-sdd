<!--
SYNC IMPACT REPORT
==================
Version change: placeholder template → 1.0.0 (constituição inicial)
Added sections:
  - I. MVP-First Development (novo)
  - II. Security by Design (novo)
  - III. Clean Architecture Separation (novo)
  - IV. Configuration Over Convention (novo)
  - V. Simplicity and Maintainability (novo)
  - Technology Standards (nova seção)
  - Development Workflow (nova seção)
  - Governance (nova seção)
Modified principles: N/A — nenhum princípio existia antes
Removed sections: N/A
Templates requiring updates:
  - ✅ .specify/templates/plan-template.md — Constitution Check reflete os 5 princípios
  - ✅ .specify/templates/spec-template.md — sem alterações estruturais necessárias
  - ✅ .specify/templates/tasks-template.md — sem alterações estruturais necessárias
Follow-up TODOs:
  - Nenhum placeholder intencional deferido
-->

# RSS Feed Reader Constitution

## Core Principles

### I. MVP-First Development

O desenvolvimento DEVE seguir a progressão incremental definida:
**MVP** (gerenciamento de assinaturas) → **Extended-MVP** (busca de feeds) → **Post-MVP**
(persistência e recursos avançados).

- Nenhuma fase pode incluir funcionalidades da fase seguinte
- Cada fase DEVE estar totalmente funcional e testável antes de avançar
- Armazenamento em memória (`List<T>`) é suficiente e obrigatório para o MVP;
  banco de dados só é introduzido no Post-MVP
- Funcionalidades marcadas como "Extended-MVP" ou "Post-MVP" nos documentos de
  stakeholders NÃO devem ser implementadas na fase MVP

### II. Security by Design

Toda entrada do usuário DEVE ser validada nos limites da API antes do processamento.

- CORS DEVE ser configurado explicitamente com origens permitidas listadas;
  wildcard (`*`) é proibido em qualquer ambiente
- Nenhuma credencial, porta, URL base ou segredo pode ser codificado diretamente
  no código-fonte (`hardcoded`)
- O frontend DEVE ler a URL base da API a partir de `wwwroot/appsettings.json`,
  nunca de `Program.cs` diretamente
- Dependências externas DEVEM ser auditadas antes da adição ao projeto

### III. Clean Architecture Separation

Backend (ASP.NET Core Web API) e frontend (Blazor WebAssembly) são camadas
estritamente separadas com responsabilidades exclusivas.

- **Backend**: lógica de negócio, armazenamento de dados, operações de feed
- **Frontend**: interface do usuário, interação e exibição
- A comunicação entre camadas DEVE ocorrer apenas por contratos de API
  bem definidos (endpoints REST)
- Nenhuma lógica de negócio pode residir no frontend
- Nenhuma operação de UI (formatação, roteamento de páginas) pode ser
  implementada no backend

### IV. Configuration Over Convention

Todos os valores específicos de ambiente DEVEM estar em arquivos de configuração,
nunca no código-fonte.

- Coordenação de portas DEVE ser mantida consistente em exatamente três
  arquivos: `backend/.../launchSettings.json`, `frontend/.../launchSettings.json`
  e `frontend/.../wwwroot/appsettings.json`
- O `wwwroot/appsettings.json` é a fonte única de verdade para a URL base
  da API no frontend
- A política CORS no backend DEVE referenciar as portas definidas no
  `launchSettings.json` do frontend — nunca valores arbitrários
- Qualquer nova configuração de ambiente DEVE seguir o mesmo padrão antes
  de ser utilizada no código

### V. Simplicity and Maintainability (YAGNI)

Implementar apenas o que a fase atual exige; complexidade antecipada é proibida.

- O código DEVE ser legível, com responsabilidades claras e sem duplicação
  desnecessária
- Comentários no código são obrigatórios apenas quando a intenção não é óbvia
  pela leitura do código
- Bibliotecas externas DEVEM ser justificadas com base em necessidade real
  da fase atual, não em uso futuro hipotético
- Refatorações de melhoria (não corretivas) só são permitidas após a fase
  atual estar totalmente funcional e testada

## Technology Standards

Padrões tecnológicos obrigatórios para todas as fases do projeto:

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Core Web API (C#, .NET) |
| Frontend | Blazor WebAssembly (C#) |
| Armazenamento MVP | In-memory (`List<T>`) |
| Armazenamento Post-MVP | EF Core + SQLite |
| Análise de feeds (Extended-MVP) | `System.ServiceModel.Syndication` |
| Cliente HTTP (Extended-MVP) | `HttpClient` via injeção de dependência |
| Testes (futuro) | xUnit |
| Plataformas suportadas | Windows, macOS, Linux |

Bibliotecas não listadas acima DEVEM ser documentadas e justificadas antes
de serem adicionadas ao projeto.

## Development Workflow

Todo desenvolvimento DEVE seguir este fluxo de trabalho, nesta ordem:

1. **Limpeza de template Blazor** (obrigatória, fase 2 — antes de qualquer
   implementação de UI): remover `Home.razor`, `Counter.razor` e `Weather.razor`;
   atualizar `NavMenu.razor` para refletir apenas recursos do MVP
2. **Verificação de roteamento**: garantir que apenas UMA página use `@page "/"`
   antes de implementar qualquer feature
3. **Verificação de configuração**: confirmar que `appsettings.json` aponta para
   a porta correta do backend e que o CORS do backend permite a origem do frontend
4. **Checklist de desenvolvimento local**: backend em execução na porta configurada,
   frontend carregando no navegador, sem erros no console do DevTools
5. **Teste por fase**: cada fase DEVE ser testada com casos de uso reais antes de
   avançar. Para Extended-MVP, o feed de referência é
   <https://devblogs.microsoft.com/dotnet/feed/>

## Governance

Esta constituição é o documento normativo mais alto do projeto. Todos os artefatos
de design (spec.md, plan.md, tasks.md) e decisões de implementação DEVEM estar
alinhados com estes princípios.

**Processo de emenda**: toda alteração a esta constituição DEVE incluir:
(a) justificativa documentada, (b) bump de versão seguindo Semantic Versioning
(MAJOR: remoção/redefinição de princípio; MINOR: novo princípio ou seção;
PATCH: correções de texto), (c) propagação das mudanças para os templates
dependentes.

**Conformidade**: toda implementação de feature DEVE ser revisada contra os
princípios desta constituição antes de ser considerada concluída. Violações
detectadas DEVEM ser corrigidas antes do avanço para a próxima fase.

**Version**: 1.0.0 | **Ratified**: 2026-05-11 | **Last Amended**: 2026-05-11
