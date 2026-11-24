"""Gera o relatório a partir de capturas reais em docs/mobile-final/capturas-finais.
Dependência: pip install reportlab
"""
from pathlib import Path
from reportlab.pdfgen import canvas
from reportlab.lib.colors import HexColor, white
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.platypus import Paragraph, Table, TableStyle
from reportlab.lib.utils import ImageReader

ROOT = Path(__file__).resolve().parents[1]
DEST = ROOT / 'docs/mobile-final/Workoutly-Mobile-Relatorio.pdf'
SHOTS = DEST.parent / 'capturas-finais'
W, H = A4
INK = HexColor('#161B17')
GREEN = HexColor('#C5F67A')
MUTED = HexColor('#465449')
c = canvas.Canvas(str(DEST), pagesize=A4)
c.setTitle('Workoutly - Trabalho Final Mobile - Versão local')
c.setAuthor('Workoutly')
body = ParagraphStyle('body', fontName='Helvetica', fontSize=10.5, leading=15, textColor=INK)
small = ParagraphStyle('small', parent=body, fontSize=9, leading=12)

def text(value, x, top, width, style=body):
    p = Paragraph(value, style)
    _, height = p.wrap(width, H)
    p.drawOn(c, x, top-height)
    return top-height

def page(number, title, subtitle):
    c.setFillColor(INK)
    c.rect(0, H-92, W, 92, fill=1, stroke=0)
    c.setFillColor(GREEN)
    c.setFont('Helvetica-Bold', 10)
    c.drawString(42, H-28, 'WORKOUTLY / DESENVOLVIMENTO MOBILE')
    c.setFillColor(white)
    c.setFont('Helvetica-Bold', 22)
    c.drawString(42, H-59, title)
    c.setFont('Helvetica', 9)
    c.drawString(42, H-77, subtitle)
    c.setStrokeColor(HexColor('#DCE3DD'))
    c.line(42, 40, W-42, 40)
    c.setFillColor(MUTED)
    c.setFont('Helvetica', 8)
    c.drawString(42, 25, 'Versão 0.13.0 | 22/11/2026 | Base: backup 498005d')
    c.drawRightString(W-42, 25, str(number))

def screen(filename, x, y, height):
    img=ImageReader(str(SHOTS/filename))
    iw, ih=img.getSize()
    c.drawImage(img, x, y, width=height*iw/ih, height=height)

page(1, 'Planejamento local do aluno', 'Quatro telas. SQLite no aparelho. Sem conta ou servidor.')
y=text('O Workoutly permite criar, consultar, editar e excluir fichas com exercícios, séries, repetições e cargas de referência. A simplificação retirou professor, vínculo, autenticação, execução e histórico de treinos, além da API e do MySQL.',42,H-119,W-84)
y=text('<b>Fluxo:</b> Início → Minhas fichas → Criação/edição → Seleção de exercícios (modal). O seletor retorna ao editor ao incluir ou cancelar. Menu lateral e abas levam aos mesmos destinos.',42,y-16,W-84)
y=text('<b>Organização do código</b>',42,y-23,W-84)
rows=[['Camada','Responsabilidade'],['View (XAML / code-behind)','Interface, diálogos e navegação.'],['ViewModels','Campos, validação, busca e filtro de favoritas.'],['Models','Ficha, exercício e item do planejamento.'],['TemplateDao','SQLite: consulta, transação e exclusão em cascata.']]
rows=[[Paragraph(cell,small) for cell in row] for row in rows]
t=Table(rows,colWidths=[160,W-244]);t.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,0),GREEN),('VALIGN',(0,0),(-1,-1),'TOP'),('LEFTPADDING',(0,0),(-1,-1),9),('RIGHTPADDING',(0,0),(-1,-1),9),('TOPPADDING',(0,0),(-1,-1),8),('BOTTOMPADDING',(0,0),(-1,-1),8),('LINEBELOW',(0,0),(-1,-1),0.4,HexColor('#DCE3DD'))]));_,th=t.wrap(W-84,H);t.drawOn(c,42,y-th-8);y-=th+29
y=text('<b>Três tabelas SQLite</b><br/>ModeloFicha guarda nome, observações, favorito e versão. ExercicioLocal contém 30 exercícios de referência. ItemModelo liga ficha e exercício e guarda ordem, séries, repetições e carga em centésimos de kg. O arquivo privado workoutly.db3 pertence à instalação.',42,y,W-84)
y=text('<b>Verificação</b><br/>Nove verificações automatizadas passaram: CRUD, persistência, transação, conflito de versão e filtro com pesquisa. Builds iOS e Android passaram. Android teve um aviso XA0033 de interpretação da versão do Java. O fluxo foi executado no iPhone 18 Pro Simulator com iOS 27.0; execução Android e aparelhos físicos não foram verificados.',42,y-20,W-84)
y=text('<b>Componentes</b><br/>Label, Entry, Editor, Button, CheckBox, RadioButtonGroup, Image, CollectionView, Grid, faixa de confirmação temporária e ContentPage modal. Shell fornece menu lateral, abas e navegação empilhada. Não utiliza as classes literais FlyoutPage, TabbedPage e NavigationPage; a equivalência deve ser confirmada na avaliação.',42,y-20,W-84)
text('Fontes: código da versão local e teste no simulador em 22/11/2026. Capturas reais com dados fictícios. Créditos das imagens/fontes em THIRD-PARTY-NOTICES.md. Uso de IA: apoio na implementação, testes e elaboração deste relatório.',42,92,W-84,small)
c.showPage()
page(2,'Início e lista de fichas','As duas abas organizam o acesso ao planejamento.')
screen('01-inicio.jpg',42,184,520)
screen('02-fichas.jpg',314,184,520)
text('<b>1. Início</b><br/>O botão abre as fichas locais. Não há tela de login. O menu lateral e as abas mantêm os destinos acessíveis.',42,161,239,small)
text('<b>2. Minhas fichas</b><br/>Busca por digitação, Nova ficha e abertura para edição. O RadioButton alterna Todas / Só favoritas. A estrela identifica a favorita.',314,161,239,small)
text('Fonte: aplicativo executado no iPhone 18 Pro Simulator, iOS 27.0. Ficha de demonstração fictícia.',42,73,W-84,small)
c.showPage()
page(3,'Editor e seleção de exercícios','O planejamento é gravado somente ao salvar a ficha.')
screen('03-editor.jpg',42,184,520)
screen('04-selecao.jpg',314,184,520)
text('<b>3. Criação/edição</b><br/>Nome, observações, CheckBox Favorita e Grid com séries, repetições e carga. Subir, Descer e Remover organizam os itens. Role para salvar, recarregar ou excluir.',42,161,239,small)
text('<b>4. Seleção modal</b><br/>Pesquisa por nome, músculo ou equipamento. As imagens são locais. Incluir retorna ao editor; Cancelar preserva a ficha. Itens já incluídos ficam ocultos.',314,161,239,small)
text('Fonte: capturas reais do mesmo teste. O editor é rolável; a imagem mostra a parte superior do formulário.',42,73,W-84,small)
c.showPage()
page(4,'Validação e confirmação','Proteção dos campos em edição e das exclusões locais.')
screen('05-validacao.jpg',42,184,520)
screen('06-exclusao.jpg',314,184,520)
text('<b>Validação antes de gravar</b><br/>Nome: 2-120 caracteres; 1-50 exercícios; 1-20 séries; 1-200 repetições; carga opcional de 0-2000 kg, até duas casas decimais.',42,161,239,small)
text('<b>Confirmação de exclusão</b><br/>Cancelar mantém a ficha. Excluir remove ficha e itens. O teste também confirmou quatro séries após encerrar e reabrir o app.',314,161,239,small)
text('O relatório descreve os resultados observados. Não comprova avaliação com participantes nem aceitação das equivalências pelo professor. Evidências detalhadas em docs/mobile-final/Verificacao.md.',42,80,W-84,small)
c.save()
print(DEST)
