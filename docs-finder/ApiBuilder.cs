using System;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace docs_finder
{
    public static class ApiBuilder
    {
        private static (string, string) GetLoginPass()
        {
            DotNetEnv.Env.Load();
            var login = DotNetEnv.Env.GetString("LOGIN", String.Empty);
            var pass = DotNetEnv.Env.GetString("PASS", String.Empty);

            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(login)) return (login, pass);
            
            Console.WriteLine("Введите логин:");
            login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            pass = Console.ReadLine();

            return (login, pass);
        }
        
        public static VkApi GetApi()
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            var api = new VkApi(services);

            var (login, pass) = GetLoginPass();

            try
            {
                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 7602258,
                    Login = login,
                    Password = pass,
                    Settings = Settings.Messages | Settings.Documents,
                    TwoFactorAuthorization = () =>
                    {
                        Console.WriteLine("Введите код из sms:");
                        return Console.ReadLine();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("При авторизации что-то отвалилось. Попробуйте ещё раз. Проверьте логин и пароль.");
                Console.WriteLine(ex.Message);
            }

            return api;
        }
    }
}