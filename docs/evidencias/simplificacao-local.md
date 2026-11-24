# Simplificação local - 22/11/2026

Base: backup `498005d`. Versão resultante: 0.13.0, com quatro telas e SQLite/DAO/MVVM, sem API ou autenticação.

- iOS Simulator arm64: build sem avisos ou erros; fluxo executado no iPhone 18 Pro, iOS 27.0.
- Android: build com êxito, um aviso XA0033 ao interpretar a versão do JDK 21; execução não verificada.
- Nove verificações locais passaram: CRUD persistente, rollback, conflito de versão e pesquisa combinada com favoritas.
- Capturas reais e PDF final renderizado e inspecionado em quatro páginas.
- Sem teste em aparelhos físicos, participantes ou auditoria de dependências.

Comandos, sequência observada e limites completos em [Verificacao.md](../mobile-final/Verificacao.md). Critérios e equivalências dos componentes em [Requisitos.md](../mobile-final/Requisitos.md).
