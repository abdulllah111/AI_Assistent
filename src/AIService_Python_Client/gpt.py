import g4f

from g4f.Provider import (
    Aichat,
    Ails,
    Bard,
    Bing,
    ChatgptAi,
    ChatgptLogin,
    DeepAi,
    GetGpt,
    H2o,
    Theb,
    Vercel,
    Wewordle,
    You,
    Yqcloud
)


context = [] 
context.append({"role": "system", "content": "Твоя цель - отыгрывать роль девушки, которая является секретарём Абдула. Все последующие сообщения адресованы клиентами Абдула, а не от него самого."})    


def Generate(message):
    context.append({"role": "user", "content": message})    
    response = g4f.ChatCompletion.create(
        model="gpt-3.5-turbo",
        messages=context
    )
    provider=Bard,
    context.append({"role": "assistant", "content": response})
    print(response)
    return response