using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Bot_Config
{
    public class VkConfig
    {
        // https://vk.com/dev/bots
        // https://vk.com/dev/bots_docs - основа

        // Создать сообщество
        // Включаем бота https://vk.com/club202670669?act=messages&tab=bots
        //
        // 1.1. Получение ключа доступа Long Poll
        // Откройте раздел «Управление сообществом» 
        // («Управление страницей», если у Вас публичная страница), 
        // выберите вкладку «Работа с API» и нажмите «Создать ключ доступа».
        // https://vk.com/club202670669?act=longpoll_api
        //
        // Получение groupId https://vk.com/dev/groups.getById
        //

        private static readonly VkConfig config;

        static VkConfig()
        {
            config = new VkConfig();
        }
        public string AccessToken => accessToken;

        public static VkConfig Instance => config;
        public ulong GroupId => groupId;

        private VkConfig()
        {
            this.accessToken = "b4bed09094e4bc06b2b9c17f637c2283f88c876365cc98618accd5fbb21fd00bd45f050d77b6b6fc92213";
            this.groupId = 209772567;

            #region other

            //this.login = "";
            //this.password = "";
            //this.appId = "";

            #endregion
        }

        string accessToken;
        ulong groupId;

        #region other

        //public string Login => login;
        //public string Password => password; 
        //public ulong AppId  => appId;
        //ulong appId;
        //string login;
        //string password;

        #endregion
    }
}
