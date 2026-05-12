# Implementation Plan Checklist: RSS Feed Reader MVP — Subscription Management

**Purpose**: Validar a qualidade, completude e clareza dos artefatos do plano de implementação
(tasks.md, data-model.md, contracts/api.md, quickstart.md, research.md) antes de iniciar a
implementação
**Created**: 2026-05-11  
**Depth**: Thorough | **Audience**: Autor — auto-revisão pré-implementação (Phase 1)  
**Artifacts**: [tasks.md](../tasks.md) · [data-model.md](../data-model.md) · [contracts/api.md](../contracts/api.md) · [quickstart.md](../quickstart.md) · [research.md](../research.md)

---

## Rastreabilidade de Requisitos (tasks.md ↔ spec.md)

- [ ] CHK001 - Cada FR da spec.md (FR-001 a FR-008) é rastreável a pelo menos uma tarefa em tasks.md? Algum requisito funcional está sem cobertura de tarefa? [Completeness, Spec §FR-001–FR-008]
- [ ] CHK002 - FR-005 ("sem operações de rede no MVP") está explicitamente reforçado em alguma tarefa ou checkpoint como restrição de implementação? [Coverage, Spec §FR-005, Gap]
- [ ] CHK003 - SC-002 ("lista atualizada em menos de 1 segundo") é rastreável a alguma decisão de design (ex.: atualização local sem re-fetch, in-memory response)? Está documentado em research.md ou data-model.md? [Traceability, Spec §SC-002, research.md §Decisão 5]
- [ ] CHK004 - SC-004 ("executável sem configuração adicional além do clone e dos comandos documentados") está coberta por quickstart.md de forma que um desenvolvedor sem contexto prévio possa seguir? [Completeness, Spec §SC-004, quickstart.md]

---

## Completude das Tarefas (tasks.md)

- [ ] CHK005 - Existe uma tarefa explícita para verificar que os dois projetos criados em T001/T002 compilam e executam com `dotnet run` antes de qualquer modificação? Ou este checkpoint é suficientemente claro no texto de Phase 1? [Clarity, tasks.md §Phase 1]
- [ ] CHK006 - T014 (controller) especifica com clareza suficiente a anotação de rota (`[Route("api/[controller]")]`) e o atributo `[ApiController]`, ou isso fica implícito para o implementador? [Clarity, tasks.md §T014, Gap]
- [ ] CHK007 - T008 (limpeza do template Blazor) especifica o estado final esperado de `NavMenu.razor` após a limpeza? O critério de "limpeza completa" está mensurável? [Clarity, tasks.md §T008, Gap]
- [ ] CHK008 - A variável `isLoading` declarada em T018 tem seu uso de UI definido em alguma tarefa? (ex.: desabilitar botão durante request, mostrar indicador visual) Ou é declarada sem propósito especificado? [Completeness, tasks.md §T018, Gap]
- [ ] CHK009 - Existe uma tarefa ou checkpoint que valide a conexão entre frontend e backend antes de iniciar as tarefas de User Story 1? O checkpoint de Phase 2 é mensurável? [Completeness, tasks.md §Phase 2 Checkpoint]
- [ ] CHK010 - Existe uma tarefa para verificar que a leitura de `ApiBaseUrl` de `appsettings.json` falha graciosamente (ex.: fallback para URL padrão) se o arquivo não estiver presente, ou está assumido que o arquivo sempre existirá? [Coverage, tasks.md §T007, Gap]

---

## Clareza das Tarefas (especificidade suficiente para implementar)

- [ ] CHK011 - T019 especifica os textos exatos das mensagens de feedback ("Assinatura adicionada!" e "Insira uma URL") ou deixa aberto para o implementador decidir? São rastreáveis a FR-007 e FR-008? [Clarity, tasks.md §T019, Spec §FR-007, FR-008]
- [ ] CHK012 - T019 especifica quando `errorMessage` deve ser limpa? (ex.: ao usuário começar a digitar, na próxima submissão bem-sucedida, ou nunca até nova tentativa) [Clarity, tasks.md §T019, Gap]
- [ ] CHK013 - T006 (CORS) especifica a posição correta do middleware em `Program.cs` (após `app.UseRouting()`, antes de `app.MapControllers()`), ou isso fica implícito? [Clarity, tasks.md §T006, research.md §Decisão 4]
- [ ] CHK014 - T021 (empty state) especifica o texto exato da mensagem de estado vazio ("Nenhuma assinatura adicionada ainda." ou similar), ou o critério visual de "sem erros visuais" (SC-005) é suficientemente específico? [Clarity, tasks.md §T021, Spec §SC-005]

---

## Dependências e Sequenciamento (tasks.md)

- [ ] CHK015 - A dependência entre T005 (criar `appsettings.json`) e T007 (HttpClient lê de `appsettings.json`) está explicitamente documentada ou apenas implícita pela ordem dentro de Phase 2? [Completeness, tasks.md §T005, T007]
- [ ] CHK016 - T008 (limpeza do template) está corretamente posicionado como pré-requisito de T018 (criar `Subscriptions.razor`)? Seria possível criar a página antes da limpeza e ter conflito de rotas (`@page "/"`)? [Consistency, tasks.md §T008, T018]
- [ ] CHK017 - As tarefas marcadas `[P]` (T004, T009, T010, T015, T023) têm seus arquivos de destino confirmados como independentes (sem conflito de escrita simultânea)? [Consistency, tasks.md §Parallel Opportunities]

---

## Completude do Modelo de Dados (data-model.md)

- [ ] CHK018 - O comportamento de `GetAll()` quando a lista está vazia (retorna `[]` vs. retorna `null`) está explicitamente especificado em data-model.md? A distinção importa para o frontend (`@foreach` em null lançaria exceção)? [Completeness, data-model.md §ISubscriptionService]
- [ ] CHK019 - A responsabilidade pelo `Trim()` da URL está claramente atribuída — é no método `Add()` do service (data-model.md) ou no controller antes de chamar o service? Existe ambiguidade entre data-model.md e contracts/api.md? [Clarity, data-model.md §InMemorySubscriptionService, contracts/api.md §POST]
- [ ] CHK020 - O requisito de thread-safety de `InMemorySubscriptionService` (lock) está rastreável a algum requisito ou assumption no spec.md ou plan.md? Ou é uma decisão de design sem fonte documentada? [Traceability, data-model.md §Armazenamento, Gap]
- [ ] CHK021 - A estratégia de serialização JSON (camelCase para o contrato REST) está documentada em data-model.md ou plan.md, de forma que o implementador saiba se precisa configurar `JsonSerializerOptions` ou se o padrão do ASP.NET Core já entrega isso? [Completeness, data-model.md §SubscriptionItemDto, Gap]

---

## Completude do Contrato de API (contracts/api.md)

- [ ] CHK022 - O comportamento da API para um body POST com `Content-Type` errado (ex.: `text/plain`) ou body malformado (JSON inválido) está documentado em contracts/api.md? O frontend precisa tratar esse caso? [Coverage, contracts/api.md §POST, Gap]
- [ ] CHK023 - O contrato especifica o formato exato do corpo de resposta 400? O frontend (`SubscriptionApiService`) é esperado parsear `{ "errors": { "Url": ["..."] } }` ou apenas checar o status HTTP? [Clarity, contracts/api.md §400, tasks.md §T016]
- [ ] CHK024 - O comportamento do backend para um request GET sem Content-Type ou com headers inesperados está documentado? É assumido que qualquer request GET é aceito? [Coverage, contracts/api.md §GET, Gap]
- [ ] CHK025 - O contrato documenta que a URL retornada no body 201 reflete o valor **após trim** (e não o valor original enviado pelo usuário)? [Clarity, contracts/api.md §POST 201, data-model.md §Add]

---

## Consistência entre Documentos

- [ ] CHK026 - Os números de porta (5151 para backend, 5213 para frontend) são consistentes em todos os locais: plan.md, contracts/api.md, quickstart.md, tasks.md (T003, T004, T005, T006)? [Consistency]
- [ ] CHK027 - O nome da propriedade `Url` (PascalCase em C#) e `url` (camelCase em JSON) está consistente entre data-model.md, contracts/api.md e o DTO `SubscriptionItemDto`? A regra de casing está documentada em algum lugar? [Consistency, data-model.md, contracts/api.md]
- [ ] CHK028 - O caminho do arquivo `frontend/RSSFeedReader.UI/Models/SubscriptionItemDto.cs` em tasks.md (T015) é consistente com o namespace `RSSFeedReader.UI.Models` em data-model.md? [Consistency, tasks.md §T015, data-model.md §SubscriptionItemDto]

---

## Cobertura de Casos de Borda

- [ ] CHK029 - O comportamento do frontend quando o backend está inacessível no momento de `OnInitializedAsync` (T020) está especificado em algum artefato? O usuário verá erro silencioso, mensagem de erro, ou lista vazia? [Coverage, Edge Case, tasks.md §T020, Gap]
- [ ] CHK030 - O comportamento do frontend quando o POST retorna 400 (validação de backend) está especificado em tasks.md ou contracts/api.md? O `SubscriptionApiService` precisa fazer parse do erro ou apenas propagar o status? [Coverage, Edge Case, tasks.md §T016, contracts/api.md §400]
- [ ] CHK031 - O cenário de submissão múltipla rápida (double-click no botão "Adicionar") está endereçado? Existe requisito de debounce ou desabilitação do botão durante a chamada HTTP? [Coverage, Edge Case, Gap]

---

## Requisitos Não-Funcionais e Segurança

- [ ] CHK032 - O requisito "sem wildcards no CORS" está rastreável da constituição (Princípio II) através do Constitution Check no plan.md, da seção CORS em contracts/api.md, e da tarefa T006? A cadeia de rastreabilidade está completa? [Traceability, Spec §constitution §II, contracts/api.md §CORS, tasks.md §T006]
- [ ] CHK033 - O requisito de segurança "frontend lê ApiBaseUrl de configuração, nunca hardcoded" (Princípio II + IV da constituição) está rastreável a tasks.md (T005, T007) e ao contrato em quickstart.md? [Traceability, plan.md §Constitution Check §II]
- [ ] CHK034 - Existe algum requisito ou constraint sobre o tamanho máximo do campo de entrada (URL) no frontend? A ausência de limite está documentada como decisão consciente? [Coverage, Gap]

---

## Prontidão para Implementação

- [ ] CHK035 - Após ler todos os artefatos do plano, um desenvolvedor consegue iniciar T001 imediatamente sem necessidade de informação adicional? Existem pré-condições implícitas não documentadas (ex.: versão específica do .NET CLI)? [Readiness, tasks.md §T001, quickstart.md §Pré-requisitos]
- [ ] CHK036 - O título/cabeçalho esperado da página `Subscriptions.razor` está especificado em algum artefato? O texto do botão "Adicionar" e o label do campo "URL do Feed" estão definidos como requisitos ou ficam à discrição do implementador? [Completeness, Gap]

---

## Notes

- Marcar itens como `[x]` conforme verificados
- Adicionar `[ISSUE]` no item caso seja encontrada lacuna que precise ser resolvida antes de implementar
- `[Gap]` = requisito ausente intencionalmente ou por omissão — avaliar se precisa ser documentado
- Itens CHK029–CHK031 (edge cases de runtime) podem ser explicitamente anotados como "fora do escopo do MVP" se confirmado
