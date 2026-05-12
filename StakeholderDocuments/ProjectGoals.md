# Project goals

Construir um leitor simples de RSS/Atom. O objetivo é demonstrar a capacidade mais básica (gerenciar uma lista de assinaturas) sem a complexidade de buscar e exibir conteúdo de feeds.

## Purpose

O aplicativo existe para demonstrar como um usuário pode construir uma lista de assinaturas para feeds RSS. Este é um proof-of-concept focado na interface de gerenciamento de assinaturas.

## Target scope (MVP only)

Esta é uma aplicação POC mínima para um único usuário, executando localmente. Foi projetada para ser desenvolvida e testada no Windows, macOS ou Linux.

O MVP inclui apenas:

- Adicionar uma assinatura de feed por URL
- Exibir a lista de assinaturas na interface

Todos os outros recursos (busca de feeds, exibição de itens, persistência, remoção de assinaturas, etc.) são adiados para Extended-MVP ou pós-MVP.

## Delivery approach

O foco é no desenvolvimento rápido do recurso MVP. Construir a funcionalidade mínima primeiro:

- Adicionar uma assinatura por URL
- Exibir a lista de assinaturas

Para manter o desenvolvimento ágil:

- Sem busca ou análise de feeds necessária para o MVP
- Sem validação de URLs de feed (assume que o usuário fornece URLs válidas)
- Armazenar assinaturas apenas em memória (abordagem mais simples)
- Manter a interface simples e funcional ao invés de polida

## What "MVP working" means

O MVP está completo quando:

1. Um usuário pode adicionar uma assinatura de feed colando uma URL
2. A interface exibe a lista atualizada de assinaturas

Não é necessária busca real de feeds, análise ou exibição de itens para o MVP.

## Extended-MVP (next phase)

Após o MVP básico estar funcionando, o Extended-MVP adiciona capacidades de busca e exibição de feeds:

1. Um usuário pode clicar em um botão para atualizar manualmente o feed
2. Itens do feed são exibidos (título e link no mínimo)

Testar com um feed RSS conhecido como <https://devblogs.microsoft.com/dotnet/feed/>.

### Local development checklist

Antes de testar o MVP, verificar:

- [ ] O backend executa sem erros e escuta na porta configurada
- [ ] O frontend executa sem erros e carrega no navegador
- [ ] A configuração do frontend (`wwwroot/appsettings.json`) aponta para a URL correta do backend
- [ ] O CORS do backend permite a origem do frontend
- [ ] O console do DevTools do navegador não mostra erros de conexão ao carregar a página

## Future enhancements (post-MVP)

Após o Extended-MVP estar funcionando (gerenciamento de assinaturas + busca de feeds + exibição de itens), estes recursos podem ser adicionados:

- **Persistence**: Salvar assinaturas e itens entre sessões (requer implementação de banco de dados)
- **Remove subscriptions**: Permitir que usuários excluam feeds que não desejam mais
- **Background polling**: Atualizar feeds automaticamente em um agendamento
- **Better error handling**: Mostrar mensagens de erro detalhadas para diferentes cenários de falha
- **Content rendering**: Exibir o conteúdo completo do item, não apenas título e link
- **Read/unread tracking**: Marcar itens como lidos e filtrar adequadamente
- **Organization**: Agrupar feeds em pastas ou categorias

## Technology selection note

Embora este MVP seja intencionalmente simples, as escolhas tecnológicas (ASP.NET Core + Blazor) devem suportar recursos futuros prontos para produção sem exigir uma reescrita completa. A arquitetura permite adicionar persistência, operações em segundo plano e capacidades avançadas de interface conforme necessário.

## How this document fits with the others

- [AppFeatures.md](AppFeatures.md) descreve os recursos específicos voltados ao usuário para o MVP
- [TechStack.md](TechStack.md) explica as escolhas tecnológicas e como elas suportam os objetivos do MVP
