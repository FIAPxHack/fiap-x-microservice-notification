# language: pt
Funcionalidade: Envio de Notificações
  Como um sistema
  Eu quero enviar não notificações por email
  Para manter os usuários informados sobre eventos importantes

  Cenário: Enviar notificação com sucesso
    Dado que tenho um usuário com ID "user123" e email "test@example.com"
    Quando eu envio uma notificação com mensagem "Seu vídeo está sendo processado"
    Então a notificação deve ser enviada com sucesso
    E o status deve ser "Sent"

  Cenário: Enviar notificação de processamento iniciado
    Dado que tenho um usuário com ID "user456" e email "user@example.com"
    Quando eu envio uma notificação do tipo "VideoProcessingStarted"
    Então a notificação deve ser enviada com sucesso
    E o assunto do email deve conter "processamento"

  Cenário: Enviar notificação de processamento concluído
    Dado que tenho um usuário com ID "user789" e email "completed@example.com"
    Quando eu envio uma notificação do tipo "VideoProcessingCompleted"
    Então a notificação deve ser enviada com sucesso
    E o status deve ser "Sent"

  Cenário: Buscar histórico de notificações vazio
    Dado que tenho um usuário sem notificações com ID "new-user"
    Quando eu busco o histórico de notificações
    Então deve retornar uma lista vazia

  Cenário: Buscar histórico de notificações com múltiplos registros
    Dado que tenho um usuário com ID "multi-user" e email "multi@example.com"
    E que foram enviadas 3 notificações para este usuário
    Quando eu busco o histórico de notificações
    Então deve retornar 3 notificações
    E todas devem pertencer ao usuário "multi-user"

  Esquema do Cenário: Enviar diferentes tipos de notificação
    Dado que tenho um usuário com ID "<userId>" e email "<email>"
    Quando eu envio uma notificação do tipo "<tipo>"
    Então a notificação deve ser enviada com sucesso
    E o tipo deve ser "<tipo>"

    Exemplos:
      | userId  | email             | tipo                        |
      | user1   | user1@test.com    | VideoProcessingStarted      |
      | user2   | user2@test.com    | VideoProcessingCompleted    |
      | user3   | user3@test.com    | VideoProcessingFailed       |
      | user4   | user4@test.com    | General                     |

  Cenário: Falha no envio de notificação
    Dado que o serviço de email está indisponível
    E tenho um usuário com ID "user999" e email "fail@example.com"
    Quando eu tento enviar uma notificação
    Então a notificação não deve ser enviada
    E o status deve ser "Failed"

  Cenário: Notificação com mensagem longa
    Dado que tenho um usuário com ID "long-user" e email "long@example.com"
    Quando eu envio uma notificação com mensagem de 1000 caracteres
    Então a notificação deve ser enviada com sucesso
    E a mensagem deve estar completa

  Cenário: Buscar notificações filtrando por status
    Dado que tenho um usuário com ID "status-user" e email "status@example.com"
    E que foram enviadas notificações com diferentes status
    Quando eu busco o histórico de notificações
    Então deve retornar notificações com status "Sent", "Failed" e "Pending"
