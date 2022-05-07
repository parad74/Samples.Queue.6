using Monitor.Service.Model.Settings;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.Domain.Configuration;

public interface IRabbitOptions
{
	string HostName { get; set; }

	string UserName { get; set; }             //C:\Count4U\trunk\Count4U\Count4U.Model

	string Password { get; set; }

}
public class RabbitOptions : IValidatable, IRabbitOptions
{
	[Required]
	public string HostName { get; set; }
	[Required]
	public string UserName { get; set; }
	[Required]
	public string Password { get; set; }

	public void Validate()
	{
		Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);

		if (string.IsNullOrWhiteSpace(HostName) == true)
		{
			throw new ArgumentException("HostName can't be empty");
		}
		if (string.IsNullOrWhiteSpace(UserName) == true)
		{
			throw new ArgumentException("UserName can't be empty");
		}
		if (string.IsNullOrWhiteSpace(Password) == true)
		{
			throw new ArgumentException("Password can't be empty");
		}
		//if (this.UseAppDataPath == true && this.UseProgramDataAppDataPath == true)
		//{
		//	throw new ArgumentException("UseAppDataPath and UseProgramDataAppDataPath cann't be both equal true in the same Count4USettings");
		//}

		//if (this.UseAppDataPath == true)
		//{
		//	if (System.IO.Directory.Exists(this.AppDataPath) == false)
		//	{
		//		throw new DirectoryNotFoundException(this.AppDataPath);
		//	}
		//}

		//if (this.UseProgramDataAppDataPath == true)
		//{
		//	if (System.IO.Directory.Exists(this.ProgramDataAppDataPath) == false)
		//	{
		//		throw new DirectoryNotFoundException(this.ProgramDataAppDataPath);
		//	}
		//}
	}
}