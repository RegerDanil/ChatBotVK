using System;
using System.Collections.Generic;
using VkNet;
using VkNet.Model;
using Chat_Bot_Config;
using VkNet.Model.GroupUpdate;
using VkNet.Enums.Filters;
using System.Threading;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using VkNet.Enums.SafetyEnums;

namespace Vk_Bot
{
    class Bot
    {
        private VkApi vkClient;

        private LongPollServerResponse longPollServerResponse;

        private VkConfig config;

        string currentTs;

        public event Action<GroupUpdate> OnMessage;

        public Bot(VkConfig config, bool showLog = false)
        {
            this.config = config;
            vkClient = new VkApi();
            vkClient.Authorize(new ApiAuthParams
            {
                AccessToken = config.AccessToken,
                Settings = Settings.All | Settings.Messages
            });

            longPollServerResponse = vkClient.Groups.GetLongPollServer(config.GroupId);
            currentTs = longPollServerResponse.Ts;
            if (showLog) ViewLog();

        }
        private void ViewLog()
        {

            Console.WriteLine($"longPollServerResponse.Key = {longPollServerResponse.Key}");
            Console.WriteLine($"longPollServerResponse.Pts = {longPollServerResponse.Pts}");

            Console.WriteLine($"longPollServerResponse.Ts = {longPollServerResponse.Ts}");
            Console.WriteLine($"longPollServerResponse.Server = {longPollServerResponse.Server}");
        }
        public void Start(Action<GroupUpdate> onMessage = null)
        {
            if (onMessage != null) OnMessage += OnMessage;
            else OnMessage += DefaultOnMessage;
            //else OnMessage += DefaultOnMessageKeyboard;

            new Thread(OnUpdate).Start();
        }
        public User GetUserInfo(long id)
        {
            #region users.get
            //var users = vkClient.Users.Get(new long[] { 560682090 });
            //users = vkClient.Users.Get(
            //    userIds: new long[] { 560682090 },
            //    fields: ProfileFields.FirstName,
            //    nameCase: NameCase.Acc

            //    );
            //foreach (var user in users)
            //{
            //    Console.WriteLine(user.FirstName);
            //    Console.WriteLine(user.LastName);
            //}
            #endregion

            return vkClient.Users.Get(new long[] { id })[0];
        }
        private void DefaultOnMessage(GroupUpdate e)
        {
            Console.WriteLine();
            Console.WriteLine(
                String.Format(
                    "Type: {0}\n Text: {1}\n " +
                    "Body: {2}\n FromId: {3}\n " +
                    "UserId: {4}\n FirstName: {5}\n ",
                    e.Type,
                    e?.Message?.Text,
                    e?.Message?.Body,
                    e?.Message?.FromId,
                    e?.Message?.UserId,
                    this.GetUserInfo(e.Message.FromId.Value).FirstName
                ));

            var ans = e?.Message?.Text switch
            {
                "Контакты приемной комиссии" => "Приемная комиссия - консультации по вопросам поступления:\n" +
                "Адрес: 644080, Омск, пр.Мира 5, корпус 1, ауд.1.139\n" +
                "Телефон: (3812) 65 - 98 - 81, 65 - 99 - 88\n" +
                "E - mail: priem_kom @sibadi.org\n" +
                "Skype: sibadi_priemnaya_komissiya\n",

                "Факультеты" => "Автомобильные дороги и мосты\n" +
                "Автомобильный транспорт\n" +
                "Заочный факультет\n" +
                "Институт магистратуры и аспирантуры\n" +
                "Информационные системы в управлении\n" +
                "Нефтегазовая и строительная техника\n" +
                "Промышленное и гражданское строительство\n" +
                "Экономика и управление\n",

                "Направления подготовки" => "СибАДИ подготавливает специалистов по следующим направлениям:\n" +
                "- Наземные транспортные системы\n" +
                "- Информационная безопасность\n" +
                "- Прикладная информатика\n" +
                "- Энергетическое машиностроение\n" +
                "Полный список можно посмотреть по ссылке:\n" +
                "https://sibadi.org/directions/9243/90years/tour/3d/90years/tour/3d/3d_tour_sibadi.html \n",

                "Военный учебный центр" => "Военный учебный центр при ФГБОУ ВО «СибАДИ» производит обучение граждан по программам подготовки офицеров," +
                " сержантов и солдат запаса из числа студентов университета очной формы обучения," +
                " граждан Российской Федерации в возрасте до 30 лет, не имеющих судимости.\n" +
                "Продолжительность обучения составляет:\n" +
                "офицеров запаса – 5 семестров;\n" +
                "сержантов запаса – 4 семестра;\n" +
                "солдат запаса – 3 семестра.\n",

                "Общежитие" => "Студенческое общежитие предназначено для временного проживания и размещения:\n" +
                "- на период обучения иногородних студентов, аспирантов, докторантов, ординаторов, интернов, обучающихся по очной форме обучения;\n" +
                "- на период сдачи экзаменов и выполнения работ по диссертации аспирантов, докторантов, обучающихся по заочной форме обучения;\n" +
                "- абитуриентов на период прохождения вступительных испытаний.",

                "!time" => DateTime.Now.ToString(),
                _ => $"Получено"
            };

            this.SendMessage(
                userId: e.Message.FromId,
                text: ans,
                replyToMessageId: e?.Message?.Id
                );
        }

        public void SendMessage(long? userId, string text,
                                long? replyToMessageId = -1,
                                MessageKeyboard keyboard = null)
        {
            var msg = new MessagesSendParams()
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                UserId = userId,
                Message = text
            };
            if (replyToMessageId != -1) msg.ReplyTo = replyToMessageId;
            if (keyboard != null) msg.Keyboard = keyboard;

            vkClient.Messages.Send(msg);
        }



        private void DefaultOnMessageKeyboard(GroupUpdate e)
        {
            bool showKeyboard = e?.Message?.Text switch
            {
                "кнопочки" => true,
                _ => false
            };


            this.SendMessage(
                userId: e.Message.FromId,
                text: "Keyboard",
                replyToMessageId: e?.Message?.Id,
                keyboard: showKeyboard switch
                {
                    true => new MessageKeyboard()
                    {

                        Inline = false,
                        OneTime = false,
                        Buttons = new List<List<MessageKeyboardButton>>()
                                {
                                    new List<MessageKeyboardButton>(){
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Text,
                                                Label="Кнопка 1.1"
                                            }
                                        },
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Text,
                                                 Label="Кнопка 1.2"
                                            }
                                        }

                                    },
                                    new List<MessageKeyboardButton>(){
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Text,
                                                Label="Кнопка 2.1"
                                            }
                                        },
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Text,
                                                 Label="Кнопка 2.2"
                                            }
                                        },
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Text,
                                                 Label="Кнопка 2.3"
                                            }
                                        }

                                    },
                                    new List<MessageKeyboardButton>(){
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.IntentSubscribe,
                                                Label="Кнопка 3.1",
                                                PeerId =  560682090,
                                                Intent = Intent.NonPromoNewsLetter
                                            }
                                        }
                                    },
                                    new List<MessageKeyboardButton>(){
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.OpenLink,
                                                Label="link",
                                                Link = new Uri("http://ksergey.ru/profcsharp/")
                                            }
                                        }
                                    },
                                    new List<MessageKeyboardButton>(){
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Location,
                                            }
                                        }
                                    },
                                    new List<MessageKeyboardButton>(){
                                        new MessageKeyboardButton() {
                                            Action = new MessageKeyboardButtonAction()
                                            {
                                                Type =  KeyboardButtonActionType.Callback,
                                                Label="Скрыть кнопочки"
                                            }
                                        }
                                    }
                                },


                    },
                    _ => new MessageKeyboard() { Buttons = new List<List<MessageKeyboardButton>>() }
                }
                );
        }


        public void OnUpdate()
        {
            while (true)
            {

                var res = vkClient.Groups.GetBotsLongPollHistory(
                    new BotsLongPollHistoryParams()
                    {
                        Key = longPollServerResponse.Key,
                        Ts = currentTs,
                        Server = longPollServerResponse.Server
                    });
                if (OnMessage != null)
                {
                    foreach (GroupUpdate item in res.Updates)
                    {

                        currentTs = res.Ts;

                        if (item?.Message?.RandomId != 0
                        || item.Type == GroupUpdateType.MessageReply
                        ) { continue; }

                        OnMessage?.Invoke(item);
                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}
