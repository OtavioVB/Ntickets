Funcionalidade: Criação de Tenant/Whitelabel (Contratante)

A short summary of the feature

@tenants
Cenário: Contratante criado com Sucesso
	Dado um usuário anônimo da plataforma com informações válidas de <FantasyName>, <LegalName>, <Document>, <Email> e <Phone>
	Quando enviar a solicitação de criação da contratação
	Então a criação do contratante deve ser realizado com sucesso
	E o status de utilização deve constar como PENDING_ANALYSIS
	E a mensagem de notificação de sucesso 'A criação do whitelabel foi executada com sucesso.' com código 'CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL' deve ser retornada
	Exemplos: 
		| FantasyName | LegalName            | Document       | Email                      | Phone         |
		| Eventos     | Eventos LTDA         | 21330277000152 | eventos-pj@ntickets.com.br | 5511958523443 |
		| Eventos     | Otavio Pessoa Fisica | 65360341041    | eventos-pf@ntickets.com.br | 5511958523443 |
		