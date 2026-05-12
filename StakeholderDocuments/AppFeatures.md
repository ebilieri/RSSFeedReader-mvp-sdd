# App features

Este leitor de RSS demonstra o gerenciamento de assinaturas como base para uma aplicação leitora de feeds.

## MVP scope (proof-of-concept version)

O MVP demonstra a funcionalidade mínima viável: gerenciar uma lista de assinaturas.

Para o MVP, o aplicativo DEVE:

- Permitir que o usuário adicione uma assinatura de feed colando uma URL
- Exibir a lista de assinaturas na interface

Para o MVP, o aplicativo PODE:

- Armazenar dados apenas em memória (os dados são perdidos quando o aplicativo fecha)
- Aceitar qualquer URL sem validação (assume URLs válidas de feeds RSS/Atom)
- Exibir assinaturas em formato de lista simples

## MVP behavior

O MVP segue regras simples:

- Usuários podem adicionar assinaturas inserindo uma URL
- A lista de assinaturas é atualizada imediatamente quando uma assinatura é adicionada
- Sem busca, análise ou validação de feeds
- Sem tratamento de erros necessário (sem operações de rede)

## Extended-MVP features

Após o MVP básico (gerenciamento de assinaturas) estar funcionando, o Extended-MVP adiciona busca e exibição de feeds:

- **Manual refresh**: Usuários podem clicar em "refresh" para buscar o conteúdo do feed
- **Item display**: Exibir itens com título e link
- **Basic error handling**: Mostrar "Failed to load feed" se algo der errado
- **No automatic polling**: Apenas atualização manual, sem atualizações em segundo plano

## Post-MVP features

Após desenvolver um aplicativo Extended-MVP bem-sucedido, os seguintes recursos podem ser considerados para versões futuras:

### Essential improvements

- **Persistence**: Armazenar assinaturas e itens em um banco de dados para que permaneçam disponíveis após reiniciar o aplicativo
- **Remove subscriptions**: Permitir que usuários excluam feeds
- **Better item display**: Exibir resumos/conteúdo dos itens, não apenas títulos
- **Newest-first sorting**: Exibir itens em ordem cronológica

### Additional capabilities

- **Background polling**: Atualizar feeds automaticamente em um agendamento
- **Read/unread tracking**: Marcar itens como lidos e filtrar por status de leitura
- **Website-to-feed discovery**: Permitir que usuários colem uma URL de site e encontrem automaticamente seu feed RSS
- **Folders/organization**: Agrupar feeds em categorias
- **Better error handling**: Mostrar mensagens de erro específicas (feed movido, acesso negado, XML malformado, etc.)
- **De-duplication**: Garantir que o mesmo item não seja armazenado várias vezes
- **HTML rendering**: Exibir com segurança conteúdo rico dos feeds

### Practical notes for developers

**For MVP (subscription management only):**

- Usar armazenamento simples em memória (List em C#)
- Sem necessidade de bibliotecas de análise de feed ainda
- Sem cliente HTTP necessário para o MVP
- Foco em interface básica e gerenciamento de estado

**For Extended-MVP (add feed fetching):**

- Usar `System.ServiceModel.Syndication` para análise
- Testar com feeds conhecidamente funcionais (ex.: <https://devblogs.microsoft.com/dotnet/feed/>)
- Evitar casos complexos de análise - lidar apenas com formatos básicos de RSS/Atom

## Additional features (longer-term)

Se o aplicativo crescer além de uma demonstração básica, estes recursos podem ser considerados:

- **Search and filtering**: Encontrar itens por palavra-chave, filtrar por data ou categoria
- **OPML import/export**: Transferir assinaturas entre leitores de feed
- **Advanced organization**: Tags, itens salvos, prioridades
- **Multi-device sync**: Compartilhar assinaturas e estado de leitura entre dispositivos
- **Notifications**: Alertar sobre novos itens de feeds importantes
- **Integrations**: Compartilhar por e-mail, ferramentas de chat ou serviços de leitura posterior
- **Offline reading**: Armazenar em cache o conteúdo completo do artigo para leitura offline
- **Mobile apps**: Aplicativos nativos para telefones e tablets
