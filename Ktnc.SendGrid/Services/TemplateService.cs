using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Ktnc.SendGrid.Services
{
    public static class TemplateService
    {
        private static string AccountName = ConfigurationManager.AppSettings["Ktnc.SendGrid.Storage.AccountName"];
        private static string AccessKey = ConfigurationManager.AppSettings["Ktnc.SendGrid.Storage.AccessKey"];
        private static string Container = ConfigurationManager.AppSettings["Ktnc.SendGrid.Storage.Container"];

        public static string MargeMessage(string templateStr, Dictionary<string, object> map)
        {
            Velocity.Init();
            var sw = new StringWriter();

            VelocityContext context = new VelocityContext();
            foreach (var val in map)
            {
                context.Put(val.Key, val.Value);
            }

            var tmp = newTemplate(templateStr);
            tmp.Merge(new VelocityContext(context), sw);
            return sw.ToString();
        }

        public static string MargeMessageWithTemplate(string template, Dictionary<string, object> map)
        {
            return MargeMessage(GetTemplateFromStorageBlob(template), map);
        }

        private static string GetTemplateFromStorageBlob(string filePath)
        {
            var accountAndKey = new StorageCredentials(AccountName, AccessKey);
            var account = new CloudStorageAccount(accountAndKey, true); // HTTPS を利用する場合は true

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(Container);
            if (!container.Exists())
            {
                throw new Exception(string.Format("コンテナ「{0}」が存在しません", Container));
            }

            CloudBlob blob = container.GetBlobReference(Path.GetFileName(filePath));
            if (!blob.Exists())
            {
                throw new Exception(string.Format("ファイル「{0}」が存在しません", filePath));
            }

            long fileByteLength = blob.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            blob.DownloadToByteArray(fileContent, 0);
            return Encoding.UTF8.GetString(fileContent);
        }

        private static Template newTemplate(String templateString)
        {
            var runtimeServices = RuntimeSingleton.RuntimeServices;
            StringReader reader = new StringReader(templateString);
            SimpleNode node;
            try
            {
                node = runtimeServices.Parse(reader, "Template name");
            }
            catch (ParseException e)
            {
                throw new Exception("パースエラー", e);
            }

            Template template = new Template();
            template.RuntimeServices = runtimeServices;
            template.Data = node;
            template.InitDocument();

            return template;
        }
    }
}