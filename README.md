# Overview
Send E-Mail using the template file that is placed on the AzureStorage. E-Mail sent will use the SendGrid service. The template can be written in Velocity.

How to use Velocity is please refer to the [official site](http://velocity.apache.org/engine/devel/user-guide.html).

# Install
To install Ktnc.SendGrid, run the following command in the Package Manager Console.

```
PM> Install-Package Ktnc.SendGrid
```

# Settings
* AzureStorage Setting
* SendGrid Setting

## AzureStorage Setting and SendGrid Setting
Add the settings that AzureStorage Setting and SendGrid Setting to "appSettins" in "App.config (or Web.config)".

### App.config (or Web.config)
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <!-- Setting TemplateService -->
    <add key="Ktnc.SendGrid.Storage.AccountName" value="--SetAccountName--" />
    <add key="Ktnc.SendGrid.Storage.AccessKey" value="--SetAccessKey--" />
    <add key="Ktnc.SendGrid.Storage.Container" value="--SetContainer--" />

    <!-- Setting SendMailService -->
    <add key="Ktnc.SendGrid.ApiKey" value="--SetApiKey--" />
    <add key="Ktnc.SendGrid.From" value="--SetDefaultFrom(MailAddress)--" />
    <add key="Ktnc.SendGrid.FromName" value="--SetDefaultFromName--" />
    ...
  </appSettings>
  ...
</configuration>
```

# Usage
* Additional Template
* Settings Subject and TemplateName
* Call of SendMail API

## Additional Template
Add the template file to AzureStorage of container. Please prepare templates that are "HTML" and "Plane".
Add "test.vm" and "test-html.vm" to the container if you want to add a template named "test".

### test.vm
```
Dear $name
#if($message)
message: $message
#end
Thank you for reading.
```

### test-html.vm
```html
<div>
  <h1>Dear $name</h1>
#if($message)
  <p>message: $message</p>
#end
  <p>Thank you for reading.</p>
<div>
```

## Settings Subject and TemplateName
Add the subject and the template name in the App.config (or Web.config).
The following code is an example of using templates that are "test.vm" and "test-html.vm" in the container of AzureStorage.

### App.config (or Web.config)
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    ...
    <add key="TestSubject" value="Dear $name, $message" />
    <add key="TestBody" value="test" />
  </appSettings>
  ...
</configuration>
```

Subject can also be written in Velocity.

## Call of SendMail API
Do the sample code below in reference.

```csharp
public void Sample() {
  Ktnc.SendGrid.Services.SendMailService.SendMailWithTemplate(
    tos: new List<string> { "to@sample.domain" },
    subject: "TestSubject",
    body: "TestBody",
    map: new Dictionary<string, object>
    {
      { "name", "Test" },
      { "message", "Hello!" }
    }
  ).Wait();
}
```

E-mail using the "test.vm" and "test-html.vm" will be sent.

# Others

## Format specified
You can specify the format (Plane or Html) at the time of e-mail transmission . The default is to send the Plane and HTML mail.

```csharp
public void HtmlMailSample() {
  Ktnc.SendGrid.Services.SendMailService.SendMailWithTemplate(
    format: SendMailService.Format.Html,
    tos: new List<string> { "to@sample.domain" },
    subject: "TestSubject",
    body: "TestBody",
    map: new Dictionary<string, object>
    {
      { "name", "Test" },
      { "message", "Hello!" }
    }
  ).Wait();
}
```

## Asynchronous call
Refer to the sample code below.

```csharp
public async void AsyncSample() {
  await Ktnc.SendGrid.Services.SendMailService.SendMailWithTemplate(
    tos: new List<string> { "to@sample.domain" },
    subject: "TestSubject",
    body: "TestBody",
    map: new Dictionary<string, object>
    {
      { "name", "Test" },
      { "message", "Hello!" }
    }
  );
}
```
