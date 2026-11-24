# Verificação da versão local - 22/11/2026

## Base e decisão

Árvore inicial limpa no commit `498005d` (backup). A simplificação foi feita nesta pasta, sem restaurar o PI. O aplicativo 0.13.0 mantém somente planejamento local do aluno em quatro telas. SQLite é o armazenamento final; não é cache de um servidor. Não há conta, papel, vínculo, publicação de ficha, execução ou histórico de treino.

Foram removidos o projeto da API, seus testes, cliente HTTP, configurações de rede permissiva, scripts MySQL e Compose. A solução contém o app MAUI e a suíte local. Os documentos acadêmicos na pasta superior e os arquivos de bancos antigos por conta não foram alterados. O novo arquivo é `workoutly.db3`, privado da instalação.

## Conceito e implementação

- **Model:** `Models/PlanItem.cs` define ficha, exercício e item do planejamento.
- **ViewModel:** valida campos, controla pesquisa e favoritas e chama o DAO. O editor usa bindings de duas vias e coleção observável; o code-behind apresenta diálogos e navega.
- **DAO:** `TemplateDao` concentra SQLite. Três tabelas: `ModeloFicha`, `ExercicioLocal`, `ItemModelo`. As chaves estrangeiras são habilitadas em cada conexão. Cabeçalho e itens são salvos em transação; excluir ficha elimina seus itens por cascata. O catálogo permanece disponível.
- **View:** início, lista, editor e seletor modal. O Shell mantém menu lateral, duas abas e pilha de navegação. A confirmação de descarte continua em IConfirmLeave. Erros de validação são revelados por FormFeedback.

O favorito é booleano com efeito real na ordenação e no filtro **Todas / Só favoritas**. Pesquisa e filtro se combinam. Não há fichas fictícias pré-carregadas; somente o catálogo de referência com 30 exercícios.

## Testes automatizados

Comando: `~/.dotnet/dotnet run --project tests/Workoutly.Local.Tests -p:NuGetAudit=false`.

Resultado: **nove verificações passaram**, com diretório temporário e dados fictícios:

1. Criar ficha e recuperar os mesmos campos por nova instância do DAO.
2. Alterar exercícios e recuperar a versão 2.
3. Falha de chave estrangeira reverte a transação inteira.
4. Exclusão com versão antiga preserva o registro atual.
5. Todas mostra duas fichas.
6. Só favoritas mostra apenas a favorita.
7. Pesquisa de ficha não favorita não fura o filtro de favoritas.
8. Voltar a Todas preserva a pesquisa e recupera o item esperado.
9. Excluir deixa a lista vazia.

O teste do filtro foi executado antes da implementação: falhou por ausência de `OnlyFavorites`. Após implementar a propriedade, a suíte passou. Os testes acessam interfaces públicas do DAO e da ViewModel, usando SQLite real, sem MySQL ou mocks. A auditoria NuGet foi desativada neste comando; não representa auditoria de vulnerabilidades.

## Compilação iOS

- SDK .NET 10.0.401 do diretório `~/.dotnet`; MAUI 10.0.110.
- Comando: `~/.dotnet/dotnet build src/Workoutly.Mobile/Workoutly.Mobile.csproj -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64 -p:NuGetAudit=false`.
- Resultado: **êxito, zero avisos e zero erros**.

## Compilação Android

- SDK Android em `~/Library/Developer/Android/sdk`, JDK 21 em `/Library/Java/JavaVirtualMachines/jdk-21.jdk/Contents/Home`.
- Comando: `~/.dotnet/dotnet build src/Workoutly.Mobile/Workoutly.Mobile.csproj -f net10.0-android -p:JavaSdkDirectory=/Library/Java/JavaVirtualMachines/jdk-21.jdk/Contents/Home -p:AndroidSdkDirectory=/Users/pedrobaldi/Library/Developer/Android/sdk -p:NuGetAudit=false`.
- Resultado: **êxito, um aviso XA0033 e zero erros**. O tooling não interpretou o número de versão `21.0.13.0.2` do Java instalado. O aviso não impediu a compilação.
- Execução Android e dispositivo físico não foram testados.

## Fluxo no iOS Simulator

Aplicativo instalado e executado em **iPhone 18 Pro Simulator, iOS 27.0, arm64**. Dados fictícios: “Ficha A - Peito” com “Supino com halteres”. Verificações observadas na interface:

- Início sem login → botão Abrir minhas fichas → lista inicialmente vazia.
- Nova ficha → Salvar sem nome → erro visível de 2 a 120 caracteres.
- Nome preenchido e favorita marcada → seletor modal → pesquisa “supino” mostra três resultados → Incluir retorna ao editor com um exercício.
- Salvar retorna à lista, que mostra uma ficha, três séries e confirmação temporária.
- Alternar Só favoritas mantém a ficha favorita visível.
- Reabrir, mudar de três para quatro séries e desmarcar favorita → salvar → desaparece de Só favoritas; Todas mostra a ficha com quatro séries.
- Digitar “inexistente” apresenta vazio de pesquisa, sem erro de carregamento.
- Encerrar o processo e relançar → lista recupera a ficha com quatro séries.
- Excluir ficha → Cancelar mantém o editor e o registro; repetir e confirmar Excluir retorna à lista vazia.
- Menu lateral abre e o destino Início retorna à tela inicial. As abas Início/Fichas estão presentes e a aba Fichas acompanha a navegação.
- Uma tentativa de retorno durante edição exibiu “Descartar alterações?”; Continuar editando preservou os campos.

Os botões inferiores do editor exigem rolagem quando o conteúdo excede a tela. As capturas não são mockups; ficam em `capturas-finais/`. O PDF foi regenerado dessas imagens e conferido por renderização. As capturas antigas foram retiradas desta entrega; continuam no backup Git.

## Limites

Não foram realizadas avaliação com participantes, publicação, envio à disciplina, teste Android em execução, teste em aparelho físico ou auditoria de dependências. O material externo da turma não foi revalidado. Shell, CollectionView e Image são equivalências MAUI dos componentes do enunciado; a aceitação das classes de navegação deve ser confirmada pelo professor, conforme Requisitos.md. A execução observada foi local; não foi necessário iniciar ou escrever em API alguma.

## Rechecagem solicitada em 22/11/2026

A revisão estática do escopo e dos padrões não identificou defeitos confirmados. Permanecem as duas ressalvas acadêmicas acima: equivalência do Shell e material externo da turma não delimitado.

Os nove testes foram reexecutados com sucesso. O build iOS voltou a passar sem avisos ou erros. Os builds inicialmente simultâneos disputaram `obj/project.assets.json`; a repetição isolada do Android passou, com o mesmo aviso XA0033 e nenhum erro. Não houve alteração de código nesta rechecagem.

No simulador, foram repetidos início → lista → editor → seletor, cancelamento do seletor preservando o nome, recusa de ficha sem exercícios, inclusão de exercício, salvamento, reabertura e exclusão confirmada. A ficha fictícia “Conferencia local” foi excluída ao concluir o teste. A execução Android continua não verificada.
