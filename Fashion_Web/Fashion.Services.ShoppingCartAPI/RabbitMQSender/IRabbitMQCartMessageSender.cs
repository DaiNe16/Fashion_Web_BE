﻿namespace Fashion.Services.ShoppingCartAPI.RabbitMQSender
{
	public interface IRabbitMQCartMessageSender
	{
		void SendMessage(string message, string queueName);
	}
}