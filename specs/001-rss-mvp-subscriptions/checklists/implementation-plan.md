# Implementation Plan Checklist: RSS Feed Reader MVP — Subscription Management

**Purpose**: Validar a qualidade, completude e clareza dos artefatos do plano de implementação
(tasks.md, data-model.md, contracts/api.md, quickstart.md, research.md) antes de iniciar a
implementação
**Created**: 2026-05-11  
**Depth**: Thorough | **Audience**: Autor — auto-revisão pré-implementação (Phase 1)  
**Artifacts**: [tasks.md](../tasks.md) · [data-model.md](../data-model.md) · [contracts/api.md](../contracts/api.md) · [quickstart.md](../quickstart.md) · [research.md](../research.md)

---

## Rastreabilidade de Requisitos (tasks.md ↔ spec.md)

- [x] CHK001 - Cada FR da spec.md (FR-001 a FR-008) é rastreável a pelo menos uma tarefa em tasks.md? Algum requisito funcional está sem cobertura de tarefa? [Completeness, Spec §FR-001–FR-008] ✅ FR-001→T019(add), FR-002→T019(lista local), FR-003→T012(service), FR-004→T010/T012, FR-005→constraint em plan.md+spec.md, FR-006→T019(limpa newUrl), FR-007→T019(successMessage), FR-008→T019(errorMessage). Cobertura completa.
- [x] CHK002 - FR-005 ("sem operações de rede no MVP") está explicitamente reforçado em alguma tarefa ou checkpoint como restrição de implementação? [Coverage, Spec §FR-005, Gap] ✅ Documentado em plan.md §Technical Context ("sem operações de rede"), spec.md §FR-005, e research.md §Decisão 1. FR-005 é requisito negativo — nenhuma tarefa de rede existe no tasks.md, o que o confirma implicitamente.
- [x] CHK003 - SC-002 ("lista atualizada em menos de 1 segundo") é rastreável a alguma decisão de design (ex.: atualização local sem re-fetch, in-memory response)? Está documentado em research.md ou data-model.md? [Traceability, Spec §SC-002, research.md §Decisão 5] ✅ plan.md §Performance Goals: "Resposta visual < 1 segundo após confirmação (SC-002)". T019 atualiza lista local sem re-fetch (item retornado pelo POST adicionado diretamente). In-memory no backend → response imediata. Rastreabilidade completa.
- [x] CHK004 - SC-004 ("executável sem configuração adicional além do clone e dos comandos documentados") está coberta por quickstart.md de forma que um desenvolvedor sem contexto prévio possa seguir? [Completeness, Spec §SC-004, quickstart.md] ✅ quickstart.md tem: pré-requisitos (.NET 8 SDK), tabela de 3 arquivos de configuração, comandos exatos para backend e frontend, verificação via curl, walkthrough de uso. Um desenvolvedor sem contexto prévio consegue seguir.

---

## Completude das Tarefas (tasks.md)

- [x] CHK005 - Existe uma tarefa explícita para verificar que os dois projetos criados em T001/T002 compilam e executam com `dotnet run` antes de qualquer modificação? Ou este checkpoint é suficientemente claro no texto de Phase 1? [Clarity, tasks.md §Phase 1] ✅ tasks.md §Phase 1 Checkpoint: "Dois projetos criados e executáveis com `dotnet run` em seus respectivos diretórios" — checkpoint explícito e mensurável após T001/T002.
- [x] CHK006 - T014 (controller) especifica com clareza suficiente a anotação de rota (`[Route("api/[controller]")]`) e o atributo `[ApiController]`, ou isso fica implícito para o implementador? [Clarity, tasks.md §T014, Gap] ✅ Resolvido em T014: agora especifica `[ApiController]`, `[Route("api/[controller]")]` e injeção de `ISubscriptionService` via construtor.
- [x] CHK007 - T008 (limpeza do template Blazor) especifica o estado final esperado de `NavMenu.razor` após a limpeza? O critério de "limpeza completa" está mensurável? [Clarity, tasks.md §T008, Gap] ✅ T023 §Phase 5 define estado final mensurável: "apenas o link para a página de Assinaturas (`/`) está presente; remover quaisquer links de template remanescentes". T008 (Phase 2) executa a limpeza inicial; T023 (Phase 5) valida o critério final.
- [x] CHK008 - A variável `isLoading` declarada em T018 tem seu uso de UI definido em alguma tarefa? (ex.: desabilitar botão durante request, mostrar indicador visual) Ou é declarada sem propósito especificado? [Completeness, tasks.md §T018, Gap] ✅ Resolvido em T018: `isLoading = true` durante chamadas HTTP; botão `disabled` enquanto `isLoading == true`
- [x] CHK009 - Existe uma tarefa ou checkpoint que valide a conexão entre frontend e backend antes de iniciar as tarefas de User Story 1? O checkpoint de Phase 2 é mensurável? [Completeness, tasks.md §Phase 2 Checkpoint] ✅ tasks.md §Phase 2 Checkpoint: "Backend roda em `http://localhost:5151`, frontend em `http://localhost:5213`; frontend consegue fazer requests ao backend sem erros de CORS" — checkpoint explícito e mensurável antes de US1.
- [x] CHK010 - Existe uma tarefa para verificar que a leitura de `ApiBaseUrl` de `appsettings.json` falha graciosamente (ex.: fallback para URL padrão) se o arquivo não estiver presente, ou está assumido que o arquivo sempre existirá? [Coverage, tasks.md §T007, Gap] ✅ Decisão de design intencional: T005 cria `appsettings.json` antes de T007 (Phase 2 sequencial). spec.md Assumptions: "URL base da API é lida de configuração, nunca hardcoded". Sem fallback no MVP — arquivo é pré-requisito obrigatório.

---

## Clareza das Tarefas (especificidade suficiente para implementar)

- [x] CHK011 - T019 especifica os textos exatos das mensagens de feedback ("Assinatura adicionada!" e "Insira uma URL") ou deixa aberto para o implementador decidir? São rastreáveis a FR-007 e FR-008? [Clarity, tasks.md §T019, Spec §FR-007, FR-008] ✅ T019 especifica: `successMessage = "Assinatura adicionada!"` (FR-007) e `errorMessage = "Insira uma URL"` (FR-008). Textos exatos rastreados aos requisitos.
- [x] CHK012 - T019 especifica quando `errorMessage` deve ser limpa? (ex.: ao usuário começar a digitar, na próxima submissão bem-sucedida, ou nunca até nova tentativa) [Clarity, tasks.md §T019, Gap] ✅ Resolvido em T019: `errorMessage` é limpa no início de cada nova submissão (antes de chamar `AddAsync`)
- [x] CHK013 - T006 (CORS) especifica a posição correta do middleware em `Program.cs` (após `app.UseRouting()`, antes de `app.MapControllers()`), ou isso fica implícito? [Clarity, tasks.md §T006, research.md §Decisão 4] ✅ contracts/api.md §CORS documenta explicitamente: "Registrar após routing, antes de endpoints: `app.UseCors("AllowBlazorFrontend");`" com exemplo de código completo em `Program.cs`.
- [x] CHK014 - T021 (empty state) especifica o texto exato da mensagem de estado vazio ("Nenhuma assinatura adicionada ainda." ou similar), ou o critério visual de "sem erros visuais" (SC-005) é suficientemente específico? [Clarity, tasks.md §T021, Spec §SC-005] ✅ T021 fornece texto sugerido com "ex.:". SC-005 apenas requer "sem erros visuais" — o spec não obriga texto específico para estado vazio. Flexibilidade intencional.

---

## Dependências e Sequenciamento (tasks.md)

- [x] CHK015 - A dependência entre T005 (criar `appsettings.json`) e T007 (HttpClient lê de `appsettings.json`) está explicitamente documentada ou apenas implícita pela ordem dentro de Phase 2? [Completeness, tasks.md §T005, T007] ✅ Phase 2 é totalmente sequencial (exceto T004[P]) com nota "⚠️ CRÍTICO: Nenhuma tarefa de user story pode começar até que esta fase esteja completa". T005 precede T007 na ordem da lista. A dependência sequencial está clara.
- [x] CHK016 - T008 (limpeza do template) está corretamente posicionado como pré-requisito de T018 (criar `Subscriptions.razor`)? Seria possível criar a página antes da limpeza e ter conflito de rotas (`@page "/"`)? [Consistency, tasks.md §T008, T018] ✅ T008 está em Phase 2 (Foundational), T018 em Phase 3 (US1). Phase 3 só inicia após conclusão de Phase 2. O conflito de rota `@page "/"` com `Home.razor` é impedido pela ordem das fases.
- [x] CHK017 - As tarefas marcadas `[P]` (T004, T009, T010, T015, T023) têm seus arquivos de destino confirmados como independentes (sem conflito de escrita simultânea)? [Consistency, tasks.md §Parallel Opportunities] ✅ T004: `frontend/launchSettings.json`; T009: `Models/SubscriptionItem.cs`; T010: `Models/AddSubscriptionRequest.cs`; T015: `frontend/Models/SubscriptionItemDto.cs`; T023: `Layout/NavMenu.razor`. Todos arquivos distintos — sem conflito de escrita.

---

## Completude do Modelo de Dados (data-model.md)

- [x] CHK018 - O comportamento de `GetAll()` quando a lista está vazia (retorna `[]` vs. retorna `null`) está explicitamente especificado em data-model.md? A distinção importa para o frontend (`@foreach` em null lançaria exceção)? [Completeness, data-model.md §ISubscriptionService] ✅ data-model.md: `_subscriptions = []` inicializado como lista vazia; `AsReadOnly()` retorna `IReadOnlyList` vazio, nunca null. contracts/api.md: "Retorna array vazio `[]` quando não há assinaturas".
- [x] CHK019 - A responsabilidade pelo `Trim()` da URL está claramente atribuída — é no método `Add()` do service (data-model.md) ou no controller antes de chamar o service? Existe ambiguidade entre data-model.md e contracts/api.md? [Clarity, data-model.md §InMemorySubscriptionService, contracts/api.md §POST] ✅ Sem ambiguidade: data-model.md `Add()` executa `url.Trim()`. contracts/api.md: "A URL recebe `Trim()` antes de ser armazenada". Responsabilidade no service.
- [x] CHK020 - O requisito de thread-safety de `InMemorySubscriptionService` (lock) está rastreável a algum requisito ou assumption no spec.md ou plan.md? Ou é uma decisão de design sem fonte documentada? [Traceability, data-model.md §Armazenamento, Gap] ✅ research.md §Decisão 3: "Lock necessário porque o ASP.NET Core processa requests concorrentemente por padrão — sem lock, múltiplos POST simultâneos poderiam corromper a lista." Rastreabilidade completa.
- [x] CHK021 - A estratégia de serialização JSON (camelCase para o contrato REST) está documentada em data-model.md ou plan.md, de forma que o implementador saiba se precisa configurar `JsonSerializerOptions` ou se o padrão do ASP.NET Core já entrega isso? [Completeness, data-model.md §SubscriptionItemDto, Gap] ✅ Resolvido em data-model.md §SubscriptionItemDto: padrão camelCase do ASP.NET Core + `ReadFromJsonAsync<T>` documentados; nenhum `[JsonPropertyName]` necessário

---

## Completude do Contrato de API (contracts/api.md)

- [x] CHK022 - O comportamento da API para um body POST com `Content-Type` errado (ex.: `text/plain`) ou body malformado (JSON inválido) está documentado em contracts/api.md? O frontend precisa tratar esse caso? [Coverage, contracts/api.md §POST, Gap] ✅ contracts/api.md §Tratamento de erros: erros de modelo retornam 400 padrão do ASP.NET Core. Frontend (T016) usa `HttpClient.PostAsJsonAsync` que sempre envia `Content-Type: application/json` correto — o cenário é irrelevante para este MVP.
- [x] CHK023 - O contrato especifica o formato exato do corpo de resposta 400? O frontend (`SubscriptionApiService`) é esperado parsear `{ "errors": { "Url": ["..."] } }` ou apenas checar o status HTTP? [Clarity, contracts/api.md §400, tasks.md §T016] ✅ contracts/api.md documenta o formato 400 exato. T019 valida URL no frontend antes de chamar `AddAsync` — 400 não ocorre em fluxo normal. Frontend não precisa parsear o corpo 400 no MVP.
- [x] CHK024 - O comportamento do backend para um request GET sem Content-Type ou com headers inesperados está documentado? É assumido que qualquer request GET é aceito? [Coverage, contracts/api.md §GET, Gap] ✅ contracts/api.md: GET "Sempre retorna 200 OK" independente de headers. Comportamento padrão do ASP.NET Core. Frontend usa `HttpClient.GetFromJsonAsync` que envia headers padrão — sem cenário de headers inesperados no MVP.
- [x] CHK025 - O contrato documenta que a URL retornada no body 201 reflete o valor **após trim** (e não o valor original enviado pelo usuário)? [Clarity, contracts/api.md §POST 201, data-model.md §Add] ✅ contracts/api.md §POST Behavior notes: "A URL recebe `Trim()` antes de ser armazenada" e "Retorna o item criado (após trim) para o frontend atualizar a lista local sem re-fetch". Explícito.

---

## Consistência entre Documentos

- [x] CHK026 - Os números de porta (5151 para backend, 5213 para frontend) são consistentes em todos os locais: plan.md, contracts/api.md, quickstart.md, tasks.md (T003, T004, T005, T006)? [Consistency] ✅ plan.md: 5151/5213 ✓; contracts/api.md Base URL: 5151 ✓; quickstart.md tabela: 5151/5213 ✓; tasks.md T003: 5151, T004: 5213, T005: `http://localhost:5151/api/`, T006: `http://localhost:5213` ✓. Consistente em todos os artefatos.
- [x] CHK027 - O nome da propriedade `Url` (PascalCase em C#) e `url` (camelCase em JSON) está consistente entre data-model.md, contracts/api.md e o DTO `SubscriptionItemDto`? A regra de casing está documentada em algum lugar? [Consistency, data-model.md, contracts/api.md] ✅ data-model.md §SubscriptionItemDto documenta estratégia camelCase (CHK021). contracts/api.md usa `"url"` no JSON. DTO tem `public string Url` (PascalCase). Regra documentada e consistente.
- [x] CHK028 - O caminho do arquivo `frontend/RSSFeedReader.UI/Models/SubscriptionItemDto.cs` em tasks.md (T015) é consistente com o namespace `RSSFeedReader.UI.Models` em data-model.md? [Consistency, tasks.md §T015, data-model.md §SubscriptionItemDto] ✅ tasks.md T015: `frontend/RSSFeedReader.UI/Models/SubscriptionItemDto.cs` ✓; data-model.md: `namespace RSSFeedReader.UI.Models` ✓. Caminho e namespace consistentes.

---

## Cobertura de Casos de Borda

- [x] CHK029 - O comportamento do frontend quando o backend está inacessível no momento de `OnInitializedAsync` (T020) está especificado em algum artefato? O usuário verá erro silencioso, mensagem de erro, ou lista vazia? [Coverage, Edge Case, tasks.md §T020, Gap] ✅ Resolvido em T020: capturar `HttpRequestException`, exibir `errorMessage` com instrução de verificar o backend, inicializar lista vazia (sem crash)
- [x] CHK030 - O comportamento do frontend quando o POST retorna 400 (validação de backend) está especificado em tasks.md ou contracts/api.md? O `SubscriptionApiService` precisa fazer parse do erro ou apenas propagar o status? [Coverage, Edge Case, tasks.md §T016, contracts/api.md §400] ✅ Frontend valida URL vazia antes de chamar `AddAsync` (T019, passo 1) — 400 não ocorre em fluxo normal. T020 cobre `HttpRequestException` para falha de conexão. Parse do corpo 400 é out-of-scope para MVP.
- [x] CHK031 - O cenário de submissão múltipla rápida (double-click no botão "Adicionar") está endereçado? Existe requisito de debounce ou desabilitação do botão durante a chamada HTTP? [Coverage, Edge Case, Gap] ✅ Coberto pelo CHK008: T018/T019 definem `isLoading = true` durante HTTP + `disabled="@isLoading"` no botão. Double-click ou cliques rápidos são prevenidos enquanto a requisição estiver em andamento.

---

## Requisitos Não-Funcionais e Segurança

- [x] CHK032 - O requisito "sem wildcards no CORS" está rastreável da constituição (Princípio II) através do Constitution Check no plan.md, da seção CORS em contracts/api.md, e da tarefa T006? A cadeia de rastreabilidade está completa? [Traceability, Spec §constitution §II, contracts/api.md §CORS, tasks.md §T006] ✅ constitution.md §II → plan.md Constitution Check II ("CORS origens explícitas") → contracts/api.md §CORS ("Sem wildcards (*). Princípio II") → tasks.md T006 ("sem wildcards"). Cadeia completa.
- [x] CHK033 - O requisito de segurança "frontend lê ApiBaseUrl de configuração, nunca hardcoded" (Princípio II + IV da constituição) está rastreável a tasks.md (T005, T007) e ao contrato em quickstart.md? [Traceability, plan.md §Constitution Check §II] ✅ plan.md Constitution Check II+IV → tasks.md T005 (cria `appsettings.json`) + T007 (lê via `builder.Configuration`) → quickstart.md tabela de arquivos sincronizados. Rastreabilidade completa.
- [x] CHK034 - Existe algum requisito ou constraint sobre o tamanho máximo do campo de entrada (URL) no frontend? A ausência de limite está documentada como decisão consciente? [Coverage, Gap] ✅ data-model.md §SubscriptionItem: "Qualquer string não vazia aceita — sem validação de formato de URL (FR-004)". spec.md Assumptions: "Nenhuma validação de URL é realizada". Ausência de limite documentada intencionalmente.

---

## Prontidão para Implementação

- [x] CHK035 - Após ler todos os artefatos do plano, um desenvolvedor consegue iniciar T001 imediatamente sem necessidade de informação adicional? Existem pré-condições implícitas não documentadas (ex.: versão específica do .NET CLI)? [Readiness, tasks.md §T001, quickstart.md §Pré-requisitos] ✅ quickstart.md: ".NET 8 SDK instalado (`dotnet --version` deve retornar `8.x.x`)". T001: comando `dotnet new webapi` exato e caminho de destino `backend/RSSFeedReader.Api`. Nenhum pré-requisito implícito.
- [x] CHK036 - O título/cabeçalho esperado da página `Subscriptions.razor` está especificado em algum artefato? O texto do botão "Adicionar" e o label do campo "URL do Feed" estão definidos como requisitos ou ficam à discrição do implementador? [Completeness, Gap] ✅ Resolvido em T018: heading `<h1>Minhas Assinaturas</h1>`; T019: label "URL do Feed", botão "Adicionar"

---

## Notes

- Marcar itens como `[x]` conforme verificados
- Adicionar `[ISSUE]` no item caso seja encontrada lacuna que precise ser resolvida antes de implementar
- `[Gap]` = requisito ausente intencionalmente ou por omissão — avaliar se precisa ser documentado
- Itens CHK029–CHK031 (edge cases de runtime) podem ser explicitamente anotados como "fora do escopo do MVP" se confirmado
