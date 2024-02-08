using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Fashion.Services.EmailAPI.Models.Dto;
using Fashion.Services.EmailAPI.Service.IService;

namespace Fashion.Services.EmailAPI.Messaging
{
	public class RabbitMQConsumer : BackgroundService
	{
		private IConnection _connection;
		private IModel _channel;
		private const string ExchangeName = "PublishSubcribePaymentUpdate_Exchange";
		private string queueName = "";
		private IAuthService _authService;
		public RabbitMQConsumer(IAuthService authService)
		{
			_authService = authService;
			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				UserName = "guest",
				Password = "guest",
			};
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			//Queue
			//_channel.QueueDeclare("checkoutqueue", false, false, false, arguments: null);
			//Fanout
			_channel.ExchangeDeclare(exchange: ExchangeName, ExchangeType.Fanout);
			queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queueName, ExchangeName, "");
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += (ch, e) =>
			{
				var content = Encoding.UTF8.GetString(e.Body.ToArray());
				HandleMessage(content).GetAwaiter().GetResult();
				_channel.BasicAck(e.DeliveryTag, false);
			};
			//Queue
			//_channel.BasicConsume("checkoutqueue", false, consumer);
			//Fanout
			_channel.BasicConsume(queueName, false, consumer);

			return Task.CompletedTask;
		}

		private async Task HandleMessage(string content)
		{
			try
			{
				string[] contentSplit = content.Split("@123@");
				ApplicationUserDto applicationUserDto = await _authService.GetUserById(contentSplit[0]);
				string email = applicationUserDto.Email;

				//Step 1: Check regex email
				// Regular expression pattern for validating an email address
				string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

				// Create a Regex object with the pattern
				Regex regex = new Regex(pattern);

				if (regex.IsMatch(email))
				{
					SendSmtp("tavandai2002@gmail.com", "nwanhjvkhffarllu", email, "Notification from Fashion_Web", contentSplit[1]);
				}
			}
			catch 
			{ }
		}

		private bool SendSmtp(string emailFrom, string passwordFrom, string emailTo, string subject, string content)
		{
			// Sender's email credentials
			string senderEmail = emailFrom;
			string senderPassword = passwordFrom;

			// Recipient email address
			string recipientEmail = emailTo;

			// Create a MailMessage object
			MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail);
			mailMessage.Subject = subject;
			mailMessage.Body = content;

			// Create a SmtpClient object
			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
			smtpClient.Port = 587;
			smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
			smtpClient.EnableSsl = true;

			try
			{
				// Send the email
				smtpClient.Send(mailMessage);
				Console.WriteLine("Email sent successfully!");
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending email: {ex.Message}");
			}
			return false;
		}
	}
}
