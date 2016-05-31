using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Ktnc.SendGrid.Services
{
    /// <summary>
    /// メール送信サービス
    /// </summary>
    public static class SendMailService
    {
        private static string apiKey = ConfigurationManager.AppSettings["Ktnc.SendGrid.ApiKey"];
        private static string From = ConfigurationManager.AppSettings["Ktnc.SendGrid.From"];
        private static string FromName = ConfigurationManager.AppSettings["Ktnc.SendGrid.FromName"];

        /// <summary>
        /// メール形式
        /// </summary>
        public enum Format
        {
            /// <summary>
            /// HTML形式
            /// </summary>
            Html,

            /// <summary>
            /// プレーン形式
            /// </summary>
            Plane,

            /// <summary>
            /// HTML、プレーン形式どちらも
            /// </summary>
            Both
        };

        /// <summary>
        /// テンプレートを利用したメール送信
        /// </summary>
        /// <param name="tos">To</param>
        /// <param name="subject">件名(Web.configのキー)</param>
        /// <param name="body">本文(Web.configのキー)</param>
        /// <param name="map">マップ定義</param>
        /// <param name="from">From</param>
        /// <param name="fromName">From名</param>
        /// <param name="format">メール形式</param>
        public static async Task SendMailWithTemplate(
            List<string> tos,
            string subject,
            string body,
            Dictionary<string, object> map,
            string from = null,
            string fromName = null,
            Format format = Format.Both)
        {
            var subjectStr = ConfigurationManager.AppSettings[subject];
            var bodyStr = ConfigurationManager.AppSettings[body];

            if (string.IsNullOrEmpty(subjectStr))
            {
                throw new Exception(string.Format("Web.config(またはApp.config)に key: {0} を指定してください。", subject));
            }
            if (string.IsNullOrEmpty(bodyStr))
            {
                throw new Exception(string.Format("Web.config(またはApp.config)に key: {0} を指定してください。", body));
            }

            var email = new SendGridMessage();
            email.AddTo(from ?? From);
            email.Header.SetTo(tos);
            email.From = new MailAddress(from ?? From, fromName ?? FromName);
            email.Subject = TemplateService.MargeMessage(subjectStr, map);
            if (format == Format.Html)
            {
                email.Html = TemplateService.MargeMessageWithTemplate(string.Format("{0}-html.vm", bodyStr), map);
            }
            else if (format == Format.Plane)
            {
                email.Text = TemplateService.MargeMessageWithTemplate(string.Format("{0}.vm", bodyStr), map);
            }
            else
            {
                email.Html = TemplateService.MargeMessageWithTemplate(string.Format("{0}-html.vm", bodyStr), map);
                email.Text = TemplateService.MargeMessageWithTemplate(string.Format("{0}.vm", bodyStr), map);
            }

            var web = new Web(apiKey);
            await web.DeliverAsync(email);
        }
    }
}