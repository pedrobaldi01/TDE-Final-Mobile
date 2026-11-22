# Workoutly · Trabalho Final de Desenvolvimento Mobile

Abra **Workoutly.slnx** no Rider ou Visual Studio e selecione **Workoutly.Mobile** como projeto de inicialização. Extraia o ZIP inteiro antes de abrir a solução.

O professor vincula alunos e publica fichas. O aluno registra o treino e ambos consultam o histórico. A aba **Modelos** guarda planejamentos reutilizáveis no próprio aparelho. O envio ao aluno acontece quando o professor abre uma ficha, escolhe um modelo e toca em **Salvar ficha**.

## Executar

Use SDK .NET 10.0.401 (ou patch compatível), workloads MAUI do projeto, Android SDK 36 e JDK 21 para Android, ou macOS com Xcode e simulador compatíveis para iOS. A API incluída usa MySQL 9.6. Na raiz:

```sh
dotnet workload restore src/Workoutly.Mobile/Workoutly.Mobile.csproj
dotnet restore Workoutly.slnx
```

**Mac com MySQL nativo:** execute `python3 scripts/dev-native.py` e mantenha o terminal aberto. O helper gera senhas em `.env`, prepara uma instância própria em `.local/`, aplica as migrações e inicia a API na porta **5186**. Ctrl+C encerra a instância preservando os dados.

**Alternativa com Docker Compose** (Windows, macOS ou Linux, com Python 3 e Docker):

```sh
python3 scripts/dev.py init
docker compose up -d --wait
python3 scripts/dev.py migrate
python3 scripts/dev.py run
```

No Windows use `py` caso esse seja o seu comando de Python. O Compose da cópia usa projeto `workoutly-mobile-final`, volume próprio e porta MySQL **3317**. Confira no computador **http://localhost:5186/health/ready**. O esperado é HTTP 200 e esquema 11. Só então rode o app pela IDE e cadastre contas fictícias de aluno e professor.

**Endereço da API** em `src/Workoutly.Mobile/Services/ApiSettings.cs`. O padrão aponta para a máquina de desenvolvimento pelo simulador iOS (`http://localhost:5186/`) ou pelo emulador Android (`http://10.0.2.2:5186/`). Em dois aparelhos físicos, configure nos dois builds a mesma URL HTTPS de uma **instância isolada**, com certificado confiável e banco próprio. O serviço precisa estar acessível aos dois telefones. Cada instalação usa o identificador independente `br.edu.workoutly.mobile.finalapp`.

## Demonstrar em sala

1. Entre como professor. Em **Início → Meus alunos**, gere o código. No outro aparelho, entre como aluno e confirme o código em **Meu professor**.
2. **Modelos → Novo modelo**. Dê um nome, marque favorito, abra **Adicionar exercício**, escolha **Incluir**, ajuste séries/repetições e salve. Feche e reabra, pesquise enquanto digita e abra para editar. O catálogo dos modelos contém os 30 exercícios compartilhados do PI com os mesmos identificadores.
3. **Início → Montar fichas → Fichas do aluno → Nova ficha → Usar modelo nesta ficha**. Escolha o modelo, revise e salve. Só este último botão publica o plano. Se for modificar uma ficha existente, o servidor cria uma revisão.
4. No celular do aluno, entre em **Minhas fichas** ou toque em **Atualizar**. Inicie o treino, registre uma série, marque **Feita** e conclua. Abra **Histórico e evolução → Ver treino**. O professor pode consultar o histórico do aluno vinculado, somente para leitura.
5. Volte a **Modelos**, abra o registro, cancele a exclusão, depois confirme. O modelo sai do SQLite. A ficha já publicada e o histórico continuam no servidor. Confira que uma segunda conta no mesmo aparelho tem sua própria lista.

## Seguir o código

| Ação | Onde acompanhar |
|---|---|
| Salvar modelo | `PlanPage.xaml` → `OnSave` → `PlanEditorViewModel.SaveAsync` → `TemplateDao.Save` → transação SQLite. |
| Salvar ficha real | A mesma View e ViewModel → `AccountClient.SavePlanAsync` → `PlanEndpoints` → `PlanService` → MySQL. |
| Retornar à lista | `PlansPage.OnAppearing` → `PlansViewModel.LoadAsync` → a lista ligada a `Visible` é atualizada. |
| Buscar exercício | `ExercisesPage` e `PlanExercisePickerPage` usam `CatalogViewModel`. |
| Registrar e consultar série | `WorkoutPage` → `AccountClient` → `WorkoutService`. A consulta de `WorkoutsPage` passa por `WorkoutHistoryService`. |

O **Model** é `Models/PlanItem.cs`. O SQLite fica no diretório privado `FileSystem.AppDataDirectory/modelos-<hash>.db3`. O hash usa endereço do serviço e ID da conta autenticada. `ModeloFicha` guarda nome, observações, favorito e versão. `ExercicioLocal` contém o catálogo reutilizável. `ItemModelo` liga os dois e guarda posição, séries, repetições e carga em centésimos de kg. `SecureStorage` guarda a sessão da conta. Uma sessão previamente autenticada e ainda válida pode abrir seus modelos quando a API está fora do ar.

## Verificar e comparar

```sh
dotnet run --project tests/Workoutly.Local.Tests
dotnet run --project tests/Workoutly.Local.Tests -- --online
dotnet test tests/Workoutly.Api.Tests
```

O segundo comando usa somente `localhost:5186` e cadastra três contas descartáveis na instância de demonstração. Para incluir todos os testes MySQL da suíte, use `python3 scripts/test-native-mysql.py` em um ambiente com o servidor nativo.

A branch **origem-pi** contém a árvore original. A branch **trabalho-final-mobile** contém a entrega. Compare com `git diff origem-pi..trabalho-final-mobile`. Consulte o [PDF com as telas](docs/mobile-final/Workoutly-Mobile-Relatorio.pdf), a [matriz do enunciado](docs/mobile-final/Requisitos.md) e o [resumo das origens, mudanças e testes](docs/mobile-final/Verificacao.md). Os demais documentos em `docs/` registram etapas históricas do PI. Os créditos originais estão em `THIRD-PARTY-NOTICES.md` e `docs/licenses/`.
