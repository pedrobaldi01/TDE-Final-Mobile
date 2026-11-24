# Arquitetura da entrega Mobile

O app MAUI tem quatro telas. A View XAML liga os campos e as listas às ViewModels. O code-behind faz navegação e diálogos. As ViewModels filtram, validam e usam `PlanDao` para acessar o SQLite. O DAO prepara o catálogo e grava cada ficha e seus itens em uma transação.

`workoutly.db3` fica no diretório privado do app. O esquema tem três tabelas. `ModeloFicha` guarda o cabeçalho, `ExercicioLocal` guarda o catálogo e `ItemModelo` relaciona ambos. A lista é lida ao entrar na tela. O seletor é modal e acrescenta um exercício ao formulário em memória. **Salvar ficha** grava o conjunto.

A solução inclui o app e o projeto de verificações locais. Consulte o [README](../README.md) para executar e a [matriz](mobile-final/Requisitos.md) para os controles da disciplina. A arquitetura do PI está no histórico Git.
