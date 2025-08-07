# 📋 GitHub Projects Configuration

## 🎯 **Projeto: @rodrigomj87's Clipper**

### **📊 Views Configuradas**

#### 1. **📋 Board View (Kanban)**
- **Colunas**:
  - 📝 **Backlog** - Issues planejadas
  - 🔄 **Ready** - Prontas para desenvolvimento
  - 🚧 **In Progress** - Em desenvolvimento
  - 👀 **In Review** - Em revisão/teste
  - ✅ **Done** - Concluídas

#### 2. **📈 Sprint View**
- **Filtros**:
  - Sprint 1: `label:sprint-1`
  - Sprint 2: `label:sprint-2`
  - Sprint 3: `label:sprint-3`

#### 3. **🔍 By Priority**
- **Ordenação**: Priority (High → Low)
- **Grouping**: Por prioridade
- **Filtros**: Apenas issues abertas

### **🏷️ Labels System**

#### **📦 Tipo de Issue**
- `epic` - Épicos principais
- `feature` - Novas funcionalidades
- `bug` - Bugs e problemas
- `task` - Tarefas técnicas
- `milestone` - Marcos do projeto

#### **🎯 Sprints**
- `sprint-1` - Sprint 1: Foundation
- `sprint-2` - Sprint 2: Core Features
- `sprint-3` - Sprint 3: AI Integration

#### **🔧 Componentes**
- `backend` - Backend/.NET
- `frontend` - Frontend/Angular
- `database` - Banco de dados
- `api` - APIs/Integrações
- `ai-integration` - IA/Processamento
- `ui/ux` - Interface do usuário

#### **📊 Prioridade**
- `critical` - Crítico (bloqueia sistema)
- `high` - Alta (funcionalidade importante)
- `medium` - Média (normal)
- `low` - Baixa (pode esperar)

#### **🔄 Status**
- `ready-for-dev` - Pronto para desenvolvimento
- `in-progress` - Em progresso
- `needs-review` - Precisa revisão
- `testing` - Em teste
- `blocked` - Bloqueado

#### **🔬 Técnicas**
- `ef-core` - Entity Framework
- `signalr` - SignalR/Real-time
- `hangfire` - Background jobs
- `testing` - Testes
- `documentation` - Documentação
- `refactoring` - Refatoração

### **📏 Story Points System**

#### **Fibonacci Scale**
- **1 ponto** - Muito simples (< 1 hora)
- **2 pontos** - Simples (1-2 horas)
- **3 pontos** - Pequeno (meio dia)
- **5 pontos** - Médio (1 dia)
- **8 pontos** - Grande (2-3 dias)
- **13 pontos** - Muito grande (1 semana)
- **21 pontos** - Épico (várias semanas)

### **🔄 Workflow Automation**

#### **Auto-move Rules**
1. **Issue criada** → Backlog
2. **Label "ready-for-dev"** → Ready
3. **Assigned + In Progress** → In Progress
4. **PR linked** → In Review
5. **PR merged** → Done

#### **Auto-labeling**
1. **Issues com [BUG]** → `bug` label
2. **Issues com [FEATURE]** → `feature` label
3. **Issues com [TASK]** → `task` label

### **📊 Sprint Planning**

#### **Sprint 1 (21 pontos)**
- 🎯 **Objetivo**: Foundation & Setup
- 📅 **Duração**: 2-3 semanas
- 🔗 **Issues**: #1, #2, #10

#### **Sprint 2 (34 pontos)**
- 🎯 **Objetivo**: Core Features
- 📅 **Duração**: 3-4 semanas
- 🔗 **Issues**: #3, #4, #5, #6, #7, #11

#### **Sprint 3 (55 pontos)**
- 🎯 **Objetivo**: AI Integration
- 📅 **Duração**: 4-5 semanas
- 🔗 **Issues**: #8, #9, #12

### **📋 Daily Workflow**

#### **Desenvolvedor**
1. Verificar **"Ready"** column
2. Mover para **"In Progress"**
3. Desenvolver feature/fix
4. Criar PR (auto-move para "In Review")
5. Merge PR (auto-move para "Done")

#### **Project Manager**
1. Triagem de novas issues (Backlog)
2. Planning de sprint
3. Review de progress
4. Update de prioridades

### **🔍 Queries Úteis**

#### **Issues em Progresso**
```
is:issue is:open label:in-progress assignee:rodrigomj87
```

#### **Ready for Development**
```
is:issue is:open label:ready-for-dev no:assignee
```

#### **Sprint Atual**
```
is:issue is:open label:sprint-1 sort:updated-desc
```

#### **Bugs Críticos**
```
is:issue is:open label:bug label:critical
```

### **📈 Métricas de Acompanhamento**

#### **Velocity**
- Sprint 1: X pontos concluídos
- Sprint 2: Y pontos concluídos
- Sprint 3: Z pontos concluídos

#### **Lead Time**
- Tempo médio: Ready → Done
- Tempo de review: In Review → Done

#### **Throughput**
- Issues por semana
- Story points por sprint