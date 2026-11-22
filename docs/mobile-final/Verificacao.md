# Origem, implementação e verificação · 22/11/2026

## Origem e escopo

| Fonte consultada | Branch | Commit conferido | O que sustenta |
|---|---|---|---|
| [Aplicativo e API incluída](https://github.com/pedrobaldi01/Workoutly) | documentacao/situacao-fase-2 | `45d880a75ad236070783714789b73ae1933594e4` | README, AGENTS.md, documentação de arquitetura, backlog, páginas MAUI e API com MySqlConnector. |
| [API separada](https://github.com/JoaoAza/Workoutly-API) | main | `c7835b3c19f1c429be360038e361afeb35d2e0f3` | Implementação ASP.NET Core com PostgreSQL. O contrato inclui dados físicos e cargas. |

Na origem, `AccountClient.cs` enviava requisições para `https://workoutly-api.onrender.com/`. Esse endereço por si só não identifica o commit em produção nem o banco conectado. A cópia usa a API MySQL que acompanha a solução e traz a migração 011. O app local lê o endereço de `Services/ApiSettings.cs`.

A cópia começou com a exportação dos arquivos versionados. A árvore Git da branch local `origem-pi` corresponde à da origem, inclusive créditos e licenças. O primeiro commit local é `e7f85cc`. A pasta de trabalho e a raiz Git foram conferidas antes da edição. O resultado de `git remote -v` era vazio. As adaptações estão em `trabalho-final-mobile`. Para revisar apenas o que mudou, use `git diff --stat origem-pi..trabalho-final-mobile`.

## Acompanhar a ficha

| Passo | Tela e código | Destino dos dados |
|---|---|---|
| Professor seleciona um aluno vinculado | ConnectionsPage → PlansPage → PlanPage. A página recebe o ID selecionado. | A API consulta o vínculo ativo. |
| Professor toca em Salvar ficha | PlanPage.OnSave → PlanEditorViewModel.SaveAsync → AccountClient.SavePlanAsync → PlanEndpoints → PlanService. | POST ou PUT em `/teaching/students/{studentId}/plans` ou `…/plans/{id}`. MySQL mantém workout_plans, workout_plan_versions e workout_plan_items. |
| Aluno consulta e inicia | PlansPage carrega a lista. Iniciar treino busca a ficha corrente em `GET /plans/{id}` e abre WorkoutPage. | As séries previstas são preparadas em memória. |
| Aluno registra séries | WorkoutPage lê as séries marcadas, valida repetições/cargas e chama AccountClient. WorkoutEndpoints delega a WorkoutService. | `POST /sessions` grava workout_sessions e workout_session_sets com a revisão usada. |
| Ambos consultam a evolução | WorkoutsPage e WorkoutDetailsPage → AccountClient → WorkoutHistoryService. | `GET /history` para o aluno e `GET /teaching/students/{studentId}/history` para o professor vinculado. A consulta de detalhes do professor é de leitura. |
| Conta salva um modelo privado | Mesmo editor → PlanEditorViewModel → TemplateDao. | SQLite do aparelho. A ação Usar modelo preenche os campos da ficha atual e mantém a identidade desta. O envio ocorre ao salvar a ficha. |

## O que ficou mais simples

- A lista e o formulário de fichas também demonstram o CRUD local. O modo selecionado determina onde salvar. A View apresenta perguntas e feedback. A ViewModel valida o nome e as metas de cada exercício. O DAO executa toda a transação local.
- `ViewModels/ListViewModels.cs` centraliza a pesquisa do catálogo e do seletor. Ambos atualizam a lista pelo texto digitado. O seletor exclui exercícios já incluídos na ficha.
- O registro e a correção de sessões reutilizam `WorkoutSetItem.FromFields` para converter e validar repetições e carga.
- Os créditos exibidos no aplicativo e os distribuídos no ZIP agora vêm do mesmo arquivo. O projeto MAUI empacota o aviso da raiz.
- O catálogo dos modelos usa os identificadores dos 30 exercícios compartilhados da origem. A associação local dispensa copiar usuários, vínculos, sessões e históricos para o aparelho.

## Evidências e limites

| Verificação | Resultado |
|---|---|
| Compilação | API, testes, Android .NET 10 (APK Debug) e iOS Simulator arm64 compilaram. A compilação de iOS neste ambiente usou o linker no processo e assinatura local ad hoc para o simulador. |
| SQLite real, testes automatizados | Seis verificações passaram. Criação e reabertura, isolamento de contas, edição com versão, rollback por chave estrangeira inválida, recusa de exclusão com versão antiga e exclusão confirmada. O teste usa o mesmo arquivo-fonte do DAO que o app. |
| SQLite real, interface iOS | Criado modelo com exercício, marcado favorito e salvo. Após encerrar e reabrir o app, a busca encontrou o modelo. As repetições passaram de 10 para 12. Cancelar a exclusão preservou os dados e confirmar removeu o modelo. O professor salvou outro modelo privado e o aluno, na mesma instalação de teste, viu a própria lista vazia. Voltar da ficha modificada exibiu a confirmação de descarte. Cancelar no seletor manteve o editor com zero exercícios. O formulário recusou um nome vazio. Ao salvar, a lista exibiu a faixa temporária de confirmação. |
| Aproveitamento do modelo | Na interface do professor, selecionar o modelo copiou as metas para o editor da ficha. Na interface do aluno, a execução permitiu marcar uma série. As respostas HTTP para os dados autenticados vieram de uma compilação separada de teste. O SQLite e a interface usados nessa compilação são os mesmos do projeto entregue. |
| HTTP da API | Quinze casos da suíte existente passaram no processo com TestServer. Cobriram validação de cadastro, autenticação requerida, limitação de tentativas, liveness e readiness. Foi necessário executar os métodos com suas entradas diretamente porque o transporte do VSTest não conseguiu abrir socket neste ambiente. |
| Online com banco e usuários independentes | Tentado com o roteiro automatizado da entrega. O MySQL 9.6.0 encerrou em `--initialize-insecure` (saída 2, sinal 11 no log interno). O daemon do Docker estava indisponível. A primeira chamada HTTP de cadastro encontrou `Permission denied (localhost:5186)`. Por isso, a migração 011, a publicação da ficha, o registro da sessão, o histórico compartilhado e a negativa para outro professor ainda precisam ser executados com a instância isolada. A consistência das rotas e as verificações de vínculo foram conferidas por leitura. |
| Aparelhos | A interface foi executada no iPhone simulado. O Android foi compilado. ADB falhou ao iniciar o listener com `Operation not permitted`. A troca de dados entre dois aparelhos físicos está pendente. |

O PDF da disciplina tem três páginas. A matriz está em `Requisitos.md`. A cópia local dos materiais da turma inclui `MAUI_01.pptx`. O enunciado remete também aos demais exemplos do Drive, sem fixar um inventário fechado. Cabe ao professor confirmar a equivalência das abas, pilha e menu lateral do Shell com as classes dos exemplos da disciplina. O app não produz documentos impressos.
