# Simplificação local - 22/11/2026

Base: backup `498005d`. Versão resultante: 0.13.0, com quatro telas e SQLite/DAO/MVVM, sem API ou autenticação.

- iOS Simulator arm64: build sem avisos ou erros; fluxo executado no iPhone 18 Pro, iOS 27.0.
- Android: build com êxito, um aviso XA0033 ao interpretar a versão do JDK 21; execução não verificada.
- Nove verificações locais passaram: CRUD persistente, rollback, conflito de versão e pesquisa combinada com favoritas.
- Capturas reais e PDF final renderizado e inspecionado em quatro páginas.
- Sem teste em aparelhos físicos, participantes ou auditoria de dependências.

Comandos, sequência observada e limites completos em [Verificacao.md](../mobile-final/Verificacao.md). Critérios e equivalências dos componentes em [Requisitos.md](../mobile-final/Requisitos.md).

## Limpeza de 23/11/2026

A limpeza retirou os documentos e as capturas do PI, os recursos sem consumidores e a configuração de plataforma que só servia ao fluxo antigo. O histórico continua acessível pelo commit `8826ddd`. O `PlanDao` (antes `TemplateDao`) mantém os nomes das tabelas e o arquivo SQLite. A seleção carrega o catálogo em segundo plano e a ViewModel do editor chama o DAO para gravar ou excluir.

O enunciado local, os slides MAUI_01 e os exemplos da turma foram consultados. A matriz registra a diferença entre o Shell e as classes de navegação demonstradas em aula.

- Nove verificações passaram com SQLite real em diretório temporário.
- iOS Simulator arm64 compilou com zero avisos e erros. No iPhone 18 Pro (iOS 27.0), a busca por “supino” mostrou três exercícios com imagens. Inclusão, salvamento, filtro de favoritas, reabertura e exclusão da ficha fictícia foram conferidos.
- Android compilou após limpar `bin` e `obj`. O build terminou com um aviso XA0033 e zero erros. A execução foi verificada no iOS Simulator.
- O PDF foi regenerado com a referência a `PlanDao` e conferido após renderização.
