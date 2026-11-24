# Workoutly · Trabalho Final de Desenvolvimento Mobile

Aplicativo local do aluno, simplificado a partir do backup `498005d`. São quatro telas: **Início**, **Minhas fichas**, **Criação/edição da ficha** e **Seleção de exercícios**. Não exige conta, internet ou servidor.

## Executar

Abra **Workoutly.slnx** no Rider ou Visual Studio e selecione **Workoutly.Mobile** como projeto de inicialização. Extraia o ZIP inteiro antes de abrir. Requer .NET SDK 10.0.401, MAUI 10.0.110 e, para Android, SDK 36 e JDK 21. Para iOS, use macOS e Xcode compatível.

```sh
dotnet workload restore src/Workoutly.Mobile/Workoutly.Mobile.csproj
dotnet restore Workoutly.slnx
dotnet build src/Workoutly.Mobile/Workoutly.Mobile.csproj -f net10.0-android
dotnet build src/Workoutly.Mobile/Workoutly.Mobile.csproj -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64
```

Neste Mac o SDK correto está em `~/.dotnet/dotnet`. Se `dotnet --list-sdks` não listar 10.0.401, use esse executável ou ajuste o PATH. O Android SDK deste Mac está em `~/Library/Developer/Android/sdk`; selecione JDK 21 na IDE ou passe `-p:JavaSdkDirectory=... -p:AndroidSdkDirectory=...` no build.

## Demonstrar

1. No início, toque em **Abrir minhas fichas**, use a aba **Fichas** ou o menu lateral.
2. Toque em **Nova ficha**, informe nome e observações e marque a CheckBox **Favorita**.
3. Em **Adicionar exercício**, pesquise por nome, grupo muscular ou equipamento e toque em **Incluir**. **Cancelar** fecha a seleção sem alterar a ficha.
4. Ajuste séries, repetições e carga opcional; use Subir, Descer ou Remover. Toque em **Salvar ficha**. A lista mostra uma confirmação temporária.
5. Pesquise enquanto digita e alterne **Todas / Só favoritas**. Abra a ficha, edite e salve. Encerre e reabra o app para verificar a persistência.
6. Abra a ficha e toque em **Excluir ficha**. Primeiro cancele; depois confirme. Voltar com alterações sem salvar também pede confirmação.

O nome exige 2 a 120 caracteres. A ficha aceita 1 a 50 exercícios diferentes, 1 a 20 séries, 1 a 200 repetições e carga opcional de 0 a 2000 kg com até duas casas decimais. Os dados usados nas capturas são fictícios.

## Organização

| Camada | Responsabilidade |
|---|---|
| Views XAML e code-behind | Quatro telas, diálogos, navegação e feedback. |
| `ViewModels/` | Campos, validações, busca e filtro de favoritas. |
| `Models/PlanItem.cs` | Ficha, item da ficha e exercício do catálogo. |
| `Data/PlanDao.cs` | Consultas e transações SQLite; único ponto de acesso ao banco. |

Salvar percorre `PlanPage.OnSave` → `PlanEditorViewModel.Save` → `PlanDao.Save`. A lista recarrega em `PlansPage.OnAppearing` e atualiza o binding `Visible`. A seleção modal usa `CatalogViewModel`, ocultando exercícios já incluídos.

O arquivo privado `FileSystem.AppDataDirectory/workoutly.db3` pertence à instalação. São três tabelas: **ModeloFicha** (nome, observações, favorito e versão), **ExercicioLocal** (catálogo inicial de 30 movimentos) e **ItemModelo** (relação, ordem, séries, repetições e carga em centésimos de kg). Os nomes das tabelas foram preservados do DAO do backup. Salvar cabeçalho e itens é uma transação; excluir remove os itens em cascata. A versão detecta edição desatualizada. Não há sincronização ou migração dos antigos arquivos por conta; eles permanecem intactos.

O Shell fornece menu lateral, abas Início/Fichas e navegação empilhada da lista ao editor. O seletor é uma ContentPage modal. Não são instâncias literais de FlyoutPage, TabbedPage ou NavigationPage; essa equivalência está registrada na matriz dos requisitos.

## Verificar

```sh
dotnet run --project tests/Workoutly.Local.Tests
```

Builds iOS e Android e nove verificações automatizadas passaram após a limpeza em 23/11/2026. O fluxo foi exercitado no iPhone 18 Pro Simulator, iOS 27.0. O build Android produziu um aviso XA0033 ao interpretar a versão do Java; execução Android e aparelhos físicos não foram verificados.

Consulte o [PDF com as telas finais](docs/mobile-final/Workoutly-Mobile-Relatorio.pdf), a [matriz de requisitos](docs/mobile-final/Requisitos.md) e as [evidências](docs/mobile-final/Verificacao.md). Para regenerar o PDF, use Python 3.10 ou superior, instale `reportlab` e execute `python3 scripts/gerar-relatorio.py`.

A API, o MySQL e os fluxos de professor, vínculo, autenticação, execução e histórico foram retirados da entrega. O histórico do PI está no Git. O commit `8826ddd` guarda a árvore anterior à limpeza. Créditos das imagens e fontes em `THIRD-PARTY-NOTICES.md` e `docs/licenses/`.
