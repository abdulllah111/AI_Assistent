from httpx import HTTPError
import g4f
import json

from g4f.Provider import (
    ChatBase,
)

from g4f.models import (
    gpt_35_turbo,
    gpt_35_long
)

def read_or_create_chats_json():
    try:
        with open("chats.json", "r") as f:
            json_data = f.read()
            chats = json.loads(json_data)
    except FileNotFoundError:
        chats = {}

    return chats

chats = read_or_create_chats_json()

# 1087968824

def Generate(context):

    response = 'Default'

    try:
        response = g4f.ChatCompletion.create(
            # model="gpt-3.5-turbo",
            model=gpt_35_turbo,
            messages=context,
            # provider=RetryProvider
        )

    except Exception as e:
        if str(e).startswith('500'):
            print('Request error')
            # return
            response = "Error 500....  \nИсключение: " + str(e)
        elif str(e).startswith('400'):
            print('Request error')
            # return
            response = "Error 400  \nИсключение: " + str(e)
        else:
            print('Request error')
            # return
            response = "Error...  \nИсключение: " + str(e)
    print(response)
    return response




from httpx import HTTPError
import g4f
import json


from g4f.models import (
    gpt_35_turbo,
    gpt_35_long
)

def read_or_create_chats_json():
    try:
        with open("chats.json", "r") as f:
            json_data = f.read()
            chats = json.loads(json_data)
    except FileNotFoundError:
        chats = {}

    return chats

chats = read_or_create_chats_json()

# 1087968824

def Generate(message, userId):
    
    context = [] 

    if userId not in chats:
        context.append({"role": "system", "content": "Ты являешься исскуственным интелектом по имене Jarvis"})
        chats[userId] = context
    
    context = chats[userId]
    context.append({"role": "user", "content": message})  
    

    
    response = 'Default'
    try:
        response = g4f.ChatCompletion.create(
            # model="gpt-3.5-turbo",
            model=gpt_35_turbo,
            messages=context,
            # provider=RetryProvider
        )
        
        context.append({"role": "assistant", "content": response})
        chats[userId] = context
        
        json_data = json.dumps(chats)
        with open("chats.json", "w") as f:
            f.write(json_data)

    except Exception as e:
        if str(e).startswith('500'):
            print('Request error')
            # return
            response = "Error 500....  \nИсключение: " + str(e)
        elif str(e).startswith('400'):
            print('Request error')
            # return
            response = "Error 400  \nИсключение: " + str(e)
        else:
            print('Request error')
            # return
            response = "Error...  \nИсключение: " + str(e)
    print(response)
    return response