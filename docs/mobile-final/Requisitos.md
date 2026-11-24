# Enunciado e demonstração - versão local

Versão 0.13.0, simplificada a partir de `498005d` em 22/11/2026. Quatro telas: início, lista de fichas, criação/edição e seleção de exercícios. A aplicação é local, sem conta, professor ou servidor. Documentos do PI na pasta superior não foram alterados.

| Requisito do enunciado | Implementação final | Como demonstrar |
|---|---|---|
| Label e campo de texto, p. 1 | PlanPage: Label, Entry e Editor | Digitar nome e observações. |
| Button, p. 1 | Todas as telas | Abrir fichas, adicionar exercício e salvar. |
| CheckBox, p. 1 | PlanPage | Marcar a ficha como favorita. |
| RadioButton / RadioGroup, p. 1 | PlansPage; RadioButtonGroup | Alternar **Todas / Só favoritas**. |
| ImageView, p. 1 | Image MAUI no início e no seletor | Ver marca e ilustrações locais. |
| ListView / RecyclerView, p. 1 | CollectionView MAUI na lista e no seletor | Abrir uma ficha e escolher um exercício. |
| Toast / Snackbar, p. 1 | Faixa temporária em PlansPage.ShowMessage | Salvar e observar “Ficha salva neste aparelho”. |
| Grid, p. 1 | PlanPage | Séries, repetições e carga em três colunas. |
| ContentPage modal, p. 1 | PlanExercisePickerPage | Incluir ou cancelar, retornando ao editor. |
| Flyout, p. 1 | Shell com menu lateral | Abrir menu e escolher Início ou Minhas fichas. |
| Navigation Pages, p. 1 | Pilha do Shell: PlansPage → PlanPage | Abrir e voltar; confirmar descarte se houver alterações. |
| Tabbed Pages, p. 1 | Shell com Tab Início e Tab Fichas | Alternar as abas. |
| Demais exemplos da turma, p. 1 | Não delimitados pelo enunciado disponível | O material externo da turma não foi revalidado; não se afirma cobertura de todos os exemplos. |
| CRUD, pp. 1-2 | PlanEditorViewModel + TemplateDao | Criar, listar, editar e excluir uma ficha. |
| Persistência no aparelho, p. 2 | workoutly.db3 em AppDataDirectory | Encerrar o processo, reabrir e recuperar a ficha. |
| Três tabelas, p. 2 | ModeloFicha, ExercicioLocal, ItemModelo | Ficha e catálogo ligados pelos itens. |
| Texto, número e booleano, p. 2 | Nome; séries/repetições/carga; favorito | Alterar os valores, salvar e reabrir. Favorito também filtra e ordena a lista. |
| DAO, p. 2 | Data/TemplateDao.cs | Save usa transação; Delete remove itens por cascata. |
| Tela inicial e navegação, p. 2 | MainPage e AppShell | Abrir fichas pelo botão, aba ou menu. |
| Cadastros com validação, p. 2 | PlanEditorViewModel e ExerciseFields | Nome vazio, ficha sem exercício ou séries fora de 1-20 são recusados. |
| Pesquisa em tempo real, p. 2 | PlansViewModel e CatalogViewModel | Digitar parte do nome sem botão Buscar. |
| Edição e confirmação de exclusão, p. 2 | PlanPage | Editar séries; cancelar exclusão e depois confirmar. |
| MVVM, p. 2 | Models/, ViewModels/ e Views XAML | A ViewModel lê os bindings, valida e chama o DAO. Code-behind cuida da apresentação e navegação. |
| Fonte e instruções, pp. 2-3 | Workoutly.slnx e README | Extrair a pasta inteira e abrir a solução. |
| PDF com funcionalidades e telas, pp. 2-3 | Workoutly-Mobile-Relatorio.pdf | Capturas reais da versão local no iOS Simulator, com dados fictícios. |
| Exemplos impressos, p. 3 | Não se aplica | O aplicativo não produz impressos. |
| Envio e apresentação, p. 3 | A cargo do aluno | Compactar e enviar à disciplina; não houve publicação automática. |

**Equivalências a confirmar na avaliação:** o projeto utiliza Shell para menu, abas e pilha, não as classes literais FlyoutPage, TabbedPage e NavigationPage. CollectionView e Image são os controles MAUI equivalentes aos exemplos Android. A confirmação temporária é uma faixa XAML, sem plugin de Snackbar. Isso já fazia parte da adaptação; não é comprovação de aceitação pelo professor.

## Fluxo e persistência

A lista inicia vazia. Os 30 exercícios são catálogo de referência, não fichas reais. A ficha exige nome de 2-120 caracteres e 1-50 exercícios diferentes. Cada item exige 1-20 séries, 1-200 repetições e carga opcional de 0-2000 kg com duas casas decimais. Fichas favoritas aparecem primeiro. Pesquisa e filtro são combinados.

O banco é único por instalação e não depende de conta. Os arquivos antigos por conta não são convertidos nem apagados. Professor, vínculo, autenticação, execução e histórico de treinos foram removidos, assim como a API e os scripts MySQL. As evidências atuais estão em Verificacao.md; documentos anteriores do PI são históricos.
