using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ktnc.SendGrid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ktnc.SendGrid.Services.Tests
{
    [TestClass()]
    public class SendMailServiceTests
    {
        [TestMethod()]
        public void SendMailWithTemplateTest()
        {
            SendMailService.SendMailWithTemplate(
                tos: new List<string> { "yasuhiro.sawabe@basic.co.jp" },
                subject: "Subject",
                body: "Body",
                map: new Dictionary<string, object>
                {
                    { "name", "テスト" },
                    { "message", "こんにちは" }
                }).Wait();
        }

        [TestMethod()]
        public void SendMailWithTemplateTest1()
        {
            try
            {
                SendMailService.SendMailWithTemplate(
                    tos: new List<string> { "yasuhiro.sawabe@basic.co.jp" },
                    subject: "Subject2",
                    body: "Body2",
                    map: new Dictionary<string, object>
                    {
                        { "name", "テスト" },
                        { "message", "こんにちは" }
                    }).Wait();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Web.config(またはApp.config)に key: Subject2 を指定してください。", e.InnerException.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod()]
        public void SendMailWithTemplateTest2()
        {
            try
            {
                SendMailService.SendMailWithTemplate(
                    tos: new List<string> { "yasuhiro.sawabe@basic.co.jp" },
                    subject: "Subject",
                    body: "Body2",
                    map: new Dictionary<string, object>
                    {
                        { "name", "テスト" },
                        { "message", "こんにちは" }
                    }).Wait();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Web.config(またはApp.config)に key: Body2 を指定してください。", e.InnerException.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod()]
        public void SendMailWithTemplateTest3()
        {
            SendMailService.SendMailWithTemplate(
                format: SendMailService.Format.Html,
                tos: new List<string> { "yasuhiro.sawabe@basic.co.jp" },
                subject: "Subject",
                body: "Body",
                map: new Dictionary<string, object>
                {
                    { "name", "テスト" },
                    { "message", "HTMLメールのみ送りました" }
                }).Wait();
        }

        [TestMethod()]
        public void SendMailWithTemplateTest4()
        {
            SendMailService.SendMailWithTemplate(
                format: SendMailService.Format.Plane,
                tos: new List<string> { "yasuhiro.sawabe@basic.co.jp" },
                subject: "Subject",
                body: "Body",
                map: new Dictionary<string, object>
                {
                    { "name", "テスト" },
                    { "message", "プレーンメールのみ送りました" }
                }).Wait();
        }
    }
}