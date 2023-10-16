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


# 1087968824

def Generate(context):

    
    response = 'Default'

    try:
        response = g4f.ChatCompletion.create(
            # model="gpt-3.5-turbo",
            model = gpt_35_turbo,
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