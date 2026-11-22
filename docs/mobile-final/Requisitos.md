# Enunciado e demonstração

## Diagnóstico antes de adaptar

| Requisito e página | Situação da origem | Evidência |
|---|---|---|
| Controles e navegações, p. 1 | parcial | ExercisesPage, WorkoutPage, AppShell. Catálogo com imagens e lista, execução com CheckBox, Shell com uma entrada. Sem rádio, snackbar, menu lateral com destinos ou abas. |
| CRUD, pp. 1–2 | atendido no servidor | PlanPage, PlansPage, AccountClient e PlanService. Excluir ficha arquiva revisões para os treinos anteriores. |
| Banco no aparelho, três tabelas e DAO, p. 2 | ausente | Cliente HTTP e SecureStorage para sessão. Formulários e listas em memória. |
| MVVM, p. 2 | ausente nas fichas e no catálogo | PlanPage.OnSave lia controles e validava cada linha. Duas páginas filtravam exercícios com o mesmo algoritmo. |
| Pesquisa, edição e confirmação, p. 2 | atendido para o catálogo e a ficha do servidor | ExercisesPage filtra durante digitação e PlanPage confirma exclusão. |

## Situação da cópia

| Requisito | Situação | Arquivo ou tela | Como demonstrar |
|---|---|---|---|
| Label e campo de texto (p. 1) | atendido | PlanPage.xaml | Digitar nome e observações. |
| Button (p. 1) | atendido | PlanPage | Salvar modelo. |
| CheckBox (p. 1) | atendido | PlanPage e WorkoutPage | Marcar favorito ou série feita. |
| RadioButton / RadioGroup (p. 1) | atendido | MainPage | Alternar aluno/professor no grupo perfil e continuar. |
| ImageView (p. 1) | atendido no MAUI | ExercisesPage | Ver a ilustração em Image. |
| ListView / RecyclerView (p. 1) | atendido no MAUI | PlansPage | Abrir um registro na CollectionView. |
| Toast / Snackbar (p. 1) | atendido | PlansPage.ShowMessage | Salvar e observar a faixa temporária de confirmação na lista. |
| Grid (p. 1) | atendido | PlanPage | Séries, repetições e carga ocupam colunas. |
| ContentPage modal (p. 1) | atendido | PlanExercisePickerPage | Adicionar exercício e escolher Incluir ou Cancelar. |
| Flyout (p. 1) | atendido em Shell | AppShell | Abrir o menu lateral e navegar para o catálogo. |
| Navigation Pages (p. 1) | parcial · equivalência a confirmar | PlansPage → PlanPage | Abrir ficha e voltar. Se o professor exigir a classe literal, confirmar equivalência. |
| Tabbed Pages (p. 1) | parcial · equivalência a confirmar | AppShell | Alternar Início e Modelos. |
| Demais exemplos da turma (p. 1) | não verificado | MAUI_01.pptx, slides 10–11, e Drive da turma | Mostrar DatePicker e Picker no histórico, Switch no catálogo e ActivityIndicator no carregamento. O enunciado não delimita os controles adicionais. |
| Create, Read, Update, Delete locais (pp. 1–2) | atendido | Modelos, PlanEditorViewModel, TemplateDao | Criar, listar, abrir, editar e excluir o mesmo modelo. |
| Persistência local (p. 2) | atendido | TemplateDao | Reiniciar o processo e recuperar o registro. |
| Três tabelas e texto, número, booleano (p. 2) | atendido | ModeloFicha, ExercicioLocal, ItemModelo | Nome, quantidade de séries, favorito que ordena a lista. |
| DAO (p. 2) | atendido | Data/TemplateDao.cs | Acompanhar Save. Cabeçalho e itens são gravados na mesma transação. |
| Tela inicial e navegação clara (p. 2) | atendido | MainPage / AppShell | Abrir destinos de cada perfil. |
| Cadastros com validação (p. 2) | atendido | PlanEditorViewModel, ExerciseFields, AuthEndpoints | Tentar salvar sem nome ou informar 21 séries. |
| Pesquisa em tempo real (p. 2) | atendido | PlansViewModel / CatalogViewModel | Digitar parte do nome sem apertar Buscar. |
| Edição e confirmação de exclusão (p. 2) | atendido | PlanPage | Editar série, cancelar exclusão e depois confirmar. |
| MVVM (p. 2) | parcial no projeto inteiro | ViewModels/, Models/PlanItem.cs | Seguir o botão de salvar pelo README. As demais páginas do PI preservam code-behind. |
| Avaliação · componentes corretos e variados (p. 2) | avaliação em aula | Relatório e telas acima | Alternar perfil, escolher exercício, marcar série e usar as abas. |
| Avaliação · CRUD completo (p. 2) | avaliação em aula | Modelos e TemplateDao | Demonstrar as quatro operações sobre um único registro. |
| Avaliação · organização e clareza (p. 2) | avaliação em aula | PlanPage, PlanEditorViewModel, PlanItem e TemplateDao | Explicar quem lê os campos, valida e grava. |
| Avaliação · usabilidade (p. 2) | avaliação em aula | Pesquisa, faixa de confirmação e diálogos | Pesquisar por digitação, voltar com alterações e cancelar uma exclusão. |
| Fonte e instruções (pp. 2–3) | atendido | ZIP, solução e README | Extrair e abrir Workoutly.slnx. |
| PDF com funcionalidades e telas (pp. 2–3) | parcial · comprovação online pendente | docs/mobile-final/ | Conferir as legendas e a origem das capturas. |
| Exemplos de documentos impressos (p. 3) | Não se aplica | Aplicativo | O aplicativo não gera impressos. |
| Envio e apresentação (p. 3) | A cargo do aluno | ZIP | Enviar o projeto compactado na pasta da disciplina. Apresentação em 24/11 para 2024T1 e 25/11 para 2024T2. |
