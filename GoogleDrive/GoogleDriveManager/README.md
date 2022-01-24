quick start
https://developers.google.com/drive/api/v3/quickstart/dotnet

# v3導入フロー

### 準備

1. google cloud にprojectを作る.
 https://developers.google.com/workspace/guides/create-project

1. [Google CloudConsole](https://console.cloud.google.com/)を開きます。
2. 左上の[メニュー]をクリックします menu > IAMと管理者 > プロジェクトを作成します。
3. [プロジェクト名]フィールドに、プロジェクトのわかりやすい名前を入力します。
オプション：プロジェクトIDを編集するには、[編集]をクリックします。プロジェクトの作成後にプロジェクトIDを変更することはできないため、プロジェクトの存続期間中のニーズを満たすIDを選択してください。

![](/picture/create_project_1.png)

4. [場所]フィールドで、[参照]をクリックして、プロジェクトの潜在的な場所を表示します。次に、[選択]をクリックします。

5. [作成]をクリックします。コンソールはダッシュボードページに移動し、プロジェクトは数分以内に作成されます。

![](/picture/create_project_2.png)


### GoogleWorkspaceAPIを有効にする

Google Drive APIを有効にする。

1. [Google CloudConsole](https://console.cloud.google.com/)を開きます。
2. 左上の[メニュー]をクリック menu > APIとサービス > ライブラリ

![](/picture/valid_api_1.png)

3. スクロールし`Google Drive API`を選択。有効にする。

![](/picture/valid_api_2.png)

### クレデンシャルの用意

https://developers.google.com/workspace/guides/create-credentials


1. [Google CloudConsole](https://console.cloud.google.com/)を開きます。
2. 左上の[メニュー]をクリックします menu > APIとサービス > 認証情報
![](/picture/create_credential_1.png)

`API`/`OAuth`/`サービスアカウント`要件に合わせクレデンシャルを発行する。

![](/picture/create_credential_2.png)

※今回は`OAuth`で進める。

1. 同意画面を設定.

![](/picture/create_credential_3.png)

2. 公開範囲の指定 (サンプルでは組織に属していないため外部)


![](/picture/create_credential_4.png)

3. 必要情報を記載

![](/picture/create_credential_5.png)

![](/picture/create_credential_5_1.png)

4. スコープ/テストユーザー に関しては必須入力項目がないため割愛。
必要に応じて設定してください。

5. OAuthの同意を通したところで認証情報の作成に入る。

![](/picture/create_credential_6.png)

6. アプリケーションの指定。

![](/picture/create_credential_7.png)

※ここではデスクトップアプリを選択

![](/picture/create_credential_8.png)

7. 認証情報の作成完了画面
ここのjsonがクレデンシャルファイル。
※credentials.jsonあたりにリネームしておくのがおススメ。

![](/picture/create_credential_9.png)

### C#を使ったCLIアプリ準備

1. Visiual Studioを使ってC# CLIアプリケーションプロジェクトを作る。

![](/picture/cs_cli_app_1.png)

2. Consoleにてnupkgをインストールする。

<pre>
PM> Install-Package Google.Apis.Drive.v3
</pre>

3. クレデンシャルファイルをソリューションに登録する。

![](/picture/cs_cli_app_2.png)

4. クレデンシャルファイルのPropertyを書き換える

![](/picture/cs_cli_app_3.png)

5. 下記サンプルコードを実行して動作確認。
tokenが無い初回はGoogleアカウントでログインを求められる。 ＞ ログインする。
→ アクセスtoken作ったりする

以上導入終わり。

```cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveQuickstart
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();

        }
    }
}
```
3～5に関してはプロジェクトに合わせてfix


なにか問題あれば以下参考。
https://developers.google.com/drive/api/v3/quickstart/dotnet#prerequisites