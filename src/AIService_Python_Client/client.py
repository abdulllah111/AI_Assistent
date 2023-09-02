import pika
import gpt

connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()
channel.queue_declare(queue='request_queue')


def callback(ch, method, properties, body):
    print(" [x] Received %r" % body)
    
    # обработать запрос
    
    response = gpt.Generate(body)
    
    
    ch.basic_publish(exchange='',
                     routing_key='response_queue',
                     properties=pika.BasicProperties(
                         correlation_id=properties.correlation_id),
                     body=str(response))
    ch.basic_ack(delivery_tag=method.delivery_tag)

channel.basic_consume(queue='request_queue',
                      on_message_callback=callback)

print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()