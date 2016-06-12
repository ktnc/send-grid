# 概要
AzureStorage上に置かれたテンプレートファイルを使ってメールを送信します。メール送信はSendGridサービスを利用します。テンプレートはVelocityで記述することができます。

![コンポーネント図](https://raw.githubusercontent.com/ktnc/send-grid/develop/doc/ktnc.send-grid.component.png)

Velocityの使い方は[オフィシャルサイト](http://velocity.apache.org/engine/devel/user-guide.html)を参照してください。

# インストール
パッケージマネージャから下記のコマンドでインストールできます。

```
PM> Install-Package Ktnc.SendGrid
```

# 設定
* AzureStorageの接続設定
* SendGridの設定

## AzureStorageの接続設定とSendGridの設定
「App.config(またはWeb.config)」の「appSettins」にAzureStorageの接続先設定とSendGridの設定を追加してください。

### App.config(またはWeb.config)
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

# 使い方
* テンプレートの追加
* 件名とテンプレート名の設定
* コード呼び出し

## テンプレートの追加
AzureStorageのコンテナにテンプレートファイルを追加します。テンプレートは「Plane」と「HTML」を用意します。  
「test」という名前のテンプレートを追加する場合は「test.vm」と「test-html.vm」をコンテナに追加します。

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

## 件名とテンプレート名の設定
App.config(またはWeb.config)に件名とテンプレート名を追加します。  
下記のコードはAzureStorageのコンテナにある「test.vm」と「test-html.vm」というテンプレートを利用する例です。

### App.config(またはWeb.config)
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

件名もVelocityで記述することができます。

## コード呼び出し
下記のサンプルのように利用します。

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

「test.vm」および「test-html.vm」を利用してメールが送信されます。

# その他

## フォーマット指定
メール送信時のフォーマット(PlaneまたはHtml)を指定できます。デフォルトは「Both」でPlaneおよびHTMLメール両方を送信します。

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

## 非同期呼び出し
下記のサンプルコード参照。

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
